# MarkovNamer
A C# library for random word generation using markov chains

Creates a random words that looks like a normal words but have no meaning

Can be used as an inspiration source or naming thing for something

Implementation of this idea https://listserv.brown.edu/archives/cgi-bin/wa?A2=ind0608A&L=CONLANG&P=R1976

Usage: 

```C#
var namer = MarkovNamer.CreateFromDictionary("english.txt");
var random = new Random();
Console.WriteLine(namer.GenWord(2, random));
```
-

Библиотека для генеации случайных слов, которые выглядят как обычные, но не имеют смысла.
Реализация идеи https://listserv.brown.edu/archives/cgi-bin/wa?A2=ind0608A&L=CONLANG&P=R1976
Может быть использована как источник для креативных процессов либо как средство для автоматического названия чего-либо
