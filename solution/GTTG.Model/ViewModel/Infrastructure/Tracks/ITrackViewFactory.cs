// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using GTTG.Model.Model.Infrastructure;

namespace GTTG.Model.ViewModel.Infrastructure.Tracks {

    /// <summary>
    /// Factory for creating visualizations of <see cref="Track"/> instances.
    /// </summary>
    /// <typeparam name="TTrackView">Visualization of tracks deriving from <see cref="TrackView"/>.</typeparam>
    public interface ITrackViewFactory<out TTrackView> where TTrackView : TrackView {

        /// <summary>
        /// Creates <typeparamref name="TTrackView"/> instance from <paramref name="track"/>
        /// </summary>
        /// <param name="track"><see cref="Track"/> to visualize.</param>
        /// <returns>Instance of <typeparamref name="TTrackView"/> as visualization of <paramref name="track"/></returns>
        TTrackView CreateTrackView(Track track);
    }
}
