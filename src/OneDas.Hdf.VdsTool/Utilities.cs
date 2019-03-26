using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace OneDas.Hdf.VdsTool
{
    public static class Utilities
    {
        public static void RemoveCharacters(int count)
        {
            for (int i = 0; i <= count - 1; i++)
            {
                Console.Write($"{ '\u0008' } { '\u0008' }");
            }
        }

        public static (string Value, bool IsEscaped) ReadLine(List<string> optionSet)
        {
            Contract.Requires(optionSet != null);

            ConsoleKeyInfo consoleKeyInfo = default;
            StringBuilder buffer = default;
            int selectedIndex = 0;

            //
            optionSet.Insert(0, string.Empty);
            buffer = new StringBuilder();

            while (true)
            {
                consoleKeyInfo = Console.ReadKey(true);

                switch (consoleKeyInfo.Key)
                {
                    case ConsoleKey.UpArrow:

                        if (optionSet.Count > 1)
                        {
                            selectedIndex = (selectedIndex + 1) % optionSet.Count;

                            Utilities.RemoveCharacters(buffer.Length);
                            buffer.Clear();

                            Console.Write(optionSet[selectedIndex]);
                            buffer.Append(optionSet[selectedIndex]);
                        }

                        break;

                    case ConsoleKey.DownArrow:

                        if (optionSet.Count > 1)
                        {
                            if (selectedIndex == 0)
                            {
                                selectedIndex = optionSet.Count;
                            }

                            selectedIndex -= 1;

                            Utilities.RemoveCharacters(buffer.Length);
                            buffer.Clear();

                            Console.Write(optionSet[selectedIndex]);
                            buffer.Append(optionSet[selectedIndex]);

                        }

                        break;

                    case ConsoleKey.Enter:

                        Console.WriteLine();

                        return (buffer.ToString(), false);

                    case ConsoleKey.Escape:

                        Utilities.RemoveCharacters(buffer.Length);
                        buffer.Clear();

                        return (string.Empty, true);

                    case ConsoleKey.Backspace:

                        if (buffer.Length > 0)
                        {
                            buffer.Remove(buffer.Length - 1, 1);
                            Utilities.RemoveCharacters(1);
                        }

                        optionSet[0] = buffer.ToString();
                        selectedIndex = 0;

                        break;

                    default:

                        if (!char.IsControl(consoleKeyInfo.KeyChar))
                        {
                            buffer.Append(consoleKeyInfo.KeyChar);
                            Console.Write(consoleKeyInfo.KeyChar);
                        }

                        optionSet[0] = buffer.ToString();
                        selectedIndex = 0;

                        break;
                }
            }
        }
    }
}
