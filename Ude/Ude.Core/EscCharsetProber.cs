namespace Ude.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class EscCharsetProber : CharsetProber
    {
        private const int CHARSETS_NUM = 4;

        private string _DetectedCharset;
        private CodingStateMachine[] _CodingSM;
        private int _ActiveSM;

        public EscCharsetProber()
        {
            _CodingSM = new[]
            {
                new CodingStateMachine( new HZSMModel() ),
                new CodingStateMachine( new ISO2022CNSMModel() ),
                new CodingStateMachine( new ISO2022JPSMModel() ),
                new CodingStateMachine( new ISO2022KRSMModel() ),
            };
            Reset();
        }

        public override void Reset()
        {
            _State = ProbingState.Detecting;
            for ( int i = 0; i < 4; i = checked(i + 1) )
            {
                _CodingSM[ i ].Reset();
            }
            _ActiveSM = 4;
            _DetectedCharset = null;
        }

        public override ProbingState HandleData( byte[] buf, int offset, int len )
        {
            checked
            {
                int num = offset + len;
                for ( int i = offset; i < num; i++ )
                {
                    if ( _State != 0 )
                    {
                        break;
                    }
                    for ( int num2 = _ActiveSM - 1; num2 >= 0; num2-- )
                    {
                        switch ( _CodingSM[ num2 ].NextState( buf[ i ] ) )
                        {
                            case 1:
                                _ActiveSM--;
                                if ( _ActiveSM == 0 )
                                {
                                    _State = ProbingState.NotMe;
                                    return _State;
                                }
                                if ( num2 != _ActiveSM )
                                {
                                    CodingStateMachine codingStateMachine = _CodingSM[ _ActiveSM ];
                                    _CodingSM[ _ActiveSM ] = _CodingSM[ num2 ];
                                    _CodingSM[ num2 ] = codingStateMachine;
                                }
                                break;
                            case 2:
                                _State = ProbingState.FoundIt;
                                _DetectedCharset = _CodingSM[ num2 ].ModelName;
                                return _State;
                        }
                    }
                }
                return _State;
            }
        }

        public override string GetCharsetName() => _DetectedCharset;
        public override float GetConfidence() => 0.99f;
    }
}