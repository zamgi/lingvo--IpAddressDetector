namespace Ude.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class EUCJPDistributionAnalyser : SJISDistributionAnalyser
    {
        public override int GetOrder( byte[] buf, int offset )
        {
            checked
            {
                if ( buf[ offset ] >= 160 )
                {
                    return 94 * (unchecked((int) buf[ offset ]) - 161) + unchecked((int) buf[ checked(offset + 1) ]) - 161;
                }
                return -1;
            }
        }
    }
}