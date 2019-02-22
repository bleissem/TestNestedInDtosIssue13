using GenericServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary1.DTOs
{

    public class InContactAddressDto : ILinkToEntity<ContactAddress>
    {
        public int ContactAddressId { get; set; }
        public string Name { get; set; }

        public virtual InAddressDto Addess { get; set; }
    }
}
