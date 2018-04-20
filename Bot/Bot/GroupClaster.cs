using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using xNet.Collections;
using xNet.Net;

namespace Bot
{
    internal class GroupClaster
    {
        public Dictionary<string, string> GetLinks()
        {
            using (var httpRequest = new HttpRequest())
            {
                var shortNames = File.ReadAllLines(@"C:\Projects\VKGroupBot\publics.txt");
                var methodName = "groups.getById";
                var response = httpRequest.Get($"https://api.vk.com/method/{methodName}.xml",
                    new StringDictionary { { "group_ids", string.Join(",", shortNames) }, { "version", "5.74" } });
                var responseXml = XDocument.Parse(response.ToString());
                var ids = responseXml.Element("response").Elements("group").Select(x => x.Element("gid").Value).ToArray();
                var urls = responseXml.Element("response").Elements("group").Select(x => x.Element("name").Value).ToArray();
                var pairs = new Dictionary<string, string>();
                for (var i = 0; i < ids.Length; i++)
                    pairs.Add(ids[i], urls[i]);
                return pairs;
            }
        }
    }
}