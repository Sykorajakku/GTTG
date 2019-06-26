// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.Immutable;

using GTTG.Core.Base;
using GTTG.Model.Model.Events;

namespace GTTG.Model.Model.Traffic {

    /// <summary>Train in railway.</summary>
    public class Train : ObservableObject {

        private ImmutableArray<TrainEvent> _schedule;

        /// <summary>Gets or sets current schedule of train.</summary>
        public ImmutableArray<TrainEvent> Schedule {
            get => _schedule;
            set => Update(ref _schedule, value);
        }

        /// <summary>Initializes a new instance of the <see cref="Train"/> with it's schedule.</summary>
        /// <param name="schedule">Collection of <see cref="TrainEvent"/> as actual schedule of the train.</param>
        public Train(IEnumerable<TrainEvent> schedule) {
            Schedule = ImmutableArray.CreateRange(schedule);
        }
    }
}
