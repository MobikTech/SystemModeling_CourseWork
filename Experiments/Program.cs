using SystemModeling_CourseWork;

int p = 20;
int simulationTime = 2000;

ModelCreator modelCreator = new ModelCreator();

Console.WriteLine($"------------------------------TEST--------------------------------");
Console.WriteLine($"creatingInterval    crashInterval    processingDuration    meanLoad");
ShowMeanLoad(p, simulationTime, 1.5, 30, 0.75);
ShowMeanLoad(p, simulationTime, 0.5, 30, 0.75);
ShowMeanLoad(p, simulationTime, 1.5, 10, 0.75);
ShowMeanLoad(p, simulationTime, 0.5, 10, 0.75);

ShowMeanLoad(p, simulationTime, 1.5, 30, 0.25);
ShowMeanLoad(p, simulationTime, 0.5, 30, 0.25);
ShowMeanLoad(p, simulationTime, 1.5, 10, 0.25);
ShowMeanLoad(p, simulationTime, 0.5, 10, 0.25);


void ShowMeanLoad(int iterations, double simulationTime, double createInterval, double crashInterval, double processingDuration)
{
    double result = 0;
    for (int i = 0; i < iterations; i++)
    {
        result += modelCreator
            .Create(createInterval, crashInterval, processingDuration)
            .StartSimulationWithoutLogs(simulationTime).meanLoad;
    }

    result /= iterations;
    Console.WriteLine($"{createInterval}                 {crashInterval}               {processingDuration}                  {result}");
}

