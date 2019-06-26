using SkiaSharp;

namespace SZDC.Model {

    public static class ViewConstants {

        public const float AdjacentStationsVerticalSpace = 50;
        public const float TrackSegmentVerticalSpace = 28;
        public const float StartOrEndStationSegmentSpace = 20;

        public static readonly SKColor TrackLinesColor = new SKColor(228, 154, 1); // golden
        public static readonly SKColor TimeLinesColor = new SKColor(228, 154, 1); // golden
        public static readonly SKColor PassengerTrainPathColor = SKColors.Black;
        public static readonly SKColor CargoTrainPathColor = SKColors.Blue;
        public static readonly SKColor SelectedTrainPathColor = SKColors.OrangeRed;
    }
}
