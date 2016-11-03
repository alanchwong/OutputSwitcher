using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OutputSwitcher.TrayApp
{
    internal class PresetToHotkeyMapPersistence
    {
        // TODO: Maybe make this something given to the class instead of hard coded?
        private static readonly string DisplayConfigurationsFilePath =
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\OutputSwitcherConsole";

        private static readonly string PresetToHotkeysMapFilename = DisplayConfigurationsFilePath + "\\PresetHotkeysMap.xml";

        public static Dictionary<string, uint> LoadPresetToHotkeysMap()
        {
            Dictionary<string, uint> presetsToHotkeysMap = null;
            List<PresetHotkeyPersistencePair> presetHotkeyPersistenceList = new List<PresetHotkeyPersistencePair>();

            // As the .NET KeyValuePair type is non-serializable, we need to use an intermediate collection
            // of preset to hotkey pairings with our custom struct.
            XmlSerializer reader = new XmlSerializer(typeof(List<PresetHotkeyPersistencePair>));

            if (File.Exists(PresetToHotkeysMapFilename))
            {
                StreamReader presetToHotkeysMapFile = new StreamReader(File.OpenRead(PresetToHotkeysMapFilename));
                presetHotkeyPersistenceList = (List<PresetHotkeyPersistencePair>)reader.Deserialize(presetToHotkeysMapFile);
                presetsToHotkeysMap = new Dictionary<string, uint>();

                foreach (PresetHotkeyPersistencePair presetToHotkeyPair in presetHotkeyPersistenceList)
                {
                    presetsToHotkeysMap.Add(presetToHotkeyPair.PresetName, presetToHotkeyPair.KeyCode);
                }

                presetToHotkeysMapFile.Close();
            }

            return presetsToHotkeysMap;
        }

        public static void SavePresetToHotkeysMap(Dictionary<string, uint> presetToHotkeysMap)
        {
            XmlSerializer writer = new XmlSerializer(typeof(List<PresetHotkeyPersistencePair>));

            if (!Directory.Exists(DisplayConfigurationsFilePath))
            {
                // This will throw exception if it fails.
                Directory.CreateDirectory(DisplayConfigurationsFilePath);
            }

            // As the .NET KeyValuePair type is non-serializable, we need to generate the intermediate
            // collection of preset to hotkey pairings with our custom struct.
            List<PresetHotkeyPersistencePair> presetHotkeyPersistenceList = new List<PresetHotkeyPersistencePair>();            
            foreach (KeyValuePair<string, uint> kvPair in presetToHotkeysMap)
            {
                presetHotkeyPersistenceList.Add(new PresetHotkeyPersistencePair(kvPair.Key, kvPair.Value));
            }

            FileStream presetToHotkeysMapFile = File.Create(PresetToHotkeysMapFilename);
            writer.Serialize(presetToHotkeysMapFile, presetHotkeyPersistenceList);
            presetToHotkeysMapFile.Close();
        }
    }
}
