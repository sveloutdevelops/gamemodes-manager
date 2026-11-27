namespace GamemodesManager.Gamemodes.ClassicDeathmatchMode.Extensions
{
    internal static class StringExtensions
    {
        internal static int SpacesCount(this string line)
        {
            var symbols = line.ToCharArray();
            if (!symbols.Contains(' ')) return 0;
            int count = 0;
            foreach (char symbol in symbols)
            {
                if (symbol == ' ') count++;
            }
            return count;
        }

        internal static string Truncate(this string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
        }
    }
}