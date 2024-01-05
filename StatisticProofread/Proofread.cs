using System;
using System.Text.RegularExpressions;

public class Proofread
{
    public static void Main(string[] args)
    {
        return;
    }

    public static Match regex(string text, string pattern)
    {
        Regex regex = new Regex(@pattern);
        return regex.Match(text);
    }

    public static MatchCollection regexes(string text, string pattern)
    {
        Regex regex = new Regex(@pattern);
        return regex.Matches(text);
    }
}