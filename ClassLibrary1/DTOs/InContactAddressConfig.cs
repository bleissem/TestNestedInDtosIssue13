using AutoMapper;
using GenericServices.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary1.DTOs
{
    public class InContactAddressConfig : PerDtoConfig<InContactAddressDto, ContactAddress>
    {
        public override Action<IMappingExpression<ContactAddress, InContactAddressDto>> AlterReadMapping
        {
            get
            {
                return cfg => cfg.ForMember(d => d.AddressNotOwned, opt => opt.MapFrom(src => src.AddressNotOwned));
            }
        }

        //See http://docs.automapper.org/en/stable/Reverse-Mapping-and-Unflattening.html#customizing-reverse-mapping on reverse mapping
        //my test showed that I didn't need the .ReverseMap() - that's most likely because I use AutoMapper's Map, rather that MapFrom
        public override Action<IMappingExpression<InContactAddressDto, ContactAddress>> AlterSaveMapping
        {
            get
            {
                return cfg => cfg.ForMember(d => d.AddressNotOwned, opt => opt.MapFrom(src => src.AddressNotOwned));
            }
        }
    }
}
