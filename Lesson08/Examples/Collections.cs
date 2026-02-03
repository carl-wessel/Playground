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
        //List<MusicGroup> music = seeder.ItemsToList<MusicGroup>(10);

        //music.ForEach(Console.WriteLine);
        //music.ForEach(mg => Console.WriteLine(mg));

        var sw = new Stopwatch();
        sw.Start();
        System.Console.WriteLine("Creating");
        var giantlist = seeder.ItemsToList<MusicGroupLazy>(1_000_000);
        sw.Stop();
        System.Console.WriteLine($"Elapsed time {sw.ElapsedMilliseconds} ms");

        var ll_giantlist = new LinkedList<MusicGroupLazy>(giantlist);
        sw.Restart();
        System.Console.WriteLine("Start adding");
        for (int i = 0; i < 1_000_000; i++)
        {
            //giantlist.Insert(0,new MusicGroupLazy().Seed(seeder));
            ll_giantlist.AddFirst(new MusicGroupLazy().Seed(seeder));

        }
        sw.Stop();
        System.Console.WriteLine($"Elapsed time {sw.ElapsedMilliseconds} ms");


        Console.WriteLine("Dictionary<TKey,TValue>");
        System.Console.WriteLine("Dictionary by MusicGenre");
        var musicDict = new Dictionary<MusicGenre, List<MusicGroupLazy>>();

        for (MusicGenre g = MusicGenre.Rock; g <= MusicGenre.Metal; g++)
        {
            musicDict[g] = ll_giantlist.Where(mg => mg.Genre == g).ToList();   
        }

        System.Console.WriteLine($"Nr of Jazz bands: {musicDict[MusicGenre.Jazz].Count()}");    
        System.Console.WriteLine($"Nr of Blues bands: {musicDict[MusicGenre.Blues].Count()}");    
    }
}