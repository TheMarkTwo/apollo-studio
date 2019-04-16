using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

using Newtonsoft.Json;

using Apollo.Core;
using Apollo.Elements;
using Apollo.Structures;

namespace Apollo.Devices {
    public class Multi: Device, IChainParent {
        public static readonly new string DeviceIdentifier = "multi";

        public Chain Preprocess;
        private List<Chain> _chains = new List<Chain>();
        private int current = -1;

        private void Reroute() {
            for (int i = 0; i < _chains.Count; i++) {
                _chains[i].Parent = this;
                _chains[i].ParentIndex = i;
            }
        }

        public Chain this[int index] {
            get => _chains[index];
        }

        public int Count {
            get => _chains.Count;
        }

        public override Device Clone() => new Multi(Preprocess.Clone(), (from i in _chains select i.Clone()).ToList());

        public void Insert(int index) {
            _chains.Insert(index, new Chain() {MIDIExit = ChainExit});
            
            Reroute();
        }

        public void Insert(int index, Chain chain) {
            chain.MIDIExit = ChainExit;
            _chains.Insert(index, chain);
            
            Reroute();
        }

        public void Add(Chain chain) {
            chain.Parent = this;
            chain.ParentIndex = _chains.Count;
            chain.MIDIExit = ChainExit;
            _chains.Add(chain);
        }

        public void Add(List<Chain> chains) {
            foreach (Chain chain in chains) Add(chain);
        }

        public void Remove(int index) {
            _chains.RemoveAt(index);

            Reroute();
        }

        public Multi(Chain preprocess = null, List<Chain> init = null): base(DeviceIdentifier) {
            Preprocess = preprocess?? new Chain();
            Preprocess.Parent = this;
            Preprocess.MIDIExit = PreprocessExit;

            Add(init?? new List<Chain>());
        }

        private void ChainExit(Signal n) => MIDIExit?.Invoke(n);

        public override void MIDIEnter(Signal n) {
            if (_chains.Count == 0) ChainExit(n);

            if (++current >= _chains.Count) current = 0;

            n.MultiTarget = current;
            Preprocess.MIDIEnter(n);
        }

        private void PreprocessExit(Signal n) {
            int target = n.MultiTarget.Value;
            n.MultiTarget = null;
            _chains[target].MIDIEnter(n);
        }

        public static Device DecodeSpecific(string jsonString) {
            Dictionary<string, object> json = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
            if (json["device"].ToString() != DeviceIdentifier) return null;

            Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json["data"].ToString());
            
            List<object> chains = JsonConvert.DeserializeObject<List<object>>(data["chains"].ToString());
            List<Chain> init = new List<Chain>();

            foreach (object chain in chains)
                init.Add(Chain.Decode(chain.ToString()));
            
            return new Multi(
                Chain.Decode(data["preprocess"].ToString()),
                init
            );
        }

        public override string EncodeSpecific() {
            StringBuilder json = new StringBuilder();

            using (JsonWriter writer = new JsonTextWriter(new StringWriter(json))) {
                writer.WriteStartObject();

                    writer.WritePropertyName("device");
                    writer.WriteValue(DeviceIdentifier);

                    writer.WritePropertyName("data");
                    writer.WriteStartObject();

                        writer.WritePropertyName("preprocess");
                        writer.WriteRawValue(Preprocess.Encode());

                        writer.WritePropertyName("chains");
                        writer.WriteStartArray();

                            for (int i = 0; i < _chains.Count; i++)
                                writer.WriteRawValue(_chains[i].Encode());

                        writer.WriteEndArray();
                        
                    writer.WriteEndObject();

                writer.WriteEndObject();
            }
            
            return json.ToString();
        }
    }
}