namespace Ude.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class ISO2022KRSMModel : SMModel
    {
        private static readonly int[] ISO2022KR_cls = new int[ /*32*/ ]
        {
        BitPackage.Pack4bits(2, 0, 0, 0, 0, 0, 0, 0),
        BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
        BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
        BitPackage.Pack4bits(0, 0, 0, 1, 0, 0, 0, 0),
        BitPackage.Pack4bits(0, 0, 0, 0, 3, 0, 0, 0),
        BitPackage.Pack4bits(0, 4, 0, 0, 0, 0, 0, 0),
        BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
        BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
        BitPackage.Pack4bits(0, 0, 0, 5, 0, 0, 0, 0),
        BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
        BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
        BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
        BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
        BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
        BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
        BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
        BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
        BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
        BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
        BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
        BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
        BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
        BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
        BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
        BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
        BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
        BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
        BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
        BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
        BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
        BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
        BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2)
        };

        private static readonly int[] ISO2022KR_st = new int[]
        {
        BitPackage.Pack4bits(0, 3, 1, 0, 0, 0, 1, 1),
        BitPackage.Pack4bits(1, 1, 1, 1, 2, 2, 2, 2),
        BitPackage.Pack4bits(2, 2, 1, 1, 1, 4, 1, 1),
        BitPackage.Pack4bits(1, 1, 1, 1, 5, 1, 1, 1),
        BitPackage.Pack4bits(1, 1, 1, 2, 0, 0, 0, 0)
        };


        public ISO2022KRSMModel(): base( new BitPackage( BitPackage.INDEX_SHIFT_4BITS, 
                                                         BitPackage.SHIFT_MASK_4BITS,
                                                         BitPackage.BIT_SHIFT_4BITS, 
                                                         BitPackage.UNIT_MASK_4BITS, 
                                                         ISO2022KR_cls ), 6, 
                                         new BitPackage( BitPackage.INDEX_SHIFT_4BITS, 
                                                         BitPackage.SHIFT_MASK_4BITS, 
                                                         BitPackage.BIT_SHIFT_4BITS, 
                                                         BitPackage.UNIT_MASK_4BITS,
                                                         ISO2022KR_st ), 
                                         ISO2022KRCharLenTable, "ISO-2022-KR" )
        {
        }

        private static readonly int[] ISO2022KRCharLenTable;
        static ISO2022KRSMModel() => ISO2022KRCharLenTable = new int[ 6 ];
    }
}