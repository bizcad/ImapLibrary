using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImapLibrary.Models;
using Newtonsoft.Json;

namespace ImapLibrary.Data
{
    public class MessageFactory
    {
        public static Message CreateFromJson(string json, int id = 0)
        {
            Message t = JsonConvert.DeserializeObject<Message>(json);
            t.Id = id;
            return t;
        }
    }
}
