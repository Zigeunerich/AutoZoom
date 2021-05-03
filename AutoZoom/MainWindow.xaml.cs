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
using WindowsInput.Native;
using ZoomNet;

namespace AutoZoom
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<Meeting, Process> _activeMeetings = new();
        InputSimulator _inputSimulator = new InputSimulator();
        MeetingScheduler _meetingScheduler = new();

        string _zoomPath = Environment.ExpandEnvironmentVariables(@"%APPDATA%\Zoom\bin\Zoom.exe");

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
                // Stop recording using ALT+F9
                _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.MENU, VirtualKeyCode.F9);

                // Kill Zoom process
                zoomProcess.Kill(true);

                // Remove from active meetings
                _activeMeetings.Remove(meeting);
            }
        }

        private void MeetingScheduler_MeetingStarted(object sender, Meeting meeting)
        {
            Task.Factory.StartNew(async () =>
            {
                // Build url from meeting id
                var url = string.Format($"zoommtg://zoom.us/join?action=join&confno={meeting.ID}");

                // Start zoom process with url as command line parameter
                var zoomProcess = Process.Start(new ProcessStartInfo()
                {
                    FileName = _zoomPath,
                    Arguments = $"\"--url={url}\"",
                    UseShellExecute = false
                });

                // Add to active meetings
                _activeMeetings.Add(meeting, zoomProcess);

                // Wait for Zoom to join meeting
                await Task.Delay(15000);

                // If password is set, enter and confirm
                if (meeting.Password != null)
                {
                    _inputSimulator.Keyboard.TextEntry(meeting.Password);

                    await Task.Delay(500);

                    _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);
                }

                await Task.Delay(10000);

                // Set Zoom to full screen using ALT+F

                _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.MENU, VirtualKeyCode.VK_F);

                await Task.Delay(10000);

                // Start recording using ALT+F9

                _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.MENU, VirtualKeyCode.F9);
            });
        }
    }
}
