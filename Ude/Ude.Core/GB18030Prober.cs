namespace Ude.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class GB18030Prober : CharsetProber
    {
        private CodingStateMachine _CodingSM;
        private GB18030DistributionAnalyser _Analyser;
        private byte[] _LastChar;

        public GB18030Prober()
        {
            _LastChar = new byte[ 2 ];
            _CodingSM = new CodingStateMachine( new GB18030SMModel() );
            _Analyser = new GB18030DistributionAnalyser();
            Reset();
        }

        public override string GetCharsetName() => "gb18030";

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
                                    _Analyser.HandleOneChar( _LastChar, 0, currentCharLen );
                                }
                                else
                                {
                                    _Analyser.HandleOneChar( buf, i - 1, currentCharLen );
                                }
                                continue;
                            }
                        default:
                            continue;
                    }
                    break;
                }
                _LastChar[ 0 ] = buf[ num2 - 1 ];
                if ( _State == ProbingState.Detecting && _Analyser.GotEnoughData() && GetConfidence() > 0.95f )
                {
                    _State = ProbingState.FoundIt;
                }
                return _State;
            }
        }

        public override float GetConfidence() => _Analyser.GetConfidence();

        public override void Reset()
        {
            _CodingSM.Reset();
            _State = ProbingState.Detecting;
            _Analyser.Reset();
        }
    }
}