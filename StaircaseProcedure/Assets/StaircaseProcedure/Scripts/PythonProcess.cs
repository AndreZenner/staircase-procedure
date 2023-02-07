using System.Diagnostics;

public class PythonProcess
{
    private string pythonPath;
    private Process p;

    public PythonProcess(string pythonPath)
    {
        this.pythonPath = pythonPath;
    }

    public void StartProcess(string args)
    {
        p = new Process();
        p.StartInfo.FileName = pythonPath;
        p.StartInfo.Arguments = args;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.CreateNoWindow = true;
        p.Start();
    }
}
