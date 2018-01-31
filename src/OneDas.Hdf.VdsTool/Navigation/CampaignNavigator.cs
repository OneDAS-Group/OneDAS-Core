using System;
using System.Collections.Generic;
using System.Linq;
using OneDas.Hdf.IO;
using HDF.PInvoke;

namespace OneDas.Hdf.VdsTool.Navigation
{
    class CampaignNavigator : NavigatorBase
    {
        private long _vdsLocationId;
        private long _vdsMetaFileId;
        private long _groupId;

        private int _baseOffset;
        private int _offset;
        private int _totalCount;

        private string _description;
        private string _currentPath;

        private bool _updateDescription;
        private bool _updateAttribute;

        List<hdf_tag_t> _vdsTagSet;
        List<hdf_tag_t> _vdsMetaTagSet;

        ConsoleKeyInfo _consoleKeyInfo;

        public CampaignNavigator(long vdsLocationId, long vdsMetaFileId, string currentPath)
        {
            _vdsLocationId = vdsLocationId;
            _vdsMetaFileId = vdsMetaFileId;
            _currentPath = currentPath;

            _groupId = -1;

            base.Start();
        }

        protected override void OnInitialize()
        {
            if (H5A.exists(_vdsLocationId, "tag_set") > 0)
            {
                _vdsTagSet = IOHelper.ReadAttribute<hdf_tag_t>(_vdsLocationId, "tag_set").ToList();
            }
            else
            {
                _vdsTagSet = new List<hdf_tag_t>();
            }

            if (IOHelper.CheckLinkExists(_vdsMetaFileId, _currentPath))
            {
                _groupId = H5G.open(_vdsMetaFileId, _currentPath);
                _description = string.Empty;

                if (H5A.exists(_groupId, "description") > 0)
                {
                    _description = IOHelper.ReadAttribute<string>(_groupId, "description").First();
                }

                if (H5A.exists(_groupId, "tag_set") > 0)
                {
                    _vdsMetaTagSet = IOHelper.ReadAttribute<hdf_tag_t>(_groupId, "tag_set").ToList();
                }
                else
                {
                    _vdsMetaTagSet = new List<hdf_tag_t>();
                }

                H5G.close(_groupId);
            }
            else
            {
                _vdsMetaTagSet = new List<hdf_tag_t>();
            }
        }

        protected override void OnRedraw()
        {
            Console.Clear();

            if (!string.IsNullOrWhiteSpace(_description))
            {
                Console.WriteLine($"\"{_description}\"\n");
                Console.WriteLine();
            }

            _baseOffset = Console.CursorTop;

            Console.WriteLine($"Suggested tags ({_vdsTagSet.Count()}):\n");

            _vdsTagSet.ForEach(x => Console.WriteLine($"[ ] {x.date_time.Replace("T", " "),-20} | {x.name,-15} | {x.mode,-15} | {x.comment}"));

            Console.WriteLine();
            Console.WriteLine($"Actual tags ({_vdsMetaTagSet.Count()}):\n");

            _vdsMetaTagSet.ForEach(x => Console.WriteLine($"[ ] {x.date_time.Replace("T", " "),-20} | {x.name,-15} | {x.mode,-15} | {x.comment}"));

            Console.WriteLine();
            Console.WriteLine("---------------------------------");
            Console.WriteLine(" navigation: arrow up, arrow down");
            Console.WriteLine("       edit: arrow right, enter");
            Console.WriteLine("       exit: arrow left, ESC");
            Console.WriteLine("        new: n");
            Console.WriteLine("       copy: c");
            Console.WriteLine("description: d");
            Console.WriteLine("       move: page up, page down");
            Console.WriteLine("     delete: del");
        }

        protected override bool OnWaitForUserInput()
        {
            _totalCount = _vdsTagSet.Count() + _vdsMetaTagSet.Count();

            if (this.SelectedIndex < 0)
            {
                this.SelectedIndex = _totalCount > 0 ? 0 : -1;
            }
            else if (this.SelectedIndex >= _totalCount)
            {
                this.SelectedIndex = _totalCount - 1;
            }

            if (this.SelectedIndex >= _vdsTagSet.Count())
            {
                _offset = _baseOffset + 3;
            }
            else
            {
                _offset = _baseOffset + 0;
            }

            if (this.LastIndex >= 0)
            {
                Console.SetCursorPosition(1, this.LastIndex);
                Console.Write(" ");
            }

            if (this.SelectedIndex >= 0)
            {
                Console.SetCursorPosition(1, this.SelectedIndex + _offset + 2);
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

                    if (this.SelectedIndex >= _vdsTagSet.Count())
                    {
                        int index = this.SelectedIndex - _vdsTagSet.Count();
                        _vdsMetaTagSet[index] = Program.PromptTagData(_vdsMetaTagSet[index]);
                        _updateAttribute = true;
                    }

                    break;

                case ConsoleKey.N:

                    _vdsMetaTagSet.Add(Program.PromptTagData(new hdf_tag_t()));
                    this.SelectedIndex = _totalCount;
                    _updateAttribute = true;

                    break;

                case ConsoleKey.C:

                    if (this.SelectedIndex >= 0)
                    {
                        _vdsMetaTagSet.Add(_vdsMetaTagSet[this.SelectedIndex]);
                        this.SelectedIndex = _totalCount;
                        this.OnRedraw();
                    }

                    break;

                case ConsoleKey.D:

                    Console.CursorVisible = true;
                    Console.Clear();
                    Console.Write($"\"{_description}\"\n");
                    Console.WriteLine();
                    Console.Write($"New description:\n");
                    _description = Console.ReadLine();
                    Console.CursorVisible = false;

                    _updateDescription = true;

                    break;

                case ConsoleKey.Delete:

                    if (this.SelectedIndex >= _vdsTagSet.Count())
                    {
                        Console.Clear();
                        Console.Write("The selected item will be deleted. Proceed (Y/N)? ");

                        if (Console.ReadKey().Key == ConsoleKey.Y)
                        {
                            _vdsMetaTagSet.RemoveAt(this.SelectedIndex - _vdsTagSet.Count());
                            _updateAttribute = true;
                        }

                        this.OnRedraw();

                        break;
                    }

                    break;

                case ConsoleKey.PageUp:

                    if (this.SelectedIndex >= _vdsTagSet.Count() + 1 && this.SelectedIndex < _totalCount)
                    {
                        int index = this.SelectedIndex - _vdsTagSet.Count();
                        hdf_tag_t hdf_tag = _vdsMetaTagSet[index];
                        _vdsMetaTagSet.RemoveAt(index);
                        _vdsMetaTagSet.Insert(index - 1, hdf_tag);
                        this.SelectedIndex -= 1;
                        _updateAttribute = true;
                    }

                    break;

                case ConsoleKey.PageDown:

                    if (this.SelectedIndex >= _vdsTagSet.Count() && this.SelectedIndex < _totalCount - 1)
                    {
                        int index = this.SelectedIndex - _vdsTagSet.Count();
                        hdf_tag_t hdf_tag = _vdsMetaTagSet[index];
                        _vdsMetaTagSet.RemoveAt(index);
                        _vdsMetaTagSet.Insert(index + 1, hdf_tag);
                        this.SelectedIndex += 1;
                        _updateAttribute = true;
                    }

                    break;

                case ConsoleKey.Escape:
                case ConsoleKey.LeftArrow:
                    return true;
            }

            // update description
            if (_updateDescription)
            {
                _groupId = IOHelper.OpenOrCreateGroup(_vdsMetaFileId, _currentPath).GroupId;
                IOHelper.PrepareAttribute(_groupId, "description", new string[] { _description }, new ulong[] { 1 }, true);
                _updateDescription = false;
                this.OnRedraw();
            }

            // update attribute
            if (_updateAttribute)
            {
                _groupId = IOHelper.OpenOrCreateGroup(_vdsMetaFileId, _currentPath).GroupId;
                IOHelper.PrepareAttribute(_groupId, "tag_set", _vdsMetaTagSet.ToArray(), new ulong[] { (ulong)_vdsMetaTagSet.Count() }, true);
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
