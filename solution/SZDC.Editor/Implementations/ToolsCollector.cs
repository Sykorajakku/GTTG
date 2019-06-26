using Autofac;
using SZDC.Editor.Tools;

namespace SZDC.Editor.Implementations {

    /// <summary>
    /// Collects tools in train timetable which represents parts of application logic.
    /// </summary>
    public class ToolsCollector {

        public CanvasTranslationCacheTool CanvasTranslationCacheTool { get; }

        public TrainViewSelectionTool TrainSelectionTool { get; }

        public TrainModificationTool TrainModificationTool { get; }

        public ComponentsTool ComponentHitTestTool { get; }

        public CurrentDateTimeTool CurrentDateTimeTool { get; }

        public ViewTimeModifierTool ViewTimeModifierTool { get; }

        public ToolsCollector(IComponentContext componentContext) {

            TrainSelectionTool = componentContext.Resolve<TrainViewSelectionTool>();
            CurrentDateTimeTool = componentContext.Resolve<CurrentDateTimeTool>();
            TrainModificationTool = componentContext.Resolve<TrainModificationTool>();
            ComponentHitTestTool = componentContext.Resolve<ComponentsTool>();
            CanvasTranslationCacheTool = componentContext.Resolve<CanvasTranslationCacheTool>();
            ViewTimeModifierTool = componentContext.Resolve<ViewTimeModifierTool>();
        }
    }
}
