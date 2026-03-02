using System.Collections.Immutable;
using Playground.Projects.Yahtzee.Extensions;
using PlayGround.Extensions;

namespace Playground.Projects.Yahtzee.Models;

public record YahtzeeScoreCard(ImmutableDictionary<string, ImmutableList<YahzeeCup>> Scores)
{
    public YahtzeeScoreCard() : this(ImmutableDictionary<string, ImmutableList<YahzeeCup>>.Empty) { }

    public override string ToString()
    {
        var sRet = "";
        foreach (var kvp in Scores)
        {
            var totalScore = kvp.Value.Sum(v => v.Score);
            sRet += $"\n  {kvp.Key}: {string.Join(", ", kvp.Value.Select(v => $"{v.GetType().Name}({v.Score})"))} | Total: {totalScore}";
        }
        return sRet;
    }
}

public static class YahtzeeScoreCardExtensions
{
    public static YahtzeeScoreCard AddCombination(this YahtzeeScoreCard scoreCard, Player player, YahzeeCup combination) // tar in scoreCard, player och kombination allt i ett
    {
        var currentList = scoreCard.Scores.GetValueOrDefault(player.Name, ImmutableList<YahzeeCup>.Empty);
        return scoreCard with { Scores = scoreCard.Scores.SetItem(player.Name, currentList.Add(combination)) };
    }

    public static IEnumerable<string> DetermineRoundWinner(this YahtzeeScoreCard scoreCard, ImmutableList<Player> playersInRound)
    {
        var maxScore = playersInRound.Max(p => scoreCard.Scores[p.Name].Last().Score);

        return playersInRound
            .Where(p => scoreCard.Scores[p.Name].Last().Score == maxScore)
            .Select(p => p.Name);
    }

    public static IEnumerable<string> DetermineOverallWinner(this YahtzeeScoreCard scoreCard)
    {
        // Kan lägga till if-sats för att hantera att det inte finns spelare/poäng men detta är onödigt då listan av spelare alltid förekommer.

        var maxTotalScore = scoreCard.Scores.Max(kvp => kvp.Value.Sum(v => v.Score));

        return scoreCard.Scores
            .Where(kvp => kvp.Value.Sum(v => v.Score) == maxTotalScore)
            .Select(kvp => kvp.Key);
    }

    public static ImmutableHashSet<string> GetUsedCategories(this YahtzeeScoreCard scoreCard, string playerName)
    {
        return scoreCard.Scores.GetValueOrDefault(playerName, ImmutableList<YahzeeCup>.Empty)
            .Select(combo => combo.GetType().Name)
            .ToImmutableHashSet();
    }

    public static void PrintRoundResults(this YahtzeeScoreCard scoreCard, ImmutableList<Player> players, int round)
    {
        var results = players
            .Select(p => $"{p.Name}: {scoreCard.Scores[p.Name].Last().GetType().Name} ({scoreCard.Scores[p.Name].Last().Score} pts)")
            .ToList();

        var winners = scoreCard.DetermineRoundWinner(players).ToList();

        var winnerMessage = winners.Count == 1
        ? $"Round {round} Winner: {winners.First()}"
        : $"Round {round} Tie between: {string.Join(" and ", winners)}";

        Console.WriteLine("\nRound results:\n" +
        string.Join("\n", results) +
        $"\n \n{winnerMessage}");
    }

    public static void PrintFinalResults(this YahtzeeScoreCard scoreCard)
    {
        Console.WriteLine("\n\n=== FINAL SCORE CARD ===\n" + scoreCard);

        var winners = scoreCard.DetermineOverallWinner().ToList();
        var totalScore = scoreCard.Scores
            .First(kvp => kvp.Key == winners.First())
            .Value
            .Sum(v => v.Score);

        Console.WriteLine($"\nOverall Winner(s): {string.Join(", ", winners)}\n   Total Score: {totalScore} points");
    }
}
