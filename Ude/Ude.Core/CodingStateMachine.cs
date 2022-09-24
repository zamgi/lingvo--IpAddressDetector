namespace Ude.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class CodingStateMachine
    {
        private int     _CurrentState;
        private SMModel _Model;
        private int     _CurrentCharLen;
        private int     _CurrentBytePos;

        public CodingStateMachine( SMModel model )
        {
            _CurrentState = 0;
            _Model        = model;
        }
        public int CurrentCharLen => _CurrentCharLen;
        public string ModelName => _Model.Name;

        public int NextState( byte b )
        {
            int @class = _Model.GetClass( b );
            if ( _CurrentState == 0 )
            {
                _CurrentBytePos = 0;
                _CurrentCharLen = _Model._CharLenTable[ @class ];
            }
            checked
            {
                _CurrentState = _Model._StateTable.Unpack( _CurrentState * _Model.ClassFactor + @class );
                _CurrentBytePos++;
                return _CurrentState;
            }
        }

        public void Reset() => _CurrentState = 0;
    }
}