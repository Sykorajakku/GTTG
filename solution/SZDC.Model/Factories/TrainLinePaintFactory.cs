using GTTG.Model.Lines;
using SZDC.Model.Factories.Exceptions;
using SZDC.Model.Infrastructure.Trains;

namespace SZDC.Model.Factories {

    public static class TrainLinePaintFactory {

        public static LinePaint CreateLinePaint(SzdcTrain train) {

            switch (train.TrainType) {
                case TrainType.Nex:
                    return new LinePaint(desiredStrokeWidth: 3, color: ViewConstants.CargoTrainPathColor);
                case TrainType.Pn:
                case TrainType.Mn:
                case TrainType.Vl:
                case TrainType.Sl:
                    return new LinePaint(desiredStrokeWidth: 2, color: ViewConstants.CargoTrainPathColor);
                case TrainType.Ex:
                case TrainType.R:
                case TrainType.Sp:
                    return new LinePaint(desiredStrokeWidth: 3, color: ViewConstants.PassengerTrainPathColor);
                case TrainType.Os:
                    return new LinePaint(desiredStrokeWidth: 2, color: ViewConstants.PassengerTrainPathColor);
                default:
                    throw new ViewFactoryException($"{nameof(TrainType)} {train.TrainType} not recognized.");
            }
        }
    }
}
