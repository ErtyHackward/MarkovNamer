using System;
using April32;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            // create a markov base from dictionary

            Console.WriteLine("Extracting markov tokens...");

            var namer = MarkovNamer.CreateFromDictionary("english.txt");

            var random = new Random();

            Console.WriteLine();

            Console.WriteLine(namer.GenWord(2, random));
            Console.WriteLine(namer.GenWord(2, random));
            Console.WriteLine(namer.GenWord(2, random));
            Console.WriteLine(namer.GenWord(2, random));
            
            Console.WriteLine();

            Console.WriteLine(namer.GenWord(3, random));
            Console.WriteLine(namer.GenWord(3, random));
            Console.WriteLine(namer.GenWord(3, random));
            Console.WriteLine(namer.GenWord(3, random));

            Console.WriteLine();

            Console.WriteLine("Saving tokens to a file...");

            MarkovNamer.SaveBinary(namer, "eng.dat");

            Console.WriteLine("Loading tokens from a file...");

            var loadedNamer = MarkovNamer.LoadBinary("eng.dat");

            Console.WriteLine();

            Console.WriteLine(loadedNamer.GenWord(2, random));
            Console.WriteLine(loadedNamer.GenWord(2, random));
            Console.WriteLine(loadedNamer.GenWord(2, random));
            Console.WriteLine(loadedNamer.GenWord(2, random));
            
            Console.ReadKey();
        }
    }

}

