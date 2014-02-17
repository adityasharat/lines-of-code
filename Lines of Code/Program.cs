using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Lines_of_Code
{
    class Program
    {
        private static List<string> paramListing = new List<string>(
                new string[]
                {
                    "-m", "-d", "-e"
                }
            );

        private static List<int> modeListing = new List<int>(
                new int[]
                {
                    0, 1
                }
            );

        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                Console.WriteLine("No arguments passed.");
                return;
            }

            if (args.Length == 1 && args[0] == "--help")
            {
                Console.WriteLine("   -m  mode\tset mode of the algorithm. 0 to add empty lines, 1 to ignore them.");
                Console.WriteLine("   -d  directory\tpath of the directory of the source files.");
                Console.WriteLine("   -e  file-extension\textension of the file(s) to read.");
                Console.WriteLine("   --help\thelp text.");
                Console.WriteLine("\n");
                return;
            }

            Dictionary<string, string> argList = new Dictionary<string, string>();

            for (int i = 0; i < args.Length; i += 2)
            {
                if (!paramListing.Contains(args[i]))
                {
                    Console.WriteLine("Invalid parameter: " + args[i] + ". For help do --help");
                    return;
                }
                else
                {
                    try
                    {
                        if (args[i + 1] == null || args[i + 1].Equals(string.Empty))
                        {
                            Console.WriteLine("No argument passed for " + args[i] + " parameter");
                            return;
                        }

                        argList.Add(args[i], args[i + 1]);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Console.WriteLine("No argument passed for " + args[i] + " parameter");
                        return;
                    }
                }
            }

            Console.WriteLine(evaluate(argList));

        }

        private static string evaluate(Dictionary<string, string> argList)
        {
            if (!argList.ContainsKey("-m"))
                return "Error: Required Parameter -m missing";

            int mode = 0;

            try
            {
                mode = Convert.ToInt32(argList["-m"]);
            }
            catch (Exception)
            {
                return "Error: Mode must be 0 or 1";
            }

            if (!modeListing.Contains(mode))
            {
                return "Error: Illegal parameter -m: " + mode;
            }

            if (!argList.ContainsKey("-d"))
                return "Error: Required Parameter -d missing";

            string dir = "";

            try
            {
                dir = argList["-d"];
            }
            catch (Exception)
            {
                return "Error: No directory specified";
            }

            try
            {
                dir = dir.Replace("\"", string.Empty);
            }
            catch (Exception)
            {
                return "Error: No directory specified";
            }


            if (!argList.ContainsKey("-e"))
                return "Error: Required Parameter -e missing";

            string ext;

            try
            {
                ext = argList["-e"];
            }
            catch (Exception)
            {
                return "Error: Required Parameter -e missing";
            }

            try
            {
                ext = ext.Replace("\"", string.Empty);
            }
            catch (Exception)
            {
                return "Error: Invalid Extension specified";
            }

            string[] allFiles = null;
            try
            {
                allFiles = Directory.GetFiles(@dir, "*." + ext, SearchOption.AllDirectories);
            }
            catch (ArgumentNullException)
            {
                return "Error: Directory path null.";
            }
            catch (UnauthorizedAccessException)
            {
                return "Error: Access Denied";
            }
            catch (SecurityException)
            {
                return "Error: Access Denied.";
            }
            catch (ArgumentOutOfRangeException)
            {
                return "Error: Out of Range.";
            }
            catch (ArgumentException)
            {
                return "Error: Invalid extension(s).";
            }
            catch (DirectoryNotFoundException)
            {
                return "Error: Directory Not Found.";
            }
            catch (PathTooLongException)
            {
                return "Error: Path Too Long.";
            }
            catch (IOException)
            {
                return "Error: IO exception";
            }

            if (allFiles.Length == 0)
            {
                return "No files with " + ext + " extension found";
            }

            Console.WriteLine("\n************** Lines of Code **************\n");
            Console.WriteLine("\tTotal number of files found: " + allFiles.Length + "\n");

            long totalLOC = 0;
            long fileErrors = 0;

            foreach (string file in allFiles)
            {
                try
                {
                    totalLOC += loc(file, mode);
                }
                catch (Exception)
                {
                    fileErrors++;
                }
            }

            if (fileErrors != 0)
            {
                Console.WriteLine("\t" + fileErrors + " files could not be read.\n");
            }

            return "\tTotal lines of code: " + totalLOC.ToString();
        }

        private static long loc(string file, int mode)
        {
            if (mode == 0 || mode == 1)
            {
                return (long)File.ReadLines(file).Count();
            }
            return 0;
        }
    }
}