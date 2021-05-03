using System;
using System.Collections.Generic;
using System.Threading;

namespace AutoZoom
{
    public class MeetingScheduler
    {
        readonly Dictionary<Meeting, (Timer, Timer)> _meetings = new();

        public void AddMeeting(Meeting meeting)
        {
            var msUntilMeetingStart = (int)(meeting.StartTime - DateTime.Now).TotalMilliseconds;
            var msUntilMeetingEnd = (int)(meeting.EndTime - DateTime.Now).TotalMilliseconds;

            var startTimer = new Timer(state => { MeetingStarted?.Invoke(this, meeting); });
            var endTimer = new Timer(state => { MeetingFinished?.Invoke(this, meeting); });

            if (startTimer.Change(msUntilMeetingStart, Timeout.Infinite)
                && endTimer.Change(msUntilMeetingEnd, Timeout.Infinite))
            {
                _meetings.Add(meeting, (startTimer, endTimer));
            }
        }

        public event EventHandler<Meeting> MeetingStarted;
        public event EventHandler<Meeting> MeetingFinished;

        public IEnumerable<Meeting> Meetings => _meetings.Keys;
    }
}
