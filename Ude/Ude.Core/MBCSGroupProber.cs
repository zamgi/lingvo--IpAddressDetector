using System;

namespace Ude.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class MBCSGroupProber : CharsetProber
    {
        private const int PROBERS_NUM = 7;

        private static readonly string[] ProberName = new string[] { "UTF8", "SJIS", "EUCJP", "GB18030", "EUCKR", "Big5", "EUCTW" };

        private CharsetProber[] _Probers = new CharsetProber[ 7 ];
        private bool[] _IsActive = new bool[ 7 ];
        private int _BestGuess;
        private int _ActiveNum;

        public MBCSGroupProber()
        {
            _Probers[ 0 ] = new UTF8Prober();
            _Probers[ 1 ] = new SJISProber();
            _Probers[ 2 ] = new EUCJPProber();
            _Probers[ 3 ] = new GB18030Prober();
            _Probers[ 4 ] = new EUCKRProber();
            _Probers[ 5 ] = new Big5Prober();
            _Probers[ 6 ] = new EUCTWProber();
            Reset();
        }

        public override string GetCharsetName()
        {
            if ( _BestGuess == -1 )
            {
                GetConfidence();
                if ( _BestGuess == -1 )
                {
                    _BestGuess = 0;
                }
            }
            return _Probers[ _BestGuess ].GetCharsetName();
        }

        public override void Reset()
        {
            _ActiveNum = 0;
            checked
            {
                for ( int i = 0; i < _Probers.Length; i++ )
                {
                    if ( _Probers[ i ] != null )
                    {
                        _Probers[ i ].Reset();
                        _IsActive[ i ] = true;
                        _ActiveNum++;
                    }
                    else
                    {
                        _IsActive[ i ] = false;
                    }
                }
                _BestGuess = -1;
                _State = ProbingState.Detecting;
            }
        }

        public override ProbingState HandleData( byte[] buf, int offset, int len )
        {
            byte[] array = new byte[ len ];
            int len2 = 0;
            bool flag = true;
            checked
            {
                int num = offset + len;
                for ( int i = offset; i < num; i++ )
                {
                    if ( (buf[ i ] & 0x80u) != 0 )
                    {
                        array[ len2++ ] = buf[ i ];
                        flag = true;
                    }
                    else if ( flag )
                    {
                        array[ len2++ ] = buf[ i ];
                        flag = false;
                    }
                }

                for ( int j = 0; j < _Probers.Length; j++ )
                {
                    if ( !_IsActive[ j ] )
                    {
                        continue;
                    }
                    switch ( _Probers[ j ].HandleData( array, 0, len2 ) )
                    {
                        case ProbingState.FoundIt:
                            _BestGuess = j;
                            _State = ProbingState.FoundIt;
                            break;
                        case ProbingState.NotMe:
                            _IsActive[ j ] = false;
                            _ActiveNum--;
                            if ( _ActiveNum > 0 )
                            {
                                continue;
                            }
                            _State = ProbingState.NotMe;
                            break;
                        default:
                            continue;
                    }
                    break;
                }
                return _State;
            }
        }

        public override float GetConfidence()
        {
            float num = 0f;
            if ( _State == ProbingState.FoundIt )
            {
                return 0.99f;
            }
            if ( _State == ProbingState.NotMe )
            {
                return 0.01f;
            }
            for ( int i = 0; i < 7; i++ )
            {
                if ( _IsActive[ i ] )
                {
                    float num2 = _Probers[ i ].GetConfidence();
                    if ( num < num2 )
                    {
                        num = num2;
                        _BestGuess = i;
                    }
                }
            }
            return num;
        }

        public override void DumpStatus()
        {
            GetConfidence();
            for ( int i = 0; i < 7; i++ )
            {
                if ( !_IsActive[ i ] )
                {
                    Console.WriteLine( "  MBCS inactive: {0} (confidence is too low).", ProberName[ i ] );
                    continue;
                }
                float confidence = _Probers[ i ].GetConfidence();
                Console.WriteLine( "  MBCS {0}: [{1}]", confidence, ProberName[ i ] );
            }
        }
    }
}