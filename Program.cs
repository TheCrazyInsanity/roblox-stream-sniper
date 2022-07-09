using System;
using CommandLine;
using Newtonsoft.Json;
using kk33.RbxStreamSniper.Json;

namespace kk33.RbxStreamSniper
{
    class Program
    {
        class Options
        {
            [Option('c', "cookie", Required = false, HelpText = "Set your .ROBLOSECURITY cookie.")]
            public string Cookie { get; set; }

            [Option('u', "user", Required = false, HelpText = "Set target user ID.")]
            public long UserId { get; set; }

            [Option('n', "username", Required = false, HelpText = "Set target user name.")]
            public string UserName { get; set; }

            [Option('p', "place", Required = false, HelpText = "Set place ID target is in.")]
            public string PlaceId { get; set; }

            [Option('s', "search", Required = false, HelpText = "Search for game by name and use start place of first result.")]
            public string GameName { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o =>
            {
                string cookie = null;
                string placeid = null;
                //long userid = null;
                string avatar = null;
                string ownid = null;
                int totalPages = 0;

                //Console.WriteLine();
                if (o.Cookie != null)
                {
                    cookie = ".ROBLOSECURITY=" + o.Cookie.Trim();
                }
                else
                {
                    //Console.WriteLine("No .ROBLOSECURITY cookie specified. You may also use \"rbxcookie\" environment variable.");
                    Environment.Exit(1);
                }

                //Console.Write("Getting your ID to check if cookie is valid... ");
                try
                {
                    //ownid = Roblox.GetOwnId(cookie);
                }
                catch (Exception e) { CatchException(e); }
                //Console.WriteLine(ownid);

                //Console.Write("Getting target user ID... ");
                if (o.UserId != null)
                {
                   long userid = o.UserId;
                }
                else
                {
                    if (o.UserName != null)
                    {
                        try
                        {
                           long userid = (long) JsonConvert.DeserializeObject<GetByUsername>(HttpHelpers.Get($"https://api.roblox.com/users/get-by-username?username={o.UserName.Trim()}")).Id.ToString();
                        }
                        catch (Exception e) { CatchException(e); }
                    }
                    else
                    {
                        Console.WriteLine("No user specified.");
                        //Console.WriteLine();
                        Environment.Exit(1);
                    }
                }
                //Console.WriteLine(userid);

                //Console.Write("Getting target user avatar url... ");
                try
                {
                    avatar = Roblox.GetAvatarHeadshotUrl(userid);
                }
                catch (Exception e) { CatchException(e); }
                //Console.WriteLine(avatar);

                //Console.Write("Getting place ID... ");
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
                        //Console.WriteLine();
                        Environment.Exit(1);
                    }
                }
                //Console.WriteLine(placeid);

                //Console.Write("Getting total pages... ");
                try
                {
                    totalPages = (int)Math.Ceiling((decimal)Roblox.GetPlaceInstances(placeid, 0, cookie).TotalCollectionSize / 10);
                }
                catch (Exception e) { CatchException(e); }
                //Console.WriteLine(totalPages);

                //Console.WriteLine();
                for (int page = 0; page <= totalPages; page++)
                {
                    try
                    {
                        PlaceInstances placeInstances = Roblox.GetPlaceInstances(placeid, page, cookie);
                        foreach (var server in placeInstances.Collection)
                            foreach (var player in server.CurrentPlayers)
                                if (player.Thumbnail.Url == avatar)
                                {
                                    //Console.WriteLine();
                                    //Console.WriteLine();
                                    Console.WriteLine(server.JoinScript);
                                    //Console.WriteLine();
                                    Environment.Exit(0);
                                }
                        string text = $" {page + 1}/{totalPages}  [";
                        int textLen = CountCharacters(text);
                        //Console.SetCursorPosition(0, Console.CursorTop);
                        //Console.Write($"{text}{ProgressBar.Generate(page * 100 / 500, Console.WindowWidth - textLen - 2)}] ");
                    }
                    catch (Exception e) { CatchException(e); }
                }
                //Console.WriteLine();
                //Console.WriteLine();
                Console.WriteLine("Cant find player...");
                //Console.WriteLine();
            });
        }

        public static int CountCharacters(string source)
        {
            int count = 0;
            foreach (char c in source)
                count++;
            return count;
        }

        public static void CatchException(Exception e)
        {
            Console.WriteLine("Unknown Error");
            //Added to make it easier to deal with inside of node 
            Console.WriteLine("\r\n===========\r\nException: " + e.ToString() + " | " + e.InnerException?.ToString());
            //*Outdated* Above should still be printed, will fuck up the node part unless i find a good way to pass it through, but it will be pretty obvious SOMETHING is wrong if it fucks up the node part.
            //Console.WriteLine();
            Environment.Exit(2);
        }
    }
}