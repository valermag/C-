using static System.Console;
using System.Text;
using System;

namespace Lab7
{
    class RationalNum : IComparable<RationalNum>, IEquatable<RationalNum>
    {
        public int _firstNum;
        public int _secondNum;

        public RationalNum(int firstNum) : this(firstNum, 1) {}
        public RationalNum(int firstNum, int secondNum)
        {
            _firstNum = firstNum;
            _secondNum = secondNum;
        }

        public int CompareTo(RationalNum comparingNum)
        {
            if (comparingNum != null)
            {
                return _firstNum.CompareTo(comparingNum);
            }
            else
            {
                throw new Exception("Невозможно сравнить объекты");
            }
        }

        //реализация перекрытия
        public static RationalNum operator +(RationalNum a, RationalNum b)
        {
            if (a != null && b != null)
            {
                return new RationalNum(a._firstNum * b._secondNum + 
                    b._firstNum * a._secondNum, 
                    a._secondNum * b._secondNum);
            }
            else
            {
                return null;
            }
        }

        public static RationalNum operator -(RationalNum a, RationalNum b)
        {
            if (a != null && b != null)
            {
                return new RationalNum(a._firstNum * b._secondNum -
                    b._firstNum * a._secondNum, 
                    a._secondNum * b._secondNum);
            }
            else
            {
                return null;
            }
        }

        public static RationalNum operator *(RationalNum a, RationalNum b)
        {
            if (a != null && b != null)
            {
                return new RationalNum(a._firstNum * b._firstNum, a._secondNum * b._secondNum);
            }
            else
            {
                return null;
            }
        }

        public static RationalNum operator /(RationalNum a, RationalNum b)
        {
            if (a != null && b != null)
            {
                return new RationalNum(a._firstNum * b._secondNum, a._secondNum * b._firstNum);
            }
            else
            {
                return null;
            }
        }

        public static bool operator >(RationalNum a, RationalNum b)
        {
            if (a != null && b != null)
            {
                return a._firstNum * b._secondNum > b._firstNum * a._secondNum;
            }
            else
            {
                return false;
            }
        }

        public static bool operator <(RationalNum a, RationalNum b)
        {
            if (a != null && b != null)
            {
                return a._firstNum * b._secondNum < b._firstNum * a._secondNum;
            }
            else
            {
                return false;
            }
        }

        public override bool Equals(object numberToCompare)
        {
            return (numberToCompare != null && numberToCompare.GetType() == this.GetType()) ? Equals(numberToCompare) : false; 
        }

        public bool Equals(RationalNum numberToCompare)
        {
                RationalNum RatNumberToComp = (RationalNum)numberToCompare;

                return ((_firstNum == RatNumberToComp._firstNum &&
                _secondNum == RatNumberToComp._secondNum) ? true : false);
        }


        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static implicit operator RationalNum(string number)
        {
            return ObjectCreator(number);
        }

        public static explicit operator string(RationalNum number)
        {
            return ToStringConverter("Rational", number);
        }

        public static implicit operator RationalNum(int number)
        {
            return new RationalNum(number);
        }

        public static explicit operator int(RationalNum number)
        {
            return Convert.ToInt32(number._firstNum / number._secondNum);
        }

        public static implicit operator RationalNum(double doubleNumber)
        {
            return new RationalNum(Convert.ToInt32(doubleNumber * 100), 100);
        }

        public static explicit operator double(RationalNum number)
        {
            return number._firstNum / number._secondNum;
        }

        public static bool operator ==(RationalNum number1, RationalNum number2)
        {
            return number1.Equals(number2);
        }

        public static bool operator !=(RationalNum number1, RationalNum number2)
        {
            return number1.Equals(number2);
        }

        //представление в виде строки
        public static string ToStringConverter(string password, RationalNum rationNum)
        {
            if (!string.IsNullOrEmpty(password))
            {
                if (password == "Rational")
                {
                    return (rationNum._firstNum.ToString() + "/" + rationNum._secondNum.ToString());
                }
                else if (password == "Real")
                {
                    return  (rationNum._firstNum / rationNum._secondNum).ToString("0.00");
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        //из строки в объект
        public static RationalNum ObjectCreator(string rationNumStr)
        {
            if (!string.IsNullOrEmpty(rationNumStr))
            {
                if (rationNumStr.IndexOf('/') != -1)
                {
                    string[] numbers = rationNumStr.Split('/');

                    //проверка
                    for (int i = 0; i < numbers.Length; i++)
                    {
                        if(int.Parse(numbers[i]) <= 0 || string.IsNullOrEmpty(numbers[i]))
                        {
                            WriteLine("Wrong numbers entered!");
                            return null;
                        }
                    }

                    return new RationalNum(int.Parse(numbers[0]), int.Parse(numbers[1]));
                }
                else 
                {
                    double enteredNum;

                    try
                    {
                        int fractional;
                        int wholeNum;

                        enteredNum = Convert.ToDouble(rationNumStr);

                        wholeNum = Convert.ToInt32(Math.Floor(enteredNum));
                        fractional = Convert.ToInt32(Math.Round((enteredNum - wholeNum), 3) * 1000);

                        return new RationalNum((wholeNum * 1000) + fractional, 1000);
                    }
                    catch(FormatException)
                    {
                        WriteLine("Wrong number entered!");

                        return null;
                    }
                }
            }
            else
            {
                WriteLine("Wrong numbers entered!");

                return null;
            }
        }
    }
}
