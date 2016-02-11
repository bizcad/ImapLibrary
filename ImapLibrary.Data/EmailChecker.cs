using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using ImapLibrary.Models;
using Joshi.Utils.Imap;
using Newtonsoft.Json;


namespace ImapLibrary.Data
{
    public class EmailChecker
    {
        private Imap _oImap;
        private string _sHost;
        private string _sPort;
        private string _sUser;
        private string _sPwd;
        private bool _bSSL;
        private string _sInbox;


        public EmailChecker(Imap oImap, string sHost, string sPort,
            string sUser, string sPwd, bool bSSL, string sInbox)
        {
            _oImap = oImap;
            _sHost = sHost;
            _sPort = sPort;
            _sUser = sUser;
            _sPwd = sPwd;
            _bSSL = bSSL;
            _sInbox = sInbox;
        }

        public string GetEmailBody(string sUid)
        {
            _oImap.Login(_sHost, Convert.ToUInt16(_sPort), _sUser, _sPwd, _bSSL);
            _oImap.SelectFolder(_sInbox);
            var sData = GetAndConvertBodyFromBase64(sUid).Replace("=\r\n", string.Empty);
            sData = sData.Replace("=\r\n", string.Empty);
            _oImap.LogOut();
            return sData;

        }
        public string GetEmailAttachment(string sUid)
        {
            _oImap.Login(_sHost, Convert.ToUInt16(_sPort), _sUser, _sPwd, _bSSL);
            _oImap.SelectFolder(_sInbox);
            var sData = GetAndConvertAttachmentFromBase64(sUid).Replace("=\r\n", string.Empty);
            sData = sData.Replace("=\r\n", string.Empty);
            _oImap.LogOut();
            return sData;

        }

        public ArrayList GetSearchEmail()
        {
            _oImap.Login(_sHost, Convert.ToUInt16(_sPort), _sUser, _sPwd, _bSSL);
            _oImap.SelectFolder(_sInbox);

            string sSearch = "ALL";
            string[] saSearchData = new String[1];
            saSearchData[0] = sSearch;
            ArrayList saArray = new ArrayList();
            _oImap.SearchMessage(saSearchData, false, saArray);
            _oImap.LogOut();
            return saArray;

        }
        public ArrayList ClearInbox()
        {
            _oImap.Login(_sHost, Convert.ToUInt16(_sPort), _sUser, _sPwd, _bSSL);
            _oImap.SelectFolder(_sInbox);

            string sSearch = "ALL";
            string[] saSearchData = new String[1];
            saSearchData[0] = sSearch;
            ArrayList saArray = new ArrayList();
            _oImap.SearchMessage(saSearchData, false, saArray);
            foreach (var item in saArray)
            {
                _oImap.DeleteMessage(item.ToString());
            }
            _oImap.LogOut();
            return saArray;

        }
        public void SendEmailToCsv()
        {
            // Login and select the Inbox
            _oImap.Login(_sHost, Convert.ToUInt16(_sPort), _sUser, _sPwd, _bSSL);
            _oImap.SelectFolder(_sInbox);
            _oImap.ConsoleLog(ImapBase.LogTypeEnum.INFO, "Checking for email");
            // Search for all messages.  The ids end up in saArray1
            string sSearch1 = "ALL";
            string[] saSearchData1 = new String[1];
            saSearchData1[0] = sSearch1;
            ArrayList saArray1 = new ArrayList();
            _oImap.SearchMessage(saSearchData1, false, saArray1);

            List<Transaction> transactions = new List<Transaction>();
            TransactionCsvWriter writer = new TransactionCsvWriter("transactions.txt");
            foreach (string sUid in saArray1)
            {
                if (sUid.Length > 0)
                {
                    string subject = _oImap.GetSubject(sUid);
                    _oImap.ConsoleLog(ImapBase.LogTypeEnum.INFO, string.Format("Message Id: {0} - {1}", sUid, subject));

                    if (subject.Contains("TradeBars"))
                    {
                        var sData = GetAndConvertAttachmentFromBase64(sUid).Replace("=\r\n", string.Empty);
                        sData = sData.Replace("=\r\n", string.Empty);
                        Message msg = MessageFactory.CreateFromJson(sData, System.Convert.ToInt32(sUid));
                        var mw = new MessageCsvWriter("messages.txt");
                        mw.WriteMessage(msg);
                        var x = PostMessage(typeof(Message).Name, sUid, sData);
                        if (null != x)
                        {
                            _oImap.DeleteMessage(sUid);
                        }

                    }
                    if (subject.Contains("Order Filled"))
                    {
                        var sData = GetAndConvertBodyFromBase64(sUid);
                        sData = sData.Replace("=\r\n", string.Empty);
                        //var x = PostMessage("string", sUid, sData);
                        sData = GetAndConvertAttachmentFromBase64(sUid).Replace("=\r\n", string.Empty);
                        sData = sData.Replace("=\r\n", string.Empty);
                        transactions.Add(TransactionFactory.CreateFromJson(sData, System.Convert.ToInt32(sUid)));
                        //writer.WriteTransactionJson(sData, sUid);

                        var x = PostMessage(typeof(Message).Name, sUid, sData);
                        if (null != x)
                        {
                            _oImap.DeleteMessage(sUid);
                        }
                    }
                    else
                    {
                        _oImap.DeleteMessage(sUid);
                    }
                }
            }

            writer.WriteTransactionList(transactions);

            _oImap.LogOut();
        }

        private string GetAndConvertBodyFromBase64(string sUid)
        {
            string s = string.Empty;
            string sData = string.Empty;
            string bodyStructure = _oImap.GetBodyStructure(sUid, false);
            string sPartPrefix = "";
            ArrayList asAttrs = new ArrayList();
            var x = _oImap.ParseBodyStructure(sUid, ref bodyStructure, sPartPrefix, false, out asAttrs);
            _oImap.FetchPartBody(sUid, "1", ref sData);

            // Restore the byte array.
            try
            {
                byte[] newBytes = Convert.FromBase64String(sData);
                //Console.WriteLine("The body: ");
                s = System.Text.Encoding.UTF8.GetString(newBytes, 0, newBytes.Length);
                //Console.WriteLine("   {0}\n", s);
            }
            catch (Exception ex)
            {
                return sData;
            }
            return s;
        }
        private string GetAndConvertAttachmentFromBase64(string sUid)
        {
            string s = string.Empty;
            string sData = string.Empty;
            _oImap.FetchPartBody(sUid, "2", ref sData);

            // Restore the byte array.
            try
            {
                byte[] newBytes = Convert.FromBase64String(sData);
                //Console.WriteLine("The Attachment: ");
                s = System.Text.Encoding.UTF8.GetString(newBytes, 0, newBytes.Length);
                //Console.WriteLine("   {0}\n", s);
            }
            catch (Exception ex)
            {
                return sData;
            }
            return s;
        }

        public static string PostMessage(String messageType, string sUid, string logmsg)
        {
            Message message = new Message
            {
                Id = System.Convert.ToInt32(sUid),
                MessageType = messageType,
                Contents = logmsg
            };
            string jsonmessage = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.UTF8;
            client.Headers.Add("Content-Type", "application/json");

            // Posts a message to the web site
            string address = @"http://bizcadsignalrchat.azurewebsites.net/Messages/Create";
            //string address = @""http://localhost:64527/Messages/Create/";
            return client.UploadString(address, jsonmessage);
            //string reply = client.GetMessage("http://localhost:64253/api/Messages/", jsonmessage);
        }
    }
}
