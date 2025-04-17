using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NetTopologySuite.Geometries;
using PatternDrafter.Library;
using PatternDrafter.Library.Pant;
using PatternEngine;

namespace PatternDrafter
{
    #region Example Usage

    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Starting Pattern Engine...");
            
            // Create body measurements (in centimeters)
            var measurements = new BodyMeasurements(
                chest: 120.0,
                waistCircumference: 111.0,
                hipCircumference: 111.0,
                shoulder: 45.0,
                shoulderToWaist: 45.0,
                neckCircumference: 44.0,
                armLength: 75.0,
                scyeDepth: 28.0,
                wristCircumference: 20.0,
                backNeckToWaist: 52.0,
                halfBack: 23.0,
                crothRise: 25,
                insideLegLength: 92,
                seat: 120,
                ankle: 27
            );
            
            Console.WriteLine("Created body measurements");
            
            // Create options
            var options = new PieceOptions { EasyFit = true, Short = true };
            ExportTShirt(measurements, options);
            
            var pantOptions = new PantOptions { Short = true, IncludeWaistband = true};
            ExportPant(measurements, pantOptions);
        }

        private static void ExportTShirt(BodyMeasurements measurements, PieceOptions options)
        {
            // Draft a T-Shirt pattern
            var tshirtDrafter = new ShirtDrafter(measurements, options);
            var tshirtPattern = tshirtDrafter.DraftPattern();
            
            // Output pattern information
            Console.WriteLine($"Pattern: {tshirtPattern.Name}");
            Console.WriteLine($"Number of pieces: {tshirtPattern.Pieces.Count}");
            
            foreach (var piece in tshirtPattern.Pieces)
            {
                Console.WriteLine($"\nPiece: {piece.Name}");
                Console.WriteLine($"Points: {piece.Points.Count}");
                Console.WriteLine($"Paths: {piece.Paths.Count}");
                Console.WriteLine($"Cut on fold: {piece.CutOnFold}");
                Console.WriteLine($"Quantity: {piece.Quantity}");
            }
            
            // Create output directory
            string outputDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PatternExports");
            Directory.CreateDirectory(outputDir);
            
            Console.WriteLine($"\nExporting pattern to: {outputDir}");
            
            // Create PNG exporter
            var exporter = new PNGExporter(
                resolution: 300,   // 300 DPI
                showGrid: true,    // Show measurement grid
                showLabels: true,  // Show point labels
                showPoints: true,  // Show all points
                scale: 15.0f,      // 15 pixels per cm for good detail
                padding: 60        // 60px padding around the pattern
            );

            // Create the exporter with default settings
            var lasercutExporter = new LaserCutExporter(
                includeLabels: true,
                padding: 1.0,
                strokeColor: "#FF0000", // Red stroke color for laser cutting
                strokeWidth: 0.1 // 0.1mm stroke width
            );
            
            // Export all pieces and get file paths
            var exportedFiles = exporter.ExportPattern(tshirtPattern, outputDir, "tshirt");
            var lasercutExports = lasercutExporter.ExportPattern(tshirtPattern, outputDir, "lasercut");
            
            // Output results
            Console.WriteLine("\nExported pattern pieces:");
            foreach (var file in exportedFiles)
            {
                Console.WriteLine($"  - {file.Key}: {Path.GetFileName(file.Value)}");
            }
            
            Console.WriteLine("\nAlso exporting each piece with seam allowance...");
            
            // Add seam allowance and export those pieces too
            foreach (var piece in tshirtPattern.Pieces)
            {
                string filePath = Path.Combine(outputDir, $"tshirt_{piece.Name.ToLower().Replace(" ", "_")}_with_seam_allowance.png");
                exporter.ExportPiece(piece, filePath);
                Console.WriteLine($"  - {piece.Name}: {Path.GetFileName(filePath)}");
            }
            
            Console.WriteLine("\nPattern export complete!");
            Console.WriteLine($"Files saved to: {outputDir}");
        }
        
        private static void ExportPant(BodyMeasurements measurements, PantOptions options)
        {
            // Draft a T-Shirt pattern
            var pantDrafter = new JoggersDrafter(measurements, options);
            var pantPieces = pantDrafter.DraftPattern();
            
            // Output pattern information
            Console.WriteLine($"Pattern: {pantPieces.Name}");
            Console.WriteLine($"Number of pieces: {pantPieces.Pieces.Count}");
            
            foreach (var piece in pantPieces.Pieces)
            {
                Console.WriteLine($"\nPiece: {piece.Name}");
                Console.WriteLine($"Points: {piece.Points.Count}");
                Console.WriteLine($"Paths: {piece.Paths.Count}");
                Console.WriteLine($"Cut on fold: {piece.CutOnFold}");
                Console.WriteLine($"Quantity: {piece.Quantity}");
            }
            
            // Create output directory
            string outputDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PatternExports");
            Directory.CreateDirectory(outputDir);
            
            Console.WriteLine($"\nExporting pattern to: {outputDir}");
            
            // Create PNG exporter
            var exporter = new PNGExporter(
                resolution: 300,   // 300 DPI
                showGrid: true,    // Show measurement grid
                showLabels: true,  // Show point labels
                showPoints: true,  // Show all points
                scale: 15.0f,      // 15 pixels per cm for good detail
                padding: 60        // 60px padding around the pattern
            );

            // Create the exporter with default settings
            var lasercutExporter = new LaserCutExporter(
                includeLabels: true,
                padding: 1.0,
                strokeColor: "#FF0000", // Red stroke color for laser cutting
                strokeWidth: 0.1 // 0.1mm stroke width
            );
            
            // Export all pieces and get file paths
            var exportedFiles = exporter.ExportPattern(pantPieces, outputDir, "tshirt");
            var lasercutExports = lasercutExporter.ExportPattern(pantPieces, outputDir, "lasercut");
            
            // Output results
            Console.WriteLine("\nExported pattern pieces:");
            foreach (var file in exportedFiles)
            {
                Console.WriteLine($"  - {file.Key}: {Path.GetFileName(file.Value)}");
            }
            
            Console.WriteLine("\nAlso exporting each piece with seam allowance...");
            
            // Add seam allowance and export those pieces too
            foreach (var piece in pantPieces.Pieces)
            {
                string filePath = Path.Combine(outputDir, $"tshirt_{piece.Name.ToLower().Replace(" ", "_")}_with_seam_allowance.png");
                exporter.ExportPiece(piece, filePath);
                Console.WriteLine($"  - {piece.Name}: {Path.GetFileName(filePath)}");
            }
            
            Console.WriteLine("\nPattern export complete!");
            Console.WriteLine($"Files saved to: {outputDir}");
        }
    }

    #endregion
}