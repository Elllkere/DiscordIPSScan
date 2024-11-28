using System.Net;
using System.Net.Sockets;

namespace DiscordIPSScan
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> regions = [
                    "atlanta",
                    "brazil",
                    "bucharest",
                    "buenos-aires",
                    "dammam",
                    "dubai",
                    "finland",
                    "frankfurt",
                    "hongkong",
                    "india",
                    "jakarta",
                    "japan",
                    "madrid",
                    "milan",
                    "montreal",
                    "newark",
                    "oregon",
                    "rotterdam",
                    "russia",
                    "santa-clara",
                    "santiago",
                    "seattle",
                    "singapore",
                    "south-korea",
                    "southafrica",
                    "st-pete",
                    "stage-scale",
                    "stockholm",
                    "sydney",
                    "tel-aviv",
                    "us-central",
                    "us-east",
                    "us-south",
                    "us-west",
                    "warsaw"
            ];

            int limit = 15000;

            long all = regions.Count * limit;
            long done = 0;

            Parallel.ForEach(regions, region =>
            {
                using StreamWriter writer = new StreamWriter($"{region}.txt");
                for (int i = 1; i <= 15000; i++)
                {
                    Interlocked.Increment(ref done);
                    Console.WriteLine($"{Math.Round((double)done / all * 100, 2)} / 100");

                    try
                    {
                        string domain = $"{region}{i}.discord.gg";
                        var ips = Dns.GetHostAddresses(domain);
                        foreach (IPAddress ip in ips)
                        {
                            if (!string.IsNullOrEmpty(ip.ToString()))
                            {
                                writer.WriteLine(ip.ToString());
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        if (ex is SocketException se)
                        {
                            if (se.SocketErrorCode == SocketError.HostNotFound)
                                continue;
                            else if (se.SocketErrorCode == SocketError.TryAgain)
                            {
                                Console.WriteLine($"retry {region}{i}.discord.gg");
                                Interlocked.Decrement(ref done);
                                i--;
                                continue;
                            }
                        }

                        Console.WriteLine($"{region}{i}.discord.gg {ex.ToString()}");
                    }
                }
            });

            List<string> ip_per_reg = new();
            foreach(var region in regions)
            {
                ip_per_reg.AddRange(File.ReadAllLines($"{region}.txt"));
            }

            File.WriteAllLines("ips.txt", ip_per_reg);
        }
    }
}
