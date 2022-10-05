using System.Diagnostics;
public class Debug
{
    public static void Log(object Message)
    {
        Trace.WriteLine((string)Message);
    }
}
