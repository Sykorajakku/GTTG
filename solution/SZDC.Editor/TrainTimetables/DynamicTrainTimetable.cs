using System.Collections.Immutable;
using System.Linq;
using Autofac;

using GTTG.Model.Model.Events;
using SZDC.Editor.Implementations;
using SZDC.Editor.Layers;
using SZDC.Editor.Services;
using SZDC.Model.Infrastructure;
using SZDC.Model.Infrastructure.Traffic;
using SZDC.Model.Infrastructure.Trains;
using SZDC.Model.Layers;

namespace SZDC.Editor.TrainTimetables {

    public class DynamicTrainTimetable : TrainTimetable {

        public TimetableInfo TimetableInfo { get; set; }
        public IEventManagementService EventManagementService { get; }

        public DynamicTrainTimetable(IComponentContext componentContext) 
            : base(componentContext) {

            AddRegisteredDrawingLayers();
            EventManagementService = componentContext.Resolve<IEventManagementService>();
            Factories = new FactoriesCollector(componentContext);
            ViewModel.TrafficView = Factories.TrafficViewFactory.CreateTrafficView(new SzdcTraffic(Enumerable.Empty<SzdcTrain>()));
            DrawingManager.ReplaceRegisteredDrawingLayer(new TrafficLayer(ViewModel));
            EnableTools();
        }

        private void EnableTools() {

            Tools.TrainSelectionTool.Enable();
            Tools.ComponentHitTestTool.EnableMouseInput();
            Tools.CurrentDateTimeTool.EnableMouseInput();
            Tools.CanvasTranslationCacheTool.EnableMouseInput();
        }

        private void AddRegisteredDrawingLayers() {

            DrawingManager.ReplaceRegisteredDrawingLayer(new SelectedTrainLayer(Tools.TrainSelectionTool));
            DrawingManager.ReplaceRegisteredDrawingLayer(new BackgroundLayer(GraphicalComponent));
            DrawingManager.ReplaceRegisteredDrawingLayer(new CurrentTimeLayer(GraphicalComponent, GraphicalComponent));
        }

        public void UpdateDynamicContext() {
            
            // redraws green vertical line
            ViewModifiedNotifier.NotifyViewChange();
        }

        /// <summary>
        /// Called by receiver implemented as window. This method or later it's parts would be called dispatched to UI thread.
        /// </summary>
        public void Modify(int trainNumber, ImmutableArray<TrainEvent> schedule) {

            var trainView = ViewModel.TrafficView.TrainViews.Find(r => r.Train.TrainNumber == trainNumber);
            if (trainView == null) return;

            var railway = (SzdcRailway) ViewModel.RailwayView.Railway; 
            var (start, end) = ApplicationEditor.SanitizeSchedule(railway, schedule, trainNumber);
            var scheduleSubset = ApplicationEditor.CreateScheduleSubset(railway, start, end, schedule);
            trainView.Train.CompleteSchedule = schedule.ToImmutableList();
            trainView.Train.Schedule = scheduleSubset.ToImmutableArray();
        }
    }
}
