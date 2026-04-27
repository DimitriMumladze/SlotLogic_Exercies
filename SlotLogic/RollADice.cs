namespace SlotLogic;

public static class RollADice
{
    static Random random = new Random();
    public static int RandomInteger()
    {
        int randomNUmber = random.Next(1, 7);

        return randomNUmber;
    }

    public static int[] RollTenK()
    {
        int[] randomTenKNumberArr = new int[10000];

        for ( int i = 0; i < 10000; i++ )
        {
            randomTenKNumberArr[i] = RandomInteger();
        }

        return randomTenKNumberArr;
    }
}
