using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OutputSwitcher.Core;

namespace OutputSwitcher.TrayApp
{
    /// <summary>
    /// Collections functions to safely apply presets.
    /// </summary>
    internal class SafePresetApplier
    {
        /// <summary>
        /// Applies the supplied DisplayPreset and displays a 15 second countdown to
        /// revert to the previous display configuration unless a user intervenes. This is
        /// to mimic the behavior of Windows when display resolutions are changed, and is
        /// a safety pattern in case the new display configuration is not visible.
        /// </summary>
        /// <param name="displayPreset">The display preset to apply.</param>
        public static void ApplyPresetWithRevertCountdown(DisplayPreset displayPreset)
        {
            DisplayPreset lastConfig = DisplayPresetRecorderAndApplier.ReturnLastConfigAndApplyPreset(displayPreset);

            // Pop up a dialog to give user the option to keep the configuration or else
            // automatically revert to the last configuration.
            // TODO: This seems weird to pass control away like this.
            UseAppliedPresetCountdownForm revertPresetCountdownForm = new UseAppliedPresetCountdownForm(lastConfig);

            revertPresetCountdownForm.Show();
        }
    }
}
