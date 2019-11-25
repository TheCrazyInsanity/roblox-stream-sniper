using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using CommandLine;
using Newtonsoft.Json;
using kk33.RbxStreamSniper.Json;
using System.Linq;

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

            [Option('g', "gameid", Required = false, HelpText = "Target game ID.")]
            public string GameId { get; set; }

            [Option('s', "gamename", Required = false, HelpText = "Search for game by name and use first result.")]
            public string GameName { get; set; }
        }

        static PlaceInstances GetPlaceInstances(string placeId, int page, string cookie)
        {
            return JsonConvert.DeserializeObject<PlaceInstances>(HttpHelpers.Get($"https://www.roblox.com/games/getgameinstancesjson?placeId={placeId}&startindex={page * 10}", cookie));
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

                if (o.Cookie == null)
                {
                    if (Environment.GetEnvironmentVariable("rbxcookie") == null)
                    {
                        Console.WriteLine("No .ROBLOSECURITY cookie specified. You may also use \"rbxcookie\" environment variable.");
                        Environment.Exit(1);
                    }
                    else
                    {
                        cookie = ".ROBLOSECURITY=" + Environment.GetEnvironmentVariable("rbxcookie").Trim();
                    }
                }
                else
                {
                    cookie = ".ROBLOSECURITY=" + o.Cookie.Trim();
                }

                Console.Write("Getting to get your ID... ");
                try
                {
                    ownid = HttpHelpers.Get($"https://www.roblox.com/game/GetCurrentUser.ashx", cookie);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString() + " | " + e.InnerException?.ToString());
                    Environment.Exit(2);
                }
                Console.WriteLine(ownid);

                Console.Write("Getting target user ID... ");
                if (o.UserId == null)
                {
                    if (o.UserName == null)
                    {
                        Console.WriteLine("No user specified.");
                        Environment.Exit(1);
                    }
                    else
                    {
                        try 
                        {
                            userid = JsonConvert.DeserializeObject<GetByUsername>(HttpHelpers.Get($"https://api.roblox.com/users/get-by-username?username={o.UserName.Trim()}")).Id.ToString();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString() + " | " + e.InnerException.ToString());
                            Environment.Exit(2);
                        }
                    }
                }
                else
                {
                    userid = o.UserId;
                }
                Console.WriteLine(userid);

                Console.Write("Getting target user avatar url... ");
                try
                {
                    avatar = HttpHelpers.GetRedirectLocation($"https://www.roblox.com/headshot-thumbnail/image?userId={userid}&width=48&height=48&format=png");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString() + " | " + e.InnerException?.ToString());
                    Environment.Exit(2);
                }
                Console.WriteLine(avatar);

                Console.Write("Getting place ID... ");
                if (o.GameId == null)
                {
                    if (o.GameName == null)
                    {
                        Console.WriteLine("No game specified.");
                        Environment.Exit(1);
                    }
                    else
                    {
                        try
                        {
                            placeid = JsonConvert.DeserializeObject<List<Place>>(HttpHelpers.Get($"https://www.roblox.com/games/list-json?MaxRows=1&Keyword={o.GameName.Trim()}"))[0].PlaceID.ToString();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString() + " | " + e.InnerException?.ToString());
                            Environment.Exit(2);
                        }
                    }
                }
                else
                {
                    placeid = o.GameId;
                }
                Console.WriteLine(placeid);

                Console.Write("Getting total server count... ");
                try
                {
                    totalPages = (int)Math.Ceiling((decimal)GetPlaceInstances(placeid, 0, cookie).TotalCollectionSize / 10);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString() + " | " + e.InnerException?.ToString());
                    Environment.Exit(2);
                }
                Console.WriteLine(totalPages);

                for (int page = 0; page <= totalPages; page++)
                {
                    try
                    {
                        PlaceInstances pin = GetPlaceInstances(placeid, page, cookie);
                        foreach (var (server, iserver) in pin.Collection.WithIndex())
                        foreach (var (player, iplayer) in server.CurrentPlayers.WithIndex())
                        if (player.Thumbnail.Url == avatar)
                        {
                            Console.WriteLine(server.JoinScript);
                            Environment.Exit(0);
                        }
                        else
                        {
                            Console.WriteLine($"Searching... Page: {page + 1}/{totalPages}, Server: {iserver + 1}/{pin.Collection.Count}, Player: {iplayer + 1}/{server.CurrentPlayers.Count}");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString() + " | " + e.InnerException?.ToString());
                        Environment.Exit(2);
                    }
                }
            });
        }
    }

    public static class Extensions
    {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self) => self.Select((item, index) => (item, index));
    }
}