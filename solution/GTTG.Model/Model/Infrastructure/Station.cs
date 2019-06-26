// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.Immutable;

using GTTG.Core.Base;

namespace GTTG.Model.Model.Infrastructure {

    /// <summary>Represents station which contains tracks.</summary>
    public class Station : ObservableObject {

        private ImmutableArray<Track> _tracks;

        /// <summary>Tracks in the station.</summary>
        public ImmutableArray<Track> Tracks {
            get => _tracks;
            set => Update(ref _tracks, value);
        }

        /// <summary>Initializes a new instance of the <see cref="Station"/> with tracks.</summary>
        /// <param name="tracks">Tracks placed in <see cref="Tracks"/>.</param>
        public Station(IEnumerable<Track> tracks) {
            Tracks = ImmutableArray.CreateRange(tracks);
        }
    }
}
