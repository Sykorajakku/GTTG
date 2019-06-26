using Autofac;

using SZDC.Editor.Layers;
using SZDC.Model.Infrastructure;
using SZDC.Model.Layers;

namespace SZDC.Editor.TrainTimetables {

    /// <summary>
    /// Scope for train diagram instance.
    /// </summary>
    public class StaticTrainTimetable : TrainTimetable {

        private SzdcRailway _railway;

        public TimetableInfo TimetableInfo { get; set; }

        public SzdcRailway Railway {
            get => _railway;
            set => Update(ref _railway, value);
        }
        
        public StaticTrainTimetable(IComponentContext componentContext) : base(componentContext) {
            EnableTools();
            AddRegisteredDrawingLayers();
        }

        private void EnableTools() {

            Tools.TrainSelectionTool.Enable();
            Tools.ComponentHitTestTool.EnableMouseInput();
            Tools.CurrentDateTimeTool.EnableMouseInput();
            Tools.CanvasTranslationCacheTool.EnableMouseInput();
        }

        private void AddRegisteredDrawingLayers() {
            DrawingManager.ReplaceRegisteredDrawingLayer(new SelectedTrainLayer(Tools.TrainSelectionTool));
            DrawingManager.ReplaceRegisteredDrawingLayer(new BackgroundLayer(ViewProvider));
        }
    }
}
