using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using CharTypeEnum = lingvo.core.IpAddressDetector.CharTypeEnum;

namespace lingvo.core
{
    /// <summary>
    /// 
    /// </summary>
    public struct ip_t
    {
        public int startIndex;
        public int length;

        public string GetValue( string text )
        {
            return (text.Substring( startIndex, length ));
        }
#if DEBUG
        public string value;
        public override string ToString()
        {
            return(value);
        }
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    unsafe public sealed class IpAddressDetector
    {
        /// <summary>
        /// 
        /// </summary>
        [Flags]
        internal enum CharTypeEnum : byte
        {
            __UNDEFINED__ = 0,

            EOS         = 1,
            Number      = (1 << 1),
            Whitespace  = (1 << 3),
            Dot         = (1 << 4),
            Punctuation = (1 << 5),
        }

        private static readonly CharTypeEnum* _CHARTYPE_MAP;

        static IpAddressDetector()
        {
            var CHARTYPE_MAP = new byte[ char.MaxValue + 1 ];

            for ( var ch = char.MinValue; ; ch++ )
            {                
                if ( '0' <= ch && ch <= '9' )
                    CHARTYPE_MAP[ ch ] |= (byte) CharTypeEnum.Number;
                if( char.IsWhiteSpace( ch ) )
                    CHARTYPE_MAP[ ch ] |= (byte) CharTypeEnum.Whitespace;
                if ( char.IsPunctuation( ch ) || char.IsSeparator( ch ) )
                    CHARTYPE_MAP[ ch ] |= (byte) CharTypeEnum.Whitespace;

                if ( ch == char.MaxValue )
                    break;
            }
            CHARTYPE_MAP[ '.'           ] = (byte) CharTypeEnum.Dot;
            CHARTYPE_MAP[ char.MaxValue ] = (byte) CharTypeEnum.Dot;
            CHARTYPE_MAP[ '\0'          ] = (byte) CharTypeEnum.EOS;
            CHARTYPE_MAP[ '='           ] |= (byte) CharTypeEnum.Punctuation;

            var gcHandle = GCHandle.Alloc( CHARTYPE_MAP, GCHandleType.Pinned );
            _CHARTYPE_MAP = (CharTypeEnum*) gcHandle.AddrOfPinnedObject();
        }

        private List< ip_t > _Ips;
        private Action< ip_t > _AddIpToListAction;

        public IpAddressDetector()
        {
            _Ips               = new List< ip_t >( 100 );
            _AddIpToListAction = new Action< ip_t >( AddIpToList );
        }

        unsafe public List< ip_t > Run( string text )
        {
            _Ips.Clear();

            Run( text, _AddIpToListAction );

            return (_Ips);
        }
        private void AddIpToList( ip_t ip )
        {
            _Ips.Add( ip );
        }
        unsafe public void Run( string text, Action< ip_t > detectedIpAction  )
        {
            var ct      = CharTypeEnum.Whitespace; //for text who's start with ip-address
            var prev_ct = default(CharTypeEnum);

            fixed ( char* _base = text )
            {

                for ( var ptr = _base; ; ptr++ )
                {
                    prev_ct = ct;
                    ct = _CHARTYPE_MAP[ *ptr ];
                    //first number
                    if ( !ct.IsNumber() )
                    {
                        if ( ct.IsEOS() )
                            break;                        
                        continue;
                    }

                    //previous char may be WhiteSpace or Punctuation
                    if ( !prev_ct.IsWhitespacePunctuation() )
                    {
                        continue;
                    }

                    //first octet
                    var len = TryRecogizeIpFirstOctet( ptr );
                    if ( len < 0 )
                        continue;
                    var startPtr = ptr;
                    ptr += len;

                    //second octet
                    len = TryRecogizeIpOctet( ptr );
                    if ( len < 0 )
                        goto SKIP_FEW_CHARS;
                    ptr += len;

                    //three octet
                    len = TryRecogizeIpOctet( ptr );
                    if ( len < 0 )
                        goto SKIP_FEW_CHARS;
                    ptr += len;

                    //four octet
                    len = TryRecogizeIpLastOctet( ptr );
                    if ( len < 0 )
                        goto SKIP_FEW_CHARS;
                    ptr += len;

                    var ip = new ip_t() 
                    {
                        startIndex = (int) (startPtr - _base), 
                        length     = (int) (ptr - startPtr),
                    #if DEBUG
                        value      = new string( startPtr, 0, (int) (ptr - startPtr) ),
                    #endif
                    };
                    detectedIpAction( ip );
                    ptr--;
                    continue;

                SKIP_FEW_CHARS:
                    if ( _CHARTYPE_MAP[ *ptr ].IsEOS() )
                        break;
                }
            }
        }

        private const int FALSE = -1;
        private const int NUMBER = 1;
        private const int NUMBER_DOT = 2;
        private const int NUMBER_NUMBER = 2;
        private const int NUMBER_NUMBER_DOT = 3;
        private const int NUMBER_NUMBER_NUMBER = 3;
        private const int NUMBER_NUMBER_NUMBER_DOT = 4;
        private static int TryRecogizeIpFirstOctet( char* ptr )
        {
            //first number - checked outside!
            /*var ch_1 = *ptr;
              if ( !CHARTYPE_MAP[ ch_1 ].IsNumber() )
                  return (FALSE);*/

            //second number-or-dot
            var ch_2 = *(ptr + 1);
            var ct = _CHARTYPE_MAP[ ch_2 ];
            if ( ct.IsDot() )
                return (NUMBER_DOT);
            if ( !ct.IsNumber() )
                return (FALSE);

            //three number-or-dot
            var ch_3 = *(ptr + 2);
            ct = _CHARTYPE_MAP[ ch_3 ];
            if ( ct.IsDot() )
                return (NUMBER_NUMBER_DOT);
            if ( ct.IsNumber() )
            {
                if ( !_CHARTYPE_MAP[ *(ptr + 3) ].IsDot() )
                    return (FALSE);

                switch ( *ptr /*ch_1*/ )
                {
                    case '0':
                    case '1':
                        return (NUMBER_NUMBER_NUMBER_DOT);
                    case '2':
                        switch ( ch_2 )
                        {
                            case '0':
                            case '1':
                            case '2':
                            case '3':
                            case '4':
                                return (NUMBER_NUMBER_NUMBER_DOT);
                            case '5':
                                if ( '0' <= ch_3 && ch_3 <= '5' )
                                    return (NUMBER_NUMBER_NUMBER_DOT);
                            break;
                        }
                    break;
                }
            }

            return (FALSE);
        }
        private static int TryRecogizeIpOctet     ( char* ptr )
        {            
            //first number
            var ch_1 = *ptr;
            if ( !_CHARTYPE_MAP[ ch_1 ].IsNumber() )
                return (FALSE);

            //second number-or-dot
            var ch_2 = *(ptr + 1);
            var ct = _CHARTYPE_MAP[ ch_2 ];
            if ( ct.IsDot() )
                return (NUMBER_DOT);
            if ( !ct.IsNumber() )
                return (FALSE);

            //three number-or-dot
            var ch_3 = *(ptr + 2);
            ct = _CHARTYPE_MAP[ ch_3 ];
            if ( ct.IsDot() )
                return (NUMBER_NUMBER_DOT);
            if ( ct.IsNumber() )
            {
                if ( !_CHARTYPE_MAP[ *(ptr + 3) ].IsDot() )
                    return (FALSE);

                switch ( ch_1 )
                {
                    case '0':
                    case '1':
                        return (NUMBER_NUMBER_NUMBER_DOT);
                    case '2':
                        switch ( ch_2 )
                        {
                            case '0':
                            case '1':
                            case '2':
                            case '3':
                            case '4':
                                return (NUMBER_NUMBER_NUMBER_DOT);
                            case '5':
                                if ( '0' <= ch_3 && ch_3 <= '5' )
                                    return (NUMBER_NUMBER_NUMBER_DOT);
                            break;
                        }
                    break;
                }
            }

            return (FALSE);
        }
        private static int TryRecogizeIpLastOctet ( char* ptr )
        {            
            //first number
            var ch_1 = *ptr;
            if ( !_CHARTYPE_MAP[ ch_1 ].IsNumber() )
                return (FALSE);

            //second number-or-whitespace-or-punctuation-or-EOS
            var ch_2 = *(ptr + 1);
            var ct = _CHARTYPE_MAP[ ch_2 ];
            if ( ct.IsWhitespacePunctuationEOS() )
            {
                return (NUMBER);
            }
            if ( !ct.IsNumber() )
            {
                //172.16.254.1. Each
                if ( ct.IsDot() )
                {
                    ch_2 = *(ptr + 2);
                    ct = _CHARTYPE_MAP[ ch_2 ];
                    if ( ct.IsWhitespacePunctuationEOS() )
                    {
                        return (NUMBER);
                    }
                }

                return (FALSE);
            }

            //three number-or-whitespace-or-punctuation-or-EOS
            var ch_3 = *(ptr + 2);
            ct = _CHARTYPE_MAP[ ch_3 ];
            if ( ct.IsWhitespacePunctuationEOS() )
            {
                return (NUMBER_NUMBER);
            }
            if ( !ct.IsNumber() )
            {
                //172.16.254.12. Each
                if ( ct.IsDot() )
                {
                    ch_3 = *(ptr + 3);
                    ct = _CHARTYPE_MAP[ ch_3 ];
                    if ( ct.IsWhitespacePunctuationEOS() )
                    {
                        return (NUMBER_NUMBER);
                    }
                }

                return (FALSE);
            }
            ct = _CHARTYPE_MAP[ *(ptr + 3) ];
            if ( !ct.IsWhitespacePunctuationEOS() )
            {
                //172.16.254.123. Each
                if ( !ct.IsDot() || !_CHARTYPE_MAP[ *(ptr + 4) ].IsWhitespacePunctuationEOS() )
                {
                    return (FALSE);
                }
            }
            switch ( ch_1 )
            {
                case '0':
                case '1':
                    return (NUMBER_NUMBER_NUMBER);
                case '2':
                    switch ( ch_2 )
                    {
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                            return (NUMBER_NUMBER_NUMBER);
                        case '5':
                            if ( '0' <= ch_3 && ch_3 <= '5' )
                                return (NUMBER_NUMBER_NUMBER);
                            break;
                    }
                    break;
            }

            return (FALSE);
        }

        /*private bool PreviousCharWhitespaceOrPunctuation( char* ptr )
        {
            if ( _BASE < ptr )
            {
                var prev_ct = CHARTYPE_MAP[ *(ptr - 1) ];
                if ( (prev_ct & CharTypeEnum.Whitespace)  != CharTypeEnum.Whitespace &&
                     (prev_ct & CharTypeEnum.Punctuation) != CharTypeEnum.Punctuation
                   )
                {
                    return (false);
                }
            }
            return (true);
        }*/

    }

    /// <summary>
    /// 
    /// </summary>
    internal static class Extensions
    {
        public static bool IsDot   ( this CharTypeEnum ct )
        {
            return ((ct & CharTypeEnum.Dot) == CharTypeEnum.Dot);
        }
        public static bool IsNumber( this CharTypeEnum ct )
        {
            return ((ct & CharTypeEnum.Number) == CharTypeEnum.Number);
        }
        public static bool IsEOS   ( this CharTypeEnum ct )
        {
            return ((ct & CharTypeEnum.EOS) == CharTypeEnum.EOS);
        }        
        public static bool IsWhitespacePunctuation( this CharTypeEnum ct )
        {
            return ((ct & CharTypeEnum.Whitespace ) == CharTypeEnum.Whitespace ||
                    (ct & CharTypeEnum.Punctuation) == CharTypeEnum.Punctuation
                   );
        }
        public static bool IsWhitespacePunctuationEOS( this CharTypeEnum ct )
        {
            return ((ct & CharTypeEnum.Whitespace ) == CharTypeEnum.Whitespace ||
                    (ct & CharTypeEnum.Punctuation) == CharTypeEnum.Punctuation ||
                    (ct & CharTypeEnum.EOS        ) == CharTypeEnum.EOS
                   );
        }
    }
}
