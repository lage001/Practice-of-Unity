using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class ProcessHelper
{
    public static Process StartProcess(string command, string param, string workDir = "")
    {
        return StartProcess(command, param, workDir, DataReceived, ErrorReceived);
    }

    public static Process StartProcess(
        string command,
        string param,
        string workDir,
        DataReceivedEventHandler dataReceived,
        DataReceivedEventHandler errorReceived
    )
    {
        Process ps = new Process
        {
            StartInfo =
            {
                FileName = command,
                Arguments = param,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = workDir,
            }
        };
        ps.OutputDataReceived += dataReceived;
        ps.ErrorDataReceived += errorReceived;
        ps.Start();
        ps.BeginOutputReadLine();
        ps.BeginErrorReadLine();

        return ps;
    }

    private static void DataReceived(object sender, DataReceivedEventArgs eventArgs)
    {
        if (eventArgs.Data != null) Debug.Log(eventArgs.Data);
    }

    private static void ErrorReceived(object sender, DataReceivedEventArgs eventArgs)
    {
        if (eventArgs.Data != null) Debug.LogError(eventArgs.Data);
    }
}