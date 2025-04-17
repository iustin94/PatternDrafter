namespace PatternDrafter.Library;

public class ShirtDrafter : PatternDrafter
{
    private TorsoMeasurements Measurements;
    private PieceOptions Options;
    
    public ShirtDrafter(BodyMeasurements measurements, PieceOptions options) 
        : base(measurements, options)
    {
        Measurements = new TorsoMeasurements(measurements, options);
        Options = options;
    }
    
    public override Pattern DraftPattern()
    {
        var pattern = new Pattern("T-Shirt");
        
        // Draft torso
        var torsoDrafter = new TorsoDrafter(Measurements, Options);
        var torso = torsoDrafter.Draft();
        foreach (var piece in torso)
        {
            pattern.AddPiece(piece);
        }
        foreach (var hemLine in torsoDrafter.HemLines())
        {
            var hem = new HemFeature(hemLine);
            var hemPieces = hem.Draft();
            foreach (var piece in hemPieces)
            {
                pattern.AddPiece(piece);
            }
        }
        
        // Draft sleeve
        var sleeveDrafter = new SleeveDrafter(Measurements.Body, Options, torsoDrafter.ArmholeDiameter, 25);
        var sleeve = sleeveDrafter.Draft();
        foreach (var piece in sleeve)
        {
            pattern.AddPiece(piece);
        }

        foreach (var hemLine in sleeveDrafter.HemLines())
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
