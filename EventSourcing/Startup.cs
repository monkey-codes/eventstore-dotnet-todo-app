using System.Text.Json;
using System.Threading.Tasks;
using Autofac;
using EventSourcing.EventSourcing;
using EventSourcing.Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace EventSourcing
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
            services.AddControllers();
            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            
            app.UseHealthChecks("/health", new HealthCheckOptions {ResponseWriter = JsonResponseWriter});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        
        /// <summary>
        /// Configure Container using Autofac: Register DI.
        /// This is called AFTER ConfigureServices.
        /// So things you register here OVERRIDE things registered in ConfigureServices.
        /// You must have the call to `UseServiceProviderFactory(new AutofacServiceProviderFactory())` in Program.cs
        /// When building the host or this won't be called.
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            #region WAY-1 (Autofac Module)

            // Add modules registrations.

            builder.RegisterModule(new EventSourcingAutofacModule());
            builder.RegisterModule(new MediatorAutofacModule());

            #endregion

            #region WAY-2 (Direct Registration)

            // Add services registrations.

            // builder.RegisterType<MyService>().As<IService>();

            #endregion
        }
        
        private async Task JsonResponseWriter(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";
            await JsonSerializer.SerializeAsync(context.Response.Body, new {Status = report.Status.ToString()},
                new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
        }
    }
}
