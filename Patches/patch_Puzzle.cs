using System.Collections.Generic;
using System.IO;
using MonoMod;
using Quintessential;
using Quintessential.Serialization;

using Conduit = class_117;

class patch_Puzzle{
	
	// Custom puzzle data
	public HashSet<string> CustomPermissions = new();
	
	// Is modded content allowed in this puzzle?
	// Controls whether this is saved to/from a vanilla `.puzzle` file, or a Quintessential `.puzzle.yaml` file
	// Don't set this if you don't know what you're doing!
	public bool IsModdedPuzzle = false;

	public Maybe<Conduit[]> EngineConduits = struct_18.field_1431;
	public Maybe<Payloads> Payloads = struct_18.field_1431;

	public bool CanSave { get
		{
			// we can save if the puzzle has no payloads, or doesn't have any puzzle initializers.
			return !this.Payloads.method_99(out var p) || p.PuzzleInitialization.Count == 0;
		} }

	// Save using the right format, and set Steam user ID to 0
	[PatchPuzzleIdWrite]
	public extern void orig_method_1248(string path);

	// Save .puzzle or .puzzle.yaml
	public void method_1248(string path){
		if(IsModdedPuzzle)
			File.WriteAllText(path, YamlHelper.Serializer.Serialize(PuzzleModel.FromPuzzle((Puzzle)(object)this)));
		else
			orig_method_1248(path);
	}

	public static extern Puzzle orig_method_1249(string path);
	public static Puzzle method_1249(string path){
		if(Path.GetExtension(path) == ".yaml"){
			Puzzle p = PuzzleModel.FromModel(YamlHelper.Deserializer.Deserialize<PuzzleModel>(File.ReadAllText(path)));
			((patch_Puzzle)(object)p).IsModdedPuzzle = true;
			if (((patch_Puzzle)(object)p).Payloads.method_99(out Payloads payloads))
			{
				foreach (Payloads.Payload payload in payloads.PuzzleInitialization)
				{
					foreach (Pair<string, PuzzlePayloadHandler> handler in QApi.PuzzlePayloadHandlers)
					{
						if (handler.Left.Equals(payload.Address))
						{
							handler.Right(p, payload.Data);
						}
					}
				}
			}
			return p;
		}
		return orig_method_1249(path);
	}

	public void ConvertFormat(bool modded){
		if (this.IsModdedPuzzle && !this.CanSave)
		{
			return;
		}
		Puzzle self = (Puzzle)(object)this;
		WorkshopManager wm = GameLogic.field_2434.field_2460;
		// delete
		File.Delete(((patch_WorkshopManager)(object)wm).method_2237(self));
		// update
		IsModdedPuzzle = modded;
		// save
		wm.method_2241(self);
	}
}