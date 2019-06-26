// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using GTTG.Model.Model.Traffic;

namespace GTTG.Model.ViewModel.Traffic {

    /// <summary>
    /// Factory for train view classes.
    /// </summary>
    /// <typeparam name="TTrainView">Train view class deriving from <see cref="TrainView{TTrain}"/>.</typeparam>
    /// <typeparam name="TTrain">Train class deriving from <see cref="Train"/>.</typeparam>
    public interface ITrainViewFactory<out TTrainView, in TTrain>
        where TTrainView : TrainView<TTrain>
        where TTrain : Train { 

        /// <summary>
        /// Creates specific implementation of train view from train instance.
        /// </summary>
        /// <param name="train"><see cref="Train"/> instance which is visualized by <typerefparam name="TTrainView"/>.</param>
        /// <returns></returns>
        TTrainView CreateTrainView(TTrain train);
    }
}
