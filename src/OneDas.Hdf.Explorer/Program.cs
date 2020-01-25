using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using OneDas.Database;
using OneDas.DataManagement.Extensibility;
using OneDas.Hdf.Explorer.Commands;
using OneDas.Hdf.Explorer.Core;
using OneDas.Hdf.Explorer.Database;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OneDas.Hdf.Explorer
{
    public class Program
    {
        #region Fields

        private static object _lock;
        private static HdfExplorerOptions _options;
        private static IConfiguration _configuration;
        private static Dictionary<IDatabase, IReadOnlyList<CampaignInfo>> _databaseToCampaignMap;
        private static Dictionary<CampaignInfo, CampaignMetaInfo> _campaignToCampaignMetaMap;
        private static ILoggerFactory _loggerFactory;

        #endregion

        #region Methods

        public static async Task<int> Main(string[] args)
        {
            _lock = new object();
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            // check interactivity
            var isUserInteractive = !args.Contains("--non-interactive");

            // configure logging
            var serviceProvider = new ServiceCollection().AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddFile(Path.Combine(Environment.CurrentDirectory, "SUPPORT", "LOGS", "HdfExplorer-{Date}.txt"), outputTemplate: OneDasConstants.FileLoggerTemplate);
            }).BuildServiceProvider();

            _loggerFactory = serviceProvider.GetService<ILoggerFactory>();

            // check operating mode
            if (args.Any() && !isUserInteractive)
            {
                if (!OneDasUtilities.ValidateDatabaseFolderPath(Environment.CurrentDirectory, out var message))
                {
                    Console.WriteLine(message);
                    return 1;
                }

                return await Program.ProcessCommand(args);
            }
            else
            {
                // load configuration
                var configurationDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OneDAS", "HDF Explorer");
                var configurationFileName = "settings.json";

                Directory.CreateDirectory(configurationDirectoryPath);

                var configurationBuilder = new ConfigurationBuilder();
                configurationBuilder.AddJsonFile(new PhysicalFileProvider(configurationDirectoryPath), path: configurationFileName, optional: true, reloadOnChange: true);

                _configuration = configurationBuilder.Build();
                _options = _configuration.Get<HdfExplorerOptions>();

                if (_options == null)
                {
                    _options = new HdfExplorerOptions();
                    _configuration.Bind(_options);
                }

                _options.Save(configurationDirectoryPath);

                // change current directory to database location
                var currentDirectory = Environment.CurrentDirectory;

                if (Directory.Exists(_options.DataBaseFolderPath))
                    Environment.CurrentDirectory = _options.DataBaseFolderPath;

                if (!OneDasUtilities.ValidateDatabaseFolderPath(Environment.CurrentDirectory, out var message))
                {
                    Console.WriteLine(message);
                    return 1;
                }

                // load campaign info
                Program.UpdateCampaignInfos();

                // service vs. interactive
                if (isUserInteractive)
                    await Program.CreateWebHost(currentDirectory).RunAsync();
                else
                    Program.CreateWebHost(currentDirectory).RunAsService();

                return 0;
            }
        }

        public static void UpdateCampaignInfos()
        {
            lock (_lock)
            {
                _databaseToCampaignMap = new Dictionary<IDatabase, IReadOnlyList<CampaignInfo>>();
                _campaignToCampaignMetaMap = new Dictionary<CampaignInfo, CampaignMetaInfo>();

                var databases = new List<IDatabase>()
                {
                    new HdfDatabase(_options.DataBaseFolderPath)
                    //new TestDatabase()
                };

                foreach (var database in databases)
                {
                    try
                    {
                        var campaignInfos = database.GetCampaignInfos();

                        foreach (var campaignInfo in campaignInfos)
                        {
                            var folderPath = Path.Combine(Environment.CurrentDirectory, "DB_META");
                            _campaignToCampaignMetaMap[campaignInfo] = Program.LoadCampaignMeta(folderPath);
                        }

                        _databaseToCampaignMap[database] = campaignInfos;
                    }
                    finally
                    {
                        database.Dispose();
                    }
                }
            }
        }

        public static void SaveCampaignMeta(CampaignMetaInfo campaignMeta)
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, "DB_META", $"{campaignMeta.Name}.json");

            if (!Program.GetCampaigns().Any(campaign => campaign.Name == campaignMeta.Name))
                throw new InvalidOperationException($"The campaign '{campaignMeta.Name}' does not exist within the database.");

            campaignMeta.Purge();

            var jsonString = JsonSerializer.Serialize(campaignMeta);
            File.WriteAllText(filePath, jsonString);
        }

        public static List<CampaignInfo> GetCampaigns()
        {
            lock (_lock)
            {
                return _databaseToCampaignMap.SelectMany(x => x.Value).ToList();
            }
        }

        public static IDatabase GetDatabase(string campaignName)
        {
            lock (_lock)
            {
                var database = _databaseToCampaignMap.Where(x => x.Value.Any(campaign => campaign.Name == campaignName)).Select(x => x.Key).FirstOrDefault();

                if (database == null)
                    throw new KeyNotFoundException("The requested campaign could not be found.");

                return database;
            }
        }

        private static IWebHost CreateWebHost(string currentDirectory)
        {
            if (!Directory.Exists(Path.Combine(currentDirectory, "wwwroot")))
                currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var webHost = new WebHostBuilder()
                .ConfigureServices(services => services.Configure<HdfExplorerOptions>(_configuration))
                .UseKestrel()
                .UseUrls(_options.AspBaseUrl)
                .UseContentRoot(currentDirectory)
                .UseStartup<Startup>()
                .SuppressStatusMessages(true)
                .Build();

            return webHost;
        }

        private static CampaignMetaInfo LoadCampaignMeta(string campaignName)
        {
            CampaignMetaInfo campaignMeta;

            var filePath = Path.Combine(Environment.CurrentDirectory, "DB_META", $"{campaignName}.json");

            if (!File.Exists(filePath))
                campaignMeta = new CampaignMetaInfo(campaignName);
            else
            {
                var jsonString = File.ReadAllText(filePath);
                campaignMeta = JsonSerializer.Deserialize<CampaignMetaInfo>(jsonString);
            }

            return campaignMeta;
        }

        private static async Task<int> ProcessCommand(string[] args)
        {
            var rootCommand = new RootCommand("Virtual dataset tool");

            rootCommand.AddCommand(Program.PrepareInitCommand());
            rootCommand.AddCommand(Program.PreparePwshCommand());
            rootCommand.AddCommand(Program.PrepareUpdateCommand());
            rootCommand.AddCommand(Program.PrepareVdsCommand());
            rootCommand.AddCommand(Program.PrepareAggregateCommand());

            return await rootCommand.InvokeAsync(args);
        }

        #endregion

        #region Commands

        private static Command PrepareInitCommand()
        {
            var command = new Command("init", "Initializes a OneDAS database in the current folder")
            {
                //
            };

            command.Handler = CommandHandler.Create(() =>
            {
                var logger = _loggerFactory.CreateLogger($"INIT");

                try
                {
                    new InitCommand().Run();
                    logger.LogInformation($"Execution 'init' command finished successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError($"Execution 'init' command failed. Error message: '{ex.Message}'.");
                    return 1;
                }

                return 0;
            });

            return command;
        }

        private static Command PreparePwshCommand()
        {
            var command = new Command("pwsh", "Runs the provided Powershell script")
            {
                new Option("--script-path", "The location of the powershell script")
                {
                    Argument = new Argument<string>(),
                    Required = true
                },
                new Option("--transaction-id", "Log messages are tagged with the transaction identifier")
                {
                    Argument = new Argument<string>(),
                    Required = true
                }
            };

            command.Handler = CommandHandler.Create((string scriptPath, string transactionId) =>
            {
                var logger = _loggerFactory.CreateLogger($"PWSH ({transactionId})");

                try
                {
                    new PwshCommand(scriptPath, logger).Run();
                    logger.LogInformation($"Execution of the 'pwsh' command finished successfully (path: '{scriptPath}').");
                }
                catch (Exception ex)
                {
                    logger.LogError($"Execution of the 'pwsh' command failed (path: '{scriptPath}'). Error message: '{ex.Message}'.");
                    return 1;
                }

                return 0;
            });

            return command;
        }

        private static Command PrepareUpdateCommand()
        {
            var command = new Command("update", "Updates the database index")
            {
                //
            };

            command.Handler = CommandHandler.Create(() =>
            {
                var logger = _loggerFactory.CreateLogger("UPDATE");

                try
                {
                    DateTime epochStart;
                    var date = DateTime.UtcNow.Date;

                    epochStart = new DateTime(date.Year, date.Month, 1).AddMonths(-1);
                    new VdsCommand(epochStart, logger).Run();

                    epochStart = new DateTime(date.Year, date.Month, 1);
                    new VdsCommand(epochStart, logger).Run();

                    epochStart = DateTime.MinValue;
                    new VdsCommand(epochStart, logger).Run();

                    logger.LogInformation($"Execution of the 'update' command finished successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError($"Execution of the 'vds' command failed. Error message: '{ex.Message}'.");
                    return 1;
                }

                return 0;
            });

            return command;
        }

        private static Command PrepareVdsCommand()
        {
            bool TryConvertArgument(SymbolResult result, out DateTime value)
            {
                return DateTime.TryParseExact(result.Token.Value, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out value);
            }

            var command = new Command("vds", "Updates the database index of files that are part of the specified epoch")
            {
                new Option("--epoch-start", "The start date of the epoch")
                {
                    Argument = new Argument<DateTime>(TryConvertArgument),
                    Required = false
                }
            };

            command.Handler = CommandHandler.Create((DateTime epochStart) =>
            {
                var logger = _loggerFactory.CreateLogger("VDS");

                try
                {
                    new VdsCommand(epochStart, logger).Run();
                    logger.LogInformation($"Execution of the 'vds' command finished successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError($"Execution of the 'vds' command failed. Error message: '{ex.Message}'.");
                    return 1;
                }

                return 0;
            });

            return command;
        }

        private static Command PrepareAggregateCommand()
        {
            var command = new Command("aggregate", "Aggregates data of channels that match the filter conditions.")
            {
                new Option("--method", "Possible arguments are 'mean', 'mean_polar', 'min', 'max', 'std', 'rms', 'min_bitwise' and 'max_bitwise'")
                {
                    Argument = new Argument<AggregationMethod>(),
                    Required = true
                },
                new Option("--argument", "Zero or more arguments for the selected aggregation method.")
                {
                    Argument = new Argument<string>(),
                    Required = true
                },
                new Option("--campaign-name", "The campaign name, e.g /A/B/C.")
                {
                    Argument = new Argument<string>(),
                    Required = true
                },
                new Option("--days", "The number of days in the past to look for files to calculate aggregations for.")
                {
                    Argument = new Argument<uint>(),
                    Required = true
                },
                new Option("--include-channel", "A regex based filter to include channels with certain names.")
                {
                    Argument = new Argument<string>(),
                    Required = false
                },
                new Option("--include-group", "A regex based filter to include channels within certain groups.")
                {
                    Argument = new Argument<string>(),
                    Required = false
                },
                new Option("--include-unit", "A regex based filter to include channels with certain unit.")
                {
                    Argument = new Argument<string>(),
                    Required = false
                },
                new Option("--exclude-channel", "A regex based filter to exclude channels with certain names.")
                {
                    Argument = new Argument<string>(),
                    Required = false
                },
                new Option("--exclude-group", "A regex based filter to exclude channels within certain groups.")
                {
                    Argument = new Argument<string>(),
                    Required = false
                },
                new Option("--exclude-unit", "A regex based filter to exclude channels with certain unit.")
                {
                    Argument = new Argument<string>(),
                    Required = false
                }
            };

            command.Handler = CommandHandler.Create((AggregationMethod method, string argument, string campaignName, uint days, ParseResult parseResult) =>
            {
                var logger = _loggerFactory.CreateLogger("AGGREGATE");
                var filters = new Dictionary<string, string>();
                var skip = 7;
                var isOption = true;

                for (int i = skip; i < parseResult.Tokens.Count - 1; i++)
                {
                    if (isOption)
                    {
                        var filterOption = parseResult.Tokens[i].Value;
                        var filterArgument = parseResult.Tokens[i + 1].Value;

                        filters[filterOption] = filterArgument;
                    }

                    isOption = !isOption;
                }

                try
                {
                    new AggregateCommand(method, argument, campaignName, days, filters, logger).Run();
                    logger.LogInformation($"Execution of the 'aggregate' command finished successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError($"Execution of the 'aggregate' command failed. Error message: '{ex.ToString()}'.");
                    return 1;
                }

                return 0;
            });

            return command;
        }

        #endregion
    }
}
