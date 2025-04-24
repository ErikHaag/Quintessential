
using MonoMod;

class patch_Editor{

    [PatchEditor, MonoModIgnore]
    public extern void method_927(AtomType param_4580, Vector2 param_4581, float param_4582, float param_4583, float param_4584, float param_4585, float param_4586, float param_4587, class_256 param_4588, class_256 param_4589, bool param_4590);


    internal class_126 GetAtomLighting(class_8 mT)
    {
        patch_MetalTextures pMT = (patch_MetalTextures)(object)mT;
        if (pMT.atomLighting is null)
        {
            return class_238.field_1989.field_81.field_610;
        }
        return pMT.atomLighting;
    }
}