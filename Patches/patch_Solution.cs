using MonoMod;
using Quintessential;

public class patch_Solution
{
    [MonoModIgnore]
    [PatchSolutionInitializer]
    public static extern Solution method_1957(Puzzle param_5505, string param_5506);

    public static bool GetConduits(Puzzle puzzle, out class_117[] conduits)
    {
        if (puzzle.field_2779.method_99(out class_261 cabinetInfo))  {
            conduits = cabinetInfo.field_2072;
            return true;
        }
        return ((patch_Puzzle)(object)puzzle).EngineConduits.method_99(out conduits);
    } 

    public static void ApplySolutionChanges(Puzzle puzzle, Solution solution)
    {
        if (((patch_Puzzle)(object)puzzle).Payloads.method_99(out Payloads payloads)) {
            foreach (Payloads.Payload p in payloads.SolutionInitialization)
            {
                foreach (var handler in QApi.SolutionPayloadHandlers)
                {
                    if (p.Address.Equals(handler.Left))
                    {
                        handler.Right(solution, p.Data);
                    }
                }
            }
        }
    }

}
