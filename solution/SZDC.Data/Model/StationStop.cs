using System.ComponentModel.DataAnnotations;

namespace SZDC.Data.Model {

    public class StationStop {

        [Key]
        public long Id { get; set; }
        public Station Station { get; set; }
        public StaticTrainEvent Arrival { get; set; }
        public StaticTrainEvent Departure { get; set; }
    }
}
