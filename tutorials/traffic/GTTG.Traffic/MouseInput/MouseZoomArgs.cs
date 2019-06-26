namespace GTTG.Traffic.MouseInput {

    public struct MouseZoomArgs {

        public float X { get; }
        public float Y { get; }
        public float Delta { get; }

        public MouseZoomArgs(MouseInputArgs args, float delta) {

            X = args.X;
            Y = args.Y;
            Delta = delta;
        }
    }
}
