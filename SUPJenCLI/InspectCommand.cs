using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Cli;
using SUPJenCLI.Model;
using SUPJenCLI.Model.Enum;

namespace SUPJenCLI
{
    internal class InspectCommand : AsyncCommand<InspectCommand.Settings>, IDisposable
    {
        private readonly HttpClient httpClient;

        public InspectCommand()
        {
            this.httpClient = new HttpClient
            {
                BaseAddress = new Uri(LoginSettings.URL),
            };

            this.httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", LoginSettings.GetBasicAuthorizationEncoded());
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            var originFolder = "/job/" + string.Join("/job/", settings.Folder.Split('/', StringSplitOptions.RemoveEmptyEntries));

            var inspect = await this.TryInspectPathAsync(originFolder);

            var type = CheckClassType(inspect.Content);

            while (type is ClassType.Folder)
            {
                string job = string.Empty;
                await using (inspect.Content)
                {
                    job = await ChooseJobAsync(inspect.Content);
                }

                inspect = await this.TryInspectPathAsync(job);

                if (inspect.ReturnCode > 0)
                {
                    return inspect.ReturnCode;
                }

                type = CheckClassType(inspect.Content);
            }

            while (type is ClassType.WorkflowJob)
            {
                string build = string.Empty;
                await using (inspect.Content)
                {
                    build = await ChooseBuildAsync(inspect.Content);
                }

                // promt operations
                await inspect.Content.DisposeAsync();
                inspect = await this.TryInspectPathAsync(build);

                if (inspect.ReturnCode > 0)
                {
                    return inspect.ReturnCode;
                }

                await using (inspect.Content)
                {
                    type = CheckClassType(inspect.Content);

                    if (type is ClassType.WorkflowRun)
                    {
                        await this.PerformeOperationAsync(build, inspect.Content);
                    }
                }
            }

            return 0;
        }

        public void Dispose()
        {
            this.httpClient.Dispose();
        }

        private static ClassType CheckClassType(Stream body)
        {
            body.Position = 0;

            var options = new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
            };

            using JsonDocument document = JsonDocument.Parse(body, options);

            document.RootElement.TryGetProperty("_class", out var classProperty);

            return MapClasses.Map[classProperty.GetString()];
        }

        private static async ValueTask<string> ChooseJobAsync(Stream body)
        {
            body.Position = 0;

            var folder = await JsonSerializer
                .DeserializeAsync<Folder>(
                    body,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

            body.Position = 0;

            var selectionPrompter = new SelectionPrompt<string>()
            {
                HighlightStyle = new Style(foreground: Color.Blue),
                Mode = SelectionMode.Leaf,
            };

            selectionPrompter
                .Title("[blue]Choose a job to inspect:[/]")
                .AddChoices(folder.Jobs.Select(b => b.Url.AbsolutePath));

            return AnsiConsole.Prompt(selectionPrompter);
        }

        private static async ValueTask<string> ChooseBuildAsync(Stream body)
        {
            body.Position = 0;

            var workflow = await JsonSerializer
                .DeserializeAsync<WorkflowJob>(
                    body,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

            body.Position = 0;

            var selectionPrompter = new SelectionPrompt<string>()
            {
                HighlightStyle = new Style(foreground: Color.Blue),
                Mode = SelectionMode.Leaf,
            };

            selectionPrompter
                .Title("[blue]Choose a build to inspect:[/]")
                .AddChoices(workflow.Builds.Select(b => b.Url.AbsolutePath));

            return AnsiConsole.Prompt(selectionPrompter);
        }

        private async ValueTask<PathInpesctResult> TryInspectPathAsync(string path)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{path.TrimEnd('/')}/api/json");

            using var response = await this.httpClient.SendAsync(request);

            var result = new PathInpesctResult(0, content: null);

            if (response.StatusCode >= HttpStatusCode.BadRequest)
            {
                AnsiConsole.Console.WriteLine(await response.Content.ReadAsStringAsync(), new Style(foreground: Color.Red1));
                result.ReturnCode = (int)response.StatusCode;
                return result;
            }

            result.Content = new MemoryStream((int)response.Content.Headers.ContentLength);
            await response.Content.CopyToAsync(result.Content);

            return result;
        }

        private async ValueTask PerformeOperationAsync(string buildUri, Stream body)
        {
            var operations = new[] { "Read Logs", "Exit" };

            body.Position = 0;

            var workflow = await JsonSerializer
                .DeserializeAsync<WorkflowRun>(
                    body,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

            body.Position = 0;

            if (workflow is null)
            {
                return;
            }

            AnsiConsole.MarkupLine($"[green]{workflow.FullDisplayName}[/]");

            var selectionPrompter = new SelectionPrompt<string>()
            {
                HighlightStyle = new Style(foreground: Color.Blue),
                Mode = SelectionMode.Leaf,
            };

            selectionPrompter.AddChoices(operations);

            var operation = AnsiConsole.Prompt(selectionPrompter);

            switch (operation)
            {
                case "Read Logs":
                    await this.PrintLogsAsync(buildUri, 0);
                    break;
            }
        }

        private async ValueTask PrintLogsAsync(string buildPath, long logIndex = 0)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"{buildPath.TrimEnd('/')}/logText/progressiveText?start={logIndex}");

            AnsiConsole.WriteLine(request.RequestUri.ToString());

            long index = 0;

            using (request)
            using (var response = await this.httpClient.SendAsync(request))
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return;
                }

                using (var streamReader = new StreamReader(await response.Content.ReadAsStreamAsync()))
                {
                    var length = response.Content.Headers.ContentLength;
                    var buffer = new char[10000];
                    for (int i = 0; i < length; i += 10000)
                    {
                        var totalRead = await streamReader.ReadBlockAsync(buffer, 0, 10000);
                        AnsiConsole.Write(buffer[0 .. (totalRead - 1)]);
                        await Task.Delay(10);
                    }
                }

                index = long.Parse(response.Headers.GetValues("X-Text-Size").FirstOrDefault() ?? "0");

                if (!response.Headers.TryGetValues("X-More-Data", out var xMoreDataHeader))
                {
                    return;
                }

                var moreData = xMoreDataHeader.FirstOrDefault() ?? "false";

                if (!moreData.Equals("true", StringComparison.OrdinalIgnoreCase) || index == 0)
                {
                    return;
                }
            }

            await this.PrintLogsAsync(buildPath, index);
        }

        internal struct PathInpesctResult
        {
            public PathInpesctResult(int returnCode, Stream content)
            {
                this.ReturnCode = returnCode;
                this.Content = content;
            }

            public int ReturnCode { get; set; }

            public Stream Content { get; set; }
        }

        internal class Settings : CommandSettings
        {
            [CommandArgument(0, "<folder>")]
            public string Folder { get; set; }
        }
    }
}
