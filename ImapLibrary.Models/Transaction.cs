using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImapLibrary.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public string Exchange { get; set; }
        public string Broker { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string ActionNameUS { get; set; }
        public string TradeDate { get; set; }
        public string SettledDate { get; set; }
        public double Interest { get; set; }
        public double Amount { get; set; }
        public double Commission { get; set; }
        public double Fees { get; set; }
        public double Net { get; set; }
        public string CUSIP { get; set; }
        public string Description { get; set; }
        public int ActionId { get; set; }
        public int TradeNumber { get; set; }
        public string RecordType { get; set; }
        public string TaxLotNumber { get; set; }
        public int OrderType { get; set; }
        public int OrderId { get; set; }
        public int Direction { get; set; }
    }
}
