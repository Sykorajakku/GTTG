namespace SZDC.Model.Infrastructure.Trains {

    public enum TrainType {
        Nex, Pn, Mn, Vl, Sl, Ex, R, Sp, Os
    }

    public static class TrainTypeExtension {

        public static bool IsCargoType(this TrainType trainType) {

            switch (trainType) {
                case TrainType.Nex:
                case TrainType.Mn:
                case TrainType.Sl:
                case TrainType.Vl:
                case TrainType.Pn:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsPassengerType(this TrainType trainType) {

            switch (trainType) {
                case TrainType.Ex:
                case TrainType.R:
                case TrainType.Sp:
                case TrainType.Os:
                    return true;
                default:
                    return false;
            }
        }
    }
}
