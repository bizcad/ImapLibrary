using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImapLibrary.Data
{
    public interface ICsvMessageWriter<T>
    {
        void WriteCsvMessageList(List<T> list);
    }
}
