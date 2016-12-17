using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using KeyStrokeEvents.Models;

namespace KeyStrokeEvents
{
    class KeyCombinationConfigurationReader : IKeyboardKeysConfigurationReader
    {
        private readonly XmlSerializer _serializer;

        public KeyCombinationConfigurationReader()
        {
            _serializer = new XmlSerializer(typeof(KeyCombinationConfiguration));
        }
        public List<Keys> GetListOfKeyStrokes()
        {
            var keysList = new List<Keys>();
            var keyCombinationConfiguration = GetKeyConfiguration();
            return ConvertKeyCombinationToListOfKeys(keysList, keyCombinationConfiguration.KeyCombination);
        }

        public KeyCombinationConfiguration GetKeyConfiguration()
        {
            using (var reader = new StreamReader(ConfigurationManager.AppSettings["KeyStrokeCombinationConfigurationPath"]))
            {
                var keyCombinationConfiguration = (KeyCombinationConfiguration)_serializer.Deserialize(reader);
                return keyCombinationConfiguration;
            }
        }

        private List<Keys> ConvertKeyCombinationToListOfKeys(List<Keys> keys, string combination)
        {
            foreach (var character in combination)
            {
                keys.Add((Keys)char.ToUpper(character));
            }
            return keys;
        }
    }
}
