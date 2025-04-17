namespace PatternDrafter.Library;

public class SleeveMeasurements: PieceMeasurements
{
    public BodyMeasurements Body { get; }
    public double SleeveLength { get; }

    public SleeveMeasurements(BodyMeasurements measurements, double length): base(measurements)
    {
        Body = measurements;
        SleeveLength = length;
    }
    
}
