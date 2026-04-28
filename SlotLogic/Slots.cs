namespace SlotLogic;

public static class Slots
{
    static Random random = new Random();
    public static string GetRandomSlot()
    {
        string[] slots = { "Cherry", "Lemon", "Orange", "Bar", "Seven" };
        int index = random.Next(slots.Length);
        return slots[index];
    }

    public static int GetRandomNumber(int number)
    {
        return random.Next(1, number + 1);
    }

    public static Dictionary<string, int> FillSlotData()
    {
        Dictionary<string, int> slotData = new Dictionary<string, int>()
        {
            { "Cherry", 40 },
            { "Lemon", 30 },
            { "Orange", 15 },
            { "Bar", 15 },
            { "Seven", 10 }
        };

        return slotData;
    }

    public static void Spin(int amount)
    {
        Dictionary<string, int> slotData = FillSlotData();

        for ( int i = 0; i < amount; i++  )
        {

        }
    }
}
