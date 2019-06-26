// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using SkiaSharp;

using GTTG.Core.Time;

namespace GTTG.Core.Component {

    /// <summary>Interactive view with scale and translate operations limited by border bounds.</summary>
    public class ViewModifier {

        /// <summary>TransformationMatrix which positions view to border.</summary>
        public SKMatrix ViewMatrix => _viewMatrix;
        
        /// <summary>Horizontal length of border where view exists.</summary>
        public float BorderWidth { get; private set; }

        /// <summary>Vertical length of border where view exists.</summary>
        public float BorderHeight { get; private set; }

        /// <summary>Horizontal length of view.</summary>
        public float ViewWidth { get; private set; }

        /// <summary>Vertical length of view.</summary>
        public float ViewHeight { get; private set; }

        private SKMatrix _viewMatrix;

        /// <summary>Creates view in border of specified sizes with view attached to the top left corner of the border.</summary>
        /// <param name="viewWidth">Initial <see cref="ViewWidth"/> value.</param>
        /// <param name="viewHeight">Initial <see cref="ViewHeight"/> value.</param>
        /// <param name="borderWidth">Initial <see cref="BorderWidth"/> value.</param>
        /// <param name="borderHeight">Initial <see cref="BorderHeight"/> value.</param>
        /// <exception cref="ArgumentException"><see cref="ViewWidth"/> higher than <see cref="BorderWidth"/>; or same for <see cref="ViewHeight"/> and <see cref="BorderHeight"/>.</exception>
        public ViewModifier(float viewWidth, float viewHeight, float borderWidth, float borderHeight) {

            if (viewWidth > borderWidth) {
                throw new ArgumentException($"{nameof(viewWidth)} > {nameof(borderWidth)}");
            }
            if (viewHeight > borderHeight) {
                throw new ArgumentException($"{nameof(viewHeight)} > {nameof(borderHeight)}");
            }

            _viewMatrix = SKMatrix.MakeIdentity();
            ViewWidth = viewWidth;
            ViewHeight = viewHeight;
            BorderWidth = borderWidth;
            BorderHeight = borderHeight;
        }

        /// <summary>Resizes the current view and border.</summary>
        /// <param name="newViewWidth">New horizontal width of view.</param>
        /// <param name="newViewHeight">New vertical width of view.</param>
        /// <returns>Result state of the operation. If operation cannot be applied, view is left unmodified.</returns>
        public ResizeTransformationResult TryResizeView(float newViewWidth, float newViewHeight) {

            if (newViewWidth < 0 || newViewHeight < 0) {
                return ResizeTransformationResult.ViewUnmodified;
            }

            var ratioX = (newViewWidth / ViewWidth);
            var ratioY = (newViewHeight / ViewHeight);

            var transX = ratioX * _viewMatrix.TransX;
            var transY = ratioY * _viewMatrix.TransY;

            _viewMatrix.TransX = transX;
            _viewMatrix.TransY = transY;

            ViewWidth = newViewWidth;
            ViewHeight = newViewHeight;
            BorderHeight = ratioY * BorderHeight;
            BorderWidth = ratioX * BorderWidth;

            return ResizeTransformationResult.ViewModified;
        }

        /// <summary>Resizes the border where view exists and positions view at new provided location.</summary>
        /// <param name="newBorderWidth">New horizontal width of the border.</param>
        /// <param name="newBorderHeight">New vertical width of the border.</param>
        /// <param name="viewHorizontalOffset">Horizontal offset from border origin [0,0] where view should be positioned.</param>
        /// <param name="viewVerticalOffset">Vertical offset from border origin [0,0] where view should be positioned.</param>
        /// <returns><see cref="ResizeTransformationResult.ViewUnmodified"/> if new border is not fitting into view or view with provided offset is out of border bounds, otherwise <see cref="ResizeTransformationResult.ViewModified"/></returns>
        public ResizeTransformationResult TryResizeBorder(float newBorderWidth, float newBorderHeight, float viewHorizontalOffset, float viewVerticalOffset) {

            if (newBorderWidth < ViewWidth || newBorderHeight < ViewHeight) {
                return ResizeTransformationResult.ViewUnmodified;
            }

            var originalViewMatrix = _viewMatrix;
            var originalCanvasWidth = BorderWidth;
            var originalCanvasHeight = BorderHeight;

            _viewMatrix.TransX = 0;
            _viewMatrix.TransY = 0;
            BorderWidth = newBorderWidth;
            BorderHeight = newBorderHeight;

            if (IsOutOfBounds((-viewHorizontalOffset * _viewMatrix.ScaleX, -viewVerticalOffset * _viewMatrix.ScaleY))) {
                _viewMatrix = originalViewMatrix;
                BorderWidth = originalCanvasWidth;
                BorderHeight = originalCanvasHeight;
                return ResizeTransformationResult.ViewUnmodified;
            }

            _viewMatrix.TransX = -viewHorizontalOffset * _viewMatrix.ScaleX;
            _viewMatrix.TransY = -viewVerticalOffset * _viewMatrix.ScaleY;
            return ResizeTransformationResult.ViewModified;
        }

        /// <summary>Resizes the border where view exists. If view's position would be out of border bounds, view is transitioned.</summary>
        /// <param name="newBorderWidth">New horizontal width of the border.</param>
        /// <param name="newBorderHeight">New vertical width of the border.</param>
        /// <returns><see cref="ResizeTransformationResult.ViewUnmodified"/> if new border is not fitting into view, otherwise <see cref="ResizeTransformationResult.ViewModified"/>.</returns>
        public ResizeTransformationResult TryResizeBorder(float newBorderWidth, float newBorderHeight) {

            if (newBorderWidth < ViewWidth || newBorderHeight < ViewHeight) {
                return ResizeTransformationResult.ViewUnmodified;
            }

            BorderWidth = newBorderWidth;
            BorderHeight = newBorderHeight;

            TranslateIfOutOfBounds();
            return ResizeTransformationResult.ViewModified;
        }

        /// <summary>Transforms view by translating it with provided vector. If view would leave border, transformation is not applied.</summary>
        /// <param name="translationVector">Vector in which direction is the move translated.</param>
        /// <returns><see cref="TranslationTransformationResult.ViewUnmodified"/> if translated view would leave border; otherwise <see cref="TranslationTransformationResult.ViewModified"/>.</returns>
        public TranslationTransformationResult TryTranslate(SKPoint translationVector) {

            if (IsOutOfBounds((- translationVector.X, - translationVector.Y))) {
                return TranslationTransformationResult.ViewUnmodified;
            }

            _viewMatrix.TransX += - translationVector.X;
            _viewMatrix.TransY += - translationVector.Y;

            return TranslationTransformationResult.ViewModified;
        }

        /// <summary>Transforms view by scaling it in both axes with added delta. Scale values applied only in positive numbers greater or equal one.</summary>
        /// <param name="origin">Point of the current view which is not transformed by scale operation. If scale operation leaves border bounds, origin is also transformed.</param>
        /// <param name="delta">Difference between original and new scale</param>
        /// <throws><see cref="ArgumentOutOfRangeException"/> if <paramref name="origin"/> is not present in view.</throws>
        /// <returns><see cref="ScaleTransformationResult.ViewModifiedWithSameOrigin"/> if view is modified and <paramref name="origin"/> in same view's position.
        /// <see cref="ScaleTransformationResult.ViewModifiedWithTransformedOrigin"/> if view is modified and <paramref name="origin"/> was transformed as view would be out of border bounds.
        /// <see cref="ScaleTransformationResult.ViewUnmodified"/> otherwise.</returns>
        public ScaleTransformationResult TryScale(SKPoint origin, float delta) {

            if (PointOutOfViewBounds(origin)) {
                throw new ArgumentOutOfRangeException($"Point {origin} is not in view [0,{ViewWidth}]x[0,{ViewHeight}]");
            }

            if (Math.Abs(_viewMatrix.ScaleX - 1.0f) < 0.0001 && delta < 1) {
                return ScaleTransformationResult.ViewUnmodified;
            }

            if (!ScaleWithDeltaGreaterThanIdentityScale(delta)) {
                delta = DeltaToResetScaleToIdentity();
            }

            Scale(origin, delta);

            if (delta < 0 && TranslateIfOutOfBounds()) {
                return ScaleTransformationResult.ViewModifiedWithTransformedOrigin;
            }

            return ScaleTransformationResult.ViewModifiedWithSameOrigin;
        }

        /// <summary>Scales current canvas view in both axes.</summary>
        /// <param name="origin">Point of the current view which is not transformed by the scale operation.</param>
        /// <param name="delta">Difference between original and new scale.</param>
        private void Scale(SKPoint origin, float delta) {

            var (newScaleX, newScaleY) = (
                _viewMatrix.ScaleX + delta,
                _viewMatrix.ScaleY + delta);

            var (scaledOriginX, scaledOriginY) =
                ((origin.X - _viewMatrix.TransX) * (newScaleX / _viewMatrix.ScaleX),
                 (origin.Y - _viewMatrix.TransY) * (newScaleY / _viewMatrix.ScaleY));

            var (newTransX, newTransY) = (
                -(scaledOriginX - origin.X),
                -(scaledOriginY - origin.Y));

            _viewMatrix = SKMatrix.MakeIdentity();
            _viewMatrix.ScaleX = newScaleX;
            _viewMatrix.ScaleY = newScaleY;
            _viewMatrix.TransX = newTransX;
            _viewMatrix.TransY = newTransY;
        }

        /// <summary>Converts location of point in view to it's canvas location.</summary>
        /// <param name="viewLocation">Location of point in view.</param>
        /// <returns>Location of point in canvas.</returns>
        /// <throws><see cref="ArgumentOutOfRangeException"/> if <paramref name="viewLocation"/> is not present in view.</throws>
        public SKPoint ConvertViewPositionToContentPosition(SKPoint viewLocation) {

            if (viewLocation.X < 0 || viewLocation.X > ViewWidth ||
                viewLocation.Y < 0 || viewLocation.Y > ViewHeight) {
                throw new ArgumentOutOfRangeException($"ViewDateTimeInterval does not contain point {viewLocation}");
            }

            return new SKPoint {
                X = (-ViewMatrix.TransX + viewLocation.X) / ViewMatrix.ScaleX,
                Y = (-ViewMatrix.TransY + viewLocation.Y) / ViewMatrix.ScaleY
            };
        }

        /// <summary>Computes border width from <see cref="DateTimeContext"/> and <see cref="ViewWidth"/> values.
        /// Can be used to get the value for <see cref="ViewModifier"/> construction.</summary>
        /// <param name="viewWidth">Horizontal length of view.</param>
        /// <param name="dateTimeContext"><see cref="DateTimeContext.ContentDateTimeInterval"/> represents border values.
        /// <see cref="DateTimeContext.ViewDateTimeInterval"/> represents view values.</param>
        /// <returns>Length of <see cref="BorderWidth"/> determined from parameters.</returns>
        public static float ComputeBorderWidth(float viewWidth, DateTimeContext dateTimeContext) {

            var start = dateTimeContext.ContentDateTimeInterval.GetMultiple(dateTimeContext.ViewDateTimeInterval.Start);
            var end = dateTimeContext.ContentDateTimeInterval.GetMultiple(dateTimeContext.ViewDateTimeInterval.End);
            var borderWidth = viewWidth / (end - start);

            return borderWidth;
        }

        private bool PointOutOfViewBounds(SKPoint point) {
            return (point.X < 0 || point.X > ViewWidth ||
                    point.Y < 0 || point.Y > ViewHeight);
        }

        private float DeltaToResetScaleToIdentity() {
            return 1 - _viewMatrix.ScaleX;
        }

        private bool ScaleWithDeltaGreaterThanIdentityScale(float delta) {
            const float identityScale = 1.0f;
            return !(_viewMatrix.ScaleX + delta < identityScale);
        }

        private bool TranslateIfOutOfBounds() {

            bool modified = false;

            if (_viewMatrix.TransX > 0) {
                _viewMatrix.TransX = 0;
                modified = true;
            }

            if (_viewMatrix.TransY > 0) {
                _viewMatrix.TransY = 0;
                modified = true;
            }

            if (-_viewMatrix.TransX + ViewWidth > BorderWidth * _viewMatrix.ScaleX) {
                _viewMatrix.TransX = -BorderWidth * _viewMatrix.ScaleX + ViewWidth;
                modified = true;
            }

            if (-_viewMatrix.TransY + ViewHeight > BorderHeight * _viewMatrix.ScaleY) {
                _viewMatrix.TransY = -BorderHeight * _viewMatrix.ScaleY + ViewHeight;
                modified = true;
            }

            return modified;
        }

        private bool IsOutOfBounds((float x, float y) translation) {

            var (widthStart, heightStart) = (-_viewMatrix.TransX, -_viewMatrix.TransY);
            var (widthEnd, heightEnd) = (widthStart + ViewWidth, heightStart + ViewHeight);
            var (scaledWidth, scaledHeight) = (BorderWidth * _viewMatrix.ScaleX, BorderHeight * _viewMatrix.ScaleY);

            if (widthStart - translation.x < 0) return true;
            if (heightStart - translation.y < 0) return true;
            if (widthEnd - translation.x > scaledWidth) return true;
            if (heightEnd - translation.y > scaledHeight) return true;

            return false;
        }
    }
}
