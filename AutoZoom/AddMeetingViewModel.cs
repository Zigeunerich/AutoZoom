using System;
using System.Windows.Input;

namespace AutoZoom
{
    public class AddMeetingViewModel : ViewModelBase
    {
        DateTime? _startDate;
        DateTime? _endDate;
        string _startTime;
        string _endTime;
        string _id;
        string _password;

        public DateTime? StartDate {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged();
                AddCommand.RaiseCanExecuteChanged();
            }
        }

        public DateTime? EndDate {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged();
                AddCommand.RaiseCanExecuteChanged();
            }
        }

        public string StartTime {
            get => _startTime;
            set
            {
                _startTime = value;
                OnPropertyChanged();
                AddCommand.RaiseCanExecuteChanged();
            }
        }

        public string EndTime {
            get => _endTime;
            set
            {
                _endTime = value;
                OnPropertyChanged();
                AddCommand.RaiseCanExecuteChanged();
            }
        }

        public string ID {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
                AddCommand.RaiseCanExecuteChanged();
            }
        }

        public string Password {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                AddCommand.RaiseCanExecuteChanged();
            }
        }

        public AddMeetingViewModel(MeetingScheduler meetingScheduler)
        {
            AddCommand = new RelayCommand(() =>
            {
                var meeting = new Meeting
                {
                    ID = ID,
                    Password = Password,
                    StartTime = StartDate.Value.Add(TimeSpan.Parse(StartTime)),
                    EndTime = EndDate.Value.Add(TimeSpan.Parse(EndTime))
                };

                meetingScheduler.AddMeeting(meeting);
            },
            () => TimeSpan.TryParse(StartTime, out var startTime)
                && TimeSpan.TryParse(EndTime, out var endTime)
                && StartDate.HasValue && (StartDate.Value.Add(startTime) > DateTime.Now)
                && EndDate.HasValue && (EndDate.Value.Add(endTime) > DateTime.Now)
                && !string.IsNullOrEmpty(ID));
        }

        public RelayCommand AddCommand { get; }
    }
}
