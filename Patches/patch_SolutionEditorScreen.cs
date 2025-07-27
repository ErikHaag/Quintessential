using MonoMod;
using System.Collections.Generic;

class patch_SolutionEditorScreen
{
    public static HexIndex method_2130(Solution param_5729, Part param_5730)
    {
        // not the prettiest, but now it won't crash!
        HexIndex primaryResut = new(0, 0);
        HexIndex secondaryResult = new(0, 0);
        static int dot(HexIndex h1, HexIndex h2) => ((2 * h1.Q + h1.R) * (2 * h2.Q + h2.R) + 3 * h1.R + h2.R) >> 2;
        int maxDistance = 0;

        foreach (HexIndex offset in method_2131(param_5729, param_5730))
        {
            bool alignedFlag = offset.Q == 0 || offset.R == 0 || offset.ImpliedS == 0;
            bool fartherFlag = offset.Length() > primaryResut.Length();
            bool rightmost = offset.Q > primaryResut.Q;
            if (alignedFlag && (fartherFlag || rightmost))
            {
                primaryResut = offset;
            }
            for (int i = 0; i < 3; i++)
            {
                HexIndex dir = HexIndex.AdjacentOffsets[i];
                int dist = dot(dir, offset);
                int absDist = dist > 0 ? dist : -dist;
                if (absDist > maxDistance)
                {
                    secondaryResult = new HexIndex(dist * dir.Q, dist * dir.R);
                    maxDistance = absDist;
                }
            }
        }
        if (maxDistance == 0)
        {
            throw new class_266("Failed to find a handle location for this glyph");
        }
        if (primaryResut.Q == 0 && primaryResut.R == 0)
        {
            return secondaryResult;
        }
        return primaryResut;
    }

    [MonoMod.MonoModIgnore]
    public static extern HashSet<HexIndex> method_2131(Solution param_5731, Part param_5732);

}