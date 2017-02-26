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
        object data;
        string fileNames;
        readonly bool is_link;

        public ClipboardData(object data, bool is_link_param, string file_names = null)
        {
            this.data = data;
            fileNames = file_names;
            is_link = is_link_param;
        }

        public Type GetDataType() => data.GetType();
        public object GetData() => data;
        public string GetVisibleText()
        {
            if (!is_link)
                return (string)GetData();
            else
                return fileNames;
        }

        public bool IsLinkOrLinks() => is_link;
    }
}
