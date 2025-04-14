
#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
//#pragma warning disable IDE1006


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
    private static readonly Texture prevAtoms = class_235.method_615("Quintessential/journal_go_left");
    private static readonly Texture prevAtomsHover = class_235.method_615("Quintessential/journal_go_left_hover");
    private static readonly Texture nextAtoms = class_235.method_615("Quintessential/journal_go_right");
    private static readonly Texture nextAtomsHover = class_235.method_615("Quintessential/journal_go_right_hover");
    public static int currentPage = 0;

    [PatchMoleculeEditorScreen]
    public extern void orig_method_50(float param);
    public void method_50(float param)
    {
        orig_method_50(param);
        if (editing is { IsModdedPuzzle: false } || Quintessential.QApi.ModAtomTypes.Count == 0)
        {
            currentPage = 0;
            return;
        }
        Vector2 uiSize = new(1516f, 922f);
        Vector2 corner = (Input.ScreenSize() / 2 - uiSize / 2 + new Vector2(-2f, -11f)).Rounded();
        Vector2 offset = new(84f, 812f);
        Vector2 lPos = corner + offset;
        Vector2 rPos = corner + offset + new Vector2(104f, 0f);
        bool inLeftBound = Bounds2.WithSize(lPos, prevAtoms.field_2056.ToVector2()).Contains(Input.MousePos());
        bool inRightBound = Bounds2.WithSize(rPos, nextAtoms.field_2056.ToVector2()).Contains(Input.MousePos());
        UI.DrawTexture(inLeftBound ? prevAtomsHover : prevAtoms, lPos);
        UI.DrawTexture(inRightBound ? nextAtomsHover : nextAtoms, rPos);
        int pages = (byte)(1 + (Quintessential.QApi.ModAtomTypes.Count + 14) / 15);
        UI.DrawText($"{currentPage + 1}/{pages}", corner + offset + new Vector2(73f, 12f), UI.Text, UI.TextColor, TextAlignment.Centred);
        if (Input.IsLeftClickPressed() && (inLeftBound || inRightBound))
        {
            class_238.field_1991.field_1821.method_28(1f);

            if (inLeftBound)
            {
                var next = currentPage - 1;
                if (next < 0)
                    next += pages;
                currentPage = next;
            }

            if (inRightBound)
            {
                var next = currentPage + 1;
                if (next >= pages)
                    next = 0;
                currentPage = next;
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