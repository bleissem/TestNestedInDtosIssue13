using System;
using System.Linq;
using ClassLibrary1;
using ClassLibrary1.DTOs;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TestSupport.EfHelpers;
using WebApplication1.Controllers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test
{
    public class UnitTest1
    {

        [Fact]
        public void TestCreateContactAddressOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<MyDBContext>();
            using (var context = new MyDBContext(options))
            {
                context.Database.EnsureCreated();


                //ATTEMPT
                context.Add(new ContactAddress
                    {Name = "unit test", AddressNotOwned = new AddressNotOwned {Address1 = "some street"}});
                context.SaveChanges();
            }
            using (var context = new MyDBContext(options))
            {
                //VERIFY
                var cAddr = context.ContactAddress.Include(x => x.AddressNotOwned).Single();
                cAddr.AddressNotOwned.ShouldNotBeNull();
            }
        }

        [Fact]
        public void TestMapContactAddressOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<MyDBContext>();
            using (var context = new MyDBContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new ContactAddress
                    { Name = "unit test", AddressNotOwned = new AddressNotOwned { Address1 = "some street" } });
                context.SaveChanges();
            }
            using (var context = new MyDBContext(options))
            {
                var utData = context.SetupSingleDtoAndEntities<InContactAddressDto>();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var result = service.ReadSingle<InContactAddressDto>(1);

                //VERIFY
                result.AddressNotOwned.ShouldNotBeNull();
            }
        }


        [Fact]
        public void TestCallControllerMethodDoesNotWorkOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<MyDBContext>();
            using (var context = new MyDBContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new ContactAddress
                    { Name = "unit test", AddressNotOwned = new AddressNotOwned { Address1 = "some street" } });
                context.SaveChanges();
            }
            using (var context = new MyDBContext(options))
            {
                var utData = context.SetupSingleDtoAndEntities<InContactAddressDto>();
                var service = new CrudServices(context, utData.ConfigAndMapper);                

                var controller = new ValuesController();

                //ATTEMPT
                var result = controller.DoesNotWorkWithoutDapper(service, context, 1);

                InContactAddressDto toBeUpdated = result.Value;

                //VERIFY
                toBeUpdated.AddressNotOwned.ShouldNotBeNull();

                toBeUpdated.AddressNotOwned.City = "London";

                // throws: AutoMapper.AutoMapperMappingException: 'Error mapping types.'
                service.UpdateAndSave(toBeUpdated, a => a.AddressNotOwned);

            }
        }
    }
}
