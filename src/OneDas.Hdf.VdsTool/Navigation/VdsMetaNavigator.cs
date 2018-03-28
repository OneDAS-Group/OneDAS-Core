using HDF.PInvoke;
using OneDas.Hdf.Core;
using OneDas.Hdf.IO;
using OneDas.Hdf.VdsTool.Documentation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OneDas.Hdf.VdsTool.Navigation
{
    class VdsMetaNavigator : NavigatorBase
    {
        private long _vdsGroupId;
        private long _vdsLocationId;
        private long _vdsMetaFileId;
        private string _currentPath;

        private IList<HdfElementBase> _currentList;

        public VdsMetaNavigator(long vdsLocationId, long vdsMetaFileId, string currentPath, IList<HdfElementBase> currentList)
        {
            _vdsLocationId = vdsLocationId;
            _vdsMetaFileId = vdsMetaFileId;
            _currentPath = currentPath;
            _currentList = currentList.OrderBy(x => x.GetDisplayName()).ToList();
            _vdsGroupId = -1;

            base.Start();
        }

        protected override void OnInitialize()
        {
            if (_currentList.Any())
            {
                this.SelectedIndex = 0;
            }
        }

        protected override void OnRedraw()
        {
            Console.BufferHeight = Math.Max(Console.BufferHeight, _currentList.Count() + 11);
            Console.Clear();

            foreach (var hdfElement in _currentList)
            {
                Console.WriteLine($"[ ] { hdfElement.GetDisplayName() }");
            }

            Console.WriteLine();
            Console.WriteLine("---------------------------------------");
            Console.WriteLine("       navigation: arrow up, arrow down");
            Console.WriteLine("       next level: arrow right, enter");
            Console.WriteLine("   previous level: arrow left, ESC");
            Console.WriteLine("description / tag: 1");
            Console.WriteLine("transfer function: 2");
            Console.WriteLine(" aggreg. function: 3");
            Console.WriteLine("    documentation: d");
            Console.WriteLine("            purge: p");
        }

        protected override bool OnWaitForUserInput()
        {
            if (this.LastIndex >= 0)
            {
                Console.SetCursorPosition(1, this.LastIndex);
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
                        this.LastIndex = this.SelectedIndex;
                        this.SelectedIndex -= 1;
                    }
                    break;

                case ConsoleKey.DownArrow:

                    if (this.SelectedIndex < _currentList.Count - 1)
                    {
                        this.LastIndex = this.SelectedIndex;
                        this.SelectedIndex += 1;
                    }
                    break;

                case ConsoleKey.Enter:
                case ConsoleKey.RightArrow:

                    if (this.SelectedIndex >= 0)
                    {
                        _vdsGroupId = H5G.open(_vdsLocationId, _currentList[this.SelectedIndex].Name);
                        new VdsMetaNavigator(_vdsGroupId, _vdsMetaFileId, GeneralHelper.CombinePath(_currentPath, _currentList[this.SelectedIndex].Name), _currentList[this.SelectedIndex].GetChildSet().ToList());
                        H5G.close(_vdsGroupId);
                        this.OnRedraw();
                    }

                    break;

                case ConsoleKey.D1:

                    if (this.SelectedIndex >= 0 && _currentList[this.SelectedIndex] is CampaignInfo)
                    {
                        _vdsGroupId = H5G.open(_vdsLocationId, _currentList[this.SelectedIndex].Name);
                        new CampaignNavigator(_vdsGroupId, _vdsMetaFileId, _currentList[this.SelectedIndex].Name);
                        H5G.close(_vdsGroupId);
                        this.OnRedraw();
                    }

                    break;

                case ConsoleKey.D2:

                    if (this.SelectedIndex >= 0 && _currentList[this.SelectedIndex] is VariableInfo)
                    {
                        VariableInfo variableInfo;

                        variableInfo = (VariableInfo)_currentList[this.SelectedIndex];
                        _vdsGroupId = H5G.open(_vdsLocationId, _currentList[this.SelectedIndex].Name);
                        new TransferFunctionNavigator(_vdsGroupId, _vdsMetaFileId, GeneralHelper.CombinePath(_currentPath, variableInfo.Name), variableInfo.UnitSet, variableInfo.TransferFunctionSet);
                        H5G.close(_vdsGroupId);
                        this.OnRedraw();
                    }

                    break;

                case ConsoleKey.D3:

                    if (this.SelectedIndex >= 0 && _currentList[this.SelectedIndex] is VariableInfo)
                    {
                        _vdsGroupId = H5G.open(_vdsLocationId, _currentList[this.SelectedIndex].Name);
                        new AggregateFunctionNavigator(_vdsGroupId, _vdsMetaFileId, GeneralHelper.CombinePath(_currentPath, _currentList[this.SelectedIndex].Name));
                        H5G.close(_vdsGroupId);
                        this.OnRedraw();
                    }

                    break;

                case ConsoleKey.D:

                    if (_currentList[this.SelectedIndex] is CampaignInfo)
                    {
                        string directoryPath;

                        try
                        {
                            directoryPath = Program.PromptDirectoryPath(Directory.GetCurrentDirectory());

                            this.WriteCampaignDocumentation(directoryPath, (CampaignInfo)_currentList[this.SelectedIndex]);
                        }
                        catch
                        {
                            //
                        }

                        this.OnRedraw();
                    }

                    break;

                case ConsoleKey.P:

                    _vdsGroupId = H5G.open(_vdsLocationId, _currentList[this.SelectedIndex].Name);
                    new PurgeNavigator(_vdsLocationId, _vdsMetaFileId, _currentPath);
                    H5G.close(_vdsGroupId);
                    this.OnRedraw();

                    break;

                case ConsoleKey.Escape:
                case ConsoleKey.LeftArrow:
                    return true;
            }

            return false;
        }

        public void WriteCampaignDocumentation(string directoryPath, CampaignInfo campaignInfo)
        {
            long campaign_groupId = -1;

            string description;
            string filePath;

            RestructuredTextWriter restructuredTextWriter;
            List<string> groupNameSet;

            Console.Clear();

            groupNameSet = campaignInfo.VariableInfoSet.Select(variableInfo => variableInfo.VariableGroupSet.Last()).Distinct().ToList();
            filePath = Path.Combine(directoryPath, $"{ campaignInfo.Name.ToLower().Replace("/", "_").TrimStart('_') }.rst");

            try
            {
                using (StreamWriter streamWriter = new StreamWriter(new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read)))
                {
                    restructuredTextWriter = new RestructuredTextWriter(streamWriter);

                    // campaign header
                    restructuredTextWriter.WriteHeading(campaignInfo.Name.TrimStart('/').Replace("/", " / "), SectionHeader.Section);

                    // campaign description
                    restructuredTextWriter.WriteLine();

                    try
                    {
                        if (IOHelper.CheckLinkExists(_vdsMetaFileId, campaignInfo.Name))
                        {
                            campaign_groupId = H5G.open(_vdsMetaFileId, campaignInfo.Name);

                            if (H5A.exists(campaign_groupId, "description") > 0)
                            {
                                description = IOHelper.ReadAttribute<string>(campaign_groupId, "description").First();
                            }
                            else
                            {
                                description = "no description available";
                            }
                        }
                        else
                        {
                            description = "no description available";
                        }

                        restructuredTextWriter.WriteNote(description);
                    }
                    finally
                    {
                        if (H5I.is_valid(campaign_groupId) > 0) { H5G.close(campaign_groupId); }
                    }

                    // groups
                    foreach (string groupName in groupNameSet)
                    {
                        RestructuredTextTable restructuredTextTable;

                        List<VariableInfo> groupedVariableInfoSet;

                        restructuredTextWriter.WriteLine();
                        restructuredTextWriter.WriteHeading(groupName, SectionHeader.SubSection);
                        restructuredTextWriter.WriteLine();

                        restructuredTextTable = new RestructuredTextTable(new List<string>() { "Name", "Unit", "Guid" });
                        groupedVariableInfoSet = campaignInfo.VariableInfoSet.Where(variableInfo => variableInfo.VariableGroupSet.Last() == groupName).OrderBy(variableInfo => variableInfo.VariableNameSet.Last()).ToList();

                        // variables
                        groupedVariableInfoSet.ForEach(variableInfo =>
                        {
                            long variable_groupId = -1;

                            string groupPath;
                            string name;
                            string guid;
                            string unit;

                            List<hdf_transfer_function_t> transferFunctionSet;
                            List<hdf_aggregate_function_t> aggregateFunctionSet;

                            // name
                            name = variableInfo.VariableNameSet.Last();

                            if (name.Count() > 43)
                            {
                                name = $"{ name.Substring(0, 40) }...";
                            }

                            // guid
                            guid = $"{ variableInfo.Name.Substring(0, 8) }...";

                            // unit, transferFunctionSet, aggregateFunctionSet
                            unit = string.Empty;

                            try
                            {
                                groupPath = variableInfo.GetPath();

                                if (IOHelper.CheckLinkExists(_vdsMetaFileId, groupPath))
                                {
                                    variable_groupId = H5G.open(_vdsMetaFileId, groupPath);

                                    if (H5A.exists(variable_groupId, "unit") > 0)
                                    {
                                        unit = IOHelper.ReadAttribute<string>(variable_groupId, "unit").FirstOrDefault();
                                    }

                                    if (H5A.exists(variable_groupId, "transfer_function_set") > 0)
                                    {
                                        transferFunctionSet = IOHelper.ReadAttribute<hdf_transfer_function_t>(variable_groupId, "transfer_function_set").ToList();
                                    }

                                    if (H5A.exists(variable_groupId, "aggregate_function_set") > 0)
                                    {
                                        aggregateFunctionSet = IOHelper.ReadAttribute<hdf_aggregate_function_t>(variable_groupId, "aggregate_function_set").ToList();
                                    }
                                }
                            }
                            finally
                            {
                                if (H5I.is_valid(variable_groupId) > 0) { H5G.close(variable_groupId); }
                            }

                            // 
                            restructuredTextTable.AddRow(new List<string> { name, unit, guid });

                            //transferFunctionSet.ForEach(x => streamWriter.Write($"{ x.date_time }, { x.type }, { x.option }, { x.argument } | "));
                            //aggregateFunctionSet.ForEach(x => streamWriter.Write($"{ x.type }, { x.argument } | "));
                        });

                        restructuredTextWriter.WriteTable(restructuredTextTable);
                        restructuredTextWriter.WriteLine();
                    }
                }

                Console.WriteLine($"The file has been successfully written to:\n{ filePath }");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("\nPress any key to continue ...");
            Console.ReadKey();

        }
    }
}
