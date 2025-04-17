namespace PatternDrafter.Library;

public class TorsoMeasurements: PieceMeasurements
{
    // Common derived measurements
    public double QuarterChest;
    public double QuarterWaist;
    public double BackNeckToWaist;
    public double QuarterHip;
    public double NeckWidth;
    public double NeckDepth;
    public double ScyeDepth;
    public double SleeveLength;
    public double ShoulderWidth;
    public double FinishedLength;
    public double HalfBack { get; set; }
    public double WristMeasurement { get; set; }

    public TorsoMeasurements(BodyMeasurements measurements, PieceOptions options): base(measurements)
    {
        QuarterChest = measurements.ChestCircumference / 4;
        QuarterWaist = measurements.WaistCircumference / 4;
        BackNeckToWaist = measurements.BackNeckToWaist;
        ShoulderWidth = measurements.Shoulder/2;
        ScyeDepth = measurements.ScyeDepth;
        SleeveLength = options.Short ? measurements.ArmLength / 3 : measurements.ArmLength;
        NeckWidth = measurements.NeckCircumference / 5;
        FinishedLength = 70 ;
        HalfBack = measurements.HalfBack;
        WristMeasurement = measurements.WristCircumference;
        
    }
}
