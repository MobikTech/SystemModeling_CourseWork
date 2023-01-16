namespace SystemModeling_CourseWork;

public class Statistic
{
    public int QuantityArrived;
    public int QuantityCompleted;
    public int Failures;
    public double TotalQueue;
    public double TotalLoad;
    public double TotalProcessingTime;

    public static int QuantitySwitched;
    public double GetFailuresProbability() => (double)Failures / QuantityArrived;

    public double GetMeanProcessingTime() => TotalProcessingTime / QuantityCompleted;
}