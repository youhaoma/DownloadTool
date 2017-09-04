using BaiduPan.Infrastructure.Exceptions;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPan.Infrastructure.Events
{

    public class DownloadStateChangedEventArgs : EventArgsBase
    {
        public long FileId { get; }

        public DownloadStateEnum OldState { get; }


        public DownloadStateEnum NewState { get; }


        public DownloadStateChangedEventArgs( DownloadStateEnum oldState, DownloadStateEnum newState, long fileld = -1)
        {

#if DEBUG

            if (!CheckArgs(oldState, newState))
                throw new InvalidDownloadStateChangeException(oldState, newState, $"{oldState} can bot be converter to {newState}.");

#endif

            FileId = fileld;
            OldState = oldState;
            NewState = newState;
        }


#if DEBUG

        private static readonly Dictionary<DownloadStateEnum, DownloadStateEnum> StateChangeRule =
            new Dictionary<DownloadStateEnum, DownloadStateEnum>
            {
                { DownloadStateEnum.Created, DownloadStateEnum.Waiting},
                { DownloadStateEnum.Waiting, DownloadStateEnum.Downloading | DownloadStateEnum.Canceld
                | DownloadStateEnum.Paused},

                { DownloadStateEnum.Downloading, DownloadStateEnum.Paused | DownloadStateEnum.Faulted
                | DownloadStateEnum.Completed | DownloadStateEnum.Canceld},

                { DownloadStateEnum.Paused, DownloadStateEnum.Downloading | DownloadStateEnum.Canceld
                | DownloadStateEnum.Paused}
            };

        private bool CheckArgs(DownloadStateEnum oldState, DownloadStateEnum newState)
        {
            if (oldState == DownloadStateEnum.Canceld ||
                oldState == DownloadStateEnum.Completed ||
                oldState == DownloadStateEnum.Faulted ||
                newState == DownloadStateEnum.Created)
                return false;


            return (StateChangeRule[oldState] & newState) == newState;
        }

#endif
    }
}
