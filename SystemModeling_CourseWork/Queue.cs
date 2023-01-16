namespace SystemModeling_CourseWork;

public class Queue
{
    public Queue(int maxLength)
    {
        MaxLength = maxLength;
    }

    public Queue? LinkedQueue;
    public readonly int MaxLength;
    public int CurrentLength => _queue.Count;
    private List<(ServableObject servableObject, double stnandTime)> _queue = new();

    public bool TryStand(ServableObject servableObject, double standTime)
    {
        if (_queue.Count >= MaxLength) return false;
        _queue.Add((servableObject, standTime));
        SortQueueByStanding();
        return true;
    }

    public ServableObject? TryMoveQueue()
    {
        if (_queue.Count <= 0) return null;
        
        var firstObject = _queue.First();
        _queue.RemoveAt(0);
        return firstObject.servableObject;
    }

    public bool IsFull()
    {
        return CurrentLength == MaxLength;
    }

    public void MoveToLinkedQueue()
    {
        if (CurrentLength > 3)
        {
            foreach (var tuple in _queue.SkipLast(3))
            {
                if (!LinkedQueue.TryStand(tuple.servableObject, tuple.stnandTime))
                    throw new Exception("Cannot stand to Queue");
                Statistic.QuantitySwitched++;
            }
            _queue = _queue.TakeLast(3).ToList();
            SortQueueByStanding();
        }
    }

    private void SortQueueByStanding()
    {
        _queue = _queue.OrderByDescending(tuple => tuple.stnandTime).ToList();
    }
}