using MonoMod;

namespace Patches;
public class patch_Solution
{
    [MonoModIgnore]
    [PatchConduitInitializer]
    public extern Solution method_1957(Puzzle param_5505, string param_5506);
}
