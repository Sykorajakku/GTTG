// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Time;

namespace GTTG.Core.Component {

    /// <summary>
    /// Represents scene of outer content rectangle with inner view rectangle inside.
    /// Modifications changes layout of those rectangles. <see cref="ContentHeight"/> maps to <see cref="ContentDateTimeInterval"/> and
    /// <see cref="ViewHeight"/> maps to <see cref="ViewDateTimeInterval"/>, forming <see cref="DateTimeContext"/>.
    /// </summary>
    public class GraphicalComponent : ObservableObject, IViewProvider {

        private DateTimeContext _dateTimeContext;
        private float _dpiScale;
        private float _scale;

        /// <inheritdoc />
        public DateTimeContext DateTimeContext {
            get => _dateTimeContext;
            protected set => Update(ref _dateTimeContext, value);
        }

        /// <inheritdoc />
        public float DpiScale {
            get => _dpiScale;
            set => Update(ref _dpiScale, value);
        }

        /// <inheritdoc />
        public float Scale {
            get => _scale;
            protected set => Update(ref _scale, value);
        }

        /// <summary><see cref="Time.DateTimeContext.ViewDateTimeInterval"/> value of <see cref="DateTimeContext"/>.</summary>
        public DateTimeInterval ViewDateTimeInterval => DateTimeContext.ViewDateTimeInterval;
        
        /// <summary><see cref="Time.DateTimeContext.ContentDateTimeInterval"/> value of <see cref="DateTimeContext"/>.</summary>
        public DateTimeInterval ContentDateTimeInterval => DateTimeContext.ContentDateTimeInterval;
        
        /// <inheritdoc />
        public SKMatrix ContentMatrix => _viewModifier.ViewMatrix;
        
        /// <inheritdoc />
        public float ContentWidth => _viewModifier.BorderWidth;
        
        /// <inheritdoc />
        public float ContentHeight => _viewModifier.BorderHeight;
        
        /// <inheritdoc />
        public float ViewWidth => _viewModifier.ViewWidth;
        
        /// <inheritdoc />
        public float ViewHeight => _viewModifier.ViewHeight;

        private readonly ViewModifier _viewModifier;

        /// <summary>Creates graphical component which must configured be modified for further use.</summary>
        public GraphicalComponent() {

            _viewModifier = new ViewModifier(100, 100, 100, 100);
            
            var start = DateTime.Now;
            var end = start.AddHours(1);
            _dateTimeContext = new DateTimeContext(new DateTimeInterval(start, end), new DateTimeInterval(start, end));
            _dpiScale = _scale = 1;
        }

        /// <summary>Creates graphical component configured by provided parameters.</summary>
        /// <param name="viewModifier">Instance of <see cref="ViewModifier"/> providing component sizes and used as engine.</param>
        /// <param name="dateTimeContext"><see cref="DateTimeContext"/> of component.</param>
        public GraphicalComponent(ViewModifier viewModifier, DateTimeContext dateTimeContext) {

            _dateTimeContext = dateTimeContext;
            _viewModifier = viewModifier;
            _viewModifier.TryTranslate(new SKPoint(GetTransitionToViewStart(), 0));
            _dpiScale = _scale = 1;
        }

        /// <summary>
        /// Resizes the view area. Global area's <see cref="ContentWidth"/> and <see cref="ContentHeight"/> are
        /// proportionally resized to have same view:global ratio as before. View is placed in global area proportionally on the same position.
        /// </summary>
        /// <param name="viewWidth">New <see cref="ViewWidth"/> value.</param>
        /// <param name="viewHeight">New <see cref="ViewHeight"/> value.</param>
        /// <returns><see cref="ResizeTransformationResult.ViewModified"/> if successful; otherwise <see cref="ResizeTransformationResult.ViewUnmodified"/>.</returns>
        public ResizeTransformationResult TryResizeView(float viewWidth, float viewHeight) {
            return _viewModifier.TryResizeView(viewWidth, viewHeight);
        }

        /// <summary>
        /// Resizes the content area. View area is not resized or moved.
        /// Modification is not executed if view area would exceed content area after modification.
        /// </summary>
        /// <param name="globalWidth">New <see cref="ContentWidth"/> value.</param>
        /// <param name="globalHeight">New <see cref="ContentHeight"/> value.</param>
        /// <returns><see cref="ResizeTransformationResult.ViewModified"/> if successful; otherwise <see cref="ResizeTransformationResult.ViewUnmodified"/>.</returns>
        public ResizeTransformationResult TryResizeContentArea(float globalWidth, float globalHeight) {
            return _viewModifier.TryResizeBorder(globalWidth, globalHeight);
        }

        /// <summary>Moves the view area in global area by translating it with provided vector. If view would leave the content area, translation is not applied.</summary>
        /// <param name="translationVector">Vector represented by <see cref="SKPoint"/> in which points of view area are translated. Y-axis increases downwards, X-axis increases left to right.</param>
        /// <returns><see cref="TranslationTransformationResult.ViewUnmodified"/> if translated view would leave content area; otherwise <see cref="TranslationTransformationResult.ViewModified"/>.</returns>
        public TranslationTransformationResult TryTranslate(SKPoint translationVector) {

            var result = _viewModifier.TryTranslate(translationVector);
            if (result == TranslationTransformationResult.ViewModified) {
                UpdateViewTimeContext();
            }
            return result;
        }

        /// <summary>
        /// Changes view area by scaling <see cref="ViewWidth"/> and <see cref="ViewHeight"/>.
        /// Scaling is applied on <paramref name="origin"/> point in view which has the same visual position in scaled view.
        /// If view area after scaling exceeds content area, translation of view area is applied with <paramref name="origin"/> no longer being on visually same position.
        /// </summary>
        /// <param name="origin">Point in view which has the same visual position after scaling.</param>
        /// <param name="delta">Difference between original and new <see cref="Scale"/> value. If new value is 3 and old is 1, difference is 2. In reverse -2.</param>
        /// <throws><see cref="ArgumentOutOfRangeException"/> if <paramref name="origin"/> is not present in view area.</throws>
        /// <returns><see cref="ScaleTransformationResult.ViewModifiedWithSameOrigin"/> if view is modified and <paramref name="origin"/> in same view's position.
        /// <see cref="ScaleTransformationResult.ViewModifiedWithTransformedOrigin"/> if view is modified and <paramref name="origin"/> was transformed as view would be out of border bounds.
        /// <see cref="ScaleTransformationResult.ViewUnmodified"/> otherwise.</returns>
        public ScaleTransformationResult TryScale(SKPoint origin, float delta) {

            var result = _viewModifier.TryScale(origin, delta);
            if (result != ScaleTransformationResult.ViewUnmodified) {

                Scale = _viewModifier.ViewMatrix.ScaleX;
                UpdateViewTimeContext();
            }
            return result;
        }

        /// <summary>
        /// Assigns new <see cref="DateTimeInterval"/> to <see cref="ViewHeight"/> in view area. <see cref="ContentHeight"/> is proportionally
        /// resized to match new <see cref="DateTimeContext"/>. Vertical position of view area in content area is not modified.
        /// </summary>
        /// <param name="viewDateTimeInterval">New <see cref="ViewDateTimeInterval"/> value. If not in <see cref="ContentDateTimeInterval"/>, modification is not applied.</param>
        /// <returns><see cref="TimeModificationResult.TimeModified"/> if successful; otherwise <see cref="TimeModificationResult.TimeUnmodified"/>.</returns>
        public TimeModificationResult TryChangeViewTime(DateTimeInterval viewDateTimeInterval) {

            if (!DateTimeContext.ContentDateTimeInterval.Contains(viewDateTimeInterval)
                || DateTimeContext.ViewDateTimeInterval.Equals(viewDateTimeInterval)) {
                return TimeModificationResult.TimeUnmodified;
            }

            var endMultiple = Math.Abs(viewDateTimeInterval.GetMultiple(DateTimeContext.ContentDateTimeInterval.End));
            var startMultiple = Math.Abs(viewDateTimeInterval.GetMultiple(DateTimeContext.ContentDateTimeInterval.Start));

            var resizeResult = ResizeHorizontalBounds(startMultiple, endMultiple);
            if (resizeResult == ResizeTransformationResult.ViewModified) {
                DateTimeContext = new DateTimeContext(DateTimeContext.ContentDateTimeInterval, viewDateTimeInterval);
                return TimeModificationResult.TimeModified;
            }

            return TimeModificationResult.TimeUnmodified;
        }

        /// <summary>
        /// Assigns new <see cref="DateTimeInterval"/> to <see cref="ContentHeight"/> in content area. Does not modify view area.
        /// </summary>
        /// <param name="contentDateTimeInterval">New <see cref="ContentDateTimeInterval"/> value. If not in <see cref="ViewDateTimeInterval"/>, modification is not applied.</param>
        /// <returns><see cref="TimeModificationResult.TimeModified"/> if successful; otherwise <see cref="TimeModificationResult.TimeUnmodified"/>.</returns>
        public TimeModificationResult TryChangeContentTime(DateTimeInterval contentDateTimeInterval) {

            var newViewTimeInterval = DateTimeContext.ViewDateTimeInterval;

            if (contentDateTimeInterval.Equals(DateTimeContext.ContentDateTimeInterval) ||
                !contentDateTimeInterval.Contains(DateTimeContext.ViewDateTimeInterval)) {
                return TimeModificationResult.TimeUnmodified;
            }

            var startMultiple = Math.Abs(newViewTimeInterval.GetMultiple(contentDateTimeInterval.Start));
            var endMultiple = Math.Abs(newViewTimeInterval.GetMultiple(contentDateTimeInterval.End));

            var resizeResult = ResizeHorizontalBounds(startMultiple, endMultiple);
            if (resizeResult == ResizeTransformationResult.ViewModified) {

                DateTimeContext = new DateTimeContext(contentDateTimeInterval, newViewTimeInterval);
                return TimeModificationResult.TimeModified;
            }

            return TimeModificationResult.TimeUnmodified;
        }

        /// <summary>
        /// Assigns new <see cref="ViewDateTimeInterval"/> to <see cref="ViewHeight"/> and then proportionally resizes <see cref="ContentHeight"/> to map itself
        /// to <see cref="ContentDateTimeInterval"/> with view area in right position.
        /// </summary>
        /// <param name="dateTimeContext">New <see cref="DateTimeContext"/>.</param>
        /// <returns><see cref="TimeModificationResult.TimeModified"/> if successful; otherwise <see cref="TimeModificationResult.TimeUnmodified"/>.</returns>
        public TimeModificationResult TryChangeDateTimeContext(DateTimeContext dateTimeContext) {

            DateTimeContext = dateTimeContext;

            var endMultiple = Math.Abs(DateTimeContext.ViewDateTimeInterval.GetMultiple(DateTimeContext.ContentDateTimeInterval.End));
            var startMultiple = Math.Abs(DateTimeContext.ViewDateTimeInterval.GetMultiple(DateTimeContext.ContentDateTimeInterval.Start));

            var resizeResult = ResizeHorizontalBounds(startMultiple, endMultiple);
            return resizeResult == ResizeTransformationResult.ViewModified ? TimeModificationResult.TimeModified : TimeModificationResult.TimeUnmodified;
        }

        /// <summary>Changes <see cref="ContentHeight"/>.</summary>
        /// <param name="height">New height of <see cref="ContentHeight"/>.</param>
        /// <returns><see cref="ResizeTransformationResult.ViewModified"/> if applied; otherwise <see cref="ResizeTransformationResult.ViewUnmodified"/>.</returns>
        public ResizeTransformationResult TryChangeBorderHeight(float height) {
            return _viewModifier.TryResizeBorder(_viewModifier.BorderWidth, height);
        }

        /// <inheritdoc />
        public SKRect GetViewRect() {

            return SKRect.Create(
                (-ContentMatrix.TransX) / ContentMatrix.ScaleX,
                (-ContentMatrix.TransY) / ContentMatrix.ScaleX,
                _viewModifier.ViewWidth / ContentMatrix.ScaleX,
                _viewModifier.ViewHeight / ContentMatrix.ScaleY);
        }

        /// <inheritdoc />
        public DateTime GetDateTimeFromContent(float globalHorizontalPosition) {

            var canvasPercentage = globalHorizontalPosition / _viewModifier.BorderWidth;
            var millisecondsFromStart = DateTimeContext.ContentDateTimeInterval.TimeSpan.TotalMilliseconds * canvasPercentage;
            return DateTimeContext.ContentDateTimeInterval.Start.AddMilliseconds(millisecondsFromStart);
        }

        /// <inheritdoc />
        public DateTime GetDateTimeFromView(float viewHorizontalPosition) {

            var canvasPercentage = viewHorizontalPosition / _viewModifier.ViewWidth;
            var millisecondsFromStart = DateTimeContext.ViewDateTimeInterval.TimeSpan.TotalMilliseconds * canvasPercentage;
            return DateTimeContext.ViewDateTimeInterval.Start.AddMilliseconds(millisecondsFromStart);
        }

        /// <inheritdoc />
        public float GetContentHorizontalPosition(DateTime dateTime) {

            var timePercentage = DateTimeContext.ContentDateTimeInterval.GetMultiple(dateTime);
            return _viewModifier.BorderWidth * timePercentage;
        }

        /// <inheritdoc />
        public float GetViewHorizontalPosition(DateTime dateTime) {

            var timePercentage = DateTimeContext.ViewDateTimeInterval.GetMultiple(dateTime);
            return _viewModifier.ViewWidth * timePercentage;
        }

        /// <inheritdoc />
        public SKPoint ConvertViewToContentLocation(SKPoint viewPoint) {
            return _viewModifier.ConvertViewPositionToContentPosition(viewPoint);
        }

        /// <inheritdoc />
        public SKPoint ConvertViewToContentLocation(float x, float y) {
            return _viewModifier.ConvertViewPositionToContentPosition(new SKPoint(x, y));
        }

        private void UpdateViewTimeContext() {

            var unscaledStart = -ContentMatrix.TransX / ContentMatrix.ScaleX;
            var newViewStart = GetDateTimeFromContent(unscaledStart);
            var newViewEnd = GetDateTimeFromContent(unscaledStart + _viewModifier.ViewWidth / ContentMatrix.ScaleX);

            // handle possible precision loss for end value
            if (newViewEnd > DateTimeContext.ContentDateTimeInterval.End &&
                (newViewEnd - DateTimeContext.ContentDateTimeInterval.End).Milliseconds < 1000) {

                newViewEnd = DateTimeContext.ContentDateTimeInterval.End;
            }

            // handle possible precision loss for start value
            if (newViewStart < DateTimeContext.ContentDateTimeInterval.Start &&
                (DateTimeContext.ContentDateTimeInterval.Start - newViewStart).Milliseconds < 1000) {

                newViewStart = DateTimeContext.ContentDateTimeInterval.Start;
            }

            var newViewInterval = new DateTimeInterval(newViewStart, newViewEnd);

            DateTimeContext = new DateTimeContext(DateTimeContext.ContentDateTimeInterval, newViewInterval);
        }

        private float GetTransitionToViewStart() {

            var timePercentage = DateTimeContext.ContentDateTimeInterval.GetMultiple(DateTimeContext.ViewDateTimeInterval.Start);
            return _viewModifier.BorderWidth * timePercentage;
        }

        // multiples of view width added to it's start and end
        private ResizeTransformationResult ResizeHorizontalBounds(float startMultiple, float endMultiple) {

            var viewWidth = _viewModifier.ViewWidth;
            var canvasLengthFromEndView = endMultiple * viewWidth - viewWidth;
            var canvasLengthFromStartView = startMultiple * viewWidth;

            var newCanvasBounds = canvasLengthFromStartView + viewWidth + canvasLengthFromEndView;

            return _viewModifier.TryResizeBorder(newCanvasBounds, _viewModifier.BorderHeight,
                canvasLengthFromStartView,
                -_viewModifier.ViewMatrix.TransY);
        }
    }
}
