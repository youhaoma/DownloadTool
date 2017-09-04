using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPan.Infrastructure.Events
{
    public class EventArgsBase : EventArgs
    {

        public DateTime Timestamp { get; } = DateTime.Now;

    }
}
