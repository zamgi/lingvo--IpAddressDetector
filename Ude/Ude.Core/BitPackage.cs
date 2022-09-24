namespace Ude.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class BitPackage
    {
        public static int INDEX_SHIFT_4BITS = 3;
        public static int INDEX_SHIFT_8BITS = 2;
        public static int INDEX_SHIFT_16BITS = 1;
        public static int SHIFT_MASK_4BITS = 7;
        public static int SHIFT_MASK_8BITS = 3;
        public static int SHIFT_MASK_16BITS = 1;
        public static int BIT_SHIFT_4BITS = 2;
        public static int BIT_SHIFT_8BITS = 3;
        public static int BIT_SHIFT_16BITS = 4;
        public static int UNIT_MASK_4BITS = 15;
        public static int UNIT_MASK_8BITS = 255;
        public static int UNIT_MASK_16BITS = 65535;

        private int _IndexShift;
        private int _ShiftMask;
        private int _BitShift;
        private int _UnitMask;
        private int[] _Data;

        public BitPackage( int indexShift, int shiftMask, int bitShift, int unitMask, int[] data )
        {
            _IndexShift = indexShift;
            _ShiftMask = shiftMask;
            _BitShift = bitShift;
            _UnitMask = unitMask;
            _Data = data;
        }

        public static int Pack16bits( int a, int b ) => (b << 16) | a;
        public static int Pack8bits( int a, int b, int c, int d ) => Pack16bits( (b << 8) | a, (d << 8) | c );
        public static int Pack4bits( int a, int b, int c, int d, int e, int f, int g, int h ) => Pack8bits( (b << 4) | a, (d << 4) | c, (f << 4) | e, (h << 4) | g );
        public int Unpack( int i ) => (_Data[ i >> _IndexShift ] >> ((i & _ShiftMask) << _BitShift)) & _UnitMask;
    }
}