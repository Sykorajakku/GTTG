using SkiaSharp;
using System.Collections.Generic;

using GTTG.Core.Strategies.Implementations;
using GTTG.Core.Strategies.Interfaces;
using GTTG.Model.Lines;
using GTTG.Model.Model.Infrastructure;
using GTTG.Model.Strategies.Types;
using GTTG.Model.ViewModel.Infrastructure.Tracks;
using SZDC.Model.Factories.Exceptions;
using SZDC.Model.Infrastructure;
using SZDC.Model.Views.Infrastructure;

namespace SZDC.Model.Factories {

    public class SzdcTrackViewFactory : ITrackViewFactory<SzdcTrackView> {

        private readonly ISegmentRegistry<LineType, MeasureableSegment> _segmentRegistry;
        private Dictionary<TrackType, LinePaint> _paintProvidersByStationType;

        public SzdcTrackViewFactory(ISegmentRegistry<LineType, MeasureableSegment> segmentRegistry) {
            _segmentRegistry = segmentRegistry;
            InitializePaintProviders();
        }

        public SzdcTrackView CreateTrackView(Track track) {

            var segment = new MeasureableSegment();
            _segmentRegistry.Register(segment).As(LineType.Of(track));
            var stationType = FindStationType(((SzdcTrack) track).TrackType);
            var trackLine = _paintProvidersByStationType[stationType].Clone();

            return new SzdcTrackView(track, trackLine, segment);
        }

        public TrackType FindStationType(TrackType trackType) {

            if (trackType.HasFlag(TrackType.PassengerStation) || trackType.HasFlag(TrackType.OvertakingStation)) {
                return TrackType.PassengerStation | TrackType.OvertakingStation;
            }

            if (trackType.HasFlag(TrackType.BranchingOffPoint)) {
                return TrackType.BranchingOffPoint;
            }

            if (trackType.HasFlag(TrackType.BlockSystem) || trackType.HasFlag(TrackType.TrainAnnunciatingPoint)) {
                return TrackType.BlockSystem | TrackType.TrainAnnunciatingPoint;
            }

            if (trackType.HasFlag(TrackType.BranchLine) || trackType.HasFlag(TrackType.LoadingYard)) {
                return TrackType.BranchLine | TrackType.LoadingYard;
            }

            if (trackType.HasFlag(TrackType.Halt)) {
                return TrackType.Halt;
            }

            throw new ViewFactoryException($"No paint for {nameof(trackType)} {trackType} provided.");
        }

        private void InitializePaintProviders() {

            var defaultColor = ViewConstants.TrackLinesColor;

            _paintProvidersByStationType = new Dictionary<TrackType, LinePaint> {

                [TrackType.PassengerStation | TrackType.OvertakingStation] =
                    new LinePaint(
                        desiredStrokeWidth: 4,
                        color: defaultColor),

                [TrackType.BranchingOffPoint] =
                    new LinePaint(
                        desiredStrokeWidth: 1,
                        paint: new SKPaint {
                            Color = defaultColor,
                            IsAntialias = true,
                            Style = SKPaintStyle.Stroke,
                            PathEffect = SKPathEffect.CreateDash(new float[] { 5, 5, 15, 5 }, 0)
                        }),

                [TrackType.BlockSystem | TrackType.TrainAnnunciatingPoint] =
                    new LinePaint(
                        desiredStrokeWidth: 1,
                        color: defaultColor),

                [TrackType.BranchLine | TrackType.LoadingYard] =
                    new LinePaint(
                        desiredStrokeWidth: 1,
                        paint: new SKPaint {
                            Color = defaultColor,
                            IsAntialias = true,
                            Style = SKPaintStyle.Stroke,
                            PathEffect = SKPathEffect.CreateDash(new float[] { 2, 2 }, 0)
                        }),

                [TrackType.Halt] =
                    new LinePaint(
                        desiredStrokeWidth: 1,
                        paint: new SKPaint {
                            Color = defaultColor,
                            IsAntialias = true,
                            Style = SKPaintStyle.Stroke,
                            PathEffect = SKPathEffect.CreateDash(new float[] { 2, 2 }, 0)
                        })
            };
        }
    }
}
