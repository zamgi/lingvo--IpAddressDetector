namespace Ude.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class ISO2022CNSMModel : SMModel
    {
        private static readonly int[] ISO2022CN_cls = new int[ /*32*/ ]
        {
        BitPackage.Pack4bits(2, 0, 0, 0, 0, 0, 0, 0),
        BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
        BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
        BitPackage.Pack4bits(0, 0, 0, 1, 0, 0, 0, 0),
        BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
        BitPackage.Pack4bits(0, 3, 0, 0, 0, 0, 0, 0),
        BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
        BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
        BitPackage.Pack4bits(0, 0, 0, 4, 0, 0, 0, 0),
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

        private static readonly int[] ISO2022CN_st = new int[]
        {
        BitPackage.Pack4bits(0, 3, 1, 0, 0, 0, 0, 0),
        BitPackage.Pack4bits(0, 1, 1, 1, 1, 1, 1, 1),
        BitPackage.Pack4bits(1, 1, 2, 2, 2, 2, 2, 2),
        BitPackage.Pack4bits(2, 2, 2, 1, 1, 1, 4, 1),
        BitPackage.Pack4bits(1, 1, 1, 2, 1, 1, 1, 1),
        BitPackage.Pack4bits(5, 6, 1, 1, 1, 1, 1, 1),
        BitPackage.Pack4bits(1, 1, 1, 2, 1, 1, 1, 1),
        BitPackage.Pack4bits(1, 1, 1, 1, 1, 2, 1, 0)
        };

        public ISO2022CNSMModel(): base( new BitPackage( BitPackage.INDEX_SHIFT_4BITS, 
                                                         BitPackage.SHIFT_MASK_4BITS,
                                                         BitPackage.BIT_SHIFT_4BITS, 
                                                         BitPackage.UNIT_MASK_4BITS, 
                                                         ISO2022CN_cls ), 9, 
                                         new BitPackage( BitPackage.INDEX_SHIFT_4BITS, 
                                                         BitPackage.SHIFT_MASK_4BITS, 
                                                         BitPackage.BIT_SHIFT_4BITS, 
                                                         BitPackage.UNIT_MASK_4BITS, 
                                                         ISO2022CN_st ), 
                                         ISO2022CNCharLenTable, "ISO-2022-CN" )
        {
        }

        private static readonly int[] ISO2022CNCharLenTable;
        static ISO2022CNSMModel() => ISO2022CNCharLenTable = new int[ 9 ];
    }
}