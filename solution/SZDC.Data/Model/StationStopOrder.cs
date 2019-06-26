using System.ComponentModel.DataAnnotations;

namespace SZDC.Data.Model {

    public class StationStopOrder {

        [Key]
        public long Id { get; set; }

        [Required]
        public StationStop StationStop { get; set; }

        [Required]
        public int Order { get; set; }
    }
}
