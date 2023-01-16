namespace SystemModeling_CourseWork.Events;

public sealed class CreateEvent : Event
{
    public CreateEvent(string name, Queue queue, MultiChannelsProcessor multiChannelsProcessor, params IRandomValueDistribution[] randomValueDistributions) 
        : base(name, queue, multiChannelsProcessor, randomValueDistributions)
    {
        Start(0.0, CreateNewObject());
    }

    public override void Complete()
    {
        double newStartTime = CompletionTimeCached;
        base.Complete();
        Start(newStartTime, CreateNewObject());
    }

    private ServableObject CreateNewObject()
    {
        return new ServableObject(0, 0);
    }
}