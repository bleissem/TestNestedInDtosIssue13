using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary1;
using ClassLibrary1.DTOs;
using GenericServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit.Extensions.AssertExtensions;
using Dapper;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet("[action]")]
        public ActionResult<IEnumerable<string>> DoesNotWork([FromServices] ICrudServices service, [FromServices] MyDBContext context, [FromServices] IConfiguration config)
        {
            //first use Dapper to insert data so only an update is done by ICrudServices
            int contactAddressId;

            using (IDbConnection dbConnection = new SqlConnection(config.GetConnectionString("MyConnectionString")))
            {
                contactAddressId = dbConnection.QueryFirst<int>(@"
                                            SET NOCOUNT ON;

                                            DECLARE @ContactAddressId INT
                                            INSERT INTO[ContactAddress]([Name])
                                            VALUES('new Address');


                                            SELECT @ContactAddressId = [ContactAddressId]
                                            FROM[ContactAddress]
                                            WHERE @@ROWCOUNT = 1 AND[ContactAddressId] = scope_identity()


                                            INSERT INTO[AddressNotOwned] ([Id], [Address1])
                                            VALUES(@ContactAddressId, 'some street');

                SELECT @ContactAddressId");
            }
          
            InContactAddressDto updateDTO = service.ReadSingle<InContactAddressDto>(w => w.ContactAddressId == contactAddressId);

            updateDTO.Name = "test2";
            updateDTO.Addess.Address1 = "other street";

            service.UpdateAndSave(updateDTO);

            //VERIFY
            service.IsValid.ShouldBeTrue(service.GetAllErrors());

            var contactUpdate = context.ContactAddress.SingleOrDefault();
            contactUpdate.Name.ShouldEqual("test2");
            contactUpdate.AddressNotOwned.Address1.ShouldEqual("other street");


            return new string[] { "value1", "value2" };
        }

        // GET api/values
        [HttpGet("[action]")]
        public ActionResult<InContactAddressDto> DoesNotWorkWithoutDapper([FromServices] ICrudServices service, [FromServices] MyDBContext context, int key)
        {
            return service.ReadSingle<InContactAddressDto>(w => w.ContactAddressId == key);
        }

        [HttpGet("[action]")]
        public ActionResult<IEnumerable<string>> WorksGreat([FromServices] ICrudServices service, [FromServices] MyDBContext context, [FromServices] IConfiguration config)
        {
            context.AddressNotOwned.RemoveRange(context.AddressNotOwned.ToList());
            context.ContactAddress.RemoveRange(context.ContactAddress.ToList());
            context.SaveChanges();

            //ATTEMPT
            var dto = new InContactAddressDto
            {
                Name = "test",
                Addess = new InAddressDto
                {
                    Address1 = "some street"
                }
            };
            service.CreateAndSave(dto);

            //VERIFY
            service.IsValid.ShouldBeTrue(service.GetAllErrors());
            var contact = context.ContactAddress.Include(i => i.AddressNotOwned).SingleOrDefault();
            contact.Name.ShouldEqual("test");
            contact.AddressNotOwned.Address1.ShouldEqual("some street");

            InContactAddressDto updateDTO = service.ReadSingle<InContactAddressDto>(w => w.Name == "test");

            updateDTO.Name = "test2";
            updateDTO.Addess.Address1 = "other street";

            service.UpdateAndSave(updateDTO);

            //VERIFY
            service.IsValid.ShouldBeTrue(service.GetAllErrors());
            contact = context.ContactAddress.SingleOrDefault();
            contact.Name.ShouldEqual("test2");
            contact.AddressNotOwned.Address1.ShouldEqual("other street");


            return new string[] { "value1", "value2" };
        }

    }
}
