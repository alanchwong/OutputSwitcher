using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutputSwitcherConsole.Data
{
    class DisplayDeviceSettingsPersistence
    {
        // TODO: Maybe make this something given to the class instead of hard coded?
        private static readonly string DisplayConfigurationsFilePath =
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\OutputSwitcherConsole";
        
        private static readonly string DisplayConfigurationsFullFilename = DisplayConfigurationsFilePath + "\\DisplayConfigurations.xml";

        public static void LoadSettings()
        {

        }

        public static void WriteSettings(List<DisplayPreset> presetCollection)
        {
            System.Xml.Serialization.XmlSerializer writer
                = new System.Xml.Serialization.XmlSerializer(typeof(List<DisplayPreset>));

            if (!System.IO.Directory.Exists(DisplayConfigurationsFilePath))
            {
                // This will throw exception if it fails.
                System.IO.Directory.CreateDirectory(DisplayConfigurationsFilePath);
            }

            System.IO.FileStream presetsFile = System.IO.File.Create(DisplayConfigurationsFullFilename);

            writer.Serialize(presetsFile, presetCollection);
            presetsFile.Close();
        }
    }
}
