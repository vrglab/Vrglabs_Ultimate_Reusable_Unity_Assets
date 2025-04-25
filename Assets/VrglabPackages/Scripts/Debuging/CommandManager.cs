using ConsoleAppEngine.PremadeCommands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace ConsoleAppEngine
{
    public class CommandManager
    {
        public static void ProcessCommands(string[] args)
        {
            bool foundCommand = false;
            foreach (var item in GetTypesMarkedWithAttrib(typeof(CommandAttribute)))
            {
                var object_Created = (ICommand)item.GetConstructors()[0].Invoke(new object[] { });
                var attribute = (CommandAttribute)item.GetCustomAttribute(typeof(CommandAttribute));

                if (args != null)
                {
                    if (args.Length > 0)
                    {
                        if (args[0] == attribute.Name)
                        {
                            foundCommand = true;
                            List<string> commandArgs = new List<string>();
                            for (int i = 1; i < args.Length; i++)
                            {
                                if (!args[i].StartsWith("--"))
                                {
                                    commandArgs.Add(args[i]);
                                }
                            }

                            if (commandArgs.Count < attribute.requiredInputAmount)
                            {
                                UnityEngine.Debug.LogError("Required command field's not provided");
                                return;
                            }

                            List<KeyValuePair<string, string>> commandOptions = new List<KeyValuePair<string, string>>();
                            for (int i = 1; i < args.Length; i++)
                            {
                                if (args[i].StartsWith("--"))
                                {
                                    string[] splitOption = args[i].Split('=');
                                    string optionName = splitOption[0].Replace("--", "");
                                    if (attribute.validOptions.Contains(optionName))
                                    {
                                        try
                                        {
                                            commandOptions.Add(new KeyValuePair<string, string>(optionName, splitOption[1]));
                                        }
                                        catch (Exception e)
                                        {
                                            commandOptions.Add(new KeyValuePair<string, string>(optionName, ""));
                                        }
                                    }
                                    else
                                    {
                                        UnityEngine.Debug.LogError($"Option \"{args[i]}\" is not a valid option");
                                        return;
                                    }
                                }
                            }
                            object_Created.Execute(commandArgs.ToArray(), commandOptions.ToArray());
                            return;
                        }
                    }
                }
            }

            if (foundCommand == false && args.Length > 0)
            {
                HelpCommand helpCommand = new HelpCommand();
                UnityEngine.Debug.LogError($"{args[0]} is not a valid command");
                helpCommand.Execute(new string[] { }, new KeyValuePair<string, string>[] { });
            }
            else
            {
                HelpCommand helpCommand = new HelpCommand();
                UnityEngine.Debug.LogError("No command provided");
                helpCommand.Execute(new string[] { }, new KeyValuePair<string, string>[] { });
            }
        }

        public static void ProcessCommands(string command)
        {
            ProcessCommands(ParseCommand(command));
        }


        static string[] ParseCommand(string input)
        {
            // Use regex to split the input while keeping quoted strings together
            var matches = Regex.Matches(input, @"[\""].+?[\""]|[^ ]+");

            var parts = new List<string>();
            foreach (Match match in matches)
            {
                // Remove the surrounding quotes if present
                string part = match.Value;
                if (part.StartsWith("\"") && part.EndsWith("\""))
                {
                    part = part.Substring(1, part.Length - 2);
                }
                parts.Add(part);
            }

            return parts.ToArray();
        }


        public static List<Type> GetTypesMarkedWithAttrib(Type TestType)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            List<Type> types = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                var classesWithAttribute = assembly.GetTypes()
                    .Where(type => type.GetCustomAttributes(TestType, true).Length > 0)
                    .ToList();
                foreach (var classType in classesWithAttribute)
                {
                    types.Add(classType);
                }
            }
            return types;
        }

        public static List<Type> GetTypesMarkedWithAttrib(Type TestType, Assembly assembly)
        {
            List<Type> types = new List<Type>();
            var classesWithAttribute = assembly.GetTypes()
                .Where(type => type.GetCustomAttributes(TestType, true).Length > 0)
                .ToList();
            foreach (var classType in classesWithAttribute)
            {
                types.Add(classType);
            }
            return types;
        }
    }
}
