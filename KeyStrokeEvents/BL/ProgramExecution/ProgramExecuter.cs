using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace KeyStrokeEvents.BL.ProgramExecution
{
    class ProgramExecuter
    {
        private readonly string _fileToExecute;
        public ProgramExecuter(string fileToExecute)
        {
            _fileToExecute = fileToExecute;
        }
        
        public void ExecuteFile()
        {
            Process.Start(_fileToExecute);
        }
    }
}
