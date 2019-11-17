using HDF.PInvoke;
using OneDas.Hdf.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OneDas.Hdf.VdsTool.Navigation
{
    class TransferFunctionNavigator : NavigatorBase
    {
        private long _vdsLocationId;
        private long _vdsMetaFileId;
        private long _groupId;

        private int _offset;
        private int _totalCount;

        private string _currentPath;

        private bool _isEscaped;
        private bool _updateUnit;
        private bool _updateAttribute;

        string _unit;

        List<string> _vdsUnitSet;
        List<hdf_transfer_function_t> _vdsTransferFunctionSet;
        List<hdf_transfer_function_t> _vdsMetaTransferFunctionSet;

        ConsoleKeyInfo _consoleKeyInfo;

        public TransferFunctionNavigator(long vdsLocationId, long vdsMetaFileId, string currentPath, IList<string> vdsUnitSet, IList<hdf_transfer_function_t> vdsTransferFunctionSet)
        {
            _vdsLocationId = vdsLocationId;
            _vdsMetaFileId = vdsMetaFileId;
            _currentPath = currentPath;
            _vdsUnitSet = vdsUnitSet.ToList();
            _vdsTransferFunctionSet = vdsTransferFunctionSet.ToList();

            _groupId = -1;

            base.Start();
        }

        protected override void OnInitialize()
        {
            if (IOHelper.CheckLinkExists(_vdsMetaFileId, _currentPath))
            {
                _groupId = H5G.open(_vdsMetaFileId, _currentPath);

                if (H5A.exists(_groupId, "unit") > 0)
                {
                    _unit = IOHelper.ReadAttribute<string>(_groupId, "unit").FirstOrDefault();
                }

                if (H5A.exists(_groupId, "transfer_function_set") > 0)
                {
                    _vdsMetaTransferFunctionSet = IOHelper.ReadAttribute<hdf_transfer_function_t>(_groupId, "transfer_function_set").ToList();
                }
                else
                {
                    _vdsMetaTransferFunctionSet = new List<hdf_transfer_function_t>();
                }

                H5G.close(_groupId);
            }
            else
            {
                _vdsMetaTransferFunctionSet = new List<hdf_transfer_function_t>();
            }
        }

        protected override void OnRedraw()
        {
            Console.Clear();
            Console.WriteLine($"Unit: { _unit }\n");
            Console.WriteLine($"Suggested transfer functions ({ _vdsTransferFunctionSet.Count() }):\n");

            _vdsTransferFunctionSet.ForEach(x => Console.WriteLine($"[ ] {x.date_time.Replace("T", " "),-20} | {x.type,-15} | {x.option,-15} | { x.argument }"));

            Console.WriteLine();
            Console.WriteLine($"Actual transfer functions ({ _vdsMetaTransferFunctionSet.Count() }):\n");

            _vdsMetaTransferFunctionSet.ForEach(x => Console.WriteLine($"[ ] {x.date_time.Replace("T", " "),-20} | {x.type,-15} | {x.option,-15} | { x.argument }"));

            Console.WriteLine();
            Console.WriteLine("--------------------------------");
            Console.WriteLine("navigation: arrow up, arrow down");
            Console.WriteLine("      edit: arrow right, enter");
            Console.WriteLine("      exit: arrow left, ESC");
            Console.WriteLine("       new: n");
            Console.WriteLine("      copy: c");
            Console.WriteLine("      unit: u");
            Console.WriteLine("      move: page up, page down");
            Console.WriteLine("    delete: del");
        }

        protected override bool OnWaitForUserInput()
        {
            _totalCount = _vdsTransferFunctionSet.Count() + _vdsMetaTransferFunctionSet.Count();

            if (this.SelectedIndex < 0)
            {
                this.SelectedIndex = _totalCount > 0 ? 0 : -1;
            }
            else if (this.SelectedIndex >= _totalCount)
            {
                this.SelectedIndex = _totalCount - 1;
            }

            if (this.SelectedIndex >= _vdsTransferFunctionSet.Count())
            {
                _offset = 3;
            }
            else
            {
                _offset = 0;
            }

            if (this.LastIndex >= 0)
            {
                Console.SetCursorPosition(1, this.LastIndex);
                Console.Write(" ");
            }

            if (this.SelectedIndex >= 0)
            {
                Console.SetCursorPosition(1, this.SelectedIndex + _offset + 4);
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

                    if (this.SelectedIndex < _totalCount - 1)
                    {
                        this.LastIndex = Console.CursorTop;
                        this.SelectedIndex += 1;
                    }

                    break;

                case ConsoleKey.Enter:
                case ConsoleKey.RightArrow:

                    if (this.SelectedIndex >= _vdsTransferFunctionSet.Count())
                    {
                        int index = this.SelectedIndex - _vdsTransferFunctionSet.Count();
                        _vdsMetaTransferFunctionSet[index] = Program.PromptTransferFunctionData(_vdsMetaTransferFunctionSet[index]);
                        _updateAttribute = true;
                    }

                    break;

                case ConsoleKey.N:

                    if ((_consoleKeyInfo.Modifiers & ConsoleModifiers.Control) != 0)
                    {
                        _vdsMetaTransferFunctionSet.Add(Program.PromptTransferFunctionData(new hdf_transfer_function_t(DateTime.MinValue.ToString("yyyy-MM-dd"), "polynomial", "permanent", string.Empty)));
                    }
                    else
                    {
                        _vdsMetaTransferFunctionSet.Add(Program.PromptTransferFunctionData(new hdf_transfer_function_t()));
                    }

                    this.SelectedIndex = _totalCount;
                    _updateAttribute = true;

                    break;

                case ConsoleKey.C:

                    if (this.SelectedIndex >= 0)
                    {
                        if (this.SelectedIndex >= 0 && this.SelectedIndex < _vdsTransferFunctionSet.Count())
                        {
                            _vdsMetaTransferFunctionSet.Add(_vdsTransferFunctionSet[this.SelectedIndex]);
                        }
                        else
                        {
                            _vdsMetaTransferFunctionSet.Add(_vdsMetaTransferFunctionSet[this.SelectedIndex - _vdsTransferFunctionSet.Count()]);
                        }

                        _updateAttribute = true;
                        this.SelectedIndex = _totalCount;
                        this.OnRedraw();
                    }

                    break;

                case ConsoleKey.U:

                    Console.CursorVisible = true;
                    Console.Clear();
                    Console.Write($"Enter value for unit ({ _unit }): ");

                    (_unit, _isEscaped) = Utilities.ReadLine(_vdsUnitSet);

                    if (!_isEscaped)
                    {
                        _updateUnit = true;
                    }

                    Console.CursorVisible = false;

                    break;

                case ConsoleKey.Delete:

                    if (this.SelectedIndex >= _vdsTransferFunctionSet.Count())
                    {
                        Console.Clear();
                        Console.Write("The selected item will be deleted. Proceed (Y/N)? ");

                        if (Console.ReadKey().Key == ConsoleKey.Y)
                        {
                            _vdsMetaTransferFunctionSet.RemoveAt(this.SelectedIndex - _vdsTransferFunctionSet.Count());
                            _updateAttribute = true;
                        }
                        else
                        {
                            this.OnRedraw();
                        }
                    }

                    break;

                case ConsoleKey.PageUp:

                    if (this.SelectedIndex >= _vdsTransferFunctionSet.Count() + 1 && this.SelectedIndex < _totalCount)
                    {
                        int index = this.SelectedIndex - _vdsTransferFunctionSet.Count();
                        hdf_transfer_function_t hdf_transfer_function = _vdsMetaTransferFunctionSet[index];
                        _vdsMetaTransferFunctionSet.RemoveAt(index);
                        _vdsMetaTransferFunctionSet.Insert(index - 1, hdf_transfer_function);
                        this.SelectedIndex -= 1;
                        _updateAttribute = true;
                    }

                    break;

                case ConsoleKey.PageDown:

                    if (this.SelectedIndex >= _vdsTransferFunctionSet.Count() && this.SelectedIndex < _totalCount - 1)
                    {
                        int index = this.SelectedIndex - _vdsTransferFunctionSet.Count();
                        hdf_transfer_function_t hdf_transfer_function = _vdsMetaTransferFunctionSet[index];
                        _vdsMetaTransferFunctionSet.RemoveAt(index);
                        _vdsMetaTransferFunctionSet.Insert(index + 1, hdf_transfer_function);
                        this.SelectedIndex += 1;
                        _updateAttribute = true;
                    }

                    break;

                case ConsoleKey.Escape:
                case ConsoleKey.LeftArrow:
                    return true;
            }

            // update unit
            if (_updateUnit)
            {
                _groupId = IOHelper.OpenOrCreateGroup(_vdsMetaFileId, _currentPath).GroupId;
                IOHelper.PrepareAttribute(_groupId, "unit", new string[] { _unit }, new ulong[] { 1 }, true);
                _updateUnit = false;
                this.OnRedraw();
            }

            // update attribute
            if (_updateAttribute)
            {
                _groupId = IOHelper.OpenOrCreateGroup(_vdsMetaFileId, _currentPath).GroupId;
                IOHelper.PrepareAttribute(_groupId, "transfer_function_set", _vdsMetaTransferFunctionSet.ToArray(), new ulong[] { (ulong)_vdsMetaTransferFunctionSet.Count() }, true);
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
