using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using xNet.Collections;
using xNet.Net;

namespace Bot
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new Bot();
            bot.DownloadGroupsContent(400);
            bot.PostAll();
        }
    }
}
