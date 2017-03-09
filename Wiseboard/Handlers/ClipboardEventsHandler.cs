using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiseboard.Handlers
{
    public class ClipboardEventsHandler
    {
        bool _keyNextHandled;
        bool _isEarlyPasted;
        bool _isSelectingItem;

        public bool CanChangePositionOfSelectedItem() => _keyNextHandled;
        public void SetChangePositionOfSelectedItem(bool value) => _keyNextHandled = value;

        public bool IsPastedBeforeDisplayClipboard() => _isEarlyPasted;
        public void SetPastedBeforeDisplayClipboard(bool value) => _isEarlyPasted = value;

        public bool IsItemSelectingFromClipboard() => _isSelectingItem;
        public void SetSelectingFromClipboard(bool value) => _isSelectingItem = value;
    }
}
