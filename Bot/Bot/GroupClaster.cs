using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using xNet.Collections;
using xNet.Net;

namespace Bot
{
    internal class GroupClaster
    {
        private string memeForceId;
        private string accessToken;

        public GroupClaster(string memeForceId, string accessToken)
        {
            this.memeForceId = memeForceId;
            this.accessToken = accessToken;
        }

        public Dictionary<string, string> GetLinks()
        {
            using (var httpRequest = new HttpRequest())
            {
                var methodName = "groups.getById";
                var response = httpRequest.Get($"https://api.vk.com/method/{methodName}.xml", new StringDictionary()
                {
                    {"group_id", memeForceId},
                    {"access_token", accessToken},
                    {"fields", "links"}
                });

                var responseXml = XDocument.Parse(response.ToString());
                var shortNames = responseXml.Element("response").Element("group").Element("links").Elements("url")
                    .Select(x => x.Value.ToString().Split('/')[3]);
                methodName = "groups.getById";
                response = httpRequest.Get($"https://api.vk.com/method/{methodName}.xml",
                    new StringDictionary { { "group_ids", string.Join(",", shortNames) } });
                responseXml = XDocument.Parse(response.ToString());
                var ids = responseXml.Element("response").Elements("group").Select(url => url.Element("gid").Value).ToArray();
                var urls = responseXml.Element("response").Elements("group").Select(url => url.Element("name").Value).ToArray();
                var pairs = new Dictionary<string, string>();
                for (var i = 0; i < ids.Length; i++)
                    pairs.Add(ids[i], urls[i]);
                return pairs;
            }
        }
    }
}