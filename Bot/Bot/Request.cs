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

        public Request(string groupId, string accessToken)
        {
            this.groupId = groupId;
            this.accessToken = accessToken;
        }

        public HttpResponse Get(HttpRequest request, string methodName, StringDictionary keys)
        {
            return request.Get($"https://api.vk.com/method/{methodName}", keys);
        }

        public HttpResponse Post(HttpRequest request, string methodName, StringDictionary keys)
        {
            return request.Post($"https://api.vk.com/method/{methodName}", keys);
        }

        public HttpResponse Post(HttpRequest request, string uploadUrl, string path)
        {
            request.AddFile("photo", path);
            return request.Post(uploadUrl);
        }

        public string PostPhoto(string[] pathes, string message)
        {
            using (var request = new HttpRequest())
            {
                HttpResponse response;
                var attachments = "";
                foreach (var path in pathes)
                {
                    #region Get the address to upload the photo
                    var uploadUrl = GetUploadUrl(request, "photos.getWallUploadServer");
                    #endregion

                    #region Send the photo to the received address
                    response = Post(request, uploadUrl, path);
                    var responseString = response.ToString();
                    dynamic responseInJson = JsonParse(responseString);
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
                        });
                    responseString = response.ToString().Replace(']', ' ').Replace('[', ' ');
                    responseInJson = JsonParse(responseString);
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
                    });
                #endregion

                return response.ToString();
            }
        }

        private string GetUploadUrl(HttpRequest request, string methodName)
        {
            var uploadUrlResponse = Post(request, methodName, new StringDictionary { { "group_id", groupId }, { "access_token", accessToken } });
            var response = uploadUrlResponse.ToString();
            dynamic responseInJson = JObject.Parse(JsonConvert.DeserializeObject(response).ToString());
            return responseInJson.response.upload_url;
        }
        
        public static JObject JsonParse(string json)
        {
            return JObject.Parse(JsonConvert.DeserializeObject(json).ToString());
        }
    }
}