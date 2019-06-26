using System.Collections.Generic;
using GTTG.Model.Model.Infrastructure;

namespace SZDC.Model.Infrastructure {

    /// <inheritdoc />
    public class SzdcStation : Station {

        public string StationName { get; }

        public SzdcStation(IEnumerable<SzdcTrack> tracks, string stationName) 
            : base(tracks) {

            StationName = stationName;
        }
    }
}
