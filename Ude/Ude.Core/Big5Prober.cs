namespace Ude.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class Big5Prober : CharsetProber
    {
        private CodingStateMachine _CodingSM;
        private BIG5DistributionAnalyser _DistributionAnalyser;
        private byte[] _LastChar = new byte[ 2 ];

        public Big5Prober()
        {
            _CodingSM             = new CodingStateMachine( new BIG5SMModel() );
            _DistributionAnalyser = new BIG5DistributionAnalyser();
            Reset();
        }

        public override ProbingState HandleData( byte[] buf, int offset, int len )
        {
            checked
            {
                int num2 = offset + len;
                for ( int i = offset; i < num2; i++ )
                {
                    switch ( _CodingSM.NextState( buf[ i ] ) )
                    {
                        case 1:
                            _State = ProbingState.NotMe;
                            break;
                        case 2:
                            _State = ProbingState.FoundIt;
                            break;
                        case 0:
                            {
                                int currentCharLen = _CodingSM.CurrentCharLen;
                                if ( i == offset )
                                {
                                    _LastChar[ 1 ] = buf[ offset ];
                                    _DistributionAnalyser.HandleOneChar( _LastChar, 0, currentCharLen );
                                }
                                else
                                {
                                    _DistributionAnalyser.HandleOneChar( buf, i - 1, currentCharLen );
                                }
                                continue;
                            }
                        default:
                            continue;
                    }
                    break;
                }
                _LastChar[ 0 ] = buf[ num2 - 1 ];
                if ( _State == ProbingState.Detecting && _DistributionAnalyser.GotEnoughData() && GetConfidence() > 0.95f )
                {
                    _State = ProbingState.FoundIt;
                }
                return _State;
            }
        }

        public override void Reset()
        {
            _CodingSM.Reset();
            _State = ProbingState.Detecting;
            _DistributionAnalyser.Reset();
        }

        public override string GetCharsetName() => "Big-5";
        public override float GetConfidence() => _DistributionAnalyser.GetConfidence();
    }
}