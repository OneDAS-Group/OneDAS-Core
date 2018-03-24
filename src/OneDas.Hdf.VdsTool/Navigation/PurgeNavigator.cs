using System;
using System.Collections.Generic;
using System.Linq;
using HDF.PInvoke;
using OneDas.Hdf.Core;
using OneDas.Hdf.IO;

namespace OneDas.Hdf.VdsTool.Navigation
{
    class PurgeNavigator : NavigatorBase
    {
        private long _vdsLocationId;
        private long _vdsMetaFileId;
        private long _vdsMetaGroupId = -1;
        private string _currentPath;

        private int offset;

        private IList<(string Path, string Name)> _currentList;

        public PurgeNavigator(long vdsLocationId, long vdsMetaFileId, string currentPath)
        {
            _vdsLocationId = vdsLocationId;
            _vdsMetaFileId = vdsMetaFileId;
            _currentPath = currentPath;

            offset = 2;

            base.Start();
        }

        protected override void OnInitialize()
        {
            _currentList = this.FindOrphanedPathSet();

            if (_currentList.Any())
            {
                this.SelectedIndex = 0;
            }
        }

        protected override void OnRedraw()
        {
            Console.Clear();
            Console.WriteLine($"The following orphaned entries are found in VDS_META.h5 but NOT in VDS.h5:");
            Console.WriteLine();

            foreach (var path in _currentList)
            {
                Console.WriteLine($"[ ] { GeneralHelper.CombinePath(path.Path, path.Name) }");
            }

            Console.WriteLine();
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("     navigation: arrow up, arrow down");
            Console.WriteLine(" previous level: arrow left, ESC");
            Console.WriteLine("   delete entry: del");
            Console.WriteLine();
            Console.WriteLine($"Location: {_currentPath}");
        }

        protected override bool OnWaitForUserInput()
        {
            if (this.SelectedIndex >= _currentList.Count())
            {
                this.SelectedIndex = -1;
            }

            if (this.LastIndex >= 0)
            {
                Console.SetCursorPosition(1, this.LastIndex + offset);
                Console.Write(" ");
            }

            if (this.SelectedIndex >= 0)
            {
                Console.SetCursorPosition(1, this.SelectedIndex + offset);
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

                case ConsoleKey.Delete:

                    if (this.SelectedIndex > -1)
                    {
                        _vdsMetaGroupId = IOHelper.OpenOrCreateGroup(_vdsMetaFileId, GeneralHelper.CombinePath(_currentPath, _currentList[this.SelectedIndex].Path)).GroupId;

                        if (!IOHelper.CheckLinkExists(_vdsMetaGroupId, _currentList[this.SelectedIndex].Name) || // if link does not exist, or else ...
                            H5L.delete(_vdsMetaGroupId, _currentList[this.SelectedIndex].Name) > -1)              // ... deletion was successful, ...
                        {
                            _currentList.RemoveAt(this.SelectedIndex);                                            // ... then remove that item from list
                        }
                        
                        // clean up
                        H5G.close(_vdsMetaGroupId);

                        this.OnRedraw();
                    }
                    break;

                case ConsoleKey.Escape:
                case ConsoleKey.LeftArrow:
                    return true;
            }

            return false;
        }

        public List<(string Path, string Name)> FindOrphanedPathSet()
        {
            List<(string Path, string Name)> pathSet;
            List<(string Path, string Name)> orphanedPathSet;

            //
            orphanedPathSet = new List<(string Path, string Name)>();

            // check if current VDS.h5 path is also included in VDS_META.h5 file
            if (IOHelper.CheckLinkExists(_vdsMetaFileId, _currentPath))
            {
                _vdsMetaGroupId = IOHelper.OpenOrCreateGroup(_vdsMetaFileId, _currentPath).GroupId;
                pathSet = GeneralHelper.GetPathSet(_vdsMetaGroupId, 5, true, true);

                foreach (var path in pathSet)
                {
                    if (!IOHelper.CheckLinkExists(_vdsLocationId, GeneralHelper.CombinePath(path.Path, path.Name)))
                    {
                        orphanedPathSet.Add(path);
                    }
                }

                // clean up
                H5G.close(_vdsMetaGroupId);
            }

            return orphanedPathSet;
        }
    }
}
