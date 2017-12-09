using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using xNet.Collections;
using xNet.Net;
using Newtonsoft.Json;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace Bot
{
    class Bot
    {
        private const string appId = "6289595";
        private const string MemeForceId = "158155713";
        private const string standartPath = @"C:\Projects\VKGroupBot\Pics\";
        private readonly string accessToken = File.ReadAllText(@"C:\Projects\VKGroupBot\access_token.txt");

        public Bot()
        {
            Main();
        }

        private void Main()
        {
            var matMexMemes = new Group("134071529", accessToken);
            var posts = matMexMemes.GetPosts(500);
            var bestPosts = matMexMemes.GetBestPosts(posts);
            matMexMemes.SaveAll(bestPosts.ToList(), standartPath);

            //var request = new Request(MemeForceId, accessToken);
            //request.PostPhoto(new[] { @"C:\Projects\VKGroupBot\Pics\1.png" }, "");
        }
    }
}
