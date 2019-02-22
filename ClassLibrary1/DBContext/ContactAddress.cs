using System;
using System.Collections.Generic;

namespace ClassLibrary1
{
    public partial class ContactAddress
    {
        public int ContactAddressId { get; set; }
        public string Name { get; set; }

        public virtual AddressNotOwned AddressNotOwned { get; set; }
    }
}
