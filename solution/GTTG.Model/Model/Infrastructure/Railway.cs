// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.Immutable;

using GTTG.Core.Base;

namespace GTTG.Model.Model.Infrastructure {

    /// <summary>Represents railway with stations which contains tracks.</summary>
    public class Railway : ObservableObject {

        private ImmutableArray<Station> _stations;

        /// <summary>Stations in railway.</summary>
        public ImmutableArray<Station> Stations {
            get => _stations;
            set => Update(ref _stations, value);
        }

        /// <summary>Initializes a new instance of the <see cref="Railway"/> with stations.</summary>
        /// <param name="stations">Stations placed in <see cref="Stations"/>.</param>
        public Railway(IEnumerable<Station> stations) {
            Stations = ImmutableArray.CreateRange(stations);
        }
    }
}
