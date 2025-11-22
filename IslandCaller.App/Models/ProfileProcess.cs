using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IslandCaller.App.Models
{
    public class ProfileProcess
    {
        public static void MigrationToCSV(string txtPath)
        {
            Log.WriteLog("ProfileProcess.cs - MigrationToCSV", "Debug", "Start to Converte TXT to CSV");
            string roamingdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string csvPath = Path.Combine(roamingdata, "IslandCaller", "Profile", "Default.csv");
            Log.WriteLog("ProfileProcess.cs - MigrationToCSV", "Debug", $"csvPath : {csvPath}");
            Directory.CreateDirectory(Path.GetDirectoryName(csvPath));
            try
            {
                string[] lines = File.ReadAllLines(txtPath);
                HashSet<string> seen = new HashSet<string>();  // 用于去重
                int index = 1;

                using (StreamWriter writer = new StreamWriter(csvPath))
                {
                    // 写入表头
                    writer.WriteLine("ID,Name,Gender");

                    foreach (string rawLine in lines)
                    {
                        string line = rawLine.Trim();

                        // 跳过空行和重复行
                        if (string.IsNullOrEmpty(line) || seen.Contains(line))
                            continue;

                        seen.Add(line);
                        string escaped = line.Replace("\"", "\"\"");  // 转义双引号
                        writer.WriteLine($"{index},\"{escaped}\",0");
                        index++;
                    }
                }

                Log.WriteLog("ProfileProcess.cs - MigrationToCSV", "Success", $"Succeed convertion");
            }
            catch (Exception ex)
            {
                Log.WriteLog("ProfileProcess.cs - MigrationToCSV", "Error", $"Failed convertion ,error : {ex.Message}");
            }
        }
        public static void CreateDemoCsv()
        {
            Log.WriteLog("MigrationCSV.cs - CreateDemoCsv", "Debug", "Start to Create Default Csv");
            string roamingdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string csvPath = Path.Combine(roamingdata, "IslandCaller", "Profile", "Default.csv");
            Log.WriteLog("MigrationCSV.cs - CreateDemoCsv", "Debug", $"csvPath : {csvPath}");
            Directory.CreateDirectory(Path.GetDirectoryName(csvPath));
            try
            {
                int index = 1;

                using (StreamWriter writer = new StreamWriter(csvPath))
                {
                    // 写入表头
                    writer.WriteLine("ID,Name,Gender");
                    string line = "小明";
                    string escaped = line.Replace("\"", "\"\"");  // 转义双引号
                    writer.WriteLine($"{index},\"{escaped}\",0");
                    index++;
                    line = "李明";
                    escaped = line.Replace("\"", "\"\"");  // 转义双引号
                    writer.WriteLine($"{index},\"{escaped}\",0");
                    index++;
                    line = "李华";
                    escaped = line.Replace("\"", "\"\"");  // 转义双引号
                    writer.WriteLine($"{index},\"{escaped}\",1");
                }

                Log.WriteLog("MigrationCSV.cs - CreateDemoCsv", "Success", $"Succeed creation");
            }
            catch (Exception ex)
            {
                Log.WriteLog("MigrationCSV.cs - CreateDemoCsv", "Error", $"Failed creation ,error : {ex.Message}");
            }
        }
        public static void EditProfile(Guid guid)
        {
            Log.WriteLog("ProfileProcess.cs - EditProfile", "Debug", "Start to Edit Csv");
            string roamingdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string csvPath = Path.Combine(roamingdata, "IslandCaller", "Profile", Settings.Instance.Profile.ProfileList[guid]);
            csvPath += ".csv";
            Log.WriteLog("ProfileProcess.cs - EditProfile", "Debug", $"csvPath : {csvPath}");

            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "notepad.exe",
                    Arguments = $"\"{csvPath}\"",
                    UseShellExecute = true
                });
                Log.WriteLog("ProfileProcess.cs - EditProfile", "Success", "Opened CSV with Notepad");
            }
            catch (Exception ex)
            {
                Log.WriteLog("ProfileProcess.cs - EditProfile", "Error", $"Failed to open CSV, error: {ex.Message}");
            }
        }
    }
}