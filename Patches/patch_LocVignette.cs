using MonoMod;
using Quintessential;
using System.Collections.Generic;
using System.IO;

[MonoModPatch("class_264")]
class patch_LocVignette {

    public struct CutsceneInfo
    {
        public string Setting;
        public string Background;
        public CutsceneInfo(string setting, string background)
        {
            this.Setting = setting;
            this.Background = background;
        }

    }

    public Maybe<CutsceneInfo> csI;

    [MonoModIgnore]
    string field_2090;
    [MonoModIgnore]
    Dictionary<Language, Vignette> field_2091;


    [MonoModConstructor]
	public void ctor(string key) {

		field_2091 = new Dictionary<Language, Vignette>();
		field_2090 = key;
        Language[] languages = {
            Language.English,
            Language.German,
            Language.French,
            Language.Russian,
            Language.Chinese,
            Language.Japanese,
            Language.Spanish,
            Language.Korean,
            Language.Turkish,
            Language.Ukrainian,
            Language.Portuguese,
            Language.Czech
        };
        foreach(Language lang in languages) {
            string path1 = Path.Combine("Content", "vignettes", $"{key}.{class_134.field_1498[lang]}.txt");

            for(int i = 0; i < QuintessentialLoader.ModContentDirectories.Count && !File.Exists(path1); i++) {
                string content = QuintessentialLoader.ModContentDirectories[i];
                path1 = Path.Combine(content, "Content", "vignettes", $"{key}.{class_134.field_1498[Language.English]}.txt");
            }
            
            string text = File.Exists(path1) ? File.ReadAllText(path1) : "";

            field_2091[lang] = new Vignette(text, Path.GetFileNameWithoutExtension(path1), lang);
            if(lang == Language.English) {
                Vignette vignette = new(text, Path.GetFileNameWithoutExtension(path1), Language.Pseudo);
                field_2091[Language.Pseudo] = vignette;
                vignette.field_4124 = class_134.method_249(vignette.field_4124);
                foreach(List<VignetteEvent> vignetteEventList in vignette.field_4125) {
                    for(int index = 0; index < vignetteEventList.Count; ++index) {
                        if(vignetteEventList[index].method_2215()) {
                            VignetteEvent.LineFields lineFields = vignetteEventList[index].method_2218();
                            vignetteEventList[index] = VignetteEvent.method_2212(lineFields.field_4136, class_134.method_249(lineFields.field_4093));
                        }
                    }
                }
            }
        }
    }
}
