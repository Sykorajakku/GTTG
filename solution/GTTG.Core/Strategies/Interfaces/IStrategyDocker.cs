// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace GTTG.Core.Strategies.Interfaces {

    /// <summary>Implements strategy by docking provided elements into segments.</summary>
    public interface IStrategyDocker {

        /// <summary>Docks elements added to strategy managed by this docker.</summary>
        void Dock();
    }
}
