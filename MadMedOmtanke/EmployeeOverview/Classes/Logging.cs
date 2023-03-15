using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EmployeeOverview.Classes
{
    internal class Logging
    {
        internal string logFilePath = $"{Environment.CurrentDirectory}\\log\\EmployeeLog.txt";

        internal void Log(string LogMessage, ColorsForLog color)
        {
            List<string> filecontent = new();
            if (!Directory.Exists(logFilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
            }

            DateTime dateTime = DateTime.Now;
            LogMessage = $"{(int)color}; {LogMessage}; {dateTime.ToString("dd/MMM/yyyy")} \n";
            if (File.Exists(logFilePath))
            {
                filecontent = ReadFile();
            }
            filecontent.Add(LogMessage);
            File.WriteAllLines(logFilePath, filecontent);
        }

        /// <summary>
        /// Reads the file at converts it into a list
        /// </summary>
        /// <returns>Returns a list of string containing all the lines from the file.</returns>
        internal List<string> ReadFile()
        {
            List<string> filecontent = new();
            filecontent = File.ReadAllLines(logFilePath).ToList();
            filecontent = filecontent.Where(x => x != string.Empty).ToList();
            return filecontent;
        }

        /// <summary>
        /// Clears the log view and make a marking in the file that is have been cleared.
        /// </summary>
        internal void ClearLog()
        {
            File.AppendAllText(logFilePath, "Cleared\n");
        }

        /// <summary>
        /// Updates the log view in the menu with latest logings by looking at the markings in the file.
        /// </summary>
        /// <param name="filecontent"></param>
        internal List<string> UpdateLogList(List<string> filecontent)
        {
            if (filecontent.Any(x => x.Contains("Cleared")))
            {
                int lastindex = filecontent.LastIndexOf("Cleared");
                for (int i = lastindex; i >= 0; i--)
                {
                    filecontent.RemoveAt(i);
                }
            }
            return filecontent;
        }
    }
}
