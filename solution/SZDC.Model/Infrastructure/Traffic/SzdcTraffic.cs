using System.Collections.Generic;
using GTTG.Model.Model.Traffic;
using SZDC.Model.Infrastructure.Trains;

namespace SZDC.Model.Infrastructure.Traffic {

    public class SzdcTraffic : Traffic<SzdcTrain> {

        public SzdcTraffic(IEnumerable<SzdcTrain> trains) : base(trains) {
        }
    }
}
