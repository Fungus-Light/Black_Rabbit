using System.Text;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CommandTerminal
{
    public static class BuiltinCommands
    {
        [RegisterCommand(Help = "Does nothing")]
        static void CommandNoop(CommandArg[] args) { }

        [RegisterCommand(Help = "Clears the Command Console", MaxArgCount = 0)]
        static void CommandClear(CommandArg[] args)
        {
            Terminal.Buffer.Clear();
        }

        [RegisterCommand(Help = "Lists all Commands or displays help documentation of a Command", MaxArgCount = 1)]
        static void CommandHelp(CommandArg[] args)
        {
            if (args.Length == 0)
            {
                foreach (var command in Terminal.Shell.Commands)
                {
                    Terminal.Log("{0}: {1}", command.Key.PadRight(16), command.Value.help);
                }
                return;
            }

            string command_name = args[0].String.ToUpper();

            if (!Terminal.Shell.Commands.ContainsKey(command_name))
            {
                Terminal.Shell.IssueErrorMessage("Command {0} could not be found.", command_name);
                return;
            }

            string help = Terminal.Shell.Commands[command_name].help;

            if (help == null)
            {
                Terminal.Log("{0} does not provide any help documentation.", command_name);
            }
            else
            {
                Terminal.Log(help);
            }
        }

        [RegisterCommand(Help = "Times the execution of a Command", MinArgCount = 1)]
        static void CommandTime(CommandArg[] args)
        {
            var sw = new Stopwatch();
            sw.Start();

            Terminal.Shell.RunCommand(JoinArguments(args));

            sw.Stop();
            Terminal.Log("Time: {0}ms", (double)sw.ElapsedTicks / 10000);
        }

        [RegisterCommand(Help = "Outputs message")]
        static void CommandPrint(CommandArg[] args)
        {
            Terminal.Log(JoinArguments(args));
        }

        [RegisterCommand(Name = "Scenes", Help = "show all usable levels in buildsettings", MinArgCount = 0, MaxArgCount = 0)]
        static void CommandShowScene(CommandArg[] args)
        {
            int count = SceneManager.sceneCountInBuildSettings;
            Terminal.Log(count+" Scenes In Build Settings");
            if (count <= 0)
            {
                Terminal.Log("No scenes in Build Settings!!!");
            }
            else
            {
                Terminal.Log("-----Here Are All Scenes In Build Settings-----");
                for (int i = 0; i < count; i++)
                {
                    string temp = SceneUtility.GetScenePathByBuildIndex(i);
                    int end = temp.LastIndexOf(".");
                    int start = temp.LastIndexOf("/");
                    Terminal.Log(temp.Substring(start+1,end-start-1));
                }
                Terminal.Log("-----------------------------------------------");
            }

        }

        [RegisterCommand(Name = "Jump", Help = "Jump to a level", MinArgCount = 1, MaxArgCount = 1)]
        static void CommandJump(CommandArg[] args)
        {
            string scene = args[0].String;
            SceneManager.LoadScene(scene);
        }

        [RegisterCommand(Name = "GameSpeed", Help = "Set Game Time Scale", MinArgCount = 1, MaxArgCount = 1)]
        static void CommandSpeed(CommandArg[] args)
        {
            float time = args[0].Float;
            Terminal.Log("Your Game speed is set to " + time);
            Time.timeScale = time;
        }


#if DEBUG
        [RegisterCommand(Help = "Outputs the StackTrace of the previous message", MaxArgCount = 0)]
        static void CommandTrace(CommandArg[] args)
        {
            int log_count = Terminal.Buffer.Logs.Count;

            if (log_count - 2 < 0)
            {
                Terminal.Log("Nothing to trace.");
                return;
            }

            var log_item = Terminal.Buffer.Logs[log_count - 2];

            if (log_item.stack_trace == "")
            {
                Terminal.Log("{0} (no trace)", log_item.message);
            }
            else
            {
                Terminal.Log(log_item.stack_trace);
            }
        }
#endif

        [RegisterCommand(Help = "Quits running Application", MaxArgCount = 0)]
        static void CommandQuit(CommandArg[] args)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        static string JoinArguments(CommandArg[] args)
        {
            var sb = new StringBuilder();
            int arg_length = args.Length;

            for (int i = 0; i < arg_length; i++)
            {
                sb.Append(args[i].String);

                if (i < arg_length - 1)
                {
                    sb.Append(" ");
                }
            }

            return sb.ToString();
        }
    }
}
