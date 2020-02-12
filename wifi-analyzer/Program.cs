using NativeWifi;
using System;
using System.Collections;
using System.Linq;
using static NativeWifi.Wlan;
using System.Collections.Generic;

namespace wifi_analyzer
{
    class Program
    {

        static void Main(string[] args)
        {
            WlanClient client = new WlanClient();
            Dictionary<int, int> GHz24 = new Dictionary<int, int>();
            int[] GHz24Channel = {1,2,3,4,5,6,7,8,9,10,11,12,13,14};
            int[] GHz24Frequency = {2412,2417,2422,2427,2432,2437,2442,2447,2452,2457,2462,2467,2472,2477,2482};
            int i = 0;

            GHz24Channel.ToList().ForEach(delegate (int channel)
            {
                GHz24.Add(GHz24Frequency[i], channel);
                i++;
            });

            /**
             * netsh wlan  show network  mode=bssid
             * https://archive.codeplex.com/?p=managedwifi
             * https://stackoverflow.com/questions/496568/how-do-i-get-the-available-wifi-aps-and-their-signal-strength-in-net
             * https://www.codeproject.com/Questions/1027840/How-to-get-list-of-Wifi-Networks-and-connect-to-on
             * 
             * https://docs.microsoft.com/en-us/windows/win32/nativewifi/portal
             * https://en.wikipedia.org/wiki/List_of_WLAN_channels
             * ToDo: Read documentation concerning the NativeWifi library
             * */

            foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
            {
                Wlan.WlanAvailableNetwork[] networks = wlanIface.GetAvailableNetworkList(0);
                Wlan.WlanBssEntry[] bssidds = wlanIface.GetNetworkBssList();

                i = 1;
                
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
                            Console.WriteLine("         Kanal:                      {0}", GHz24[(int)bssidd.chCenterFrequency / 1000]);
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
 
 