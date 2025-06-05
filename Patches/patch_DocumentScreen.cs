using MonoMod;
using Quintessential;

class patch_DocumentScreen
{
    // make important fields public
    [MonoModPublic]
    public readonly string[] field_2408;
    [MonoModPublic]
    public readonly class_264 field_2409;

    public void method_50(float useless)
    {
        // not the most efficient due to the logic of orig_method_50, potentially use a try catch here instead?
        if (QApi.VanillaDocumentLayouts.Contains(field_2409.field_2090))
        {
            // handle default logic
            orig_method_50(useless);
            return;
        }
        if (!QApi.DocumentLayoutRenderers.TryGetValue(field_2409.field_2090, out var renderer))
        {
            Logger.Log($"Unknown renderer \"{field_2409.field_2090}\", Closing.");

            UI.InstantCloseScreen();
            return;
        }
        // use custom logic
        renderer((DocumentScreen)(object)this, useless);
    }

    [MonoModIgnore]
    public extern void orig_method_50(float useless);
}
