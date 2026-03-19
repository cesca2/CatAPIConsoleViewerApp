using System.Text.RegularExpressions;
public class Validator { 

	public static bool IsValidInputString(string Input)
    {
    return Regex.IsMatch(Input, @"^[a-zA-Z]+$");
    }
    
}