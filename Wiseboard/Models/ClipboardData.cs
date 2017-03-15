using System;

namespace Wiseboard.Models
{
    public interface IClipboardData
    {
        Type GetDataType();
        object GetData();
        string GetVisibleText();
        bool IsLinkOrLinks();
    }

    public class ClipboardData : IClipboardData
    {
        private readonly object _data;
        private readonly string _fileNames;
        private readonly bool _isLink;

        public ClipboardData(object data, bool isLinkParam, string fileNames = null)
        {
            _data = data;
            _fileNames = fileNames;
            _isLink = isLinkParam;
        }

        public Type GetDataType() => _data.GetType();
        public object GetData() => _data;
        public string GetVisibleText()
        {
            if (!_isLink)
                return (string)GetData();
            return _fileNames;
        }

        public bool IsLinkOrLinks() => _isLink;
    }
}
