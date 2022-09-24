using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;

namespace IpAddressDetector.WebService
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class Startup
    {
        //private IConfiguration _Configuration;
        //public Startup( IConfiguration configuration ) => _Configuration = configuration;

        public void ConfigureServices( IServiceCollection services )
        {
            services.AddControllers().AddJsonOptions( options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.Converters.Add( new JsonStringEnumConverter() );
            });

            //For application running on IIS:
            services.Configure< IISServerOptions >( opts => opts.MaxRequestBodySize = int.MaxValue );
            //For application running on Kestrel:
            services.Configure< KestrelServerOptions >( opts => opts.Limits.MaxRequestBodySize = int.MaxValue );
            //Form's MultipartBodyLengthLimit
            services.Configure< FormOptions >( opts =>
            {
                opts.ValueLengthLimit            = int.MaxValue;
                opts.MultipartBodyLengthLimit    = int.MaxValue; // if don't set default value is: 128 MB
                opts.MultipartHeadersLengthLimit = int.MaxValue;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
        {
            if ( env.IsDevelopment() )
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseDefaultFiles();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints( endpoints => endpoints.MapControllers() );
            //-------------------------------------------------------------//
            
            OpenBrowserIfRunAsConsole( app );
        }
        private static void OpenBrowserIfRunAsConsole( IApplicationBuilder app )
        {
            #region [.open browser if run as console.]
            if ( !WindowsServiceHelpers.IsWindowsService() ) //IsRunAsConsole
            {
                var server    = app.ApplicationServices.GetRequiredService< IServer >();
                var addresses = server.Features?.Get< IServerAddressesFeature >()?.Addresses;
                var address   = addresses?.FirstOrDefault();
                
                if ( address == null )
                {
                    var config = app.ApplicationServices.GetService< IConfiguration >();
                    address = config.GetSection( "Kestrel:Endpoints:Https:Url" ).Value ??
                              config.GetSection( "Kestrel:Endpoints:Http:Url"  ).Value;
                    if ( address != null )
                    {
                        address = address.Replace( "/*:", "/localhost:" );
                    }
                }

                //System.Console.WriteLine( $"[ADDRESS: {address ?? "NULL"}]" );

                if ( address != null )
                {
                    using ( Process.Start( new ProcessStartInfo( address.TrimEnd('/') + "/index.html" ) { UseShellExecute = true } ) ) { };
                }                
            }
            #endregion
        }
    }
}
