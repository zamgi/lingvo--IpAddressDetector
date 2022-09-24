namespace Ude.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class SJISProber : CharsetProber
    {
        private CodingStateMachine _CodingSM;
        private SJISContextAnalyser _ContextAnalyser;
        private SJISDistributionAnalyser _DistributionAnalyser;
        private byte[] _LastChar = new byte[ 2 ];

        public SJISProber()
        {
            _CodingSM             = new CodingStateMachine( new SJISSMModel() );
            _DistributionAnalyser = new SJISDistributionAnalyser();
            _ContextAnalyser      = new SJISContextAnalyser();
            Reset();
        }

        public override string GetCharsetName() => "Shift-JIS";

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
                                    _ContextAnalyser.HandleOneChar( _LastChar, 2 - currentCharLen, currentCharLen );
                                    _DistributionAnalyser.HandleOneChar( _LastChar, 0, currentCharLen );
                                }
                                else
                                {
                                    _ContextAnalyser.HandleOneChar( buf, i + 1 - currentCharLen, currentCharLen );
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
                if ( _State == ProbingState.Detecting && _ContextAnalyser.GotEnoughData() && GetConfidence() > 0.95f )
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
            _ContextAnalyser.Reset();
            _DistributionAnalyser.Reset();
        }

        public override float GetConfidence()
        {
            float confidence = _ContextAnalyser.GetConfidence();
            float confidence2 = _DistributionAnalyser.GetConfidence();
            if ( !(confidence > confidence2) )
            {
                return confidence2;
            }
            return confidence;
        }
    }
}