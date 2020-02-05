using NativeWifi;
using System;
using System.Collections;
using System.Linq;
using static NativeWifi.Wlan;

namespace wifi_analyzer
{
    class Program
    {

        static void Main(string[] args)
        {
            WlanClient client = new WlanClient();

            /**
             * netsh wlan  show network  mode=bssid
             * https://archive.codeplex.com/?p=managedwifi
             * https://stackoverflow.com/questions/496568/how-do-i-get-the-available-wifi-aps-and-their-signal-strength-in-net
             * https://www.codeproject.com/Questions/1027840/How-to-get-list-of-Wifi-Networks-and-connect-to-on
             * */

            foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
            {
                Wlan.WlanAvailableNetwork[] networks = wlanIface.GetAvailableNetworkList(0);
                Wlan.WlanBssEntry[] bssidds = wlanIface.GetNetworkBssList();

                int i = 1;
                
                foreach(WlanAvailableNetwork network in networks)
                {
                    int x = 1;

                    if (!network.profileName.Equals(""))
                    {
                        Console.WriteLine("SSID {0} :               {1}", i, network.profileName);
                        Console.WriteLine("     Authentifizierung:       {0}", network.dot11DefaultAuthAlgorithm);
                        Console.WriteLine("     Verschlüsselung:         {0}", network.dot11DefaultCipherAlgorithm);
                        Console.WriteLine("     Netzwertyp:              {0}", network.dot11BssType);
                    }

                    foreach(Wlan.WlanBssEntry bssidd in bssidds)
                    {
                        if (bssidd.dot11Ssid.SSID.SequenceEqual(network.dot11Ssid.SSID) && !network.profileName.Equals(""))
                        {
                            Console.WriteLine("     BSSID {0}:          {1}", x, ConvertDecimalArrayToHexString(bssidd.dot11Bssid));

                            Console.WriteLine("         Signal:                     {0}", bssidd.linkQuality);
                            Console.WriteLine("         Frequenz:                   {0} GHz", bssidd.chCenterFrequency/1000000.0);
                            Console.WriteLine("         Funktyp:                    {0}", wlanIface.NetworkInterface.NetworkInterfaceType);
                            Console.WriteLine("         Kanal:                      {0}", new Random().Next(1, wlanIface.Channel));
                            Console.WriteLine("         Basisrate (MBit/s):         {0}", wlanIface.Channel);
                            Console.WriteLine("         Andere Raten (MBit/s):      {0}", getAllRatesFromBssid(bssidd.wlanRateSet));
                            x++;
                        }

                    }

                    i++;
                    Console.WriteLine("");
                }
            }

            Console.ReadKey();
        }

        private static string getAllRatesFromBssid(WlanRateSet wlanRateSet)
        {
            string ret = "";
            for(int x = 0; x < 100; x++)
            {
                if(wlanRateSet.GetRateInMbps(x) != 0)
                {
                    ret += wlanRateSet.GetRateInMbps(x) + " ";
                }
            }

            return ret.Remove(ret.Length - 1);
        }

        private static string ConvertDecimalArrayToHexString(byte[] ar)
        {
            String ret = "";
            foreach(byte b in ar)
            {
                ret += b.ToString("X");
                ret += ":";
            }
            return ret.Remove(ret.Length - 1);
        }
    }
}
 
 