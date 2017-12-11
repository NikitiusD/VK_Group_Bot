﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using xNet.Collections;
using xNet.Net;

namespace Bot
{
    class Group
    {
        private readonly string groupId;
        private readonly string groupName;
        private readonly string accessToken;
        private readonly List<WallPost> posts = new List<WallPost>();

        private double averageOfLikes = 0.0;
        private double averageOfReposts = 0.0;
        const int step = 40;

        public Group(KeyValuePair<string, string> groupId, string accessToken)
        {
            this.groupId = groupId.Key;
            this.groupName = (new string(groupId.Value.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray())).Trim();
            this.accessToken = accessToken;
        }

        public IEnumerable<WallPost> GetPosts(int amount)
        {
            var request = new Request(groupId, accessToken);
            Console.WriteLine($"Parsing {groupName} right now");
            using (var httpRequest = new HttpRequest())
            {
                var countOfPosts = 1;
                for (var i = 0; i < Math.Min(countOfPosts, amount); i += step)
                {
                    Thread.Sleep(300);
                    //Console.WriteLine($"Now at {i} post");
                    var response = request.Get(httpRequest, "wall.get", new StringDictionary()
                    {
                        {"owner_id", $"-{groupId}"},
                        {"access_token", accessToken},
                        {"offset", $"{i}"},
                        {"count", $"{step}"}
                    }, Request.Format.Xml);

                    var responseXml = XDocument.Parse(response.ToString());
                    if (i == 0) countOfPosts = int.Parse(responseXml.Element("response").Element("count").Value);
                    foreach (var post in responseXml.Element("response").Elements("post"))
                    {
                        try
                        {
                            var likes = post.Element("likes").Element("count").Value;
                            var reposts = post.Element("reposts").Element("count").Value;
                            var text = post.Element("text").Value;
                            var photos = post.Elements("attachments").Elements("attachment").Elements("photo")
                                .Select(photo => photo.Element("src_big").Value).ToArray();
                            if (!photos.IsEmpty())
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
                averageOfLikes = Math.Round(averageOfLikes, 2);
                averageOfReposts /= posts.Count;
                averageOfReposts = Math.Round(averageOfReposts, 2);
                Console.WriteLine($"Parsing of the {groupName} was successful");
                return posts;
            }
        }

        public List<WallPost> GetBestPosts(IEnumerable<WallPost> posts)
        {
            var bestPosts = posts
                .Where(x => int.Parse(x.likes) >= averageOfLikes * 2.5 || int.Parse(x.reposts) >= averageOfReposts * 4)
                .Select(x => x).ToList();
            Console.WriteLine($"The selection of {groupName} was successful\nLikes avg = {averageOfLikes} and reposts avg = {averageOfReposts}");
            return bestPosts;
        }

        public void SaveAll(List<WallPost> bestPosts, string path)
        {
            path = $@"C:\Projects\VKGroupBot\Pics\{groupName}\";
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
            Console.WriteLine($"Downloading the {groupName} was successful");
        }
    }
}