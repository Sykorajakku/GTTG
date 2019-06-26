using System.Collections.Immutable;
using System.ComponentModel;

using GTTG.Core.Base;
using SZDC.Editor.Interfaces;

namespace SZDC.Editor.Selectors {

    /// <summary>
    /// Manages selection state of railway and segments. On newly selected railway updates the available segments.
    /// </summary>
    public class InfrastructureSelector : ObservableObject {

        private ImmutableArray<RailwaySegmentBriefDescription> _availableRailwaySegments;
        private ImmutableArray<string> _availableRailways;
        private RailwaySegmentBriefDescription _selectedRailwaySection;
        private string _selectedRailway;
        private readonly ApplicationEditor _applicationEditor;

        /// <summary>
        /// Available railway segments of <see cref="SelectedRailway"/>.
        /// </summary>
        public ImmutableArray<RailwaySegmentBriefDescription> AvailableRailwaySegments {
            get => _availableRailwaySegments;
            set => Update(ref _availableRailwaySegments, value);
        }
   
        /// <summary>
        /// Selected segment from <see cref="AvailableRailwaySegments"/>.
        /// </summary>
        public RailwaySegmentBriefDescription SelectedRailwaySection {
            get => _selectedRailwaySection;
            set => Update(ref _selectedRailwaySection, value);
        }

        /// <summary>
        /// Available railways denoted by their railway numbers.
        /// </summary>
        public ImmutableArray<string> AvailableRailways {
            get => _availableRailways;
            set => Update(ref _availableRailways, value);
        }

        /// <summary>
        /// Selected railway number from <see cref="AvailableRailways"/>.
        /// </summary>
        public string SelectedRailway {
            get => _selectedRailway;
            set => Update(ref _selectedRailway, value);
        }

        public InfrastructureSelector(ApplicationEditor applicationEditor) {

            _applicationEditor = applicationEditor;
            _availableRailways = ImmutableArray.Create<string>();
            _availableRailwaySegments = ImmutableArray.Create<RailwaySegmentBriefDescription>();

            PropertyChanged += OnPropertyChanged;
            AvailableRailways = applicationEditor.GetRailways().ToImmutableArray();
        }
        
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {

            if (e.PropertyName != nameof(SelectedRailway)) {
                return;
            }

            // Updates railway sections to selected railway
            AvailableRailwaySegments = _applicationEditor
                .GetRailwaySegments(SelectedRailway)
                .ToImmutableArray();
        }
    }
}
