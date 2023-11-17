// See https://aka.ms/new-console-template for more information
using beatleader_parser;
using BSMapDiffGenerator;
using BSMapDiffGenerator.Models;
using Newtonsoft.Json;
using Parser.Map;
using Parser.Map.Difficulty.V3.Base;
using System.Diagnostics;

Console.WriteLine("Hello, World!");
Console.WriteLine("input old map location");
string location = Console.ReadLine();

Console.WriteLine("input new map location");
string newlocation = Console.ReadLine();

Console.WriteLine("input difficulty to compare (no input = ExpertPlus)");
string diffToCompare = Console.ReadLine();

Console.WriteLine("input characteristic to compare (no input = Standard)");
string charToCompare = Console.ReadLine();

if (diffToCompare == "") diffToCompare = "ExpertPlus";
if (charToCompare == "") charToCompare = "Standard";

Parse parser = new();
BeatmapV3 oldMap = parser.TryLoadPath(location);
BeatmapV3 newMap = parser.TryLoadPath(newlocation);

Stopwatch sw = new Stopwatch();

sw.Start();

var diff = MapDiffGenerator.GenerateDifficultyDiff(newMap.Difficulties.First(x => x.Difficulty == diffToCompare && x.Characteristic == charToCompare).Data, oldMap.Difficulties.First(x => x.Difficulty == diffToCompare && x.Characteristic == charToCompare).Data);

sw.Stop();

Console.WriteLine(JsonConvert.SerializeObject(diff, Formatting.Indented));

Console.WriteLine($"Time taken: {sw.ElapsedMilliseconds}ms");

File.WriteAllText("diff.json", JsonConvert.SerializeObject(diff, Formatting.Indented));
File.WriteAllText("newMap.json", JsonConvert.SerializeObject(newMap.Difficulties[0].Data, Formatting.Indented));
File.WriteAllText("oldMap.json", JsonConvert.SerializeObject(oldMap.Difficulties[0].Data, Formatting.Indented));

diff.Sort((x, y) => x.Object.Beats.CompareTo(y.Object.Beats));

foreach (var entry in diff)
{
    switch (entry.Type)
    {
        case (DiffType)0:
            if (entry.Object is BeatmapGridObject obj) Console.WriteLine($"+ Added    {obj.Beats} at x {obj.x} y {obj.y} (in {entry.CollectionType})");
            else Console.WriteLine($"+ Added    {entry.Object.Beats} (in {entry.CollectionType})");
            break;
        case (DiffType)1:
            if (entry.Object is BeatmapGridObject obj2) Console.WriteLine($"- Removed  {obj2.Beats} at x {obj2.x} y {obj2.y} (in {entry.CollectionType})");
            else Console.WriteLine($"- Removed  {entry.Object.Beats} (in {entry.CollectionType})");
            break;
        case (DiffType)2:
            if (entry.Object is BeatmapGridObject obj3) Console.WriteLine($"/ Modified {obj3.Beats} at x {obj3.x} y {obj3.y} (in {entry.CollectionType})");
            else Console.WriteLine($"/ Modified {entry.Object.Beats} (in {entry.CollectionType})");
            break;
        default: break;
    }
}

Console.WriteLine("Meow! Press any key to exit");
Console.ReadKey();