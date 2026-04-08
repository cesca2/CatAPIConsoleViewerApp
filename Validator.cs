using System.Text.RegularExpressions;
public class Validator
{

    public static bool IsValidInputString(string input)
    {
        return Regex.IsMatch(input, @"^[a-zA-Z]+$");
    }

}