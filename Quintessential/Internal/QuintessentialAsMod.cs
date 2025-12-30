using System;

namespace Quintessential.Internal;

public class QuintessentialAsMod : QuintessentialMod {

	public override Type SettingsType => typeof(QuintessentialSettings);

	public override void Load() { }

    public override void LoadPuzzleContent()
    {
        QApi.AddPuzzlePayloadHandler("Quintessential:dummy", (puzzle, data) =>
        {
            // do nothing
        });

        QApi.AddSolutionPayloadHandler("Quintessential:place", (solution, data) =>
        {
            string[] parameters = data.Split(',');
            if (parameters.Length != 4)
            {
                return;
            }
            HexIndex position = new(int.Parse(parameters[0]), int.Parse(parameters[1]));
            HexRotation rotation = new(int.Parse(parameters[2]));

            if (!class_191.method_498(parameters[3]).method_99(out class_139 partType))
            {
                partType = class_191.field_1782;
            }

            Part part = new(partType, false);
            solution.method_1939(part, position);
            part.method_1197(solution, rotation);
        });
    }

    public override void PostLoad() { }

	public override void Unload() { }
}
