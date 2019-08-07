namespace Webrtc
{
    public enum RtcErrorType
    {
        None,
        UnsupportedOperation,
        UnsupportedParameter,
        InvalidParameter,
        InvalidRange,
        SyntaxError,
        InvalidState,
        InvalidModification,
        NetworkError,
        ResourceExhausted,
        InternalError,
    }

    public readonly struct RtcError
    {
        public bool OK => Type == RtcErrorType.None;
        public RtcErrorType Type { get; }
        public string Message { get; }

        public RtcError(RtcErrorType type, string message)
        {
            Type = type;
            Message = message;
        }
    }

    public readonly struct RtcErrorOr<T>
    {
        public RtcError Error { get; }

        private readonly T _value;

        public T Value
        {
            get
            {
                System.Diagnostics.Debug.Assert(Error.OK);
                return _value;
            }
        }

        internal RtcErrorOr(RtcError error, T value)
        {
            Error = error;
            _value = value;
        }
    }
}
