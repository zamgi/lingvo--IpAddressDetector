namespace Ude.Core
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class JapaneseContextAnalyser
    {
        protected const int CATEGORIES_NUM = 6;
        protected const int ENOUGH_REL_THRESHOLD = 100;
        protected const int MAX_REL_THRESHOLD = 1000;
        protected const int MINIMUM_DATA_THRESHOLD = 4;
        protected const float DONT_KNOW = -1f;

        protected static byte[,] jp2CharContext = new byte[ 83, 83 ]
        {
            {
                0, 0, 0, 2, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 1, 0, 0,
                0, 1, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 1
            },
            {
                2, 4, 0, 4, 0, 3, 0, 4, 0, 3,
                4, 4, 4, 2, 4, 3, 3, 4, 3, 2,
                3, 3, 4, 2, 3, 3, 3, 2, 4, 1,
                4, 3, 3, 1, 5, 4, 3, 4, 3, 4,
                3, 5, 3, 0, 3, 5, 4, 2, 0, 3,
                1, 0, 3, 3, 0, 3, 3, 0, 1, 1,
                0, 4, 3, 0, 3, 3, 0, 4, 0, 2,
                0, 3, 5, 5, 5, 5, 4, 0, 4, 1,
                0, 3, 4
            },
            {
                0, 0, 1, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 1, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 2
            },
            {
                0, 4, 0, 5, 0, 5, 0, 4, 0, 4,
                5, 4, 4, 3, 5, 3, 5, 1, 5, 3,
                4, 3, 4, 4, 3, 4, 3, 3, 4, 3,
                5, 4, 4, 3, 5, 5, 3, 5, 5, 5,
                3, 5, 5, 3, 4, 5, 5, 3, 1, 3,
                2, 0, 3, 4, 0, 4, 2, 0, 4, 2,
                1, 5, 3, 2, 3, 5, 0, 4, 0, 2,
                0, 5, 4, 4, 5, 4, 5, 0, 4, 0,
                0, 4, 4
            },
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0
            },
            {
                0, 3, 0, 4, 0, 3, 0, 3, 0, 4,
                5, 4, 3, 3, 3, 3, 4, 3, 5, 4,
                4, 3, 5, 4, 4, 3, 4, 3, 4, 4,
                4, 4, 5, 3, 4, 4, 3, 4, 5, 5,
                4, 5, 5, 1, 4, 5, 4, 3, 0, 3,
                3, 1, 3, 3, 0, 4, 4, 0, 3, 3,
                1, 5, 3, 3, 3, 5, 0, 4, 0, 3,
                0, 4, 4, 3, 4, 3, 3, 0, 4, 1,
                1, 3, 4
            },
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 1, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0
            },
            {
                0, 4, 0, 3, 0, 3, 0, 4, 0, 3,
                4, 4, 3, 2, 2, 1, 2, 1, 3, 1,
                3, 3, 3, 3, 3, 4, 3, 1, 3, 3,
                5, 3, 3, 0, 4, 3, 0, 5, 4, 3,
                3, 5, 4, 4, 3, 4, 4, 5, 0, 1,
                2, 0, 1, 2, 0, 2, 2, 0, 1, 0,
                0, 5, 2, 2, 1, 4, 0, 3, 0, 1,
                0, 4, 4, 3, 5, 4, 3, 0, 2, 1,
                0, 4, 3
            },
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0
            },
            {
                0, 3, 0, 5, 0, 4, 0, 2, 1, 4,
                4, 2, 4, 1, 4, 2, 4, 2, 4, 3,
                3, 3, 4, 3, 3, 3, 3, 1, 4, 2,
                3, 3, 3, 1, 4, 4, 1, 1, 1, 4,
                3, 3, 2, 0, 2, 4, 3, 2, 0, 3,
                3, 0, 3, 1, 1, 0, 0, 0, 3, 3,
                0, 4, 2, 2, 3, 4, 0, 4, 0, 3,
                0, 4, 4, 5, 3, 4, 4, 0, 3, 0,
                0, 1, 4
            },
            {
                1, 4, 0, 4, 0, 4, 0, 4, 0, 3,
                5, 4, 4, 3, 4, 3, 5, 4, 3, 3,
                4, 3, 5, 4, 4, 4, 4, 3, 4, 2,
                4, 3, 3, 1, 5, 4, 3, 2, 4, 5,
                4, 5, 5, 4, 4, 5, 4, 4, 0, 3,
                2, 2, 3, 3, 0, 4, 3, 1, 3, 2,
                1, 4, 3, 3, 4, 5, 0, 3, 0, 2,
                0, 4, 5, 5, 4, 5, 4, 0, 4, 0,
                0, 5, 4
            },
            {
                0, 5, 0, 5, 0, 4, 0, 3, 0, 4,
                4, 3, 4, 3, 3, 3, 4, 0, 4, 4,
                4, 3, 4, 3, 4, 3, 3, 1, 4, 2,
                4, 3, 4, 0, 5, 4, 1, 4, 5, 4,
                4, 5, 3, 2, 4, 3, 4, 3, 2, 4,
                1, 3, 3, 3, 2, 3, 2, 0, 4, 3,
                3, 4, 3, 3, 3, 4, 0, 4, 0, 3,
                0, 4, 5, 4, 4, 4, 3, 0, 4, 1,
                0, 1, 3
            },
            {
                0, 3, 1, 4, 0, 3, 0, 2, 0, 3,
                4, 4, 3, 1, 4, 2, 3, 3, 4, 3,
                4, 3, 4, 3, 4, 4, 3, 2, 3, 1,
                5, 4, 4, 1, 4, 4, 3, 5, 4, 4,
                3, 5, 5, 4, 3, 4, 4, 3, 1, 2,
                3, 1, 2, 2, 0, 3, 2, 0, 3, 1,
                0, 5, 3, 3, 3, 4, 3, 3, 3, 3,
                4, 4, 4, 4, 5, 4, 2, 0, 3, 3,
                2, 4, 3
            },
            {
                0, 2, 0, 3, 0, 1, 0, 1, 0, 0,
                3, 2, 0, 0, 2, 0, 1, 0, 2, 1,
                3, 3, 3, 1, 2, 3, 1, 0, 1, 0,
                4, 2, 1, 1, 3, 3, 0, 4, 3, 3,
                1, 4, 3, 3, 0, 3, 3, 2, 0, 0,
                0, 0, 1, 0, 0, 2, 0, 0, 0, 0,
                0, 4, 1, 0, 2, 3, 2, 2, 2, 1,
                3, 3, 3, 4, 4, 3, 2, 0, 3, 1,
                0, 3, 3
            },
            {
                0, 4, 0, 4, 0, 3, 0, 3, 0, 4,
                4, 4, 3, 3, 3, 3, 3, 3, 4, 3,
                4, 2, 4, 3, 4, 3, 3, 2, 4, 3,
                4, 5, 4, 1, 4, 5, 3, 5, 4, 5,
                3, 5, 4, 0, 3, 5, 5, 3, 1, 3,
                3, 2, 2, 3, 0, 3, 4, 1, 3, 3,
                2, 4, 3, 3, 3, 4, 0, 4, 0, 3,
                0, 4, 5, 4, 4, 5, 3, 0, 4, 1,
                0, 3, 4
            },
            {
                0, 2, 0, 3, 0, 3, 0, 0, 0, 2,
                2, 2, 1, 0, 1, 0, 0, 0, 3, 0,
                3, 0, 3, 0, 1, 3, 1, 0, 3, 1,
                3, 3, 3, 1, 3, 3, 3, 0, 1, 3,
                1, 3, 4, 0, 0, 3, 1, 1, 0, 3,
                2, 0, 0, 0, 0, 1, 3, 0, 1, 0,
                0, 3, 3, 2, 0, 3, 0, 0, 0, 0,
                0, 3, 4, 3, 4, 3, 3, 0, 3, 0,
                0, 2, 3
            },
            {
                2, 3, 0, 3, 0, 2, 0, 1, 0, 3,
                3, 4, 3, 1, 3, 1, 1, 1, 3, 1,
                4, 3, 4, 3, 3, 3, 0, 0, 3, 1,
                5, 4, 3, 1, 4, 3, 2, 5, 5, 4,
                4, 4, 4, 3, 3, 4, 4, 4, 0, 2,
                1, 1, 3, 2, 0, 1, 2, 0, 0, 1,
                0, 4, 1, 3, 3, 3, 0, 3, 0, 1,
                0, 4, 4, 4, 5, 5, 3, 0, 2, 0,
                0, 4, 4
            },
            {
                0, 2, 0, 1, 0, 3, 1, 3, 0, 2,
                3, 3, 3, 0, 3, 1, 0, 0, 3, 0,
                3, 2, 3, 1, 3, 2, 1, 1, 0, 0,
                4, 2, 1, 0, 2, 3, 1, 4, 3, 2,
                0, 4, 4, 3, 1, 3, 1, 3, 0, 1,
                0, 0, 1, 0, 0, 0, 1, 0, 0, 0,
                0, 4, 1, 1, 1, 2, 0, 3, 0, 0,
                0, 3, 4, 2, 4, 3, 2, 0, 1, 0,
                0, 3, 3
            },
            {
                0, 1, 0, 4, 0, 5, 0, 4, 0, 2,
                4, 4, 2, 3, 3, 2, 3, 3, 5, 3,
                3, 3, 4, 3, 4, 2, 3, 0, 4, 3,
                3, 3, 4, 1, 4, 3, 2, 1, 5, 5,
                3, 4, 5, 1, 3, 5, 4, 2, 0, 3,
                3, 0, 1, 3, 0, 4, 2, 0, 1, 3,
                1, 4, 3, 3, 3, 3, 0, 3, 0, 1,
                0, 3, 4, 4, 4, 5, 5, 0, 3, 0,
                1, 4, 5
            },
            {
                0, 2, 0, 3, 0, 3, 0, 0, 0, 2,
                3, 1, 3, 0, 4, 0, 1, 1, 3, 0,
                3, 4, 3, 2, 3, 1, 0, 3, 3, 2,
                3, 1, 3, 0, 2, 3, 0, 2, 1, 4,
                1, 2, 2, 0, 0, 3, 3, 0, 0, 2,
                0, 0, 0, 1, 0, 0, 0, 0, 2, 2,
                0, 3, 2, 1, 3, 3, 0, 2, 0, 2,
                0, 0, 3, 3, 1, 2, 4, 0, 3, 0,
                2, 2, 3
            },
            {
                2, 4, 0, 5, 0, 4, 0, 4, 0, 2,
                4, 4, 4, 3, 4, 3, 3, 3, 1, 2,
                4, 3, 4, 3, 4, 4, 5, 0, 3, 3,
                3, 3, 2, 0, 4, 3, 1, 4, 3, 4,
                1, 4, 4, 3, 3, 4, 4, 3, 1, 2,
                3, 0, 4, 2, 0, 4, 1, 0, 3, 3,
                0, 4, 3, 3, 3, 4, 0, 4, 0, 2,
                0, 3, 5, 3, 4, 5, 2, 0, 3, 0,
                0, 4, 5
            },
            {
                0, 3, 0, 4, 0, 1, 0, 1, 0, 1,
                3, 2, 2, 1, 3, 0, 3, 0, 2, 0,
                2, 0, 3, 0, 2, 0, 0, 0, 1, 0,
                1, 1, 0, 0, 3, 1, 0, 0, 0, 4,
                0, 3, 1, 0, 2, 1, 3, 0, 0, 0,
                0, 0, 0, 3, 0, 0, 0, 0, 0, 0,
                0, 4, 2, 2, 3, 1, 0, 3, 0, 0,
                0, 1, 4, 4, 4, 3, 0, 0, 4, 0,
                0, 1, 4
            },
            {
                1, 4, 1, 5, 0, 3, 0, 3, 0, 4,
                5, 4, 4, 3, 5, 3, 3, 4, 4, 3,
                4, 1, 3, 3, 3, 3, 2, 1, 4, 1,
                5, 4, 3, 1, 4, 4, 3, 5, 4, 4,
                3, 5, 4, 3, 3, 4, 4, 4, 0, 3,
                3, 1, 2, 3, 0, 3, 1, 0, 3, 3,
                0, 5, 4, 4, 4, 4, 4, 4, 3, 3,
                5, 4, 4, 3, 3, 5, 4, 0, 3, 2,
                0, 4, 4
            },
            {
                0, 2, 0, 3, 0, 1, 0, 0, 0, 1,
                3, 3, 3, 2, 4, 1, 3, 0, 3, 1,
                3, 0, 2, 2, 1, 1, 0, 0, 2, 0,
                4, 3, 1, 0, 4, 3, 0, 4, 4, 4,
                1, 4, 3, 1, 1, 3, 3, 1, 0, 2,
                0, 0, 1, 3, 0, 0, 0, 0, 2, 0,
                0, 4, 3, 2, 4, 3, 5, 4, 3, 3,
                3, 4, 3, 3, 4, 3, 3, 0, 2, 1,
                0, 3, 3
            },
            {
                0, 2, 0, 4, 0, 3, 0, 2, 0, 2,
                5, 5, 3, 4, 4, 4, 4, 1, 4, 3,
                3, 0, 4, 3, 4, 3, 1, 3, 3, 2,
                4, 3, 0, 3, 4, 3, 0, 3, 4, 4,
                2, 4, 4, 0, 4, 5, 3, 3, 2, 2,
                1, 1, 1, 2, 0, 1, 5, 0, 3, 3,
                2, 4, 3, 3, 3, 4, 0, 3, 0, 2,
                0, 4, 4, 3, 5, 5, 0, 0, 3, 0,
                2, 3, 3
            },
            {
                0, 3, 0, 4, 0, 3, 0, 1, 0, 3,
                4, 3, 3, 1, 3, 3, 3, 0, 3, 1,
                3, 0, 4, 3, 3, 1, 1, 0, 3, 0,
                3, 3, 0, 0, 4, 4, 0, 1, 5, 4,
                3, 3, 5, 0, 3, 3, 4, 3, 0, 2,
                0, 1, 1, 1, 0, 1, 3, 0, 1, 2,
                1, 3, 3, 2, 3, 3, 0, 3, 0, 1,
                0, 1, 3, 3, 4, 4, 1, 0, 1, 2,
                2, 1, 3
            },
            {
                0, 1, 0, 4, 0, 4, 0, 3, 0, 1,
                3, 3, 3, 2, 3, 1, 1, 0, 3, 0,
                3, 3, 4, 3, 2, 4, 2, 0, 1, 0,
                4, 3, 2, 0, 4, 3, 0, 5, 3, 3,
                2, 4, 4, 4, 3, 3, 3, 4, 0, 1,
                3, 0, 0, 1, 0, 0, 1, 0, 0, 0,
                0, 4, 2, 3, 3, 3, 0, 3, 0, 0,
                0, 4, 4, 4, 5, 3, 2, 0, 3, 3,
                0, 3, 5
            },
            {
                0, 2, 0, 3, 0, 0, 0, 3, 0, 1,
                3, 0, 2, 0, 0, 0, 1, 0, 3, 1,
                1, 3, 3, 0, 0, 3, 0, 0, 3, 0,
                2, 3, 1, 0, 3, 1, 0, 3, 3, 2,
                0, 4, 2, 2, 0, 2, 0, 0, 0, 4,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 2, 1, 2, 0, 1, 0, 1, 0, 0,
                0, 1, 3, 1, 2, 0, 0, 0, 1, 0,
                0, 1, 4
            },
            {
                0, 3, 0, 3, 0, 5, 0, 1, 0, 2,
                4, 3, 1, 3, 3, 2, 1, 1, 5, 2,
                1, 0, 5, 1, 2, 0, 0, 0, 3, 3,
                2, 2, 3, 2, 4, 3, 0, 0, 3, 3,
                1, 3, 3, 0, 2, 5, 3, 4, 0, 3,
                3, 0, 1, 2, 0, 2, 2, 0, 3, 2,
                0, 2, 2, 3, 3, 3, 0, 2, 0, 1,
                0, 3, 4, 4, 2, 5, 4, 0, 3, 0,
                0, 3, 5
            },
            {
                0, 3, 0, 3, 0, 3, 0, 1, 0, 3,
                3, 3, 3, 0, 3, 0, 2, 0, 2, 1,
                1, 0, 2, 0, 1, 0, 0, 0, 2, 1,
                0, 0, 1, 0, 3, 2, 0, 0, 3, 3,
                1, 2, 3, 1, 0, 3, 3, 0, 0, 1,
                0, 0, 0, 0, 0, 2, 0, 0, 0, 0,
                0, 2, 3, 1, 2, 3, 0, 3, 0, 1,
                0, 3, 2, 1, 0, 4, 3, 0, 1, 1,
                0, 3, 3
            },
            {
                0, 4, 0, 5, 0, 3, 0, 3, 0, 4,
                5, 5, 4, 3, 5, 3, 4, 3, 5, 3,
                3, 2, 5, 3, 4, 4, 4, 3, 4, 3,
                4, 5, 5, 3, 4, 4, 3, 4, 4, 5,
                4, 4, 4, 3, 4, 5, 5, 4, 2, 3,
                4, 2, 3, 4, 0, 3, 3, 1, 4, 3,
                2, 4, 3, 3, 5, 5, 0, 3, 0, 3,
                0, 5, 5, 5, 5, 4, 4, 0, 4, 0,
                1, 4, 4
            },
            {
                0, 4, 0, 4, 0, 3, 0, 3, 0, 3,
                5, 4, 4, 2, 3, 2, 5, 1, 3, 2,
                5, 1, 4, 2, 3, 2, 3, 3, 4, 3,
                3, 3, 3, 2, 5, 4, 1, 3, 3, 5,
                3, 4, 4, 0, 4, 4, 3, 1, 1, 3,
                1, 0, 2, 3, 0, 2, 3, 0, 3, 0,
                0, 4, 3, 1, 3, 4, 0, 3, 0, 2,
                0, 4, 4, 4, 3, 4, 5, 0, 4, 0,
                0, 3, 4
            },
            {
                0, 3, 0, 3, 0, 3, 1, 2, 0, 3,
                4, 4, 3, 3, 3, 0, 2, 2, 4, 3,
                3, 1, 3, 3, 3, 1, 1, 0, 3, 1,
                4, 3, 2, 3, 4, 4, 2, 4, 4, 4,
                3, 4, 4, 3, 2, 4, 4, 3, 1, 3,
                3, 1, 3, 3, 0, 4, 1, 0, 2, 2,
                1, 4, 3, 2, 3, 3, 5, 4, 3, 3,
                5, 4, 4, 3, 3, 0, 4, 0, 3, 2,
                2, 4, 4
            },
            {
                0, 2, 0, 1, 0, 0, 0, 0, 0, 1,
                2, 1, 3, 0, 0, 0, 0, 0, 2, 0,
                1, 2, 1, 0, 0, 1, 0, 0, 0, 0,
                3, 0, 0, 1, 0, 1, 1, 3, 1, 0,
                0, 0, 1, 1, 0, 1, 1, 0, 0, 0,
                0, 0, 2, 0, 0, 0, 0, 0, 0, 0,
                0, 1, 1, 2, 2, 0, 3, 4, 0, 0,
                0, 1, 1, 0, 0, 1, 0, 0, 0, 0,
                0, 1, 1
            },
            {
                0, 1, 0, 0, 0, 1, 0, 0, 0, 0,
                4, 0, 4, 1, 4, 0, 3, 0, 4, 0,
                3, 0, 4, 0, 3, 0, 3, 0, 4, 1,
                5, 1, 4, 0, 0, 3, 0, 5, 0, 5,
                2, 0, 1, 0, 0, 0, 2, 1, 4, 0,
                1, 3, 0, 0, 3, 0, 0, 3, 1, 1,
                4, 1, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 1, 0, 0, 0, 0, 0,
                0, 0, 0
            },
            {
                1, 4, 0, 5, 0, 3, 0, 2, 0, 3,
                5, 4, 4, 3, 4, 3, 5, 3, 4, 3,
                3, 0, 4, 3, 3, 3, 3, 3, 3, 2,
                4, 4, 3, 1, 3, 4, 4, 5, 4, 4,
                3, 4, 4, 1, 3, 5, 4, 3, 3, 3,
                1, 2, 2, 3, 3, 1, 3, 1, 3, 3,
                3, 5, 3, 3, 4, 5, 0, 3, 0, 3,
                0, 3, 4, 3, 4, 4, 3, 0, 3, 0,
                2, 4, 3
            },
            {
                0, 1, 0, 4, 0, 0, 0, 0, 0, 1,
                4, 0, 4, 1, 4, 2, 4, 0, 3, 0,
                1, 0, 1, 0, 0, 0, 0, 0, 2, 0,
                3, 1, 1, 1, 0, 3, 0, 0, 0, 1,
                2, 1, 0, 0, 1, 1, 1, 1, 0, 1,
                0, 0, 0, 1, 0, 0, 3, 0, 0, 0,
                0, 3, 2, 0, 2, 2, 0, 1, 0, 0,
                0, 2, 3, 2, 3, 3, 0, 0, 0, 0,
                2, 1, 0
            },
            {
                0, 5, 1, 5, 0, 3, 0, 3, 0, 5,
                4, 4, 5, 1, 5, 3, 3, 0, 4, 3,
                4, 3, 5, 3, 4, 3, 3, 2, 4, 3,
                4, 3, 3, 0, 3, 3, 1, 4, 4, 3,
                4, 4, 4, 3, 4, 5, 5, 3, 2, 3,
                1, 1, 3, 3, 1, 3, 1, 1, 3, 3,
                2, 4, 5, 3, 3, 5, 0, 4, 0, 3,
                0, 4, 4, 3, 5, 3, 3, 0, 3, 4,
                0, 4, 3
            },
            {
                0, 5, 0, 5, 0, 3, 0, 2, 0, 4,
                4, 3, 5, 2, 4, 3, 3, 3, 4, 4,
                4, 3, 5, 3, 5, 3, 3, 1, 4, 0,
                4, 3, 3, 0, 3, 3, 0, 4, 4, 4,
                4, 5, 4, 3, 3, 5, 5, 3, 2, 3,
                1, 2, 3, 2, 0, 1, 0, 0, 3, 2,
                2, 4, 4, 3, 1, 5, 0, 4, 0, 3,
                0, 4, 3, 1, 3, 2, 1, 0, 3, 3,
                0, 3, 3
            },
            {
                0, 4, 0, 5, 0, 5, 0, 4, 0, 4,
                5, 5, 5, 3, 4, 3, 3, 2, 5, 4,
                4, 3, 5, 3, 5, 3, 4, 0, 4, 3,
                4, 4, 3, 2, 4, 4, 3, 4, 5, 4,
                4, 5, 5, 0, 3, 5, 5, 4, 1, 3,
                3, 2, 3, 3, 1, 3, 1, 0, 4, 3,
                1, 4, 4, 3, 4, 5, 0, 4, 0, 2,
                0, 4, 3, 4, 4, 3, 3, 0, 4, 0,
                0, 5, 5
            },
            {
                0, 4, 0, 4, 0, 5, 0, 1, 1, 3,
                3, 4, 4, 3, 4, 1, 3, 0, 5, 1,
                3, 0, 3, 1, 3, 1, 1, 0, 3, 0,
                3, 3, 4, 0, 4, 3, 0, 4, 4, 4,
                3, 4, 4, 0, 3, 5, 4, 1, 0, 3,
                0, 0, 2, 3, 0, 3, 1, 0, 3, 1,
                0, 3, 2, 1, 3, 5, 0, 3, 0, 1,
                0, 3, 2, 3, 3, 4, 4, 0, 2, 2,
                0, 4, 4
            },
            {
                2, 4, 0, 5, 0, 4, 0, 3, 0, 4,
                5, 5, 4, 3, 5, 3, 5, 3, 5, 3,
                5, 2, 5, 3, 4, 3, 3, 4, 3, 4,
                5, 3, 2, 1, 5, 4, 3, 2, 3, 4,
                5, 3, 4, 1, 2, 5, 4, 3, 0, 3,
                3, 0, 3, 2, 0, 2, 3, 0, 4, 1,
                0, 3, 4, 3, 3, 5, 0, 3, 0, 1,
                0, 4, 5, 5, 5, 4, 3, 0, 4, 2,
                0, 3, 5
            },
            {
                0, 5, 0, 4, 0, 4, 0, 2, 0, 5,
                4, 3, 4, 3, 4, 3, 3, 3, 4, 3,
                4, 2, 5, 3, 5, 3, 4, 1, 4, 3,
                4, 4, 4, 0, 3, 5, 0, 4, 4, 4,
                4, 5, 3, 1, 3, 4, 5, 3, 3, 3,
                3, 3, 3, 3, 0, 2, 2, 0, 3, 3,
                2, 4, 3, 3, 3, 5, 3, 4, 1, 3,
                3, 5, 3, 2, 0, 0, 0, 0, 4, 3,
                1, 3, 3
            },
            {
                0, 1, 0, 3, 0, 3, 0, 1, 0, 1,
                3, 3, 3, 2, 3, 3, 3, 0, 3, 0,
                0, 0, 3, 1, 3, 0, 0, 0, 2, 2,
                2, 3, 0, 0, 3, 2, 0, 1, 2, 4,
                1, 3, 3, 0, 0, 3, 3, 3, 0, 1,
                0, 0, 2, 1, 0, 0, 3, 0, 3, 1,
                0, 3, 0, 0, 1, 3, 0, 2, 0, 1,
                0, 3, 3, 1, 3, 3, 0, 0, 1, 1,
                0, 3, 3
            },
            {
                0, 2, 0, 3, 0, 2, 1, 4, 0, 2,
                2, 3, 1, 1, 3, 1, 1, 0, 2, 0,
                3, 1, 2, 3, 1, 3, 0, 0, 1, 0,
                4, 3, 2, 3, 3, 3, 1, 4, 2, 3,
                3, 3, 3, 1, 0, 3, 1, 4, 0, 1,
                1, 0, 1, 2, 0, 1, 1, 0, 1, 1,
                0, 3, 1, 3, 2, 2, 0, 1, 0, 0,
                0, 2, 3, 3, 3, 1, 0, 0, 0, 0,
                0, 2, 3
            },
            {
                0, 5, 0, 4, 0, 5, 0, 2, 0, 4,
                5, 5, 3, 3, 4, 3, 3, 1, 5, 4,
                4, 2, 4, 4, 4, 3, 4, 2, 4, 3,
                5, 5, 4, 3, 3, 4, 3, 3, 5, 5,
                4, 5, 5, 1, 3, 4, 5, 3, 1, 4,
                3, 1, 3, 3, 0, 3, 3, 1, 4, 3,
                1, 4, 5, 3, 3, 5, 0, 4, 0, 3,
                0, 5, 3, 3, 1, 4, 3, 0, 4, 0,
                1, 5, 3
            },
            {
                0, 5, 0, 5, 0, 4, 0, 2, 0, 4,
                4, 3, 4, 3, 3, 3, 3, 3, 5, 4,
                4, 4, 4, 4, 4, 5, 3, 3, 5, 2,
                4, 4, 4, 3, 4, 4, 3, 3, 4, 4,
                5, 5, 3, 3, 4, 3, 4, 3, 3, 4,
                3, 3, 3, 3, 1, 2, 2, 1, 4, 3,
                3, 5, 4, 4, 3, 4, 0, 4, 0, 3,
                0, 4, 4, 4, 4, 4, 1, 0, 4, 2,
                0, 2, 4
            },
            {
                0, 4, 0, 4, 0, 3, 0, 1, 0, 3,
                5, 2, 3, 0, 3, 0, 2, 1, 4, 2,
                3, 3, 4, 1, 4, 3, 3, 2, 4, 1,
                3, 3, 3, 0, 3, 3, 0, 0, 3, 3,
                3, 5, 3, 3, 3, 3, 3, 2, 0, 2,
                0, 0, 2, 0, 0, 2, 0, 0, 1, 0,
                0, 3, 1, 2, 2, 3, 0, 3, 0, 2,
                0, 4, 4, 3, 3, 4, 1, 0, 3, 0,
                0, 2, 4
            },
            {
                0, 0, 0, 4, 0, 0, 0, 0, 0, 0,
                1, 0, 1, 0, 2, 0, 0, 0, 0, 0,
                1, 0, 2, 0, 1, 0, 0, 0, 0, 0,
                3, 1, 3, 0, 3, 2, 0, 0, 0, 1,
                0, 3, 2, 0, 0, 2, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 3, 4, 0, 2, 0, 0, 0, 0,
                0, 0, 2
            },
            {
                0, 2, 1, 3, 0, 2, 0, 2, 0, 3,
                3, 3, 3, 1, 3, 1, 3, 3, 3, 3,
                3, 3, 4, 2, 2, 1, 2, 1, 4, 0,
                4, 3, 1, 3, 3, 3, 2, 4, 3, 5,
                4, 3, 3, 3, 3, 3, 3, 3, 0, 1,
                3, 0, 2, 0, 0, 1, 0, 0, 1, 0,
                0, 4, 2, 0, 2, 3, 0, 3, 3, 0,
                3, 3, 4, 2, 3, 1, 4, 0, 1, 2,
                0, 2, 3
            },
            {
                0, 3, 0, 3, 0, 1, 0, 3, 0, 2,
                3, 3, 3, 0, 3, 1, 2, 0, 3, 3,
                2, 3, 3, 2, 3, 2, 3, 1, 3, 0,
                4, 3, 2, 0, 3, 3, 1, 4, 3, 3,
                2, 3, 4, 3, 1, 3, 3, 1, 1, 0,
                1, 1, 0, 1, 0, 1, 0, 1, 0, 0,
                0, 4, 1, 1, 0, 3, 0, 3, 1, 0,
                2, 3, 3, 3, 3, 3, 1, 0, 0, 2,
                0, 3, 3
            },
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                3, 0, 2, 0, 3, 0, 0, 0, 0, 0,
                0, 0, 3, 0, 0, 0, 0, 0, 0, 0,
                3, 0, 3, 0, 3, 1, 0, 1, 0, 1,
                0, 0, 1, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 3, 0,
                2, 0, 2, 3, 0, 0, 0, 0, 0, 0,
                0, 0, 3
            },
            {
                0, 2, 0, 3, 1, 3, 0, 3, 0, 2,
                3, 3, 3, 1, 3, 1, 3, 1, 3, 1,
                3, 3, 3, 1, 3, 0, 2, 3, 1, 1,
                4, 3, 3, 2, 3, 3, 1, 2, 2, 4,
                1, 3, 3, 0, 1, 4, 2, 3, 0, 1,
                3, 0, 3, 0, 0, 1, 3, 0, 2, 0,
                0, 3, 3, 2, 1, 3, 0, 3, 0, 2,
                0, 3, 4, 4, 4, 3, 1, 0, 3, 0,
                0, 3, 3
            },
            {
                0, 2, 0, 1, 0, 2, 0, 0, 0, 1,
                3, 2, 2, 1, 3, 0, 1, 1, 3, 0,
                3, 2, 3, 1, 2, 0, 2, 0, 1, 1,
                3, 3, 3, 0, 3, 3, 1, 1, 2, 3,
                2, 3, 3, 1, 2, 3, 2, 0, 0, 1,
                0, 0, 0, 0, 0, 0, 3, 0, 1, 0,
                0, 2, 1, 2, 1, 3, 0, 3, 0, 0,
                0, 3, 4, 4, 4, 3, 2, 0, 2, 0,
                0, 2, 4
            },
            {
                0, 0, 0, 1, 0, 1, 0, 0, 0, 0,
                1, 0, 0, 0, 1, 0, 0, 0, 0, 0,
                0, 0, 1, 1, 1, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 2, 2, 0, 0, 0, 1,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 1, 0, 0,
                0, 0, 1, 3, 1, 0, 0, 0, 0, 0,
                0, 0, 3
            },
            {
                0, 3, 0, 3, 0, 2, 0, 3, 0, 3,
                3, 3, 2, 3, 2, 2, 2, 0, 3, 1,
                3, 3, 3, 2, 3, 3, 0, 0, 3, 0,
                3, 2, 2, 0, 2, 3, 1, 4, 3, 4,
                3, 3, 2, 3, 1, 5, 4, 4, 0, 3,
                1, 2, 1, 3, 0, 3, 1, 1, 2, 0,
                2, 3, 1, 3, 1, 3, 0, 3, 0, 1,
                0, 3, 3, 4, 4, 2, 1, 0, 2, 1,
                0, 2, 4
            },
            {
                0, 1, 0, 3, 0, 1, 0, 2, 0, 1,
                4, 2, 5, 1, 4, 0, 2, 0, 2, 1,
                3, 1, 4, 0, 2, 1, 0, 0, 2, 1,
                4, 1, 1, 0, 3, 3, 0, 5, 1, 3,
                2, 3, 3, 1, 0, 3, 2, 3, 0, 1,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0,
                0, 4, 0, 1, 0, 3, 0, 2, 0, 1,
                0, 3, 3, 3, 4, 3, 3, 0, 0, 0,
                0, 2, 3
            },
            {
                0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
                2, 0, 1, 0, 0, 0, 0, 0, 1, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                3, 0, 0, 0, 0, 0, 0, 1, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 2, 1, 0, 0, 1, 0, 0, 0,
                0, 0, 3
            },
            {
                0, 1, 0, 3, 0, 4, 0, 3, 0, 2,
                4, 3, 1, 0, 3, 2, 2, 1, 3, 1,
                2, 2, 3, 1, 1, 1, 2, 1, 3, 0,
                1, 2, 0, 1, 3, 2, 1, 3, 0, 5,
                5, 1, 0, 0, 1, 3, 2, 1, 0, 3,
                0, 0, 1, 0, 0, 0, 0, 0, 3, 4,
                0, 1, 1, 1, 3, 2, 0, 2, 0, 1,
                0, 2, 3, 3, 1, 2, 3, 0, 1, 0,
                1, 0, 4
            },
            {
                0, 0, 0, 1, 0, 3, 0, 3, 0, 2,
                2, 1, 0, 0, 4, 0, 3, 0, 3, 1,
                3, 0, 3, 0, 3, 0, 1, 0, 3, 0,
                3, 1, 3, 0, 3, 3, 0, 0, 1, 2,
                1, 1, 1, 0, 1, 2, 0, 0, 0, 1,
                0, 0, 1, 0, 0, 0, 0, 0, 0, 0,
                0, 2, 2, 1, 2, 0, 0, 2, 0, 0,
                0, 0, 2, 3, 3, 3, 3, 0, 0, 0,
                0, 1, 4
            },
            {
                0, 0, 0, 3, 0, 3, 0, 0, 0, 0,
                3, 1, 1, 0, 3, 0, 1, 0, 2, 0,
                1, 0, 0, 0, 0, 0, 0, 0, 1, 0,
                3, 0, 2, 0, 2, 3, 0, 0, 2, 2,
                3, 1, 2, 0, 0, 1, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 1, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 3, 0, 0, 2, 0, 0, 0,
                0, 2, 3
            },
            {
                2, 4, 0, 5, 0, 5, 0, 4, 0, 3,
                4, 3, 3, 3, 4, 3, 3, 3, 4, 3,
                4, 4, 5, 4, 5, 5, 5, 2, 3, 0,
                5, 5, 4, 1, 5, 4, 3, 1, 5, 4,
                3, 4, 4, 3, 3, 4, 3, 3, 0, 3,
                2, 0, 2, 3, 0, 3, 0, 0, 3, 3,
                0, 5, 3, 2, 3, 3, 0, 3, 0, 3,
                0, 3, 4, 5, 4, 5, 3, 0, 4, 3,
                0, 3, 4
            },
            {
                0, 3, 0, 3, 0, 3, 0, 3, 0, 3,
                3, 4, 3, 2, 3, 2, 3, 0, 4, 3,
                3, 3, 3, 3, 3, 3, 3, 0, 3, 2,
                4, 3, 3, 1, 3, 4, 3, 4, 4, 4,
                3, 4, 4, 3, 2, 4, 4, 1, 0, 2,
                0, 0, 1, 1, 0, 2, 0, 0, 3, 1,
                0, 5, 3, 2, 1, 3, 0, 3, 0, 1,
                2, 4, 3, 2, 4, 3, 3, 0, 3, 2,
                0, 4, 4
            },
            {
                0, 3, 0, 3, 0, 1, 0, 0, 0, 1,
                4, 3, 3, 2, 3, 1, 3, 1, 4, 2,
                3, 2, 4, 2, 3, 4, 3, 0, 2, 2,
                3, 3, 3, 0, 3, 3, 3, 0, 3, 4,
                1, 3, 3, 0, 3, 4, 3, 3, 0, 1,
                1, 0, 1, 0, 0, 0, 4, 0, 3, 0,
                0, 3, 1, 2, 1, 3, 0, 4, 0, 1,
                0, 4, 3, 3, 4, 3, 3, 0, 2, 0,
                0, 3, 3
            },
            {
                0, 3, 0, 4, 0, 1, 0, 3, 0, 3,
                4, 3, 3, 0, 3, 3, 3, 1, 3, 1,
                3, 3, 4, 3, 3, 3, 0, 0, 3, 1,
                5, 3, 3, 1, 3, 3, 2, 5, 4, 3,
                3, 4, 5, 3, 2, 5, 3, 4, 0, 1,
                0, 0, 0, 0, 0, 2, 0, 0, 1, 1,
                0, 4, 2, 2, 1, 3, 0, 3, 0, 2,
                0, 4, 4, 3, 5, 3, 2, 0, 1, 1,
                0, 3, 4
            },
            {
                0, 5, 0, 4, 0, 5, 0, 2, 0, 4,
                4, 3, 3, 2, 3, 3, 3, 1, 4, 3,
                4, 1, 5, 3, 4, 3, 4, 0, 4, 2,
                4, 3, 4, 1, 5, 4, 0, 4, 4, 4,
                4, 5, 4, 1, 3, 5, 4, 2, 1, 4,
                1, 1, 3, 2, 0, 3, 1, 0, 3, 2,
                1, 4, 3, 3, 3, 4, 0, 4, 0, 3,
                0, 4, 4, 4, 3, 3, 3, 0, 4, 2,
                0, 3, 4
            },
            {
                1, 4, 0, 4, 0, 3, 0, 1, 0, 3,
                3, 3, 1, 1, 3, 3, 2, 2, 3, 3,
                1, 0, 3, 2, 2, 1, 2, 0, 3, 1,
                2, 1, 2, 0, 3, 2, 0, 2, 2, 3,
                3, 4, 3, 0, 3, 3, 1, 2, 0, 1,
                1, 3, 1, 2, 0, 0, 3, 0, 1, 1,
                0, 3, 2, 2, 3, 3, 0, 3, 0, 0,
                0, 2, 3, 3, 4, 3, 3, 0, 1, 0,
                0, 1, 4
            },
            {
                0, 4, 0, 4, 0, 4, 0, 0, 0, 3,
                4, 4, 3, 1, 4, 2, 3, 2, 3, 3,
                3, 1, 4, 3, 4, 0, 3, 0, 4, 2,
                3, 3, 2, 2, 5, 4, 2, 1, 3, 4,
                3, 4, 3, 1, 3, 3, 4, 2, 0, 2,
                1, 0, 3, 3, 0, 0, 2, 0, 3, 1,
                0, 4, 4, 3, 4, 3, 0, 4, 0, 1,
                0, 2, 4, 4, 4, 4, 4, 0, 3, 2,
                0, 3, 3
            },
            {
                0, 0, 0, 1, 0, 4, 0, 0, 0, 0,
                0, 0, 1, 1, 1, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 1, 0, 0, 0, 0, 0,
                0, 0, 1, 0, 3, 2, 0, 0, 1, 0,
                0, 0, 1, 0, 0, 0, 0, 0, 0, 1,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0,
                0, 0, 2
            },
            {
                0, 2, 0, 3, 0, 4, 0, 4, 0, 1,
                3, 3, 3, 0, 4, 0, 2, 1, 2, 1,
                1, 1, 2, 0, 3, 1, 1, 0, 1, 0,
                3, 1, 0, 0, 3, 3, 2, 0, 1, 1,
                0, 0, 0, 0, 0, 1, 0, 2, 0, 2,
                2, 0, 3, 1, 0, 0, 1, 0, 1, 1,
                0, 1, 2, 0, 3, 0, 0, 0, 0, 1,
                0, 0, 3, 3, 4, 3, 1, 0, 1, 0,
                3, 0, 2
            },
            {
                0, 0, 0, 3, 0, 5, 0, 0, 0, 0,
                1, 0, 2, 0, 3, 1, 0, 1, 3, 0,
                0, 0, 2, 0, 0, 0, 1, 0, 0, 0,
                1, 1, 0, 0, 4, 0, 0, 0, 2, 3,
                0, 1, 4, 1, 0, 2, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 3,
                0, 0, 0, 0, 0, 1, 0, 0, 0, 0,
                0, 0, 0, 2, 0, 0, 3, 0, 0, 0,
                0, 0, 3
            },
            {
                0, 2, 0, 5, 0, 5, 0, 1, 0, 2,
                4, 3, 3, 2, 5, 1, 3, 2, 3, 3,
                3, 0, 4, 1, 2, 0, 3, 0, 4, 0,
                2, 2, 1, 1, 5, 3, 0, 0, 1, 4,
                2, 3, 2, 0, 3, 3, 3, 2, 0, 2,
                4, 1, 1, 2, 0, 1, 1, 0, 3, 1,
                0, 1, 3, 1, 2, 3, 0, 2, 0, 0,
                0, 1, 3, 5, 4, 4, 4, 0, 3, 0,
                0, 1, 3
            },
            {
                0, 4, 0, 5, 0, 4, 0, 4, 0, 4,
                5, 4, 3, 3, 4, 3, 3, 3, 4, 3,
                4, 4, 5, 3, 4, 5, 4, 2, 4, 2,
                3, 4, 3, 1, 4, 4, 1, 3, 5, 4,
                4, 5, 5, 4, 4, 5, 5, 5, 2, 3,
                3, 1, 4, 3, 1, 3, 3, 0, 3, 3,
                1, 4, 3, 4, 4, 4, 0, 3, 0, 4,
                0, 3, 3, 4, 4, 5, 0, 0, 4, 3,
                0, 4, 5
            },
            {
                0, 4, 0, 4, 0, 3, 0, 3, 0, 3,
                4, 4, 4, 3, 3, 2, 4, 3, 4, 3,
                4, 3, 5, 3, 4, 3, 2, 1, 4, 2,
                4, 4, 3, 1, 3, 4, 2, 4, 5, 5,
                3, 4, 5, 4, 1, 5, 4, 3, 0, 3,
                2, 2, 3, 2, 1, 3, 1, 0, 3, 3,
                3, 5, 3, 3, 3, 5, 4, 4, 2, 3,
                3, 4, 3, 3, 3, 2, 1, 0, 3, 2,
                1, 4, 3
            },
            {
                0, 4, 0, 5, 0, 4, 0, 3, 0, 3,
                5, 5, 3, 2, 4, 3, 4, 0, 5, 4,
                4, 1, 4, 4, 4, 3, 3, 3, 4, 3,
                5, 5, 2, 3, 3, 4, 1, 2, 5, 5,
                3, 5, 5, 2, 3, 5, 5, 4, 0, 3,
                2, 0, 3, 3, 1, 1, 5, 1, 4, 1,
                0, 4, 3, 2, 3, 5, 0, 4, 0, 3,
                0, 5, 4, 3, 4, 3, 0, 0, 4, 1,
                0, 4, 4
            },
            {
                1, 3, 0, 4, 0, 2, 0, 2, 0, 2,
                5, 5, 3, 3, 3, 3, 3, 0, 4, 2,
                3, 4, 4, 4, 3, 4, 0, 0, 3, 4,
                5, 4, 3, 3, 3, 3, 2, 5, 5, 4,
                5, 5, 5, 4, 3, 5, 5, 5, 1, 3,
                1, 0, 1, 0, 0, 3, 2, 0, 4, 2,
                0, 5, 2, 3, 2, 4, 1, 3, 0, 3,
                0, 4, 5, 4, 5, 4, 3, 0, 4, 2,
                0, 5, 4
            },
            {
                0, 3, 0, 4, 0, 5, 0, 3, 0, 3,
                4, 4, 3, 2, 3, 2, 3, 3, 3, 3,
                3, 2, 4, 3, 3, 2, 2, 0, 3, 3,
                3, 3, 3, 1, 3, 3, 3, 0, 4, 4,
                3, 4, 4, 1, 1, 4, 4, 2, 0, 3,
                1, 0, 1, 1, 0, 4, 1, 0, 2, 3,
                1, 3, 3, 1, 3, 4, 0, 3, 0, 1,
                0, 3, 1, 3, 0, 0, 1, 0, 2, 0,
                0, 4, 4
            },
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0
            },
            {
                0, 3, 0, 3, 0, 2, 0, 3, 0, 1,
                5, 4, 3, 3, 3, 1, 4, 2, 1, 2,
                3, 4, 4, 2, 4, 4, 5, 0, 3, 1,
                4, 3, 4, 0, 4, 3, 3, 3, 2, 3,
                2, 5, 3, 4, 3, 2, 2, 3, 0, 0,
                3, 0, 2, 1, 0, 1, 2, 0, 0, 0,
                0, 2, 1, 1, 3, 1, 0, 2, 0, 4,
                0, 3, 4, 4, 4, 5, 2, 0, 2, 0,
                0, 1, 3
            },
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 1, 0, 0, 0, 0, 0,
                1, 1, 1, 0, 0, 1, 1, 0, 0, 0,
                4, 2, 1, 1, 0, 1, 0, 3, 2, 0,
                0, 3, 1, 1, 1, 2, 2, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 3, 0, 1, 0, 0, 0, 2, 0, 0,
                0, 1, 4, 0, 4, 2, 1, 0, 0, 0,
                0, 0, 1
            },
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                0, 1, 0, 0, 0, 0, 1, 0, 0, 0,
                0, 0, 0, 1, 0, 1, 0, 0, 0, 0,
                3, 1, 0, 0, 0, 2, 0, 2, 1, 0,
                0, 1, 2, 1, 0, 1, 1, 0, 0, 3,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 1, 3, 1, 0, 0, 0, 0, 0, 1,
                0, 0, 2, 1, 0, 0, 0, 0, 0, 0,
                0, 0, 2
            },
            {
                0, 4, 0, 4, 0, 4, 0, 3, 0, 4,
                4, 3, 4, 2, 4, 3, 2, 0, 4, 4,
                4, 3, 5, 3, 5, 3, 3, 2, 4, 2,
                4, 3, 4, 3, 1, 4, 0, 2, 3, 4,
                4, 4, 3, 3, 3, 4, 4, 4, 3, 4,
                1, 3, 4, 3, 2, 1, 2, 1, 3, 3,
                3, 4, 4, 3, 3, 5, 0, 4, 0, 3,
                0, 4, 3, 3, 3, 2, 1, 0, 3, 0,
                0, 3, 3
            },
            {
                0, 4, 0, 3, 0, 3, 0, 3, 0, 3,
                5, 5, 3, 3, 3, 3, 4, 3, 4, 3,
                3, 3, 4, 4, 4, 3, 3, 3, 3, 4,
                3, 5, 3, 3, 1, 3, 2, 4, 5, 5,
                5, 5, 4, 3, 4, 5, 5, 3, 2, 2,
                3, 3, 3, 3, 2, 3, 3, 1, 2, 3,
                2, 4, 3, 3, 3, 4, 0, 4, 0, 2,
                0, 4, 3, 2, 2, 1, 2, 0, 3, 0,
                0, 4, 1
            }
        };

        private int[] _RelSample = new int[ 6 ];
        private int _TotalRel;
        private int _LastCharOrder;
        private int _NeedToSkipCharNum;
        private bool _Done;

        public JapaneseContextAnalyser() => Reset();

        public float GetConfidence()
        {
            if ( _TotalRel > 4 )
            {
                return (float) checked(_TotalRel - _RelSample[ 0 ]) / (float) _TotalRel;
            }
            return -1f;
        }

        public void HandleData( byte[] buf, int offset, int len )
        {
            checked
            {
                int num = offset + len;
                if ( _Done )
                {
                    return;
                }
                int num2 = _NeedToSkipCharNum + offset;
                while ( num2 < num )
                {
                    int charLen;
                    int order = GetOrder( buf, num2, out charLen );
                    num2 += charLen;
                    if ( num2 > num )
                    {
                        _NeedToSkipCharNum = num2 - num;
                        _LastCharOrder = -1;
                        continue;
                    }
                    if ( order != -1 && _LastCharOrder != -1 )
                    {
                        _TotalRel++;
                        if ( _TotalRel > 1000 )
                        {
                            _Done = true;
                            break;
                        }
                        _RelSample[ jp2CharContext[ _LastCharOrder, order ] ]++;
                    }
                    _LastCharOrder = order;
                }
            }
        }

        public void HandleOneChar( byte[] buf, int offset, int charLen )
        {
            if ( _TotalRel > 1000 )
            {
                _Done = true;
            }
            checked
            {
                if ( !_Done )
                {
                    int num = ((charLen == 2) ? GetOrder( buf, offset ) : (-1));
                    if ( num != -1 && _LastCharOrder != -1 )
                    {
                        _TotalRel++;
                        _RelSample[ jp2CharContext[ _LastCharOrder, num ] ]++;
                    }
                    _LastCharOrder = num;
                }
            }
        }

        public void Reset()
        {
            _TotalRel = 0;
            for ( int i = 0; i < 6; i++ )
            {
                _RelSample[ i ] = 0;
                _NeedToSkipCharNum = 0;
                _LastCharOrder = -1;
                _Done = false;
            }
        }

        protected abstract int GetOrder( byte[] buf, int offset, out int charLen );
        protected abstract int GetOrder( byte[] buf, int offset );
        public bool GotEnoughData() => (100 < _TotalRel);
    }
}