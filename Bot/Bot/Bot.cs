using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Bot
{
    class Bot
    {
        private const string appId = "6289595";
        private const string MemeForceId = "158155713";
        private const string MatMexMemesId = "134071529";
        private const string StandartPath = @"C:\Projects\VKGroupBot\Pics\";
        private readonly string accessToken = File.ReadAllText(@"C:\Projects\VKGroupBot\access_token.txt");

        public Bot() { }

        public void Main()
        {
            Console.WriteLine("If you want to do full list of tasks, then enter Full" +
                              "\nIf you want to post additioanl posts, then enter Part" +
                              "\nPress Enter if you want to exit");
            var full = Console.ReadLine();
            if (full == "Full")
            {
                Console.WriteLine("If you sure you want to download memes and post part of them, then enter Yes" +
                                  "\nPress Enter if you want to exit");
                var sure = Console.ReadLine();
                if (sure == "Yes")
                {
                    Console.WriteLine("Type amount of posts that Bot will handle on every wall");
                    try
                    {
                        var amount = int.Parse(Console.ReadLine());
                        File.WriteAllText(StandartPath + "current_post_to_post.txt", "0");
                        File.WriteAllText(StandartPath + "current_post_to_save.txt", "0");
                        File.Delete(StandartPath + "order.txt");
                        File.Delete(StandartPath + "time.txt");
                        DownloadGroupsContent(amount);
                        PostAll(true);
                    }
                    catch
                    {
                        throw new Exception("Wrong amount");
                    }
                }
                if (string.IsNullOrEmpty(sure))
                    Console.WriteLine("The program completed without performing tasks");
                else
                    throw new Exception("Wrong answer on previous question");
            }
            if (full == "Part")
                PostAll(false);
            if (string.IsNullOrEmpty(full))
                Console.WriteLine("The program completed without performing tasks");
            else
                throw new Exception("Wrong answer on previous question");
        }

        private void PostAll(bool isFull)
        {
            var random = new Random();
            var postponementTime = 0;
            long currentTime;

            var directories = Directory.GetDirectories(StandartPath);
            var request = new Request(MemeForceId, accessToken);
            var currentPostToPost = int.Parse(File.ReadAllText(StandartPath + "current_post_to_post.txt"));
            var border = Math.Min(directories.Length, 150 + currentPostToPost);

            string[] orderArray;
            if (isFull)
            {
                orderArray = new string[directories.Length];
                for (var i = 0; i < orderArray.Length; i++)
                {
                    orderArray[i] = i.ToString();
                }
                orderArray = orderArray.OrderBy(x => random.Next()).ToArray();
                File.WriteAllLines(StandartPath + "order.txt", orderArray);
                currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            }
            else
            {
                currentTime = long.Parse(File.ReadAllLines(StandartPath + "time.txt")[0]);
                orderArray = File.ReadAllLines(StandartPath + "order.txt");
            }

            for (var i = currentPostToPost; i < border; i++, currentPostToPost++)
            {
                var directory = directories[int.Parse(orderArray[i])];
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
                postponementTime += nextRandom > 0.5 ? (int)(7200 * nextRandom) : 3600;
                var response = request.PostAPost(photos, text, currentTime + postponementTime);
                Console.WriteLine($"Posted the {i} post");
                Thread.Sleep(300);
            }
            File.WriteAllText(StandartPath + "time.txt", (currentTime + postponementTime).ToString());
            File.WriteAllText(StandartPath + "current_post_to_post.txt", currentPostToPost.ToString());
        }

        private void DownloadGroupsContent(int amount)
        {
            var groups = new GroupClaster();
            var groupsInfo = groups.GetLinks();
            foreach (var groupInfo in groupsInfo)
            {
                var group = new Group(groupInfo, accessToken);
                var posts = group.GetPosts(amount);
                var bestPosts = group.GetBestPosts(posts);
                group.SaveAll(bestPosts.ToList());
            }
        }
    }
}
