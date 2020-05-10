using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Newtonsoft.Json;
using static System.Console;
using static DetailedLogSwitcher.NlogState;
using Formatting = Newtonsoft.Json.Formatting;

namespace DetailedLogSwitcher
{
    public static class NlogState
    {
        public const string Trace = "Trace";
        public const string Debug = "Debug";
    }

    public static class Setup
    {
        public static void ConfigureGlobalSettings(List<string> paths, bool newState)
        {
            foreach (string path in paths)
            {
                string json = File.ReadAllText(path);
                GlobalSettings[] cfg = JsonConvert.DeserializeObject<GlobalSettings[]>(json);

                foreach (GlobalSettings c in cfg)
                {
                    if (c.ClnName == "DetailedLogEnabled" && c.ClnSubGroup == "Log")
                    {
                        string newStateStr = newState.ToString();

                        if (c.ClnValue == newStateStr)
                        {
                            WriteLine($"Подробный лог в глобальной настройке уже {(newState ? "включён" : "выключен")} по пути: " + path);
                            continue;
                        }
                        else
                        {
                            c.ClnValue = newStateStr;
                            WriteLine("Включён подробный лог в глобальных настройках по пути: " + path);

                            string output = JsonConvert.SerializeObject(cfg, Formatting.Indented);
                            File.WriteAllText(path, output);
                            break;
                        }
                    }
                }
            }
        }

        public static void ConfigureNLogs(List<string> paths, string newState)
        {
            foreach (string path in paths)
            {
                XmlDocument doc = new XmlDocument {PreserveWhitespace = true};
                doc.Load(path);
                XPathNavigator navigator = doc.CreateNavigator();

                const string xPath = "a:nlog/a:rules/a:logger/@minlevel";

                XmlNamespaceManager manager = new XmlNamespaceManager(navigator.NameTable);
                manager.AddNamespace("a", "http://www.nlog-project.org/schemas/NLog.xsd");
                XPathExpression query = navigator.Compile(xPath);
                query.SetContext(manager);
                XPathNodeIterator nodes = navigator.Select(query);

                bool isEdited = false;

                if (nodes.Count != 0)
                {
                    while (nodes.MoveNext())
                    {
                        XPathNavigator nav = nodes.Current;
                        string val = nav.Value;
                        if(val == "Fatal") continue;
                        if (val != newState)
                        {
                            nav.SetValue(newState);
                            isEdited = true;
                        }
                    }

                    if (isEdited)
                    {
                        var writer = XmlWriter.Create(path, new XmlWriterSettings()
                        {
                            Indent = false,
                            Encoding = Encoding.Default,
                            //NewLineOnAttributes = false,
                            CloseOutput = true,
                            CheckCharacters = true,
                            //NewLineHandling = NewLineHandling.None,
                        });
                        using (writer) doc.Save(writer);

                        WriteLine("Настроен NLog.config по пути: " + path);
                    }
                    else
                    {
                        WriteLine("NLog.config уже настроен по пути: " + path);
                    }
                }
                else
                {
                    WriteLine($"Не были получены элементы NLog.config по пути: " + path);
                    continue;
                }
            }
        }
    }
}