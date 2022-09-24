namespace Ude.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class EUCTWProber : CharsetProber
    {
        private CodingStateMachine _CodingSM;
        private EUCTWDistributionAnalyser _DistributionAnalyser;
        private byte[] _LastChar = new byte[ 2 ];

        public EUCTWProber()
        {
            _CodingSM = new CodingStateMachine( new EUCTWSMModel() );
            _DistributionAnalyser = new EUCTWDistributionAnalyser();
            Reset();
        }

        public override ProbingState HandleData( byte[] buf, int offset, int len )
        {
            checked
            {
                int num = offset + len;
                for ( int i = 0; i < num; i++ )
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
                _LastChar[ 0 ] = buf[ num - 1 ];
                if ( _State == ProbingState.Detecting && _DistributionAnalyser.GotEnoughData() && GetConfidence() > 0.95f )
                {
                    _State = ProbingState.FoundIt;
                }
                return _State;
            }
        }

        public override string GetCharsetName() => "x-euc-tw";

        public override void Reset()
        {
            _CodingSM.Reset();
            _State = ProbingState.Detecting;
            _DistributionAnalyser.Reset();
        }

        public override float GetConfidence() => _DistributionAnalyser.GetConfidence();
    }
}