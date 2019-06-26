// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace GTTG.Core.Component {

    /// <summary>
    /// Specifies the scale modification result.
    /// </summary>
    public enum ScaleTransformationResult {

        /// <summary>
        /// Equal to modification success with translated origin point which no longer has same position in view.
        /// </summary>
        ViewModifiedWithTransformedOrigin,

        /// <summary>
        /// Equal to modification success with origin point on same position in view.
        /// </summary>
        ViewModifiedWithSameOrigin,

        /// <summary>
        /// Equal to modification failure.
        /// </summary>
        ViewUnmodified
    }

    /// <summary>
    /// Specifies the translation modification result.
    /// </summary>
    public enum TranslationTransformationResult {

        /// <summary>
        /// Equal to modification success.
        /// </summary>
        ViewModified,

        /// <summary>
        /// Equal to modification failure.
        /// </summary>
        ViewUnmodified
    }

    /// <summary>
    /// Specifies the resize modification result.
    /// </summary>
    public enum ResizeTransformationResult {


        /// <summary>
        /// Equal to modification success.
        /// </summary>
        ViewModified,

        /// <summary>
        /// Equal to modification failure.
        /// </summary>
        ViewUnmodified
    }

    /// <summary>
    /// Specifies the time modification result.
    /// </summary>
    public enum TimeModificationResult {


        /// <summary>
        /// Equal to modification success.
        /// </summary>
        TimeModified,

        /// <summary>
        /// Equal to modification failure.
        /// </summary>
        TimeUnmodified
    }
}
