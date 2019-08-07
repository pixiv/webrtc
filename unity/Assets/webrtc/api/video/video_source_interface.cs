namespace Rtc
{
    public sealed class VideoSinkWants
    {
        public bool RotationApplied { get; set; } = false;
        public bool BlackFrames { get; set; } = false;
        public int MaxPixelCount { get; set; } = int.MaxValue;
        public int? TargetPixelCount { get; set; }
        public int MaxFramerateFps { get; set; } = int.MaxValue;
    }
}
