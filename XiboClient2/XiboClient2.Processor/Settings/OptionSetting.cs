using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiboClient2.Processor.Logic;

namespace XiboClient2.Processor.Settings
{
    public class OptionSettings
    {
        private HardwareKey _hardwareKey;

        public void SaveSettings()
        {
            try
            {
                // Get a hardware key here, just in case we havent been able to get one before
                _hardwareKey = new HardwareKey();

                // Also tweak the address of the xmds1
                //xmds.Url = ApplicationSettings.Default.XiboClient_xmds_xmds + "&method=registerDisplay";

                //// Call register
                //xmds.RegisterDisplayAsync(
                //    ApplicationSettings.Default.ServerKey,
                //    ApplicationSettings.Default.HardwareKey,
                //    ApplicationSettings.Default.DisplayName,
                //    "windows",
                //    ApplicationSettings.Default.ClientVersion,
                //    ApplicationSettings.Default.ClientCodeVersion,
                //    Environment.OSVersion.ToString(),
                //    _hardwareKey.MacAddress,
                //    _hardwareKey.Channel,
                //    _hardwareKey.getXmrPublicKey());
            }
            catch(Exception ex)
            {

            }
        }
    }
}
