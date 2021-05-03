using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsInput;
using ZoomNet;

namespace AutoZoom
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            _meetingScheduler.MeetingStarted += MeetingScheduler_MeetingStarted;
            _meetingScheduler.MeetingFinished += MeetingScheduler_MeetingFinished;

            this.DataContext = new AddMeetingViewModel(_meetingScheduler);
        }

        private void MeetingScheduler_MeetingFinished(object sender, Meeting meeting)
        {
            if (_activeMeetings.TryGetValue(meeting, out var zoomProcess))
            {
                zoomProcess.Kill(true);
                _activeMeetings.Remove(meeting);
            }
        }

        Dictionary<Meeting, Process> _activeMeetings = new();

        string _zoomPath = Environment.ExpandEnvironmentVariables(@"%APPDATA%\Zoom\bin\Zoom.exe");

        private void MeetingScheduler_MeetingStarted(object sender, Meeting meeting)
        {
            Task.Factory.StartNew(async () =>
            {
                var url = string.Format($"zoommtg://zoom.us/join?action=join&confno={meeting.ID}");

                var zoomProcess = Process.Start(new ProcessStartInfo()
                {
                    FileName = _zoomPath,
                    Arguments = $"\"--url={url}\"",
                    UseShellExecute = false
                });

                _activeMeetings.Add(meeting, zoomProcess);

                await Task.Delay(15000);

                if (meeting.Password != null)
                {
                    _inputSimulator.Keyboard.TextEntry(meeting.Password);

                    await Task.Delay(500);
                }

                _inputSimulator.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.RETURN);
            });
        }

        InputSimulator _inputSimulator = new InputSimulator();
        MeetingScheduler _meetingScheduler = new();
    }
}
