using System;
using SkiaSharp;

using GTTG.Core.Strategies.Implementations;
using GTTG.Core.Strategies.Interfaces;
using GTTG.Model.Lines;
using GTTG.Model.Model.Infrastructure;
using GTTG.Model.Strategies.Types;
using GTTG.Model.ViewModel.Infrastructure.Tracks;
using GTTG.Traffic.Model;

namespace GTTG.Traffic.ViewModel {


    public class TutorialTrackViewFactory : ITrackViewFactory<TutorialTrackView> {

        public static float RedLineStrokeWidth = 3;
        public static float BlueLineStrokeWidth = 1;

        private readonly ISegmentRegistry<LineType, MeasureableSegment> _lineSegments;

        public TutorialTrackViewFactory(ISegmentRegistry<LineType, MeasureableSegment> lineSegments) {
            _lineSegments = lineSegments;
        }

        public TutorialTrackView CreateTrackView(Track track) {

            var lineType = LineType.Of(track);
            var segment = new MeasureableSegment();
            _lineSegments.Register(segment).As(lineType);
            return new TutorialTrackView(track, CreateTrackLine(track), segment);
        }

        private static LinePaint CreateTrackLine(Track track) {

            if (!(track is TutorialTrack demoTrack)) {
                throw new ArgumentException("Type of track was not recognized.");
            }

            switch (demoTrack.TrackType) {
                case TrackType.Cargo:
                    return new LinePaint(BlueLineStrokeWidth, SKColors.Blue);
                case TrackType.Passenger:
                    return new LinePaint(RedLineStrokeWidth, SKColors.Red);
                default:
                    throw new ArgumentException($"{nameof(demoTrack.TrackType)} enum member was not recognized.");
            }
        }
    }
}
