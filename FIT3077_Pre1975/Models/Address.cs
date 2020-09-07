using System;
using System.Collections.Generic;

namespace FIT3077_Pre1975.Models
{
    /// <summary>
    /// Models an address component
    /// </summary>
    public class Address
    {
        public List<string> Line { get; set; }

        public string City { get; set; }
        
        public string District { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public string Text => ToString();

        public override string ToString()
        {
            string address = "";
            
            foreach (var line in Line)
            {
                address += line + " ";
            }

            if (!String.IsNullOrEmpty(City))
            {
                address += City + ", ";
            }

            if (!String.IsNullOrEmpty(District))
            {
                address += District + ", ";
            }

            if (!String.IsNullOrEmpty(State))
            {
                address += State + " ";
            }

            if (!String.IsNullOrEmpty(PostalCode))
            {
                address += PostalCode + ", ";
            }

            if (!String.IsNullOrEmpty(Country))
            {
                address += Country + " ";
            }
            return address;
        }
    }
}
