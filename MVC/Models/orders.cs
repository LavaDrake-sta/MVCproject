//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MVC.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class orders
    {
        public double order_number { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public double id { get; set; }
        public string card_owner_name { get; set; }
        public double card_number { get; set; }
        public string expiry_date { get; set; }
        public double CVC { get; set; }
        public double number_of_payments { get; set; }
        public decimal price { get; set; }
        public string product { get; set; }
        public string buy_borrow { get; set; }
        public System.DateTime date { get; set; }
    }
}
