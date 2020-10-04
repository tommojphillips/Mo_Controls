using System;
using System.Reflection;
using MSCLoader;
using TommoJProductions.MoControls;

namespace TommoJProductions.Debugging
{
    /// <summary>
    /// Represents a console command that lists all loaded assemblies.
    /// </summary>
    internal class ListLoadedAssembliesConsoleCommand : ConsoleCommand
    {
        // Written, 05.01.2019

        public override string Name => "mclistassem";

        public override string Help => "Lists all loaded assemblies. if an index is passed. eg 'mclistassem 3', it will list details about a specific assembly.";

        public override void Run(string[] inArgs)
        {
            // Written, 05.01.2019

            if (inArgs.Length == 0)
            {

                string assemList = "";
                int i = 1;
                foreach (Assembly assem in AppDomain.CurrentDomain.GetAssemblies())
                {
                    assemList += String.Format("{0}.) {1}\r\n", i, assem.GetName().Name);
                    i++;
                }
                MoControlsMod.print("Loaded Assemblies:\r\n" + assemList, DebugTypeEnum.none);
            }
            else
            {
                int _index = -1;
                bool invaildArgs = false;

                if (inArgs.Length == 1)
                {   
                    try
                    {
                        _index = int.Parse(inArgs[0]);
                    }
                    catch
                    {
                        // Invaild args.
                        invaildArgs = true;
                    }
                }
                else
                {
                    // Invaild args.
                    invaildArgs = true;
                }

                if (!invaildArgs)
                {
                    Assembly assem = AppDomain.CurrentDomain.GetAssemblies()[_index + 1];
                    MoControlsMod.print("Assembly: <b>" + assem.GetName().Name + "</b>\r\nLocation: " + assem.Location + "\r\n" + assem.FullName, DebugTypeEnum.none);
                }
                else
                {
                    MoControlsMod.print("Error! Invaild args.", DebugTypeEnum.none);
                }
            }
        }
    }
}
