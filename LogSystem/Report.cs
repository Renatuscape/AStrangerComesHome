using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class Report
{
    internal static void Write(
    string input,
    [CallerFilePath] string sourceFilePath = "",
    [CallerMemberName] string memberName = "")
    {
        // Extract the class name from the file path
        string className = Path.GetFileNameWithoutExtension(sourceFilePath);

        string formattedEntry = $"\n{DateTime.Now} - {className}.{memberName} | {input}";
        Debug.Log(formattedEntry);
        DebugManager.LogDebugStatement(formattedEntry);
    }

    internal static void WriteError(
    string input,
    [CallerFilePath] string sourceFilePath = "",
    [CallerMemberName] string memberName = "")
    {
        // Extract the class name from the file path
        string className = Path.GetFileNameWithoutExtension(sourceFilePath);

        string formattedEntry = $"\n{DateTime.Now} - {className}.{memberName} | ERROR! | {input}";
        Debug.LogError(formattedEntry);
        DebugManager.LogDebugStatement(formattedEntry);
    }

    internal static void WriteWarning(
    string input,
    [CallerFilePath] string sourceFilePath = "",
    [CallerMemberName] string memberName = "")
    {
        // Extract the class name from the file path
        string className = Path.GetFileNameWithoutExtension(sourceFilePath);

        string formattedEntry = $"\n{DateTime.Now} - {className}.{memberName} | WARNING | {input}";
        Debug.LogWarning(formattedEntry);
        DebugManager.LogDebugStatement(formattedEntry);
    }
}