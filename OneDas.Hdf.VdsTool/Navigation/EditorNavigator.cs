using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OneDas.Hdf.IO;
using HDF.PInvoke;

namespace OneDas.Hdf.VdsTool.Navigation
{
    class EditorNavigator : NavigatorBase
    {
        List<Action<Dictionary<string,string>>> _actionSet;

        public EditorNavigator()
        {
            base.Start();
        }

        protected override void OnInitialize()
        {
            _actionSet = new List<Action<Dictionary<string, string>>>();

            _actionSet.Add((x) => this.Menu_1(x));
            _actionSet.Add((x) => this.Menu_2(x));

            this.SelectedIndex = 0;
        }

        protected override void OnRedraw()
        {
            Console.Clear();
            Console.WriteLine("[ ] edit group name");
            Console.WriteLine("[ ] edit variable name");
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

                    if (this.SelectedIndex < _actionSet.Count - 1)
                    {
                        this.LastIndex = this.SelectedIndex;
                        this.SelectedIndex += 1;
                    }
                    break;

                case ConsoleKey.Enter:
                case ConsoleKey.RightArrow:

                    if (this.SelectedIndex >= 0)
                    {
                        _actionSet[this.SelectedIndex].Invoke(this.PromptLocationSettings());
                        this.OnRedraw();
                    }

                    break;

                case ConsoleKey.Escape:
                case ConsoleKey.LeftArrow:
                    return true;
            }

            return false;
        }

        private void Menu_1(Dictionary<string, string> settings) // edit group name
        {
            long fileId;
            long groupId;

            string parentGroupPath;
            string oldGroupName;
            string newGroupName;

            List<string> filePathSet;

            //
            settings = this.PromptEditGroupNameData(settings);

            if (settings.ContainsKey("DirectoryPath"))
            {
                filePathSet = Directory.GetFiles(settings["DirectoryPath"], settings["SearchPattern"], settings["IncludeSubDirectories"] == "Yes" ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).ToList();
            }
            else
            {
                filePathSet = new List<string>() { settings["FilePath"] };
            }

            parentGroupPath = settings["ParentGroupPath"];
            oldGroupName = settings["OldGroupName"];
            newGroupName = settings["NewGroupName"];

            foreach (string filePath in filePathSet)
            {
                fileId = H5F.open(filePath, H5F.ACC_RDWR, 0);

                if (IOHelper.CheckLinkExists(fileId, parentGroupPath))
                {
                    groupId = H5G.open(fileId, parentGroupPath);

                    if (IOHelper.CheckLinkExists(groupId, oldGroupName))
                    {
                        H5L.move(groupId, oldGroupName, groupId, newGroupName);
                    }

                    // clean up
                    H5G.close(groupId);
                }

                // clean up
                H5F.close(fileId);
            }
        }

        private void Menu_2(Dictionary<string, string> settings) // edit variable name
        {
            long fileId;
            long groupId;

            string variableGroupPath;
            string attributeName;
            string[] attributeContentSet;

            List<string> filePathSet;

            //
            settings = this.PromptEditVariableNameData(settings);

            if (settings.ContainsKey("DirectoryPath"))
            {
                filePathSet = Directory.GetFiles(settings["DirectoryPath"], settings["SearchPattern"], settings["IncludeSubDirectories"] == "Yes" ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).ToList();
            }
            else
            {
                filePathSet = new List<string>() { settings["FilePath"] };
            }

            variableGroupPath = settings["VariableGroupPath"];
            attributeName = "name_set";
            attributeContentSet = new string[] { settings["VariableName"] };

            foreach (string filePath in filePathSet)
            {
                fileId = H5F.open(filePath, H5F.ACC_RDWR, 0);

                if (IOHelper.CheckLinkExists(fileId, variableGroupPath))
                {
                    groupId = H5G.open(fileId, variableGroupPath);

                    if (settings["AppendMode"] == "Append")
                    {
                        IOHelper.PrepareAttribute(groupId, attributeName, attributeContentSet, new ulong[] { H5S.UNLIMITED }, true);
                    }
                    else
                    {
                        IOHelper.WriteAttribute(groupId, attributeName, attributeContentSet);
                    }

                    // clean up
                    H5G.close(groupId);
                }

                // clean up
                H5F.close(fileId);
            }
        }

        public Dictionary<string, string> PromptLocationSettings()
        {
            var settings = new Dictionary<string, string>();
            string value;
            Uri uri;

            Console.CursorVisible = true;

            // file or directory path
            while (true)
            {
                Console.Clear();
                Console.Write($"Enter existing file or directory path: ");

                if (!string.IsNullOrWhiteSpace(value = Console.ReadLine()))
                {
                    if (Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out uri))
                    {
                        if (!uri.IsAbsoluteUri)
                        {
                            uri = new Uri(new Uri(Program.BaseDirectoryPath), uri);
                        }

                        if (uri.IsFile)
                        {
                            if (Directory.Exists(uri.LocalPath))
                            {
                                settings["DirectoryPath"] = uri.LocalPath;
                                break;
                            }
                            else if (File.Exists(uri.LocalPath))
                            {
                                settings["FilePath"] = uri.LocalPath;
                                break;
                            }
                        }
                    }
                }
            }

            // search pattern
            if (settings.ContainsKey("DirectoryPath"))
            {
                while (true)
                {
                    Console.Clear();
                    Console.Write($"Enter search pattern: ");

                    if (!string.IsNullOrWhiteSpace(value = Console.ReadLine()))
                    {
                        settings["SearchPattern"] = "*.*";

                        break;
                    }
                }
            }

            // sub directories
            if (settings.ContainsKey("DirectoryPath"))
            {
                while (!settings.ContainsKey("IncludeSubDirectories"))
                {
                    Console.Clear();
                    Console.Write($"Include sub directories? (Y)es (N)o: ");

                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.Y:
                            settings["IncludeSubDirectories"] = "Yes";
                            break;

                        case ConsoleKey.N:
                            settings["IncludeSubDirectories"] = "Yes";
                            break;

                        default:
                            break;
                    }
                }
            }

            //
            Console.CursorVisible = false;

            return settings;
        }

        public Dictionary<string, string> PromptEditGroupNameData(Dictionary<string, string> settings)
        {
            string value;

            Console.CursorVisible = true;

            // parent group path
            while (true)
            {
                Console.Clear();
                Console.Write($"Enter parent group path: ");

                if (!string.IsNullOrWhiteSpace(value = Console.ReadLine()))
                {
                    settings["ParentGroupPath"] = value;

                    break;
                }
            }

            // old group name
            while (true)
            {
                Console.Clear();
                Console.Write($"Enter old group name: ");

                if (!string.IsNullOrWhiteSpace(value = Console.ReadLine()))
                {
                    settings["OldGroupName"] = value;

                    break;
                }
            }

            // new group name
            while (true)
            {
                Console.Clear();
                Console.Write($"Enter new group name: ");

                if (!string.IsNullOrWhiteSpace(value = Console.ReadLine()))
                {
                    settings["NewGroupName"] = value;

                    break;
                }
            }

            //
            Console.CursorVisible = false;

            return settings;
        }

        public Dictionary<string, string> PromptEditVariableNameData(Dictionary<string, string> settings)
        {
            string value;

            Console.CursorVisible = true;

            // variable group path
            while (true)
            {
                Console.Clear();
                Console.Write($"Enter variable group path: ");

                if (!string.IsNullOrWhiteSpace(value = Console.ReadLine()))
                {
                    settings["VariableGroupPath"] = value;

                    break;
                }
            }

            // variable name
            while (true)
            {
                Console.Clear();
                Console.Write($"Enter new variable name: ");

                if (!string.IsNullOrWhiteSpace(value = Console.ReadLine()))
                {
                    settings["VariableName"] = value;

                    break;
                }
            }

            // append variable name?
            while (!settings.ContainsKey("AppendMode"))
            {
                Console.Clear();
                Console.Write($"(A)ppend or (O)verwrite new variable name? ");

                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.A:
                        settings["AppendMode"] = "Append";
                        break;

                    case ConsoleKey.O:
                        settings["AppendMode"] = "Overwrite";
                        break;

                    default:
                        break;
                }
            }

            //
            Console.CursorVisible = false;

            return settings;
        }
    }
}
