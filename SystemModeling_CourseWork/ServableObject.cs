namespace SystemModeling_CourseWork;

public class ServableObject
{
    public int Type { get; }
    public int QueuePriority { get; }

    public ServableObject(int type, int queuePriority)
    {
        Type = type;
        QueuePriority = queuePriority;
    }

    public static ServableObject CreateDefaultObject() => new ServableObject(0, 0);
}