using System.Collections.Immutable;

namespace Playground.Lesson10.Exercises;

public static class AggregateExercises1Teacher
{
    public static void RunExercises()
    {
        Console.WriteLine("=== Aggregate Exercises ===\n");

        Exercise1_CarryLoop1ToLoop3();
    }

    // Exercise 1: Carry "Loop1" all the way to the Loop3 action
    // Goal: Pass the "Loop1" string from the outermost loop down to Loop3 so it can be used in Loop3's action
    static void Exercise1_CarryLoop1ToLoop3()
    {
        Console.WriteLine("--- Exercise 1: Carry Loop1 Identifier to Loop3 ---");
        
        // TODO: Modify the state to carry "Loop1" from Loop1 down through Loop2 to Loop3
        // Hint: You'll need to include the Loop1 identifier in the state passed to Loop2 and Loop3
        
        string variableToBeUpdate = "Initial Value";
        
        var resultLoop1 = Enumerable.Range(1, 3)
            .Aggregate(
                ("Loop1", ImmutableList<string>.Empty, variableToBeUpdate),    // Initial state for Loop1
                (stateLoop1, idxLoop1) =>
                {
                    Console.WriteLine($"Loop1 iteration: {idxLoop1}");
                    
                    // TODO: Pass stateLoop1.Item1 into Loop2's state
                    var resultLoop2 = Enumerable.Range(1, 3)
                        .Aggregate(
                            ("Loop2", ImmutableList<string>.Empty, stateLoop1.Item3), // TODO: Include Loop1 identifier here
                            (stateLoop2, idxLoop2) =>
                            {
                                Console.WriteLine($"   Loop2 iteration: {idxLoop2}");

                                // TODO: Pass the Loop1 identifier into Loop3's state
                                var resultLoop3 = Enumerable.Range(1, 3)
                                    .Aggregate(
                                        ("Loop3", ImmutableList<string>.Empty, stateLoop2.Item3), // TODO: Include Loop1 identifier here
                                        (stateLoop3, idxLoop3) =>
                                        {
                                            // TODO: Use the Loop1 identifier here (e.g., print it or add it to the result)
                                            Console.WriteLine($"      Loop3 iteration: {idxLoop3}");
                                            // Expected output should show "Loop1" is accessible here

                                            var newStateLoop3 = (stateLoop3.Item1, 
                                            stateLoop3.Item2.Add($"{idxLoop1} - {idxLoop2} - {idxLoop3}"), $"Final Value{idxLoop3}");
                                            return newStateLoop3;
                                        });

                                var newStateLoop2 = (stateLoop2.Item1, stateLoop2.Item2.AddRange(resultLoop3.Item2), resultLoop3.Item3);
                                return newStateLoop2;
                            });
                    
                    var newStateLoop1 = (stateLoop1.Item1, stateLoop1.Item2.AddRange(resultLoop2.Item2), resultLoop2.Item3);
                    return newStateLoop1;
                });
        System.Console.WriteLine(resultLoop1.Item3);
       // Console.WriteLine($"\nAll results: {resultLoop1.Item1}\n{string.Join("\n", resultLoop1.Item2)}\n");
    }
}
