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
    }
}
