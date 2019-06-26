using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SZDC.Data.Model {

    public class Railway {

        [Key]
        public string RailwayNumber { get; set; }
        public List<RailwaySection> RailwaySections { get; set; }
    }
}
