using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImapLibrary.Models;
using Newtonsoft.Json;

namespace ImapLibrary.Data
{
    /// <summary>
    /// A class for writing email messages to a file as csv
    /// </summary>
    public class TransactionCsvWriter : ICsvMessageWriter<Transaction>
    {
        private readonly string _filepath;
        private bool _appendFields = true;
        #region "Constructors"
        /// <summary>
        /// Empty constructor
        /// </summary>
        public TransactionCsvWriter()
        {
            _filepath = AssemblyLocator.ExecutingDirectory() + "messagelog.text";
        }
        /// <summary>
        /// Constructor with the filename as a parameter
        /// </summary>
        /// <param name="filename">string - the name of the file to be written</param>
        public TransactionCsvWriter(string filename)
        {
            _filepath = AssemblyLocator.ExecutingDirectory() + filename;
            _appendFields = AppendFields();
        }
        #endregion
        #region "non async methods"
        /// <summary>
        /// Writes a Transaction list to a file;
        /// </summary>
        /// <param name="transactions"></param>
        public void WriteTransactionList(List<Transaction> transactions)
        {
            WriteCsvMessageList(transactions);
        }
        /// <summary>
        /// Writes a single json message to a file appending if it already exists.
        /// </summary>
        /// <param name="json">string - the json of the transaction</param>
        /// <param name="messageId">string - the Imap message Id</param>
        public void WriteTransactionJson(string json, string messageId)
        {
            var transactions = new List<Transaction>
            {
                TransactionFactory.CreateFromJson(json, System.Convert.ToInt32(messageId))
            };
            WriteCsvMessageList(transactions);
        }
        /// <summary>
        /// Writes a single Transaction to the file, prepending column titles if the file does not exist
        /// and append a csv line when the file does not exist.
        /// </summary>
        /// <param name="transaction"></param>
        public void WriteTransaction(Transaction transaction)
        {
            WriteTransactionList(new List<Transaction> { transaction });
        }
        /// <summary>
        /// Writes a transaction list as a csv file
        /// </summary>
        /// <param name="list">the list of transactions</param>
        public void WriteCsvMessageList(List<Transaction> list)
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

        #endregion
        #region "Async Methods"
        /// <summary>
        /// Writes a Transaction list to a file;
        /// </summary>
        /// <param name="transactions"></param>
        public async Task WriteTransactionListAsync(List<Transaction> transactions)
        {
            await WriteCsvMessageListAsync(transactions);
        }
        /// <summary>
        /// Writes a single json message async to a file appending if it already exists.
        /// </summary>
        /// <param name="json">string - the json of the transaction</param>
        /// <param name="messageId">string - the Imap message Id</param>
        public async Task WriteTransactionJsonAsync(string json, string messageId)
        {
            var transactions = new List<Transaction>
            {
                TransactionFactory.CreateFromJson(json, System.Convert.ToInt32(messageId))
            };
            await WriteCsvMessageListAsync(transactions);
        }
        /// <summary>
        /// Writes a single Transaction to the file, prepending column titles if the file does not exist
        /// and append a csv line when the file does not exist.
        /// </summary>
        /// <param name="transaction"></param>
        public async Task WriteTransactionAsync(Transaction transaction)
        {
            await WriteTransactionListAsync(new List<Transaction> { transaction });
        }

        /// <summary>
        /// Asynchronosly writes a transaction list to a file
        /// </summary>
        /// <param name="list">the list of transactions</param>
        /// <returns>nothing</returns>
        public async Task WriteCsvMessageListAsync(List<Transaction> list)
        {
            var csv = CsvSerializer.Serialize(",", list, AppendFields());
            using (var fs = new StreamWriter(_filepath, true))
            {
                foreach (var item in csv)
                {
                    await fs.WriteLineAsync(item);
                }
                fs.Flush();
                fs.Close();
            }
        }
        #endregion 
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
