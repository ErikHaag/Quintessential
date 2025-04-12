
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

class patch_MoleculeEditorScreen{

	private static ILHook method_50_hook;
	internal patch_Puzzle editing;
	internal byte page = 0;
	
	public void create_method_50_hook() {
		method_50_hook = new(typeof(patch_MoleculeEditorScreen).GetMethod("orig_method_50"), DrawAtomsHook);
	}

	private static void DrawAtomsHook(ILContext hook)
	{
		ILCursor cursor = new(hook);
		if (!cursor.TryGotoNext(MoveType.Before,
			x => x.MatchLdarg(0),
			x => x.MatchLdloc(7),
			x => x.MatchLdfld(out _),
			x => x.MatchLdcI4(1),
			x => x.MatchCallvirt("ModdedLightning.MoleculeEditorScreen", "method_1130")
		))
		{
			throw new Exception("MoleculeEditorScreen does not contain the specified pattern!");
		}
		Quintessential.Logger.Log(cursor.Index);
	}

    public extern void orig_method_50(float param);
	public void method_50(float param) {
		orig_method_50(param);
        if (editing is { IsModdedPuzzle: true })
        { // find the correct position to put the atoms
            Vector2 uiSize = new(1516f, 922f);
            Vector2 corner = (Input.ScreenSize() / 2 - uiSize / 2 + new Vector2(-2f, -11f)).Rounded();
            Vector2 atomSize = new(95f, -90f);
            Vector2 atomPos = corner + new Vector2(169f, 754f); // vanilla atoms
            atomPos.X += atomSize.X * 3;
            foreach (var type in QApi.ModAtomTypes)
            {
                method_1130(atomPos, type, true);
                //atomPos.X += atomSize.X;
                atomPos.Y += atomSize.Y;
            }
        }
    }

	[MonoMod.MonoModIgnore]
	private extern void method_1130(Vector2 pos, AtomType type, bool b);
}