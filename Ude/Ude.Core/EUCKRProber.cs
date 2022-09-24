namespace Ude.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class EUCKRProber : CharsetProber
    {
        private CodingStateMachine _CodingSM;
        private EUCKRDistributionAnalyser _DistributionAnalyser;
        private byte[] _LastChar = new byte[ 2 ];

        public EUCKRProber()
        {
            _CodingSM             = new CodingStateMachine( new EUCKRSMModel() );
            _DistributionAnalyser = new EUCKRDistributionAnalyser();
            Reset();
        }

        public override string GetCharsetName() => "EUC-KR";

        public override ProbingState HandleData( byte[] buf, int offset, int len )
        {
            checked
            {
                int num = offset + len;
                for ( int i = offset; i < num; i++ )
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

        public override float GetConfidence() => _DistributionAnalyser.GetConfidence();

        public override void Reset()
        {
            _CodingSM.Reset();
            _State = ProbingState.Detecting;
            _DistributionAnalyser.Reset();
        }
    }
}