namespace GeolocationUnitTest.Function
{
    public static class Helper
    {
        public static string DebugText(string input, string result, string expected)
        {
            return $"input: {input} \nresult:  {result} \nexpected: {expected}";
        }

        public static string DebugText(string input, string result, bool expected)
        {
            return $"input: {input} \nresult:  {result} \nexpected: {expected}";
        }
    }
}