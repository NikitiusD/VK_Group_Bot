using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot
{
    internal class WallPost
    {
        public readonly string likes;
        public readonly string reposts;
        public readonly string[] photosUrls;
        public readonly string text;

        public WallPost(string likes,string reposts, string[] photosUrls, string text)
        {
            this.likes = likes;
            this.reposts = reposts;
            this.photosUrls = photosUrls;
            this.text = text;
        }
    }
}
