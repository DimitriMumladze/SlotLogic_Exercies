namespace SlotLogic;

public static class RollADice
{
    static Random random = new Random();

    public static Dictionary<int, int> DictSeeder()
    {
        var dictonary = new Dictionary<int, int>()
        {
            {1, 0},
            {2, 0},
            {3, 0},
            {4, 0},
            {5, 0},
            {6, 0}
        };

        return dictonary;
    }

    public static int RandomInteger()
    {
        int randomNUmber = random.Next(1, 7);

        return randomNUmber;
    }

    public static int[] RollAndFill (int amount)
    {
        int[] randomNumberArr = new int[amount];

        for ( int i = 0; i < amount; i++ )
        {
            randomNumberArr[i] = RandomInteger();
        }

        return randomNumberArr;
    }

    public static Dictionary<int, int> CountDiceRolls(int amount)
    {
        int[] randomNumberArr = RollAndFill(amount);
        var dictonary = DictSeeder();

        foreach (var number in randomNumberArr)
        {
            dictonary[number]++;
        }

        return dictonary;
    }

    public static void PrintResults(int amount)
    {
        var dictonary = CountDiceRolls(amount);
        foreach (var item in dictonary)
        {
            Console.WriteLine($"Number {item.Key} was rolled {item.Value} times.");
        }
    }
}
