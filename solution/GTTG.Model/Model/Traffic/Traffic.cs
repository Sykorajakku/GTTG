// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.Immutable;

using GTTG.Core.Base;

namespace GTTG.Model.Model.Traffic {

    /// <summary>Trains as traffic in <see cref="Infrastructure.Railway"/>.</summary>
    public class Traffic<TTrain> : ObservableObject where TTrain : Train {

        private ImmutableArray<TTrain> _trains;

        /// <summary>Trains in traffic.</summary>
        public ImmutableArray<TTrain> Trains {
            get => _trains;
            set => Update(ref _trains, value);
        }

        /// <summary>Initializes a new instance of the <see cref="Traffic{TTrain}"/> with trains.</summary>
        /// <param name="trains">Trains placed in <see cref="Trains"/>.</param>
        public Traffic(IEnumerable<TTrain> trains) {
            Trains = ImmutableArray.CreateRange(trains);
        }
    }
}
