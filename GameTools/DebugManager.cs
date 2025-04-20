using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Unity.VisualScripting;
public static class DebugManager
{
    private static readonly List<string> debugLogs = new List<string>();
    private static readonly string sessionStart = "Debug report session " + DateTime.Now;

    static DebugManager()
    {
        debugLogs.Add(sessionStart);
    }

    public static void LogDebugStatement(string report)
    {
        debugLogs.Add(report);
    }

    public static void WriteDebugSessionLog(bool overwrite)
    {
        string filePath = Application.streamingAssetsPath + "/DebugSessionLog.txt";

        try
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            bool fileExists = File.Exists(filePath);

            if (overwrite || !fileExists)
            {
                File.WriteAllLines(filePath, debugLogs);
            }
            else
            {
                File.AppendAllLines(filePath, debugLogs);
            }
        }
        catch (Exception e)
        {
            Report.Write($"Failed to write debug log: {e.Message}");
        }
    }
}