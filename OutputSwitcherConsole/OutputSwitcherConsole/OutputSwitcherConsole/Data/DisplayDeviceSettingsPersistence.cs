using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace OutputSwitcherConsole.Data
{
    class DisplayDeviceSettingsPersistence
    {
        // TODO: Maybe make this something given to the class instead of hard coded?
        private static readonly string DisplayConfigurationsFilePath =
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\OutputSwitcherConsole";
        
        private static readonly string DisplayConfigurationsFullFilename = DisplayConfigurationsFilePath + "\\DisplayConfigurations.xml";

        public static List<DisplayPreset> LoadSettings()
        {
            List<DisplayPreset> displayPresets = null;

            XmlSerializer reader = new XmlSerializer(typeof(List<DisplayPreset>));

            if (File.Exists(DisplayConfigurationsFullFilename))
            {
                StreamReader presetsFile = new StreamReader(File.OpenRead(DisplayConfigurationsFullFilename));
                displayPresets = (List<DisplayPreset>)reader.Deserialize(presetsFile);
                presetsFile.Close();
            }

            return displayPresets;
        }

        public static void WriteSettings(List<DisplayPreset> presetCollection)
        {
            XmlSerializer writer = new XmlSerializer(typeof(List<DisplayPreset>));

            if (!Directory.Exists(DisplayConfigurationsFilePath))
            {
                // This will throw exception if it fails.
                Directory.CreateDirectory(DisplayConfigurationsFilePath);
            }

            FileStream presetsFile = File.Create(DisplayConfigurationsFullFilename);

            writer.Serialize(presetsFile, presetCollection);
            presetsFile.Close();
        }
    }
}
