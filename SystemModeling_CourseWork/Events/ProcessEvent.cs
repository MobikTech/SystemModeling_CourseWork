namespace SystemModeling_CourseWork.Events;

public sealed class ProcessEvent : Event
{
    public ProcessEvent(string name, Queue queue, MultiChannelsProcessor multiChannelsProcessor, params IRandomValueDistribution[] randomValueDistributions) 
        : base(name, queue, multiChannelsProcessor, randomValueDistributions)
    {
        if (Name is "CRASH" or "REPAIR_INTERRUPT")
        {
            Start(0.0, ServableObject.CreateDefaultObject());
        }
    }
}