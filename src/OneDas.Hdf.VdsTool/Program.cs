using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneDas.Hdf.VdsTool.Commands;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OneDas.Hdf.VdsTool
{
    class Program
    {
        #region Fields

        private static ILoggerFactory _loggerFactory;

        #endregion

        #region Methods

        private static async Task<int> Main(string[] args)
        {
            #warning How to handle other, non-HDF databases?
            if (!args.Contains("pwsh"))
            {
                if (!OneDasUtilities.ValidateDatabaseFolderPath(Environment.CurrentDirectory, out var message))
                {
                    Console.WriteLine(message);
                    return 1;
                }
            }

            Console.Title = "VdsTool";

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            // paths
            var appdataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OneDAS", "Explorer");
            Directory.CreateDirectory(appdataFolderPath);

            var logFolderPath = Path.Combine(appdataFolderPath, "LOGS");
            Directory.CreateDirectory(logFolderPath);

            // configure logging
            _loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddFile(Path.Combine(logFolderPath, "VdsTool-{Date}.txt"), outputTemplate: OneDasConstants.FileLoggerTemplate);
            });

            // configure CLI
            var rootCommand = new RootCommand("Virtual dataset tool");

            rootCommand.AddCommand(Program.PrepareAutoVdsCommand());
            rootCommand.AddCommand(Program.PrepareVdsCommand());
            rootCommand.AddCommand(Program.PrepareInitCommand());
            rootCommand.AddCommand(Program.PrepareAggregateCommand());

            return await rootCommand.InvokeAsync(args);
        }

        #endregion

        #region Commands

        private static Command PrepareAutoVdsCommand()
        {
            var command = new Command("auto-vds", "Updates the database index")
            {
                //
            };

            command.Handler = CommandHandler.Create(() =>
            {
                var logger = _loggerFactory.CreateLogger("AUTO-VDS");

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

                    logger.LogInformation($"Execution of the 'auto-vds' command finished successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError($"Execution of the 'auto-vds' command failed. Error message: '{ex.Message}'.");
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
                return DateTime.TryParseExact(result.Tokens.First().Value, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out value);
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

        private static Command PrepareInitCommand()
        {
            var command = new Command("init", "Initializes a OneDAS database in the current folder")
            {
                //
            };

            command.Handler = CommandHandler.Create(() =>
            {
                var logger = _loggerFactory.CreateLogger("INIT");

                try
                {
                    new InitCommand().Run();
                    logger.LogInformation($"Execution of the 'init' command finished successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError($"Execution of the 'init' command failed. Error message: '{ex.Message}'.");
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
                new Option("--chunk-size", "The aggregation chunk size in MB.")
                {
                    Argument = new Argument<uint>(() => 200),
                    Required = false
                },
                new Option("--days", "The number of days in the past to look for files to calculate aggregations for.")
                {
                    Argument = new Argument<uint>(),
                    Required = true
                },
                new Option("--force", "Force recalculation of existing aggregations.")
                {
                    Argument = new Argument<bool>(() => false),
                    Required = false
                }
            };

            command.Handler = CommandHandler.Create((uint chunkSize, uint days, bool force) =>
            {
                var logger = _loggerFactory.CreateLogger("AGGREGATE");
                
                try
                {
                    new AggregateCommand(days, chunkSize, force, logger).Run();
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
