using System.Linq;
using Newtonsoft.Json.Linq;
using xNet.Collections;
using xNet.Net;
using System.Xml.Linq;

namespace Bot
{
    internal class Request
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

        public string PostAPost(string[] pathes, string message, long unixTime)
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
                    response = Get(request, methodName,
                        new StringDictionary { { "group_id", groupId }, { "access_token", accessToken } }, Format.Xml);
                    var responseXml = XDocument.Parse(response.ToString());
                    var uploadUrl = responseXml.Element("response").Element("upload_url").Value;
                    #endregion

                    #region Send the photo to the received address
                    response = Post(request, uploadUrl, path);
                    var responseJson = JObject.Parse(response.ToString());
                    var server = responseJson.Property("server").Value.ToString();
                    var photo = responseJson.Property("photo").Value.ToString();
                    var hash = responseJson.Property("hash").Value.ToString();

                    response = Get(request, "photos.saveWallPhoto",
                        new StringDictionary
                        {
                            {"group_id", groupId},
                            {"access_token", accessToken},
                            {"server", server},
                            {"photo", photo},
                            {"hash", hash}
                        }, Format.Xml);
                    responseXml = XDocument.Parse(response.ToString());
                    var id = responseXml.Element("response").Element("photo").Element("id").Value;
                    attachments += "," + id;
                    #endregion
                }

                #region Save information about the uploaded photos

                response = Get(request, "wall.post",
                    new StringDictionary
                    {
                        {"owner_id", "-" + groupId},
                        {"access_token", accessToken},
                        {"attachments", attachments},
                        {"from_group", "1"},
                        {"message", message},
                        {"publish_date", unixTime.ToString()}
                    }, Format.Xml);
                #endregion

                return response.ToString();
            }
        }
    }
}