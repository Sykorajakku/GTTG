using System.ComponentModel.DataAnnotations;

namespace SZDC.Data.Model {

    public class OrderedTrack {

        [Key]
        public long Id { get; set; }

        [Required]
        public Track Track { get; set; }

        [Required]
        public int Order { get; set; }
    }
}
