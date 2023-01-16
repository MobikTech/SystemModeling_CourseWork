namespace SystemModeling_CourseWork.RandomValueDistributions;

public class ExponentialDistribution : IRandomValueDistribution
{
    private readonly double _meanValue;
    private readonly Random _random = new Random();

    public ExponentialDistribution(double meanValue)
    {
        _meanValue = meanValue;
    }

    public double GetRandomValue()
    {
        return Extreme.Statistics.Distributions.ExponentialDistribution.Sample(_random, _meanValue);
    }
}