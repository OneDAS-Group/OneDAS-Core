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
            string dateTime;
            bool isEscaped;
            DateTime epochStart;

            epochStart = default;

            Console.CursorVisible = true;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Please enter the year and month (yyyy-mm) of the source files:");

                (dateTime, isEscaped) = VdsToolUtilities.ReadLine(new List<string>());

                if (isEscaped)
                {
                    Console.CursorVisible = false;
                    return;
                }
                else if (string.IsNullOrWhiteSpace(dateTime) || DateTime.TryParseExact(dateTime, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out epochStart))
                {
                    break;
                }
            }

            Console.CursorVisible = false;
            Program.CreateVirtualDatasetFile(epochStart);
        }

        private void Menu_2()
        {
            long vdsFileId = -1;
            long vdsMetaFileId = -1;
            long fcPropertyId = -1;

            string vdsFilePath;
            string vdsMetaFilePath;

            List<CampaignInfo> campaignInfoSet;
            IList<HdfElementBase> currentList;

            //
            vdsFilePath = Path.Combine(Program.BaseDirectoryPath, "VDS.h5");
            vdsMetaFilePath = Path.Combine(Program.BaseDirectoryPath, "VDS_META.h5");

            try
            {
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
                currentList = campaignInfoSet.Cast<HdfElementBase>().ToList();

                new VdsMetaNavigator(vdsFileId, vdsMetaFileId, "/", currentList);
            }
            finally
            {
                if (H5I.is_valid(vdsFileId) > 0) { H5F.close(vdsFileId); }
                if (H5I.is_valid(vdsMetaFileId) > 0) { H5F.close(vdsMetaFileId); }
            }
        }

        private void Menu_3()
        {
            string dateTime;
            bool isEscaped;

            DateTime epochStart = default;

            //
            Console.CursorVisible = true;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Please enter the year and month (yyyy-mm) of the source files:");

                (dateTime, isEscaped) = VdsToolUtilities.ReadLine(new List<string>());

                if (isEscaped)
                {
                    Console.CursorVisible = false;

                    return;
                }
                else if (DateTime.TryParseExact(dateTime, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out epochStart))
                {
                    break;
                }
            }

            Console.CursorVisible = false;
            Program.CreateAggregatedFiles(epochStart);
        }

        private void Menu_4()
        {
            new EditorNavigator();
        }
    }
}
