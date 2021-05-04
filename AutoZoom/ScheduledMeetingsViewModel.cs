using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AutoZoom
{
    public class ScheduledMeetingsViewModel : ViewModelBase
    {
        readonly MeetingScheduler _meetingScheduler;

        public IEnumerable<ScheduledMeetingViewModel> Meetings =>
            _meetingScheduler.Meetings.Select(meeting => new ScheduledMeetingViewModel(meeting));

        public ScheduledMeetingViewModel SelectedMeeting { get; set; }

        public ICommand DeleteCommand { get; }

        public ScheduledMeetingsViewModel(MeetingScheduler meetingScheduler)
        {
            _meetingScheduler = meetingScheduler;

            _meetingScheduler.MeetingAdded += (sender, meeting) =>
            {
                OnPropertyChanged(nameof(Meetings));
            };

            _meetingScheduler.MeetingRemoved += (sender, meeting) =>
            {
                OnPropertyChanged(nameof(Meetings));
            };

            DeleteCommand = new RelayCommand(() => _meetingScheduler.RemoveMeeting(SelectedMeeting.Meeting),
                () => SelectedMeeting != null);
        }
    }

    public class ScheduledMeetingViewModel : ViewModelBase
    {
        Meeting _meeting;
        DateTime? _startDate;
        DateTime? _endDate;
        string _startTime;
        string _endTime;
        string _id;
        string _password;
        string _name;

        public DateTime? StartDate {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime? EndDate {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged();
            }
        }

        public string StartTime {
            get => _startTime;
            set
            {
                _startTime = value;
                OnPropertyChanged();
            }
        }

        public string EndTime {
            get => _endTime;
            set
            {
                _endTime = value;
                OnPropertyChanged();
            }
        }

        public string ID {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public string Password {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public string Name {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public Meeting Meeting => _meeting;

        public ScheduledMeetingViewModel(Meeting meeting)
        {
            _meeting = meeting;

            Name = meeting.Name;
            ID = meeting.ID;
            Password = meeting.Password;
            StartDate = meeting.StartTime.Date;
            StartTime = meeting.StartTime.TimeOfDay.ToString(@"hh\:mm");
            EndDate = meeting.EndTime.Date;
            EndTime = meeting.EndTime.TimeOfDay.ToString(@"hh\:mm");
        }
    }
}
