using System.Diagnostics;

public class PythonProcess
{
    private string pythonPath;

    public PythonProcess(string pythonPath)
    {
        this.pythonPath = pythonPath;
    }

    public void StartProcess(string args)
    {
        Process p = new Process();
        p.StartInfo.FileName = pythonPath;
        p.StartInfo.Arguments = args;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = false;
        p.StartInfo.CreateNoWindow = true;
        p.Start();
    }
}
