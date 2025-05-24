
#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
#pragma warning disable IDE1006


using Quintessential;
using MonoMod;
using System.Collections.Generic;

using Texture = class_256;

class patch_MoleculeEditorScreen
{

    internal patch_Puzzle editing;

    private static readonly Texture prevAtoms = class_235.method_615("Quintessential/editor_go_left");
    private static readonly Texture prevAtomsFaded = class_235.method_615("Quintessential/editor_go_left_faded");
    private static readonly Texture prevAtomsHover = class_235.method_615("Quintessential/editor_go_left_hover");
    private static readonly Texture nextAtoms = class_235.method_615("Quintessential/editor_go_right");
    private static readonly Texture nextAtomsFaded = class_235.method_615("Quintessential/editor_go_right_faded");
    private static readonly Texture nextAtomsHover = class_235.method_615("Quintessential/editor_go_right_hover");
    // for measuring stuff in debugging
    //private static readonly Texture dot = class_235.method_618(Color.Red);

    public static int currentPage = 0;


    private bool ShowExtraUI => editing is { IsModdedPuzzle: true } && Quintessential.QApi.ModAtomTypes.Count > 0;

    [PatchMoleculeEditorScreen]
    public extern void orig_method_50(float param);
    public void method_50(float param)
    {
        orig_method_50(param);
        if (!ShowExtraUI)
        {
            currentPage = 0;
            return;
        }
        
        // This was being instantiated before all other mods could call LoadPuzzleContent, causing the list to be unpopulated.
        // This only occurred when Reductive Metallurgy Campaign is loaded for me, though I've had other mods on.
        // Thankfully it's only an integer division, but I should find a more sensible patch.
        int LastPage = (Quintessential.QApi.ModAtomTypes.Count + 14) / 15;

        Vector2 uiSize = new(1516f, 922f);
        Vector2 corner = (Input.ScreenSize() / 2 - uiSize / 2 + new Vector2(-2f, -11f)).Rounded();
        Vector2 lPos = corner + new Vector2(90f, 800f);
        Vector2 rPos = lPos;
        rPos.X += 350 - nextAtoms.field_2056.X;
        bool inLeftBound = Bounds2.WithSize(lPos, prevAtoms.field_2056.ToVector2()).Contains(Input.MousePos());
        bool inRightBound = Bounds2.WithSize(rPos, nextAtoms.field_2056.ToVector2()).Contains(Input.MousePos());
        UI.DrawTexture(currentPage > 0 ? inLeftBound ? prevAtomsHover : prevAtoms : prevAtomsFaded, lPos);
        UI.DrawTexture(currentPage < LastPage ? inRightBound ? nextAtomsHover : nextAtoms : nextAtomsFaded, rPos);
        UI.DrawText($"{currentPage + 1}/{LastPage + 1}", corner + new Vector2(262f, 800f), UI.Text, UI.TextColor, TextAlignment.Centred);
        if (Input.IsLeftClickPressed() && (inLeftBound || inRightBound))
        {
            class_238.field_1991.field_1821.method_28(1f);

            if (inLeftBound && currentPage > 0)
            {
                currentPage--;
            }

            if (inRightBound && currentPage < LastPage)
            {
                currentPage++;
            }
        }
    }

    [MonoMod.MonoModIgnore]
    private extern void method_1130(Vector2 pos, AtomType type, bool b);
    internal void DrawAtoms(Vector2 corner, Vector2 spacing)
    {
        List<AtomType> atoms = new()
        {
            class_175.field_1675,
            class_175.field_1676,
            class_175.field_1678,
            class_175.field_1680,
            class_175.field_1679,
            class_175.field_1677,
            class_175.field_1681,
            class_175.field_1683,
            class_175.field_1684,
            class_175.field_1682,
            class_175.field_1685,
            class_175.field_1686,
            class_175.field_1687,
            class_175.field_1688,
            class_175.field_1690
        };
        if (editing is { IsModdedPuzzle: true })
        {
            atoms.AddRange(Quintessential.QApi.ModAtomTypes);

        }
        Vector2 pos = corner;
        bool showExtra = Input.IsShiftHeld();
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                int index = 15 * currentPage + 3 * y + x;
                if (index >= atoms.Count)
                {
                    goto outer;
                }
                this.method_1130(pos, atoms[index], true);
                if (showExtra)
                {
                    bool hovering = Bounds2.WithSize(pos - new Vector2(30, 30), new Vector2(61, 61)).Contains(Input.MousePos());
                    if (hovering) {
                        UI.DrawText(atoms[index].field_2284, pos + new Vector2(0, -40), class_238.field_1990.field_2140, UI.TextColor, TextAlignment.Centred);
                    }
                }
                pos.X += spacing.X;
            }
            pos.X = corner.X;
            pos.Y += spacing.Y;
        }
    outer:
        // ...
        return;
    }
}