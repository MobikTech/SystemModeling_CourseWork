namespace SystemModeling_CourseWork;

public abstract class Event
{
    public event Action? Started;
    public event Action? Completed;
    
    public readonly string Name;
    public double StartTimeCached;
    public double CompletionTimeCached;
    public INextEvent NextEvent;
    public readonly Queue Queue;
    public readonly MultiChannelsProcessor MultiChannelsProcessor;
    public readonly Statistic Statistic;
    public bool InProcessing => MultiChannelsProcessor.CurrentActiveChannels > 0;
    public bool Locked;
    
    protected readonly IRandomValueDistribution[] _randomValueDistributions;
    private ServableObject? _currentObject;


    public Event(String name, Queue queue, MultiChannelsProcessor multiChannelsProcessor, params IRandomValueDistribution[] randomValueDistributions)
    {
        Name = name;
        Queue = queue;
        _randomValueDistributions = randomValueDistributions;
        MultiChannelsProcessor = multiChannelsProcessor;
        Statistic = new Statistic();
        CompletionTimeCached = Double.MaxValue;
    }

    public virtual void Start(double startTime, ServableObject servableObject)
    {
        Statistic.QuantityArrived++;
        if (MultiChannelsProcessor.IsFull())
        {
            if (!Queue.TryStand(servableObject, startTime)) Statistic.Failures++;
        }
        else
        {
            MultiChannelsProcessor.TryAddProcess((servableObject, startTime, GetEventCompletionTime(startTime)));
            RecalculateTime();
            Started?.Invoke();
        }
    }

    public virtual void Complete()
    {
        MultiChannelsProcessor.ClearNearestChannel();
        Statistic.QuantityCompleted++;
        Statistic.TotalProcessingTime += CompletionTimeCached - StartTimeCached;

        while (Queue.CurrentLength > 0 && !MultiChannelsProcessor.IsFull())
        {
            ServableObject nextObject = Queue.TryMoveQueue()!;
            MultiChannelsProcessor.TryAddProcess((nextObject, CompletionTimeCached, GetEventCompletionTime(CompletionTimeCached)));
        }
        Completed?.Invoke();

        Event? nextEvent = NextEvent.GetNextEvent();
        nextEvent?.Start(CompletionTimeCached, _currentObject);
        
        RecalculateTime();
    }

    public void MoveUncompletedToLinkedQueue()
    {
        Queue.LinkedQueue?.TryStand(_currentObject, StartTimeCached);
        if (MultiChannelsProcessor.CurrentActiveChannels > 0)
        {
            MultiChannelsProcessor.ClearNearestChannel();
            Statistic.QuantitySwitched++;
        }

        _currentObject = null;
        StartTimeCached = Double.MaxValue;
        CompletionTimeCached = Double.MaxValue;
    }

    public virtual void PrintStatistic()
    {
        Console.WriteLine($"MeanProcessingTime:{Statistic.GetMeanProcessingTime()}");
    }

    public void UpdateStatistic(double timeDelta)
    {
        Statistic.TotalQueue += Queue.CurrentLength * timeDelta;
        if (MultiChannelsProcessor.CurrentActiveChannels > 0) Statistic.TotalLoad += timeDelta;
    }


    protected bool RecalculateTime()
    {
        (ServableObject, Double, Double)? nextChannelInfo = MultiChannelsProcessor.GetNearestChannelInfoOrNull();
        if (!nextChannelInfo.HasValue)
        {
            _currentObject = null;
            StartTimeCached = Double.MaxValue;
            CompletionTimeCached = Double.MaxValue;
            return false;
        }

        _currentObject = nextChannelInfo.Value.Item1;
        StartTimeCached = nextChannelInfo.Value.Item2;
        CompletionTimeCached = nextChannelInfo.Value.Item3;
        return true;
    }

    private double GetEventCompletionTime(double startTime)
    {
        return startTime + _randomValueDistributions.Sum(randomValueDistribution => randomValueDistribution.GetRandomValue());
    }
}