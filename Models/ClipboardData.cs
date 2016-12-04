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
    }

    public class ClipboardData : IClipboardData
    {
        object data;
        string fileNames;

        public ClipboardData(object data, string file_names = null)
        {
            this.data = data;
            fileNames = file_names;
        }

        public Type GetDataType() => data.GetType();
        public object GetData() => data;
        public string GetVisibleText()
        {
            if (data.GetType() == Type.GetType("System.String"))
                return (string)GetData();
            else
                return fileNames;
        }
    }
}
