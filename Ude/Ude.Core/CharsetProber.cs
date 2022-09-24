using System.IO;

namespace Ude.Core
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CharsetProber
    {
        protected const float SHORTCUT_THRESHOLD = 0.95f;
        private const byte SPACE = 32;
        private const byte CAPITAL_A = 65;
        private const byte CAPITAL_Z = 90;
        private const byte SMALL_A = 97;
        private const byte SMALL_Z = 122;
        private const byte LESS_THAN = 60;
        private const byte GREATER_THAN = 62;

        protected ProbingState _State;

        public abstract ProbingState HandleData( byte[] buf, int offset, int len );
        public abstract void Reset();
        public abstract string GetCharsetName();
        public abstract float GetConfidence();

        public virtual ProbingState GetState() => _State;
        public virtual void SetOption() { }
        public virtual void DumpStatus() { }

        protected static byte[] FilterWithoutEnglishLetters( byte[] buf, int offset, int len )
        {
            checked
            {
                using MemoryStream memoryStream = new MemoryStream( buf.Length );
                bool flag = false;
                int num = offset + len;
                int num2 = offset;
                int i;
                for ( i = offset; i < num; i++ )
                {
                    byte b = buf[ i ];
                    if ( (b & 0x80u) != 0 )
                    {
                        flag = true;
                    }
                    else if ( b < 65 || (b > 90 && b < 97) || b > 122 )
                    {
                        if ( flag && i > num2 )
                        {
                            memoryStream.Write( buf, num2, i - num2 );
                            memoryStream.WriteByte( 32 );
                            flag = false;
                        }
                        num2 = i + 1;
                    }
                }
                if ( flag && i > num2 )
                {
                    memoryStream.Write( buf, num2, i - num2 );
                }
                memoryStream.SetLength( memoryStream.Position );
                return memoryStream.ToArray();
            }
        }

        protected static byte[] FilterWithEnglishLetters( byte[] buf, int offset, int len )
        {
            checked
            {
                using var ms = new MemoryStream( buf.Length );
                bool flag = false;
                int num = offset + len;
                int num2 = offset;
                int i;
                for ( i = offset; i < num; i++ )
                {
                    byte b = buf[ i ];
                    switch ( b )
                    {
                        case 62:
                            flag = false;
                            break;
                        case 60:
                            flag = true;
                            break;
                    }
                    if ( (b & 0x80u) != 0 )
                    {
                        continue;
                    }
                    switch ( b )
                    {
                        case 65:
                        case 66:
                        case 67:
                        case 68:
                        case 69:
                        case 70:
                        case 71:
                        case 72:
                        case 73:
                        case 74:
                        case 75:
                        case 76:
                        case 77:
                        case 78:
                        case 79:
                        case 80:
                        case 81:
                        case 82:
                        case 83:
                        case 84:
                        case 85:
                        case 86:
                        case 87:
                        case 88:
                        case 89:
                        case 90:
                        case 97:
                        case 98:
                        case 99:
                        case 100:
                        case 101:
                        case 102:
                        case 103:
                        case 104:
                        case 105:
                        case 106:
                        case 107:
                        case 108:
                        case 109:
                        case 110:
                        case 111:
                        case 112:
                        case 113:
                        case 114:
                        case 115:
                        case 116:
                        case 117:
                        case 118:
                        case 119:
                        case 120:
                        case 121:
                        case 122:
                            continue;
                    }
                    if ( i > num2 && !flag )
                    {
                        ms.Write( buf, num2, i - num2 );
                        ms.WriteByte( 32 );
                    }
                    num2 = i + 1;
                }
                if ( !flag && i > num2 )
                {
                    ms.Write( buf, num2, i - num2 );
                }
                ms.SetLength( ms.Position );
                return ms.ToArray();
            }
        }
    }
}