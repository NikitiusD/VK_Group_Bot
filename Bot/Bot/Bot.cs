using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
        private const string groupId = "158155713";
        private readonly string accessToken = File.ReadAllText(@"C:\Projects\VKGroupBot\access_token.txt");
        
        public Bot()
        {
            Main();
        }

        private void Main()
        {
            var request = new Request(groupId, accessToken);
            var response = request.PostPhoto(@"C:\Projects\VKGroupBot\Pics\pic.png");
            Console.WriteLine(response);
        }
    }
}
