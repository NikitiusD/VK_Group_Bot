using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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
        }

        public void PostAll()
        {
            var twoHours = 7200;
            var random = new Random();
            var postponementTime = 0;
            var currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            var directories = Directory.GetDirectories(StandartPath);
            var request = new Request(MemeForceId, accessToken);
            var currentPostToPost = int.Parse(File.ReadAllText(StandartPath + @"\current_post_to_post.txt"));
            var border = Math.Min(directories.Length, 3 + currentPostToPost);
            for (var i = currentPostToPost; i < border; i++ , currentPostToPost++)
            {
                var directory = directories[i];
                var photos = Directory.GetFiles(directory, "*.jpg");
                string text;
                try
                {
                    text = File.ReadAllText(directory + @"\text.txt");
                }
                catch
                {
                    text = "";
                }
                var nextRandom = random.NextDouble();
                postponementTime += nextRandom > 0.5 ? (int)(twoHours * nextRandom) : 3600;
                var response = request.PostAPost(photos, text, currentTime + postponementTime);
            }
            File.WriteAllText(StandartPath + @"\current_post_to_post.txt", currentPostToPost.ToString());
        }

        public void DownloadGroupsContent(int amount)
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
