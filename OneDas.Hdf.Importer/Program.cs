using System;
using System.IO;

namespace OneDas.Hdf.Importer
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime dateTimeBegin;
            DateTime dateTimeEnd;

            string campaignName;
            string sourceDirectoryPath;
            string targetDirectoryPath;
            string logDirectoryPath;
            string logFilePath;

            dateTimeEnd = DateTime.UtcNow.Date.AddDays(-1);
            dateTimeBegin = dateTimeEnd.AddMonths(-1);

            campaignName = "LEHE_LEHE03_GENERAL_DAQ_V27";
            sourceDirectoryPath = $@"\\192.168.14.150\mdas\data\{ campaignName }_c7c054f2\HDF_DW1";
            targetDirectoryPath = @"M:\DATABASE";
            logDirectoryPath = Path.Combine(targetDirectoryPath, "SUPPORT", "LOGS", "HDF Importer");
            logFilePath = Path.Combine(logDirectoryPath, $"{ campaignName }.txt");

            //
            Directory.CreateDirectory(logDirectoryPath);
            File.AppendAllText(logFilePath, $"BEGIN from { dateTimeBegin.ToString("yyyy-MM-dd") } to { dateTimeEnd.ToString("yyyy-MM-dd") }{ Environment.NewLine }");

            for (int i = 0; i <= (dateTimeEnd - dateTimeBegin).TotalDays; i++)
            {
                DateTime currentDateTime;

                string currentFileName;
                string currentSourceFilePath;
                string currentTargetFilePath;

                currentDateTime = dateTimeBegin.AddDays(i);
                currentFileName = $"{ campaignName }_{ currentDateTime.ToString("yyyy-MM-ddTHH-mm-ssZ") }.h5";
                currentSourceFilePath = Path.Combine(sourceDirectoryPath, currentFileName);
                currentTargetFilePath = Path.Combine(targetDirectoryPath, "DB_NATIVE", currentDateTime.ToString("yyyy-MM"), currentFileName);

                System.Diagnostics.Trace.WriteLine(currentSourceFilePath);

                if (File.Exists(currentSourceFilePath))
                {
                    if (!File.Exists(currentTargetFilePath))
                    {
                        try
                        {
                            Console.Write($"Copying file { currentFileName } ... ");
                            Directory.CreateDirectory(Path.GetDirectoryName(currentTargetFilePath));
                            File.Copy(currentSourceFilePath, currentTargetFilePath);
                            Console.WriteLine($"Done.");
                            File.AppendAllText(logFilePath, $"\tsuccessful: { currentSourceFilePath }{ Environment.NewLine }");
                        }
                        catch (Exception)
                        {
                            Console.WriteLine($"Failed.");
                            File.AppendAllText(logFilePath, $"\tfailed: { currentSourceFilePath }{ Environment.NewLine }");
                        }
                    }
                }
                else
                {
                    File.AppendAllText(logFilePath, $"\tmissing: { currentSourceFilePath }{ Environment.NewLine }");
                }
            }

            File.AppendAllText(logFilePath, $"END{ Environment.NewLine }{ Environment.NewLine }");
        }
    }
}
