using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Win32;
using Newtonsoft.Json;
using static System.Console;
using static DetailedLogSwitcher.Result;

namespace DetailedLogSwitcher
{
    public static class SearchConfigs
    {
        internal static Result SearchNLogs(out List<string> nLogPaths)
        {
            WriteLine("Получение установленных программ...\n");

            nLogPaths = new List<string>();

            using RegistryKey zlKey = Registry.CurrentUser.OpenSubKey(@"Software\ZennoLab");
            if (zlKey != null)
            {
                string[] langs = {"RU", "EN", "CN"};
                foreach (string lang in langs)
                {
                    using RegistryKey langKey = zlKey.OpenSubKey(lang);
                    if(langKey == null) continue;

                    string[] products = langKey.GetSubKeyNames();
                    foreach (string product in products)
                    {
                        using RegistryKey productKey = langKey.OpenSubKey(product);
                        if(productKey == null) continue;

                        string[] versions = productKey.GetSubKeyNames();
                        foreach (string ver in versions)
                        {
                            using RegistryKey verKey = productKey.OpenSubKey(ver);

                            if(verKey == null || (string)verKey.GetValue("SuccessInstall") != "True") continue;

                            string productPath = (string)verKey.GetValue("InstallDir");
                            if (!string.IsNullOrEmpty(productPath))
                            {
                                string nLogPath = productPath + @"\Progs\NLog.config";
                                if (File.Exists(nLogPath))
                                {
                                    nLogPaths.Add(nLogPath);
                                    WriteLine(@"Найден NLog.config версии " + ver);
                                }
                                else
                                {
                                    WriteLine($"Продукт {product} найден в реестре, но NLog.config не найден по пути: " + nLogPath);
                                    continue;
                                }
                            }
                            else
                            {
                                WriteLine("Продукт найден в реестре, но InstallDir не установлен.");
                                continue;
                            }
                        }
                    }
                }
            }
            else
            {
                return Error(@"Ни одна программа не установлена. Не найден: HKCU\Software\ZennoLab");
            }

            if (nLogPaths.Count == 0)
                return Error("Ни один NLog.config не найден.");

            return Ok(nLogPaths);
        }

        internal static Result SearchGlobalSettings(out List<string> globalSettingsPaths)
        {
            WriteLine("\nПроверка наличия глобальных настроек...\n");

            globalSettingsPaths = new List<string>();

            string appDataPath = Environment.GetEnvironmentVariable("APPDATA");

            string[] versions = {"5", "7"};

            foreach (string ver in versions)
            {
                string path = $@"{appDataPath}\ZennoLab\ZennoPoster\{ver}\Settings\globalsettings.settings.json";

                if (File.Exists(path))
                {
                    globalSettingsPaths.Add(path);
                    WriteLine($@"Глобальный конфиг {ver} версии найден.");
                }
            }

            if (globalSettingsPaths.Count == 0)
                return Error("Ни один из глобальных конфигов не найден!");

            return Ok(globalSettingsPaths);

            #region

            // string zennoposterPath = Environment.GetEnvironmentVariable("ZennoPosterCurrentPath");
            //
            // if(string.IsNullOrEmpty(zennoposterPath))
            //     WriteLine("Переменная окружения ZennoPosterCurrentPath не установлена. Поиск путей по умолчанию...");
            //
            // string[] zennolabPaths = {@"C:\Program Files\ZennoLab", @"C:\Program Files (x86)\ZennoLab"};
            // string[] langs = {"EN", "RU", /*"CN"*/};
            //
            // foreach (string zlPath in zennolabPaths)
            // {
            //     foreach (string lang in langs)
            //     {
            //         string path = $@"{zlPath}\{lang}";
            //     }
            // }

            #endregion
        }
    }
}