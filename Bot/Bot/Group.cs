using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using xNet.Collections;
using xNet.Net;

namespace Bot
{
    class Group
    {
        private readonly string groupId;
        private readonly string accessToken;
        private readonly List<WallPost> posts = new List<WallPost>();

        private double averageOfLikes = 0.0;
        private double averageOfReposts = 0.0;

        public Group(string groupId, string accessToken)
        {
            this.groupId = groupId;
            this.accessToken = accessToken;
        }

        public IEnumerable<WallPost> GetPosts(int amount)
        {
            var req = new Request(groupId, accessToken);
            Console.WriteLine($"Parsing {groupId} right now");
            using (var request = new HttpRequest())
            {
                var countOfPosts = 1;
                var step = 40;
                for (var i = 0; i < Math.Min(countOfPosts, amount); i += step)
                {
                    Thread.Sleep(300);
                    Console.WriteLine($"Now at {i} post");
                    var response = req.Get(request, "wall.get", new StringDictionary()
                    {
                        {"owner_id", $"-{groupId}"},
                        {"access_token", accessToken},
                        {"offset", $"{i}"},
                        {"count", $"{step}"}
                    });
                    var responseString = response.ToString();
                    var responseInJson = WallPostJson.FromJson(responseString);
                    if (i == 0) countOfPosts = (int)responseInJson.Response[0].Integer;
                    var cuttedResponseInJson = responseInJson.Response.Skip(1).ToArray();
                    foreach (var cuttedResponse in cuttedResponseInJson)
                    {
                        try
                        {
                            var photos = cuttedResponse.PurpleResponse.Attachments.Select(x => x.Photo.SrcBig.ToString()).ToArray();
                            var likes = cuttedResponse.PurpleResponse.Likes.Count.ToString();
                            var reposts = cuttedResponse.PurpleResponse.Reposts.Count.ToString();
                            var text = cuttedResponse.PurpleResponse.Text;
                            posts.Add(new WallPost(likes, reposts, photos, text));
                        }
                        catch { }
                    }
                }
                foreach (var post in posts)
                {
                    averageOfLikes += int.Parse(post.likes);
                    averageOfReposts += int.Parse(post.reposts);
                }
                averageOfLikes /= posts.Count;
                averageOfReposts /= posts.Count;
                Console.WriteLine($"Parsing of the {groupId} was successful");
                return posts;
            }
        }

        public IEnumerable<WallPost> GetBestPosts(IEnumerable<WallPost> posts)
        {
            Console.WriteLine("The selection was successful");
            return posts
                .Where(x => int.Parse(x.likes) >= averageOfLikes * 2 || int.Parse(x.reposts) >= averageOfReposts * 3)
                .Select(x => x);

        }

        public void SaveAll(List<WallPost> bestPosts, string path)
        {
            path = $@"C:\Projects\VKGroupBot\Pics\{groupId}\";
            Directory.CreateDirectory(path);

            for (var i = 0; i < bestPosts.Count; i++)
            {
                using (var client = new WebClient())
                {
                    for (var j = 0; j < bestPosts[i].photosUrls.Length; j++)
                    {
                        client.DownloadFile(bestPosts[i].photosUrls[j], path + i + "_" + j + ".jpg");
                    }
                }
                if (string.IsNullOrEmpty(bestPosts[i].text)) continue;
                using (var sw = File.CreateText(path + i + ".txt"))
                {
                    sw.WriteLine(bestPosts[i].text.Replace("<br>", " "));
                }
            }
            Console.WriteLine($"Downloading the {groupId} was successful");
        }
    }
}
