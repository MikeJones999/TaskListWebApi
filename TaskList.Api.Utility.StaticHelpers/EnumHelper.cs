namespace TaskList.Api.Utility.StaticHelpers
{
    public static class EnumHelper
    {
        public static int GetMaxValue<T>() where T : Enum 
        { 
            return Enum.GetValues(typeof(T)).Cast<int>().Max();
        }
    }
}
