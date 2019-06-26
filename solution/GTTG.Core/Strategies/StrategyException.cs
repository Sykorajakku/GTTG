// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace GTTG.Core.Strategies {

    /// <summary>Initializes a new instance of the <see cref="StrategyException" /> class with a specified error message.</summary>
    public class StrategyException : ArgumentException {

        /// <inheritdoc />
        public StrategyException(string message) : base(message) {
        }
    }
}
