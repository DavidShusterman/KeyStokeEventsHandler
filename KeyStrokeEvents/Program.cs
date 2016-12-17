using System;   
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using KeyStrokeEvents.BL.ProgramExecution;

namespace KeyStrokeEvents
{
    class Program
    {
        
        static void Main(string[] args)
        {
            KeyCombinationConfigurationReader reader = new KeyCombinationConfigurationReader();
            var keys = reader.GetListOfKeyStrokes();
            ProgramExecuter executer = new ProgramExecuter(reader.GetKeyConfiguration().ProgramToExecute);
            KeyWatcher keyWatcher = new KeyWatcher(keys);
            keyWatcher.MagicSequencePressed += () => executer.ExecuteFile();
            keyWatcher.InitializeLogging();
        }

    }
}
