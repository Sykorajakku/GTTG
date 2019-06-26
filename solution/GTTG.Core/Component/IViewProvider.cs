// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using SkiaSharp;

using GTTG.Core.Time;

namespace GTTG.Core.Component {

    /// <summary>
    /// Providing state and conversions tools of graphical component.
    /// </summary>
    public interface IViewProvider : INotifyPropertyChanged {

        /// <summary>
        /// Representation of <see cref="DateTimeInterval"/> for <see cref="Drawing.Canvases.ContentDrawingCanvas"/> and
        /// <see cref="Drawing.Canvases.ViewDrawingCanvas"/>. Updates via <see cref="INotifyPropertyChanged"/>.
        /// </summary>
        DateTimeContext DateTimeContext { get; }

        /// <summary>
        /// Width of <see cref="Drawing.Canvases.ContentDrawingCanvas"/>.
        /// </summary>
        float ContentWidth { get; }

        /// <summary>
        /// Height of <see cref="Drawing.Canvases.ContentDrawingCanvas"/>.
        /// </summary>
        float ContentHeight { get; }

        /// <summary>
        /// Width of <see cref="Drawing.Canvases.ViewDrawingCanvas"/>.
        /// </summary>
        float ViewWidth { get; }

        /// <summary>
        /// Height of <see cref="Drawing.Canvases.ViewDrawingCanvas"/>.
        /// </summary>
        float ViewHeight { get; }

        /// <summary>
        /// Transformation matrix for setting accurate area of <see cref="Drawing.Canvases.ContentDrawingCanvas"/> into graphical component.
        /// </summary>
        SKMatrix ContentMatrix { get; }

        /// <summary>
        /// Scale factor of current DPI and device independent pixel DPI. If device independent pixel is 96 DPI and current is 192, value is 2.  
        /// </summary>
        float DpiScale { get; }

        /// <summary>
        /// Scale factor of current view scale and unscaled view. If view is zoomed in twice, scale is 2.
        /// </summary>
        float Scale { get; }

        /// <summary>
        /// Returns visible area of <see cref="Drawing.Canvases.ContentDrawingCanvas"/> in graphical component in coordinates of <see cref="Drawing.Canvases.ContentDrawingCanvas"/>.
        /// </summary>
        SKRect GetViewRect();

        /// <summary>Converts <see cref="Drawing.Canvases.ContentDrawingCanvas"/> horizontal position to <see cref="DateTime"/>.</summary>
        /// <param name="contentHorizontalPosition">Horizontal position on <see cref="Drawing.Canvases.ContentDrawingCanvas"/>.</param>
        /// <returns><see cref="DateTime"/> representation of <paramref name="contentHorizontalPosition"/>.</returns>
        DateTime GetDateTimeFromContent(float contentHorizontalPosition);

        /// <summary>Converts <see cref="Drawing.Canvases.ViewDrawingCanvas"/> horizontal position to <see cref="DateTime"/>.</summary>
        /// <param name="viewHorizontalPosition">Horizontal position on <see cref="Drawing.Canvases.ViewDrawingCanvas"/>.</param>
        /// <returns><see cref="DateTime"/> representation of <paramref name="viewHorizontalPosition"/>.</returns>
        DateTime GetDateTimeFromView(float viewHorizontalPosition);

        /// <summary>Converts <see cref="DateTime"/> to <see cref="Drawing.Canvases.ContentDrawingCanvas"/> position.</summary>
        /// <param name="dateTime"><see cref="DateTime"/> to convert.</param>
        /// <returns><see cref="Drawing.Canvases.ContentDrawingCanvas"/> horizontal position.</returns>
        float GetContentHorizontalPosition(DateTime dateTime);

        /// <summary>Converts <see cref="DateTime"/> to <see cref="Drawing.Canvases.ViewDrawingCanvas"/> position.</summary>
        /// <param name="dateTime"><see cref="DateTime"/> to convert.</param>
        /// <returns><see cref="Drawing.Canvases.ViewDrawingCanvas"/> horizontal position.</returns>
        float GetViewHorizontalPosition(DateTime dateTime);

        /// <summary>Converts <see cref="Drawing.Canvases.ViewDrawingCanvas"/> position to <see cref="Drawing.Canvases.ContentDrawingCanvas"/> position.</summary>
        /// <param name="viewPoint">Position in <see cref="Drawing.Canvases.ViewDrawingCanvas"/>.</param>
        /// <returns>Position in <see cref="Drawing.Canvases.ContentDrawingCanvas"/>.</returns>
        SKPoint ConvertViewToContentLocation(SKPoint viewPoint);

        /// <summary>Converts <see cref="Drawing.Canvases.ViewDrawingCanvas"/> position to <see cref="Drawing.Canvases.ContentDrawingCanvas"/> position.</summary>
        /// <param name="x">Horizontal position in <see cref="Drawing.Canvases.ViewDrawingCanvas"/>.</param>
        /// <param name="y">Vertical position in <see cref="Drawing.Canvases.ViewDrawingCanvas"/>.</param>
        /// <returns>Position in <see cref="Drawing.Canvases.ContentDrawingCanvas"/>.</returns>
        SKPoint ConvertViewToContentLocation(float x, float y);
    }

}
