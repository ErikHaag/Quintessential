using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.InlineRT;
using Quintessential;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoMod;

[MonoModCustomMethodAttribute(nameof(MonoModRules.PatchSettingsStaticInit))]
class PatchSettingsStaticInit : Attribute { }

[MonoModCustomMethodAttribute(nameof(MonoModRules.PatchPuzzleIdWrite))]
class PatchPuzzleIdWrite : Attribute { }

[MonoModCustomMethodAttribute(nameof(MonoModRules.PatchScoreManagerLoad))]
class PatchScoreManagerLoad : Attribute { }

[MonoModCustomMethodAttribute(nameof(MonoModRules.PatchGifRecorderFrame))]
class PatchGifRecorderFrame : Attribute { }

[MonoModCustomMethodAttribute(nameof(MonoModRules.PatchMoleculeEditorScreenAtomTray))]
class PatchMoleculeEditorScreenAtomTray : Attribute { }

[MonoModCustomMethodAttribute(nameof(MonoModRules.PatchMoleculeEditorScreenMoleculeError))]
class PatchMoleculeEditorScreenMoleculeError : Attribute { }

[MonoModCustomMethodAttribute(nameof(MonoModRules.PatchPuzzleEditorScreen))]
class PatchPuzzleEditorScreen : Attribute { }

[MonoModCustomMethodAttribute(nameof(MonoModRules.PatchJournalScreen))]
class PatchJournalScreen : Attribute { }

[MonoModCustomMethodAttribute(nameof(MonoModRules.PatchJournalPuzzleBackgrounds))]
class PatchJournalPuzzleBackgrounds : Attribute { }

[MonoModCustomMethodAttribute(nameof(MonoModRules.PatchSolutionInitializer))]
class PatchSolutionInitializer : Attribute { }

[MonoModCustomMethodAttribute(nameof(MonoModRules.PatchGlyphEffectConstructor))]
class PatchGlyphEffectConstructor : Attribute { }

[MonoModCustomMethodAttribute(nameof(MonoModRules.PatchGlyphEffectRenderer))]
class PatchGlyphEffectRenderer : Attribute { }

static class MonoModRules
{

    static MonoModRules()
    {
        MonoModRule.Modder.Log("Patching OM");
    }

    public static void PatchSettingsStaticInit(MethodDefinition method, CustomAttribute attrib)
    {
        MonoModRule.Modder.Log("Patching settings static init");
        // Set "class_110.field_1012" (Steam support) to false
        if (method.HasBody)
        {
            ILCursor cursor = new(new ILContext(method));
            if (cursor.TryGotoNext(MoveType.Before,
                   instr => instr.MatchLdcI4(1),
                   instr => instr.MatchStsfld("class_110", "field_1012")))
            {
                cursor.Remove();
                cursor.Emit(OpCodes.Ldc_I4_0);
            }
            else
            {
                Console.WriteLine("Failed to disable Steam setting in class_110!");
                throw new Exception();
            }
        }
        else
        {
            Console.WriteLine("Failed to disable Steam setting in class_110!");
            throw new Exception();
        }
    }

    public static void PatchPuzzleIdWrite(MethodDefinition method, CustomAttribute attrib)
    {
        MonoModRule.Modder.Log("Patching puzzle ids");
        // Replace "SteamUser.GetSteamID().m_SteamID" with "0" (until a proper format is created)
        if (method.HasBody)
        {
            ILCursor cursor = new(new ILContext(method));
            if (cursor.TryGotoNext(MoveType.Before,
                   instr => instr.MatchCall("Steamworks.SteamUser", "GetSteamID"),
                   instr => instr.MatchLdfld("Steamworks.CSteamID", "m_SteamID")))
            {
                cursor.Remove();
                cursor.Remove();
                cursor.Emit(OpCodes.Ldc_I8, (long)0);
            }
        }
        else
        {
            Console.WriteLine("Failed to modify puzzle serialization!");
            throw new Exception();
        }
    }

    public static void PatchScoreManagerLoad(MethodDefinition method, CustomAttribute attrib)
    {
        MonoModRule.Modder.Log("Patching ScoreManager loading");
        if (method.HasBody)
        {
            ILCursor cursor = new(new ILContext(method));
            if (cursor.TryGotoNext(MoveType.After, instr => instr.Match(OpCodes.Brfalse_S))
               && cursor.TryGotoNext(MoveType.After, instr => instr.Match(OpCodes.Brfalse_S))
               && cursor.TryGotoNext(MoveType.After, instr => instr.Match(OpCodes.Brfalse_S)))
            {
                cursor.Emit(OpCodes.Ret);
            }
            else
            {
                Console.WriteLine("Failed to modify ScoreManager loading (no match)!");
                throw new Exception();
            }
        }
        else
        {
            Console.WriteLine("Failed to modify ScoreManager loading (no body)!");
            throw new Exception();
        }
    }

    public static void PatchGifRecorderFrame(MethodDefinition method, CustomAttribute attrib)
    {
        MonoModRule.Modder.Log("Patching GIF recorder frame rendering");
        if (method.HasBody)
        {
            ILCursor cursor = new(new ILContext(method));
            if (cursor.TryGotoNext(MoveType.After, instr => instr.MatchCall("class_135", "method_272")))
            {
                // "class_135.method_272(class_238.field_1989.field_81.field_613.field_632, new Vector2());"
                TypeDefinition holder = MonoModRule.Modder.FindType("class_250").Resolve();
                MethodDefinition to = holder.Methods.First(m => m.Name.Equals("MarkOnFrame"));
                cursor.Emit(OpCodes.Call, to);
            }
            else
            {
                Console.WriteLine("Failed to modify GIF recorder frame rendering (no match)!");
                throw new Exception();
            }
        }
        else
        {
            Console.WriteLine("Failed to modify GIF recorder frame rendering (no body)!");
            throw new Exception();
        }
    }

    public static void PatchMoleculeEditorScreenAtomTray(MethodDefinition method, CustomAttribute attrib)
    {
        MonoModRule.Modder.Log("Patching molecule editor screen atom tray");
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

        TypeDefinition holder = MonoModRule.Modder.FindType("MoleculeEditorScreen").Resolve();
        MethodDefinition to = holder.Methods.First(m => m.Name.Equals("DrawAtoms"));

        cursor.Goto(start);
        cursor.RemoveRange(end - start); // Go bye with you
        cursor.Emit(OpCodes.Ldarg_0); // this
        cursor.Emit(OpCodes.Ldloc, 7);
        cursor.Emit(OpCodes.Ldloc, 6);
        cursor.Emit(OpCodes.Callvirt, to);
    }

    public static void PatchMoleculeEditorScreenMoleculeError(MethodDefinition method, CustomAttribute attrib)
    {
        MonoModRule.Modder.Log("Patching molecule editor screen error detector");
        if (!method.HasBody)
        {
            Console.WriteLine("Failed to modify molecule editor error detector (no body)!");
            throw new Exception();
        }
        ILCursor cursor = new(new ILContext(method)); // Create cursor

        if (!cursor.TryGotoNext(MoveType.After,
            instr => instr.MatchLdarg(0),
            instr => instr.MatchLdfld(out FieldReference f) && f.Name == "field_2656",
            instr => instr.OpCode == OpCodes.Callvirt,
            instr => instr.OpCode == OpCodes.Callvirt,
            instr => instr.MatchStloc(8),
            instr => instr.OpCode == OpCodes.Br
        ))
        {
            Console.WriteLine("Failed to modify molecule editor error detector (no bond checker)!");
            throw new Exception();
        }
        Instruction start = cursor.Previous;
        cursor.Goto((Instruction)cursor.Previous.Operand);

        if (!cursor.TryGotoNext(MoveType.After,
            instr => instr.OpCode == OpCodes.Brtrue,
            instr => instr.OpCode == OpCodes.Leave_S
        ))
        {
            Console.WriteLine("Failed to modify molecule editor error detector (no last leave)");
            throw new Exception();
        }
        Instruction end = cursor.Previous;
        // Immediately leaves the loop
        start.Operand = end;
    }

    public static void PatchPuzzleEditorScreen(MethodDefinition method, CustomAttribute attrib)
    {
        MonoModRule.Modder.Log("Patching puzzle editor screen");
        if (!method.HasBody)
        {
            Console.WriteLine("Failed to modify puzzle editor screen (no body)!");
            throw new Exception();
        }

        ILCursor cursor = new(new ILContext(method));
        Instruction target = null; // will definitely be set

        // kill off `flag5` and make the Upload puzzle button never clickable
        if (!cursor.TryGotoNext(MoveType.Before, instr => instr.MatchLdloc(27)))
        {
            Console.WriteLine("Failed to modify puzzle editor screen (no 1st match)!");
            throw new Exception();
        }

        cursor.Remove();
        cursor.Emit(OpCodes.Ldc_I4_0);

        if (!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdflda("PuzzleEditorScreen", "field_2789"),
                instr => instr.MatchCall(out MethodReference mref) && mref.Name.Equals("method_1085"),
                instr =>
                {
                    bool ret = instr.OpCode == OpCodes.Brfalse;
                    if (ret)
                        target = (Instruction)instr.Operand;
                    return ret;
                }))
        {
            Console.WriteLine("Failed to modify puzzle editor screen (no 2nd match)!");
            throw new Exception();
        }

        // "if(this.field_2789.method_1085()){" ... "} if (!this.field_2789.method_1085()){"
        TypeDefinition holder = MonoModRule.Modder.FindType("PuzzleEditorScreen").Resolve();
        MethodDefinition to = holder.Methods.First(m => m.Name.Equals("DisplayEditorPanelScreen"));
        cursor.Emit(OpCodes.Ldarg_0); // this
        cursor.Emit(OpCodes.Call, to); // call reimplementation
        cursor.Emit(OpCodes.Br, target); // skip rest of `if` statement

        // Carriage Return
        cursor.Index = 0;
        // Ding!

        if (!cursor.TryGotoNext(MoveType.Before,
            instr => instr.MatchLdfld(out FieldReference fr) && fr.Name == "field_2767",
            instr => instr.MatchCall(out MethodReference mr) && mr.Name == "op_Implicit",
            instr => instr.MatchLdloc(13)
            ))
        {
            Console.WriteLine("Failed to modify puzzle editor screen (no puzzle name found)");
            throw new Exception();
        }
        cursor.RemoveRange(2);

        if (!cursor.TryGotoNext(MoveType.Before,
            instr => instr.OpCode == OpCodes.Call,
            instr => instr.MatchStloc(18),
            instr => instr.MatchLdloca(18)))
        {
            Console.WriteLine("Failed to modify puzzle editor screen (no ButtonDrawLogic instantiation)");
            throw new Exception();
        }

        cursor.RemoveRange(3);

        if (!cursor.TryGotoNext(MoveType.Before,
            instr => instr.OpCode == OpCodes.Call,
            instr => instr.OpCode == OpCodes.Brfalse,
            instr => instr.MatchLdloc(16)))
        {
            Console.WriteLine("Failed to modify puzzle editor screen (no ButtonDrawLogic call)");
            throw new Exception();
        }

        MethodDefinition getName = holder.Methods.First(m => m.Name.Equals("DrawPuzzleButton"));
        cursor.Remove();
        cursor.Emit(OpCodes.Call, getName);
    }

    public static void PatchJournalScreen(MethodDefinition method, CustomAttribute attrib)
    {
        MonoModRule.Modder.Log("Patching journal screen");
        if (method.HasBody)
        {
            ILCursor cursor = new(new ILContext(method));
            if (cursor.TryGotoNext(MoveType.Before, instr => instr.MatchLdstr("The Journal of Alchemical Engineering")))
            {
                cursor.Remove();
                TypeDefinition holder = MonoModRule.Modder.FindType("JournalScreen").Resolve();
                MethodDefinition to = holder.Methods.First(m => m.Name.Equals("CurrentJournalName"));
                cursor.Emit(OpCodes.Call, to);
            }
            else
            {
                Console.WriteLine("Failed to modify journal screen (no match)!");
                throw new Exception();
            }
        }
        else
        {
            Console.WriteLine("Failed to modify journal screen (no body)!");
            throw new Exception();
        }
    }

    public static void PatchJournalPuzzleBackgrounds(MethodDefinition method, CustomAttribute attrib)
    {
        MonoModRule.Modder.Log("Patching journal screen puzzle backgrounds");
        if (method.HasBody)
        {
            ILCursor cursor = new(new ILContext(method));
            if (cursor.TryGotoNext(MoveType.After, instr => instr.MatchStloc(1)))
            {
                cursor.Emit(OpCodes.Ldloc_1);
                cursor.Emit(OpCodes.Ldarg_3);
                TypeDefinition holder = MonoModRule.Modder.FindType("JournalScreen").Resolve();
                MethodDefinition to = holder.Methods.First(m => m.Name.Equals("CurrentJournalBg"));
                cursor.Emit(OpCodes.Call, to);
                cursor.Emit(OpCodes.Stloc_1);
            }
            else
            {
                Console.WriteLine("Failed to modify journal screen puzzle backgrounds (no match)!");
                throw new Exception();
            }
        }
        else
        {
            Console.WriteLine("Failed to modify journal screen puzzle backgrounds (no body)!");
            throw new Exception();
        }
    }

    public static void PatchSolutionInitializer(MethodDefinition method, CustomAttribute attrib)
    {
        MonoModRule.Modder.Log("Patching solution initializer");
        if (!method.HasBody)
        {
            Console.WriteLine("Failed to modify solution initializer (no body)!");
            throw new Exception();
        }

        ILCursor cursor = new(new ILContext(method));

        if (!cursor.TryGotoNext(MoveType.Before,
           instr => instr.MatchLdarg(0),
           instr => instr.OpCode == OpCodes.Ldfld,
           instr => instr.MatchLdloca(1),
           instr => instr.MatchCall(out MethodReference m) && m.ReturnType.Name == "Boolean",
           instr => instr.OpCode == OpCodes.Brfalse))
        {
            Console.WriteLine("Failed to modify solution initializer (no production info branch)");
            throw new Exception();
        }
        int begin = cursor.Index;
        Instruction ifEnd = (Instruction)cursor.Instrs[cursor.Index + 4].Operand;
        if (!cursor.TryGotoNext(MoveType.After,
            instr => instr.MatchLdloc(1),
            instr => instr.OpCode == OpCodes.Ldfld,
            instr => instr.MatchStloc(3)
        ))
        {
            Console.WriteLine("Failed to modify solution initializer (no conduit assignment)");
            throw new Exception();
        }
        int end = cursor.Index;
        cursor.Index = begin;
        cursor.RemoveRange(end - begin);

        TypeDefinition holder = MonoModRule.Modder.FindType("Solution").Resolve();
        MethodDefinition to = holder.Methods.First(m => m.Name.Equals("GetConduits"));
        // Puzzle
        cursor.Emit(OpCodes.Ldarg_0);
        // conduit list address
        cursor.Emit(OpCodes.Ldloca, 3);
        // Solution.GetConduits
        cursor.Emit(OpCodes.Call, to);
        // if body skipping
        cursor.Emit(OpCodes.Brfalse, ifEnd);
        Instruction branch = cursor.Prev;
        // assign the first conduit's ID to 100
        cursor.Emit(OpCodes.Ldc_I4, 100);
        cursor.Emit(OpCodes.Stloc, 2);

        // jump to end of if statement
        if (!cursor.TryGotoNext(instr => instr == ifEnd))
        {
            Console.WriteLine("Failed to modify solution initializer (no end of if body)");
            throw new Exception();
        }
        to = holder.Methods.First(m => m.Name.Equals("ApplyChanges"));

        // Why does cursor.MoveAfterLabels not work like I expect?
        cursor.Emit(OpCodes.Ldarg_0);
        branch.Operand = cursor.Prev;
        cursor.Emit(OpCodes.Ldloc_0);
        cursor.Emit(OpCodes.Call, to);

    }

    public static void PatchGlyphEffectConstructor(MethodDefinition method, CustomAttribute attrib)
    {
        MonoModRule.Modder.Log("Patching glyph effect (1/2)");

        if (!method.HasBody)
        {
            Console.WriteLine("Unable to patch glyph effect constructor (no body)");
            throw new Exception();
        }

        ILCursor gremlin = new(new ILContext(method));

        if (!gremlin.TryGotoNext(MoveType.Before,
            instr => instr.MatchRet()
        ))
        {
            Console.WriteLine("Unable to patch glyph effect constructor (no return)");
            throw new Exception();
        }

        TypeDefinition holder = MonoModRule.Modder.FindType("class_228").Resolve();
        FieldDefinition colorProp = holder.Fields.First((f) => f.Name == "Color");

        holder = MonoModRule.Modder.FindType("Color").Resolve();
        FieldDefinition colorWhite = holder.Fields.First((f) => f.IsStatic && f.Name == "White");


        gremlin.Emit(OpCodes.Ldarg_0);
        gremlin.Emit(OpCodes.Ldsfld, colorWhite);
        gremlin.Emit(OpCodes.Stfld, colorProp);

    }

    public static void PatchGlyphEffectRenderer(MethodDefinition method, CustomAttribute attrib)
    {
        MonoModRule.Modder.Log("Patching glyph effect (2/2)");

        if (!method.HasBody)
        {
            Console.WriteLine("Unable to patch glyph effect renderer (no body)");
            throw new Exception();
        }

        ILCursor gremlin = new(new ILContext(method));
       
        TypeDefinition holder = MonoModRule.Modder.FindType("Color").Resolve();
        FieldDefinition colorWhite = holder.Fields.First((f) => f.IsStatic && f.Name == "White");




        if (!gremlin.TryGotoNext(MoveType.Before,
            instr =>
            {
                FieldReference testOperand = instr.Operand as FieldReference;
                return instr.OpCode == OpCodes.Ldsfld && testOperand.FieldType == colorWhite.FieldType && testOperand.Name == colorWhite.Name;
            },
            instr => instr.MatchLdloc(6),
            instr => instr.OpCode == OpCodes.Call
        ))
        {
            Console.WriteLine("Unable to patch glyph effect renderer (no draw call)");
            throw new Exception();
        }

        holder = MonoModRule.Modder.FindType("class_228").Resolve();
        FieldDefinition colorProp = holder.Fields.First((f) => f.Name == "Color");

        gremlin.Remove();
        gremlin.Emit(OpCodes.Ldloc, 1);
        gremlin.Emit(OpCodes.Ldfld, colorProp);
    }
}