using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace SZDC.Data.Model {

    public class Station {

        [Key]
        public string Name { get; set; }
        public List<OrderedTrack> Tracks { get; set; }
    }
}
