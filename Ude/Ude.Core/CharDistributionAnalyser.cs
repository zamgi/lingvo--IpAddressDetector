namespace Ude.Core
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CharDistributionAnalyser
    {
        protected const float SURE_YES = 0.99f;
        protected const float SURE_NO = 0.01f;
        protected const int MINIMUM_DATA_THRESHOLD = 4;
        protected const int ENOUGH_DATA_THRESHOLD = 1024;

        protected bool _Done;
        protected int _FreqChars;
        protected int _TotalChars;
        protected int[] _CharToFreqOrder;
        protected int _TableSize;
        protected float _TypicalDistributionRatio;

        public CharDistributionAnalyser() => Reset();

        public abstract int GetOrder( byte[] buf, int offset );

        public void HandleOneChar( byte[] buf, int offset, int charLen )
        {
            int num = ((charLen == 2) ? GetOrder( buf, offset ) : (-1));
            checked
            {
                if ( num >= 0 )
                {
                    _TotalChars++;
                    if ( num < _TableSize && 512 > _CharToFreqOrder[ num ] )
                    {
                        _FreqChars++;
                    }
                }
            }
        }

        public virtual void Reset()
        {
            _Done = false;
            _TotalChars = 0;
            _FreqChars = 0;
        }

        public virtual float GetConfidence()
        {
            if ( _TotalChars <= 0 || _FreqChars <= 4 )
            {
                return 0.01f;
            }
            if ( _TotalChars != _FreqChars )
            {
                float num = (float) _FreqChars / ((float) checked(_TotalChars - _FreqChars) * _TypicalDistributionRatio);
                if ( num < 0.99f )
                {
                    return num;
                }
            }
            return 0.99f;
        }

        public bool GotEnoughData() => (1024 < _TotalChars);
    }
}