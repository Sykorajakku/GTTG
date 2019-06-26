using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SZDC.Data.Model {

    public class RailwaySection {

        [Key]
        public long Id { get; set; }
        public List<RailwaySectionStation> Stations { get; set; }
    }
}
