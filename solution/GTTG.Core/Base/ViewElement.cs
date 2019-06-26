// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using SkiaSharp;

using GTTG.Core.Drawing.Canvases;
using GTTG.Core.Extensions;
using GTTG.Core.Utils;

namespace GTTG.Core.Base {

    /// <summary>Element which can be drawn, arranged in layout and placed by strategies.</summary>
    public abstract class ViewElement : Visual {

        /// <summary>
        /// Clockwise rotation of view element in radians.
        /// </summary>
        public float Rotation { get; private set; } // 0 rad

        /// <summary>
        /// Scale of element, values between 0 and 1 makes element smaller to original scale.
        /// </summary>
        public float ScaleFactor { get; private set; } = 1;

        /// <summary>
        /// Height of element's content without applying rotate and scale transformations.
        /// Does not include margin measures.
        /// </summary>
        public float UnscaledHeight { get; private set; }

        /// <summary>
        /// Width of element's content without applying rotate and scale transformations.
        /// Does not include margin measures.
        /// </summary>
        public float UnscaledWidth { get; private set; }

        /// <summary>
        /// Height which element occupies on canvas after applied rotate and scale transformations.
        /// Margin measures included.
        /// </summary>
        public float ContentWidth { get; private set; }

        /// <summary>
        /// Width which element occupies on canvas after applied rotate and scale transformations.
        /// Margin measures included.
        /// </summary>
        public float ContentHeight { get; private set; }

        /// <summary>
        /// Position of upper left vertex (with 0 rad rotation) on <see cref="Drawing.Canvases.ContentDrawingCanvas"/>, after applied rotate and scale transformations.
        /// Margin measures included.
        /// </summary>
        public SKPoint ContentLeftTop { get; private set; }

        /// <summary>
        /// Position of lower left vertex (with 0 rad rotation) on <see cref="Drawing.Canvases.ContentDrawingCanvas"/>, after applied rotate and scale transformations.
        /// Margin measures included.
        /// </summary>
        public SKPoint ContentLeftBottom { get; private set; }

        /// <summary>
        /// Position of right top vertex (with 0 rad rotation) on <see cref="Drawing.Canvases.ContentDrawingCanvas"/>, after applied rotate and scale transformations.
        /// Margin measures included.
        /// </summary>
        public SKPoint ContentRightTop { get; private set; }

        /// <summary>
        /// Position of right bottom vertex (with 0 rad rotation) on <see cref="Drawing.Canvases.ContentDrawingCanvas"/>, after applied rotate and scale transformations.
        /// Margin measures included.
        /// </summary>
        public SKPoint ContentRightBottom { get; private set; }

        /// <summary>
        /// TransformationMatrix applied to canvas to map <see cref="SKPoint.Empty"/> to start of element's content.
        /// Allows drawing of element's content in it's coordinate system, with <see cref="Rotation"/> and <see cref="ScaleFactor"/> applied in the matrix.
        /// </summary>
        public SKMatrix PlacementMatrix { get; protected set; }

        /// <summary>
        /// Top margin of the element.
        /// </summary>
        public float TopMargin { get; set; }

        /// <summary>
        /// Bottom margin of the element.
        /// </summary>
        public float BottomMargin { get; set; }

        /// <summary>
        /// Right margin of the element.
        /// </summary>
        public float RightMargin { get; set; }

        /// <summary>
        /// Left margin of the element.
        /// </summary>
        public float LeftMargin { get; set; }

        /// <summary>
        /// Preferred width of element for layout, contains <see cref="LeftMargin"/> and <see cref="RightMargin"/>. 
        /// If set and <see cref="ArrangeCore"/> not overriden and falls between values of <see cref="MaxWidth"/> and <see cref="MinWidth"/>,
        /// has same priority behaviour as <see cref="MaxWidth"/> and <see cref="MinWidth"/> for <see cref="ArrangeOverride"/>.
        /// </summary>
        public float Width { get; set; } = float.NaN;

        /// <summary>
        /// Preferred height of element for layout, contains <see cref="TopMargin"/> and <see cref="BottomMargin"/>. 
        /// If set and <see cref="ArrangeCore"/> not overriden and falls between values of <see cref="MaxHeight"/> and <see cref="MinHeight"/>,
        /// has same priority behaviour as <see cref="MaxHeight"/> and <see cref="MinHeight"/> for <see cref="ArrangeOverride"/>.
        /// </summary>
        public float Height { get; set; } = float.NaN;

        /// <summary>
        /// Preferred maximal height of element content for layout.
        /// If set and <see cref="ArrangeCore"/> not overriden, height value from <see cref="ArrangeOverride"/> has lower priority if max is lower.
        /// </summary>
        public float MaxHeight { get; set; } = float.MaxValue;

        /// <summary>
        /// Preferred minimal height of element content for layout.
        /// If set and <see cref="ArrangeCore"/> not overriden, height value from <see cref="ArrangeOverride"/> has lower priority if min is higher.
        /// </summary>
        public float MinHeight { get; set; } = 0;

        /// <summary>
        /// Preferred maximal width of element content for layout.
        /// If set and <see cref="ArrangeCore"/> not overriden, width value from <see cref="ArrangeOverride"/> has lower priority if max is lower.
        /// </summary>
        public float MaxWidth { get; set; } = float.MaxValue;

        /// <summary>
        /// Preferred minimal width of element content for layout.
        /// If set and <see cref="ArrangeCore"/> not overriden, width value from <see cref="ArrangeOverride"/> has lower priority if min is higher.
        /// </summary>
        public float MinWidth { get; set; } = 0;

        /// <summary>
        /// Height of element set by <see cref="Arrange(SkiaSharp.SKPoint,SkiaSharp.SKSize)"/> or other similar methods.
        /// </summary>
        public float ArrangedHeight { get; private set; }

        /// <summary>
        /// Width of element set by <see cref="Arrange(SkiaSharp.SKPoint,SkiaSharp.SKSize)"/> or other similar methods.
        /// </summary>
        public float ArrangedWidth { get; private set; }

        /// <summary>
        /// Latest size given to view element in <see cref="Arrange(SKPoint, SKSize)"/>
        /// or <see cref="Arrange(SKPoint, SKSize, ViewElement)"/>. When <see cref="Rotate"/> and similar
        /// operations applied, rearrange uses this value.
        /// </summary>
        public SKSize ArrangeSize { get; private set; }

        /// <summary>
        /// Desired size of element after latest <see cref="Measure"/> call.
        /// </summary>
        public SKSize DesiredSize { get; private set; }

        /// <summary>
        /// If true, <see cref="Clip"/> is applied before drawing of the element.
        /// </summary>
        public bool HasClipEnabled { get; protected set; } = false;

        /// <summary>
        /// If applied, any drawing in view element outside this clip area is not visible.
        /// Origin and size of clip are both not scaled and rotated; clip is applied to <see cref="PlacementMatrix"/>.
        /// </summary>
        public SKRect Clip { get; private set; }

        /// <summary>
        /// Minimal bounding rectangle with edges perpendicular to axes of cartesian graph around
        /// <see cref="ContentLeftTop"/>, <see cref="ContentRightTop"/>, <see cref="ContentRightBottom"/>, <see cref="ContentLeftBottom"/>.
        /// </summary>
        public SKRect BoundingRect { get; private set; }

        /// <summary>
        /// Matrix used for arrange cycle, equal to parent <see cref="PlacementMatrix"/>.
        /// In comparison to instance's <see cref="PlacementMatrix"/> does not account margins and transformations.
        /// </summary>
        protected SKMatrix ArrangeMatrix;

        /// <inheritdoc />
        public sealed override void Draw(DrawingCanvas drawingCanvas) {

            var previousMatrix = drawingCanvas.Canvas.TotalMatrix;
            if (!IsDrawableOnCanvas(drawingCanvas)) return;
            if (!IsInView(drawingCanvas.View)) return;

            var elementDrawingCanvas = drawingCanvas.Create(this);

            if (HasClipEnabled) {
                var stackPointer = drawingCanvas.Canvas.Save();
                elementDrawingCanvas.Canvas.ClipRect(Clip);
                OnDraw(elementDrawingCanvas);
                drawingCanvas.Canvas.RestoreToCount(stackPointer);
            }
            else {
                OnDraw(elementDrawingCanvas);
                drawingCanvas.Canvas.SetMatrix(previousMatrix);
            }
        }

        /// <summary>Scales view element and it's content. Does not re-arrange view element to fits it's parent after scale. Combines with current <see cref="ScaleFactor"/>.
        /// Arranges the view element and it's children with <see cref="ArrangeSize"/> with changed scale.</summary>
        /// <param name="scaleMultiple">If higher than 0, resizes element by scaling it by provided value. Otherwise does nothing.</param>
        /// <param name="isCombined">If true, multiplies <see cref="ScaleFactor"/> with current <see cref="ScaleFactor"/>. Otherwise resets scale and sets this value instead.</param>
        public void Scale(float scaleMultiple, bool isCombined = true) {

            if (scaleMultiple <= 0) return;
            var newScaleMultiple = isCombined ? scaleMultiple : (1 / ScaleFactor) * scaleMultiple;

            SKMatrix.Concat(ref ArrangeMatrix, ArrangeMatrix, SKMatrix.MakeScale(newScaleMultiple, newScaleMultiple));
            ScaleFactor = isCombined ? scaleMultiple * ScaleFactor : scaleMultiple;
            Arrange(ArrangeSize);
        }

        /// <summary>Rotates view element and it's content. Does not rearrange view element to fits it's parent after rotation.
        /// Arranges the view element and it's children with <see cref="ArrangeSize"/> with changed rotation.</summary>
        /// <param name="radRotation">Clockwise rotation in radians to be added to current <see cref="Rotation"/>.</param>
        /// <param name="isCombined">If true, add rotation to current <see cref="Rotation"/>. Otherwise resets rotation and sets this value instead.</param>
        public void Rotate(float radRotation, bool isCombined = true) {

            var transformation = isCombined ? radRotation : (float) (Math.PI * 2 - GetRoundRad(Rotation)) + radRotation;
            SKMatrix.Concat(ref ArrangeMatrix, ArrangeMatrix, SKMatrix.MakeRotation(transformation));
            Rotation = isCombined ? Rotation + radRotation : radRotation;
            Arrange(ArrangeSize);
        }

        /// <summary>Changes position of view element and then arranges it and it's children with <see cref="ArrangeSize"/> with the changed position.</summary>
        /// <param name="origin">Position in <see cref="ContentDrawingCanvas"/>.</param>
        public void Reposition(SKPoint origin) {
            Reposition(origin, SKMatrix.MakeIdentity());
        }

        /// <summary>Changes position of view element and then arranges it and it's children with <see cref="ArrangeSize"/> with the changed position.</summary>
        /// <param name="origin">Origin in content of <paramref name="viewElement"/> coordinate system.</param>
        /// <param name="viewElement"><see cref="ViewElement"/> in whose content is this view element repositioned.</param>
        public void Reposition(SKPoint origin, ViewElement viewElement) {
            Reposition(origin, viewElement.PlacementMatrix);
        }

        private void Reposition(SKPoint origin, SKMatrix matrix) {

            var globalOrigin = matrix.MapPoint(origin);
            ArrangeMatrix.TransX = globalOrigin.X;
            ArrangeMatrix.TransY = globalOrigin.Y;

            Arrange(ArrangeSize);
        }

        /// <summary>Arranges element directly to coordinate system of <see cref="ContentDrawingCanvas"/>. Resets <see cref="ScaleFactor"/> and <see cref="Rotation"/> to default values.</summary>
        /// <param name="origin">Position in <see cref="ContentDrawingCanvas"/>.</param>
        /// <param name="size">Size provided to element.</param>
        public void Arrange(SKPoint origin, SKSize size) {
            Arrange(origin, size, SKMatrix.MakeIdentity());
        }

        /// <summary>Arranges element to coordinate system of <paramref name="parentElement"/>. Resets <see cref="ScaleFactor"/> and <see cref="Rotation"/> to default values.</summary>
        /// <param name="origin">Position in <paramref name="parentElement"/>.</param>
        /// <param name="size">Size provided to <paramref name="parentElement"/></param>
        /// <param name="parentElement"><see cref="ViewElement"/> whose coordinate system is used to place this view element.</param>
        public void Arrange(SKPoint origin, SKSize size, ViewElement parentElement) {
            Arrange(origin, size, parentElement.PlacementMatrix);
        }

        private void Arrange(SKPoint origin, SKSize size, SKMatrix globalMatrix) {

            ArrangeMatrix = globalMatrix;
            ScaleFactor = 1;
            Rotation = 0;

            var transformedOrigin = ArrangeMatrix.MapPoint(origin);
            ArrangeMatrix.TransX = transformedOrigin.X;
            ArrangeMatrix.TransY = transformedOrigin.Y;

            ArrangeSize = size;
            Arrange(ArrangeSize);
        }

        /// <summary>Measures view element. Assigns virtual <see cref="MeasureCore"/> with no modifications to <see cref="DesiredSize"/>.</summary>
        /// <param name="availableSize">Recommended available size to use.</param>
        public void Measure(SKSize availableSize) {
            DesiredSize = MeasureCore(availableSize);
        }

        /// <summary>
        /// Override-able base logic for <see cref="MeasureOverride"/> calls.
        /// Calls <see cref="MeasureOverride"/> and modified it's return size if needed.
        /// If <see cref="MeasureOverride"/> returned size does not falls into <see cref="MaxHeight"/>, <see cref="MaxWidth"/>, <see cref="MinHeight"/>, <see cref="MinWidth"/>, modified to fit.
        /// Newly min-max-modified value is then also modified to fit <paramref name="availableSize"/>.
        /// </summary>
        /// <param name="availableSize">Maximum <see cref="SKSize"/> to be returned.</param>
        /// <returns>Value to be assigned to <see cref="DesiredSize"/>.</returns>
        protected virtual SKSize MeasureCore(SKSize availableSize) {
            
            // determine size from (min/max) width and height properties, to make availableSize
            var minMax = new MinMax(this);

            float horizontalMargin = LeftMargin + RightMargin;
            float verticalMargin = TopMargin + BottomMargin;

            SKSize removedMarginSize = new SKSize(
                (float) Math.Max(0.0, availableSize.Width - horizontalMargin),
                (float) Math.Max(0.0, availableSize.Height - verticalMargin));

            // priority: min width > max width > removed margin size (select lowest priority first if not logically incorrect to upper priority)
            var sizeWidth = Math.Max(minMax.MinWidth, Math.Min(removedMarginSize.Width, minMax.MaxWidth));
            var sizeHeight = Math.Max(minMax.MinHeight, Math.Min(removedMarginSize.Height, minMax.MaxHeight));
            SKSize size = new SKSize(sizeWidth, sizeHeight);

            // pass to measure available size with size left from margin subtraction
            var measuredSize = MeasureOverride(size);

            // check override value with max values and override if logically incorrect
            if (measuredSize.Width > minMax.MaxWidth) {
                measuredSize.Width = minMax.MaxWidth;
            }
            if (measuredSize.Height > minMax.MaxHeight) {
                measuredSize.Height = minMax.MaxHeight;
            }

            // check override value with min values and override if logically incorrect
            if (measuredSize.Width < minMax.MinWidth) {
                measuredSize.Width = minMax.MinWidth;
            }
            if (measuredSize.Height < minMax.MinHeight) {
                measuredSize.Height = minMax.MinHeight;
            }

            // add margins to value returned from override
            var measureCoreWidth = measuredSize.Width + horizontalMargin;
            var measureCoreHeight = measuredSize.Height + verticalMargin;

            // Height, Width guard which accounts margins
            if (measureCoreHeight > availableSize.Height) {
                measureCoreHeight = availableSize.Height;
            }
            if (measureCoreWidth > availableSize.Width) {
                measureCoreWidth = availableSize.Width;
            }

            return new SKSize(measureCoreWidth, measureCoreHeight);
        }

        /// <summary>User measure for this element called from <see cref="Measure"/>.</summary>
        /// <param name="availableSize">Recommended available size to use.</param>
        /// <returns>Measured size of this element by user.</returns>
        protected virtual SKSize MeasureOverride(SKSize availableSize) {
            return SKSize.Empty;
        }

        private void Arrange(SKSize rect) {

            PlacementMatrix = ArrangeMatrix;

            var (contentRect, size) = ArrangeCore(new SKSize(rect.Width, rect.Height));
            var arrangeRect = SKRect.Create(SKPoint.Empty, ArrangeSize);
            var sizeRect = SKRect.Create(SKPoint.Empty, size);

            // sizes returned from ArrangeCore does not match available size, make element not visible and return
            if (!sizeRect.ContainsWithDelta(contentRect) || !arrangeRect.ContainsWithDelta(sizeRect)) {

                ArrangedWidth = UnscaledHeight = 0;
                ArrangedHeight = UnscaledWidth = 0;
                Clip = SKRect.Empty;
                BoundingRect = SKRect.Empty;
                ArrangeMatrix = new SKMatrix();
                return;
            }

            ArrangedWidth = size.Width;
            ArrangedHeight = size.Height;
            UnscaledWidth = contentRect.Width;
            UnscaledHeight = contentRect.Height;

            // for drawing & hit testing
            Clip = SKRect.Create(SKPoint.Empty, new SKSize(UnscaledWidth, UnscaledHeight));

            ContentLeftTop = PlacementMatrix.MapPoint(- contentRect.Left, - contentRect.Top);
            ContentRightTop = PlacementMatrix.MapPoint(contentRect.Width + contentRect.Left, - contentRect.Top);
            ContentLeftBottom = PlacementMatrix.MapPoint(- contentRect.Left, contentRect.Height + contentRect.Top);
            ContentRightBottom = PlacementMatrix.MapPoint(contentRect.Width + contentRect.Left, contentRect.Height + contentRect.Top);

            var width = ContentLeftTop - ContentRightTop;
            var height = ContentLeftTop - ContentLeftBottom;
            ContentWidth = PlacementUtils.ComputesVectorLength(width);
            ContentHeight = PlacementUtils.ComputesVectorLength(height);

            BoundingRect = GetBoundingRectangle();
        }

        /// <summary>Override-able base logic for <see cref="ArrangeOverride"/> calls.</summary>
        /// <param name="size">Available size to use. If exceeded, element has empty size and is not arranged.</param>
        /// <returns>Content positioned in size (size contains margins). Content is moved from [0,0] by <see cref="TopMargin"/> and <see cref="LeftMargin"/>.</returns>
        protected virtual (SKRect Content, SKSize Size) ArrangeCore(SKSize size) {

            var horizontalMargin = LeftMargin + RightMargin;
            var verticalMargin = TopMargin + BottomMargin;

            var removedMarginsSize = size;
            removedMarginsSize.Width = Math.Max(0.0f, removedMarginsSize.Width - horizontalMargin);
            removedMarginsSize.Height = Math.Max(0.0f, removedMarginsSize.Height - verticalMargin);

            var minMax = new MinMax(this);

            // prepare placement matrix for children and content --> move by margin [0,0] in matrix points to [left,top] margin
            var contentStartOffset = ArrangeMatrix.MapPoint(LeftMargin, TopMargin);
            var contentMatrix = PlacementMatrix;
            contentMatrix.TransX = contentStartOffset.X;
            contentMatrix.TransY = contentStartOffset.Y;
            PlacementMatrix = contentMatrix;

            var overrideSize = ArrangeOverride(removedMarginsSize);

            if (size.Width < minMax.MinWidth) minMax.MinWidth = size.Width;
            if (size.Height < minMax.MinHeight) minMax.MinHeight = size.Height;

            overrideSize = new SKSize(Math.Min(overrideSize.Width, minMax.MaxWidth), Math.Min(overrideSize.Height, minMax.MaxHeight));
            overrideSize = new SKSize(Math.Max(overrideSize.Width, minMax.MinWidth), Math.Max(overrideSize.Height, minMax.MinHeight));

            var contentOrigin = new SKPoint(LeftMargin, TopMargin);
            var content = SKRect.Create(contentOrigin, new SKSize(overrideSize.Width, overrideSize.Height));
            var finalSize = overrideSize + new SKSize(horizontalMargin, verticalMargin);
            return (content, finalSize);
        }

        /// <summary>User measure for this element called from <see cref="Arrange(SKPoint,SKSize)"/> or similar methods.</summary>
        /// <param name="finalSize">Available size to use.</param>
        /// <returns>Final size of element which should not exceed <paramref name="finalSize"/>.</returns>
        protected virtual SKSize ArrangeOverride(SKSize finalSize) {
            return finalSize;
        }

        /// <inheritdoc/>
        public override bool HasHit(SKPoint contentPoint) {

            if (IsRotatedInContentCanvas()) {
                return HitRotated(contentPoint);
            }

            return (contentPoint.X >= ContentLeftTop.X &&
                    contentPoint.X <= ContentRightTop.X &&
                    contentPoint.Y <= ContentRightBottom.Y &&
                    contentPoint.Y >= ContentLeftTop.Y);
        }

        /// <summary>Determines if this element is in visible area of <see cref="ContentDrawingCanvas"/>.</summary>
        /// <param name="view">Rectangular area in <see cref="ContentDrawingCanvas"/> representing visible area.</param>
        /// <returns>True if this element is in area of <paramref name="view"/>; otherwise false.</returns>
        public bool IsInView(SKRect view) {
            return view.Contains(BoundingRect) || view.IntersectsWith(BoundingRect);
        }

        /// <summary>Creates bounding rectangle around points of rectangle determined by <see cref="ViewElement.ContentLeftTop"/> and <see cref="ViewElement.ContentRightBottom"/>.</summary>
        public SKRect GetBoundingRectangle() {

            var leftTop = ContentLeftTop;
            var rightTop = ContentRightTop;
            var leftBottom = ContentLeftBottom;
            var rightBottom = ContentRightBottom;

            var left = Math.Min(Math.Min(leftBottom.X, rightBottom.X), Math.Min(leftTop.X, rightTop.X));
            var top = Math.Min(Math.Min(leftBottom.Y, rightBottom.Y), Math.Min(leftTop.Y, rightTop.Y));
            var right = Math.Max(Math.Max(leftBottom.X, rightBottom.X), Math.Max(leftTop.X, rightTop.X));
            var bottom = Math.Max(Math.Max(leftBottom.Y, rightBottom.Y), Math.Max(leftTop.Y, rightTop.Y));

            return new SKRect(left, top, right, bottom);
        }

        private bool HitRotated(SKPoint p) {

            var isInBounding = (p.X >= BoundingRect.Left &&
                              p.X <= BoundingRect.Right &&
                              p.Y <= BoundingRect.Bottom &&
                              p.Y >= BoundingRect.Top);

            if (!isInBounding) return false;
            else return HasHitPrecise(p);
        }

        private bool HasHitPrecise(SKPoint p) {

            // tests against half-plane of view element edges
            var v = ContentRightTop - ContentLeftTop;
            if (-v.Y * p.X + v.X * p.Y - (-v.Y * ContentRightTop.X + v.X * ContentRightTop.Y) < 0) return false;
            v = ContentRightBottom - ContentLeftBottom;
            if (-v.Y * p.X + v.X * p.Y - (-v.Y * ContentRightBottom.X + v.X * ContentRightBottom.Y) > 0) return false;
            v = ContentLeftBottom - ContentLeftTop;
            if (-v.Y * p.X + v.X * p.Y - (-v.Y * ContentLeftBottom.X + v.X * ContentLeftBottom.Y) > 0) return false;
            v = ContentRightBottom - ContentRightTop;
            if (-v.Y * p.X + v.X * p.Y - (-v.Y * ContentRightBottom.X + v.X * ContentRightBottom.Y) < 0) return false;

            return true;
        }

        private bool IsRotatedInContentCanvas() {
            return Math.Abs(ScaleFactor - PlacementMatrix.ScaleX) > 0;
        }

        private static float GetRoundRad(float rad) {

            while (Math.PI * 2 < rad) {
                rad -= (float)Math.PI * 2;
            }
            return rad;
        }

        /// <summary>Determines maximal and minimal width and height values considering various user properties states.</summary>
        protected struct MinMax {

            internal float MinWidth;
            internal readonly float MaxWidth;
            internal float MinHeight;
            internal readonly float MaxHeight;

            internal MinMax(ViewElement viewElement) {

                MaxHeight = viewElement.MaxHeight;
                MinHeight = viewElement.MinHeight;
                var height = viewElement.Height;
                MaxHeight = Math.Max(Math.Min(float.IsNaN(height) ? float.MaxValue : height, MaxHeight), MinHeight);
                MinHeight = Math.Max(Math.Min(MaxHeight, float.IsNaN(height) ? 0 : height), MinHeight);

                MaxWidth = viewElement.MaxWidth;
                MinWidth = viewElement.MinWidth;
                var width = viewElement.Width;
                MaxWidth = Math.Max(Math.Min(float.IsNaN(width) ? float.MaxValue : width, MaxWidth), MinWidth);
                MinWidth = Math.Max(Math.Min(MaxWidth, float.IsNaN(width) ? 0 : width), MinWidth);
            }
        }
    }
}
