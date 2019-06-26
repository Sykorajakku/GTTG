using System.ComponentModel.DataAnnotations;

namespace SZDC.Data.Model {

    public class StaticTrainEvent {

        [Key]
        public long Id { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public bool HasMoreThan30Seconds { get; set; }

        public override bool Equals(object obj) {

            if (!(obj is StaticTrainEvent other)) {
                return false;
            }
            return Hours == other.Hours && Minutes == other.Minutes && HasMoreThan30Seconds == other.HasMoreThan30Seconds;
        }

        public override int GetHashCode() {
            var hashCode = -677674611;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + Hours.GetHashCode();
            hashCode = hashCode * -1521134295 + Minutes.GetHashCode();
            hashCode = hashCode * -1521134295 + HasMoreThan30Seconds.GetHashCode();
            return hashCode;
        }

        public override string ToString() {
            char pl = HasMoreThan30Seconds ? '+' : ' ';
            return $"{Hours} : {Minutes} { pl }";
        }
    }
}
