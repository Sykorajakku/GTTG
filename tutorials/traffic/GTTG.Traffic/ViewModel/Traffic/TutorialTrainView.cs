using GTTG.Model.Lines;
using GTTG.Model.Model.Traffic;
using GTTG.Model.Strategies;
using GTTG.Model.Strategies.Types;
using GTTG.Model.ViewModel.Traffic;
using GTTG.Traffic.Components;

namespace GTTG.Traffic.ViewModel.Traffic {

    public class TutorialTrainView : StrategyTrainView<Strategy, Train> {

        public TutorialTrainView(Train train, ITrainPath trainPath, Strategy strategy) 
            : base(train, trainPath, strategy) {

        }

        public override void UpdateTrainViewContent() {
            base.UpdateTrainViewContent();

            foreach (var trainEvent in Train.Schedule) {

                var trainEventPlacement = new TrainEventPlacement(trainEvent, AnglePlacement.Acute);
                Strategy.TrackStrategyManager.Add(trainEventPlacement, new TimeComponent(trainEvent));
            }
        }
    }
}
