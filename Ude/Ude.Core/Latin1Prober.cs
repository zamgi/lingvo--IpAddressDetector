using System;

namespace Ude.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class Latin1Prober : CharsetProber
    {
        private const int FREQ_CAT_NUM = 4;
        private const int UDF = 0;
        private const int OTH = 1;
        private const int ASC = 2;
        private const int ASS = 3;
        private const int ACV = 4;
        private const int ACO = 5;
        private const int ASV = 6;
        private const int ASO = 7;
        private const int CLASS_NUM = 8;

        private static readonly byte[] Latin1_CharToClass = new byte[ /*256*/ ]
        {
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 2, 2, 2, 2, 2,
        2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
        2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
        2, 1, 1, 1, 1, 1, 1, 3, 3, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        3, 3, 3, 1, 1, 1, 1, 1, 1, 0,
        1, 7, 1, 1, 1, 1, 1, 1, 5, 1,
        5, 0, 5, 0, 0, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 7, 1, 7, 0, 7, 5,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 4, 4, 4, 4, 4, 4, 5, 5,
        4, 4, 4, 4, 4, 4, 4, 4, 5, 5,
        4, 4, 4, 4, 4, 1, 4, 4, 4, 4,
        4, 5, 5, 5, 6, 6, 6, 6, 6, 6,
        7, 7, 6, 6, 6, 6, 6, 6, 6, 6,
        7, 7, 6, 6, 6, 6, 6, 1, 6, 6,
        6, 6, 6, 7, 7, 7
        };

        private static readonly byte[] Latin1ClassModel = new byte[ /*64*/ ]
        {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 3,
        3, 3, 3, 3, 3, 3, 0, 3, 3, 3,
        3, 3, 3, 3, 0, 3, 3, 3, 1, 1,
        3, 3, 0, 3, 3, 3, 1, 2, 1, 2,
        0, 3, 3, 3, 3, 3, 3, 3, 0, 3,
        1, 3, 1, 1, 1, 3, 0, 3, 1, 3,
        1, 1, 3, 3
        };

        private byte _LastCharClass;

        private int[] _FreqCounter = new int[ 4 ];

        public Latin1Prober() => Reset();
        public override string GetCharsetName() => "windows-1252";

        public override void Reset()
        {
            _State = ProbingState.Detecting;
            _LastCharClass = 1;
            for ( int i = 0; i < 4; i++ )
            {
                _FreqCounter[ i ] = 0;
            }
        }

        public override ProbingState HandleData( byte[] buf, int offset, int len )
        {
            var array = FilterWithEnglishLetters( buf, offset, len );
            checked
            {
                for ( int i = 0; i < array.Length; i++ )
                {
                    byte b_1 = Latin1_CharToClass[ array[ i ] ];
                    byte b_2 = Latin1ClassModel[ unchecked(_LastCharClass) * 8 + unchecked(b_1) ];
                    if ( b_2 == 0 )
                    {
                        _State = ProbingState.NotMe;
                        break;
                    }
                    _FreqCounter[ b_2 ]++;
                    _LastCharClass = b_1;
                }
                return (_State);
            }
        }

        public override float GetConfidence()
        {
            if ( _State == ProbingState.NotMe )
            {
                return 0.01f;
            }

            var n = 0;
            checked
            {
                for ( var i = 0; i < 4; i++ )
                {
                    n += _FreqCounter[ i ];
                }
                float x;
                if ( n <= 0 )
                {
                    x = 0f;
                }
                else
                {
                    x  = _FreqCounter[ 3 ] * 1f  / n;
                    x -= _FreqCounter[ 1 ] * 20f / n;
                }
                return (0f <= x) ? (x * 0.5f) : 0f;
            }
        }

        public override void DumpStatus() => Console.WriteLine( $" Latin1Prober: {GetConfidence()} [{GetCharsetName()}]" );
    }
}