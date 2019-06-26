// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using GTTG.Model.Model.Infrastructure;
using GTTG.Model.ViewModel.Infrastructure.Tracks;

namespace GTTG.Model.ViewModel.Infrastructure.Stations {

    /// <summary>
    /// Factory for station view classes.
    /// </summary>
    /// <typeparam name="TStationView">Station view class deriving from <see cref="StationView{TTrackView}"/>.</typeparam>
    /// <typeparam name="TTrackView"><typeparamref name="TStationView"/> contains track views of <typeparamref name="TTrackView"/>.</typeparam>
    public interface IStationViewFactory<out TStationView, TTrackView>
        where TStationView : StationView<TTrackView>
        where TTrackView : TrackView {

        /// <summary>
        /// Creates specific implementation of station view from station instance.
        /// </summary>
        /// <param name="station"><see cref="Station"/> instance visualized by this view.</param>
        /// <returns>Implementation of station view derived from <see cref="StationView{TTrackView}"/>.</returns>
        TStationView CreateStationView(Station station);
    }
}
