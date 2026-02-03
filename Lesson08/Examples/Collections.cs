using System.Collections.Immutable;
using System.Diagnostics;
using Models.Music;
using Seido.Utilities.SeedGenerator;

namespace Playground.Lesson08.Examples;

public static class CollectionsExamples
{
    public static void RunExamples()
    {

        var seeder = new SeedGenerator();

        //List<T>
        List<MusicGroup> music = seeder.ItemsToList<MusicGroup>(10);

        //music.ForEach(Console.WriteLine);
        //music.ForEach(mg => Console.WriteLine(mg));

        System.Console.WriteLine("Start adding");
        var sw = new Stopwatch();
        sw.Start();
        var giantlist = seeder.ItemsToList<MusicGroup>(1_000);
        //var ll_giantlist = new LinkedList<MusicGroup>(giantlist);

        // for (int i = 0; i < 100_000; i++)
        // {
        //     giantlist.Insert(0,new MusicGroup().Seed(seeder));
        //     //ll_giantlist.AddFirst(new MusicGroup().Seed(seeder));

        // }
        // sw.Stop();
        // System.Console.WriteLine($"Elapsed time {sw.ElapsedMilliseconds} ms");

        Console.WriteLine("Dictionary<TKey,TValue>");
        System.Console.WriteLine("Dictionary by MusicGenre");
        var musicDict = new Dictionary<MusicGenre, List<MusicGroup>>();

        for (MusicGenre g = MusicGenre.Rock; g <= MusicGenre.Metal; g++)
        {
            musicDict[g] = giantlist.FindAll(mg => mg.Genre == g);   
        }

        System.Console.WriteLine("Jazz bands:");    
        musicDict[MusicGenre.Jazz].ForEach(Console.WriteLine);
        System.Console.WriteLine("Blues bands:");
        musicDict[MusicGenre.Blues].ForEach(Console.WriteLine);
    }
}