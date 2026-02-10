using System.Collections.Immutable;
using Playground.Projects.Poker.Extensions;
using Playground.Projects.Poker.Models;
using PlayGround.Extensions;

namespace Playground.Projects.Poker;

public static class PokerGameAnswers
{
    public static void RunSimulation()
    {
        System.Console.WriteLine("\nPoker Round Simulation:");
        System.Console.WriteLine("Your code should implement the Poker round simulation below.");
        System.Console.WriteLine("========================");

        //Create players
        ImmutableList<Player> players = ImmutableList.Create(
            new Player("Alice", new PokerHand()) 
            ,new Player("Bob", new PokerHand())
            ,new Player("Diana", new PokerHand())
            )
            .Tap(p => Console.WriteLine(string.Join(", ", p.Select(pl => $"{pl.Name} has {pl.Hand.cards.Count} cards on hand"))));   

        var fullDeck = CardDeck.Create()
            //.Sort(cards => cards.OrderBy(c => c.Rank).ThenBy(c => c.Suit))
            //.Sort(cards => cards.OrderBy(c => c.Suit).ThenBy(c => c.Rank))
            .Shuffle()
            .Tap(deck => Console.WriteLine("Shuffled Deck:\n" + deck));
        
        var scoreCard = new ScoreCard();
        // #### 1. Round Management
        // One round deals 5 cards to each player using functional patterns
        var maxNrOfRounds = fullDeck.cards.Count / (players.Count * 5);
        var gameResult = Enumerable.Range(1, maxNrOfRounds)
            .Aggregate(
                (Deck: fullDeck, Players: players, ScoreCard: scoreCard),
                (state, round) =>
                {
                    var stateAfterDeal = DealRound(state.Deck, state.Players, round);

                    stateAfterDeal.Players
                        .Tap(ps => Console.WriteLine($"\nPlayer hands after round {round}:\n" + string.Join("\n", 
                                    ps.Select(p => $"{p.Name}: {string.Join(", ", p.Hand.cards)} -> {p.Hand.GetPokerRank().GetType().Name}"))));

                    var winnerNames = stateAfterDeal.Players.DetermineWinners().Select(p => p.Name)
                        .Tap(names => Console.WriteLine($"\nRound {round} winner(s): {string.Join(", ", names)}"));

                    stateAfterDeal.Deck
                        .Tap(deck => Console.WriteLine($"\nRemaining cards in deck after round {round}: {deck.cards.Count}"));

                    var updatedScoreCard = stateAfterDeal.Players
                        .Aggregate(state.ScoreCard, (scoreCard, player) => scoreCard.AddScore(player, player.Hand));
                    
                    return (stateAfterDeal.Deck, state.Players, updatedScoreCard);
                });

        gameResult.ScoreCard
            .Tap(sc => Console.WriteLine("\nFinal Score Card:\n" + sc));

        gameResult.ScoreCard
            .DetermineAllRoundWinners()
            .Tap(roundWinners => Console.WriteLine("\nWinners for each round:\n" + string.Join("\n", roundWinners.Select(rw => $"Round {rw.Key}: {string.Join(", ", rw.Value)}"))));

    }

    private static (CardDeck Deck, ImmutableList<Player> Players) DealRound(CardDeck deck, ImmutableList<Player> players, int roundNumber)
    {
        // Deal cards round-robin: one card to each player, repeated 5 times
        var (updatedDeck, playersWithHands) = Enumerable.Range(1, 5)
            .Aggregate(
                (Deck: deck, Players: players),
                (state, cardNumber) =>
                {
                    // Deal one card to each player in this round
                    var (deckAfterRound, updatedPlayers) = state.Players
                        .Aggregate(
                            (Deck: state.Deck, Players: ImmutableList<Player>.Empty),
                            (innerState, player) =>
                            {
                                // Draw one card for this player
                                var deckAfterDraw = innerState.Deck.Draw(out var card);
                                
                                // Add card to player's hand
                                var updatedPlayer = player with 
                                { 
                                    Hand = new PokerHand { cards = player.Hand.cards.Add(card) } 
                                };
                                
                                return (deckAfterDraw, innerState.Players.Add(updatedPlayer));
                            });
                    
                    return (deckAfterRound, updatedPlayers);
                });

        return (updatedDeck, playersWithHands);
    }
}