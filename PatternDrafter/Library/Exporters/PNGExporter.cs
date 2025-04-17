using System;
using System.Collections.Generic;
using System.IO;
using SkiaSharp;
using NetTopologySuite.Geometries;
using PatternDrafter.Library;

namespace PatternEngine
{
    /// <summary>
    /// Exports pattern pieces to PNG image files using SkiaSharp for cross-platform compatibility
    /// </summary>
    public class PNGExporter
    {
        private readonly Dictionary<PathType, SKColor> _pathColors = new Dictionary<PathType, SKColor>
        {
            { PathType.CutLine, SKColors.Black },
            { PathType.SeamLine, SKColors.Blue },
            { PathType.FoldLine, SKColors.Green },
            { PathType.ConstructionLine, SKColors.LightGray },
            { PathType.GrainLine, SKColors.Red },
            { PathType.ButtonLine, SKColors.Purple },
            { PathType.HemLine, new SKColor(184, 134, 11) } // DarkGoldenrod
        };

        private readonly Dictionary<PathType, float> _pathThickness = new Dictionary<PathType, float>
        {
            { PathType.CutLine, 2.0f },
            { PathType.SeamLine, 1.5f },
            { PathType.FoldLine, 1.5f },
            { PathType.ConstructionLine, 0.5f },
            { PathType.GrainLine, 1.0f },
            { PathType.ButtonLine, 1.0f },
            { PathType.HemLine, 1.5f }
        };

        private readonly Dictionary<PathType, SKPathEffect> _pathEffects = new Dictionary<PathType, SKPathEffect>
        {
            { PathType.CutLine, null }, // Solid
            { PathType.SeamLine, null }, // Solid
            { PathType.FoldLine, SKPathEffect.CreateDash(new float[] { 10, 5 }, 0) }, // Dash
            { PathType.ConstructionLine, SKPathEffect.CreateDash(new float[] { 2, 2 }, 0) }, // Dot
            { PathType.GrainLine, SKPathEffect.CreateDash(new float[] { 10, 5, 2, 5 }, 0) }, // DashDot
            { PathType.ButtonLine, SKPathEffect.CreateDash(new float[] { 10, 5, 2, 5, 2, 5 }, 0) }, // DashDotDot
            { PathType.HemLine, null } // Solid
        };

        private readonly Dictionary<PointType, SKColor> _pointColors = new Dictionary<PointType, SKColor>
        {
            { PointType.Construction, SKColors.LightGray },
            { PointType.Landmark, SKColors.Black },
            { PointType.Notch, SKColors.Red },
            { PointType.CutPoint, SKColors.Black },
            { PointType.SeamLine, SKColors.Blue },
            { PointType.FoldLine, SKColors.Green }
        };

        private readonly int _resolution;
        private readonly bool _showGrid;
        private readonly bool _showLabels;
        private readonly bool _showPoints;
        private readonly float _scale;
        private readonly int _padding;
        
        /// <summary>
        /// Initializes a new instance of the PNGExporter class
        /// </summary>
        /// <param name="resolution">Image resolution in DPI</param>
        /// <param name="showGrid">Whether to show grid lines</param>
        /// <param name="showLabels">Whether to show point labels</param>
        /// <param name="showPoints">Whether to show all points</param>
        /// <param name="scale">Scale factor (1.0 = 1:1 scale)</param>
        /// <param name="padding">Padding around the pattern in pixels</param>
        public PNGExporter(
            int resolution = 300, 
            bool showGrid = true, 
            bool showLabels = true, 
            bool showPoints = true,
            float scale = 10.0f,  // 10 pixels per cm by default
            int padding = 50)
        {
            _resolution = resolution;
            _showGrid = showGrid;
            _showLabels = showLabels;
            _showPoints = showPoints;
            _scale = scale;
            _padding = padding;
        }

        /// <summary>
        /// Exports a single pattern piece to a PNG file
        /// </summary>
        /// <param name="piece">The pattern piece to export</param>
        /// <param name="filePath">The file path to save the PNG to</param>
        /// <returns>True if export was successful, false otherwise</returns>
        public bool ExportPiece(PatternPiece piece, string filePath)
        {
            // Calculate bounds
            var bounds = CalculateBounds(piece);
            bounds = ExpandBounds(bounds, _padding / _scale);
            
            // Calculate image dimensions
            int width = (int)((bounds.MaxX - bounds.MinX) * _scale) + _padding * 2;
            int height = (int)((bounds.MaxY - bounds.MinY) * _scale) + _padding * 2;
            
            // Create the SkiaSharp image surface
            using (var surface = SKSurface.Create(new SKImageInfo(width, height)))
            {
                var canvas = surface.Canvas;
                
                // Clear to white background
                canvas.Clear(SKColors.White);
                
                // Draw grid if requested
                if (_showGrid)
                {
                    DrawGrid(canvas, bounds, width, height);
                }
                
                // Draw paths
                foreach (var path in piece.Paths)
                {
                    if (path.IsVisible)
                    {
                        DrawPath(canvas, path, bounds);
                    }
                }
                
                // Draw points if requested
                if (_showPoints)
                {
                    foreach (var point in piece.Points.Values)
                    {
                        DrawPoint(canvas, point, bounds);
                        
                        if (_showLabels)
                        {
                            DrawLabel(canvas, point, bounds);
                        }
                    }
                }
                
                // Draw piece information
                DrawPieceInfo(canvas, piece, width, height);
                
                // Ensure directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                
                // Save the image
                using (var image = surface.Snapshot())
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var stream = File.OpenWrite(filePath))
                {
                    data.SaveTo(stream);
                }
            }
            
            return true;
        }

        /// <summary>
        /// Exports an entire pattern to PNG files (one per piece)
        /// </summary>
        /// <param name="pattern">The pattern to export</param>
        /// <param name="directory">The directory to save the PNG files in</param>
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
                    $"{safeName}.png" : 
                    $"{fileNamePrefix}_{safeName}.png";
                
                string filePath = Path.Combine(directory, fileName);
                
                if (ExportPiece(piece, filePath))
                {
                    result[piece.Name] = filePath;
                }
            }
            
            return result;
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
        /// Converts pattern coordinates to image coordinates
        /// </summary>
        private SKPoint TransformPoint(PatternPoint point, (double MinX, double MinY, double MaxX, double MaxY) bounds)
        {
            return new SKPoint(
                (float)((point.X - bounds.MinX) * _scale) + _padding,
                (float)((point.Y - bounds.MinY) * _scale) + _padding
            );
        }

        /// <summary>
        /// Converts pattern coordinates to image coordinates
        /// </summary>
        private SKPoint TransformCoordinate(double x, double y, (double MinX, double MinY, double MaxX, double MaxY) bounds)
        {
            return new SKPoint(
                (float)((x - bounds.MinX) * _scale) + _padding,
                (float)((y - bounds.MinY) * _scale) + _padding
            );
        }

        /// <summary>
        /// Draws a grid on the image
        /// </summary>
        private void DrawGrid(SKCanvas canvas, (double MinX, double MinY, double MaxX, double MaxY) bounds, 
            int width, int height)
        {
            // Calculate grid spacing (1 cm by default)
            float gridSpacing = _scale;
            
            // Draw minor grid lines (1 cm)
            using (var paint = new SKPaint
            {
                Color = new SKColor(0, 0, 0, 20), // Light gray, partially transparent
                StrokeWidth = 0.5f,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            })
            {
                // Round min/max values to nearest cm
                double startX = Math.Floor(bounds.MinX);
                double startY = Math.Floor(bounds.MinY);
                double endX = Math.Ceiling(bounds.MaxX);
                double endY = Math.Ceiling(bounds.MaxY);
                
                // Draw vertical lines
                for (double x = startX; x <= endX; x += 1.0)
                {
                    var p1 = TransformCoordinate(x, bounds.MinY, bounds);
                    var p2 = TransformCoordinate(x, bounds.MaxY, bounds);
                    canvas.DrawLine(p1.X, p1.Y, p2.X, p2.Y, paint);
                }
                
                // Draw horizontal lines
                for (double y = startY; y <= endY; y += 1.0)
                {
                    var p1 = TransformCoordinate(bounds.MinX, y, bounds);
                    var p2 = TransformCoordinate(bounds.MaxX, y, bounds);
                    canvas.DrawLine(p1.X, p1.Y, p2.X, p2.Y, paint);
                }
            }
            
            // Draw major grid lines (5 cm)
            using (var paint = new SKPaint
            {
                Color = new SKColor(0, 0, 0, 50), // Darker gray, partially transparent
                StrokeWidth = 1.0f,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            })
            {
                // Text paint for labels
                var textPaint = new SKPaint
                {
                    Color = SKColors.Gray,
                    TextSize = 8,
                    IsAntialias = true
                };
                
                // Round min/max values to nearest 5 cm
                double startX = Math.Floor(bounds.MinX / 5) * 5;
                double startY = Math.Floor(bounds.MinY / 5) * 5;
                double endX = Math.Ceiling(bounds.MaxX / 5) * 5;
                double endY = Math.Ceiling(bounds.MaxY / 5) * 5;
                
                // Draw vertical lines
                for (double x = startX; x <= endX; x += 5.0)
                {
                    var p1 = TransformCoordinate(x, bounds.MinY, bounds);
                    var p2 = TransformCoordinate(x, bounds.MaxY, bounds);
                    canvas.DrawLine(p1.X, p1.Y, p2.X, p2.Y, paint);
                    
                    // Add label
                    var textBounds = new SKRect();
                    string text = x.ToString();
                    textPaint.MeasureText(text, ref textBounds);
                    canvas.DrawText(text, p1.X - textBounds.Width / 2, height - 10, textPaint);
                }
                
                // Draw horizontal lines
                for (double y = startY; y <= endY; y += 5.0)
                {
                    var p1 = TransformCoordinate(bounds.MinX, y, bounds);
                    var p2 = TransformCoordinate(bounds.MaxX, y, bounds);
                    canvas.DrawLine(p1.X, p1.Y, p2.X, p2.Y, paint);
                    
                    // Add label
                    string text = y.ToString();
                    canvas.DrawText(text, 5, p1.Y + 4, textPaint);
                }
            }
        }

        /// <summary>
        /// Draws a path element (line or curve)
        /// </summary>
        private void DrawPath(SKCanvas canvas, PathElement path, (double MinX, double MinY, double MaxX, double MaxY) bounds)
        {
            SKColor color = _pathColors.ContainsKey(path.Type) ? _pathColors[path.Type] : SKColors.Black;
            float thickness = _pathThickness.ContainsKey(path.Type) ? _pathThickness[path.Type] : 1.0f;
            SKPathEffect effect = _pathEffects.ContainsKey(path.Type) ? _pathEffects[path.Type] : null;
            
            using (var paint = new SKPaint
            {
                Color = color,
                StrokeWidth = thickness,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                PathEffect = effect
            })
            {
                if (path is PatternLine line)
                {
                    var p1 = TransformPoint(line.Start, bounds);
                    var p2 = TransformPoint(line.End, bounds);
                    canvas.DrawLine(p1.X, p1.Y, p2.X, p2.Y, paint);
                }
                else if (path is PatternCurve curve)
                {
                    var points = curve.Discretize(20);
                    
                    // Create a path from the points
                    using (var skPath = new SKPath())
                    {
                        bool isFirst = true;
                        foreach (var point in points)
                        {
                            var transformedPoint = TransformCoordinate(point.X, point.Y, bounds);
                            
                            if (isFirst)
                            {
                                skPath.MoveTo(transformedPoint);
                                isFirst = false;
                            }
                            else
                            {
                                skPath.LineTo(transformedPoint);
                            }
                        }
                        
                        canvas.DrawPath(skPath, paint);
                    }
                }
            }
        }

        /// <summary>
        /// Draws a pattern point
        /// </summary>
        private void DrawPoint(SKCanvas canvas, PatternPoint point, (double MinX, double MinY, double MaxX, double MaxY) bounds)
        {
            // Skip construction points if not specifically requested
            if (point.Type == PointType.Construction && !_showPoints)
                return;
            
            SKColor color = _pointColors.ContainsKey(point.Type) ? _pointColors[point.Type] : SKColors.Black;
            
            var p = TransformPoint(point, bounds);
            
            // Draw different symbols based on point type
            switch (point.Type)
            {
                case PointType.Notch:
                    // Draw a triangle for notches
                    float size = 5.0f;
                    using (var path = new SKPath())
                    {
                        path.MoveTo(p.X, p.Y - size);
                        path.LineTo(p.X - size, p.Y + size);
                        path.LineTo(p.X + size, p.Y + size);
                        path.Close();
                        
                        using (var paint = new SKPaint
                        {
                            Color = color,
                            IsAntialias = true,
                            Style = SKPaintStyle.Fill
                        })
                        {
                            canvas.DrawPath(path, paint);
                        }
                    }
                    break;
                    
                case PointType.Construction:
                    // Draw a small circle for construction points
                    using (var paint = new SKPaint
                    {
                        Color = color,
                        IsAntialias = true,
                        Style = SKPaintStyle.Stroke,
                        StrokeWidth = 1.0f
                    })
                    {
                        canvas.DrawCircle(p.X, p.Y, 2, paint);
                    }
                    break;
                    
                default:
                    // Draw a filled circle for other points
                    using (var paint = new SKPaint
                    {
                        Color = color,
                        IsAntialias = true,
                        Style = SKPaintStyle.Fill
                    })
                    {
                        canvas.DrawCircle(p.X, p.Y, 3, paint);
                    }
                    break;
            }
        }

        /// <summary>
        /// Draws a label for a pattern point
        /// </summary>
        private void DrawLabel(SKCanvas canvas, PatternPoint point, (double MinX, double MinY, double MaxX, double MaxY) bounds)
        {
            // Skip labels for construction points
            if (point.Type == PointType.Construction && !_showPoints)
                return;
            
            var p = TransformPoint(point, bounds);
            
            using (var paint = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = 8,
                IsAntialias = true
            })
            {
                // Measure text for background
                var textBounds = new SKRect();
                paint.MeasureText(point.Name, ref textBounds);
                
                // Draw text with white background for better readability
                using (var bgPaint = new SKPaint
                {
                    Color = new SKColor(255, 255, 255, 200),
                    Style = SKPaintStyle.Fill
                })
                {
                    canvas.DrawRect(p.X + 5, p.Y - textBounds.Height / 2, 
                        textBounds.Width, textBounds.Height, bgPaint);
                }
                
                canvas.DrawText(point.Name, p.X + 5, p.Y + textBounds.Height / 2, paint);
            }
        }

        /// <summary>
        /// Draws piece information (name, cut quantity, etc.)
        /// </summary>
        private void DrawPieceInfo(SKCanvas canvas, PatternPiece piece, int width, int height)
        {
            // Create piece information text
            string info = $"{piece.Name}";
            
            if (piece.Quantity > 1)
            {
                info += $" (Cut {piece.Quantity})";
            }
            
            if (piece.CutOnFold)
            {
                info += " - CUT ON FOLD";
            }
            
            // Draw in bottom right corner
            using (var paint = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = 12,
                IsAntialias = true,
                Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright)
            })
            {
                // Measure text for background
                var textBounds = new SKRect();
                paint.MeasureText(info, ref textBounds);
                
                float rectX = width - textBounds.Width - 10;
                float rectY = height - textBounds.Height - 10;
                float rectWidth = textBounds.Width + 6;
                float rectHeight = textBounds.Height + 4;
                
                // Draw text with white background
                using (var bgPaint = new SKPaint
                {
                    Color = new SKColor(255, 255, 255, 240),
                    Style = SKPaintStyle.Fill
                })
                {
                    canvas.DrawRect(rectX, rectY, rectWidth, rectHeight, bgPaint);
                }
                
                // Draw border
                using (var borderPaint = new SKPaint
                {
                    Color = SKColors.Gray,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 1
                })
                {
                    canvas.DrawRect(rectX, rectY, rectWidth, rectHeight, borderPaint);
                }
                
                canvas.DrawText(info, width - textBounds.Width - 7, height - 10, paint);
            }
            
            // Draw grain line if specified
            if (piece.GrainLine.Count >= 2)
            {
                // Add "GRAIN LINE" text
                using (var paint = new SKPaint
                {
                    Color = SKColors.Red,
                    TextSize = 9,
                    IsAntialias = true
                })
                {
                    canvas.DrawText("GRAIN LINE", 10, 15, paint);
                }
            }
            
            // Draw seam allowance info if not zero
            if (piece.SeamAllowance > 0)
            {
                string seamInfo = $"Seam Allowance: {piece.SeamAllowance}cm";
                
                using (var paint = new SKPaint
                {
                    Color = SKColors.Gray,
                    TextSize = 9,
                    IsAntialias = true
                })
                {
                    var textBounds = new SKRect();
                    paint.MeasureText(seamInfo, ref textBounds);
                    canvas.DrawText(seamInfo, 10, height - 10, paint);
                }
            }
        }
    }
}