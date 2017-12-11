using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using xNet.Collections;
using xNet.Net;
using System.IO;
using System.Net.Http;
using System.Xml.Linq;

namespace Bot
{
    class Request
    {
        private readonly string groupId;
        private readonly string accessToken;

        public enum Format
        {
            Xml,
            Json
        }

        public Request(string groupId, string accessToken)
        {
            this.groupId = groupId;
            this.accessToken = accessToken;
        }

        public HttpResponse Get(HttpRequest request, string methodName, StringDictionary keys, Format format)
        {
            var response = request.Get($"https://api.vk.com/method/{methodName}" + (format == Format.Xml ? ".xml" : ""), keys);
            return response;
        }

        public HttpResponse Post(HttpRequest request, string methodName, StringDictionary keys, Format format)
        {
            var response = request.Post($"https://api.vk.com/method/{methodName}" + (format == Format.Xml ? ".xml" : ""), keys);
            return response;
        }

        public HttpResponse Post(HttpRequest request, string uploadUrl, string path)
        {
            request.AddFile("photo", path);
            var response = request.Post(uploadUrl);
            return response;
        }

        public string PostPhoto(string[] pathes, string message)
        {
            if (pathes.Length > 10)
                pathes = pathes.Take(10).ToArray();
            using (var request = new HttpRequest())
            {
                HttpResponse response;
                var attachments = "";
                foreach (var path in pathes)
                {
                    #region Get the address to upload the photo

                    var methodName = "photos.getWallUploadServer";
                    response = Post(request, methodName,
                        new StringDictionary { { "group_id", groupId }, { "access_token", accessToken } }, Format.Json);
                    dynamic responseInJson = JObject.Parse(JsonConvert.DeserializeObject(response.ToString()).ToString());
                    var uploadUrl = responseInJson.response.upload_url;
                    #endregion

                    #region Send the photo to the received address
                    response = Post(request, uploadUrl, path);
                    responseInJson = JsonParse(response.ToString());
                    string server = responseInJson.server;
                    string photo = responseInJson.photo;
                    string hash = responseInJson.hash;

                    response = Post(request, "photos.saveWallPhoto",
                        new StringDictionary
                        {
                            {"group_id", groupId},
                            {"access_token", accessToken},
                            {"server", server},
                            {"photo", photo},
                            {"hash", hash}
                        }, Format.Json);
                    responseInJson = JsonParse(response.ToString().Replace('[', ' ').Replace(']', ' '));
                    attachments += "," + responseInJson.response.id;
                    #endregion
                }

                #region Save information about the uploaded photos
                response = Post(request, "wall.post",
                    new StringDictionary
                    {
                        {"owner_id", "-" + groupId},
                        {"access_token", accessToken},
                        {"attachments", attachments},
                        {"from_group", "1"},
                        {"message", message }
                    }, Format.Xml);
                #endregion

                return response.ToString();
            }
        }

        private string GetUploadUrl(HttpRequest request, string methodName)
        {
            var uploadUrlResponse = Post(request, methodName,
                new StringDictionary { { "group_id", groupId }, { "access_token", accessToken } }, Format.Json);
            var response = uploadUrlResponse.ToString();
            dynamic responseInJson = JObject.Parse(JsonConvert.DeserializeObject(response).ToString());
            return responseInJson.response.upload_url;
        }

        private static JObject JsonParse(string json)
        {
            return JObject.Parse(JsonConvert.DeserializeObject(json).ToString());
        }
    }
}