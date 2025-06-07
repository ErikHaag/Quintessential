using MonoMod;

[MonoModPatch("class_252")]
public class patch_Cutscene
{
    public class_256 Background;
    public string Setting;

    [MonoModConstructor]
    public void ctor(class_264 param_4171, class_186 param_4172)
    {
        orig_ctor(param_4171, param_4172);
        Maybe<patch_LocVignette.CutsceneInfo> csI = ((patch_LocVignette)(object)param_4171).csI;
        if (csI.method_99(out var cutsceneInfo))
        {
            Background = class_235.method_615(cutsceneInfo.Background);
            Setting = cutsceneInfo.Setting;
        }
        else
        {
            Background = null;
            Setting = "";
        }
    }

    [MonoModIgnore]
    public extern void orig_ctor(class_264 param_4171, class_186 param_4172);

    [PatchCutsceneRenderer, MonoModIgnore]
    public extern void method_50(float param_4176);
}