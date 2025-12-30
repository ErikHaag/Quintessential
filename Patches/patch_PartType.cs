using System;
using System.Collections.Generic;
using MonoMod;

[MonoModPatch("class_139")]
class patch_PartType{
	
	// When non-null, the predicate is run on the puzzle's set of custom permissions to check that the part is allowed
	public Predicate<HashSet<string>> CustomPermissionCheck;

	// When true, this part type can't be removed from the board, use this with field_1552 to prevent cloning this part
	public bool IsForced = false;
}