using System;
using UnityEngine;

public static class Log
{
    internal static string Report {  get; private set; }
    internal static void Write(string input)
    {
        string formattedEntry = $"\n{DateTime.Now}| {input}";
        Debug.Log(formattedEntry);
        Report += formattedEntry;
    }

    internal static void ToConsole()
    {
        Debug.Log(Report);
    }

    internal static void WriteError(string input)
    {
        string formattedEntry = $"\n{DateTime.Now}| ERROR | {input}";
        Debug.LogError(formattedEntry);
        Report += formattedEntry;
    }
}