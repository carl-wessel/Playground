using System.Collections.Immutable;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;

namespace Playground.Projects.Yahtzee.Models;

public record YahzeeCup : CupOfDice
{
    IEnumerable<IGrouping<DiePip, Die>> dicePipGroups => this.dice.GroupBy(d => d.Pip);
    IOrderedEnumerable<DiePip> sortedDicePips => this.dice.Select(d => d.Pip).OrderBy(v => v);

    Dictionary<string, List<DiePip>> straightsCombinations
    {
        get
        {
            return new Dictionary<string, List<DiePip>>
        {
            {"SmallStraight1", new List<DiePip> {DiePip.One, DiePip.Two, DiePip.Three, DiePip.Four}},
            {"SmallStraight2", new List<DiePip> {DiePip.Two, DiePip.Three, DiePip.Four, DiePip.Five}},
            {"SmallStraight3", new List<DiePip> {DiePip.Three, DiePip.Four, DiePip.Five, DiePip.Six}},
            {"LargeStraight1", new List<DiePip> {DiePip.One, DiePip.Two, DiePip.Three, DiePip.Four, DiePip.Five}},
            {"LargeStraight2", new List<DiePip> {DiePip.Two, DiePip.Three, DiePip.Four, DiePip.Five, DiePip.Six}}
        };
        }
    }

    public override string ToString() => base.ToString();

    public int Score => this switch
    {
        Ones => this.dice.Where(d => d.Pip == DiePip.One).Sum(d => (int)d.Pip),
        Twos => this.dice.Where(d => d.Pip == DiePip.Two).Sum(d => (int)d.Pip),
        Threes => this.dice.Where(d => d.Pip == DiePip.Three).Sum(d => (int)d.Pip),
        Fours => this.dice.Where(d => d.Pip == DiePip.Four).Sum(d => (int)d.Pip),
        Fives => this.dice.Where(d => d.Pip == DiePip.Five).Sum(d => (int)d.Pip),
        Sixes => this.dice.Where(d => d.Pip == DiePip.Six).Sum(d => (int)d.Pip),
        ThreeOfAKind => this.dice.Sum(d => (int)d.Pip),
        FourOfAKind => this.dice.Sum(d => (int)d.Pip),
        FullHouse => 25,
        SmallStraight => 30,
        LargeStraight => 40,
        Yahtzee => 50,
        Chance => this.dice.Sum(d => (int)d.Pip),
        _ => 0
    };
    public YahzeeCup() : base(5)
    { }

    public YahzeeCup GetYahtzeeCombination(ImmutableHashSet<string> alreadyUsedCombinations = null)
    {
        if (dice.Count() != 5)
            return new NoCombination() { dice = this.dice };

        alreadyUsedCombinations ??= ImmutableHashSet<string>.Empty; // Detta är för att spelaren inte ska kunna använda samma kombination flera gånger, t.ex. Full house två gånger.

        bool isOnes = sortedDicePips.Any(pip => pip == DiePip.One);
        bool isTwos = sortedDicePips.Any(pip => pip == DiePip.Two);
        bool isThrees = sortedDicePips.Any(pip => pip == DiePip.Three);
        bool isFours = sortedDicePips.Any(pip => pip == DiePip.Four);
        bool isFives = sortedDicePips.Any(pip => pip == DiePip.Five);
        bool isSixes = sortedDicePips.Any(pip => pip == DiePip.Six);


        bool isThreeOfAKind = dicePipGroups.Any(g => g.Count() >= 3);
        bool isFourOfAKind = dicePipGroups.Any(group => group.Count() == 4);
        bool isFullHouse = dicePipGroups.Any(group => group.Count() == 3) && dicePipGroups.Any(group => group.Count() == 2);

        bool isSmallStraight = straightsCombinations
            .Where(kvp => kvp.Key.StartsWith("SmallStraight"))
            .Any(kvp => kvp.Value.All(pip => sortedDicePips.Contains(pip)));

        bool isLargeStraight = straightsCombinations
            .Where(kvp => kvp.Key.StartsWith("LargeStraight"))
            .Any(kvp => kvp.Value.All(pip => sortedDicePips.Contains(pip)));

        bool isYahtzee = dicePipGroups.Any(g => g.Count() == 5);

        return new (bool IsValid, YahzeeCup Combo, string Name)[]
    {
        (isYahtzee, new Yahtzee() { dice = this.dice }, nameof(Yahtzee)),
        (isLargeStraight, new LargeStraight() { dice = this.dice }, nameof(LargeStraight)),
        (isSmallStraight, new SmallStraight() { dice = this.dice }, nameof(SmallStraight)),
        (isFullHouse, new FullHouse() { dice = this.dice }, nameof(FullHouse)),
        (isFourOfAKind, new FourOfAKind() { dice = this.dice }, nameof(FourOfAKind)),
        (isThreeOfAKind, new ThreeOfAKind() { dice = this.dice }, nameof(ThreeOfAKind)),
        (isSixes, new Sixes() { dice = this.dice }, nameof(Sixes)),
        (isFives, new Fives() { dice = this.dice }, nameof(Fives)),
        (isFours, new Fours() { dice = this.dice }, nameof(Fours)),
        (isThrees, new Threes() { dice = this.dice }, nameof(Threes)),
        (isTwos, new Twos() { dice = this.dice }, nameof(Twos)),
        (isOnes, new Ones() { dice = this.dice }, nameof(Ones)),
        (true, new Chance() { dice = this.dice }, nameof(Chance))
    }
    .Where(c => c.IsValid)
    .Where(c => !alreadyUsedCombinations.Contains(c.Name)) // För att filtrera bort redan använda kombinationer
    .Select(c => c.Combo)
    .MaxBy(c => c.Score)
    ?? new NoCombination() { dice = this.dice }; // När ingen giltig kombination hittas, returnera NoCombination
    }

    //Disciminators for yahtzee combinations
    public record Yahtzee : YahzeeCup
    {
    }
    public record LargeStraight : YahzeeCup
    {
    }
    public record SmallStraight : YahzeeCup
    {
    }
    public record FullHouse : YahzeeCup
    {
    }
    public record FourOfAKind : YahzeeCup
    {
    }
    public record ThreeOfAKind : YahzeeCup
    {
    }
    public record Sixes : YahzeeCup
    {
    }
    public record Fives : YahzeeCup
    {
    }
    public record Fours : YahzeeCup
    {
    }
    public record Threes : YahzeeCup
    {
    }
    public record Twos : YahzeeCup
    {
    }
    public record Ones : YahzeeCup
    {
    }
    public record Chance : YahzeeCup
    {
    }
    public record NoCombination : YahzeeCup
    {
    }
}