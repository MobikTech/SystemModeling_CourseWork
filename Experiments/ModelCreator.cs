using SystemModeling_CourseWork.Events;
using SystemModeling_CourseWork.NextEventTypes;
using SystemModeling_CourseWork.RandomValueDistributions;

namespace SystemModeling_CourseWork;

public class ModelCreator
{
    public Model Create(double createInterval, double crashInterval, double processingDuration)
    {
        List<Event> events = CreateEvents(createInterval, crashInterval, processingDuration);
        return new Model(events);
    }
    
    List<Event> CreateEvents(double createInterval, double crashInterval, double processingDuration)
{
    // Створення СМО
    CreateEvent createEvent = new CreateEvent("CREATOR",
        new Queue(0),
        new MultiChannelsProcessor(1),
        new ExponentialDistribution(createInterval));
    
    ProcessEvent mainMachine = new ProcessEvent("MACHINE",
        new Queue(Int32.MaxValue),
        new MultiChannelsProcessor(1),
        new NormalDistribution(processingDuration, processingDuration / 5), new UniformDistribution(0.2, 0.5));
    
    ProcessEvent subMachine = new ProcessEvent("SUB_MACHINE",
        new Queue(Int32.MaxValue),
        new MultiChannelsProcessor(1),
        new NormalDistribution(0.5, 0.1), new UniformDistribution(0.2, 0.5));
   
    
    ProcessEvent crash = new ProcessEvent("CRASH",
        new Queue(0),
        new MultiChannelsProcessor(1),
        new NormalDistribution(crashInterval, crashInterval / 10));
    
    ProcessEvent sparesReceiving = new ProcessEvent("SPARES_RECEIVING",
        new Queue(0),
        new MultiChannelsProcessor(1),
        new ExponentialDistribution(3));
    
    ProcessEvent repair = new ProcessEvent("REPAIR",
        new Queue(0),
        new MultiChannelsProcessor(1),
        new ErlangDistribution(0.75, 3));
    
    ProcessEvent repairInterrupt = new ProcessEvent("REPAIR_INTERRUPT",
        new Queue(0),
        new MultiChannelsProcessor(1),
        new ExponentialDistribution(100));
    
    // Налаштування маршрутів та розгалужень
    createEvent.NextEvent = new NextEventByPriority(new List<(Event, int)>
    {
        (mainMachine, 1),
        (subMachine, 0),
    });
    mainMachine.NextEvent = new NextEventByPriority(new List<(Event, int)>());
    subMachine.NextEvent = new NextEventByPriority(new List<(Event, int)>());
    crash.NextEvent = new NextEventByPriority(new List<(Event, int)>
    {
        (sparesReceiving, 1),
        (repair, 0),
    });
    sparesReceiving.NextEvent = new NextEventByPriority(new List<(Event, int)>{(repair, 0)});
    repair.NextEvent = new NextEventByPriority(new List<(Event, int)>{(crash, 0)});
    repairInterrupt.NextEvent = new NextEventByPriority(new List<(Event, int)>{(repairInterrupt, 0)});

    // Налаштування виклику потрібних дій при блокуваннях та переходів між чергами
    mainMachine.Queue.LinkedQueue = subMachine.Queue;
    subMachine.Locked = true;
    sparesReceiving.Locked = true;
    crash.Completed += () =>
    {
        mainMachine.Locked = true;
        subMachine.Locked = false;
        mainMachine.Queue.MoveToLinkedQueue();
        mainMachine.MoveUncompletedToLinkedQueue();
    };
    repair.Completed += () =>
    {
        mainMachine.Locked = false;
        subMachine.Locked = true;
    };
    repairInterrupt.Completed += () => sparesReceiving.Locked = false;
    sparesReceiving.Completed += () => sparesReceiving.Locked = true;

    // Вертаємо список всіх об'єктів подій системи
    return new List<Event>
    {
        createEvent, mainMachine, subMachine, crash, sparesReceiving, repair, repairInterrupt
    };
}
}