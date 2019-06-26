// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using SkiaSharp;

using GTTG.Core.Drawing.Canvases;
using GTTG.Model.Model.Events;

namespace GTTG.Model.Lines {

    /// <summary>
    /// Contract for train path created from schedule. 
    /// </summary>
    public interface ITrainPath {

        /// <summary>
        /// Maps <see cref="TrainEvent"/> of updated schedule to index of point in path and the point itself.
        /// </summary>
        IReadOnlyDictionary<TrainEvent, (int Index, SKPoint PathPoint)> PointsByTrainPathEvents { get; }

        /// <summary>
        /// Provides all mapped movement events of updated schedule.
        /// </summary>
        IReadOnlyList<TrainEvent> TrainPathEvents { get; }

        /// <summary>
        /// Line to create and draw train path from.
        /// </summary>
        LinePaint LinePaint { get; }

        /// <summary>
        /// Points of train path.
        /// </summary>
        /// <param name="index">Index in train path with number of points equal to <see cref="PointCount"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">Lower than 0 or higher or equal than <see cref="PointCount"/>.</exception>
        /// <returns>Point at specified index.</returns>
        SKPoint this[int index] { get; }

        /// <summary>
        /// Number of points in path.
        /// </summary>
        int PointCount { get; }

        /// <summary>
        /// Reset train path and removes points. Needs to create new with <see cref="Update"/>.
        /// </summary>
        void Clear();

        /// <summary>
        /// Arranges points in path.
        /// </summary>
        void Arrange();

        /// <summary>
        /// Updates value from which train path is created.
        /// </summary>
        /// <param name="schedule">Schedule of events converted path.</param>
        void Update(ImmutableArray<TrainEvent> schedule);

        /// <summary>
        /// Draws train path on canvas.
        /// </summary>
        /// <param name="drawingCanvas">Drawing canvas to draw onto.</param>
        void Draw(DrawingCanvas drawingCanvas);

        /// <summary>
        /// Measures maximal path stroke width with ornaments included.
        /// </summary>
        /// <returns>Measured stroke width.</returns>
        float MeasurePathStrokeWidth();

        /// <summary>
        /// Measures closest distance of train path to provided point.
        /// </summary>
        /// <param name="point">Provided point to determine distance from.</param>
        /// <returns>Closest distance of path to provided point.</returns>
        float DistanceFromPoint(SKPoint point);

        /// <summary>
        /// Color of path.
        /// </summary>
        SKColor PathColor { get; set; }
    }
}
