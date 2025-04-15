
#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
#pragma warning disable IDE1006


using System.Drawing.Design;
using Quintessential;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Mono.Cecil.Cil;
using System.Reflection;
using Mono.Cecil;
using System;
using MonoMod;
using System.Collections.Generic;

using Texture = class_256;

class patch_MoleculeEditorScreen
{

    internal patch_Puzzle editing;

    private static readonly Texture prevAtoms = class_235.method_615("Quintessential/editor_go_left");
    private static readonly Texture prevAtomsHover = class_235.method_615("Quintessential/editor_go_left_hover");
    private static readonly Texture nextAtoms = class_235.method_615("Quintessential/editor_go_right");
    private static readonly Texture nextAtomsHover = class_235.method_615("Quintessential/editor_go_right_hover");
    private static readonly int LastPage = (Quintessential.QApi.ModAtomTypes.Count + 14) / 15;

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
        Vector2 uiSize = new(1516f, 922f);
        Vector2 corner = (Input.ScreenSize() / 2 - uiSize / 2 + new Vector2(-2f, -11f)).Rounded();
        Vector2 lPos = corner + new Vector2(90f, 800f);
        Vector2 rPos = lPos;
        rPos.X += 350 - nextAtoms.field_2056.X;
        bool inLeftBound = Bounds2.WithSize(lPos, prevAtoms.field_2056.ToVector2()).Contains(Input.MousePos());
        bool inRightBound = Bounds2.WithSize(rPos, nextAtoms.field_2056.ToVector2()).Contains(Input.MousePos());
        UI.DrawTexture(inLeftBound ? prevAtomsHover : prevAtoms, lPos);
        UI.DrawTexture(inRightBound ? nextAtomsHover : nextAtoms, rPos);
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