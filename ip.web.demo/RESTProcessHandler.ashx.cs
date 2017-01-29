using System;
using System.Configuration;
using System.Web;

using Newtonsoft.Json;

namespace lingvo.core
{
    /// <summary>
    /// Summary description for RESTProcessHandler
    /// </summary>
    public sealed class RESTProcessHandler : IHttpHandler
    {
        #region [.config.]
        private static readonly int CONCURRENT_FACTORY_INSTANCE_COUNT = int.Parse( ConfigurationManager.AppSettings[ "CONCURRENT_FACTORY_INSTANCE_COUNT" ] );
        #endregion

        /// <summary>
        /// 
        /// </summary>
        private struct result
        {
            public result( Exception ex ) : this()
            {
                exceptionMessage = ex.ToString();
            }
            public result( ip_t[] _ips ) : this()
            {
                ips = _ips;
            }

            [JsonProperty(PropertyName="err")]
            public string exceptionMessage
            {
                get;
                private set;
            }

            [JsonProperty(PropertyName="ips")] public ip_t[] ips
            {
                get;
                set;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private struct http_context_data
        {
            private static readonly object _SyncLock = new object();
            private readonly HttpContext _Context;

            public http_context_data( HttpContext context ) : this()
            {
                _Context = context;
            }

            /*private ConcurrentFactory _ConcurrentFactory
            {
                get { return ((ConcurrentFactory) _Context.Cache[ "_ConcurrentFactory" ]); }
                set
                {
                    _Context.Cache.Remove( "_ConcurrentFactory" );
                    if ( value != null )
                        _Context.Cache[ "_ConcurrentFactory" ] = value;
                }
            }*/

            private static ConcurrentFactory _ConcurrentFactory;

            public ConcurrentFactory GetConcurrentFactory()
            {
                var f = _ConcurrentFactory;
                if ( f == null )
                {
                    lock ( _SyncLock )
                    {
                        f = _ConcurrentFactory;
                        if ( f == null )
                        {
                            f = new ConcurrentFactory( CONCURRENT_FACTORY_INSTANCE_COUNT );
                            _ConcurrentFactory = f;
                        }
                    }
                }
                return (f);
            }
        }

        static RESTProcessHandler()
        {
            Environment.CurrentDirectory = HttpContext.Current.Server.MapPath( "~/" );
        }

        public bool IsReusable
        {
            get { return (true); }
        }

        public void ProcessRequest( HttpContext context )
        {
            try
            {
                var text = context.Request[ "text" ];

                var hcd = new http_context_data( context );
                var factory = hcd.GetConcurrentFactory();

                var ips = factory.Run( text );

                SendJsonResponse( context, ips );
            }
            catch ( Exception ex )
            {
                SendJsonResponse( context, ex );
            }
        }

        private static void SendJsonResponse( HttpContext context, ip_t[] ips )
        {
            SendJsonResponse( context, new result( ips ) );
        }
        private static void SendJsonResponse( HttpContext context, Exception ex )
        {
            SendJsonResponse( context, new result( ex ) );
        }
        private static void SendJsonResponse( HttpContext context, result result )
        {
            context.Response.ContentType = "application/json";
            //---context.Response.Headers.Add( "Access-Control-Allow-Origin", "*" );

            var json = JsonConvert.SerializeObject( result );
            context.Response.Write( json );
        }
    }
}