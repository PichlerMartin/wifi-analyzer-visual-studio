using NativeWifi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NativeWifi.Wlan;

namespace wifi_analyzer
{
    class Program
    {

        static void Main(string[] args)
        {
            WlanClient client = new WlanClient();

            foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
            {
                Wlan.WlanAvailableNetwork[] networks = wlanIface.GetAvailableNetworkList(0);
                Wlan.WlanBssEntry[] bssidd = wlanIface.GetNetworkBssList();

                int i = 1;
                
                foreach(WlanAvailableNetwork network in networks)
                {
                    if (!network.profileName.Equals(""))
                    {
                        Console.WriteLine("SSID {0} : {1}", i, network.profileName);
                        Console.WriteLine("Authentifizierung:       {0}", network.dot11DefaultAuthAlgorithm);
                        Console.WriteLine("Verschlüsselung:         {0}", network.dot11DefaultCipherAlgorithm);
                        Console.WriteLine("Netzwertyp:              {0}", network.dot11BssType);
                        Console.WriteLine("\n");
                    }

                    i++;
                }
            }

            Console.ReadKey();
        }
    }
}