namespace Ude.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class UTF8Prober : CharsetProber
    {
        private static float ONE_CHAR_PROB = 0.5f;
        private CodingStateMachine _CodingSM;
        private int _NumOfMBChar;

        public UTF8Prober()
        {
            _NumOfMBChar = 0;
            _CodingSM    = new CodingStateMachine( new UTF8SMModel() );
            Reset();
        }

        public override string GetCharsetName() => "UTF-8";
        public override void Reset()
        {
            _CodingSM.Reset();
            _NumOfMBChar = 0;
            _State       = ProbingState.Detecting;
        }

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
                            if ( _CodingSM.CurrentCharLen >= 2 )
                            {
                                _NumOfMBChar++;
                            }
                            continue;
                        default:
                            continue;
                    }
                    break;
                }
                if ( (_State == ProbingState.Detecting) && (GetConfidence() > 0.95f) )
                {
                    _State = ProbingState.FoundIt;
                }
                return (_State);
            }
        }

        public override float GetConfidence()
        {
            var n = 0.99f;
            if ( _NumOfMBChar < 6 )
            {
                for ( var i = 0; i < _NumOfMBChar; i++ )
                {
                    n *= ONE_CHAR_PROB;
                }
                return (1f - n);
            }
            return (0.99f);
        }
    }
}