using HDF.PInvoke;
using OneDas.Hdf.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace OneDas.Hdf.VdsTool.Navigation
{
    class MainMenuNavigator : NavigatorBase
    {
        List<Action> _actionSet;

        public MainMenuNavigator()
        {
            base.Start();
        }

        protected override void OnInitialize()
        {
            _actionSet = new List<Action>();

            _actionSet.Add(() => this.Menu_1());
            _actionSet.Add(() => this.Menu_2());
            _actionSet.Add(() => this.Menu_3());
            _actionSet.Add(() => this.Menu_4());

            this.SelectedIndex = 0;

            Console.CursorVisible = false;
        }

        protected override void OnRedraw()
        {
            Console.Clear();
            Console.WriteLine("[x] VDS file");
            Console.WriteLine("[ ] VDS metadata file");
            Console.WriteLine("[ ] Aggregations");
            Console.WriteLine("[ ] Editor");
        }

        protected override bool OnWaitForUserInput()
        {
            for (int i = 0; i < _actionSet.Count; i++)
            {
                Console.SetCursorPosition(1, i);
                Console.Write(" ");
            }

            if (this.SelectedIndex >= 0)
            {
                Console.SetCursorPosition(1, this.SelectedIndex);
                Console.Write("x");
            }

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.UpArrow:

                    if (this.SelectedIndex > 0)
                    {
                        this.SelectedIndex -= 1;
                    }
                    break;

                case ConsoleKey.DownArrow:

                    if (this.SelectedIndex < _actionSet.Count - 1)
                    {
                        this.SelectedIndex += 1;
                    }
                    break;

                case ConsoleKey.Enter:
                case ConsoleKey.RightArrow:

                    _actionSet[this.SelectedIndex]();
                    this.OnRedraw();

                    break;

                case ConsoleKey.Escape:
                    return true;

                default:
                    break;
            }

            return false;
        }

        private void Menu_1()
        {
            bool isTopLevel;

            string dateTime;
            string vdsFilePath;
            var sourceDirectoryPath = new List<string>();

            bool isEscaped = false;

            DateTime epochStart = default;
            DateTime epochEnd;

            //
            Console.CursorVisible = true;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Please enter the year and month (yyyy-mm) of the source files:");

                dateTime = VdsToolUtilities.ReadLine(new List<string>(), ref isEscaped);

                if (isEscaped)
                {
                    Console.CursorVisible = false;
                    return;
                }
                else if (string.IsNullOrWhiteSpace(dateTime) || DateTime.TryParseExact(dateTime, "yyyy-MM", CultureInfo.CurrentCulture, DateTimeStyles.AdjustToUniversal, out epochStart))
                {
                    break;
                }
            }

            Console.CursorVisible = false;

            if (epochStart > DateTime.MinValue)
            {
                epochEnd = epochStart.AddMonths(1);
                sourceDirectoryPath.Add(Path.Combine(Program.BaseDirectoryPath, "DB_AGGREGATION", epochStart.ToString("yyyy-MM")));
                sourceDirectoryPath.Add(Path.Combine(Program.BaseDirectoryPath, "DB_IMPORT", epochStart.ToString("yyyy-MM")));
                sourceDirectoryPath.Add(Path.Combine(Program.BaseDirectoryPath, "DB_NATIVE", epochStart.ToString("yyyy-MM")));
                vdsFilePath = Path.Combine(Program.BaseDirectoryPath, "VDS", $"{epochStart.ToString("yyyy-MM")}.h5");
                isTopLevel = false;
            }
            else
            {
                epochStart = new DateTime(2017, 01, 01, 0, 0, 0, DateTimeKind.Utc);
                epochEnd = new DateTime(2030, 01, 01, 0, 0, 0, DateTimeKind.Utc);
                sourceDirectoryPath.Add(Path.Combine(Program.BaseDirectoryPath, "VDS"));
                vdsFilePath = Path.Combine(Program.BaseDirectoryPath, "VDS.h5");
                isTopLevel = true;
            }

            Console.WriteLine();
            Console.WriteLine($"Epoch start: {epochStart.ToString("yyyy-MM-dd")}");
            Console.WriteLine($"Epoch end:   {epochEnd.ToString("yyyy-MM-dd")}");
            Console.WriteLine();

            Program.CreateVirtualDatasetFile(sourceDirectoryPath, vdsFilePath, epochStart, epochEnd, isTopLevel);
        }

        private void Menu_2()
        {
            long vdsFileId = -1;
            long vdsMetaFileId = -1;
            long fcPropertyId = -1;

            string vdsFilePath;
            string vdsMetaFilePath;

            List<CampaignInfo> campaignInfoSet;

            //
            vdsFilePath = Path.Combine(Program.BaseDirectoryPath, "VDS.h5");
            vdsMetaFilePath = Path.Combine(Program.BaseDirectoryPath, "VDS_META.h5");

            if (File.Exists(vdsFilePath))
            {
                vdsFileId = H5F.open(vdsFilePath, H5F.ACC_RDONLY);
            }
            else
            {
                return;
            }

            if (File.Exists(vdsMetaFilePath))
            {
                vdsMetaFileId = H5F.open(vdsMetaFilePath, H5F.ACC_RDWR);
            }

            if (vdsMetaFileId == -1)
            {
                fcPropertyId = H5P.create(H5P.FILE_CREATE);
                H5P.set_file_space(fcPropertyId, H5F.file_space_type_t.ALL_PERSIST);
                vdsMetaFileId = H5F.create(vdsMetaFilePath, H5F.ACC_TRUNC, fcPropertyId);
            }

            campaignInfoSet = GeneralHelper.GetCampaignInfoSet(vdsFileId, true);
            IList<HdfElementBase> currentList = campaignInfoSet.Cast<HdfElementBase>().ToList();

            new VdsMetaNavigator(vdsFileId, vdsMetaFileId, "/", currentList);

            // clean up
            H5F.close(vdsFileId);
            H5F.close(vdsMetaFileId);
        }

        private void Menu_3()
        {
            string dateTime;
            string sourceDirectoryPath;
            string targetDirectoryPath;
            string logDirectoryPath;
            string vdsMetaFilePath;

            bool isEscaped = false;

            DateTime epochStart = default;
            DateTime epochEnd;

            //
            Console.CursorVisible = true;

            vdsMetaFilePath = Path.Combine(Program.BaseDirectoryPath, "VDS_META.h5");

            if (!File.Exists(vdsMetaFilePath))
            {
                return;
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Please enter the year and month (yyyy-mm) of the source files:");

                dateTime = VdsToolUtilities.ReadLine(new List<string>(), ref isEscaped);

                if (isEscaped)
                {
                    Console.CursorVisible = false;
                    
                    return;
                }
                else if (DateTime.TryParseExact(dateTime, "yyyy-MM", CultureInfo.CurrentCulture, DateTimeStyles.AdjustToUniversal, out epochStart))
                {
                    break;
                }
            }

            Console.CursorVisible = false;

            epochEnd = epochStart.AddMonths(1);
            sourceDirectoryPath = Path.Combine(Program.BaseDirectoryPath, "DB_NATIVE", epochStart.ToString("yyyy-MM"));
            targetDirectoryPath = Path.Combine(Program.BaseDirectoryPath, "DB_AGGREGATION", epochStart.ToString("yyyy-MM"));
            logDirectoryPath = Path.Combine(Program.BaseDirectoryPath, "SUPPORT", "LOGS", "HDF VdsTool");

            Console.WriteLine();
            Console.WriteLine($"Epoch start: {epochStart.ToString("yyyy-MM-dd")}");
            Console.WriteLine($"Epoch end:   {epochEnd.ToString("yyyy-MM-dd")}");
            Console.WriteLine();

            Program.CreateAggregatedFiles(sourceDirectoryPath, targetDirectoryPath, logDirectoryPath, vdsMetaFilePath, epochStart, epochEnd);
        }

        private void Menu_4()
        {
            new EditorNavigator();
        }
    }
}
