namespace SystemModeling_CourseWork;

public class MultiChannelsProcessor
{
    public MultiChannelsProcessor(int maxChannels)
    {
        MaxChannels = maxChannels;
    }

    public readonly int MaxChannels;
    public int CurrentActiveChannels => _channelsInfo.Count;
    private List<(ServableObject servableObject, Double startTime, Double completeTime)> _channelsInfo = new();

    
    public bool TryAddProcess((ServableObject, Double, Double) channelInfo)
    {
        if (_channelsInfo.Count >= MaxChannels) return false;
        _channelsInfo.Add(channelInfo);
        SortChannels();
        return true;
    }

    public void SortChannels()
    {
        _channelsInfo = _channelsInfo.OrderBy(tuple => tuple.completeTime).ToList();
    }

    public (ServableObject, Double, Double)? GetNearestChannelInfoOrNull()
    {
        if (_channelsInfo.Count > 0)
        {
            return _channelsInfo.ElementAt(0);
        }

        return null;
    }

    public void ClearNearestChannel()
    {
        _channelsInfo.RemoveAt(0);
    }

    public bool IsFull()
    {
        return _channelsInfo.Count == MaxChannels;
    }
}