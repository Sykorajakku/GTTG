namespace GTTG.Infrastructure.MouseInput  {

    public struct MouseInputArgs {

        public float X { get; }
        public float Y { get; }

        public MouseInputArgs(float x, float y) {
            X = x;
            Y = y;
        }
    }
}
