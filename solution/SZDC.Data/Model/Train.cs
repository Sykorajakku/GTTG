using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

using SZDC.Model.Infrastructure.Trains;

namespace SZDC.Data.Model {

    public class Train {

        [Key]
        public int TrainNumber { get; set; }
        public TrainType TrainType { get; set; }
        public List<StationStopOrder> Schedule { get; set; }
    }
}
