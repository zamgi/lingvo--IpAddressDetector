using System;

using lingvo.core;

using JP = System.Text.Json.Serialization.JsonPropertyNameAttribute;

namespace IpAddressDetector.WebService
{
    /// <summary>
    /// 
    /// </summary>
    public struct InitParamsVM
    {
        public string Text { get; set; }

#if DEBUG
        public override string ToString() => Text;
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    internal readonly struct ResultVM
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly struct value_t
        {
            [JP( "startIndex")] public int    startIndex { get; init; }
            [JP( "length")    ] public int    length     { get; init; }
#if DEBUG
            [JP("value")      ] public string value      { get; init; }
#endif
        }

        public ResultVM( in InitParamsVM m, Exception ex ) : this() => (InitParams, ExceptionMessage) = (m, ex.ToString());
        public ResultVM( in InitParamsVM m, in ip_t[] ips ) : this()
        {
            InitParams = m;
            Values     = new value_t[ ips.Length ];
            for ( int i = 0, len = ips.Length; i < len; i++ )
            {
                var ip = ips[ i ];
                Values[ i ] = new value_t()
                {
                    startIndex = ip.startIndex,
                    length     = ip.length,
#if DEBUG
                    value      = ip.value,
#endif
                };
            }
        }

        [JP("ip") ] public InitParamsVM InitParams       { get; }
        [JP("err")] public string       ExceptionMessage { get; }
        [JP("ips")] public value_t[]    Values           { get; }
    }
}
