using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyStrokeEvents.Models
{
    [Serializable]
    public class KeyCombinationConfiguration
    {
        public string KeyCombination { get; set; }

        public string ProgramToExecute { get; set; }
    }
}
