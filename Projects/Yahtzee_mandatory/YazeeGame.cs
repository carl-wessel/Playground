using System.Collections.Immutable;
using Playground.Projects.Yahtzee.Extensions;
using Playground.Projects.Yahtzee.Models;
using PlayGround.Extensions;

namespace Playground.Projects.Yahtzee;

public static class YahzeeGame
{
    public static void RunSimulation()
    {
        System.Console.WriteLine("\nYahtzee Round Simulation:");
        System.Console.WriteLine("========================\n");

        ImmutableList<Player> players = ImmutableList.Create(
            new Player("Carl", new YahzeeCup()),
            new Player("Josef", new YahzeeCup()),
            new Player("Martin", new YahzeeCup()))
            .Tap(pl => Console.WriteLine(string.Join("\n", pl.Select(pl => $"{pl.Name} ready"))));

        var scoreCard = new YahtzeeScoreCard();
        var maxAmountOfRoundsInYahtzee = 13;

        var gameResult = Enumerable.Range(1, maxAmountOfRoundsInYahtzee)
            .Aggregate(
                (Players: players, ScoreCard: scoreCard),
                (state, round) =>
                {
                    var afterRoll = state.Players
                        .Select(pl => pl with { YahzeeCup = pl.YahzeeCup.ShakeAndRoll() })
                        .ToImmutableList()
                        .Tap(ps => Console.WriteLine($"\n=== Round {round} ===\n" + string.Join("\n", ps.Select(pl => $"{pl.Name}: {pl.YahzeeCup}"))));


                    var updatedScoreCard = afterRoll.Aggregate(
                        state.ScoreCard,
                        (sc, pl) => sc.AddCombination(pl, pl.YahzeeCup.GetYahtzeeCombination(sc.GetUsedCategories(pl.Name))));


                    updatedScoreCard
                        .Tap(sc => sc.PrintRoundResults(afterRoll, round));

                    return (afterRoll, updatedScoreCard);
                });

        gameResult.ScoreCard
            .Tap(sc => sc.PrintFinalResults());
    }

}
