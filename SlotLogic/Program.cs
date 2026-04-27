using System;

namespace SlotLogic
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int i = 10000;
            int[] randomNumbers = RollADice.RollTenK();
            foreach (int number in randomNumbers)
                Console.WriteLine(number);
        }
    }
}