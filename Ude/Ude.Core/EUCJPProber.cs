namespace Ude.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class EUCJPProber : CharsetProber
    {
        private CodingStateMachine _CodingSM;
        private EUCJPContextAnalyser _ContextAnalyser;
        private EUCJPDistributionAnalyser _DistributionAnalyser;
        private byte[] _LastChar = new byte[ 2 ];

        public EUCJPProber()
        {
            _CodingSM             = new CodingStateMachine( new EUCJPSMModel() );
            _DistributionAnalyser = new EUCJPDistributionAnalyser();
            _ContextAnalyser      = new EUCJPContextAnalyser();
            Reset();
        }

        public override string GetCharsetName() => "EUC-JP";

        public override ProbingState HandleData( byte[] buf, int offset, int len )
        {
            checked
            {
                var n = offset + len;
                for ( var i = offset; i < n; i++ )
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
                                    _ContextAnalyser.HandleOneChar( _LastChar, 0, currentCharLen );
                                    _DistributionAnalyser.HandleOneChar( _LastChar, 0, currentCharLen );
                                }
                                else
                                {
                                    _ContextAnalyser.HandleOneChar( buf, i - 1, currentCharLen );
                                    _DistributionAnalyser.HandleOneChar( buf, i - 1, currentCharLen );
                                }
                                continue;
                            }
                        default:
                            continue;
                    }
                    break;
                }
                _LastChar[ 0 ] = buf[ n - 1 ];
                if ( _State == ProbingState.Detecting && _ContextAnalyser.GotEnoughData() && GetConfidence() > 0.95f )
                {
                    _State = ProbingState.FoundIt;
                }
                return (_State);
            }
        }

        public override void Reset()
        {
            _CodingSM.Reset();
            _State = ProbingState.Detecting;
            _ContextAnalyser.Reset();
            _DistributionAnalyser.Reset();
        }

        public override float GetConfidence()
        {
            float confidence  = _ContextAnalyser.GetConfidence();
            float confidence2 = _DistributionAnalyser.GetConfidence();
            return (confidence < confidence2) ? confidence2 : confidence;
        }
    }
}