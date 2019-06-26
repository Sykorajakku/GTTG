using SkiaSharp;

using GTTG.Core.Utils;
using GTTG.Model.Strategies.Converters;
using GTTG.Model.Strategies.Types;
using SZDC.Model.Components;
using SZDC.Model.Events;

namespace SZDC.Model.Views.Traffic {

    public class TrainNumberPlacementSelector {

        private readonly TrainEventPlacementConverter _trainMovementTypeConverter;

        public TrainNumberPlacementSelector(TrainEventPlacementConverter trainMovementTypeConverter) {
            _trainMovementTypeConverter = trainMovementTypeConverter;
        }

        public void AddTrainNumberComponentsTo(SzdcTrainView trainView) {

            var departureOrPassageCount = 0;
            var longestVectorLength = 0f;
            var longestVector = SKPoint.Empty;
            var secondLongestVector = SKPoint.Empty;
            SzdcTrainEvent longestTrainEvent = null;
            SzdcTrainEvent secondLongestTrainEvent = null;

            SzdcTrainEvent lastDepartureOrPassageEvent = null;

            foreach (var trainEvent in trainView.Train.Schedule) {

                /*
                 * place train numbers to departure / passage events (for each valid schedule at least one exists)
                 * as train continues maybe to other railways, skip last departure / passage point
                 */
                if (trainEvent is SzdcTrainEvent szdcTrainEvent && !szdcTrainEvent.IsArrival) {

                    departureOrPassageCount++;

                    lastDepartureOrPassageEvent = szdcTrainEvent;
                    var vectorToNextMovementEvent = _trainMovementTypeConverter
                        .ComputeVectorFromEvent(lastDepartureOrPassageEvent).VectorFromBase;
                    var vectorToNextMovementEventLength = PlacementUtils.ComputesVectorLength(vectorToNextMovementEvent);

                    if (vectorToNextMovementEventLength > longestVectorLength) {

                        longestVectorLength = vectorToNextMovementEventLength;

                        secondLongestTrainEvent = longestTrainEvent;
                        longestTrainEvent = szdcTrainEvent;

                        secondLongestVector = longestVector;
                        longestVector = vectorToNextMovementEvent;
                    }
                }
            }

            if (longestTrainEvent != null &&
                (longestTrainEvent != lastDepartureOrPassageEvent || departureOrPassageCount == 1)) {

                var containerMovementEventType = new TrainEventPlacement(longestTrainEvent, GetAnglePlacement(longestVector));
                trainView.Strategy.StationStrategyManager.Add(containerMovementEventType, new TrainNumberComponent(trainView));
            }

            if (secondLongestTrainEvent != null &&
                longestTrainEvent != secondLongestTrainEvent) {

                var containerMovementEventType = new TrainEventPlacement(secondLongestTrainEvent, GetAnglePlacement(secondLongestVector));
                trainView.Strategy.StationStrategyManager.Add(containerMovementEventType, new TrainNumberComponent(trainView));
            }
        }

        private AnglePlacement GetAnglePlacement(SKPoint vector) {
            // y greater than zero is down
            return vector.Y > 0 ? AnglePlacement.Acute : AnglePlacement.Obtuse;
        }
    }
}
