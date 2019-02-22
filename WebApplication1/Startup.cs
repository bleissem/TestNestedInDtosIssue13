using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ClassLibrary1;
using GenericServices.Configuration;
using GenericServices.Setup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);            

            services.AddDbContext<MyDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("MyConnectionString")));

            services.GenericServicesSimpleSetup<MyDBContext>(
                            new GenericServicesConfig
                            {
                                DtoAccessValidateOnSave = true,  //This causes validation to happen on create/update via DTOs
                    DirectAccessValidateOnSave = true, //This causes validation to happen on direct create/update and delete
                    NoErrorOnReadSingleNull = true //When working with WebAPI you should set this flag. Responce then sends 404 on null result
                }, Assembly.GetAssembly(typeof(ClassLibrary1.DTOs.InContactAddressDto)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
