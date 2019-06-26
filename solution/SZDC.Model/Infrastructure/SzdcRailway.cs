using System.Collections.Generic;

using GTTG.Model.Model.Infrastructure;
using SZDC.Model.Infrastructure.StationContent;

namespace SZDC.Model.Infrastructure {

    /// <inheritdoc />
    public class SzdcRailway : Railway {

        public string RailwayNumber { get; }
        
        /// <summary>
        /// Contains data of stations connected to this railway (stations can have different kilometers distances in multiple railways)
        /// </summary>
        public IDictionary<Station, SzdcStationInfo> StationInfo { get; }

        public SzdcRailway(string railwayNumber, IEnumerable<SzdcStation> stations, IDictionary<Station, SzdcStationInfo> stationInfo) 
            : base(stations) {

            StationInfo = stationInfo;
            RailwayNumber = railwayNumber;
        }
    }
}
