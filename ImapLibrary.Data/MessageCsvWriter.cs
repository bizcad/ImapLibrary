using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImapLibrary.Models;

namespace ImapLibrary.Data
{
    public class MessageCsvWriter: ICsvMessageWriter<Message>
    {
        private readonly string _filepath;
        private bool _appendFields = true;
        #region "Constructors"
        /// <summary>
        /// Constructor with the filename as a parameter
        /// </summary>
        /// <param name="filename">string - the name of the file to be written</param>
        public MessageCsvWriter(string filename)
        {
            _filepath = AssemblyLocator.ExecutingDirectory() + filename;
            _appendFields = AppendFields();
        }
        #endregion

        public void WriteMessageList(List<Message> transactions)
        {
            WriteCsvMessageList(transactions);
        }
        /// <summary>
        /// Writes a single json message to a file appending if it already exists.
        /// </summary>
        /// <param name="json">string - the json of the transaction</param>
        /// <param name="messageId">string - the Imap message Id</param>
        public void WriteMessageJson(string json, string messageId)
        {
            var transactions = new List<Message>
            {
                MessageFactory.CreateFromJson(json, System.Convert.ToInt32(messageId))
            };
            WriteCsvMessageList(transactions);
        }
        /// <summary>
        /// Writes a single Message to the file, prepending column titles if the file does not exist
        /// and append a csv line when the file does not exist.
        /// </summary>
        /// <param name="transaction"></param>
        public void WriteMessage(Message transaction)
        {
            WriteMessageList(new List<Message> { transaction });
        }
        /// <summary>
        /// Writes a transaction list as a csv file
        /// </summary>
        /// <param name="list">the list of transactions</param>
        public void WriteCsvMessageList(List<Message> list)
        {
            var csv = CsvSerializer.Serialize(",", list, AppendFields());
            using (var fs = new StreamWriter(_filepath, true))
            {
                foreach (var item in csv)
                {
                    fs.WriteLine(item);
                }
                fs.Flush();
                fs.Close();
            }
        }
        #region "Private"
        private bool AppendFields()
        {
            if (_appendFields)
            {
                if (File.Exists(_filepath))
                    _appendFields = false;
            }
            return _appendFields;
        }
        #endregion 

    }
}
