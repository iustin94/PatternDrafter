
using System;
using System.Collections.Generic;

namespace PatternDrafter.Library
{
  /// <summary>
    /// Abstract base class for pattern piece drafters
    /// </summary>
    public abstract class PieceDrafter
    {
        protected PieceMeasurements PieceMeasurements { get; }
        protected PieceOptions Options { get; }
        
        // Strongly-typed properties for landmarks and paths
        protected PatternLandmarks Landmarks { get; set; }
        protected PatternPaths Paths { get; set; }
        
        public PieceDrafter(PieceMeasurements measurements, PieceOptions options)
        {
            PieceMeasurements = measurements;
            Options = options;
        }
        
        /// <summary>
        /// Template method that defines the drafting process structure
        /// </summary>
        public IEnumerable<PatternPiece> Draft()
        {
            // Initialize pattern-specific landmark and path objects
            InitializeContainers();
            
            // Step 1: Create all landmark points (based directly on measurements)
            AddLandmarkPoints();
            
            // Step 2: Create paths between landmark points
            AddLandmarkPaths();
            
            // Step 3: Create points relative to landmark paths/points
            AddRelativePointsAndPaths();
            
            // Step 4: Add additional features specific to the piece
            AddAdditionalFeatures();
            
            // Step 5: Build the final pattern piece
            return BuildPieces();
        }
        
        /// <summary>
        /// Initialize pattern-specific containers for landmarks and paths
        /// </summary>
        protected abstract void InitializeContainers();
        
        /// <summary>
        /// Creates points that are directly calculated from measurements
        /// </summary>
        protected abstract void AddLandmarkPoints();
        
        /// <summary>
        /// Creates paths that connect landmark points
        /// </summary>
        protected abstract void AddLandmarkPaths();
        
        /// <summary>
        /// Creates points and paths that are relative to landmark points and paths
        /// </summary>
        protected abstract void AddRelativePointsAndPaths();
        
        /// <summary>
        /// Adds any additional features to the pattern piece
        /// </summary>
        protected virtual void AddAdditionalFeatures() { }

        protected virtual IEnumerable<PatternPiece> BuildPieces()
        {
            yield return BuildMainPiece();
        }
        
        /// <summary>
        /// Builds the final pattern piece from all collected components
        /// </summary>
        protected virtual PatternPiece BuildMainPiece()
        {
            var pieceBuilder = Pattern.Piece()
                .WithName(GetPieceName());
                
            // Add piece-specific properties
            ConfigurePieceProperties(pieceBuilder);
            
            // Add all landmark points
            foreach (var point in Landmarks.GetAllLandmarkPoints())
            {
                pieceBuilder.WithPoint(point);
            }
            
            // Add all relative points
            foreach (var point in Landmarks.GetAllRelativePoints())
            {
                pieceBuilder.WithPoint(point);
            }
            
            // Add all landmark paths
            foreach (var path in Paths.GetAllLandmarkPaths())
            {
                pieceBuilder.WithPath(path);
            }
            
            // Add all relative paths
            foreach (var path in Paths.GetAllRelativePaths())
            {
                pieceBuilder.WithPath(path);
            }
            
            // Add instructions
            AddInstructions(pieceBuilder);
            
            return pieceBuilder.Build();
        }
        
        /// <summary>
        /// Gets the name of the pattern piece
        /// </summary>
        protected abstract string GetPieceName();
        
        /// <summary>
        /// Configures piece-specific properties like CutOnFold, Quantity, etc.
        /// </summary>
        protected virtual void ConfigurePieceProperties(PatternPiece.PatternPieceBuilder builder)
        {
            // Default implementation - override in subclasses
        }
        
        /// <summary>
        /// Adds instructions to the pattern piece
        /// </summary>
        protected virtual void AddInstructions(PatternPiece.PatternPieceBuilder builder)
        {
            // Default implementation - override in subclasses
        }

        public virtual IEnumerable<PatternLine> HemLines()
        {
            throw new NotImplementedException();
        }
    }
}