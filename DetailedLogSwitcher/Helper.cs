using static System.Console;

namespace DetailedLogSwitcher
{
    public class Helper
    {
        internal static void Pause() => ReadKey(true);

        internal static void ShowError(string message)
        {
            WriteLine(message);
            Pause();
        }
    }
}