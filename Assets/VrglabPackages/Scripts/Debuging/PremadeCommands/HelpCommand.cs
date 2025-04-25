using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppEngine.PremadeCommands
{
    [Command("help", 0, new string[] {"a"})]
    public class HelpCommand : ICommand
    {
        public void Execute(string[] args, KeyValuePair<string, string>[] options)
        {
            StringBuilder helpData = new StringBuilder();
            foreach (var item in CommandManager.GetTypesMarkedWithAttrib(typeof(CommandAttribute)))
            {
                CommandAttribute attribute = (CommandAttribute)item.GetCustomAttribute(typeof(CommandAttribute));
                HelpCommandDataAttribute helpDataAttrib = (HelpCommandDataAttribute)item.GetCustomAttribute(typeof(HelpCommandDataAttribute));
                var object_Created = (ICommand)item.GetConstructors()[0].Invoke(new object[] { });

                try
                {
                    if (args[0] != null)
                    {
                        if (attribute.Name == args[0].TrimStart().TrimEnd())
                        {
                            if (options.Contains("a"))
                            {
                                try
                                {
                                    if (item.GetMethod("additionalHelpData") != null)
                                    {
                                        helpData.AppendLine(GetCommandHelpString(attribute, helpDataAttrib));
                                        helpData.AppendLine(item.GetMethod("additionalHelpData").Invoke(object_Created, null).ToString());
                                        break;
                                    }
                                }
                                catch (Exception e)
                                {
                                    break;
                                }
                            }
                            helpData.AppendLine(GetCommandHelpString(attribute, helpDataAttrib));
                            break;
                        }
                    }
                    else
                    {

                    }
                }
                catch (Exception e)
                {
                    helpData.AppendLine(GetCommandHelpString(attribute, helpDataAttrib));
                }
            }
            UnityEngine.Debug.Log(helpData.ToString());
        }

        private string GetCommandHelpString(CommandAttribute attribute, HelpCommandDataAttribute helpDataAttrib)
        {
            StringBuilder commandData = new StringBuilder();
            commandData.Append(attribute.Name + "\t");
            if (helpDataAttrib != null)
            {
                if (helpDataAttrib.requiredInput.Length >= attribute.requiredInputAmount)
                {
                    foreach (var input in helpDataAttrib.requiredInput)
                    {
                        commandData.Append($"[{input}]\t");
                    }
                }
                else
                {
                    for (int i = 0; i < attribute.requiredInputAmount; i++)
                    {
                        commandData.Append("[FIELD]\t");
                    }
                }


                if (helpDataAttrib.validOptionNames != null)
                {
                    for (int i = 0; i < helpDataAttrib.validOptionNames.Length; i++)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(helpDataAttrib.validOptionValue[i]))
                            {
                                commandData.Append($"<{helpDataAttrib.validOptionNames[i]}>\t");
                            }
                            else
                            {
                                commandData.Append($"<{helpDataAttrib.validOptionNames[i]}:{helpDataAttrib.validOptionValue[i]}>\t");
                            }
                        }
                        catch (Exception e)
                        {
                            commandData.Append($"<{helpDataAttrib.validOptionNames[i]}>\t");
                        }

                    }
                }
                else
                {
                    foreach (var validOption in attribute.validOptions)
                    {
                        commandData.Append($"<{validOption}>\t");
                    }
                }


                commandData.Append($"{helpDataAttrib.description}");
            }
            else
            {
                for (int i = 0; i < attribute.requiredInputAmount; i++)
                {
                    commandData.Append("[FIELD]\t");
                }

                foreach (var validOption in attribute.validOptions)
                {
                    commandData.Append($"<{validOption}>\t");
                }
            }
            return commandData.ToString();
        }
    }
}
