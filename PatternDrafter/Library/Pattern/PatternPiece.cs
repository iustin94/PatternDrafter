using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Buffer;
using PatternEngine;

namespace PatternDrafter.Library;

public class PatternPiece
    {
        public string Name { get; private set; }
        public Dictionary<string, PatternPoint> Points { get; } = new Dictionary<string, PatternPoint>();
        public List<PathElement> Paths { get; } = new List<PathElement>();
        public double SeamAllowance { get; private set; } = 1.0;
        public bool CutOnFold { get; private set; }
        public int Quantity { get; private set; } = 1; 
        public List<string> GrainLine { get; } = new List<string>(); // Point names defining grain line
        public Dictionary<string, string> Instructions { get; } = new Dictionary<string, string>();
        
        private readonly GeometryFactory _geometryFactory = new GeometryFactory();
        
        private PatternPiece() { }
        
        // Factory method
        public static PatternPieceBuilder Create() => new PatternPieceBuilder();
        
        public PatternPoint GetPoint(string name)
        {
            if (!Points.ContainsKey(name))
            {
                throw new KeyNotFoundException($"Point '{name}' not found in pattern piece '{Name}'");
            }
            
            return Points[name];
        }
        
        public Polygon GetOutline()
        {
            // Convert paths to a list of LineStrings
            var lineStrings = new List<LineString>();
            
            foreach (var path in Paths)
            {
                if (path.IsVisible && (path.Type == PathType.CutLine || path.Type == PathType.SeamLine))
                {
                    lineStrings.Add((LineString)path.ToGeometry(_geometryFactory));
                }
            }
            
            if (lineStrings.Count == 0)
            {
                return null;
            }
            
            // Try to build a polygon from the linestrings
            try
            {
                var boundaries = _geometryFactory.CreateMultiLineString(lineStrings.ToArray());
                var polygonizer = new NetTopologySuite.Operation.Polygonize.Polygonizer();
                polygonizer.Add(boundaries);
                
                var polygons = polygonizer.GetPolygons();
                if (polygons.Count > 0)
                {
                    // Use the largest polygon as the piece outline
                    return (Polygon)polygons.OrderByDescending(p => p.Area).First();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating outline for {Name}: {ex.Message}");
            }
            
            return null;
        }
        
        // Builder class
        public class PatternPieceBuilder
        {
            private string _name;
            private double _seamAllowance = 1.0;
            private bool _cutOnFold = false;
            private int _quantity = 1;
            private readonly Dictionary<string, PatternPoint> _points = new Dictionary<string, PatternPoint>();
            private readonly List<PathElement> _paths = new List<PathElement>();
            private readonly List<string> _grainLine = new List<string>();
            private readonly Dictionary<string, string> _instructions = new Dictionary<string, string>();

            internal PatternPieceBuilder() { }

            public PatternPieceBuilder WithName(string name)
            {
                _name = name;
                return this;
            }

            public PatternPieceBuilder WithSeamAllowance(double allowance)
            {
                _seamAllowance = allowance;
                return this;
            }

            public PatternPieceBuilder WithCutOnFold(bool cutOnFold)
            {
                _cutOnFold = cutOnFold;
                return this;
            }

            public PatternPieceBuilder WithQuantity(int quantity)
            {
                _quantity = quantity;
                return this;
            }

            public PatternPieceBuilder WithPoint(PatternPoint point)
            {
                _points[point.Name] = point;
                return this;
            }

            public PatternPieceBuilder WithPoints(IEnumerable<PatternPoint> points)
            {
                foreach (var point in points)
                {
                    _points[point.Name] = point;
                }
                return this;
            }

            public PatternPieceBuilder WithPath(PathElement path)
            {
                _paths.Add(path);
                return this;
            }

            public PatternPieceBuilder WithPaths(IEnumerable<PathElement> paths)
            {
                _paths.AddRange(paths);
                return this;
            }

            public PatternPieceBuilder WithGrainLine(params string[] pointNames)
            {
                _grainLine.AddRange(pointNames);
                return this;
            }

            public PatternPieceBuilder WithInstruction(string key, string value)
            {
                _instructions[key] = value;
                return this;
            }

            public PatternPiece Build()
            {
                if (string.IsNullOrEmpty(_name))
                {
                    throw new InvalidOperationException("Pattern piece name is required");
                }

                var patternPiece = new PatternPiece
                {
                    Name = _name,
                    SeamAllowance = _seamAllowance,
                    CutOnFold = _cutOnFold,
                    Quantity = _quantity
                };

                // Add points
                foreach (var point in _points.Values)
                {
                    patternPiece.Points[point.Name] = point;
                }

                // Add paths
                patternPiece.Paths.AddRange(_paths);

                // Add grain line
                patternPiece.GrainLine.AddRange(_grainLine);

                // Add instructions
                foreach (var instruction in _instructions)
                {
                    patternPiece.Instructions[instruction.Key] = instruction.Value;
                }

                return patternPiece;
            }

            public PatternPiece BuildWithSeamAllowance(double allowance = 1.0)
            {
                var piece = Build();
                
                if (allowance <= 0)
                {
                    return piece;
                }
                
                var outline = piece.GetOutline();
                if (outline == null)
                {
                    return piece;
                }
                
                var expandedOutline = (Polygon)outline.Buffer(allowance, NetTopologySuite.Operation.Buffer.EndCapStyle.Round);
                
                // Create new builder for piece with seam allowance
                var newPieceBuilder = Create()
                    .WithName($"{_name} with seam allowance")
                    .WithCutOnFold(_cutOnFold)
                    .WithQuantity(_quantity)
                    .WithSeamAllowance(0); // Already included
                
                // Copy existing points
                foreach (var point in _points.Values)
                {
                    newPieceBuilder.WithPoint(point);
                }
                
                // Add the original outline paths as seam lines
                foreach (var path in _paths)
                {
                    newPieceBuilder.WithPath(ConvertPathToSeamLine(path));
                }
                
                // Add the seam allowance boundary
                var coords = expandedOutline.ExteriorRing.Coordinates;
                var cutPoints = new List<PatternPoint>();
                
                // Create points for the seam allowance
                for (int i = 0; i < coords.Length - 1; i++)
                {
                    var pointName = $"sa_{i}";
                    var point = PatternPoint.Create()
                        .WithName(pointName)
                        .WithPosition(coords[i].X, coords[i].Y)
                        .WithType(PointType.CutPoint)
                        .Build();
                    
                    cutPoints.Add(point);
                    newPieceBuilder.WithPoint(point);
                }
                
                // Create lines between the cut points
                for (int i = 1; i < cutPoints.Count; i++)
                {
                    var line = PatternLine.Create()
                        .WithStart(cutPoints[i-1])
                        .WithEnd(cutPoints[i])
                        .WithType(PathType.CutLine)
                        .Build();
                    
                    newPieceBuilder.WithPath(line);
                }
                
                // Close the loop
                var closingLine = PatternLine.Create()
                    .WithStart(cutPoints[cutPoints.Count - 1])
                    .WithEnd(cutPoints[0])
                    .WithType(PathType.CutLine)
                    .Build();
                
                newPieceBuilder.WithPath(closingLine);
                
                return newPieceBuilder.Build();
            }
            
            private PathElement ConvertPathToSeamLine(PathElement path)
            {
                if (path is PatternLine line)
                {
                    return PatternLine.Create()
                        .WithStart(line.Start)
                        .WithEnd(line.End)
                        .WithType(PathType.SeamLine)
                        .Build();
                }
                else if (path is PatternCurve curve)
                {
                    return PatternCurve.Create()
                        .WithStart(curve.Start)
                        .WithEnd(curve.End)
                        .WithControlPoint(curve.Control, 2)
                        .WithType(PathType.SeamLine)
                        .Build();
                }
                
                return path; // Return original if not recognized
            }
        }
    }