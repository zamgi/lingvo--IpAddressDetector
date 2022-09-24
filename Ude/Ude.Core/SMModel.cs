namespace Ude.Core
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class SMModel
    {
        public const int START = 0;
        public const int ERROR = 1;
        public const int ITSME = 2;

        public BitPackage _ClassTable;
        public BitPackage _StateTable;
        public int[] _CharLenTable;
        private string _Name;
        private int _ClassFactor;

        public SMModel( BitPackage classTable, int classFactor, BitPackage stateTable, int[] charLenTable, string name )
        {
            _ClassTable   = classTable;
            _ClassFactor  = classFactor;
            _StateTable   = stateTable;
            _CharLenTable = charLenTable;
            _Name         = name;
        }
        public string Name => _Name;
        public int ClassFactor => _ClassFactor;
        public int GetClass( byte b ) => _ClassTable.Unpack( b );
    }
}