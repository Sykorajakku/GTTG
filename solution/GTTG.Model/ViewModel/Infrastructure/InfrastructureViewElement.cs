// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using GTTG.Core.Base;

namespace GTTG.Model.ViewModel.Infrastructure {

    /// <summary>
    /// Represents view element of infrastructure with infinite width.
    /// </summary>
    public abstract class InfrastructureViewElement : ViewElement {

        /// <summary>
        /// Creates infrastructure view element with infinite width.
        /// </summary>
        protected InfrastructureViewElement() {
            Width = float.PositiveInfinity;
        }
    }
}
