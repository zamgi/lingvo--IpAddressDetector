namespace Ude.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class EUCKRSMModel : SMModel
    {
        private static readonly int[] EUCKR_cls = new int[ /*32*/ ]
        {
        BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
        BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 0, 0),
        BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
        BitPackage.Pack4bits(1, 1, 1, 0, 1, 1, 1, 1),
        BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
        BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
        BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
        BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
        BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
        BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
        BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
        BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
        BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
        BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
        BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
        BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
        BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
        BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
        BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
        BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
        BitPackage.Pack4bits(0, 2, 2, 2, 2, 2, 2, 2),
        BitPackage.Pack4bits(2, 2, 2, 2, 2, 3, 3, 3),
        BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
        BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
        BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
        BitPackage.Pack4bits(2, 3, 2, 2, 2, 2, 2, 2),
        BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
        BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
        BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
        BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
        BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
        BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 0)
        };

        private static readonly int[] EUCKR_st = new int[]
        {
        BitPackage.Pack4bits(1, 0, 3, 1, 1, 1, 1, 1),
        BitPackage.Pack4bits(2, 2, 2, 2, 1, 1, 0, 0)
        };

        private static readonly int[] EUCKRCharLenTable = new int[] { 0, 1, 2, 0 };

        public EUCKRSMModel(): base( new BitPackage( BitPackage.INDEX_SHIFT_4BITS, 
                                                     BitPackage.SHIFT_MASK_4BITS,
                                                     BitPackage.BIT_SHIFT_4BITS, 
                                                     BitPackage.UNIT_MASK_4BITS, 
                                                     EUCKR_cls ), 4,
                                     new BitPackage( BitPackage.INDEX_SHIFT_4BITS, 
                                                     BitPackage.SHIFT_MASK_4BITS, 
                                                     BitPackage.BIT_SHIFT_4BITS, 
                                                     BitPackage.UNIT_MASK_4BITS,
                                                     EUCKR_st ),
                                     EUCKRCharLenTable, "EUC-KR" )
        {
        }
    }
}