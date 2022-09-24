namespace Ude.Core
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class SequenceModel
    {
        protected byte[] _CharToOrderMap;
        protected byte[] _PrecedenceMatrix;
        protected float _TypicalPositiveRatio;
        protected bool _KeepEnglishLetter;
        protected string _CharsetName;

        public SequenceModel( byte[] charToOrderMap, byte[] precedenceMatrix, float typicalPositiveRatio, bool keepEnglishLetter, string charsetName )
        {
            _CharToOrderMap       = charToOrderMap;
            _PrecedenceMatrix     = precedenceMatrix;
            _TypicalPositiveRatio = typicalPositiveRatio;
            _KeepEnglishLetter    = keepEnglishLetter;
            _CharsetName          = charsetName;
        }

        public float TypicalPositiveRatio => _TypicalPositiveRatio;
        public bool KeepEnglishLetter => _KeepEnglishLetter;
        public string CharsetName => _CharsetName;

        public byte GetOrder( byte b ) => _CharToOrderMap[ b ];
        public byte GetPrecedence( int pos ) => _PrecedenceMatrix[ pos ];
    }
}