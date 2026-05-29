using MonoMod;

#pragma warning disable IDE1006 // Naming Styles

[MonoModPatch("class_228")]
public class patch_GlyphEffect
{
    public Color Color;

    public patch_GlyphEffect WithColor(Color color)
    {
        Color = color;
        return this;
    }

    [PatchGlyphEffectConstructor]
    [MonoModConstructor]
    [MonoModIgnore]
    public extern void ctor(SolutionEditorBase param_3958, enum_7 param_3959, Vector2 param_3960, class_256[] param_3961, float param_3962, Vector2 param_3963, float param_3964);
}