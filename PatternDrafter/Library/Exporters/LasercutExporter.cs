using System.Text;
using System.Xml;

namespace PatternDrafter.Library
{
    /// <summary>
    /// Exports pattern pieces to SVG files optimized for laser cutting
    /// </summary>
    public class LaserCutExporter
    {
        private readonly bool _includeLabels;
        private readonly double _padding;
        private readonly string _strokeColor;
        private readonly double _strokeWidth;
        
        /// <summary>
        /// Initializes a new instance of the LaserCutExporter class
        /// </summary>
        /// <param name="includeLabels">Whether to include piece labels in the SVG</param>
        /// <param name="padding">Padding around the pattern in centimeters</param>
        /// <param name="strokeColor">Stroke color for cut lines (HTML/CSS color)</param>
        /// <param name="strokeWidth">Stroke width for cut lines in millimeters</param>
        public LaserCutExporter(
            bool includeLabels = true,
            double padding = 1.0,
            string strokeColor = "#FF0000",
            double strokeWidth = 0.1)
        {
            _includeLabels = includeLabels;
            _padding = padding;
            _strokeColor = strokeColor;
            _strokeWidth = strokeWidth;
        }

        /// <summary>
        /// Exports a single pattern piece to an SVG file
        /// </summary>
        /// <param name="piece">The pattern piece to export</param>
        /// <param name="filePath">The file path to save the SVG to</param>
        /// <returns>True if export was successful, false otherwise</returns>
        public bool ExportPiece(PatternPiece piece, string filePath)
        {
            // Calculate bounds
            var bounds = CalculateBounds(piece);
            bounds = ExpandBounds(bounds, _padding);
            
            // Calculate SVG dimensions in centimeters
            double width = bounds.MaxX - bounds.MinX;
            double height = bounds.MaxY - bounds.MinY;
            
            // Create XML settings for formatting
            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                Encoding = Encoding.UTF8
            };
            
            // Ensure directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            
            using (var writer = XmlWriter.Create(filePath, settings))
            {
                // Start SVG document
                writer.WriteStartDocument();
                writer.WriteStartElement("svg", "http://www.w3.org/2000/svg");
                
                // Set dimensions and viewport to maintain centimeter scale
                writer.WriteAttributeString("width", $"{width}cm");
                writer.WriteAttributeString("height", $"{height}cm");
                writer.WriteAttributeString("viewBox", $"0 0 {width} {height}");
                writer.WriteAttributeString("xmlns", "http://www.w3.org/2000/svg");
                writer.WriteAttributeString("version", "1.1");
                
                // Add metadata
                writer.WriteStartElement("metadata");
                writer.WriteStartElement("title");
                writer.WriteString(piece.Name);
                writer.WriteEndElement(); // title
                
                writer.WriteStartElement("description");
                writer.WriteString($"Pattern piece: {piece.Name}");
                if (piece.CutOnFold)
                    writer.WriteString(", Cut on fold");
                if (piece.Quantity > 1)
                    writer.WriteString($", Quantity: {piece.Quantity}");
                writer.WriteEndElement(); // description
                writer.WriteEndElement(); // metadata
                
                // Collect all cut lines from the piece
                var cutLines = piece.Paths
                    .Where(p => p.IsVisible && p.Type == PathType.CutLine)
                    .ToList();
                
                // Group for cut lines
                writer.WriteStartElement("g");
                writer.WriteAttributeString("id", "cutlines");
                writer.WriteAttributeString("stroke", _strokeColor);
                writer.WriteAttributeString("stroke-width", _strokeWidth.ToString());
                writer.WriteAttributeString("fill", "none");
                
                // Draw all cut lines
                foreach (var path in cutLines)
                {
                    DrawPath(writer, path, bounds);
                }
                
                writer.WriteEndElement(); // g (cutlines)
                
                // Add labels if requested
                if (_includeLabels)
                {
                    writer.WriteStartElement("g");
                    writer.WriteAttributeString("id", "labels");
                    writer.WriteAttributeString("fill", "#000000");
                    writer.WriteAttributeString("font-family", "Arial, sans-serif");
                    writer.WriteAttributeString("font-size", "0.5");
                    
                    // Add piece name
                    writer.WriteStartElement("text");
                    writer.WriteAttributeString("x", (width / 2).ToString());
                    writer.WriteAttributeString("y", (height - 0.5).ToString());
                    writer.WriteAttributeString("text-anchor", "middle");
                    
                    string labelText = piece.Name;
                    if (piece.CutOnFold)
                        labelText += " - CUT ON FOLD";
                    if (piece.Quantity > 1)
                        labelText += $" (Cut {piece.Quantity})";
                        
                    writer.WriteString(labelText);
                    writer.WriteEndElement(); // text
                    
                    writer.WriteEndElement(); // g (labels)
                }
                
                // Close SVG document
                writer.WriteEndElement(); // svg
                writer.WriteEndDocument();
            }
            
            return true;
        }

        /// <summary>
        /// Exports an entire pattern to SVG files (one per piece)
        /// </summary>
        /// <param name="pattern">The pattern to export</param>
        /// <param name="directory">The directory to save the SVG files in</param>
        /// <param name="fileNamePrefix">Optional prefix for file names</param>
        /// <returns>Dictionary mapping piece names to file paths</returns>
        public Dictionary<string, string> ExportPattern(Pattern pattern, string directory, string fileNamePrefix = "")
        {
            var result = new Dictionary<string, string>();
            
            // Ensure directory exists
            Directory.CreateDirectory(directory);
            
            foreach (var piece in pattern.Pieces)
            {
                // Create safe filename
                string safeName = piece.Name.Replace(" ", "_").Replace("/", "_").ToLower();
                string fileName = string.IsNullOrEmpty(fileNamePrefix) ? 
                    $"{safeName}.svg" : 
                    $"{fileNamePrefix}_{safeName}.svg";
                
                string filePath = Path.Combine(directory, fileName);
                
                if (ExportPiece(piece, filePath))
                {
                    result[piece.Name] = filePath;
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// Exports all pattern pieces combined into a single SVG file for efficient laser cutting
        /// </summary>
        /// <param name="pattern">The pattern to export</param>
        /// <param name="filePath">The file path to save the SVG to</param>
        /// <param name="autoArrange">Whether to automatically arrange pieces to minimize material waste</param>
        /// <returns>True if export was successful, false otherwise</returns>
        public bool ExportCombinedPattern(Pattern pattern, string filePath, bool autoArrange = true)
        {
            try
            {
                // Calculate overall bounds for all pieces
                var allPieces = pattern.Pieces.ToList();
                
                // If auto-arrange is enabled, we'll position pieces to minimize waste
                // For this simple implementation, we'll just line them up horizontally
                double currentX = _padding;
                double maxHeight = 0;
                
                List<(PatternPiece Piece, double OffsetX, double OffsetY)> positionedPieces = 
                    new List<(PatternPiece, double, double)>();
                
                foreach (var piece in allPieces)
                {
                    var bounds = CalculateBounds(piece);
                    double pieceWidth = bounds.MaxX - bounds.MinX;
                    double pieceHeight = bounds.MaxY - bounds.MinY;
                    
                    // Position piece relative to the current X position
                    positionedPieces.Add((piece, currentX - bounds.MinX, _padding - bounds.MinY));
                    
                    // Update position for next piece
                    currentX += pieceWidth + _padding;
                    maxHeight = Math.Max(maxHeight, pieceHeight + _padding * 2);
                }
                
                // Calculate SVG dimensions
                double width = currentX;
                double height = maxHeight;
                
                // Create XML settings for formatting
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    Encoding = Encoding.UTF8
                };
                
                // Ensure directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                
                using (var writer = XmlWriter.Create(filePath, settings))
                {
                    // Start SVG document
                    writer.WriteStartDocument();
                    writer.WriteStartElement("svg", "http://www.w3.org/2000/svg");
                    
                    // Set dimensions and viewport to maintain centimeter scale
                    writer.WriteAttributeString("width", $"{width}cm");
                    writer.WriteAttributeString("height", $"{height}cm");
                    writer.WriteAttributeString("viewBox", $"0 0 {width} {height}");
                    writer.WriteAttributeString("xmlns", "http://www.w3.org/2000/svg");
                    writer.WriteAttributeString("version", "1.1");
                    
                    // Add metadata
                    writer.WriteStartElement("metadata");
                    writer.WriteStartElement("title");
                    writer.WriteString(pattern.Name);
                    writer.WriteEndElement(); // title
                    
                    writer.WriteStartElement("description");
                    writer.WriteString($"Pattern: {pattern.Name}, Pieces: {pattern.Pieces.Count}");
                    writer.WriteEndElement(); // description
                    writer.WriteEndElement(); // metadata
                    
                    // Group for all cut lines
                    writer.WriteStartElement("g");
                    writer.WriteAttributeString("id", "cutlines");
                    writer.WriteAttributeString("stroke", _strokeColor);
                    writer.WriteAttributeString("stroke-width", _strokeWidth.ToString());
                    writer.WriteAttributeString("fill", "none");
                    
                    // Process each positioned piece
                    foreach (var (piece, offsetX, offsetY) in positionedPieces)
                    {
                        writer.WriteStartElement("g");
                        writer.WriteAttributeString("id", $"piece_{piece.Name.Replace(" ", "_")}");
                        writer.WriteAttributeString("transform", $"translate({offsetX} {offsetY})");
                        
                        // Draw all cut lines for this piece
                        foreach (var path in piece.Paths
                            .Where(p => p.IsVisible && p.Type == PathType.CutLine))
                        {
                            DrawPathWithoutTransform(writer, path);
                        }
                        
                        // Add piece label if requested
                        if (_includeLabels)
                        {
                            var bounds = CalculateBounds(piece);
                            double pieceWidth = bounds.MaxX - bounds.MinX;
                            double pieceHeight = bounds.MaxY - bounds.MinY;
                            
                            writer.WriteStartElement("text");
                            writer.WriteAttributeString("x", (pieceWidth / 2).ToString());
                            writer.WriteAttributeString("y", (pieceHeight / 2).ToString());
                            writer.WriteAttributeString("text-anchor", "middle");
                            writer.WriteAttributeString("fill", "#000000");
                            writer.WriteAttributeString("font-family", "Arial, sans-serif");
                            writer.WriteAttributeString("font-size", "0.5");
                            
                            writer.WriteString(piece.Name);
                            writer.WriteEndElement(); // text
                        }
                        
                        writer.WriteEndElement(); // g (piece)
                    }
                    
                    writer.WriteEndElement(); // g (cutlines)
                    
                    // Close SVG document
                    writer.WriteEndElement(); // svg
                    writer.WriteEndDocument();
                }
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting combined pattern to SVG: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Calculates the bounds of a pattern piece
        /// </summary>
        private (double MinX, double MinY, double MaxX, double MaxY) CalculateBounds(PatternPiece piece)
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;
            
            // Check all points
            foreach (var point in piece.Points.Values)
            {
                minX = Math.Min(minX, point.X);
                minY = Math.Min(minY, point.Y);
                maxX = Math.Max(maxX, point.X);
                maxY = Math.Max(maxY, point.Y);
            }
            
            return (minX, minY, maxX, maxY);
        }

        /// <summary>
        /// Expands bounds by a specified amount
        /// </summary>
        private (double MinX, double MinY, double MaxX, double MaxY) ExpandBounds(
            (double MinX, double MinY, double MaxX, double MaxY) bounds, double amount)
        {
            return (
                bounds.MinX - amount,
                bounds.MinY - amount,
                bounds.MaxX + amount,
                bounds.MaxY + amount
            );
        }

        /// <summary>
        /// Writes a path element (line or curve) to SVG
        /// </summary>
        private void DrawPath(XmlWriter writer, PathElement path, 
            (double MinX, double MinY, double MaxX, double MaxY) bounds)
        {
            // Transform coordinates relative to bounds
            if (path is PatternLine line)
            {
                writer.WriteStartElement("line");
                writer.WriteAttributeString("x1", (line.Start.X - bounds.MinX).ToString());
                writer.WriteAttributeString("y1", (line.Start.Y - bounds.MinY).ToString());
                writer.WriteAttributeString("x2", (line.End.X - bounds.MinX).ToString());
                writer.WriteAttributeString("y2", (line.End.Y - bounds.MinY).ToString());
                writer.WriteEndElement(); // line
            }
            else if (path is PatternCurve curve)
            {
                // Discretize curve into points
                var points = curve.Discretize(20);
                
                // Create SVG path
                writer.WriteStartElement("path");
                StringBuilder pathData = new StringBuilder();
                
                // Move to first point
                var firstPoint = points.First();
                pathData.Append($"M {firstPoint.X - bounds.MinX} {firstPoint.Y - bounds.MinY}");
                
                // Line to each subsequent point
                foreach (var point in points.Skip(1))
                {
                    pathData.Append($" L {point.X - bounds.MinX} {point.Y - bounds.MinY}");
                }
                
                writer.WriteAttributeString("d", pathData.ToString());
                writer.WriteEndElement(); // path
            }
        }
        
        /// <summary>
        /// Writes a path element without transforming coordinates (for use with translate transform)
        /// </summary>
        private void DrawPathWithoutTransform(XmlWriter writer, PathElement path)
        {
            if (path is PatternLine line)
            {
                writer.WriteStartElement("line");
                writer.WriteAttributeString("x1", line.Start.X.ToString());
                writer.WriteAttributeString("y1", line.Start.Y.ToString());
                writer.WriteAttributeString("x2", line.End.X.ToString());
                writer.WriteAttributeString("y2", line.End.Y.ToString());
                writer.WriteEndElement(); // line
            }
            else if (path is PatternCurve curve)
            {
                // Discretize curve into points
                var points = curve.Discretize(20);
                
                // Create SVG path
                writer.WriteStartElement("path");
                StringBuilder pathData = new StringBuilder();
                
                // Move to first point
                var firstPoint = points.First();
                pathData.Append($"M {firstPoint.X} {firstPoint.Y}");
                
                // Line to each subsequent point
                foreach (var point in points.Skip(1))
                {
                    pathData.Append($" L {point.X} {point.Y}");
                }
                
                writer.WriteAttributeString("d", pathData.ToString());
                writer.WriteEndElement(); // path
            }
        }
    }
}