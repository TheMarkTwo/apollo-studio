using Avalonia.Threading;

using RtMidi.Core.Devices.Infos;
using RtMidi.Core.Messages;

using Apollo.Core;
using Apollo.Helpers;
using Apollo.Structures;

namespace Apollo.Elements {
    public class AbletonLaunchpad: Launchpad {
        private Launchpad _target = null;
        public Launchpad Target {
            get => _target;
            set {
                if (_target != value) {
                    _target?.AbletonLaunchpads.Remove(this);

                    _target = value;

                    _target?.AbletonLaunchpads.Add(this);
                }
            }
        }

        public override RotationType Rotation {
            get => Target.Rotation;
            set {}
        }

        public override void Send(Signal n) {}

        public override void Clear(bool manual = false) {
            if (!Available || (manual && PatternWindow != null)) return;
            
            Target?.Clear(manual);

            Signal n = new Signal(this, 0, new Color(0));

            for (int i = 0; i < 100; i++) {
                n.Index = (byte)i;
                Window?.SignalRender(n);
            }

            AbletonConnector.Send(this, n);
        }

        public override void Render(Signal n) => Target?.Render(n);

        public AbletonLaunchpad(string name) {
            Type = LaunchpadType.PRO;
            Name = name;

            Target = MIDI.NoOutput;
        }

        public override void Connect(IMidiInputDeviceInfo input = null, IMidiOutputDeviceInfo output = null) {
            Available = true;

            Program.Log($"MIDI Created {Name}");
        }
        
        public override void Disconnect() {
            Program.Log($"MIDI Disconnected {Name}");

            Dispatcher.UIThread.InvokeAsync(() => Window?.Close());

            Available = false;
        }
    }
}