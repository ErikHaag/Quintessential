using MonoMod;

class patch_AtomType
{

    // String atom type ID
    public string QuintAtomType;

    [MonoModReplace]
    public override bool Equals(object obj)
    {
        return obj is patch_AtomType type && this == type;
    }

    [MonoModReplace]
    public override int GetHashCode()
    {
        return ((AtomType)(object)this).field_2283.GetHashCode() ^ QuintAtomType.GetHashCode();
    }

    [MonoModReplace]
    public static bool operator ==(patch_AtomType param_4426, patch_AtomType param_4427)
    {
        return (((AtomType)(object)param_4426).field_2283 == ((AtomType)(object)param_4427).field_2283) && string.Equals(param_4426.QuintAtomType, param_4427.QuintAtomType);
    }

    [MonoModReplace]
    public static bool operator !=(patch_AtomType param_122, patch_AtomType param_3693)
    {
        return !(param_122 == param_3693);
    }
}