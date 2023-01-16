using Extreme.Statistics.Distributions;

namespace SystemModeling_CourseWork.RandomValueDistributions;

public class UniformDistribution : IRandomValueDistribution
{
    private readonly double _min;
    private readonly double _max;
    private readonly Random _random = new Random();

    public UniformDistribution(double min, double max)
    {
        _min = min;
        _max = max;
    }

    public double GetRandomValue()
    {
        return ContinuousUniformDistribution.Sample(_random, _min, _max);
    }
}