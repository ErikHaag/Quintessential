using System;

namespace Quintessential.Internal;

public class QuintessentialAsMod : QuintessentialMod {

	public override Type SettingsType => typeof(QuintessentialSettings);

	public override void Load() { }

    public override void LoadPuzzleContent()
    {
        #region Puzzle Payload Handlers
        QApi.AddPuzzlePayloadHandler("Quintessential:dummy", (puzzle, data) =>
        {
            // do nothing
        });

        QApi.AddPuzzlePayloadHandler("Quintessential:log", (puzzle, data) =>
        {
            Logger.Log(data);
        });
        #endregion
        #region Solution Payload Handlers
        QApi.AddSolutionPayloadHandler("Quintessential:dummy", (solution, data) =>
        {
            // do nothing
        });

        QApi.AddSolutionPayloadHandler("Quintessential:log", (solution, data) =>
        {
            Logger.Log(data);
        });
        // Q,R,Turns,PartID
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
                // place equalibrium instead
                partType = class_191.field_1782;
            }

            Part part = new(partType, false);
            solution.method_1939(part, position);
            part.method_1197(solution, rotation);
        });
        #endregion
    }

    public override void PostLoad() { }

	public override void Unload() { }
}