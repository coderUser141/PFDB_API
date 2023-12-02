using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

class FileParseAsync
{

    public static Task<List<int>> indexFinderAsync(string filetext, string word)
    {
        List<int> output = new();
        for (; filetext.Contains(word, StringComparison.CurrentCultureIgnoreCase);)
        {
            output.Add(filetext.LastIndexOf(word, StringComparison.CurrentCultureIgnoreCase));
            try
            {
                filetext = filetext.Remove(filetext.LastIndexOf(word, StringComparison.CurrentCultureIgnoreCase), word.Length);
            }
            catch (ArgumentOutOfRangeException) { break; }
        }
        return Task.FromResult(output);
    }


    public static Task<List<int>> indexFinderAsync(string filetext, char letter)
    {
        List<int> output = new();
        for (; filetext.Contains(letter, StringComparison.CurrentCultureIgnoreCase);)
        {
            if (filetext.LastIndexOf(letter) == -1) break;
            output.Add(filetext.LastIndexOf(letter));

            try { filetext = filetext.Remove(filetext.LastIndexOf(letter)); }
            catch (ArgumentOutOfRangeException) { break; } //catches filetext.Remove(-1) -> ArgumentOutOfRangeException
        }
        return Task.FromResult(output);
    }

    public static Task<bool> fileVerifierAsync(string filepath)
    {
        if (filepath == null) throw new ArgumentNullException(nameof(filepath), "File path specified cannot be null.");
        if (!File.Exists(filepath)) throw new FileNotFoundException($"File not found.", filepath);
        return Task.FromResult(true);
    }



    public static Task<string> fileReaderAsync(string filepath)
    {
        if (filepath == null) throw new ArgumentNullException(nameof(filepath), "File path specified cannot be null.");
        if (!File.Exists(filepath)) throw new FileNotFoundException($"File not found.", filepath);
        return File.ReadAllTextAsync(filepath);
    }

}
