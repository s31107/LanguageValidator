namespace LanguageValidator;

public class LanguageValidator
{
    private const int AlphabetSize = 26;
    private const char LetterStart = 'a';
    private readonly PerceptronLayer _perceptronLayer;

    private LanguageValidator(string languagesDirectory, double learningRate)
    {
        var trainingData = ParseData(languagesDirectory);
        _perceptronLayer = new PerceptronLayer(
            AlphabetSize, trainingData.Select(
                item => item.Item1).Distinct().ToArray(), learningRate);
        _perceptronLayer.Train(trainingData);
        _perceptronLayer.NormalizeWeights();
    }

    private string? Compute(double[] vector) => _perceptronLayer.MaximumSelector(vector);
    
    private static Tuple<string, double[]>[] ParseData(string directory)
    {
        return Directory.EnumerateDirectories(directory).ToList().SelectMany(languageDir =>
        {
            return Directory.EnumerateFiles(languageDir).ToList().Select(
                CountLettersFromFile).Select(sumVector =>
            {
                var sum = sumVector.Sum();
                return sumVector.Select(stat => stat / sum).ToArray();
            }).Select(vector => new Tuple<string, double[]>(Path.GetFileName(languageDir), vector));
        }).ToArray();
    }
    private static double[] CountLettersFromFile(string path)
    {
        var allChars = new double[AlphabetSize];
        using var reader = new StreamReader(path);
        while (reader.ReadLine() is { } line)
        {
            var iter = 0;
            foreach (var item in CountLetters(line)) allChars[iter++] += item;
        } return allChars;
    }

    private static double[] CountLetters(string text)
    {
        var chars = new double[AlphabetSize];
        foreach (var c in text.ToLower().Where(c => c >= LetterStart && c < LetterStart + AlphabetSize))
        {
            ++chars[c - LetterStart];
        } return chars;
    }
    
    public static void Main(string[] args)
    {
        var learningRate = double.Parse(args[0]);
        var trainingLanguagesPath = args[1];
        if (!Directory.Exists(trainingLanguagesPath))
        {
            throw new FileNotFoundException($"Directory {trainingLanguagesPath} doesn't exist!");
        }
        
        var languageValidator = new LanguageValidator(trainingLanguagesPath, learningRate);
        while (true)
        {
            Console.Write(">>> ");
            var line = Console.ReadLine();
            if (line == null) continue;
            Console.WriteLine(languageValidator.Compute(CountLetters(line)));
        }
    }
}