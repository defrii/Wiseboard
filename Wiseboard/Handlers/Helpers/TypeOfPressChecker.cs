namespace Wiseboard.Handlers.Helpers
{
    public static class TypeOfPressChecker
    {
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_SYSKEYUP = 0x105;

        public static bool IsKeyDownPressed(int type) => type == WM_KEYDOWN || type == WM_SYSKEYDOWN;
        public static bool IsKeyUpPressed(int type) => type == WM_KEYUP || type == WM_SYSKEYUP;
    }
}
