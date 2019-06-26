using System.ComponentModel.DataAnnotations;

namespace SZDC.Data.Model {

    public class RailwaySectionStation {

        [Key]
        public long Id { get; set; }

        [Required]
        public Station Station { get; set; }

        public double KilometersInSegment { get; set; }

        public string KilometersString { get; set; }

        [Required]
        public int StationOrder { get; set; }
    }

}
