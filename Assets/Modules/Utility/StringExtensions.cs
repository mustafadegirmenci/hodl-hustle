using System.Text;

namespace SunkCost.HH.Modules.Utility
{
    public static class StringExtensions
    {
        public static string SeparatePascalCaseWithSpaces(this string input)
        {
            var result = new StringBuilder();

            for (var i = 0; i < input.Length; i++)
            {
                var currentChar = input[i];

                if (i > 0 && char.IsUpper(currentChar))
                {
                    result.Append(' ');
                }

                result.Append(currentChar);
            }

            return result.ToString();
        }
    }
}
