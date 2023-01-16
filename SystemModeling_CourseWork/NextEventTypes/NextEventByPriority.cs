namespace SystemModeling_CourseWork.NextEventTypes;

public class NextEventByPriority : INextEvent
{
    public List<(Event @event, int priority)> BranchedEvents;

    public NextEventByPriority(List<(Event, int)> branchedEvents)
    {
        BranchedEvents = branchedEvents;
    }

    public Event? GetNextEvent()
    {
        if (BranchedEvents.Count == 0)
            return null;

        return BranchedEvents
            .Where(tuple => !tuple.@event.Locked)
            .MaxBy(tuple => tuple.priority)
            .@event;
    }
}