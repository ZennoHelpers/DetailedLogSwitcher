using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using static System.Console;

namespace DetailedLogSwitcher
{
    internal static class Program
    {
        private static void Pause() => ReadKey(true);

        private static void Configure(List<string> paths, bool newState)
        {
            WriteLine("Установка значения...");
            foreach (var path in paths)
            {
                string json = File.ReadAllText(path);
                Config[] cfg = JsonConvert.DeserializeObject<Config[]>(json);

                foreach (Config c in cfg)
                {
                    if (c.ClnName == "DetailedLogEnabled" && c.ClnSubGroup == "Log")
                    {
                        string newStateStr = newState.ToString();
                        
                        if (c.ClnValue == newStateStr)
                        {
                            WriteLine("Настройка уже включена.");
                            return;
                        }
                        else c.ClnValue = newStateStr;
                    }
                }

                string output = JsonConvert.SerializeObject(cfg, Formatting.Indented);
                File.WriteAllText(path, output);
            }
            WriteLine("Значение установлено.");
        }

        private static void Main(string[] args)
        {
            try
            {
                WriteLine("Проверка наличия конфигов\n");

                string appDataPath = Environment.GetEnvironmentVariable("APPDATA"); // Папка %APPDATA%

                string[] versions = {"5", "7"};

                List<string> existConfigs = new List<string>();

                foreach (string ver in versions)
                {
                    string path = $@"{appDataPath}\ZennoLab\ZennoPoster\{ver}\Settings\globalsettings.settings.json";
                    bool isExist = File.Exists(path);

                    if (isExist)
                    {
                        existConfigs.Add(path);
                        WriteLine($@"Конфиг {ver} версии найден.");
                    }
                }

                if (existConfigs.Count == 0)
                {
                    WriteLine("Ни один из конфигов не найден!");
                    Pause();
                    return;
                }

                for (int i = 1;; i++)
                {
                    WriteLine("\nУправление:\n1 - включить\n2 - выключить\nq - выйти");
                    Write("\nВвод: ");
                    ConsoleKeyInfo keyInfo = ReadKey();
                    WriteLine();

                    if (keyInfo.Key == ConsoleKey.Q) return;
                    else if (keyInfo.Key == ConsoleKey.D1)
                    {
                        Configure(existConfigs, true);
                        break;
                    }
                    else if (keyInfo.Key == ConsoleKey.D2)
                    {
                        Configure(existConfigs, false);
                        break;
                    }
                    else if (i == 5)
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