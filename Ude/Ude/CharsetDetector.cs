using System.IO;

using Ude.Core;

namespace Ude
{
    /// <summary>
    /// 
    /// </summary>
    public class CharsetDetector : UniversalDetector, ICharsetDetector
    {
        private string _Charset;
        private float  _Confidence;
        public CharsetDetector() : base( 31 ) { }

        public string Charset => _Charset;
        public float Confidence => _Confidence;

        public void Feed( Stream stream )
        {
            var array = new byte[ 1024 ];
            int len;
            while ( (len = stream.Read( array, 0, array.Length )) > 0 && !_Done )
            {
                Feed( array, 0, len );
            }
        }

        public bool IsDone() => _Done;
        public override void Reset()
        {
            _Charset    = null;
            _Confidence = 0f;
            base.Reset();
        }
        protected override void Report( string charset, float confidence )
        {
            _Charset    = charset;
            _Confidence = confidence;
        }
    }
}

