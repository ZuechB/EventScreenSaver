using System;
using System.Windows;
using System.IO;

namespace ScreenSaver
{
	public class Enigma
	{
		[STAThread]
		static void Main(string[] args) 
		{
            if (!Directory.Exists(@"c:\screen\"))
            {
                Directory.CreateDirectory(@"c:\screen\");
            }

            //if (!File.Exists(@"c:\screen\edit.txt"))
            //{
            //    var write = new StreamWriter(@"c:\screen\edit.txt");
            //    write.WriteLine("Days till ship");
            //    write.WriteLine("4/6/2015");
            //    write.WriteLine("Ship now");
            //    write.Close();
            //}


			if (args.Length > 0)
			{
				if (args[0].ToLower().Trim().Substring(0,2) == "/c")
				{
                    var setting = new Settings();
                    setting.ShowDialog();
				}
				else if (args[0].ToLower() == "/s")
				{
                    var mainWin = new MainWindow();
                    mainWin.ShowDialog();
				}
			}
			else
			{
                var mainWin = new MainWindow();
                mainWin.ShowDialog();
			}
		}
	}
}
