namespace SlotLogic;

public static class _1_RollADie
{
    public static int RandomInteger()
    {
        Random random = new Random();
        int randomNUmber = random.Next(1, 7);

        return randomNUmber;
    }
}
