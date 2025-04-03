namespace LanguageValidator;

public class PerceptronLayer
{
    private readonly Dictionary<string, Perceptron> _layer;
    private readonly Random _rand;
    
    public PerceptronLayer(int dimension, string[] labels, double learningRate)
    {
        _layer = new Dictionary<string, Perceptron>();
        _rand = new Random();
        foreach (var label in labels)
        {
            _layer.Add(label, new Perceptron(dimension, learningRate));
        }
    }

    public void NormalizeWeights()
    {
        foreach (var perceptron in _layer.Values) perceptron.NormalizeWeights();
    }
    
    public void Train(Tuple<string, double[]>[] inputs)
    {
        if (inputs.Any(item => !_layer.ContainsKey(item.Item1)))
        {
            throw new ArgumentException("Specified labels are not in Perceptron layer labels");
        }

        var shuffledElements = inputs.OrderBy(_ => _rand.Next()).ToList();
        
        foreach (var perceptron in _layer)
        {
            perceptron.Value.Learn(shuffledElements.Select(elem => elem.Item2).ToArray(), 
                shuffledElements.Select(elem => elem.Item1 == perceptron.Key).ToArray());
        }
    }

    public string? MaximumSelector(double[] vector)
    {
        var maxSelect = _layer.Select(perceptron => 
            new Tuple<string, double>(perceptron.Key, perceptron.Value.ComputeNet(vector))).MaxBy(
            t => t.Item2);
        return maxSelect?.Item1;
    }
}