using System;
using System.Collections;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Xml;
using ImapLibraryTest;
using Joshi.Utils.Imap;
using System.IO;
using ImapLibrary.Data;

namespace ConsoleApplication1
{
    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    class Class1
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string sHost = "imap.1and1.com";
            string sPort = "993";
            string sSslEnabled = "true";
            bool bSSL = true;
            string sUser = "quantconnect@bizcad.com";
            string sPwd = "Wolfst93";
            string sInbox = "INBOX";
            XmlTextWriter oXmlWriter;
            System.Threading.Timer _timerThread = null;
            int _period = 2000;

            ImapLibrary.Data.EmailChecker checker;

            Imap oImap = new Imap();
            bool bNotExit = true;
            while (bNotExit)
            {
                try
                {
                    Console.WriteLine("Select the action");
                    Console.WriteLine("1    Login");
                    Console.WriteLine("2    Select/Examine");
                    Console.WriteLine("3    Search");
                    Console.WriteLine("4    FetchHeader");
                    Console.WriteLine("5    MoveMessage");
                    Console.WriteLine("6    DeleteMessage");
                    Console.WriteLine("7    MarkMessageUnRead");
                    Console.WriteLine("8    GetQuota");
                    Console.WriteLine("9    GetMessageSize");
                    Console.WriteLine("10   Logout");
                    Console.WriteLine("11   Exit");
                    Console.WriteLine("20   Email To Csv");
                    Console.WriteLine("22   Display Body");
                    Console.WriteLine("23   Display Attachment");
                    Console.WriteLine("26   Clear Inbox");
                    Console.WriteLine("88   Stop Checking for Email");
                    Console.WriteLine("99   Begin Checking for Email");

                    Console.Write("Input  :[11]");
                    string sInput = Console.ReadLine();
                    if (sInput.Length > 0)
                    {
                        int input = 0;
                        try
                        {
                            input = Convert.ToInt32(sInput, 10);
                        }
                        catch (FormatException e)
                        {
                            Console.WriteLine("Error:{0}:{1}", e.Message, e.InnerException);
                        }
                        switch (input)
                        {
                            case 1:
                                {
                                    Console.WriteLine("########################################");
                                    Console.WriteLine("Host:{0}", sHost);
                                    Console.WriteLine("Port:{0}", sPort);
                                    Console.WriteLine("SSLEnabled:{0}", bSSL.ToString());
                                    Console.WriteLine("User:{0}", sUser);
                                    Console.WriteLine("########################################");
                                    oImap.Login(sHost, Convert.ToUInt16(sPort), sUser, sPwd, bSSL);
                                }
                                break;

                            case 2:
                                {
                                    //Console.Write("Folder: [INBOX]");
                                    //sInbox = Console.ReadLine();
                                    sInbox = "INBOX";
                                    //Console.Write("Select:s Examine:e [s]");
                                    //string sChoice = Console.ReadLine();
                                    string sChoice = "s";
                                    if (sInbox.Length < 1)
                                        sInbox = "INBOX";
                                    if (sChoice == "e")
                                        oImap.ExamineFolder(sInbox);
                                    else
                                        oImap.SelectFolder(sInbox);

                                }
                                break;
                            case 3:
                                {
                                    checker = new EmailChecker(oImap, sHost, sPort, sUser, sPwd, bSSL, sInbox);
                                    ArrayList saArray = checker.GetSearchEmail();
                                    foreach (var line in saArray)
                                    {
                                        Console.WriteLine(line);
                                    }
                                    
                                }
                                break;
                            case 4:
                                {
                                    oImap.Login(sHost, Convert.ToUInt16(sPort), sUser, sPwd, bSSL);
                                    oImap.SelectFolder(sInbox);
                                    Console.Write("Message UID[]:");
                                    string sUid = Console.ReadLine();
                                    if (sUid.Length < 1)
                                        sUid = "";
                                    Console.Write("Fetch Body:[true]");
                                    string sFetchBody = Console.ReadLine();
                                    if (sFetchBody.Length < 1)
                                        sFetchBody = "true";
                                    bool bFetchBody = sFetchBody.ToLower() == "true";
                                    ArrayList saArray = new ArrayList();
                                    string sFileName = sUid + ".xml";
                                    oXmlWriter = new XmlTextWriter(sFileName, System.Text.Encoding.UTF8);
                                    FetchMessageXML(oXmlWriter, sUid, oImap, bFetchBody);
                                    oXmlWriter.Close();

                                }
                                break;
                            case 5:
                                {
                                    Console.Write("Message UID:");
                                    string sUid = Console.ReadLine();
                                    Console.Write("Folder To Move:");
                                    string sFolder = Console.ReadLine();
                                    oImap.MoveMessage(sUid, sFolder);
                                }
                                break;
                            case 6:
                                {
                                    oImap.Login(sHost, Convert.ToUInt16(sPort), sUser, sPwd, bSSL);
                                    oImap.SelectFolder(sInbox);
                                    Console.Write("Message UID:");
                                    string sUid = Console.ReadLine();
                                    oImap.SetFlag(sUid, "\\Deleted");
                                    oImap.Expunge();
                                }
                                break;
                            case 7:
                                {
                                    Console.Write("Message UID:");
                                    string sUid = Console.ReadLine();
                                    oImap.SetFlag(sUid, "\\Seen", true);
                                    oImap.Expunge();
                                }
                                break;
                            case 8:
                                {
                                    bool bUnlimitedQuota = false;
                                    int nUsedKBytes = 0;
                                    int nTotalKBytes = 0;

                                    oImap.GetQuota("inbox", ref bUnlimitedQuota, ref nUsedKBytes, ref nTotalKBytes);
                                    Console.WriteLine("Unlimitedquota:{0}, UsedKBytes:{1}, TotalKBytes:{2}",
                                        bUnlimitedQuota, nUsedKBytes, nTotalKBytes);
                                }
                                break;
                            case 9:
                                {
                                    Console.Write("Message UID:");
                                    string sUid = Console.ReadLine();

                                    oImap.Login(sHost, Convert.ToUInt16(sPort), sUser, sPwd, bSSL);
                                    oImap.SelectFolder(sInbox);


                                    long size = oImap.GetMessageSize(sUid);
                                    Console.WriteLine("Message size {0}",size);

                                }
                                break;
                            case 10:
                                oImap.LogOut();
                                break;
                            case 11:
                                oImap.LogOut();
                                if (_timerThread != null)
                                {
                                    _timerThread.Dispose();
                                }
                                bNotExit = false;
                                break;
                            case 20:
                                checker = new EmailChecker(oImap, sHost, sPort, sUser, sPwd, bSSL, sInbox);
                                checker.SendEmailToCsv();
                                checker = null;
                                break;
                            case 22:
                                Console.Write("Message UID[]:");
                                string bodyUid = Console.ReadLine();
                                if (bodyUid.Length < 1)
                                {
                                    Console.WriteLine("Message Id cannot be blank.");
                                    bodyUid = "";
                                }
                                else
                                {
                                    checker = new EmailChecker(oImap, sHost, sPort, sUser, sPwd, bSSL, sInbox);
                                    Console.WriteLine(checker.GetEmailBody(bodyUid));
                                }
                                break;
                            case 23:
                                Console.Write("Message UID[]:");
                                string attachmentUid = Console.ReadLine();
                                if (attachmentUid.Length < 1)
                                {
                                    Console.WriteLine("Message Id cannot be blank.");
                                    bodyUid = "";
                                }
                                else
                                {
                                    checker = new EmailChecker(oImap, sHost, sPort, sUser, sPwd, bSSL, sInbox);
                                    Console.WriteLine(checker.GetEmailAttachment(attachmentUid));
                                }
                                break;
                            case 26:
                                checker = new EmailChecker(oImap, sHost, sPort, sUser, sPwd, bSSL, sInbox);
                                Console.WriteLine(checker.ClearInbox());
                                break;
                            case 88:
                                if (_timerThread != null)
                                {
                                    // Stop the timer;
                                    _timerThread.Change(Timeout.Infinite, Timeout.Infinite);

                                    _timerThread.Dispose();
                                }
                                checker = null;
                                break;
                            case 99:
                                // Create an event to signal the timeout count threshold in the timer callback.
                                checker = new EmailChecker(oImap, sHost, sPort, sUser, sPwd, bSSL, sInbox);

                                AutoResetEvent autoEvent = new AutoResetEvent(false);
                                //StatusChecker statusChecker = new StatusChecker(10);
                                //// Create an inferred delegate that invokes methods for the timer.
                                //// TimerCallback tcb = statusChecker.CheckStatus;
                                //TimerCallback tcb = checker.CheckEmail;

                                //// Create a timer that signals the delegate to invoke 
                                //// CheckStatus after one second, and every 1/4 second 
                                //// thereafter.
                                //Console.WriteLine("{0} Creating timer.\n", DateTime.Now.ToString("h:mm:ss.fff"));
                                //Timer stateTimer = new Timer(tcb, autoEvent, 1000, 250);

                                _timerThread = new System.Threading.Timer((o) =>
                                {
                                    // Stop the timer;
                                    _timerThread.Change(Timeout.Infinite, Timeout.Infinite);

                                    // Process your data
                                    checker.SendEmailToCsv();

                                    // start timer again (BeginTime, Interval)
                                    _timerThread.Change(5000, _period);
                                }, null, (long)0, _period);




                                //// When autoEvent signals, change the period to every
                                //// 1/2 second.
                                //autoEvent.WaitOne(5000, false);
                                //stateTimer.Change(0, 500);
                                //Console.WriteLine("\nChanging period.\n");

                                //// When autoEvent signals the second time, dispose of 
                                //// the timer.
                                //autoEvent.WaitOne(5000, false);
                                //stateTimer.Dispose();
                                //Console.WriteLine("\nDestroying timer.");

                                //CheckEmail(oImap, sHost, sPort, sUser, sPwd, bSSL, sInbox);

                                break;
                        }
                    }
                }
                catch (ImapException e)
                {
                    Console.WriteLine("Error:{0}:{1}", e.Message, e.InnerException);
                }
            }


        }

        //private static void CheckEmail(Imap oImap, string sHost, string sPort, string sUser, string sPwd, bool bSSL,
        //    string sInbox)
        //{
        //    // Login and select the Inbox
        //    oImap.Login(sHost, Convert.ToUInt16(sPort), sUser, sPwd, bSSL);
        //    oImap.SelectFolder(sInbox);

        //    // Search for all messages.  The ids end up in saArray1
        //    string sSearch1 = "ALL";
        //    string[] saSearchData1 = new String[1];
        //    saSearchData1[0] = sSearch1;
        //    ArrayList saArray1 = new ArrayList();
        //    oImap.SearchMessage(saSearchData1, false, saArray1);

        //    foreach (string sUid in saArray1)
        //    {
        //        string subject = oImap.GetSubject(sUid);
        //        if (subject.Contains("TradeBars"))
        //        {
        //            string sData = string.Empty;
        //            oImap.FetchPartBody(sUid, "", ref sData);
        //            string nData = sData.Replace("=\r\n", string.Empty);
        //            var x = PostMessage(typeof (Message).Name, sUid, nData);
        //            if (null != x)
        //            {
        //                oImap.DeleteMessage(sUid);
        //            }
        //        }
        //    }

        //    oImap.LogOut();
        //}

        private static void FetchMessageXML(XmlTextWriter oXmlWriter, string sUid, Imap oImap, bool bFetchBody)
        {
            oXmlWriter.Formatting = Formatting.Indented;
            oXmlWriter.WriteStartDocument(true);
            oXmlWriter.WriteStartElement("Message");
            oXmlWriter.WriteAttributeString("UID", sUid);
            oImap.FetchMessage(sUid, oXmlWriter, bFetchBody);
            oXmlWriter.WriteEndElement();
            oXmlWriter.WriteEndDocument();
            oXmlWriter.Flush();

        }

        /// <summary>
        /// Read password
        /// </summary>
        /// <returns></returns>
        public static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    password += info.KeyChar;
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        // remove one character from the list of password characters
                        password = password.Substring(0, password.Length - 1);
                        // get the location of the cursor
                        int pos = Console.CursorLeft;
                        // move the cursor to the left by one character
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        // replace it with space
                        Console.Write(" ");
                        // move the cursor to the left by one character again
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
                info = Console.ReadKey(true);
            }

            // add a new line because user pressed enter at the end of their password
            Console.WriteLine();
            return password;
        }
        //public static string PostMessage(String messageType, string sUid, string logmsg)
        //{
        //    Message message = new Message
        //    {
        //        Id = System.Convert.ToInt32(sUid),
        //        MessageType = messageType,
        //        Contents = logmsg
        //    };
        //    string jsonmessage = Newtonsoft.Json.JsonConvert.SerializeObject(message);
        //    WebClient client = new WebClient();
        //    client.Encoding = System.Text.Encoding.UTF8;
        //    client.Headers.Add("Content-Type", "application/json");

        //    // Posts a message to the web site
        //    return client.UploadString("http://localhost:64253/api/Messages/", jsonmessage);
        //    //string reply = client.GetMessage("http://localhost:64253/api/Messages/", jsonmessage);
        //}
    }
}
