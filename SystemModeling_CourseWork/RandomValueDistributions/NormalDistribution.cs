namespace SystemModeling_CourseWork.RandomValueDistributions;

public class NormalDistribution : IRandomValueDistribution
{
    private readonly double _mean;
    private readonly double _deviation;
    private readonly Random _random = new Random();

    public NormalDistribution(double mean, double deviation)
    {
        _mean = mean;
        _deviation = deviation;
    }

    public double GetRandomValue()
    {
        return Extreme.Statistics.Distributions.NormalDistribution.Sample(_random, _mean, _deviation);
    }
}