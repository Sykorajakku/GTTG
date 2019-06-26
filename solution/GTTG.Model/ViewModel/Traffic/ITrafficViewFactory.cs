// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using GTTG.Model.Model.Traffic;

namespace GTTG.Model.ViewModel.Traffic {

    /// <summary>
    /// Factory for traffic view classes.
    /// </summary>
    /// <typeparam name="TTrafficView">Traffic view class deriving from <see cref="TrafficView{TTrainView,TTrain}"/>.</typeparam>
    /// <typeparam name="TTrainView">Train view class deriving from <see cref="TrainView{TTrain}"/>.</typeparam>
    /// <typeparam name="TTrain">Train class deriving from <see cref="Train"/>.</typeparam>
    public interface ITrafficViewFactory<out TTrafficView, TTrainView, TTrain>
        where TTrain : Train
        where TTrainView : TrainView<TTrain>
        where TTrafficView : TrafficView<TTrainView, TTrain> {

        /// <summary>
        /// Creates specific implementation of traffic view from traffic instance.
        /// </summary>
        /// <param name="traffic"><see cref="Traffic{TTrain}"/> instance visualized by this view.</param>
        /// <returns></returns>
        TTrafficView CreateTrafficView(Traffic<TTrain> traffic);
    }
}
