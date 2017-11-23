using System;
using System.Collections.Generic;
using System.Linq;
using OneDas.Hdf.IO;
using HDF.PInvoke;

namespace OneDas.Hdf.VdsTool.Navigation
{
    class AggregateFunctionNavigator : NavigatorBase
    {
        private long _vdsLocationId;
        private long _vdsMetaFileId;
        private long _groupId;
        private bool _updateAttribute;
        private string _currentPath;

        List<hdf_aggregate_function_t> _vdsMetaAggregateFunctionSet;

        ConsoleKeyInfo _consoleKeyInfo;

        public AggregateFunctionNavigator(long vdsLocationId, long vdsMetaFileId, string currentPath)
        {
            _vdsLocationId = vdsLocationId;
            _vdsMetaFileId = vdsMetaFileId;
            _currentPath = currentPath;

            _groupId = -1;

            base.Start();
        }

        protected override void OnInitialize()
        {
            if (IOHelper.CheckLinkExists(_vdsMetaFileId, _currentPath))
            {
                _groupId = H5G.open(_vdsMetaFileId, _currentPath);

                if (H5A.exists(_groupId, "aggregate_function_set") > 0)
                {
                    _vdsMetaAggregateFunctionSet = IOHelper.ReadAttribute<hdf_aggregate_function_t>(_groupId, "aggregate_function_set").ToList();
                }
                else
                {
                    _vdsMetaAggregateFunctionSet = new List<hdf_aggregate_function_t>();
                }

                H5G.close(_groupId);
            }
            else
            {
                _vdsMetaAggregateFunctionSet = new List<hdf_aggregate_function_t>();
            }
        }

        protected override void OnRedraw()
        {
            Console.Clear();
            Console.WriteLine($"Actual aggregation functions ({_vdsMetaAggregateFunctionSet.Count()}):\n");

            _vdsMetaAggregateFunctionSet.ForEach(x => Console.WriteLine($"[ ] {x.type, -15} | {x.argument}"));

            Console.WriteLine();
            Console.WriteLine("--------------------------------");
            Console.WriteLine("navigation: arrow up, arrow down");
            Console.WriteLine("      edit: arrow right, enter");
            Console.WriteLine("      exit: arrow left, ESC");
            Console.WriteLine("   default: d");
            Console.WriteLine("       new: n");
            Console.WriteLine("      copy: c");
            Console.WriteLine("      move: page up, page down");
            Console.WriteLine("    delete: del");
        }

        protected override bool OnWaitForUserInput()
        {
            if (this.SelectedIndex < 0)
            {
                this.SelectedIndex = _vdsMetaAggregateFunctionSet.Count() > 0 ? 0 : -1;
            }
            else if (this.SelectedIndex >= _vdsMetaAggregateFunctionSet.Count())
            {
                this.SelectedIndex = _vdsMetaAggregateFunctionSet.Count() - 1;
            }

            if (this.LastIndex >= 0)
            {
                Console.SetCursorPosition(1, this.LastIndex);
                Console.Write(" ");
            }

            if (this.SelectedIndex >= 0)
            {
                Console.SetCursorPosition(1, this.SelectedIndex + 2);
                Console.Write("x");
            }

            _consoleKeyInfo = Console.ReadKey(true);

            switch (_consoleKeyInfo.Key)
            {
                case ConsoleKey.UpArrow:

                    if (this.SelectedIndex > 0)
                    {
                        this.LastIndex = Console.CursorTop;
                        this.SelectedIndex -= 1;
                    }

                    break;

                case ConsoleKey.DownArrow:

                    if (this.SelectedIndex < _vdsMetaAggregateFunctionSet.Count() - 1)
                    {
                        this.LastIndex = Console.CursorTop;
                        this.SelectedIndex += 1;
                    }

                    break;

                case ConsoleKey.Enter:
                case ConsoleKey.RightArrow:

                    if (this.SelectedIndex >= 0)
                    {
                        _vdsMetaAggregateFunctionSet[this.SelectedIndex] = Program.PromptAggregateFunctionData(_vdsMetaAggregateFunctionSet[this.SelectedIndex]);
                        _updateAttribute = true;
                    }

                    break;

                case ConsoleKey.N:

                    _vdsMetaAggregateFunctionSet.Add(Program.PromptAggregateFunctionData(new hdf_aggregate_function_t()));

                    this.SelectedIndex = _vdsMetaAggregateFunctionSet.Count();
                    _updateAttribute = true;

                    break;

                case ConsoleKey.C:

                    if (this.SelectedIndex >= 0)
                    {
                        _vdsMetaAggregateFunctionSet.Add(_vdsMetaAggregateFunctionSet[this.SelectedIndex]);
                        this.SelectedIndex = _vdsMetaAggregateFunctionSet.Count();
                        this.OnRedraw();
                    }

                    break;

                case ConsoleKey.D:

                    _vdsMetaAggregateFunctionSet.Add(new hdf_aggregate_function_t("mean", "none"));
                    _vdsMetaAggregateFunctionSet.Add(new hdf_aggregate_function_t("min", "none"));
                    _vdsMetaAggregateFunctionSet.Add(new hdf_aggregate_function_t("max", "none"));
                    _vdsMetaAggregateFunctionSet.Add(new hdf_aggregate_function_t("std", "none"));
                    _updateAttribute = true;
                    this.OnRedraw();

                    break;

                case ConsoleKey.Delete:

                    if (this.SelectedIndex >= 0)
                    {
                        Console.Clear();
                        Console.Write("The selected item will be deleted. Proceed (Y/N)? ");

                        if (Console.ReadKey().Key == ConsoleKey.Y)
                        {
                            _vdsMetaAggregateFunctionSet.RemoveAt(this.SelectedIndex);
                            _updateAttribute = true;
                        }

                        this.OnRedraw();
                    }

                    break;

                case ConsoleKey.PageUp:

                    if (this.SelectedIndex >= 1 && this.SelectedIndex < _vdsMetaAggregateFunctionSet.Count())
                    {
                        hdf_aggregate_function_t hdf_aggregate_function = _vdsMetaAggregateFunctionSet[this.SelectedIndex];
                        _vdsMetaAggregateFunctionSet.RemoveAt(this.SelectedIndex);
                        _vdsMetaAggregateFunctionSet.Insert(this.SelectedIndex - 1, hdf_aggregate_function);
                        this.SelectedIndex -= 1;
                        _updateAttribute = true;
                    }

                    break;

                case ConsoleKey.PageDown:

                    if (this.SelectedIndex >= 0 && this.SelectedIndex < _vdsMetaAggregateFunctionSet.Count() - 1)
                    {
                        hdf_aggregate_function_t hdf_aggregate_function = _vdsMetaAggregateFunctionSet[this.SelectedIndex];
                        _vdsMetaAggregateFunctionSet.RemoveAt(this.SelectedIndex);
                        _vdsMetaAggregateFunctionSet.Insert(this.SelectedIndex + 1, hdf_aggregate_function);
                        this.SelectedIndex += 1;
                        _updateAttribute = true;
                    }

                    break;

                case ConsoleKey.Escape:
                case ConsoleKey.LeftArrow:
                    return true;
            }

            // update attribute
            if (_updateAttribute)
            {
                _groupId = IOHelper.OpenOrCreateGroup(_vdsMetaFileId, _currentPath).GroupId;
                IOHelper.PrepareAttribute(_groupId, "aggregate_function_set", _vdsMetaAggregateFunctionSet.ToArray(), new ulong[] { (ulong)_vdsMetaAggregateFunctionSet.Count() }, true);
                _updateAttribute = false;
                this.OnRedraw();
            }

            // clean up
            if (H5I.is_valid(_groupId) > 0)
            {
                H5F.flush(_groupId, H5F.scope_t.GLOBAL);
                H5G.close(_groupId);
            }

            return false;
        }
    }
}
