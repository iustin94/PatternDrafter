namespace PatternDrafter.Library.Pant;

public class JoggersDrafter : PatternDrafter
{
    private PantMeasurements Measurements;
    private PantOptions Options;

    public JoggersDrafter(BodyMeasurements measurements, PantOptions options)
        : base(measurements, options)
    {
        Measurements = new PantMeasurements(measurements);
        Options = options;
    }

    public override Pattern DraftPattern()
    {
        var pattern = new Pattern("Pant");

        // Draft torso
        var pantDrafter = new PantDrafter(Measurements, Options);
        var pant = pantDrafter.Draft();
        foreach (var piece in pant)
        {
            pattern.AddPiece(piece);
        }

        foreach (var hemLine in pantDrafter.HemLines())
        {
            var hem = new HemFeature(hemLine);
            var hemPieces = hem.Draft();
            foreach (var piece in hemPieces)
            {
                pattern.AddPiece(piece);
            }
        }

        return pattern;
    }
}
