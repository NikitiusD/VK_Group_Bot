using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using xNet.Collections;
using xNet.Net;

namespace Bot
{
    class Request
    {
        private readonly string groupId;
        private readonly string accessToken;

        public Request(string groupId, string accessToken)
        {
            this.groupId = groupId;
            this.accessToken = accessToken;
        }

        public HttpResponse Get(HttpRequest request, string methodName, StringDictionary keys)
        {
            var response = request.Get($"https://api.vk.com/method/{methodName}", keys);
            return response;
        }

        public HttpResponse Post(HttpRequest request, string methodName, StringDictionary keys)
        {
            var response = request.Post($"https://api.vk.com/method/{methodName}", keys);
            return response;
        }

        public HttpResponse Post(HttpRequest request, string uploadUrl, string path)
        {
            request.AddFile("photo", path);
            var response = request.Post(uploadUrl);
            return response;
        }

        public string PostPhoto(string path)
        {
            using (var request = new HttpRequest())
            {
                var uploadUrl = GetUploadUrl(request, "photos.getWallUploadServer");
                Console.WriteLine("1 complete");

                var response = Post(request, uploadUrl, path);
                dynamic c = JObject.Parse(JsonConvert.DeserializeObject(response.ToString()).ToString());
                string server = c.server;
                string photo = c.photo;
                string hash = c.hash;
                Console.WriteLine("2 complete");

                response = Post(request, "photos.saveWallPhoto",
                    new StringDictionary
                    {
                        {"group_id", groupId},
                        {"access_token", accessToken},
                        {"server", server},
                        {"photo", photo},
                        {"hash", hash}
                    });
                dynamic e = JObject.Parse(JsonConvert
                    .DeserializeObject(response.ToString().Replace(']', ' ').Replace('[', ' ')).ToString());
                string attachments = e.response.id;
                Console.WriteLine("3 complete");

                response = Post(request, "wall.post",
                    new StringDictionary
                    {
                        {"owner_id", "-" + groupId},
                        {"access_token", accessToken},
                        {"attachments", attachments},
                        {"from_group", "1"}
                    });
                Console.WriteLine("4 complete");

                return response.ToString();
            }
        }

        private string GetUploadUrl(HttpRequest request, string methodName)
        {
            var uploadUrlResponse = Post(request, methodName, new StringDictionary { { "group_id", groupId }, { "access_token", accessToken } });
            var a = uploadUrlResponse.ToString();
            dynamic b = JObject.Parse(JsonConvert.DeserializeObject(a).ToString());
            return b.response.upload_url;
        }
    }
}