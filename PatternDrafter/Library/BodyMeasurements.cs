namespace PatternDrafter.Library;

public struct BodyMeasurements
{
    // Primary measurements
    public double ChestCircumference;
    public double WaistCircumference;
    public double BackNeckToWaist { get; set; }
    public double WristCircumference { get; set; }
    public double Seat { get; set; }
    public double CrothRise { get; set; }
    public double InsideLegLength { get; set; }
    public double Ankle { get; set; }

    public double Shoulder;
    public double NeckCircumference;
    public double ArmLength;
    public double ScyeDepth;
    public double HalfBack;

    // Constructor for convenience
    public BodyMeasurements(
        double chest, 
        double waistCircumference, 
        double hipCircumference, 
        double shoulder,
        double shoulderToWaist, 
        double neckCircumference, 
        double armLength,
        double scyeDepth, 
        double wristCircumference, 
        double backNeckToWaist,
        double halfBack,
        double seat,
        double crothRise,
        double insideLegLength,
        double ankle)
    {
        ChestCircumference = chest;
        WaistCircumference = waistCircumference;
        Shoulder = shoulder;
        NeckCircumference = neckCircumference;
        ArmLength = armLength;
        ScyeDepth = scyeDepth;
        HalfBack = hipCircumference - shoulderToWaist;
        WristCircumference = wristCircumference;
        BackNeckToWaist = backNeckToWaist;
        HalfBack = halfBack;
        Seat = seat;
        CrothRise = crothRise;
        InsideLegLength = insideLegLength;
        Ankle = ankle;
    }
}