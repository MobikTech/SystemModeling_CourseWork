namespace SystemModeling_CourseWork.RandomValueDistributions;

public class ErlangDistribution : IRandomValueDistribution
{
    private readonly double _meanValue;
    private readonly int _k;
    private readonly Random _random = new Random();

    public ErlangDistribution(double meanValue, int k)
    {
        _meanValue = meanValue;
        _k = k;
    }

    public double GetRandomValue()
    {
        return Extreme.Statistics.Distributions.ErlangDistribution.Sample(_random, _k, _meanValue);
    }
}