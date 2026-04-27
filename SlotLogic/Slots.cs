namespace SlotLogic;

public static class Slots
{
    static Random random = new Random();
    public static string GetRandomSlot()
    {
        string[] slots = { "Cherry", "Lemon", "Orange", "Plum", "Bell", "Bar" };
        int index = random.Next(slots.Length);
        return slots[index];
    }
}
