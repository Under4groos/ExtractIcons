using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace ExtractIcons
{

    public class ExtractExe
    {
        [DllImport("User32.dll")]
        internal static extern uint PrivateExtractIcons(
            string szFileName, int nIconIndex, int cxIcon,
            int cyIcon, IntPtr[] phicon, uint[] piconid,
            uint nIcons, uint flags);
        public Size Size { get; set; } = new Size(64, 64);
        public string Path_File { get; set; }
        public string Path_Save_File { get; set; }
        public void Extract()
        {
            uint _nIcons = PrivateExtractIcons(Path_File, 0, 0, 0, null, null, 0, 0);
            IntPtr[] phicon = new IntPtr[_nIcons];
            uint[] piconid = new uint[_nIcons];
            uint nIcons = PrivateExtractIcons(Path_File, 0, Size.Width, Size.Height, phicon, piconid, 1, 0);
            for (int i = 0; i < nIcons; i++)
            {
                using (Icon icon = Icon.FromHandle(phicon[i]))
                {
                    Bitmap bitmap = icon.ToBitmap();
                    if (File.Exists(Path_Save_File))
                        File.Delete(Path_Save_File);
                    if (Path_Save_File == null)
                    {
                        Path_Save_File = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())).Remove(5) + ".png";
                    }

                    bitmap.Save(Path_Save_File, ImageFormat.Png);
                    Console.WriteLine($"Save: {Path_Save_File}");
                }

            }
        }
    }
    internal class Program
    {
        static ExtractExe extractExe = new ExtractExe();
        static string[] ARGS = { };
        static void Main(string[] args)
        {
            Console.WriteLine("\"C:\\myexe.exe\" 64x64 \"C:\\myexe.png\"");
            Console.WriteLine("=========================================");
            while (true)
            {
                ARGS = Console.ReadLine().Split(',');
                if (ARGS.Length == 3)
                {
                    extractExe.Path_File = ARGS[0].Replace("\"", "");
                    extractExe.Path_Save_File = ARGS[2].Replace("\"", "");
                    if (!File.Exists(extractExe.Path_File))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("File.Exists == false");
                        continue;
                    }

                    bool b = Regex.IsMatch(ARGS[1], "[0-9]+?x[0-9]+");
                    if (b)
                    {
                        string[] arg_ = Regex.Match(ARGS[1], "[0-9]+?x[0-9]+").Value.ToLower().Split('x');

                        Size size = new Size();
                        int si_ = 0;

                        if (int.TryParse(arg_[0], out si_))
                        {
                            size.Width = si_;
                        }
                        if (int.TryParse(arg_[1], out si_))
                        {
                            size.Height = si_;
                        }

                        extractExe.Size = size;
                        extractExe.Extract();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Size error");
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ERROR!");
                    Console.WriteLine("ARGS.Length != 3");
                }

            }




        }

    }
}
