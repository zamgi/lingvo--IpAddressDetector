namespace Ude.Core
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class UniversalDetector
    {
        protected const int FILTER_CHINESE_SIMPLIFIED = 1;
        protected const int FILTER_CHINESE_TRADITIONAL = 2;
        protected const int FILTER_JAPANESE = 4;
        protected const int FILTER_KOREAN = 8;
        protected const int FILTER_NON_CJK = 16;
        protected const int FILTER_ALL = 31;
        protected const float SHORTCUT_THRESHOLD = 0.95f;
        protected const float MINIMUM_THRESHOLD = 0.2f;
        protected const int   PROBERS_NUM = 3;
        protected static int  FILTER_CHINESE = 3;
        protected static int  FILTER_CJK = 15;

        internal InputState _InputState;
        protected bool _Start;
        protected bool _GotData;
        protected bool _Done;
        protected byte _LastChar;
        protected int _BestGuess;
        protected int _LanguageFilter;
        protected CharsetProber[] _CharsetProbers = new CharsetProber[ 3 ];
        protected CharsetProber _EscCharsetProber;
        protected string _DetectedCharset;

        public UniversalDetector( int languageFilter )
        {
            _Start          = true;
            _InputState     = InputState.PureASCII;
            _LastChar       = 0;
            _BestGuess      = -1;
            _LanguageFilter = languageFilter;
        }

        public virtual void Feed( byte[] buf, int offset, int len )
        {
            if ( _Done )
            {
                return;
            }
            if ( len > 0 )
            {
                _GotData = true;
            }
            if ( _Start )
            {
                _Start = false;
                if ( len > 3 )
                {
                    switch ( buf[ 0 ] )
                    {
                        case 239:
                            if ( 187 == buf[ 1 ] && 191 == buf[ 2 ] )
                            {
                                _DetectedCharset = "UTF-8";
                            }
                            break;
                        case 254:
                            if ( byte.MaxValue == buf[ 1 ] && buf[ 2 ] == 0 && buf[ 3 ] == 0 )
                            {
                                _DetectedCharset = "X-ISO-10646-UCS-4-3412";
                            }
                            else if ( byte.MaxValue == buf[ 1 ] )
                            {
                                _DetectedCharset = "UTF-16BE";
                            }
                            break;
                        case 0:
                            if ( buf[ 1 ] == 0 && 254 == buf[ 2 ] && byte.MaxValue == buf[ 3 ] )
                            {
                                _DetectedCharset = "UTF-32BE";
                            }
                            else if ( buf[ 1 ] == 0 && byte.MaxValue == buf[ 2 ] && 254 == buf[ 3 ] )
                            {
                                _DetectedCharset = "X-ISO-10646-UCS-4-2143";
                            }
                            break;
                        case byte.MaxValue:
                            if ( 254 == buf[ 1 ] && buf[ 2 ] == 0 && buf[ 3 ] == 0 )
                            {
                                _DetectedCharset = "UTF-32LE";
                            }
                            else if ( 254 == buf[ 1 ] )
                            {
                                _DetectedCharset = "UTF-16LE";
                            }
                            break;
                    }
                }
                if ( _DetectedCharset != null )
                {
                    _Done = true;
                    return;
                }
            }
            checked
            {
                for ( int i = 0; i < len; i++ )
                {
                    if ( (buf[ i ] & 0x80u) != 0 && buf[ i ] != 160 )
                    {
                        if ( _InputState != InputState.Highbyte )
                        {
                            _InputState = InputState.Highbyte;
                            if ( _EscCharsetProber != null )
                            {
                                _EscCharsetProber = null;
                            }
                            if ( _CharsetProbers[ 0 ] == null )
                            {
                                _CharsetProbers[ 0 ] = new MBCSGroupProber();
                            }
                            if ( _CharsetProbers[ 1 ] == null )
                            {
                                _CharsetProbers[ 1 ] = new SBCSGroupProber();
                            }
                            if ( _CharsetProbers[ 2 ] == null )
                            {
                                _CharsetProbers[ 2 ] = new Latin1Prober();
                            }
                        }
                    }
                    else
                    {
                        if ( _InputState == InputState.PureASCII && (buf[ i ] == 51 || (buf[ i ] == 123 && _LastChar == 126)) )
                        {
                            _InputState = InputState.EscASCII;
                        }
                        _LastChar = buf[ i ];
                    }
                }
                ProbingState probingState;
                switch ( _InputState )
                {
                    case InputState.EscASCII:
                        if ( _EscCharsetProber == null )
                        {
                            _EscCharsetProber = new EscCharsetProber();
                        }
                        probingState = _EscCharsetProber.HandleData( buf, offset, len );
                        if ( probingState == ProbingState.FoundIt )
                        {
                            _Done = true;
                            _DetectedCharset = _EscCharsetProber.GetCharsetName();
                        }
                        break;
                    case InputState.Highbyte:
                        {
                            for ( int j = 0; j < 3; j++ )
                            {
                                if ( _CharsetProbers[ j ] != null )
                                {
                                    probingState = _CharsetProbers[ j ].HandleData( buf, offset, len );
                                    if ( probingState == ProbingState.FoundIt )
                                    {
                                        _Done = true;
                                        _DetectedCharset = _CharsetProbers[ j ].GetCharsetName();
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                }
            }
        }

        public virtual void DataEnd()
        {
            if ( !_GotData )
            {
                return;
            }
            if ( _DetectedCharset != null )
            {
                _Done = true;
                Report( _DetectedCharset, 1f );
            }
            else if ( _InputState == InputState.Highbyte )
            {
                float num = 0f;
                float num2 = 0f;
                int num3 = 0;
                for ( int i = 0; i < 3; i++ )
                {
                    if ( _CharsetProbers[ i ] != null )
                    {
                        num = _CharsetProbers[ i ].GetConfidence();
                        if ( num > num2 )
                        {
                            num2 = num;
                            num3 = i;
                        }
                    }
                }
                if ( num2 > 0.2f )
                {
                    Report( _CharsetProbers[ num3 ].GetCharsetName(), num2 );
                }
            }
            else if ( _InputState == InputState.PureASCII )
            {
                Report( "ASCII", 1f );
            }
        }

        public virtual void Reset()
        {
            _Done = false;
            _Start = true;
            _DetectedCharset = null;
            _GotData = false;
            _BestGuess = -1;
            _InputState = InputState.PureASCII;
            _LastChar = 0;
            if ( _EscCharsetProber != null )
            {
                _EscCharsetProber.Reset();
            }
            for ( int i = 0; i < 3; i++ )
            {
                if ( _CharsetProbers[ i ] != null )
                {
                    _CharsetProbers[ i ].Reset();
                }
            }
        }

        protected abstract void Report( string charset, float confidence );
    }
}