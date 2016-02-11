using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImapLibrary.Models;
using Newtonsoft.Json;

namespace ImapLibrary.Data
{
    public static class TransactionFactory
    {
        public static Transaction CreateFromJson(string json, int id = 0)
        {
            Transaction t = JsonConvert.DeserializeObject<Transaction>(json);
            if (t.Id <= 0)
                t.Id = id;
            return t;
        }
    }
}
