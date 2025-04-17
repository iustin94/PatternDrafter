namespace PatternDrafter.Library;

/// <summary>
/// Contains all landmark points for pant pattern pieces
/// </summary>
public class PantLandmarks : PatternLandmarks
{
    // Front Section Points (0-15)
    public PatternPoint Point0 { get; set; }  // Origin
    public PatternPoint Point1 { get; set; }  // Body rise
    public PatternPoint Point2 { get; set; }  // Bottom of leg
    public PatternPoint Point3 { get; set; }  // Knee line
    public PatternPoint Point4 { get; set; }  // Hip line
    public PatternPoint Point5 { get; set; }  // Point on hip line
    public PatternPoint Point6 { get; set; }  // Top front
    public PatternPoint Point7 { get; set; }  // Front crotch curve point
    public PatternPoint Point8 { get; set; }  // Front crotch point
    public PatternPoint Point9 { get; set; }  // Center leg line
    public PatternPoint Point10 { get; set; } // Point on center leg
    public PatternPoint Point11 { get; set; } // Bottom of center line
    public PatternPoint Point12 { get; set; } // Bottom of front
    public PatternPoint Point13 { get; set; } // Inner ankle front
    public PatternPoint Point14 { get; set; } // Knee point on side
    public PatternPoint Point15 { get; set; } // Knee point on inside

    // Back Section Points (16-25)
    public PatternPoint Point16 { get; set; } // Back waist point
    public PatternPoint Point17 { get; set; } // Top back
    public PatternPoint Point18 { get; set; } // Back center waist
    public PatternPoint Point19 { get; set; } // Back crotch curve point
    public PatternPoint Point20 { get; set; } // Back crotch extension
    public PatternPoint Point21 { get; set; } // Back crotch point
    public PatternPoint Point22 { get; set; } // Back ankle inside
    public PatternPoint Point23 { get; set; } // Back bottom
    public PatternPoint Point24 { get; set; } // Back knee side
    public PatternPoint Point25 { get; set; } // Back knee inside

    // Additional Feature Points
    public PatternPoint FrontGrainLineTop { get; set; }
    public PatternPoint FrontGrainLineBottom { get; set; }
    public PatternPoint BackGrainLineTop { get; set; }
    public PatternPoint BackGrainLineBottom { get; set; }

    public override IEnumerable<PatternPoint> GetAllLandmarkPoints()
    {
        var points = new List<PatternPoint>();
        
        // Add all landmark points that are not null
        if (Point0 != null) points.Add(Point0);
        if (Point1 != null) points.Add(Point1);
        if (Point2 != null) points.Add(Point2);
        if (Point3 != null) points.Add(Point3);
        if (Point4 != null) points.Add(Point4);
        if (Point5 != null) points.Add(Point5);
        if (Point6 != null) points.Add(Point6);
        if (Point7 != null) points.Add(Point7);
        if (Point8 != null) points.Add(Point8);
        if (Point9 != null) points.Add(Point9);
        if (Point10 != null) points.Add(Point10);
        if (Point11 != null) points.Add(Point11);
        if (Point12 != null) points.Add(Point12);
        if (Point13 != null) points.Add(Point13);
        if (Point14 != null) points.Add(Point14);
        if (Point15 != null) points.Add(Point15);
        if (Point16 != null) points.Add(Point16);
        if (Point17 != null) points.Add(Point17);
        if (Point18 != null) points.Add(Point18);
        if (Point19 != null) points.Add(Point19);
        if (Point20 != null) points.Add(Point20);
        if (Point21 != null) points.Add(Point21);
        if (Point23 != null) points.Add(Point23);
        if (Point24 != null) points.Add(Point24);
        if (Point22 != null) points.Add(Point22);
        if (Point25 != null) points.Add(Point25);
        
        return points;
    }
    
    public override IEnumerable<PatternPoint> GetAllRelativePoints()
    {
        var points = new List<PatternPoint>();
        
        if (FrontGrainLineTop != null) points.Add(FrontGrainLineTop);
        if (FrontGrainLineBottom != null) points.Add(FrontGrainLineBottom);
        if (BackGrainLineTop != null) points.Add(BackGrainLineTop);
        if (BackGrainLineBottom != null) points.Add(BackGrainLineBottom);
        
        return points;
    }
}