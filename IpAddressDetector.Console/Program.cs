using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;

using Ude;

namespace lingvo.core
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class Config
    {
        public static Config Inst { get; } = new Config();

        private Config()
        {
            var value = ConfigurationManager.AppSettings[ "INPUT_FOLDERS" ];
            if ( value.IsNullOrEmpty() )
            {
                throw (new ArgumentNullException( "INPUT_FOLDERS" ));
            }
            INPUT_FOLDERS = value
                            .Split( new[]{ ',', ';' }, StringSplitOptions.RemoveEmptyEntries )
                            .Select( _ => _.Trim() )
                            .Where( _ => !_.IsNullOrEmpty() )
                            .ToArray();
            if ( !INPUT_FOLDERS.Any() )
            {
                throw (new ArgumentNullException("INPUT_FOLDERS"));
            }

            value = ConfigurationManager.AppSettings[ "FILE_SEARCH_MASK" ];
            if ( value.IsNullOrEmpty() )
            {
                throw (new ArgumentNullException( "FILE_SEARCH_MASK" ));
            }
            FILE_SEARCH_MASKS = value
                                .Split( new[]{ ',', ';' }, StringSplitOptions.RemoveEmptyEntries )
                                .Select( _ => _.Trim() )
                                .Where( _ => !_.IsNullOrEmpty() )
                                .ToArray();
            if ( !FILE_SEARCH_MASKS.Any() )
            {
                throw (new ArgumentNullException("FILE_SEARCH_MASK"));
            }
            FILE_SEARCH_MASKS_SET = new HashSet< string >( from v in FILE_SEARCH_MASKS select v.TrimStart( '*' ) );

            if ( !int.TryParse( ConfigurationManager.AppSettings[ "MAX_FILE_SIZE_IN_BYTES" ], out var x ) )
            {
                throw (new ArgumentNullException( "MAX_FILE_SIZE_IN_BYTES" ));
            }
            MAX_FILE_SIZE_IN_BYTES = x;

            if ( !int.TryParse( ConfigurationManager.AppSettings[ "SNIPPET_LENGTH" ], out x ) )
            {
                throw (new ArgumentNullException( "SNIPPET_LENGTH" ));
            }
            SNIPPET_LENGTH = x;
        }

        public int               MAX_FILE_SIZE_IN_BYTES { get; } // = (1024 * 1024 * 512); // 500MB
        public string[]          INPUT_FOLDERS          { get; }
        public string[]          FILE_SEARCH_MASKS      { get; }
        public HashSet< string > FILE_SEARCH_MASKS_SET  { get; }
        public int               SNIPPET_LENGTH         { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    internal static class Program
    {
        private static readonly Dictionary< string, string > _CharsetNameDict = new Dictionary< string, string >();
        static Program()
        {
            _CharsetNameDict.Add( "x-mac-cyrillic", "windows-1251" );
            _CharsetNameDict.Add( "windows-1252"  , "windows-1251" );
        }

        private static Encoding GetEncodingByCharsetName( string charsetName )
        {
            if ( charsetName.IsNullOrEmpty() )
            {
                return (Encoding.UTF8);
            }

            if ( _CharsetNameDict.TryGetValue( charsetName, out var newCharsetName ) )
            {
                return (Encoding.GetEncoding( newCharsetName ));
            }
            return (Encoding.GetEncoding( charsetName ));
        }
        private static string GetFileText( ICharsetDetector cdet, byte[] buffer, string fullFileName )
        {            
            try
            {
                var fi = new FileInfo( fullFileName );
                if ( fi.Length < Config.Inst.MAX_FILE_SIZE_IN_BYTES )
                {
                    using ( var fs = File.OpenRead( fullFileName ) )
                    {
                        var length = fs.Read( buffer, 0, Math.Min( buffer.Length, (int) fs.Length ) );

                        cdet.Reset();
                        cdet.Feed( buffer, 0, length );
                        cdet.DataEnd();

                        fs.Position = 0;
                        return (new StreamReader( fs, GetEncodingByCharsetName( cdet.Charset ) ).ReadToEnd());
                    }                    
                }
            }
            catch ( Exception ex )
            {
                Debug.WriteLine( ex.GetType().Name + ": '" + ex.Message + '\'' );
            }
            return (null);
        }
        private static IEnumerable< string > EnumerateAllFiles( string path )
        {
            try
            {
                var seq = Directory.EnumerateDirectories( path ).SafeWalk()
                                   .SelectMany( _path => EnumerateAllFiles( _path ) );
                return (seq.Concat( Directory.EnumerateFiles( path, "*.*" )/*.SafeWalk()*/ ));
            }
            catch ( Exception ex )
            {
                Debug.WriteLine( ex.GetType().Name + ": '" + ex.Message + '\'' );
                return (Enumerable.Empty< string >());
            }
        }

        private static void Main( string[] args )
        {
            try
            {
                var th = new Thread( MainRoutine, 50 * 1024 * 1024 );
                th.Start();
                th.Join();
            }
            catch ( Exception ex )
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine( ex );
                Console.ResetColor();
            }
            Console.WriteLine( Environment.NewLine + "[.....finita fusking comedy.....]" );
            Console.ReadLine();
        }

        private static void MainRoutine()
        {
            var cdet       = new CharsetDetector();
            var buffer     = new byte[ 1024 * 1024 ];
            var ipDetector = new IpAddressDetector();

            var tuples = from inputFolder in Config.Inst.INPUT_FOLDERS
                         from fullFileName in EnumerateAllFiles( inputFolder )
                         where (Config.Inst.FILE_SEARCH_MASKS_SET.Contains( Path.GetExtension( fullFileName ) ))
                         let text = GetFileText( cdet, buffer, fullFileName )
                         where (!string.IsNullOrEmpty( text ))
                         select new { text = text, file = fullFileName };

            var sw = new Stopwatch();
            var totalLength     = 0L;
            var totalFileNumber = 0;
            var fileNumber      = 0;
            var title = Console.Title;
            var ip_snippet_filename = Path.Combine( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ), "ip-snippet.html" );
            using ( var swt = new StreamWriter( ip_snippet_filename, false ) )
            {
                swt.WriteLine( "<html>" + 
                               "    <head>" + 
                               "        <title>'{0}' => '{1}'</title>" + 
                               "        <style>" + 
                               "            body {{ font-family: Tahoma; }}" + 
                               "            ol {{ font-size: x-small; color: gray; }}" + 
                               "            li > span {{ font-size: 14px; color: black; }}" +
                               "            li > span.b-a {{ color: gray; }}" +
                               "            li > span.ip {{ color: #23B100; font-weight: bold; border-bottom: 1px dotted silver; }}" +
                               "            table {{ font-size: 18px; }}" +
                               "        </style>" + 
                               "    </head>" +
                               "    <body>"
                               , string.Join( "', '", Config.Inst.INPUT_FOLDERS )
                               , string.Join( "', '", Config.Inst.FILE_SEARCH_MASKS ) );

                foreach ( var t in tuples )
                {
                    var msg = (++totalFileNumber) + ".'" + t.file + "':";
                    Console.Title = msg;

                    var text = t.text;
                    totalLength += text.Length;

                    sw.Start();
                    var ips = ipDetector.Run( text );
                    sw.Stop();
                
                    if ( ips.Any() )
                    {                        
                        Console.Write( msg + Environment.NewLine );
                        msg = (++fileNumber) + ".'" + t.file + "':";
                        swt.WriteLine( "<div><h4>{0}</h4><ol>", msg );

                        foreach ( var ip in ips )
                        {
                            var snippet = GetSnippet( in ip, text, Config.Inst.SNIPPET_LENGTH );

                            Console.Write( '\t' );
                            Console.Write( snippet.before );
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write( snippet.value );
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.WriteLine( snippet.after );

                            swt.WriteLine( "<li>" + 
                                             "<span class='b-a'>{0}</span>" +
                                             "<span>{1}</span>" +
                                             "<span class='ip'>{2}</span>" +
                                             "<span>{3}</span>" +
                                             "<span class='b-a'>{4}</span>" +
                                            "</li>"
                                , snippet.has_before_before ? "..." : string.Empty
                                , HttpUtility.HtmlEncode( snippet.before ) //snippet.before
                                , snippet.value
                                , HttpUtility.HtmlEncode( snippet.after ) //snippet.after
                                , snippet.has_after_after ? "..." : string.Empty );
                        }

                        swt.WriteLine( "</ol></div>" );
                        swt.Flush();
                    }
                }

                swt.WriteLine( "<div>" +
                               "    <hr/>" +
                               "    <table>" +
                               "        <tr><td>elapsed:    </td><td>{0}</td><td /></tr>" +
                               "        <tr><td>text-length:</td><td>{1}</td><td> (chars)</td></tr>" +
                               "        <tr><td>speed:      </td><td>{2}</td><td> (chars/seconds)</td></tr>" +
                               "    </table>" +
                               "</div>" +
                               "</body>" +
                               "</html>"
                               , sw.Elapsed
                               , totalLength.ToString("0,0")
                               , (totalLength / sw.Elapsed.TotalSeconds).ToString("0,0") );

            }
            Console.Title = title;
            Console.WriteLine( "\r\n elapsed: \t" + sw.Elapsed + "\r\n text-length: \t" + totalLength.ToString("0,0") + "\t chars" +
                               "\r\n speed: \t" + (totalLength / sw.Elapsed.TotalSeconds).ToString("0,0") + "\t chars/seconds" );

            Process.Start( ip_snippet_filename );
        }

        /// <summary>
        /// 
        /// </summary>
        private struct snippet_t
        {
            public string before;
            public string value;
            public string after;

            public bool has_before_before;
            public bool has_after_after;
        }

        private static snippet_t GetSnippet( in ip_t ip, string text, int snippetLength )
        {
            var snippet = new snippet_t();

            if ( snippetLength <= ip.startIndex )
            {
                snippet.before = text.Substring( ip.startIndex - snippetLength, snippetLength )
                                     .Replace( '\r', ' ' ).Replace( '\n', ' ' ).Replace( '\t', ' ' );
                snippet.has_before_before = (snippetLength < ip.startIndex);
            }

            snippet.value = ip.GetValue( text );

            if ( snippetLength <= (text.Length - (ip.startIndex + ip.length)) )
            {
                snippet.after = text.Substring( ip.startIndex + ip.length, snippetLength )
                                    .Replace( '\r', ' ' ).Replace( '\n', ' ' ).Replace( '\t', ' ' );
                snippet.has_after_after = (snippetLength < (text.Length - (ip.startIndex + ip.length)));
            }

            return (snippet);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal static class Extensions
    {
        public static bool IsNullOrEmpty( this string value ) => string.IsNullOrEmpty( value );
        public static IEnumerable< T > SafeWalk< T >( this IEnumerable< T > source )
        {
            using ( var enumerator = source.GetEnumerator() )
            {
                for ( ; ; )
                {
                    try
                    {
                        if ( !enumerator.MoveNext() )
                            break;
                    }
                    catch ( Exception ex )
                    {
                        Debug.WriteLine( ex.GetType().Name + ": '" + ex.Message + '\'' );
                        continue;
                    }

                    yield return (enumerator.Current);
                }
            }
        }
    }
}
