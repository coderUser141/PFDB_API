using System;
using System.Text.RegularExpressions;

public class Proofread
{
    public static Match regex(string text, string pattern)
    {
        Regex regex = new Regex(pattern);
        return regex.Match(text);
    }
}