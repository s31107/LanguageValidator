using System.Globalization;
using System.Numerics;

namespace LanguageValidator;

public class LanguageValidator
{
    private readonly Dictionary<string, double[]> languageToProportions;
    private const int AlphabetSize = 26;
    private const char LetterStart = 'a';

    public LanguageValidator(string languagesDirectory)
    {
        languageToProportions = new Dictionary<string, double[]>();
        ParseData(languagesDirectory);
    }
    
    private void ParseData(string directory)
    {
        Directory.EnumerateDirectories(directory).ToList().ForEach(dir =>
        {
            Directory.EnumerateDirectories(dir).ToList().ForEach(languageDir =>
            {
                var summedLetters = Directory.EnumerateFiles(languageDir).ToList().Select(
                    CountLetters).Aggregate((total, item) =>
                {
                    for (var iter = 0; iter < total.Length; ++iter)
                    {
                        total[iter] += item[iter];
                    } return total;
                });
                var totalLetters = summedLetters.Aggregate((total, item) => total + item);
                languageToProportions.Add(languageDir, summedLetters.Select(
                    item => item / totalLetters).ToArray());
            });
        });
    }
    private static double[] CountLetters(string path)
    {
        var allChars = new double[AlphabetSize];
        using var reader = new StreamReader(path);
        while (reader.ReadLine() is { } line)
        {
            foreach (var c in line.ToLower().Where(c => c >= LetterStart && c <= LetterStart + AlphabetSize))
            {
                ++allChars[c - LetterStart];
            }
        } return allChars;
    }
    
    static void Main(string[] args)
    {
        var learningRate = double.Parse(args[0]);
        var trainingLanguagesPath = args[1];
        if (!Directory.Exists(trainingLanguagesPath))
        {
            throw new FileNotFoundException($"Directory {trainingLanguagesPath} doesn't exist!");
        }

        var languageValidator = new LanguageValidator(trainingLanguagesPath);
        
    }
}