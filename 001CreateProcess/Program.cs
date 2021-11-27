using System;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace CreateProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            Process.Start("notepad.exe");
            Console.WriteLine("Started a process using notepad.exe, press enter to continue...");

            Console.ReadLine();
            
            CreateFile();

            var process = new Process();
            process.StartInfo.FileName = "notepad.exe";
            process.StartInfo.Arguments = "SaveFile.txt";
            process.Start();

            Console.WriteLine("Waiting for user to close the notepad.");

            process.PriorityClass = ProcessPriorityClass.RealTime;
            process.WaitForExit();

            Console.WriteLine("User closed notepad, press enter to continue...");

            Console.ReadLine();

            // Close all notepad processes
            Console.WriteLine("Closing all processes with name 'notepad'");
            var processes = Process.GetProcesses();
            foreach (var p in processes)
            {
                Console.WriteLine(p.ProcessName);
                if (p.ProcessName == "notepad")
                {
                    p.Kill();
                }
            }

            Console.ReadLine();
        }

        static void CreateFile()
        {
            FileStream stream = File.Create("SaveFile.txt");
            byte[] data = Encoding.ASCII.GetBytes("# Write something in the next line... save and close the file.");
            stream.Write(data, 0, data.Length);
            stream.Close();
        }
    }
}
