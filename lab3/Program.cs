using static System.Console;
using System;
//using System.Object;

namespace Lab7
{
    class Program
    {
        static void Main(string[] args)
        {
            RationalNum[] rationNumbers = null;

            RationNumberArrFiller(ref rationNumbers);

            DataOutput(rationNumbers);

            TwoNumsComparing(rationNumbers);
        }


        public static void RationNumberArrFiller(ref RationalNum[] rationNumbers)
        {
            string numberQuantityStr;

            //задание количества рационалных чисел
            WriteLine("How many numbers do you whant to create?");
            numberQuantityStr = ReadLine();
            int.TryParse((((string.IsNullOrEmpty(numberQuantityStr)) || (int.Parse(numberQuantityStr) <= 0)) ?
                "0" : numberQuantityStr), out int numbersQuantity);

            if (numbersQuantity <= 0)
            {
                Write("wrong number entered");
                return;
            }

            rationNumbers = new RationalNum[numbersQuantity];

            //заполнение массива рац. чисел
            for (int i = 0; i < rationNumbers.Length; i++)
            {
                string entringNumStr;

                int numerator;
                int denumerator;

                for (; ;)
                {
                    //вводим проверяем числитель
                    WriteLine("Enter numerator please");
                    entringNumStr = ReadLine();
                    if ((string.IsNullOrEmpty(entringNumStr)) || (int.Parse(entringNumStr) < 0))
                    {
                        WriteLine("Wrong value was entered, try again please!");
                    }
                    else
                    {
                        numerator = int.Parse(entringNumStr);
                        break;
                    }
                }

                for (; ;)
                {
                    //знаменатель
                    WriteLine("Enter denumerator please");
                    entringNumStr = ReadLine();
                    if ((string.IsNullOrEmpty(entringNumStr)) || (int.Parse(entringNumStr) < 0))
                    {
                        WriteLine("Wrong value was entered, try again please!");
                    }
                    else
                    {
                        denumerator = int.Parse(entringNumStr);
                        break;
                    }
                }

                rationNumbers[i] = new RationalNum(numerator, denumerator);
            }
        }

        //функция вывода всех имеющихся чисел
        public static void DataOutput(RationalNum[] rationNumbers)
        {
            for (int i = 0; i < rationNumbers.Length; i++)
            {
                Write($"{i + 1})");

                //вывод как рационального
                WriteLine(rationNumbers[i]._firstNum.ToString() + "/" + rationNumbers[i]._secondNum.ToString());

                //вывод как вещественного
                WriteLine((rationNumbers[i]._firstNum / rationNumbers[i]._secondNum).ToString());

                WriteLine("");
            }
        }


        //функция, реализующая сравнение
        public static void TwoNumsComparing(RationalNum[] rationNumbers)
        {
            string numToCompStr;

            int firstNumToComp = 0;
            int secondNumToComp = 0;


            WriteLine("What numbers you whant to compare? Enter its count numbersat array");

            for (; ;)
            {
                WriteLine("Enter first number to compare");
                numToCompStr = ReadLine();
                if ((string.IsNullOrEmpty(numToCompStr)) || (int.Parse(numToCompStr) < 0)
                    || int.Parse(numToCompStr) > rationNumbers.Length)
                {
                    WriteLine("Wrong value was entered, try again please!");
                }
                else
                {
                    firstNumToComp = int.Parse(numToCompStr);
                }

                WriteLine("Enter second number to compare");
                numToCompStr = ReadLine();
                if ((string.IsNullOrEmpty(numToCompStr)) || (int.Parse(numToCompStr) < 0)
                    || int.Parse(numToCompStr) > rationNumbers.Length)
                {
                    WriteLine("Wrong value was entered, try again please!");
                }
                else
                {
                    secondNumToComp = int.Parse(numToCompStr);
                    break;
                }
            }

            WriteLine(rationNumbers[firstNumToComp] == rationNumbers[secondNumToComp]);

            //метод с компаратором
            Comparer objComp = new Comparer();
            if (objComp.Compare(rationNumbers[firstNumToComp], rationNumbers[secondNumToComp]) == 0)
            {
                WriteLine("These numbers are equal!");
            }
            else if(objComp.Compare(rationNumbers[firstNumToComp], rationNumbers[secondNumToComp]) == 1)
            {
                WriteLine("The first number is bigger than second one");
            }
            else
            {
                WriteLine("The second number is bigger than first one");
            }
        }
    }
}
