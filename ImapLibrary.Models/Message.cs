﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ImapLibrary.Models
{
    public class Message
    {
        /// <summary>
        /// The Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// A message type so it can map to headers and get an instantiation type
        /// </summary>
        
        public string MessageType { get; set; }
        /// <summary>
        /// The contents of the message.
        /// </summary>
        public string Contents { get; set; }
        /*
        /// <summary>
        /// The date and time when the message was sent.  Filled in by the sender
        /// </summary>
        //public DateTime WhenSent { get; set; }
        /// <summary>
        /// The date and time when the message was received.  Filled in by the Controller
        /// </summary>
        //public DateTime? WhenReceived { get; set; }
         */
    }

}
