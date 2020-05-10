using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using static System.Console;
using static DetailedLogSwitcher.Helper;
using static DetailedLogSwitcher.SearchConfigs;

namespace DetailedLogSwitcher
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                Result gsResult = SearchGlobalSettings(out List<string> globalSettingsPaths);
                switch(gsResult)
                {
                    case Error result:
                        ShowError(result.Message);
                        return;
                }

                Result nLogsResult = SearchNLogs(out List<string> existNLogs);
                switch(nLogsResult)
                {
                    case Error result:
                        ShowError(result.Message);
                        return;
                }

                for (int i = 1;; i++)
                {
                    WriteLine("\nУправление:\n1 - включить\n2 - выключить\nq - выйти");
                    Write("\nВвод: ");
                    ConsoleKeyInfo keyInfo = ReadKey();
                    WriteLine();

                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.Q:
                            return;
                        case ConsoleKey.D1: // Включение
                            Setup.ConfigureGlobalSettings(globalSettingsPaths, true);
                            Setup.ConfigureNLogs(existNLogs, NlogState.Trace);
                            return;
                        case ConsoleKey.D2: // Выключение
                            Setup.ConfigureGlobalSettings(globalSettingsPaths, false);
                            Setup.ConfigureNLogs(existNLogs, NlogState.Debug);
                            return;
                    }

                    if (i == 5)
                    {
                        WriteLine("\nМножественный неверный ввод. Завершение работы...");
                        Thread.Sleep(2000);
                        return;
                    }

                    Thread.Sleep(250);
                }
            }
            catch (Exception e)
            {
                WriteLine(e);
            }
            finally
            {
                Pause();
            }
        }
    }
}