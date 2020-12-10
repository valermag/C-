using System.Collections.Generic;

namespace Lab7
{
    class Comparer: IComparer<RationalNum>
    {
        public int Compare(RationalNum obj1, RationalNum obj2)
        {
            if (obj1._firstNum * obj2._secondNum > obj2._firstNum * obj1._secondNum)
            {
                return 1;
            }
            else if ((obj1._firstNum == obj2._firstNum) && (obj1._secondNum == obj2._secondNum))
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }
}
