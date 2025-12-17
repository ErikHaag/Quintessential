using MonoMod;

public class patch_Solution
{
    [MonoModIgnore]
    [PatchConduitInitializer]
    public static extern Solution method_1957(Puzzle param_5505, string param_5506);

    public static bool GetConduits(Puzzle puzzle, out class_117[] conduits)
    {
        conduits = default;
        if (puzzle.field_2779.method_99(out class_261 cabinetInfo))  {
            conduits = cabinetInfo.field_2072;
            return true;
        }
        return ((patch_Puzzle)(object)puzzle).EngineConduits.method_99(out conduits);
    } 
}
