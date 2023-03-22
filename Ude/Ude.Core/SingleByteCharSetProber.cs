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
        private int[] _SeqCounters = new int[ NUMBER_OF_SEQ_CAT ];
        private int _FreqChar;
        private CharsetProber _NameProber;

        public SingleByteCharSetProber( SequenceModel model ) : this( model, reversed: false, null ) { }
        public SingleByteCharSetProber( SequenceModel model, bool reversed, CharsetProber nameProber )
        {
            _Model      = model;
            _Reversed   = reversed;
            _NameProber = nameProber;
            Reset();
        }

        public override ProbingState HandleData( byte[] buf, int offset, int len )
        {
            checked
            {
                var num = offset + len;
                for ( var i = offset; i < num; i++ )
                {
                    byte order = _Model.GetOrder( buf[ i ] );
                    if ( order < SYMBOL_CAT_ORDER )
                    {
                        _TotalChar++;
                    }
                    if ( order < SAMPLE_SIZE )
                    {
                        _FreqChar++;
                        if ( _LastOrder < SAMPLE_SIZE )
                        {
                            _TotalSeqs++;
                            if ( !_Reversed )
                            {
                                _SeqCounters[ _Model.GetPrecedence( unchecked((int) _LastOrder) * SAMPLE_SIZE + unchecked((int) order) ) ]++;
                            }
                            else
                            {
                                _SeqCounters[ _Model.GetPrecedence( unchecked((int) order) * SAMPLE_SIZE + unchecked((int) _LastOrder) ) ]++;
                            }
                        }
                    }
                    _LastOrder = order;
                }
                if ( _State == ProbingState.Detecting && _TotalSeqs > SB_ENOUGH_REL_THRESHOLD )
                {
                    float confidence = GetConfidence();
                    if ( confidence > POSITIVE_SHORTCUT_THRESHOLD )
                    {
                        _State = ProbingState.FoundIt;
                    }
                    else if ( confidence < NEGATIVE_SHORTCUT_THRESHOLD )
                    {
                        _State = ProbingState.NotMe;
                    }
                }
                return (_State);
            }
        }

        public override void DumpStatus() => Console.WriteLine( $"  SBCS: {GetConfidence()} [{GetCharsetName()}]" );

        public override float GetConfidence()
        {
            if ( _TotalSeqs > 0 )
            {
                var n = 1f * (float) _SeqCounters[ POSITIVE_CAT ] / (float) _TotalSeqs / _Model.TypicalPositiveRatio;
                    n = n * (float) _FreqChar / (float) _TotalChar;
                if ( n >= 1f )
                {
                    n = 0.99f;
                }
                return (n);
            }
            return (0.01f);
        }

        public override void Reset()
        {
            _State = ProbingState.Detecting;
            _LastOrder = byte.MaxValue;
            for ( var i = 0; i < NUMBER_OF_SEQ_CAT; i++ )
            {
                _SeqCounters[ i ] = 0;
            }
            _TotalSeqs = 0;
            _TotalChar = 0;
            _FreqChar  = 0;
        }

        public override string GetCharsetName() => _NameProber?.GetCharsetName() ?? _Model.CharsetName;
    }
}