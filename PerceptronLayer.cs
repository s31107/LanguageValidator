namespace LanguageValidator;

// Przerobić int na etykiety string.

public class PerceptronLayer
{
    private readonly Perceptron[] _layer;
    private readonly Random _rand;
    
    public PerceptronLayer(int dimension, int numberOfClasses, double learningRate)
    {
        _layer = new Perceptron[numberOfClasses];
        _rand = new Random();
        for (var i = 0; i < numberOfClasses; i++)
        {
            _layer[i] = new Perceptron(dimension, learningRate);
        }
    }

    public void Train(Tuple<int, double[]>[] inputs)
    {
        if (inputs.Any(item => item.Item1 >= _layer.Length || item.Item1 < 0))
        {
            throw new ArgumentException($"Class id should be between 0 and {_layer.Length}");
        }

        var shuffledElements = inputs.OrderBy(_ => _rand.Next()).ToList();

        for (var perceptronIndex = 0; perceptronIndex < _layer.Length; perceptronIndex++)
        {
            _layer[perceptronIndex].Learn(shuffledElements.Select(elem => elem.Item2).ToArray(), 
                shuffledElements.Select(elem => elem.Item1 == perceptronIndex).ToArray());
        }
    }

    public int MaximumSelector(double[] vector)
    {
        var results = _layer.Select(perceptron => perceptron.ComputeNet(vector)).ToList();
        return results.IndexOf(results.Max());
    }
}