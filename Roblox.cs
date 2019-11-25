using System.Collections.Generic;
using kk33.RbxStreamSniper.Json;
using Newtonsoft.Json;

namespace kk33.RbxStreamSniper
{
    public static class Roblox
    {
        public static string GetOwnId(string cookie)
        {
            return HttpHelpers.Get($"https://www.roblox.com/game/GetCurrentUser.ashx", cookie);
        }

        public static PlaceInstances GetPlaceInstances(string placeId, int page, string cookie)
        {
            return JsonConvert.DeserializeObject<PlaceInstances>(HttpHelpers.Get($"https://www.roblox.com/games/getgameinstancesjson?placeId={placeId.Trim()}&startindex={page * 10}", cookie));
        }
        
        public static string GetAvatarHeadshotUrl(string userid, int size = 48)
        {
            return HttpHelpers.GetRedirectLocation($"https://www.roblox.com/headshot-thumbnail/image?userId={userid.Trim()}&width={size}&height={size}&format=png");
        }

        public static Place FindFirstPlace(string searchTerm)
        {
            return JsonConvert.DeserializeObject<List<Place>>(HttpHelpers.Get($"https://www.roblox.com/games/list-json?MaxRows=1&Keyword={searchTerm.Trim()}"))[0];
        }
    }
}