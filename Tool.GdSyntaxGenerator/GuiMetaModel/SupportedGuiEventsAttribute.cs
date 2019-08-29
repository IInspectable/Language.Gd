using System;

namespace Pharmatechnik.Apotheke.XTplus.Framework.Tools.Generators.GuiModelGenerator.GuiMetaModel {
    [AttributeUsageAttribute(AttributeTargets.Class, AllowMultiple = true)]
    public class SupportedGuiEventsAttribute : Attribute {
        private string[] _events;

        public SupportedGuiEventsAttribute(string nameList) {
            _events = nameList.Split(',');
            for (int i = 0; i < _events.Length; i++) {
                _events[i] = _events[i].Trim();
            }
        }

        public string[] Events {
            get { return _events; }
        }
    }
}