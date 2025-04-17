namespace PatternDrafter.Library.Pant
{
    /// <summary>
    /// Contains measurements needed for drafting pant patterns
    /// </summary>
    public class PantMeasurements: PieceMeasurements
    {
        public double Waist => Body.WaistCircumference;
        public double Seat => Body.Seat;
        public double Rise => Body.CrothRise;
        public double InsideLeg => Body.InsideLegLength;
        public double Ankle => Body.Ankle;
        
        public PantMeasurements(BodyMeasurements measurements): base(measurements)
        {
        }
    }
    
    public enum PantStyle
    {
        Straight,
        Tapered,
        Wide,
        BootCut,
        Skinny
    }
}