// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using GTTG.Core.Base;
using GTTG.Model.ViewModel.Traffic;

namespace GTTG.Model.Strategies  {

    /// <summary>
    /// Represents contract for strategy implementation used by <see cref="TrainView{TTrain}"/>.
    /// </summary>
    public interface IStrategy : IVisual {

        /// <summary>
        /// Rearranges visuals in strategy.
        /// </summary>
        void Dock();
        
        /// <summary>
        /// Removes all visuals from strategy.
        /// </summary>
        void Clear();
    }
}
