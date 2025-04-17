namespace PatternDrafter.Library.Pant;

/// <summary>
/// Options for customizing pant pattern drafting
/// </summary>
public class PantOptions : PieceOptions
{
    /// <summary>
    /// The style of the pants (straight, tapered, boot cut, etc.)
    /// </summary>
    public PantStyle Style { get; set; }

    /// <summary>
    /// Whether to add back pockets
    /// </summary>
    public bool BackPockets { get; set; }

    /// <summary>
    /// Whether to add front pockets
    /// </summary>
    public bool FrontPockets { get; set; }

    /// <summary>
    /// Whether to include a waistband pattern
    /// </summary>
    public bool IncludeWaistband { get; set; }

    /// <summary>
    /// Width of the waistband in centimeters
    /// </summary>
    public double WaistbandWidth { get; set; }

    /// <summary>
    /// Flag for using looser fit with more ease
    /// </summary>
    public bool EasyFit { get; set; }

    /// <summary>
    /// Create new PantOptions with default values
    /// </summary>
    public PantOptions()
    {
        Style = PantStyle.Straight;
        BackPockets = true;
        FrontPockets = true;
        IncludeWaistband = true;
        WaistbandWidth = 4.0; // 4cm waistband
        EasyFit = false; // Standard fit by default
    }
}
