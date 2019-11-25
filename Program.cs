using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using CommandLine;
using Newtonsoft.Json;
using kk33.RbxStreamSniper.Json;
using System.Linq;
using System.Threading;

namespace kk33.RbxStreamSniper
{
    class Program
    {
        class Options
        {
            [Option('c', "cookie", Required = false, HelpText = "Specify your .ROBLOSECURITY cookie.")]
            public string Cookie { get; set; }

            [Option('i', "userid", Required = false, HelpText = "Target user ID.")]
            public string UserId { get; set; }

            [Option('n', "username", Required = false, HelpText = "Target user name.")]
            public string UserName { get; set; }

            [Option('p', "placeid", Required = false, HelpText = "Target game ID.")]
            public string PlaceId { get; set; }

            [Option('s', "game", Required = false, HelpText = "Search for game by name and use first result.")]
            public string GameName { get; set; }
        }

        

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o =>
            {
                string cookie = null;
                string placeid = null;
                string userid = null;
                string avatar = null;
                string ownid = null;
                int totalPages = 0;


                Console.WriteLine();
                if (o.Cookie != null)
                {
                    cookie = ".ROBLOSECURITY=" + o.Cookie.Trim();
                }
                else
                {
                    if (Environment.GetEnvironmentVariable("rbxcookie") != null)
                    {
                        cookie = ".ROBLOSECURITY=" + Environment.GetEnvironmentVariable("rbxcookie").Trim();
                    }
                    else
                    {
                        Console.WriteLine("No .ROBLOSECURITY cookie specified. You may also use \"rbxcookie\" environment variable.");
                        Environment.Exit(1);
                    }
                }

                Console.Write("Getting your ID to test cookie... ");
                try
                {
                    ownid = Roblox.GetOwnId(cookie);
                }
                catch (Exception e) { CatchException(e); }
                Console.WriteLine(ownid);

                Console.Write("Getting target user ID... ");
                if (o.UserId != null)
                {
                    userid = o.UserId;
                }
                else
                {
                    if (o.UserName != null)
                    {
                        try
                        {
                            userid = JsonConvert.DeserializeObject<GetByUsername>(HttpHelpers.Get($"https://api.roblox.com/users/get-by-username?username={o.UserName.Trim()}")).Id.ToString();
                        }
                        catch (Exception e) { CatchException(e); }
                    }
                    else
                    {
                        Console.WriteLine("No user specified.");
                        Environment.Exit(1);
                    }
                }
                Console.WriteLine(userid);

                Console.Write("Getting target user avatar url... ");
                try
                {
                    avatar = Roblox.GetAvatarHeadshotUrl(userid);
                }
                catch (Exception e) { CatchException(e); }
                Console.WriteLine(avatar);

                Console.Write("Getting place ID... ");
                if (o.PlaceId != null)
                {
                    placeid = o.PlaceId;
                }
                else
                {
                    if (o.GameName != null)
                    {
                        try
                        {
                            placeid = Roblox.FindFirstPlace(o.GameName).PlaceID.ToString();
                        }
                        catch (Exception e) { CatchException(e); }

                    }
                    else
                    {
                        Console.WriteLine("No game specified.");
                        Environment.Exit(1);
                    }
                }
                Console.WriteLine(placeid);

                Console.Write("Getting total server count... ");
                try
                {
                    totalPages = (int)Math.Ceiling((decimal)Roblox.GetPlaceInstances(placeid, 0, cookie).TotalCollectionSize / 10);
                }
                catch (Exception e) { CatchException(e); }
                Console.WriteLine(totalPages);

                Console.WriteLine();
                for (int page = 0; page <= totalPages; page++)
                {
                    try
                    {
                        PlaceInstances placeInstances = Roblox.GetPlaceInstances(placeid, page, cookie);
                        foreach (var (server, iserver) in placeInstances.Collection.WithIndex())
                            foreach (var (player, iplayer) in server.CurrentPlayers.WithIndex())
                                if (player.Thumbnail.Url == avatar)
                                {
                                    Console.WriteLine();
                                    Console.WriteLine();
                                    Console.WriteLine(" Result: " + server.JoinScript);
                                    Console.WriteLine();
                                    Environment.Exit(0);
                                }
                                else
                                {
                                    string text = $" {page + 1}/{totalPages}  [";
                                    int textLen = text.CountCharacters();
                                    Console.SetCursorPosition(0, Console.CursorTop);
                                    Console.Write(text + ProgressBar.Generate(page * 100 / 500, Console.WindowWidth - textLen - 2) + "] ");
                                }
                    }
                    catch (Exception e) { CatchException(e); }
                }
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Cant find player...");
                Console.WriteLine();
            });
        }

        public static void CatchException(Exception e)
        {
            Console.WriteLine("\r\n===========\r\nException: " + e.ToString() + " | " + e.InnerException?.ToString());
            Environment.Exit(2);
        }
    }

    public static class Extensions
    {
        public static int CountCharacters(this string source)
        {
            int count = 0;
            foreach (char c in source)
                count++;
            return count;
        }
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self) => self?.Select((item, index) => (item, index)) ?? new List<(T, int)>();
    }
}