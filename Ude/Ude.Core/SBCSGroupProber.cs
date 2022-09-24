using System;

namespace Ude.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class SBCSGroupProber : CharsetProber
    {
        private const int PROBERS_NUM = 13;

        private CharsetProber[] _Probers = new CharsetProber[ 13 ];
        private bool[] _IsActive = new bool[ 13 ];
        private int _BestGuess;
        private int _ActiveNum;

        public SBCSGroupProber()
        {
            _Probers[ 0 ] = new SingleByteCharSetProber( new Win1251Model() );
            _Probers[ 1 ] = new SingleByteCharSetProber( new Koi8rModel() );
            _Probers[ 2 ] = new SingleByteCharSetProber( new Latin5Model() );
            _Probers[ 3 ] = new SingleByteCharSetProber( new MacCyrillicModel() );
            _Probers[ 4 ] = new SingleByteCharSetProber( new Ibm866Model() );
            _Probers[ 5 ] = new SingleByteCharSetProber( new Ibm855Model() );
            _Probers[ 6 ] = new SingleByteCharSetProber( new Latin7Model() );
            _Probers[ 7 ] = new SingleByteCharSetProber( new Win1253Model() );
            _Probers[ 8 ] = new SingleByteCharSetProber( new Latin5BulgarianModel() );
            _Probers[ 9 ] = new SingleByteCharSetProber( new Win1251BulgarianModel() );
            var hebrewProber = new HebrewProber();
            _Probers[ 10 ] = hebrewProber;
            _Probers[ 11 ] = new SingleByteCharSetProber( new Win1255Model(), reversed: false, hebrewProber );
            _Probers[ 12 ] = new SingleByteCharSetProber( new Win1255Model(), reversed: true, hebrewProber );
            hebrewProber.SetModelProbers( _Probers[ 11 ], _Probers[ 12 ] );
            Reset();
        }

        public override ProbingState HandleData( byte[] buf, int offset, int len )
        {
            byte[] array = FilterWithoutEnglishLetters( buf, offset, len );
            if ( array.Length == 0 )
            {
                return _State;
            }
            checked
            {
                for ( int i = 0; i < 13; i++ )
                {
                    if ( !_IsActive[ i ] )
                    {
                        continue;
                    }
                    switch ( _Probers[ i ].HandleData( array, 0, array.Length ) )
                    {
                        case ProbingState.FoundIt:
                            _BestGuess = i;
                            _State = ProbingState.FoundIt;
                            break;
                        case ProbingState.NotMe:
                            _IsActive[ i ] = false;
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
            switch ( _State )
            {
                case ProbingState.FoundIt:
                    return 0.99f;
                case ProbingState.NotMe:
                    return 0.01f;
                default:
                    {
                        for ( int i = 0; i < 13; i = checked(i + 1) )
                        {
                            if ( _IsActive[ i ] )
                            {
                                float confidence = _Probers[ i ].GetConfidence();
                                if ( num < confidence )
                                {
                                    num = confidence;
                                    _BestGuess = i;
                                }
                            }
                        }
                        return num;
                    }
            }
        }

        public override void DumpStatus()
        {
            float confidence = GetConfidence();
            Console.WriteLine( " SBCS Group Prober --------begin status" );
            for ( int i = 0; i < 13; i = checked(i + 1) )
            {
                if ( !_IsActive[ i ] )
                {
                    Console.WriteLine( " inactive: [{0}] (i.e. confidence is too low).", _Probers[ i ].GetCharsetName() );
                }
                else
                {
                    _Probers[ i ].DumpStatus();
                }
            }
            Console.WriteLine( " SBCS Group found best match [{0}] confidence {1}.", _Probers[ _BestGuess ].GetCharsetName(), confidence );
        }

        public override void Reset()
        {
            int num = 0;
            checked
            {
                for ( int i = 0; i < 13; i++ )
                {
                    if ( _Probers[ i ] != null )
                    {
                        _Probers[ i ].Reset();
                        _IsActive[ i ] = true;
                        num++;
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
    }
}