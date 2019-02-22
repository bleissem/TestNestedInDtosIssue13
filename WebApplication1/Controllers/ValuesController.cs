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
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get([FromServices] ICrudServices service, [FromServices] MyDBContext context, [FromServices] IConfiguration config)
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
          
            InContactAddressDto updateDTO = service.ReadSingle<InContactAddressDto>(w => w.ContactAddressId == contactAddressId); //.ReadManyNoTracked<InContactAddressDto>().Include(i => i.Addess).Where(w => w.Name == "test").FirstOrDefault();

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
       
    }
}
