using System.Collections.Immutable;
using Autofac;

using GTTG.Core.Base;
using GTTG.Core.Component;
using GTTG.Core.Drawing.Layers;
using SZDC.Editor.Implementations;
using SZDC.Model.Views;

namespace SZDC.Editor.TrainTimetables {

    public abstract partial class TrainTimetable : ObservableObject {

        private ImmutableArray<string> _errors;

        /// <summary>
        /// Errors connected with this railway view reported to user.
        /// </summary>
        public ImmutableArray<string> Errors {
            get => _errors;
            set => Update(ref _errors, value);
        }

        public IViewProvider ViewProvider { get; }
        public IViewModel ViewModel { get; }
        public DrawingManager DrawingManager { get; }

        protected IComponentContext ComponentContext;
        protected ViewModifiedNotifier ViewModifiedNotifier;
        protected GraphicalComponent GraphicalComponent;

        public ComponentsCollector Components { get; protected set; }
        public FactoriesCollector Factories { get; protected set; }
        public ToolsCollector Tools { get; protected set; }
        public float Height { get; protected set; }

        protected TrainTimetable(IComponentContext componentContext) {

            ComponentContext = componentContext;

            DrawingManager = componentContext.Resolve<DrawingManager>();
            ViewProvider = componentContext.Resolve<IViewProvider>();
            GraphicalComponent = componentContext.Resolve<GraphicalComponent>();
            ViewModel = componentContext.Resolve<IViewModel>();
            ViewModifiedNotifier = componentContext.Resolve<ViewModifiedNotifier>();
            Components = new ComponentsCollector(componentContext);
            Tools = new ToolsCollector(componentContext);
            Factories = new FactoriesCollector(componentContext);

            Errors = ImmutableArray<string>.Empty;
            InitializeMouseInput();
            Tools.TrainSelectionTool.PropertyChanged += TrainSelectionToolOnPropertyChanged;
        }
    }
}
