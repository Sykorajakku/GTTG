// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using GTTG.Model.Model.Infrastructure;
using GTTG.Model.ViewModel.Infrastructure.Stations;
using GTTG.Model.ViewModel.Infrastructure.Tracks;

namespace GTTG.Model.ViewModel.Infrastructure.Railways {

    /// <summary>
    /// Factory for railway view classes deriving from <see cref="RailwayView{TStationView,TTrackView}"/>.
    /// </summary>
    /// <typeparam name="TRailwayView">Implementation of railway view.</typeparam>
    /// <typeparam name="TStationView">Implementation of railway view contains stations views of <typeparamref name="TStationView"/>.</typeparam>
    /// <typeparam name="TTrackView"><typeparamref name="TStationView"/> contains track views of <typeparamref name="TTrackView"/>.</typeparam>
    public interface IRailwayViewFactory<out TRailwayView, TStationView, TTrackView>
        
        where TTrackView : TrackView
        where TStationView : StationView<TTrackView>
        where TRailwayView : RailwayView<TStationView, TTrackView> {

        /// <summary>
        /// Creates specific implementation of railway view from railway instance.
        /// </summary>
        /// <param name="railway"><see cref="Railway"/> instance which is backed by this view.</param>
        /// <returns>Implementation of railway view derived from <see cref="RailwayView{TStationView,TTrackView}"/>.</returns>
        TRailwayView CreateRailwayView(Railway railway);
    }
}
