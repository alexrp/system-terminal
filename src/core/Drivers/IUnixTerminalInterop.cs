namespace System.Drivers
{
    interface IUnixTerminalInterop
    {
        (int Width, int Height)? WindowSize { get; }

        void RefreshSettings();

        bool SetRawMode(bool raw, bool discard);
    }
}