namespace SystemModeling_CourseWork
{
    public class Model
    {
        public Model(List<Event> events)
        {
            _events = events;
        }

        private readonly List<Event> _events;

        public void StartSimulation(double simulationTime)
        {
            double currentTime = 0.0;

            while (currentTime < simulationTime)
            {
                Event nearestEvent = FindNearestEvent();
                double completionTime = nearestEvent.CompletionTimeCached;

                double timeDelta = completionTime - currentTime;
                _events.ForEach(@event => @event.UpdateStatistic(timeDelta));

                PrintEventInfo(nearestEvent);
                nearestEvent.Complete();

                currentTime = completionTime;
            }

            PrintResults(simulationTime);
        }
    
        public (double meanLoad, double meanProcessingTime) StartSimulationWithoutLogs(double simulationTime)
        {
            double currentTime = 0.0;

            while (currentTime < simulationTime)
            {
                Event nearestEvent = FindNearestEvent();
                double completionTime = nearestEvent.CompletionTimeCached;

                double timeDelta = completionTime - currentTime;
                _events.ForEach(@event => @event.UpdateStatistic(timeDelta));

                nearestEvent.Complete();

                currentTime = completionTime;
            }

            var machine = GetEvent("MACHINE");
            return (Math.Clamp(machine.Statistic.TotalLoad / simulationTime, 0, 1), machine.Statistic.GetMeanProcessingTime());
        }

        private void PrintResults(double simulationTime)
        {
            Console.WriteLine("---------------RESULTS----------------");
            foreach (Event @event in _events)
            {
                Console.WriteLine($"Name:{@event.Name}, Failures:{@event.Statistic.Failures} " +
                                  $"Arrived:{@event.Statistic.QuantityArrived}, Completed:{@event.Statistic.QuantityCompleted}");
                Console.WriteLine($"FailureProbability:{@event.Statistic.GetFailuresProbability()}");
                Console.WriteLine($"MeanQueue:{@event.Statistic.TotalQueue / simulationTime}");
                Console.WriteLine($"MeanLoad:{Math.Clamp(@event.Statistic.TotalLoad / simulationTime, 0, 1)}");
                @event.PrintStatistic();
                Console.WriteLine("\n");
            }

            Console.WriteLine($"SwitchedQuantity:{Statistic.QuantitySwitched}");
            Console.WriteLine($"SwitchingPercent:{(float)Statistic.QuantitySwitched / GetEvent("MACHINE").Statistic.QuantityArrived}");
        }

        private void PrintEventInfo(Event @event)
        {
            Console.WriteLine("-----------------------");
            Console.WriteLine($"CurrentTime={@event.CompletionTimeCached}");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{@event.Name} completed during ({@event.StartTimeCached}, {@event.CompletionTimeCached})");
            Console.ResetColor();   
        
            Event machine = GetEvent("MACHINE");
            Event subMachine = GetEvent("SUB_MACHINE");
        
            Console.ForegroundColor = machine.InProcessing ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine($"MACHINE[queue={machine.Queue.CurrentLength}; Active:{machine.InProcessing}; Time:({machine.StartTimeCached}, " +
                              $"{machine.CompletionTimeCached}); Arrived:{machine.Statistic.QuantityArrived}; Completed:{machine.Statistic.QuantityCompleted}]");
            Console.ResetColor();
       
            Console.ForegroundColor = subMachine.InProcessing ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine($"SUB_MACHINE[queue={subMachine.Queue.CurrentLength}; Active:{subMachine.InProcessing}; Time:({subMachine.StartTimeCached}, " +
                              $"{subMachine.CompletionTimeCached}); Arrived:{subMachine.Statistic.QuantityArrived}; Completed:{subMachine.Statistic.QuantityCompleted}]");
            Console.ResetColor();
        }

        private Event FindNearestEvent() => _events.OrderBy(@event => @event.CompletionTimeCached).First();

        private Event GetEvent(string name) => _events.First(@event => @event.Name == name);
    }
}