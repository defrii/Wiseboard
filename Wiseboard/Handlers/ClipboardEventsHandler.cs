namespace Wiseboard.Handlers
{
    public class ClipboardEventsHandler
    {
        private bool _keyNextHandled;
        private bool _isEarlyPasted;
        private bool _isSelectingItem;

        public bool CanChangePositionOfSelectedItem() => _keyNextHandled;
        public void SetChangePositionOfSelectedItem(bool value) => _keyNextHandled = value;

        public bool IsPastedBeforeDisplayClipboard() => _isEarlyPasted;
        public void SetPastedBeforeDisplayClipboard(bool value) => _isEarlyPasted = value;

        public bool IsItemSelectingFromClipboard() => _isSelectingItem;
        public void SetSelectingFromClipboard(bool value) => _isSelectingItem = value;
    }
}
