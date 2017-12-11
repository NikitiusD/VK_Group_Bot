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
        private const string MatMexMemes = "134071529";
        private const string StandartPath = @"C:\Projects\VKGroupBot\Pics\";
        private readonly string accessToken = File.ReadAllText(@"C:\Projects\VKGroupBot\access_token.txt");

        public Bot()
        {
            Main();
        }

        private void Main()
        {
            //var matMexMemes = new Group(new KeyValuePair<string, string>(MatMexMemes, "MATMEX MEMES"), accessToken);
            //var posts = matMexMemes.GetPosts(500);
            //var bestPosts = matMexMemes.GetBestPosts(posts);
            //matMexMemes.SaveAll(bestPosts, StandartPath);

            DownloadGroupsContent(400);

            var request = new Request(MemeForceId, accessToken);
            request.PostPhoto(new[] { @"C:\Projects\VKGroupBot\Pics\1.png" }, "");
        }

        private void DownloadGroupsContent(int amount)
        {
            var groups = new GroupClaster(MemeForceId, accessToken);
            var groupsInfo = groups.GetLinks();
            foreach (var groupInfo in groupsInfo)
            {
                var matMexMemes = new Group(groupInfo, accessToken);
                var posts = matMexMemes.GetPosts(amount);
                var bestPosts = matMexMemes.GetBestPosts(posts);
                matMexMemes.SaveAll(bestPosts.ToList(), StandartPath);
            }
        }
    }
}
