using System.Collections.Generic;

namespace Pharmatechnik.Apotheke.XTplus.Framework.Tools.Generators.GuiModelGenerator.GuiMetaModel {

    public class EventDelegateInfo {

        public EventDelegateInfo(string eventName, string v1, string v2, string[] v3) {
        }

        public string DelegateName { get; set; }

        public string EventName { get; set; }

        public EventDelegateInfo Clone() {
            throw new System.NotImplementedException();
        }

    }

    public class GuiControlsReflectedInfo {

        public static GuiControlsReflectedInfo Instance => new GuiControlsReflectedInfo();

        public EventDelegateInfo GetControlEventInfo(string controlName, string eventName, Control control) {
            throw new System.NotImplementedException();
        }

        public IEnumerable<EventDelegateInfo> GetControlEventInfos(Control control) {
            throw new System.NotImplementedException();
        }

    }

    public class GuiModelFactory {

        public static EditorUIControl CreateEditor(ColumnInfoTypeType currentColType, Container parentRootContainer, Table currentColParent, ColumnInfoType currentCol) {
            throw new System.NotImplementedException();
        }

    }

}