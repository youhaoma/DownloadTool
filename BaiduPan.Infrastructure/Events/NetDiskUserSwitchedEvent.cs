using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPan.Infrastructure.Events
{
    public class NetDiskUserSwitchedEvent : PubSubEvent<NetDiskUserSwitchedEventArgs>
    {
    }

    public class NetDiskUserSwitchedEventArgs :EventArgsBase
    {

        public Guid UserId { get; }

        public NetDiskUserSwitchedEventArgs(Guid userId)
        {

            UserId = userId;
        }
    }
}
