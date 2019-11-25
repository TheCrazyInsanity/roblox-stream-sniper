using System.Collections.Generic;

namespace kk33.RbxStreamSniper.Json
{
    public class GetByUsername
    {
        public int Id;
    }
    public class Place
    {
        public string PlaceID;
    }
    public class PlaceInstances
    {

        public int TotalCollectionSize;
        public List<PlaceInstance> Collection;
        public class PlaceInstance
        {
            public List<Player> CurrentPlayers;
            public class Player
            {
                public UserThumbnail Thumbnail;
                public class UserThumbnail
                {
                    public string Url;
                }
            }
            public string JoinScript;
        }
    }
}