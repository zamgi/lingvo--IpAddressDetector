using System;

namespace Ude.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class SingleByteCharSetProber : CharsetProber
    {
        private const int SAMPLE_SIZE = 64;
        private const int SB_ENOUGH_REL_THRESHOLD = 1024;
        private const float POSITIVE_SHORTCUT_THRESHOLD = 0.95f;
        private const float NEGATIVE_SHORTCUT_THRESHOLD = 0.05f;
        private const int SYMBOL_CAT_ORDER = 250;
        private const int NUMBER_OF_SEQ_CAT = 4;
        private const int POSITIVE_CAT = 3;
        private const int NEGATIVE_CAT = 0;

        protected SequenceModel _Model;
        private bool _Reversed;
        private byte _LastOrder;
        private int _TotalSeqs;
        private int _TotalChar;
        private int[] _SeqCounters = new int[ 4 ];
        private int _FreqChar;
        private CharsetProber _NameProber;

        public SingleByteCharSetProber( SequenceModel model ) : this( model, reversed: false, null ) { }
        public SingleByteCharSetProber( SequenceModel model, bool reversed, CharsetProber nameProber )
        {
            _Model = model;
            _Reversed = reversed;
            _NameProber = nameProber;
            Reset();
        }

        public override ProbingState HandleData( byte[] buf, int offset, int len )
        {
            checked
            {
                int num = offset + len;
                for ( int i = offset; i < num; i++ )
                {
                    byte order = _Model.GetOrder( buf[ i ] );
                    if ( order < 250 )
                    {
                        _TotalChar++;
                    }
                    if ( order < 64 )
                    {
                        _FreqChar++;
                        if ( _LastOrder < 64 )
                        {
                            _TotalSeqs++;
                            if ( !_Reversed )
                            {
                                _SeqCounters[ _Model.GetPrecedence( unchecked((int) _LastOrder) * 64 + unchecked((int) order) ) ]++;
                            }
                            else
                            {
                                _SeqCounters[ _Model.GetPrecedence( unchecked((int) order) * 64 + unchecked((int) _LastOrder) ) ]++;
                            }
                        }
                    }
                    _LastOrder = order;
                }
                if ( _State == ProbingState.Detecting && _TotalSeqs > 1024 )
                {
                    float confidence = GetConfidence();
                    if ( confidence > 0.95f )
                    {
                        _State = ProbingState.FoundIt;
                    }
                    else if ( confidence < 0.05f )
                    {
                        _State = ProbingState.NotMe;
                    }
                }
                return _State;
            }
        }

        public override void DumpStatus() => Console.WriteLine( "  SBCS: {0} [{1}]", GetConfidence(), GetCharsetName() );

        public override float GetConfidence()
        {
            float num = 0f;
            if ( _TotalSeqs > 0 )
            {
                num = 1f * (float) _SeqCounters[ 3 ] / (float) _TotalSeqs / _Model.TypicalPositiveRatio;
                num = num * (float) _FreqChar / (float) _TotalChar;
                if ( num >= 1f )
                {
                    num = 0.99f;
                }
                return num;
            }
            return 0.01f;
        }

        public override void Reset()
        {
            _State = ProbingState.Detecting;
            _LastOrder = byte.MaxValue;
            for ( int i = 0; i < 4; i = checked(i + 1) )
            {
                _SeqCounters[ i ] = 0;
            }
            _TotalSeqs = 0;
            _TotalChar = 0;
            _FreqChar = 0;
        }

        public override string GetCharsetName()
        {
            if ( _NameProber != null )
            {
                return _NameProber.GetCharsetName();
            }
            return _Model.CharsetName;
        }
    }
}