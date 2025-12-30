namespace Menu4Tech.Helper;

public static class Extensions
{
    public static int ToInt(this Enum @enum) => Convert.ToInt32(@enum);
}