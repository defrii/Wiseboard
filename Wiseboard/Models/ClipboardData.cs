using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        readonly object _data;
        readonly string _fileNames;
        readonly bool _isLink;

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
