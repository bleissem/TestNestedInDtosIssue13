using System;
using System.Collections.Generic;

namespace ClassLibrary1
{
    public partial class AddressNotOwned
    {
        public int Id { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateOrProvice { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }

        public virtual ContactAddress IdNavigation { get; set; }
    }
}
