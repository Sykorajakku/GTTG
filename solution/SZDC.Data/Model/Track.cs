using System.ComponentModel.DataAnnotations;
using SZDC.Model.Infrastructure;

namespace SZDC.Data.Model {

    public class Track {

        [Key]
        public long Id { get; set; }
        public string Number { get; set; }
        public TrackType? TrackType { get; set; }
    }
}
