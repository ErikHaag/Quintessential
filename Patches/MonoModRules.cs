using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.InlineRT;
using System;
using System.Linq;
using System.Reflection;

namespace MonoMod;

[MonoModCustomMethodAttribute(nameof(MonoModRules.PatchSettingsStaticInit))]
class PatchSettingsStaticInit : Attribute{}

[MonoModCustomMethodAttribute(nameof(MonoModRules.PatchPuzzleIdWrite))]
class PatchPuzzleIdWrite : Attribute{}

[MonoModCustomMethodAttribute(nameof(MonoModRules.PatchScoreManagerLoad))]
class PatchScoreManagerLoad : Attribute{}

[MonoModCustomMethodAttribute(nameof(MonoModRules.PatchGifRecorderFrame))]
class PatchGifRecorderFrame : Attribute{}

[MonoModCustomMethodAttribute(nameof(MonoModRules.PatchCutsceneRenderer))]
class PatchCutsceneRenderer : Attribute{}

[MonoModCustomMethodAttribute(nameof(MonoModRules.PatchMoleculeEditorScreen))]
class PatchMoleculeEditorScreen : Attribute{}

[MonoModCustomMethodAttribute(nameof(MonoModRules.PatchPuzzleEditorScreen))]
class PatchPuzzleEditorScreen : Attribute{}

[MonoModCustomMethodAttribute(nameof(MonoModRules.PatchJournalScreen))]
class PatchJournalScreen : Attribute{}

[MonoModCustomMethodAttribute(nameof(MonoModRules.PatchJournalPuzzleBackgrounds))]
class PatchJournalPuzzleBackgrounds : Attribute{}

static class MonoModRules{

	static MonoModRules(){
		MonoModRule.Modder.Log("Patching OM");
	}

	public static void PatchSettingsStaticInit(MethodDefinition method, CustomAttribute attrib){
		MonoModRule.Modder.Log("Patching settings static init");
		// Set "class_110.field_1012" (Steam support) to false
		if(method.HasBody){
			ILCursor cursor = new(new ILContext(method));
			if(cursor.TryGotoNext(MoveType.Before,
				   instr => instr.MatchLdcI4(1),
				   instr => instr.MatchStsfld("class_110", "field_1012"))){
				cursor.Remove();
				cursor.Emit(OpCodes.Ldc_I4_0);
			}else{
				Console.WriteLine("Failed to disable Steam setting in class_110!");
				throw new Exception();
			}
		}else{
			Console.WriteLine("Failed to disable Steam setting in class_110!");
			throw new Exception();
		}
	}

	public static void PatchPuzzleIdWrite(MethodDefinition method, CustomAttribute attrib){
		MonoModRule.Modder.Log("Patching puzzle ids");
		// Replace "SteamUser.GetSteamID().m_SteamID" with "0" (until a proper format is created)
		if(method.HasBody){
			ILCursor cursor = new(new ILContext(method));
			if(cursor.TryGotoNext(MoveType.Before,
				   instr => instr.MatchCall("Steamworks.SteamUser", "GetSteamID"),
				   instr => instr.MatchLdfld("Steamworks.CSteamID", "m_SteamID"))){
				cursor.Remove();
				cursor.Remove();
				cursor.Emit(OpCodes.Ldc_I8, (long)0);
			}
		}else{
			Console.WriteLine("Failed to modify puzzle serialization!");
			throw new Exception();
		}
	}

	public static void PatchScoreManagerLoad(MethodDefinition method, CustomAttribute attrib){
		MonoModRule.Modder.Log("Patching ScoreManager loading");
		if(method.HasBody){
			ILCursor cursor = new(new ILContext(method));
			if(cursor.TryGotoNext(MoveType.After, instr => instr.Match(OpCodes.Brfalse_S))
			   && cursor.TryGotoNext(MoveType.After, instr => instr.Match(OpCodes.Brfalse_S))
			   && cursor.TryGotoNext(MoveType.After, instr => instr.Match(OpCodes.Brfalse_S))){
				cursor.Emit(OpCodes.Ret);
			}else{
				Console.WriteLine("Failed to modify ScoreManager loading (no match)!");
				throw new Exception();
			}
		}else{
			Console.WriteLine("Failed to modify ScoreManager loading (no body)!");
			throw new Exception();
		}
	}

	public static void PatchGifRecorderFrame(MethodDefinition method, CustomAttribute attrib){
		MonoModRule.Modder.Log("Patching GIF recorder frame rendering");
		if(method.HasBody) {
			ILCursor cursor = new(new ILContext(method));
			if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchCall("class_135", "method_272"))){
				// "class_135.method_272(class_238.field_1989.field_81.field_613.field_632, new Vector2());"
				TypeDefinition holder = MonoModRule.Modder.FindType("class_250").Resolve();
				MethodDefinition to = holder.Methods.First(m => m.Name.Equals("MarkOnFrame"));
				cursor.Emit(OpCodes.Call, to);
			}else{
				Console.WriteLine("Failed to modify GIF recorder frame rendering (no match)!");
				throw new Exception();
			}
		}else{
			Console.WriteLine("Failed to modify GIF recorder frame rendering (no body)!");
			throw new Exception();
		}
	}
	
	public static void PatchCutsceneRenderer(MethodDefinition method, CustomAttribute attrib)
	{
		MonoModRule.Modder.Log("Patching cutscene renderer");
		if (!method.HasBody)
		{
			Console.WriteLine("Failed to modify cutscene renderer (no body)!");
			throw new Exception();
		}
		ILContext context = new(method);
        ILCursor cursor = new(context);
		if (!cursor.TryGotoNext(MoveType.After,
			instr => instr.MatchLdarg(0),
			instr => instr.MatchLdarg(0),
			instr => instr.MatchLdfld("class_252", "field_2043"),
			instr => instr.MatchLdarg(1),
			instr => instr.MatchAdd(),
			instr => instr.MatchStfld("class_252", "field_2043")
        ))
		{
            Console.WriteLine("Failed to modify cutscene renderer (failed to find start of hash table)");
            throw new Exception();
        }
		MethodInfo stringIsNullOrEmpty = typeof(String).GetMethod("IsNullOrEmpty");

		TypeDefinition cutsceneType = MonoModRule.Modder.FindType("class_252").Resolve();
		FieldDefinition settingFD = cutsceneType.Fields.First(f => f.Name.Equals("Setting"));
		FieldDefinition backgroundFD = cutsceneType.Fields.First(f => f.Name.Equals("Background"));

        // wrap an if-else block around existing code.
        cursor.Emit(OpCodes.Ldarg_0);
        MonoModRule.Modder.Log("B");
        cursor.Emit(OpCodes.Ldfld, settingFD);
        MonoModRule.Modder.Log("C");
        cursor.Emit(OpCodes.Call, stringIsNullOrEmpty);
        MonoModRule.Modder.Log("D");
        int ifBody = cursor.Index;
		cursor.Emit(OpCodes.Ldarg_0);
        cursor.Emit(OpCodes.Ldfld, settingFD);
		cursor.Emit(OpCodes.Stloc, 4);
		cursor.Emit(OpCodes.Ldarg_0);
		cursor.Emit(OpCodes.Ldfld, backgroundFD);
		cursor.Emit(OpCodes.Stloc, 5);
		int elseBody = cursor.Index;

		if (!cursor.TryGotoNext(MoveType.AfterLabel,
				instr => instr.MatchLdsfld("Color", "Black"),
				instr => instr.MatchLdsfld("Vector2", "Zero"),
				instr => instr.MatchLdsfld("class_115", "field_1433"),
				instr => instr.MatchCall("class_135", "method_279")
		))
		{
			Console.WriteLine("Failed to modify cutscene renderer (failed to find end of hash table)");
			throw new Exception();
		}
		int elseEnd = cursor.Index;
		cursor.Index = elseBody;
		cursor.Emit(OpCodes.Br, cursor.Instrs[elseEnd]);
        cursor.Index = ifBody;
        cursor.Emit(OpCodes.Brtrue_S, cursor.Instrs[elseBody + 1]);

		// TODO: add more control to cutscenes, like flipping characters and dismissing them
    }


    public static void PatchMoleculeEditorScreen(MethodDefinition method, CustomAttribute attrib)
	{
		MonoModRule.Modder.Log("Patching molecule editor screen");
		if (!method.HasBody)
		{
			Console.WriteLine("Failed to modify molecule editor rendering (no body)!");
			throw new Exception();
		}
		ILCursor cursor = new(new ILContext(method)); // Create cursor
		if (!cursor.TryGotoNext(MoveType.Before,
			instr => instr.MatchLdarg(0),
			instr => instr.MatchLdloc(7),
			instr => instr.MatchLdsfld("class_175", "field_1675"),
			instr => instr.MatchLdcI4(1),
			instr => instr.MatchCallvirt("MoleculeEditorScreen", "method_1130") // Move to the function call
		))
		{
			Console.WriteLine("Failed to modify molecule editor rendering (no start match)!");
			throw new Exception();
		}
		int start = cursor.Index;
        if (!cursor.TryGotoNext(MoveType.After,
            instr => instr.MatchLdarg(0),
            instr => instr.MatchLdloc(7),
            instr => instr.MatchLdsfld("class_175", "field_1690"),
            instr => instr.MatchLdcI4(1),
            instr => instr.MatchCallvirt("MoleculeEditorScreen", "method_1130") // Move to the function call
        ))
        {
            Console.WriteLine("Failed to modify molecule editor rendering (no near end match)!");
            throw new Exception();
        }
        if (!cursor.TryGotoNext(MoveType.After,
            instr => instr.MatchStindR4()
        ))
        {
            Console.WriteLine("Failed to modify molecule editor rendering (no end match)!");
            throw new Exception();
        }
        int end = cursor.Index;

        TypeDefinition host = MonoModRule.Modder.FindType("MoleculeEditorScreen").Resolve();
		MethodDefinition parasite = host.Methods.First(m => m.Name.Equals("DrawAtoms"));

        cursor.Goto(start);
		cursor.RemoveRange(end - start); // Go bye with you
		cursor.Emit(OpCodes.Ldarg_0); // this
		cursor.Emit(OpCodes.Ldloc, 7);
        cursor.Emit(OpCodes.Ldloc, 6);
		cursor.Emit(OpCodes.Callvirt, parasite);
	}

	public static void PatchPuzzleEditorScreen(MethodDefinition method, CustomAttribute attrib) {
		MonoModRule.Modder.Log("Patching puzzle editor screen");
		if(method.HasBody){
			ILCursor cursor = new(new ILContext(method));
			Instruction target = null; // will definitely be set
			
			// kill off `flag5` and make the Upload puzzle button never clickable
			if(cursor.TryGotoNext(MoveType.Before, instr => instr.MatchLdloc(27))){
				cursor.Remove();
				cursor.Emit(OpCodes.Ldc_I4_0);
			}else{
				Console.WriteLine("Failed to modify puzzle editor screen (no 1st match)!");
				throw new Exception();
			}

			if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdflda("PuzzleEditorScreen", "field_2789"),
				   instr => instr.MatchCall(out MethodReference mref) && mref.Name.Equals("method_1085"),
				   instr => {
					   bool ret = instr.OpCode == OpCodes.Brfalse;
					   if(ret)
						   target = (Instruction)instr.Operand;
					   return ret;
				   })){
				// "if(this.field_2789.method_1085()){" ... "} if (!this.field_2789.method_1085()){"
				TypeDefinition holder = MonoModRule.Modder.FindType("PuzzleEditorScreen").Resolve();
				MethodDefinition to = holder.Methods.First(m => m.Name.Equals("DisplayEditorPanelScreen"));
				cursor.Emit(OpCodes.Ldarg_0); // this
				cursor.Emit(OpCodes.Call, to); // call reimplementation
				cursor.Emit(OpCodes.Br, target); // skip rest of `if` statement
			}else{
				Console.WriteLine("Failed to modify puzzle editor screen (no 2nd match)!");
				throw new Exception();
			}
		}else{
			Console.WriteLine("Failed to modify puzzle editor screen (no body)!");
			throw new Exception();
		}
	}

	public static void PatchJournalScreen(MethodDefinition method, CustomAttribute attrib){
		MonoModRule.Modder.Log("Patching journal screen");
		if(method.HasBody){
			ILCursor cursor = new(new ILContext(method));
			if(cursor.TryGotoNext(MoveType.Before, instr => instr.MatchLdstr("The Journal of Alchemical Engineering"))){
				cursor.Remove();
				TypeDefinition holder = MonoModRule.Modder.FindType("JournalScreen").Resolve();
				MethodDefinition to = holder.Methods.First(m => m.Name.Equals("CurrentJournalName"));
				cursor.Emit(OpCodes.Call, to);
			}else{
				Console.WriteLine("Failed to modify journal screen (no match)!");
				throw new Exception();
			}
		}else{
			Console.WriteLine("Failed to modify journal screen (no body)!");
			throw new Exception();
		}
	}

	public static void PatchJournalPuzzleBackgrounds(MethodDefinition method, CustomAttribute attrib){
		MonoModRule.Modder.Log("Patching journal screen puzzle backgrounds");
		if(method.HasBody){
			ILCursor cursor = new(new ILContext(method));
			if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchStloc(1))){
				cursor.Emit(OpCodes.Ldloc_1);
				cursor.Emit(OpCodes.Ldarg_3);
				TypeDefinition holder = MonoModRule.Modder.FindType("JournalScreen").Resolve();
				MethodDefinition to = holder.Methods.First(m => m.Name.Equals("CurrentJournalBg"));
				cursor.Emit(OpCodes.Call, to);
				cursor.Emit(OpCodes.Stloc_1);
			}else{
				Console.WriteLine("Failed to modify journal screen puzzle backgrounds (no match)!");
				throw new Exception();
			}
		}else{
			Console.WriteLine("Failed to modify journal screen puzzle backgrounds (no body)!");
			throw new Exception();
		}
	}
}