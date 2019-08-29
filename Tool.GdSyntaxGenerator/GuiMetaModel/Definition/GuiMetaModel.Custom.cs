using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Pharmatechnik.Apotheke.XTplus.Framework.Tools.Generators.GuiModelGenerator.GuiMetaModel {
    /// <summary>
    /// Helper functionen
    /// </summary>
    internal class HelperMethods {
        /// <summary>
        /// Formatiert Variablen Namen nach Namenskonvetion
        /// </summary>
        /// <param name="variableName">Name der Variable, die formatiert werden soll</param>
        /// <returns>Formatierter Name der Variable</returns>
        public static string FormatVariableName(string variableName) {
            if (string.IsNullOrEmpty(variableName)) {
                return string.Empty;
            }
            return variableName.Substring(0, 1).ToLower() + variableName.Substring(1, variableName.Length - 1);
        }

        /// <summary>
        /// Formatiert Variablen Namen nach Camel case
        /// </summary>
        /// <param name="key">Name der Variable, die formatiert werden soll</param>
        /// <returns>Formatierter Name der Variable</returns>
        public static string FormatFormKey(string key) {
            if (string.IsNullOrEmpty(key)) {
                return string.Empty;
            }
            return key.Substring(0, 1).ToUpper() + key.Substring(1).ToLower();
        }
    }


    [SupportedProperties("Title, ExternWFS, WFSInterface, UniqueKey")]
    public partial class Dialog {
        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "Frm"; }
        }

        /// <summary>
        /// Generate PreventEventStorms code
        /// </summary>
        [XmlIgnore]
        public override bool PreventEventStorms {
            get { return true; }
        }


        [XmlIgnore]
        public override bool IsForm {
            get { return false; }
        }

        [XmlIgnore]
        public override bool IsDialog {
            get { return true; }
        }

        [XmlIgnore]
        public override bool IsUserControl {
            get { return false; }
        }
    }

    public partial class ContainerGuiElements {
        public ContainerGuiElements() {
            Items = new GuiElement[0];
        }
    }

    public partial class PanelGuiElements {
        public PanelGuiElements() {
            Items = new GuiElement[0];
        }
    }

    [SupportedProperties("Title, ExternWFS, WFSInterface, UniqueKey")]
    public partial class Form {
        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "Frm"; }
        }


        /// <summary>
        /// Generate PreventEventStorms code
        /// </summary>
        [XmlIgnore]
        public override bool PreventEventStorms {
            get { return true; }
        }

        [XmlIgnore]
        public override bool IsForm {
            get { return true; }
        }

        [XmlIgnore]
        public override bool IsDialog {
            get { return false; }
        }

        [XmlIgnore]
        public override bool IsUserControl {
            get { return false; }
        }
    }

    public partial class UserControl {
        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "Usr"; }
        }

        /// <summary>
        /// Generate PreventEventStorms code
        /// </summary>
        [XmlIgnore]
        public override bool PreventEventStorms {
            get { return false; }
        }

        [XmlIgnore]
        public override bool IsForm {
            get { return false; }
        }

        [XmlIgnore]
        public override bool IsDialog {
            get { return false; }
        }

        [XmlIgnore]
        public override bool IsUserControl {
            get { return true; }
        }
    }


    [SupportedKeyWords("NAMESPACE, USER CONTROL, DIALOG, FORM")]
    public partial class GuiDescription {
        public void AddContainer(Container container) {
            List<Container> containers = new List<Container>();
            if (containersField.Items != null) {
                containers.AddRange(containersField.Items);
            }
            containers.Add(container);
            containersField.Items = containers.ToArray();
        }

        public void RemoveContainer(Container container) {
            List<Container> containers = new List<Container>(containersField.Items);
            containers.Remove(container);
            containersField.Items = containers.ToArray();
        }

        [XmlIgnore]
        public List<List<GuiElement>> AllGuiElements {
            get {
                List<List<GuiElement>> containerElementList = new List<List<GuiElement>>();
                foreach (Container c in Containers.Items) {
                    IGuiElementParent parent = c;
                    if (parent != null) {
                        containerElementList.Add(new List<GuiElement>(parent.GetAllGuiElements()));
                    }
                }

                return containerElementList;
            }
        }

        [XmlIgnore]
        public string TONamespace {
            get { return IWFLNamespace; }
        }

        [XmlIgnore]
        public string IWFLNamespace {
            get {
                if (Namespace != null && Namespace.Length > 0) {
                    if (Namespace.IndexOf(".GUI") > -1) {
                        return Namespace.Substring(0, Namespace.LastIndexOf(".GUI")) + ".IWFL";
                    } else {
                        int lastPoint = Namespace.LastIndexOf(".");
                        if (lastPoint > 0) {
                            return Namespace.Substring(0, lastPoint) + ".IWFL";
                        } else {
                            return Namespace + ".IWFL";
                        }
                    }
                }
                return Namespace;
            }
        }

        [XmlIgnore]
        public string TOFullName {
            get {
                string toName = Containers.Items.Length != 0 ? Containers.Items[0].Name + "TO" : "NothingTO";
                return TONamespace + "." + toName;
            }
        }

        [XmlIgnore]
        public string IWFSFullName {
            get {
                if (Containers.Items.Length == 0) {
                    return IWFLNamespace + ".INothingWFS";
                }
                //falls externwfs nicht gesetzt, dann mit IWFSInterface
                if (string.IsNullOrEmpty(Containers.Items[0].ExternWFS)) {
                    //falls wfsinterface nicht gesetzt, dann mit default
                    if (!string.IsNullOrEmpty(Containers.Items[0].WFSInterface)) {
                        return IWFLNamespace + "." + Containers.Items[0].WFSInterface;
                    } else {
                        return IWFLNamespace + ".I" + Containers.Items[0].Name + "WFS";
                    }
                } else {
                    return Containers.Items[0].ExternWFS;
                }
            }
        }

        [XmlIgnore]
        public string IWFSName {
            get { return IWFSFullName.Substring(IWFSFullName.LastIndexOf(".") + 1); }
        }


        public bool ContainsUserControlReferences() {
            foreach (GuiElement guiElement in Containers.Items[0].GetAllGuiElements()) {
                if (guiElement is UserControlReference) {
                    return true;
                }
            }
            return false;
        }

        [XmlIgnore]
        public string[] AllWFSMethods {
            get {
                List<string> wfsMethods = new List<string>();
                foreach (Container c in Containers.Items) {
                    if (c.HotKeys != null && c.HotKeys.AddHotKey != null) {
                        foreach (HotKey hotKey in c.HotKeys.AddHotKey) {
                            AddWFSMethod(hotKey.WFSMethodName, wfsMethods);
                        }
                    }
                }

                foreach (List<GuiElement> element in AllGuiElements) {
                    foreach (GuiElement e in element) {
                        if (e.HotKeys != null && e.HotKeys.AddHotKey != null) {
                            foreach (HotKey hotKey in e.HotKeys.AddHotKey) {
                                AddWFSMethod(hotKey.WFSMethodName, wfsMethods);
                            }
                        }
                        if (e is Control) {
                            Control cntrl = (Control) e;
                            foreach (Event ev in cntrl.DefinedEvents) {
                                AddWFSMethod(ev.ExtendInfo.DelegateName, wfsMethods);
                            }
                        }
                        if (e is Table) {
                            if (((Table) e).ColumnInfos != null) {
                                foreach (ColumnInfoType columnInfo in ((Table) e).ColumnInfos) {
                                    if (columnInfo.EditorUIControl != null) {
                                        foreach (Event definedEvent in columnInfo.EditorUIControl.DefinedEvents) {
                                            AddWFSMethod(definedEvent.ExtendInfo.DelegateName, wfsMethods);
                                        }
                                    }
                                }
                            }
                        }
                        if (e is TabNavigation) {
                            var tabnav = e as TabNavigation;
                            if (tabnav.SharedControls != null) {
                                foreach (Control control in tabnav.SharedControls) {
                                    foreach (Event ev in control.DefinedEvents) {
                                        AddWFSMethod(ev.ExtendInfo.DelegateName, wfsMethods);
                                    }
                                }
                            }
                        }

                        if (e.ContextMenu != null) {
                            foreach (var definedEvent in e.ContextMenu.RegisteredMethodInfos) {
                                AddWFSMethod(definedEvent.MethodName, wfsMethods);
                            }

                        }
                        
                        
                    }
                }
                return wfsMethods.ToArray();
            }
        }

        private void AddWFSMethod(string methodName, ICollection<string> wfsMethods) {
            if (!wfsMethods.Contains(methodName)) {
                wfsMethods.Add(methodName);
            }
        }
    }

    [SupportedProperties("Style, TabIndex")]
    [SupportedKeyWords("PROPERTIES, EVENTS, LAYOUTINFO, HOTKEYS")]
    public partial class GuiElement : IHotkeysSupported {
        private readonly List<HotKey> _hotKeysToAdd = new List<HotKey>();
        private readonly List<HotKey> _hotKeysToRemove = new List<HotKey>();
        private IContainer _parentContainer;

        protected Container _rootContainer = null;
        private string _ptName;

        [XmlIgnore]
        public virtual string PTName {
            get { return _ptName ?? (ControlNamePrefix ?? string.Empty) + Name; }
            set { _ptName = value; }
        }
        
        [XmlIgnore]
        public virtual string ControlNamePrefix {
            get { return "ctr"; }
        }

        [XmlIgnore]
        public virtual bool ClickAssigned {
            get { return false; }
        }

        [XmlIgnore]
        public virtual bool IsVirtual {
            get { return this is INonVisualGuiElement; }
        }

        [XmlIgnore]
        public bool HasContextMenu {
            get {
                return contextMenuField!=null && this.contextMenuField.ToolsCount > 0;
            }
        }


        /// <summary>
        /// Name of the appearance
        /// </summary>
        public string AppearanceName {
            get { return GetType().Name + Name; }
        }

        /// <summary>
        /// Layout infos
        /// </summary>
        [XmlIgnore]
        public LayoutInfo LayoutInfo {
            get { return itemField; }
            set { itemField = value; }
        }

        /// <summary>
        /// Name of the variable used by generator to
        /// declare this guielement
        /// </summary>
        [XmlIgnore]
        public string MemberVariableName {
            get { return HelperMethods.FormatVariableName(Name); }
        }

        /// <summary>
        /// Name of the property used by generator to
        /// declare this guielement
        /// </summary>
        [XmlIgnore]
        public string MemberPropertyName {
            get { return Name; }
        }

        /// <summary>
        /// Is TabIndex setted
        /// </summary>
        [XmlIgnore]
        public bool HasTabIndexValue {
            get { return TabIndex > -1; }
        }


        [XmlIgnore]
        public virtual bool SupportThemesProperty {
            get { return true; }
        }


        public void AddHotKeyToAdd(HotKey hotKey) {
            _hotKeysToAdd.Add(hotKey);
            HotKeys.AddHotKey = _hotKeysToAdd.ToArray();
        }

        public void AddHotKeyToRemove(HotKey hotKey) {
            _hotKeysToRemove.Add(hotKey);
            HotKeys.RemoveHotKey = _hotKeysToRemove.ToArray();
        }


        [XmlIgnore]
        public bool IsButtonBarManager {
            get { return GetType() == typeof (ButtonBarManager); }
        }

        [XmlIgnore]
        public virtual Container RootContainer {
            get { return _rootContainer; }
            set { _rootContainer = value; }
        }


        /// <summary>
        /// Hier sollte der Parent des Controls stehen
        /// </summary>
        [XmlIgnore]
        public virtual IContainer ParentContainer {
            get { return _parentContainer; }
            set { _parentContainer = value; }
        }

        [XmlIgnore]
        public virtual List<RegisteredMethodInfo> UnregisteredEventMethods {
            get { return new List<RegisteredMethodInfo>(); }
        }

        [XmlIgnore]
        public virtual List<RegisteredMethodInfo> UnregisteredEventMethodsDeclares {
            get { return new List<RegisteredMethodInfo>(); }
        }

        [XmlIgnore]
        public virtual List<RegisteredMethodInfo> UnregisteredHotKeyMethods {
            get {
                if (HotKeys == null) {
                    HotKeys = new GuiElementHotKeys();
                }
                return RootContainer.RegisterHotKeysReturnsNewAdded(HotKeys.AddHotKey, false);
            }
        }

        [XmlIgnore]
        public virtual List<RegisteredMethodInfo> UnregisteredHotKeyMethodsDeclares {
            get {
                if (HotKeys == null) {
                    HotKeys = new GuiElementHotKeys();
                }
                return RootContainer.RegisterHotKeysReturnsNewAdded(HotKeys.AddHotKey, true);
            }
        }

        [XmlIgnore]
        public virtual bool AutoWriteTOValue {
            get { return false; }
        }

        /// <summary>
        /// Custom init code used after deserialization
        /// </summary>
        internal virtual void InitElement() {
            return;
        }

        [XmlIgnore]
        public virtual List<Event> DefinedEvents {
            get { return new List<Event>(); }
        }
    }

    partial class GuiElementHotKeys {
        public GuiElementHotKeys() {
            AddHotKey = new HotKey[0];
            RemoveHotKey = new HotKey[0];
        }
    }

    partial class HotKey {
        private readonly List<string> _required = new List<string>();
        private readonly List<string> _disabled = new List<string>();

        public void AddRequired(string key) {
            key = key.StartsWith("+") ? key.Substring(1) : key;
            _required.Add(key);
            RequiredAddKeys = _required.ToArray();
        }

        public void AddDisabled(string key) {
            key = key.StartsWith("-") ? key.Substring(1) : key;
            _disabled.Add(key);
            DisabledAddKeys = _disabled.ToArray();
        }

        public string UniqueKeyName {
            get {
                string ukey = Key;

                if (RequiredAddKeys != null) {
                    foreach (string req in RequiredAddKeys) {
                        ukey += "_r" + req;
                    }
                }

                if (DisabledAddKeys != null) {
                    foreach (string dis in DisabledAddKeys) {
                        ukey += "_d" + dis;
                    }
                }

                return ukey;
            }
        }


        private List<string> CodeFragment(string[] keys, string fragTextt, string toRemove) {
            List<string> _codeFragment = new List<string>();

            if (keys == null || keys.Length == 0) {
                return _codeFragment;
            }

            foreach (string key in keys) {
                _codeFragment.Add(UniqueKeyName + toRemove + fragTextt + key + " = true;");
            }
            return _codeFragment;
        }


        [XmlIgnore]
        public List<string> RequiredHotKeysCodeFragmentToRemove {
            get { return CodeFragment(RequiredAddKeys, ".Required.", "_ToRemove"); }
        }

        [XmlIgnore]
        public List<string> DisabledHotKeysCodeFragmentToRemove {
            get { return CodeFragment(DisabledAddKeys, ".Disabled.", "_ToRemove"); }
        }


        [XmlIgnore]
        public List<string> RequiredHotKeysCodeFragment {
            get { return CodeFragment(RequiredAddKeys, ".Required.", ""); }
        }

        [XmlIgnore]
        public List<string> DisabledHotKeysCodeFragment {
            get { return CodeFragment(DisabledAddKeys, ".Disabled.", ""); }
        }

        [XmlIgnore]
        public string FormattedKey {
            get {
                if (string.IsNullOrEmpty(Key)) {
                    return string.Empty;
                }
                return HelperMethods.FormatFormKey(Key);
            }
        }
    }

    [SupportedProperties("Width, Height, Help, EnableChangeTracking, Pooled, CachedInstancesAtStartup, ProcessAnyKeyControl")]
    [SupportedKeyWords("CONTROLS, LAYOUT, PROPERTIES, HOTKEYS")]
    [SupportedControls(
        "PANEL, DETAILSPANEL, MULTIVIEW, TABPAGE, TimeTextbox, ReportPreview, BARMANAGER, TABNAVIGATION, PhoneTextbox, Table, Chart, TabStrip, Label, UserControlReference, Textbox, PersistentPictureBox, Picturebox, Combobox, BrowsableTextbox, AmountTextbox, NumericTextbox, FormattedLabel,FormattedTextbox, DynamicLabel, DynamicButton, RadiobuttonGroup, Radiobutton, FunctionButton, FunctionFunctionButton, Tree, DateTextbox, MaskTextbox, Checkbox, Dropdownbox, SelectionList, HeaderScroller, ExplorerBar, Cave, Scanner"
        )]
    public partial class Container : IContainer, IGuiElementParent {
        private readonly List<HotKey> _hotKeysToAdd = new List<HotKey>();
        private readonly List<HotKey> _hotKeysToRemove = new List<HotKey>();


        [XmlIgnore]
        public virtual bool HasCachedInstancesAtStartup {
            get { return CachedInstancesAtStartup>0; }
        }


        [XmlIgnore]
        public virtual bool HasProcessAnyKeyControl {
            get { return !string.IsNullOrEmpty(ProcessAnyKeyControl); }
        }

        [XmlIgnore]
        public virtual string GetProcessAnyKeyControl {
            get {
                foreach (var guiElement in AllGuiElements) {
                    if (guiElement.Name == ProcessAnyKeyControl) {
                        return guiElement.PTName;
                    }
                }

                return null;
            }
        }

        [XmlIgnore]
        public virtual bool HasTitle {
            get { return !string.IsNullOrEmpty(Title); }
        }

        [XmlIgnore]
        public virtual GuiElement[] NonVirtualGuiElements {
            get {
                if (GuiElements == null) {
                    GuiElements = new ContainerGuiElements();
                }


                List<GuiElement> guiElements = new List<GuiElement>(GuiElements.Items);
                foreach (GuiElement element in GuiElements.Items) {
                    if (element is IVirtualGuiElement) {
                        int indexOfIVrtual = guiElements.IndexOf(element);
                        guiElements.Remove(element);
                        guiElements.InsertRange(indexOfIVrtual, ((IVirtualGuiElement) element).IternalGuiElements);
                    }
                }
                return guiElements.ToArray();
            }
        }

        public void AddHotKeyToAdd(HotKey hotKey) {
            _hotKeysToAdd.Add(hotKey);
            HotKeys.AddHotKey = _hotKeysToAdd.ToArray();
        }

        public void AddHotKeyToRemove(HotKey hotKey) {
            _hotKeysToRemove.Add(hotKey);
            HotKeys.RemoveHotKey = _hotKeysToRemove.ToArray();
        }

        [XmlIgnore]
        public virtual GuiElement[] FunctionButtonBars {
            get {
                List<GuiElement> bars = new List<GuiElement>();
                foreach (GuiElement control in AllGuiElements) {
                    if (control is ButtonBarManager) {
                        bars.Add(control);
                        bars.AddRange(((ButtonBarManager) control).FunctionButtonBars);
                    }
                }
                return bars.ToArray();
            }
        }

        [XmlIgnore]
        public virtual GuiElement[] AllContainers {
            get {
                List<GuiElement> panels = new List<GuiElement>();
                foreach (GuiElement control in AllGuiElements) {
                    if (control is Panel || control is MultiViewContainer || control is TabNavigation) {
                        //TODO: Andreas muss es checken. Panel (autoGeneratedPanel wird doppelt angegeben.
                        if (null == panels.Find(delegate(GuiElement element) { return element.Name == control.Name; })) {
                            panels.Add(control);
                        }
                    }
                }
                return panels.ToArray();
            }
        }


        [XmlIgnore]
        public virtual GuiElement[] AllTabPages {
            get {
                List<GuiElement> panels = new List<GuiElement>();
                foreach (GuiElement control in AllGuiElements) {
                    if (control is TabNavigation) {
                        foreach (TabPage page in ((TabNavigation) control).TabPages) {
                            panels.Add(page);
                        }
                    }
                }
                return panels.ToArray();
            }
        }


        [XmlIgnore]
        public virtual string PTName {
            get { return (ControlNamePrefix ?? string.Empty) + Name; }
        }

        [XmlIgnore]
        public virtual string ControlNamePrefix {
            get { return "con"; }
        }

        [XmlIgnore]
        public Layout Layout {
            get { return itemField; }
            set { itemField = value; }
        }

        [XmlIgnore]
        public List<GuiElement> AllGuiElements {
            get {
                List<GuiElement> list = new List<GuiElement>();
                TraversElements(list, GuiElements.Items);
                return list;
            }
        }


        //TODO: Refactorn
        private void TraversElements(List<GuiElement> list, IEnumerable<GuiElement> items) {
            foreach (GuiElement e in items) {
                list.Add(e);
                if (e is Panel) {
                    TraversElements(list, ((Panel) e).GuiElements.Items);
                } else if (e is FunctionButtonBar) {
                    TraversElements(list, ((FunctionButtonBar) e).FunctionButtons);
                } else if (e is ButtonBarManager) {
                    list.AddRange(((ButtonBarManager) e).Controls);
                }
            }
        }

        public void AddTopPanel(Panel p) {
            List<GuiElement> elements = new List<GuiElement>();
            elements.AddRange(GuiElements.Items);
            p.GuiElements.Items = elements.ToArray();

            GuiElements.Items = new GuiElement[] {p};
        }

        public void AddGuiElement(GuiElement element) {
            List<GuiElement> elements = new List<GuiElement>();
            if (guiElementsField != null) {
                elements.AddRange(guiElementsField.Items);
            }
            elements.Add(element);
            guiElementsField.Items = elements.ToArray();
        }

        public void RemoveGuiElement(GuiElement element) {
            List<GuiElement> elements = new List<GuiElement>(guiElementsField.Items);
            elements.Remove(element);
            guiElementsField.Items = elements.ToArray();
        }

        [XmlIgnore]
        public abstract bool IsForm { get; }

        [XmlIgnore]
        public abstract bool IsDialog { get; }

        [XmlIgnore]
        public abstract bool IsUserControl { get; }

        [XmlIgnore]
        public abstract bool PreventEventStorms { get; }


        [XmlIgnore]
        public GuiElement[] Controls {
            get { return guiElementsField.Items; }
            set { guiElementsField.Items = value; }
        }

        #region IGuiElementParent Members

        private List<GuiElement> _allGuiElements = null;

        public GuiElement[] GetAllGuiElements() {
            if (_allGuiElements == null) {
                _allGuiElements = new List<GuiElement>();
                foreach (GuiElement element in Controls) {
                    _allGuiElements.Add(element);

                    IGuiElementParent parent = element as IGuiElementParent;
                    if (parent != null) {
                        _allGuiElements.AddRange(parent.GetAllGuiElements());
                    }
                }
            }
            return _allGuiElements.ToArray();
        }

        #endregion

        private readonly List<RegisteredMethodInfo> _registeredEvents = new List<RegisteredMethodInfo>();
        private readonly List<RegisteredMethodInfo> _registeredEventDeclares = new List<RegisteredMethodInfo>();

        public List<RegisteredMethodInfo> RegisterEventsReturnsNewAdded(List<Event> events, bool declares) {
            if (declares) {
                return RegisterEvents(events.ToArray(), _registeredEventDeclares);
            } else {
                return RegisterEvents(events.ToArray(), _registeredEvents);
            }
        }

        public List<RegisteredMethodInfo> RegisterHotKeysReturnsNewAdded(HotKey[] hotkey, bool declares) {
            if (declares) {
                return RegisterHotKey(hotkey, _registeredEventDeclares);
            } else {
                return RegisterHotKey(hotkey, _registeredEvents);
            }
        }


        [XmlIgnore]
        public virtual List<RegisteredMethodInfo> UnregisteredHotKeyMethods {
            get {
                if (HotKeys == null) {
                    HotKeys = new ContainerHotKeys();
                }
                return RegisterHotKeysReturnsNewAdded(HotKeys.AddHotKey, false);
            }
        }

        [XmlIgnore]
        public virtual List<RegisteredMethodInfo> UnregisteredHotKeyMethodsDeclare {
            get {
                if (HotKeys == null) {
                    HotKeys = new ContainerHotKeys();
                }
                return RegisterHotKeysReturnsNewAdded(HotKeys.AddHotKey, true);
            }
        }


        private List<RegisteredMethodInfo> _allDefinedEventsAndHotkeys;

        [XmlIgnore]
        public virtual List<RegisteredMethodInfo> AllDefinedEventsAndHotkeys {
            get {
                if (_allDefinedEventsAndHotkeys == null) {
                    _allDefinedEventsAndHotkeys = new List<RegisteredMethodInfo>();
                    if (HotKeys != null) {
                        RegisterHotKey(HotKeys.AddHotKey, _allDefinedEventsAndHotkeys);
                    }

                    foreach (GuiElement guiElement in AllGuiElements) {
                        if (guiElement.HotKeys != null) {
                            RegisterHotKey(guiElement.HotKeys.AddHotKey, _allDefinedEventsAndHotkeys);
                        }
                        if (guiElement is Control) {
                            RegisterEvents(((Control) guiElement).Events, _allDefinedEventsAndHotkeys);
                        }
                    }
                }
                return _allDefinedEventsAndHotkeys;
            }
        }

        private List<RegisteredMethodInfo> RegisterHotKey(IEnumerable<HotKey> hotkeys, List<RegisteredMethodInfo> checkList) {
            List<RegisteredMethodInfo> result = new List<RegisteredMethodInfo>();
            if (hotkeys != null) {
                foreach (HotKey hotkey in hotkeys) {
                    if (
                        !checkList.Exists(
                             delegate(RegisteredMethodInfo methodInfo) { return methodInfo.MethodName.Equals(hotkey.WFSMethodName); })) {
                        checkList.Add(new RegisteredMethodInfo(hotkey.WFSMethodName, hotkey.CallType));
                        result.Add(new RegisteredMethodInfo(hotkey.WFSMethodName, hotkey.CallType));
                    }
                }
            }
            return result;
        }


        private List<RegisteredMethodInfo> RegisterEvents(IEnumerable<Event> events, List<RegisteredMethodInfo> checkList) {
            List<RegisteredMethodInfo> result = new List<RegisteredMethodInfo>();
            if (events != null) {
                foreach (Event ev in events) {
                    if (
                        !checkList.Exists(
                             delegate(RegisteredMethodInfo methodInfo) { return methodInfo.MethodName.Equals(ev.ExtendInfo.DelegateName); })) {
                        checkList.Add(new RegisteredMethodInfo(ev.ExtendInfo.DelegateName, ev.CallType));
                        result.Add(new RegisteredMethodInfo(ev.ExtendInfo.DelegateName, ev.CallType));
                    }
                }
            }
            return result;
        }


        [XmlIgnore]
        public bool HasUserControlReferences {
            get { return AllUserControlReferences.Count > 0; }
        }

        private List<UserControlReference> _allUserControlReferences;

        [XmlIgnore]
        public List<UserControlReference> AllUserControlReferences {
            get {
                if (_allUserControlReferences == null) {
                    _allUserControlReferences = new List<UserControlReference>();
                    foreach (GuiElement guiElement in AllGuiElements) {
                        if (guiElement is IUserControlReferenceContainer) {
                            _allUserControlReferences.AddRange(((IUserControlReferenceContainer) guiElement).GetAllUserControlReferences());
                        }

                        //TODO: DELETE
                        //if (guiElement is TabNavigation) {
                        //    //Bug in TabNavigation: Manuelle Rekursion
                        //    foreach (GuiElement subElement in ((TabNavigation) guiElement).GetAllGuiElements()) {
                        //        if (subElement is UserControlReference) {
                        //            _allUserControlReferences.Add((UserControlReference) subElement);
                        //        }
                        //    }
                        //}

                        //Multview
                        //if (guiElement is MultiViewContainer) {
                        //    foreach (GuiElement subElement in ((MultiViewContainer) guiElement).GetAllGuiElements()) {
                        //        if (subElement is UserControlReference) {
                        //            _allUserControlReferences.Add((UserControlReference) subElement);
                        //        }
                        //    }
                        //}


                        if (guiElement is UserControlReference) {
                            _allUserControlReferences.Add((UserControlReference) guiElement);
                        }
                    }
                }
                return _allUserControlReferences;
            }
        }
    }

    internal interface IUserControlReferenceContainer {
        IEnumerable<UserControlReference> GetAllUserControlReferences();
    }

    public class RegisteredMethodInfo {
        private string _methodName;
        private CallType _callType;

        public RegisteredMethodInfo(string methodName, CallType callType) {
            _methodName = methodName;
            _callType = callType;
        }

        public string MethodName {
            get { return _methodName; }
            set { _methodName = value; }
        }


        public CallType CallType {
            get { return _callType; }
            set { _callType = value; }
        }


        public bool CustomCall {
            get { return (CallType.CustomCall == CallType); }
        }

        public bool CustomWFS {
            get { return (CallType.CustomWFS == CallType); }
        }

        public bool ModifiedCall {
            get { return (CallType.ModifiedCall == CallType); }
        }
    }


    /// <summary>
    /// Base class for all controls
    /// </summary>
    [SupportedProperties("Height, Enabled, ForeColor, BackColor, Visible, Focus, IgnoreTO, ToolTip, DataBinding, TabStop, EnableChangeTracking, UniqueKey, EnableLizenz, VisibleLizenz, AllowDrop, NullText")]
    [SupportedGuiEvents("Click, Validated, ValueChanged")]
    public partial class Control {
        private List<ControlStateInfo> _controlStateInfos = null;
        private List<Event> _definedEvents;
        private List<Event> _allUsableEvents;


        [XmlIgnore]
        public virtual bool ApplyBinding {
            get { return DataBinding == BooleanControlState.@true || DataBinding == BooleanControlState.UndefinedControlState; }
        }

        [XmlIgnore]
        public virtual bool IsChangeTrackingDefined {
            get { return EnableChangeTracking != ControlEnableChangeTracking.undefined; }
        }

        [XmlIgnore]
        public virtual bool HasNullText {
            get { return !string.IsNullOrEmpty(NullText); }
        }

        [XmlIgnore]
        public virtual string EnabledChangeTrackingValue {
            get {
                if (enableChangeTrackingField == ControlEnableChangeTracking.@false)
                    return "false";
                else if (enableChangeTrackingField == ControlEnableChangeTracking.@true) {
                    return "true";
                }
                else {
                    return "null";
                }
            }
        }

        [XmlIgnore]
        public virtual bool IsVisibleLizenz
        {
            get { return !string.IsNullOrEmpty(VisibleLizenz); }
        }

        [XmlIgnore]
        public virtual bool IsEnableLizenz
        {
            get { return !string.IsNullOrEmpty(EnableLizenz); }
        }

        [XmlIgnore]
        public override bool ClickAssigned {
            get {
                foreach (var definedEvent in DefinedEvents) {
                    if (definedEvent.IsClick) {
                        return true;
                    }
                }

                return false;
            
            }
        }


        [XmlIgnore]
        public virtual string StyleName {
            get { return null; }
        }

        [XmlIgnore]
        public virtual bool AllowTabStop {
            get { return true; }
        }

        /// <summary>
        /// Gets defined control state infos
        /// </summary>
        [XmlIgnore]
        public virtual ControlStateInfo[] ControlStateInfos {
            get {
                if (_controlStateInfos == null) {
                    _controlStateInfos = ControlStateReflector.Find(this);
                }
                return _controlStateInfos.ToArray();
            }
        }

        /// <summary>
        /// Events defined by User to use
        /// </summary>
        [XmlIgnore]
        public override List<Event> DefinedEvents {
            get { return ReadEventInfos(); }
        }

        private List<Event> ReadEventInfos() {
            if (_definedEvents == null) {
                _definedEvents = new List<Event>();
                if (Events == null) {
                    return _definedEvents;
                }

                foreach (Event ev in Events) {
                    EventDelegateInfo evExtInfo;
                    string eventName, controlName;

                    if (!string.IsNullOrEmpty(ev.BaseControlName) && !string.IsNullOrEmpty(ev.BaseEventName)) {
                        eventName = ev.BaseEventName;
                        controlName = ev.BaseControlName;
                    } else {
                        eventName = ev.Name;
                        controlName = GetType().Name;
                    }
                    evExtInfo = GuiControlsReflectedInfo.Instance.GetControlEventInfo(controlName, eventName, this);
                    if (evExtInfo == null) {
                        evExtInfo =
                            new EventDelegateInfo(eventName, "System.EventHandler", "System.Void",
                                                  new string[] {"System.Object", "System.EventArgs"});
                        //throw new NotSupportedException(string.Format("Event <{0}> is not supported", eventName, controlName));
                    }
                    ev.ExtendInfo = (EventDelegateInfo) evExtInfo.Clone();
                    ev.ExtendInfo.DelegateName = ev.Method;
                    _definedEvents.Add(ev);
                }
            }

            return _definedEvents;
        }

        /// <summary>
        /// All events supported by this Control
        /// 
        /// </summary>
        [XmlIgnore]
        public List<Event> AllSupportedEvents {
            get {
                if (_allUsableEvents == null) {
                    _allUsableEvents = new List<Event>();
                    foreach (EventDelegateInfo extInfo in GuiControlsReflectedInfo.Instance.GetControlEventInfos(this)) {
                        _allUsableEvents.Add(new Event(extInfo.EventName, extInfo.DelegateName, extInfo));
                    }
                }
                return _allUsableEvents;
            }
        }


        /// <summary>
        /// Adds event
        /// </summary>
        /// <param name="name">Name of the event</param>
        /// <param name="methodName">Name of the method handels this event</param>
        /// 
        public void AddEvent(string name, string methodName, CallType methodCallType) {
            List<Event> elements = new List<Event>();
            if (eventsField != null) {
                elements.AddRange(eventsField);
            }

            elements.Add(BuildEvent(name, methodName, methodCallType));
            eventsField = elements.ToArray();
        }

        private Event BuildEvent(string name, string methodName, CallType methodCallType) {
            if (string.IsNullOrEmpty(name)) {
                throw new ArgumentException("Der Name eines Events ist nicht definiert!");
            }
            Event ev = new Event();
            Match match =
                Regex.Match(name, @"(?<eventName>.+)\[(?<eventOrig>.*)\,\s*(?<controlNameOrig>.*)\]\s*",
                            RegexOptions.Singleline | RegexOptions.IgnoreCase);

            if (match.Success) {
                ev.Name = match.Groups["eventName"].Value;
                ev.BaseEventName = match.Groups["eventOrig"].Value;
                ev.BaseControlName = match.Groups["controlNameOrig"].Value;
            } else {
                ev.Name = name;
            }

            ev.CallType = methodCallType;
            ev.Method = methodName;
            ev.SenderControlName = PTName;
            return ev;
        }

        /// <summary>
        /// Removes an event from the list
        /// </summary>
        /// <param name="element">Event object</param>
        public void RemoveEvent(Event element) {
            List<Event> elements = new List<Event>(eventsField);
            elements.Remove(element);
            eventsField = elements.ToArray();
        }


        /// <summary>
        /// Gets true if Height of the control is defined
        /// </summary>
        [XmlIgnore]
        public bool HasHeight {
            get { return Height == 0 ? false : true; }
        }

        /// <summary>
        /// Gets disabled status of the control
        /// </summary>
        [XmlIgnore]
        public bool IsDisabled {
            get { return Enabled == BooleanControlState.@false; }
        }

        #region -- [ Controlstates ] ---

        /// <summary>
        /// Gets Enabled control state
        /// </summary>
        [XmlIgnore]
        public ControlState<BooleanControlState> EnableControlState {
            get { return new EnumValueControlState<BooleanControlState>(Enabled, "bool"); }
        }

        [XmlIgnore]
        public bool EnabledIfModified {
            get { return Enabled == BooleanControlState.modified; }
        }


        /// <summary>
        /// Gets Visible control state
        /// </summary>
        [XmlIgnore]
        public ControlState<BooleanControlState> VisibleControlState {
            get { return new EnumValueControlState<BooleanControlState>(Visible, "bool"); }
        }

        /// <summary>
        /// Gets TabStop
        /// </summary>
        [XmlIgnore]
        public ControlState<BooleanControlState> TabStopControlState {
            get { return new EnumValueControlState<BooleanControlState>(TabStop, "bool"); }
        }


        /// <summary>
        /// Gets Visible control state
        /// </summary>
        [XmlIgnore]
        public ControlState<BooleanControlState> FocusControlState {
            get { return new EnumValueControlState<BooleanControlState>(Focus, "bool"); }
        }


        /// <summary>
        /// Gets ForeColor control state
        /// </summary>
        [XmlIgnore]
        public ControlState<string> ForeColorControlState {
            get { return new StringValueControlState(ForeColor); }
        }

        /// <summary>
        /// Gets BackColor control state
        /// </summary>
        [XmlIgnore]
        public ControlState<string> BackColorControlState {
            get { return new StringValueControlState(BackColor); }
        }

        #endregion

        #region RootContainer Properties

        /// <summary>
        /// Root container is a form
        /// </summary>
        [XmlIgnore]
        public bool IsRootContainerForm {
            get { return RootContainer is Form; }
        }


        /// <summary>
        /// Root container is a dialog
        /// </summary>
        [XmlIgnore]
        public bool IsRootContainerDialog {
            get { return RootContainer is Dialog; }
        }


        /// <summary>
        /// Root container is a UserControl
        /// </summary>
        [XmlIgnore]
        public bool IsRootContainerUserControl {
            get { return RootContainer is UserControl; }
        }

        #endregion

        [XmlIgnore]
        public override List<RegisteredMethodInfo> UnregisteredEventMethods {
            get { return RootContainer.RegisterEventsReturnsNewAdded(DefinedEvents, false); }
        }

        [XmlIgnore]
        public override List<RegisteredMethodInfo> UnregisteredEventMethodsDeclares {
            get { return RootContainer.RegisterEventsReturnsNewAdded(DefinedEvents, true); }
        }

        [XmlIgnore]
        public virtual bool IsUserControlReference {
            get { return this is UserControlReference; }
        }


        [XmlIgnore]
        public virtual bool HasToolTip {
            get { return !string.IsNullOrEmpty(toolTipField); }
        }

        [XmlIgnore]
        public virtual bool UseAddFormHotKey {
            get { return false; }
        }
    }

    #region -- Event Gruppen --

    /// <summary>
    /// Hält Listen mit definierten Events.
    /// Vorerst gibt es Standard-Events, die die gleiche Signatur haben.
    /// 
    /// Diese Klasse wird im GUI-Generator verwendet, um zu prüfen welche Events-Methoden generiert werden müssen.
    /// </summary>
    internal class EventGroups {
        private static EventGroups _instance = null;
        private static readonly object thisLockObject = new object();
        private readonly List<string> _defaultEventHandler;

        public static bool IsSimpleEventHandler(string eventName) {
            return Instance._defaultEventHandler.Contains(eventName);
        }

        private static EventGroups Instance {
            get {
                if (_instance == null) {
                    lock (thisLockObject) {
                        if (_instance == null) {
                            _instance = new EventGroups();
                        }
                    }
                }
                return _instance;
            }
        }


        private EventGroups() {
            _defaultEventHandler = new List<string>();
            _defaultEventHandler.Add("Click");
            _defaultEventHandler.Add("Validated");
            _defaultEventHandler.Add("AfterRowActivate");
            _defaultEventHandler.Add("AfterCellActivate");
            _defaultEventHandler.Add("ValueChanged");
            _defaultEventHandler.Add("Leave");
            _defaultEventHandler.Add("CheckedValueChanged");
            _defaultEventHandler.Add("CheckedChanged");
            _defaultEventHandler.Add("SelectionChanged");
        }
    }

    #endregion

    public partial class Event {
        private EventDelegateInfo _extendedInfo = null;

        public Event(string eventName, string methodName, EventDelegateInfo extInfo) {
            Name = eventName;
            Method = methodName;
            _extendedInfo = extInfo;
        }

        public Event(string eventName, string methodName, CallType callType, EventDelegateInfo extInfo) {
            Name = eventName;
            Method = methodName;
            _extendedInfo = extInfo;
            CallType = callType;
        }

        [XmlIgnore]
        public EventDelegateInfo ExtendInfo {
            get { return _extendedInfo; }
            set { _extendedInfo = value; }
        }


        //TODO: folgende Eventeigenschaften später löschen!!!
        //-------------------------->
        //
        [XmlIgnore]
        public bool IsClick {
            get { return Name.Equals("Click", StringComparison.OrdinalIgnoreCase); }
        }

        [XmlIgnore]
        public bool IsSimpleEventHandler {
            get { return EventGroups.IsSimpleEventHandler(Name); }
        }

        #region TreeView Events

        [XmlIgnore]
        public bool IsAfterNodeSelect {
            get { return Name.Equals("AfterNodeSelect", StringComparison.OrdinalIgnoreCase); }
        }

        [XmlIgnore]
        public bool IsBeforeNodeSelect {
            get { return Name.Equals("BeforeNodeSelect", StringComparison.OrdinalIgnoreCase); }
        }

        [XmlIgnore]
        public bool IsAfterDataNodesCollectionPopulated {
            get { return Name.Equals("AfterDataNodesCollectionPopulated", StringComparison.OrdinalIgnoreCase); }
        }

        #endregion

        #region HeaderScroller

        [XmlIgnore]
        public bool IsHeaderScrollerNextItemClick {
            get { return Name.Equals("NextItemClick", StringComparison.OrdinalIgnoreCase); }
        }

        #endregion

        #region BrowsableTextbox

        [XmlIgnore]
        public bool IsEditorButtonClick {
            get { return Name.Equals("EditorButtonClick", StringComparison.OrdinalIgnoreCase); }
        }

        [XmlIgnore]
        public bool IsButtonClick {
            get { return Name.Equals("ButtonClick", StringComparison.OrdinalIgnoreCase); }
        }

        [XmlIgnore]
        public bool IsDoubleClick {
            get { return Name.Equals("DoubleClick", StringComparison.OrdinalIgnoreCase) || Name.Equals("DoubleClickRow", StringComparison.OrdinalIgnoreCase); }
        }

        [XmlIgnore]
        public bool IsButtonInfoClick {
            get { return Name.Equals("ButtonInfoClick", StringComparison.OrdinalIgnoreCase); }
        }

        #endregion

        [XmlIgnore]
        public bool IsValueChanged {
            get { return Name.Equals("ValueChanged", StringComparison.OrdinalIgnoreCase); }
        }

        [XmlIgnore]
        public bool IsSelectedTabChanged {
            get { return Name.Equals("SelectedTabChanged", StringComparison.OrdinalIgnoreCase); }
        }

        #region Grid Events

        [XmlIgnore]
        public bool IsAfterGridRowActivate {
            get { return Name.Equals("AfterGridRowActivate", StringComparison.OrdinalIgnoreCase); }
        }

        [XmlIgnore]
        public bool IsAfterSelectChange {
            get { return Name.Equals("AfterSelectChange", StringComparison.OrdinalIgnoreCase); }
        }

        #endregion
    }

    [SupportedKeyWords("CONTROLS, LAYOUT")]
    [SupportedControls(
        "Table, Chart, TABNAVIGATION, MULTIVIEW, ReportPreview, TimeTextbox, PhoneTextbox, Label, Textbox, PersistentPictureBox, Picturebox, FormattedLabel,FormattedTextbox, UserControlReference, TabStrip, Combobox, BrowsableTextbox, AmountTextbox, NumericTextbox, DynamicLabel, DynamicButton, RadiobuttonGroup, Radiobutton, FunctionButton, FunctionFunctionButton, Tree, DateTextbox, MaskTextbox, Checkbox, Dropdownbox, SelectionList, HeaderScroller, ExplorerBar, Cave, Scanner"
        )]
    public partial class Panel : IContainer, IGuiElementParent {
        [XmlIgnore]
        public virtual GuiElement[] NonVirtualGuiElements {
            get {
                List<GuiElement> guiElements = new List<GuiElement>(GuiElements.Items);
                foreach (GuiElement element in GuiElements.Items) {
                    if (element is IVirtualGuiElement) {
                        int indexOfIVrtual = guiElements.IndexOf(element);
                        guiElements.Remove(element);
                        guiElements.InsertRange(indexOfIVrtual, ((IVirtualGuiElement) element).IternalGuiElements);
                    }
                }
                return guiElements.ToArray();
            }
        }


        [XmlIgnore]
        public virtual GuiElement FirstElement {
            get {
                GuiElement[] elements = NonVirtualGuiElements;
                if (elements.Length < 1) {
                    throw new NotSupportedException("You must define at least one subelement.");
                }

                return elements[0];
            }
        }

        [XmlIgnore]
        public virtual GuiElement SecondElement {
            get {
                GuiElement[] elements = NonVirtualGuiElements;
                if (elements.Length < 2) {
                    throw new NotSupportedException("You must define two subelements.");
                }

                return elements[1];
            }
        }


        public void AddGuiElement(GuiElement element) {
            List<GuiElement> elements = new List<GuiElement>();
            if (guiElementsField != null) {
                elements.AddRange(guiElementsField.Items);
            }
            elements.Add(element);
            guiElementsField.Items = elements.ToArray();
        }

        public void RemoveGuiElement(GuiElement element) {
            List<GuiElement> elements = new List<GuiElement>(guiElementsField.Items);
            elements.Remove(element);
            guiElementsField.Items = elements.ToArray();
        }

        [XmlIgnore]
        public GuiElement[] Controls {
            get { return guiElementsField.Items; }
            set { guiElementsField.Items = value ?? new GuiElement[0]; }
        }


        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "pnl"; }
        }

        [XmlIgnore]
        public Layout Layout {
            get { return Item1; }
            set { Item1 = value; }
        }

        #region IGuiElementParent Members

        public GuiElement[] GetAllGuiElements() {
            List<GuiElement> elements = new List<GuiElement>();
            foreach (GuiElement element in Controls) {
                elements.Add(element);

                IGuiElementParent parent = element as IGuiElementParent;
                if (parent != null) {
                    elements.AddRange(parent.GetAllGuiElements());
                }
            }
            return elements.ToArray();
        }

        #endregion

        [XmlIgnore]
        public bool EnabledIfModified {
            get { return false; }
        }

        [XmlIgnore]
        public bool HasToolTip {
            get { return false; }
        }

        [XmlIgnore]
        public bool IsChangeTrackingDefined {
            get { return false; }
        }

        [XmlIgnore]
        public bool IsEnableLizenz {
            get { return false; }
        }

        [XmlIgnore]
        public bool IsVisibleLizenz {
            get { return false; }
        }

        [XmlIgnore]
        public virtual bool ApplyBinding {
            get { return false; }
        }
    }

    [SupportedProperties("DetailText")]
    public partial class DetailsPanel {}

    #region MultiView

    /// <summary>
    /// Extendcontrol additionally contains GuiElements
    /// </summary>
    public partial class ExtendedControl {
        public ExtendedControl() {
            guiElementsField = new ExtendedControlGuiElements();
            HotKeys = new GuiElementHotKeys();
            Events = new Event[0];
            LayoutInfo = new AbsolutLayoutInfo();
        }
    }

    public partial class ExtendedControlGuiElements {
        public ExtendedControlGuiElements() {
            itemsField = new GuiElement[0];
        }
    }


    [SupportedControls("UserControlReference")]
    public partial class MultiViewContainer : IContainer, IGuiElementParent, IUserControlReferenceContainer {
        public MultiViewContainer() {
            Controls = new GuiElement[0];
        }

        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "mvc"; }
        }


        [XmlIgnore]
        public virtual GuiElement[] NonVirtualGuiElements {
            get {
                List<GuiElement> guiElements = new List<GuiElement>(GuiElements.Items);
                foreach (GuiElement element in GuiElements.Items) {
                    if (element is IVirtualGuiElement) {
                        int indexOfIVrtual = guiElements.IndexOf(element);
                        guiElements.Remove(element);
                        guiElements.InsertRange(indexOfIVrtual, ((IVirtualGuiElement) element).IternalGuiElements);
                    }
                }
                return guiElements.ToArray();
            }
        }


        [XmlIgnore]
        public virtual string SelectedControlName {
            get { return Controls.Length > 0 ? Controls[0].Name : null; }
        }

        [XmlIgnore]
        public virtual string UserControlNamesString {
            get {
                string controlString = string.Empty;
                foreach (GuiElement gui in Controls) {
                    controlString += gui.Name + ",";
                }
                return controlString;
            }
        }


        [XmlIgnore]
        private List<GuiElement> GuiElementList {
            get { return new List<GuiElement>(GuiElements.Items); }
        }

        private Layout _layout = new AbsolutLayout();

        [XmlIgnore]
        public Layout Layout {
            get { return _layout; }
            set { _layout = value; }
        }

        [XmlIgnore]
        public GuiElement[] Controls {
            get { return GuiElements.Items; }
            set { GuiElements.Items = value; }
        }


        public void AddGuiElement(GuiElement element) {
            List<GuiElement> elements = GuiElementList;
            elements.Add(element);
            GuiElements.Items = elements.ToArray();
        }

        public void RemoveGuiElement(GuiElement element) {
            List<GuiElement> elements = GuiElementList;
            elements.Remove(element);
            GuiElements.Items = elements.ToArray();
        }


        public GuiElement[] GetAllGuiElements() {
            return GuiElementList.ToArray();
        }

        public IEnumerable<UserControlReference> GetAllUserControlReferences() {
            foreach (GuiElement control in GetAllGuiElements()) {
                if (control is UserControlReference) {
                    yield return control as UserControlReference;
                }
            }
        }
    }

    #endregion

    public enum MergeAlign {
        Left = 0,
        Right = 1
    }

    [SupportedProperties("Row, Col, RowSpan, ColSpan, ColMode, Width, NewLine,NewLines, NewColumn, Merge, MergeAlign, MergeSpan")]
    public partial class GridLayoutInfo {
        private bool _newLine;
        private bool _newColumn = false;
        private bool _merge;
        private int _mergeSpan;
        private MergeAlign _mergeAlign = MergeAlign.Left;

        [XmlIgnore]
        public bool HasColumnSpan {
            get { return colSpanField > 1; }
        }

        [XmlIgnore]
        public bool HasRowSpan {
            get { return rowSpanField > 1; }
        }

        [XmlIgnore]
        public bool IsFill {
            get { return ColMode == GridLayoutInfoColMode.Fill; }
        }

        [XmlIgnore]
        public bool HasWidth {
            get { return Width > 0; }
        }

        [XmlIgnore]
        public bool Merge {
            get { return _merge; }
            set { _merge = value; }
        }

        [XmlIgnore]
        public int MergeSpan {
            get { return _mergeSpan; }
            set { _mergeSpan = value; }
        }

        [XmlIgnore]
        public MergeAlign MergeAlign {
            get { return _mergeAlign; }
            set { _mergeAlign = value; }
        }

        [XmlIgnore]
        public bool NewLine {
            get { return _newLine; }
            set { _newLine = value; }
        }

        private int _newLinesCount = 0;

        [XmlIgnore]
        public int NewLines {
            get { return _newLinesCount; }
            set { _newLinesCount = value; }
        }

        [XmlIgnore]
        public bool NewColumn {
            get { return _newColumn; }
            set { _newColumn = value; }
        }
    }


    [SupportedProperties("X, Y, Width, Height")]
    public partial class AbsolutLayoutInfo {}

    [SupportedProperties(
        "SplitterMode, FixedPanel, SplitterDistance, SplitterWidth, SplitterIncrement, Panel1Collapsed, Panel1MinSize, Panel2Collapsed, Panel2MinSize, SplitterOrientation, IsSplitterFixed"
        )]
    public partial class SplitLayout {
        public bool IsDialog {
            get { return false; }
        }

        public bool IsHeader {
            get { return false; }
        }

        public bool IsContent {
            get { return true; }
        }

        public bool IsFooter {
            get { return false; }
        }
    }


    [SupportedProperties(
        "ColumnCount, RowCount, RowInfos, ColumnInfos, RowDefaultMode, RowDefaultValue, ColumnDefaultMode, ColumnDefaultValue, Type")]
    public partial class GridLayout {
        public bool IsDialog {
            get { return false; }
        }

        public bool IsHeader {
            get { return Type == GridLayoutType.Header; }
        }

        public bool IsContent {
            get { return Type == GridLayoutType.Content; }
        }

        public bool IsFooter {
            get { return Type == GridLayoutType.Footer; }
        }

        [XmlIgnore]
        public int RowCount {
            get { return Rows; }

            set {
                Rows = value;
                RowInfos = new GridLayoutConstraint[value];
                for (int i = 0; i < RowInfos.Length; i++) {
                    RowInfos[i] = new GridLayoutConstraint();
                }
                UpdateDefaultRowSettings();
            }
        }

        [XmlIgnore]
        public int ColumnCount {
            get { return Columns; }

            set {
                Columns = value;
                ColumnInfos = new GridLayoutConstraint[value];
                for (int i = 0; i < ColumnInfos.Length; i++) {
                    ColumnInfos[i] = new GridLayoutConstraint();
                }
                UpdateDefaultColSettings();
            }
        }

        private GridLayoutConstraintMode _rowDefaultMode = GridLayoutConstraintMode.Variable;

        [XmlIgnore]
        public GridLayoutConstraintMode RowDefaultMode {
            get { return _rowDefaultMode; }
            set {
                _rowDefaultMode = value;
                UpdateDefaultRowSettings();
            }
        }

        private int _rowDefaultValue;

        [XmlIgnore]
        public int RowDefaultValue {
            get { return _rowDefaultValue; }
            set {
                _rowDefaultValue = value;
                UpdateDefaultRowSettings();
            }
        }

        private GridLayoutConstraintMode _columnDefaultMode = GridLayoutConstraintMode.Variable;

        [XmlIgnore]
        public GridLayoutConstraintMode ColumnDefaultMode {
            get { return _columnDefaultMode; }
            set {
                _columnDefaultMode = value;
                UpdateDefaultColSettings();
            }
        }

        private int _columnDefaultValue = 100;

        [XmlIgnore]
        public int ColumnDefaultValue {
            get { return _columnDefaultValue; }
            set {
                _columnDefaultValue = value;
                UpdateDefaultColSettings();
            }
        }

        private void UpdateDefaultColSettings() {
            if (ColumnInfos != null) {
                foreach (GridLayoutConstraint colInfo in ColumnInfos) {
                    colInfo.Mode = ColumnDefaultMode;
                    colInfo.Value = ColumnDefaultValue;
                }
            }
        }

        private void UpdateDefaultRowSettings() {
            if (RowInfos != null) {
                foreach (GridLayoutConstraint rowInfo in RowInfos) {
                    rowInfo.Mode = RowDefaultMode;
                    rowInfo.Value = RowDefaultValue;
                }
            }
        }
    }


    [SupportedProperties("Mode, Value")]
    public partial class GridLayoutConstraint {
        [XmlIgnore]
        public bool IsVariable {
            get { return Mode == GridLayoutConstraintMode.Variable; }
        }

        [XmlIgnore]
        public bool IsAutoSize {
            get { return Mode == GridLayoutConstraintMode.Variable && Value == 0; }
        }
    }

    [SupportedProperties("*Text")]
    public partial class Label {
        [XmlIgnore]
        public override bool ApplyBinding {
            get { return DataBinding == BooleanControlState.@true; }
        }

        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "lbl"; }
        }

        [XmlIgnore]
        public bool IsDynamic {
            get { return false; }
        }

        [XmlIgnore]
        public override bool AllowTabStop {
            get { return false; }
        }
    }

    /// <summary>
    /// Reference description for custom user controls
    /// </summary>
    [SupportedProperties("*TypeName, SetPrefix, TypeNameTO, NoGdRef")]
    public partial class UserControlReference {
        /// <summary>
        /// Gets the name of the Control with the prefix
        ///
        /// </summary>
        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "usr"; }
        }

        [XmlIgnore]
        public override bool SupportThemesProperty {
            get { return false; }
        }

        [XmlIgnore]
        public virtual bool IsInAdvanceTabPage {
            get { return ParentContainer is TabPage && ((TabPage) ParentContainer).IsAdvanced; }
        }

        [XmlIgnore]
        public virtual bool Shared {
            get { return ParentContainer is TabPage && ((TabPage) ParentContainer).Shared; }
        }

        private string _typeFullName;

        /// <summary>
        /// FullName of the controls type.
        /// Pharmatechnik controls contains a prefix "<b>Usr</b>"
        /// 
        /// Use SetPrefix to enable/disable prefix creation for this type name 
        /// </summary>
        [XmlIgnore]
        public virtual string TypeFullName {
            get {
                if (_typeFullName == null) {
                    int lastIndex = TypeName.LastIndexOf(".");
                    _typeFullName = SetPrefix && lastIndex > -1 ? TypeName.Insert(TypeName.LastIndexOf(".") + 1, "Usr") : TypeName;
                }
                return _typeFullName;
            }
        }

        private string _fullTOName;

        /// <summary>
        /// Gets the type name of the TO
        /// </summary>
        [XmlIgnore]
        public string TOTypeFullName {
            get {
                if (_fullTOName == null) {
                    TypeNameTO = TypeNameTO ?? string.Empty;
                    _fullTOName = TypeNameTO.Length > 0 ? TypeNameTO : GetTONamespace(TypeName) + "." + GetNameOnly(TypeName) + "TO";
                }
                return _fullTOName;
            }
        }

        private string GetNameOnly(string fullName) {
            if (fullName == null || fullName.Length == 0) {
                return string.Empty;
            }

            int lastPoint = fullName.LastIndexOf(".");
            if (lastPoint > 0) {
                fullName = fullName.Substring(lastPoint + 1);
            }
            return fullName;
        }


        private string GetTONamespace(string controlTypeName) {
            if (controlTypeName != null && controlTypeName.Length > 0) {
                if (controlTypeName.IndexOf(".GUI") > -1) {
                    return controlTypeName.Substring(0, controlTypeName.LastIndexOf(".GUI")) + ".IWFL";
                } else {
                    int lastPoint = controlTypeName.LastIndexOf(".");
                    if (lastPoint > 0) {
                        return controlTypeName.Substring(0, lastPoint) + ".IWFL";
                    } else {
                        return controlTypeName + ".IWFL";
                    }
                }
            }
            return controlTypeName;
        }


        public override bool Equals(object obj) {
            UserControlReference g = obj as UserControlReference;
            if (g == null) {
                return false;
            }

            return g.PTName.Equals(PTName);
        }

        public override int GetHashCode() {
            return PTName.GetHashCode();
        }


        [XmlIgnore]
        public bool IsNotMultiViewParent {
            get { return !(ParentContainer is MultiViewContainer); }
        }
    }

    [SupportedProperties("Text")]
    public partial class DynamicLabel {
        [XmlIgnore]
        public bool IsDynamic {
            get { return true; }
        }

        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "lbl"; }
        }

        [XmlIgnore]
        public override bool AllowTabStop {
            get { return false; }
        }
    }

    [SupportedProperties("Text")]
    public partial class FormattedLabel {
        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "lbl"; }
        }
    }

    [SupportedProperties("Text, ReadOnly")]
    public partial class FormattedTextbox {
        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "tbx"; }
        }

        public override bool AutoWriteTOValue {
            get { return true; }
        }
    }

    [SupportedGuiEvents("CurrentPageChanged")]
    public partial class ReportPreview {

        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "rpt"; }
        }

        [XmlIgnore]
        public bool IsDynamic {
            get { return true; }
        }
    }

    [SupportedProperties("Text, Length, ReadOnly, Password, Multiline, ValidatingExpression, ImageKey, ImageLibrary, AutoCapitalizeFirstCharacter")]
    [SupportedGuiEvents("Changed")]
    public partial class Textbox {
        [XmlIgnore]
        public bool HasLength {
            get { return Length > 0; }
        }

        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "tbx"; }
        }

        [XmlIgnore]
        public override bool AutoWriteTOValue {
            get { return true; }
        }

        /// <summary>
        /// Gets ImageKey control state
        /// </summary>
        [XmlIgnore]
        public ControlState<string> ImageKeyControlState {
            get { return new StringValueControlState(ImageKey); }
        }


        /// <summary>
        /// Gets ImageKey control state
        /// </summary>
        [XmlIgnore]
        public ControlState<string> ImageLibraryControlState {
            get {
                
                var lib = new StringValueControlState(ImageLibrary);
                if (lib.Undefined) {
                    lib.Value = "Keine";
                }

                return lib;
            }
        }
    }


    [SupportedProperties("Caption, AutoFitStyle, ColHeadersVisible")]
    public partial class SelectionList {
        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "lbx"; }
        }
    }


    [SupportedProperties("ImageKey, ImageLibrary, ScaleImage, Enable3DEffekt, ScaleImageOnlyDownsize")]
    public partial class Picturebox {
        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "pbx"; }
        }




        [XmlIgnore]
        public override bool AllowTabStop {
            get { return false; }
        }

        [XmlIgnore]
        public string ResolvedImageLibrary {
            get {
                if (string.IsNullOrEmpty(imageLibraryField)) {
                    return "Keine";
                }

                return imageLibraryField;
            }
        }
    }

    [SupportedProperties("ScaleImage, ScaleImageOnlyDownsize")]
    public partial class PersistentPictureBox {
        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "pbx"; }
        }


        [XmlIgnore]
        public override bool AllowTabStop {
            get { return false; }
        }
    }

    [SupportedProperties("TabInfos, *TabCount")]
    public partial class TabStrip {
        private int _tabCount = 0;

        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "tbc"; }
        }

        [XmlIgnore]
        public int TabCount {
            get { return _tabCount; }

            set {
                _tabCount = value;
                TabInfos = new TabStripTabInfo[_tabCount];
                for (int i = 0; i < TabInfos.Length; i++) {
                    TabInfos[i] = new TabStripTabInfo();
                }
            }
        }


        [XmlIgnore]
        public string TabRange {
            get {
                string tabInfos = string.Empty;

                foreach (TabStripTabInfo tabInfo in TabInfos) {
                    tabInfos += string.Format("{0}{1}Tab,", PTName, tabInfo.Key);
                }
                tabInfos = tabInfos.Remove(tabInfos.Length - 1, 1);

                return tabInfos;
            }
        }
    }

    /// <summary>
    /// Tab of the tab control
    /// </summary>
    [SupportedProperties("*Key, Width, *Caption, Hidden, UserControlReference")]
    public partial class TabStripTabInfo {
        /// <summary>
        /// Checks if control has defined control references
        /// </summary>
        [XmlIgnore]
        public bool HasUserControlReference {
            get { return (UserControlReference != null) && (UserControlReference.Trim() != string.Empty); }
        }
    }

    /// <summary>
    /// Headerscroller control is used to navigate and preview list items data
    /// </summary>
    [SupportedProperties("HideSpinButtons")]
    [SupportedGuiEvents("NextItemClick")]
    public partial class HeaderScroller {
        /// <summary>
        /// Gets Name of the control with defined prefix
        /// </summary>
        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "hds"; }
        }


        [XmlIgnore]
        public override InputModeBehavior InputModeBehavior {
            get { return new InputModeBehavior(GridColumnBehavior.ReadOnly); }
        }

        [XmlIgnore]
        public string PTGrid {
            get { return PTName + ".PT_Grid"; }
        }


        [XmlIgnore]
        public TableAutoFitStyle HeaderAutoFitStyle {
            get {
                if (AutoFitStyle == TableAutoFitStyle.None) {
                    return TableAutoFitStyle.ResizeAllColumns;
                } else {
                    return AutoFitStyle;
                }
            }
        }


        /// <summary>
        /// Check if this control supports microsoft windows themes
        /// </summary>
        [XmlIgnore]
        public override bool SupportThemesProperty {
            get { return false; }
        }
    }

    public partial class TableWithPaging : Table{
        
    }

    [SupportedGuiEvents("AfterRowActivate, AfterCellActivate, AfterSelectChange, DragDropRowMoveExtended")]
    [SupportedProperties(
       "#PreparedBandInfoTypes, Caption, Behavior, ColumnInfos, *ColumnCount, DefaultColWidth, DefaultRowHeight, MinRowHeight, AllowAddNewRow, AddNewRowText, AutoFitStyle,RowSelectionMode, CellLinesCount, BandInfos, BandCount, ShowRowSelector, Sortable, HotKeyUpDownLoop, Paging, CachePageCount, DragAndDrop"
        )]
    public partial class Table {
        private int _bandCount = 1;
        private int _columnCount = 2;

        public override bool AutoWriteTOValue {
            get { return true; }
        }

        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "grd"; }
        }

        public virtual bool HasRowSelector {
            get { return ShowRowSelector == TableShowRowSelector.True; }
        }

        [XmlIgnore]
        public int ColumnCount {
            get { return _columnCount; }

            set {
                _columnCount = value;
                ColumnInfos = new ColumnInfoType[_columnCount];
                for (int i = 0; i < ColumnInfos.Length; i++) {
                    ColumnInfos[i] = new ColumnInfoType(this);
                }
            }
        }


        [XmlIgnore]
        public int BandCount {
            get { return _bandCount; }
            set { SetInitBandCount(value); }
        }

        [XmlIgnore]
        public string FirstTemplateEnum {
            get { return "T" + Name + PreparedBandInfoTypes[0].TOBandName + "Columns"; }
        }


        [XmlIgnore]
        public bool HasManagedColumns {
            get { return ColumnInfos.Any(columnInfoType => columnInfoType.Managed); }
        }

        [XmlIgnore]
        public string TemplateEnums {
            get {
                string templateEnums = "";
                foreach (BandInfoType band in PreparedBandInfoTypes) {
                    templateEnums += "T" + Name + band.TOBandName + "Columns";
                    templateEnums += ",";
                }

                return templateEnums.Substring(0, templateEnums.Length - 1);
            }
        }

        [XmlIgnore]
        public string FirstEnum {
            get { return Name + PreparedBandInfoTypes[0].TOBandName + "Columns"; }
        }

        [XmlIgnore]
        public string Enums {
            get {
                string templateEnums = "";
                foreach (BandInfoType band in PreparedBandInfoTypes) {
                    templateEnums += Name + band.TOBandName + "Columns";
                    templateEnums += ",";
                }

                return templateEnums.Substring(0, templateEnums.Length - 1);
            }
        }

        public BandInfoType[] BandInfosWithoutFirst {
            get {
                List<BandInfoType> list = new List<BandInfoType>(PreparedBandInfoTypes);
                list.RemoveAt(0);
                return list.ToArray();
            }
        }

        private void SetInitBandCount(int bandCount) {
            _bandCount = bandCount;
            BandInfos = new BandInfoType[_bandCount];
            for (int i = 0; i < BandInfos.Length; i++) {
                BandInfoType tblBandInfo = new BandInfoType();
                tblBandInfo.Key = Name;
                BandInfos[i] = tblBandInfo;
            }
        }

        [XmlIgnore]
        public bool IsMultiBand {
            get { return (PreparedBandInfoTypes.Length > 1); }
        }

        [XmlIgnore]
        public bool IsMultiLine {
            get { return (CellLinesCount > 1); }
        }

        [XmlIgnore]
        public BandInfoType MainBand {
            get { return BandInfos[0]; }
        }

        private bool _bandsPrepared = false;

        [XmlIgnore]
        public BandInfoType[] PreparedBandInfoTypes {
            get {
                PrepareBandsAndCoulumns();
                return BandInfos;
            }
        }

        [XmlIgnore]
        public ColumnInfoType[] PreparedColumnInfos {
            get {
                PrepareBandsAndCoulumns();
                return ColumnInfos;
            }
        }

        [XmlIgnore]
        public virtual string ViewStyle {
            get { return IsMultiBand ? "MultiBand" : "SingleBand"; }
        }

        [XmlIgnore]
        public virtual bool AddNewRowEnabled {
            get { return (AllowAddNewRow != AllowAddNewRow.Default && AllowAddNewRow != AllowAddNewRow.No); }
        }

        private void PrepareBandsAndCoulumns() {
            if (!_bandsPrepared) {
                BandInfos = BandColumnMapper.PrepareBands(ColumnInfos, BandInfos, this);
                _bandsPrepared = true;
            }
        }

        public void InitalizeEditorControls() {
            foreach (ColumnInfoType columnInfo in ColumnInfos) {
                columnInfo.EditorUIControl = columnInfo.EditorUiFactory.Create();
            }
        }

        [XmlIgnore]
        public override Container RootContainer {
            get { return base.RootContainer; }
            set {
                base.RootContainer = value;
                if (ColumnInfos == null) {
                    return;
                }

                foreach (ColumnInfoType columnInfo in ColumnInfos) {
                    if (columnInfo.EditorUIControl != null) {
                        columnInfo.EditorUIControl.RootContainer = value;
                        columnInfo.EditorUIControl.Parent = this;
                        columnInfo.EditorUIControl.CurrentCol = columnInfo;
                    }
                }
            }
        }

        /// <summary>
        /// Nach der Deserialsierung werden die Bands und Columns verlinkt...
        /// </summary>
        internal override void InitElement() {
            base.InitElement();
            PrepareBandsAndCoulumns();
        }


        private InputModeBehavior _inputModeBehavior;

        [XmlIgnore]
        public virtual InputModeBehavior InputModeBehavior {
            get {
                if (_inputModeBehavior == null) {
                    _inputModeBehavior = new InputModeBehavior(Behavior);
                }
                return _inputModeBehavior;
            }
        }
    }


    //[SupportedGuiEvents("AfterRowActivate, AfterCellActivate, AfterSelectChange")]
    [SupportedProperties("ColumnInfos, *ColumnCount, SwapRowsAndColumns, Title, Legend, LegendLocation, LegendSpanPercentage, YItemLabelWidth, XItemLabelHeight, XSeriesLabelFormat, YSeriesLabelFormat, XItemLabelFormat, YItemLabelFormat, ColumnColorPalette, ShowBorder, Backgroundcolor, LegendShowBorder, YRangeRounding, YRangeDataMinFaktor, YRangeDataMaxFaktor, TitleStyle")]
    public partial class Chart {

        private int _columnCount = 2;

        [XmlIgnore]
        public bool IsSwapRowsAndColumns {
            get {
                if (swapRowsAndColumnsField == ChartSwapRowsAndColumns.@true) {
                    return true;
                }
                return false;
            }
        }


        [XmlIgnore]
        public bool IsGanttChart {
            get {
                if (Style == "Gantt") {
                    return true;
                }
                return false;
            }
        }

        [XmlIgnore]
        public bool HasLegend {
            get {
                if (Legend == ChartLegend.@true) {
                    return true;
                }
                return false;
            }
        }

        [XmlIgnore]
        public bool HasLegendSpanPercentage {
            get {
                if (LegendSpanPercentage >= 0) {
                    return true;
                }
                return false;
            }
        }

        [XmlIgnore]
        public bool HasXItemLabelHeight {
            get {
                if (XItemLabelHeight >= 0) {
                    return true;
                }
                return false;
            }
        }

        [XmlIgnore]
        public bool HasYItemLabelWidth {
            get {
                if (YItemLabelWidth >= 0) {
                    return true;
                }
                return false;
            }
        }
        


        [XmlIgnore]
        public bool HasToolTipColumn {
            get {
                foreach (var info in ColumnInfos) {
                    if (info.IsTooltipColumn) {
                        return true;
                    }
                }
                return false;
            }
        }

        [XmlIgnore]
        public string ToolTipColumn {
            get {
                foreach (var info in ColumnInfos) {
                    if (info.IsTooltipColumn) {
                        return info.Key;
                    }
                }
                throw new NotSupportedException("no ToolTipColumn");
            }
        }



        [XmlIgnore]
        public bool HasTaskTitleColumn {
            get {
                foreach (var info in ColumnInfos) {
                    if (info.IsTaskTitleColumn) {
                        return true;
                    }
                }
                return false;
            }
        }

        [XmlIgnore]
        public string TaskTitleColumn {
            get {
                foreach (var info in ColumnInfos) {
                    if (info.IsTaskTitleColumn) {
                        return info.Key;
                    }
                }
                throw new NotSupportedException("no TaskTitleColumn");
            }
        }




        [XmlIgnore]
        public bool HasColorColumn {
            get {
                foreach (var info in ColumnInfos) {
                    if (info.Type == "PT_Color") {
                        return true;
                    }
                }
                return false;
            }
        }

        [XmlIgnore]
        public string ColorColumn {
            get {
                foreach (var info in ColumnInfos) {
                    if (info.Type == "PT_Color") {
                        return info.Key;
                    }
                }
                throw new NotSupportedException("no ColorColumn");
            }
        }

        [XmlIgnore]
        public bool HasHatchColumn {
            get {
                foreach (var info in ColumnInfos) {
                    if (info.Type == "FillHatchStyleEnum") {
                        return true;
                    }
                }
                return false;
            }
        }

       

        [XmlIgnore]
        public string HatchColumn {
            get {
                foreach (var info in ColumnInfos) {
                    if (info.Type == "FillHatchStyleEnum") {
                        return info.Key;
                    }
                }
                throw new NotSupportedException("no HatchColumn");
            }
        }

        [XmlIgnore]
        public string IdColumn {
            get {
                foreach (var info in ColumnInfos) {
                    if (info.IsIdColumn) {
                        return info.Key;
                    }
                }
                throw new NotSupportedException("no IdColumn");
            }
        }




        [XmlIgnore]
        public string SeriesColumn {
            get {
                return ColumnInfos[0].Key;
            }
        }

        [XmlIgnore]
        public string TaskColumn {
            get {
                return ColumnInfos[1].Key;
            }
        }

        [XmlIgnore]
        public bool IsChartWithColumnCaptions {
            get {
                if (Style == "Line" || Style == "Bar") {
                    return true;
                }
                return false;
            }
        }

        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "cha"; }
        }

        [XmlIgnore]
        public int ColumnCount {
            get { return _columnCount; }

            set {
                _columnCount = value;
                ColumnInfos = new ChartColumnType[_columnCount];
                for (int i = 0; i < ColumnInfos.Length; i++) {
                    ColumnInfos[i] = new ChartColumnType(this);
                }
            }
        }

        [XmlIgnore]
        public ChartColumnType[] DataRows {
            get {
                List<ChartColumnType> result = new List<ChartColumnType>();
                foreach (var info in ColumnInfos) {
                    if (info.Type == "double" || info.Type == "int" || info.Type == "float" || info.Type == "decimal" ||
                        info.Type == "double?" || info.Type == "int?" || info.Type == "float?" || info.Type == "decimal?") {
                        result.Add(info);
                    }
                }

                return result.ToArray();
            }
        }
     
        [XmlIgnore]
        public ChartColumnType[] PreparedColumnInfos {
            get {
                return ColumnInfos;
            }
        }

        /// <summary>
        /// Gets range of column names separated by comma
        /// </summary>
        [XmlIgnore]
        public string ColumnNames {
            get {
                string colInfos = string.Empty;

                foreach (ChartColumnType colInfo in ColumnInfos) {
                    colInfos += string.Format("{0},", colInfo.Key);
                }
                colInfos = colInfos.Length > 0 ? colInfos.Remove(colInfos.Length - 1, 1) : colInfos;

                return colInfos;
            }
        }

        /// <summary>
        /// Gets range of column names separated by comma
        /// </summary>
        [XmlIgnore]
        public string ColumnTOs {
            get {
                string colInfos = string.Empty;

                foreach (ChartColumnType colInfo in ColumnInfos) {
                    colInfos += string.Format("to.{0},", colInfo.Key);
                }
                colInfos = colInfos.Length > 0 ? colInfos.Remove(colInfos.Length - 1, 1) : colInfos;

                return colInfos;
            }
        }

        /// <summary>
        /// Gets range of column names separated by comma
        /// </summary>
        [XmlIgnore]
        public string ColumnMembers {
            get {
                string colInfos = string.Empty;

                foreach (ChartColumnType colInfo in ColumnInfos) {
                    colInfos += string.Format("{0},", colInfo.MemberVariableName);
                }
                colInfos = colInfos.Length > 0 ? colInfos.Remove(colInfos.Length - 1, 1) : colInfos;

                return colInfos;
            }
        }

        /// <summary>
        /// Gets range of column names separated by comma
        /// </summary>
        [XmlIgnore]
        public string ColumnCaptions {
            get {
                switch (Style) {
                    case "Bar":
                        return string.Join(", ", ColumnInfos.Where(c => c.IsDataType).Select(c => $@"""{c.ColumnCaption}"""));
                    default:
                        //TODO Eisi: Eigentlich das gleiche wie oben. Ich traue mich aber nicht das zu ändern...
                        int start = 0;
                        if (IsSwapRowsAndColumns) {
                            //die erste Column ist hier DateTime, und somit nicht in der Legende
                            start = 1;
                        }
                        return string.Join(", ", ColumnInfos.Skip(start).Select(c => $@"""{c.ColumnCaption}"""));
                }
                
            }
        }


        /// <summary>
        /// Gets range of column names separated by comma
        /// </summary>
        [XmlIgnore]
        public string ColumnMembersWithType {
            get {
                string colInfos = string.Empty;

                foreach (ChartColumnType colInfo in ColumnInfos) {
                    colInfos += string.Format("{0} {1},", colInfo.Type, colInfo.MemberVariableName);
                }
                colInfos = colInfos.Length > 0 ? colInfos.Remove(colInfos.Length - 1, 1) : colInfos;

                return colInfos;
            }
        }

        [XmlIgnore]
        public string Caption {
            get { return null; }
        }
    }



    /// <summary>
    /// Mapps ColumnsInfos Bands to Bands
    /// </summary>
    internal class BandColumnMapper {
        public static BandInfoType[] PrepareBands(ColumnInfoType[] tableColumns, BandInfoType[] bandInfos, Control table) {
            //wenn keine Band definiert ist, dann wird eine Default-Band erstellt
            if (bandInfos == null || bandInfos.Length == 0) {
                BandInfoType bandInfo = new BandInfoType();
                bandInfo.Key = table.Name;

                bandInfos = new BandInfoType[] {bandInfo};
            }

            //Setzen aller ParentBandKeys für alle Bands, falls diese nicht vorhanden sind.
            for (int i = 0; i < bandInfos.Length; i++) {
                if (bandInfos[i].ParentBand == null || bandInfos[i].ParentBand.Length == 0) {
                    if (i == 0) {
                        continue;
                    } else {
                        bandInfos[i].ParentBand = bandInfos[i - 1].Key;
                    }
                }
            }

            //für alle Bands die Childs zuordnen und Setzen des ParentBandIndex für Infragistics-Hierarchie
            int bandIndex = 0;
            foreach (BandInfoType bandInfo in bandInfos) {
                BandInfoType parentBand = GetBandByKey(bandInfo.ParentBand, bandInfos);
                if (parentBand != null) {
                    parentBand.ChildBands.Add(bandInfo);
                    bandInfo.ParentBandIndex = GetBandIndexByKey(parentBand, bandInfos);
                } else {
                    bandInfo.ParentBandIndex = -1;
                }
                bandInfo.Table = table;
                bandInfo.BandIndex = bandIndex++;
            }


            //Zuordnung von den Columns zu den Bands
            string lastBandKey = bandInfos[0].Key;
            BandInfoType band;

            foreach (ColumnInfoType colInfo in tableColumns) {
                if (colInfo.Band != null) {
                    lastBandKey = colInfo.Band;
                }

                band = GetBandByKey(lastBandKey, bandInfos);
                if (band != null) {
                    //                    band.TableName = table.PTName;
                    //                    band.Table = table;
                    colInfo.Band = band.Key;
                    colInfo.BandInfo = band;
                    if (!band.ColumnInfos.Contains(colInfo)) {
                        band.ColumnInfos.Add(colInfo);
                    }
                }
            }

            return bandInfos;
        }

        private static int GetBandIndexByKey(BandInfoType band, BandInfoType[] bandInfos) {
            for (int i = 0; i < bandInfos.Length; i++) {
                if (bandInfos[i].Key.Equals(band.Key, StringComparison.CurrentCultureIgnoreCase)) {
                    return i;
                }
            }
            return -1;
        }


        private static BandInfoType GetBandByKey(string key, IEnumerable<BandInfoType> bands) {
            foreach (BandInfoType band in bands) {
                if (band.Key.Equals(key, StringComparison.CurrentCultureIgnoreCase)) {
                    return band;
                }
            }
            return null;
        }
    }

    public class InputModeBehavior {
        private readonly GridColumnBehavior _behavior = GridColumnBehavior.Default;

        public InputModeBehavior(GridColumnBehavior behavior) {
            _behavior = behavior;
        }

        public bool IsDefault {
            get { return _behavior == GridColumnBehavior.Default; }
        }

        public bool IsReadOnly {
            get { return _behavior == GridColumnBehavior.ReadOnly; }
        }

        public bool IsEditable {
            get { return _behavior == GridColumnBehavior.Editable; }
        }
    }


    [SupportedProperties("Key, ColHeadersVisible, ParentBand, Behavior, AllowColSizing, AllowDelete, AllowAddNew, AllowUpdate, Inherits")]
    public partial class BandInfoType {
        private List<ColumnInfoType> _columnInfos = null;
        private Control _table = null;
        private int _parentBandIndex = -1;
        private int _bandIndex = 0;

        [XmlIgnore]
        public int BandIndex {
            get { return _bandIndex; }
            set { _bandIndex = value; }
        }


        [XmlIgnore]
        public bool IsInherited {
            get { return !string.IsNullOrEmpty(this.Inherits); }
        }

        [XmlIgnore]
        public bool IsRootBand {
            get { return (ParentBandIndex == -1); }
        }

        [XmlIgnore]
        public string TOBandName {
            get { return ParentBandIndex != -1 ? Key : string.Empty; }
        }

        [XmlIgnore]
        public string ParentPath {
            get {
                var path = "Parent.Parent";
                for (int i = -1; i < _parentBandIndex; i++) {
                    path += ".Parent.Parent";
                }
                return path;
            }
            
        }

        [XmlIgnore]
        public int ParentBandIndex {
            get { return _parentBandIndex; }
            set { _parentBandIndex = value; }
        }

        private List<BandInfoType> _childBands = new List<BandInfoType>();

        [XmlIgnore]
        public string TableName {
            get { return Table.PTName; }
        }

        [XmlIgnore]
        public Control Table {
            get { return _table; }
            set { _table = value; }
        }

        private InputModeBehavior _inputModeBehavior;

        [XmlIgnore]
        public InputModeBehavior InputModeBehavior {
            get {
                if (_inputModeBehavior == null) {
                    _inputModeBehavior = new InputModeBehavior(Behavior);
                }
                return _inputModeBehavior;
            }
        }

        [XmlIgnore]
        public List<ColumnInfoType> ColumnInfos {
            get {
                if (_columnInfos == null) {
                    _columnInfos = new List<ColumnInfoType>();
                }

                return _columnInfos;
            }
            set { _columnInfos = value; }
        }

        [XmlIgnore]
        public List<BandInfoType> ChildBands {
            get { return _childBands; }
            set { _childBands = value; }
        }


        [XmlIgnore]
        public bool HasChildBands {
            get { return _childBands.Count > 0; }
        }

        [XmlIgnore]
        public string MemberVariableName {
            get { return HelperMethods.FormatVariableName(Key); }
        }

        /// <summary>
        /// Gets range of column names (Table+Band+ColumnName) separated by comma
        /// </summary>
        [XmlIgnore]
        public string ColumnRange {
            get {
                string colInfos = string.Empty;

                foreach (ColumnInfoType colInfo in ColumnInfos) {
                    colInfos += string.Format("{0}{1}{2},", TableName, Key, colInfo.Key);
                    if (colInfo.SubControlCount > 0) {
                        colInfos += string.Format("{0}{1}{2},", TableName, Key, colInfo.Key + "_ControlName");
                    }
                }
                colInfos += string.Format("{0}{1}{2},", TableName, Key, "ColumnOid");
                //Hier werden Dummy Spalten für ChildBands hinzugefügt
                foreach (BandInfoType childBandInfo in ChildBands) {
                    colInfos += string.Format("{0}{1}{2},", TableName, Key, childBandInfo.Key);
                }

                colInfos = colInfos.Remove(colInfos.Length - 1, 1);

                return colInfos;
            }
        }

        /// <summary>
        /// Gets range of column names separated by comma
        /// </summary>
        [XmlIgnore]
        public string ColumnNames {
            get {
                string colInfos = string.Empty;

                foreach (ColumnInfoType colInfo in ColumnInfos) {
                    colInfos += string.Format("{0},", colInfo.Key);
                    if(colInfo.SubControlCount > 0){
                        colInfos += string.Format("{0},", colInfo.Key+"_ControlName");                        
                    }

                    if (colInfo.ColSpan > 1) {
                        colInfos += string.Format("{0}_Span,", colInfo.Key);
                    }
                }
                colInfos = colInfos.Length > 0 ? colInfos.Remove(colInfos.Length - 1, 1) : colInfos;

                return colInfos;
            }
        }

        /// <summary>
        /// Gets range of sub controls names for each column, separated by comma
        /// </summary>
        [XmlIgnore]
        public string SubControlsEnumTemplate {
            get {
                string colInfos = string.Empty;

                foreach (ColumnInfoType colInfo in ColumnInfos) {
                    if(colInfo.SubControlCount > 0){
                        colInfos += "public enum " + TableName + this.TOBandName + colInfo.Key + "SubControls { ";
                        for (int cnt = 0; cnt < colInfo.SubControlCount; cnt++)
                        {
                            colInfos += string.Format("{0},", colInfo.Key + "_" + cnt.ToString());    
                        }
                        colInfos = colInfos.Length > 0 ? colInfos.Remove(colInfos.Length - 1, 1) : colInfos;
                        colInfos += " }\r\n";
                    }
                }

                return colInfos;
            }
        }

        [XmlIgnore]
        public bool HasSubControl {
            get { return false; }
        }
    }




    //TODO: TableColumnInfo muss refactored werden: ColumnInfos sollten typisiert werden, Decimal, Amount, Date usw..... 
    //TODO: AmountTableColumnInfo, DecimalColumnInfo usw...
    [SupportedProperties(
     "*Key, Hidden, Editable, Width, Caption, Behavior, MaskInput, FormatInput, MaxWidth, MinWidth, DefaultImageKey, Band, Type,Style, CellMultiLine,CaptionHAlign, TextHAlign, TextVAlign, ImageHAlign, ImageVAlign, PictureLibrary, Sortable, Nullable, MinValue, MaxValue, MaxLength, NullText, SubControlCount, EnableChangeTracking, TableConfigCaption, TableConfigMoveable, TableConfigHideable, TableConfigAllowed, MaxVisibleDropDownItems, ColSpan, Style, ColumnMode, Managed, DefaultCellValue"
        )]
    public partial class ColumnInfoType {
        private BandInfoType _bandInfo = null;

        public Table Parent {
            get { return _parent; }
        }

        private readonly Table _parent;

        public EditorUIFactory EditorUiFactory {
            get { return _editorUiFactory; }
        }

        private readonly EditorUIFactory _editorUiFactory;

        public ColumnInfoType(Table parent) : this() {
            _parent = parent;
            _editorUiFactory = new EditorUIFactory(this);
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ResolvedPictureLibrary {
            get {
                if (string.IsNullOrEmpty(pictureLibraryField)) {
                    return "Keine";
                }
                return this.pictureLibraryField;
            }
        }


        [XmlIgnore]
        public BandInfoType BandInfo {
            get { return _bandInfo; }
            set { _bandInfo = value; }
        }

        [XmlIgnore]
        public string ColumnCaption {
            get { return Caption; }
        }

        [XmlIgnore]
        public bool HasDefaultCellValue {
            get { return !string.IsNullOrEmpty(DefaultCellValue); }
        }

       

        [XmlIgnore]
        public string ColumnCaption4TableConfig {
            get {
                return !string.IsNullOrEmpty(TableConfigCaption) ? TableConfigCaption : !string.IsNullOrEmpty(ColumnCaption) ? ColumnCaption : Key;
            }
        }



        [XmlIgnore]
        public virtual bool IsNullTextDefined {
            get { return nullTextField != null; }
        }

        [XmlIgnore]
        public string BandInfoString {
            get {
                return
                    string.Format("{0} - {1} - {2} - {3}", _bandInfo.Key, _bandInfo.ParentBandIndex, _bandInfo.TOBandName,
                                  _bandInfo.TableName);
            }
        }

        private InputModeBehavior _inputModeBehavior;

        [XmlIgnore]
        public InputModeBehavior InputModeBehavior {
            get {
                if (_inputModeBehavior == null) {
                    _inputModeBehavior = new InputModeBehavior(Behavior);
                }
                return _inputModeBehavior;
            }
        }

        [XmlIgnore]
        public bool IsVisible {
            get { return Hidden == ColumnInfoTypeHidden.@false; }
        }

        [XmlIgnore]
        public bool HasWidth {
            get { return Width > 0; }
        }

        [XmlIgnore]
        public bool HasMinWidth {
            get { return MinWidth > 0; }
        }

        [XmlIgnore]
        public bool HasMaxVisibleDropDownItems {
            get { return MaxVisibleDropDownItems > 0; }
        }
        
        [XmlIgnore]
        public bool HasColSpan {
            get { return ColSpan > 1; }
        }

        [XmlIgnore]
        public bool HasMaxWidth {
            get { return MaxWidth > 0; }
        }

        [XmlIgnore]
        public bool HasMaxLength {
            get { return MaxLength > 0; }
        }

        [XmlIgnore]
        public bool HasMaxValue {
            get { return !string.IsNullOrEmpty(MaxValue); }
        }

        [XmlIgnore]
        public bool HasMinValue {
            get { return !string.IsNullOrEmpty(MinValue); }
        }

        [XmlIgnore]
        public string MinValueResolved {
            get { return GetValueResolved(MinValue); }
        }

        [XmlIgnore]
        public string MaxValueResolved {
            get { return GetValueResolved(MaxValue); }
        }

        [XmlIgnore]
        public bool IsCellMultiLine {
            get { return cellMultiLineField==ColumnInfoTypeCellMultiLine.True || cellMultiLineField==ColumnInfoTypeCellMultiLine.Default; }
        }

        

        private string GetValueResolved(string value) {
            if (Type == ColumnInfoTypeType.Date) {
                return string.Format("new DateTime({0})", DateTime.Parse(value).Ticks);
            } else if (Type == ColumnInfoTypeType.Time) {
                return
                    string.Format("new TimeData(null,{0},{1},{2})", DateTime.Parse(value).Hour, DateTime.Parse(value).Minute,
                                  DateTime.Parse(value).Second);
            } else {
                return value;
            }
        }

        private static int _guidCounter=0;

        public string FixedGuid {
            get {
                //3-Stellen variabel.
                return string.Format("new Guid(\"00000000-0000-0000-0000-000000000{0:000}\")", ++_guidCounter);
            }
        }

        [XmlIgnore]
        public bool IsComboDropDownEditor {
            get { return (Type == ColumnInfoTypeType.Combobox) || (IsDropdownbox); }
        }

        [XmlIgnore]
        public bool IsCombobox {
            get { return (Type == ColumnInfoTypeType.Combobox) || (IsDropdownbox); }
        }

        [XmlIgnore]
        public bool IsRealCombobox {
            get { return Type == ColumnInfoTypeType.Combobox; }
        }

        [XmlIgnore]
        public bool IsCheckbox {
            get { return Type == ColumnInfoTypeType.Checkbox; }
        }

        [XmlIgnore]
        public bool IsButton {
            get { return Type == ColumnInfoTypeType.Button; }
        }

        [XmlIgnore]
        public bool IsAmountbox {
            get { return Type == ColumnInfoTypeType.AmountTextbox; }
        }

        [XmlIgnore]
        public bool IsImageTextEditor {
            get { return Type == ColumnInfoTypeType.ImageTextEditor; }
        }

        

        [XmlIgnore]
        public bool IsPercent {
            get { return Type == ColumnInfoTypeType.Percent; }
        }

        [XmlIgnore]
        public bool IsReal {
            get { return Type == ColumnInfoTypeType.REAL; }
        }

        [XmlIgnore]
        public bool IsUrl {
            get { return Type == ColumnInfoTypeType.URL; }
        }

        [XmlIgnore]
        public bool IsInteger {
            get { return Type == ColumnInfoTypeType.Integer; }
        }

        [XmlIgnore]
        public bool IsNumeric {
            get { return Type == ColumnInfoTypeType.Numeric; }
        }

        [XmlIgnore]
        public bool IsDouble {
            get { return Type == ColumnInfoTypeType.Double; }
        }

        [XmlIgnore]
        public bool IsIcon {
            get { return Type == ColumnInfoTypeType.Icon; }
        }

        [XmlIgnore]
        public bool IsPZN {
            get { return Type == ColumnInfoTypeType.PZN; }
        }

        [XmlIgnore]
        public bool IsBORef {
            get { return Type == ColumnInfoTypeType.BORef; }
        }


        [XmlIgnore]
        public bool IsBrowsableTextbox {
            get { return Type == ColumnInfoTypeType.BrowsableTextbox; }
        }

        [XmlIgnore]
        public bool IsMaskedTextbox {
            get { return Type == ColumnInfoTypeType.MaskedTextbox; }
        }
        
        [XmlIgnore]
        public bool IsAutoSuggestion {
            get { return Type == ColumnInfoTypeType.AutoSuggestion; }
        }

        [XmlIgnore]
        public bool IsDropdownbox {
            get { return Type == ColumnInfoTypeType.Dropdownbox; }
        }

        [XmlIgnore]
        public bool IsTimeControl {
            get { return Type == ColumnInfoTypeType.Time; }
        }

        [XmlIgnore]
        public bool IsDateControl {
            get { return Type == ColumnInfoTypeType.Date; }
        }

        [XmlIgnore]
        public bool IsFormattedTextbox {
            get { return Type == ColumnInfoTypeType.FormattedTextbox || Type == ColumnInfoTypeType.BORef; }
        }


        [XmlIgnore]
        public bool IsFormattedLabel {
            get { return Type == ColumnInfoTypeType.FormattedLabel; }
        }

        [XmlIgnore]
        public bool IsPhoneControl {
            get { return Type == ColumnInfoTypeType.Phone; }
        }

        [XmlIgnore]
        public bool IsDefaultTextHAlign {
            get { return TextHAlign == TextAlignment.Left; }
        }

        [XmlIgnore]
        public bool IsDefaultTextVAlign {
            get { return TextVAlign == TextVAlignment.Top; }
        }

        [XmlIgnore]
        public bool IsDefaultCaptionHAlign {
            get { return CaptionHAlign == TextAlignment.Default; }
        }


        [XmlIgnore]
        public string MemberVariableName {
            get { return HelperMethods.FormatVariableName(Key); }
        }

        [XmlIgnore]
        public bool IsStringValueColumn {
            get { return DataType.Equals("string"); }
        }

        [XmlIgnore]
        public bool IsComplexTOType {
            get { return (IsTimeControl || IsPhoneControl || IsImageTextEditor); }
        }

        [XmlIgnore]
        public bool MustCopyValues {
            get { return (IsTimeControl || IsPhoneControl || IsAmountbox || IsImageTextEditor); }
        }

        /// <summary>
        /// Gibt anhand des Spaltentypen den Namen des Styletypen zurück. 
        /// Falls keine Spaltentyp definiert ist wird NULL zurück gegeben.
        /// </summary>
        [XmlIgnore]
        public string StyleTypeName {
            get {

                switch (Type) {
                    case ColumnInfoTypeType.None:
                        return null;
                    case ColumnInfoTypeType.BORef:
                        return ColumnInfoTypeType.FormattedTextbox.ToString();
                    default:
                        return Type.ToString();
                }
            }
        }


        [XmlIgnore]
        public string DataType {
            get {
                string nullable = nullableField==ColumnInfoTypeNullable.@false ? "" : "?";

                switch (Type) {
                    case ColumnInfoTypeType.None:
                        return "string";
                    case ColumnInfoTypeType.Combobox:
                        return "string"; 
                    case ColumnInfoTypeType.Dropdownbox:
                        if (nullableField==ColumnInfoTypeNullable.@true)
                            return "Pharmatechnik.Apotheke.XTplus.Framework.Util.BORef" + nullable;
                        else
                            return "Pharmatechnik.Apotheke.XTplus.Framework.Util.BORef";
                    case ColumnInfoTypeType.Checkbox:
                        return "bool";
                    case ColumnInfoTypeType.Button:
                        return "System.Guid?";
                    case ColumnInfoTypeType.AmountTextbox:
                        return "Pharmatechnik.Apotheke.XTplus.Framework.Amounts.IWFL.AmountTO";
                        //case ColumnInfoTypeType.Percent:
                        //return "decimal?";
                    case ColumnInfoTypeType.ImageTextEditor:
                        return "Pharmatechnik.Apotheke.XTplus.Framework.Core.IWFL.Data.ImageTextEditorDataUI";
                    case ColumnInfoTypeType.BrowsableTextbox:
                        return "string";
                    case ColumnInfoTypeType.Icon:
                        return "string";
                    case ColumnInfoTypeType.Date:
                        return "System.DateTime" + nullable;
                    case ColumnInfoTypeType.Time:
                        return "TimeData";
                    case ColumnInfoTypeType.Numeric:
                        return "long" + nullable;
                    case ColumnInfoTypeType.Integer:
                        return "int" + nullable;
                    case ColumnInfoTypeType.REAL:
                        return "decimal" + nullable;
                    case ColumnInfoTypeType.Percent:
                    case ColumnInfoTypeType.Double:
                        return "double" + nullable;
                    case ColumnInfoTypeType.Phone:
                        return "PhoneData";
                    case ColumnInfoTypeType.MaskedTextbox:
                    case ColumnInfoTypeType.FormattedLabel:
                    case ColumnInfoTypeType.FormattedTextbox:
                        return "string";
                    case ColumnInfoTypeType.BORef:
                        return "BORef";
                    default:
                        return "string";
                }
            }
        }

        [XmlIgnore]
        public string CaptionTextHAlign {
            get {
                if (!IsDefaultCaptionHAlign) {
                    return CaptionHAlign.ToString();
                } else {
                    switch (Type) {
                        case ColumnInfoTypeType.None:
                        case ColumnInfoTypeType.BrowsableTextbox:
                        case ColumnInfoTypeType.Date:
                        case ColumnInfoTypeType.Time:
                        case ColumnInfoTypeType.Combobox:
                        case ColumnInfoTypeType.Dropdownbox:
                        case ColumnInfoTypeType.Phone:
                        case ColumnInfoTypeType.MaskedTextbox:
                        case ColumnInfoTypeType.BORef:
                            return "Left";

                        case ColumnInfoTypeType.Checkbox:
                        case ColumnInfoTypeType.Button:
                        case ColumnInfoTypeType.Icon:
                            return "Center";

                        case ColumnInfoTypeType.AmountTextbox:
                        case ColumnInfoTypeType.Numeric:
                        case ColumnInfoTypeType.Integer:
                        case ColumnInfoTypeType.REAL:
                        case ColumnInfoTypeType.Percent:
                        case ColumnInfoTypeType.Double:
                        case ColumnInfoTypeType.ImageTextEditor:
                            return "Right";
                        default:
                            return "Left";
                    }
                }
            }
        }

        [XmlIgnore]
        public string InitValue {
            get {
                string init = "this";
                switch (Type) {
                    case ColumnInfoTypeType.Icon:
                        init += "," + @"""" + DefaultImageKey + @"""";
                        break;
                    case ColumnInfoTypeType.Checkbox:
                        init += "," + "false";
                        break;
                }

                return init;
            }
        }

        [XmlIgnore]
        public bool HasSubControl {
            get { return SubControlCount > 0; }
        }

        [XmlIgnore]
        public string SubControlDeclaration {
            get {
                //protected Pharmatechnik.Apotheke.XTplus.Framework.Core.GUI.Controls.PTComboBoxUI $gui.PTName$$it.BandInfo.TOBandName$$it.Key$ComboBox;
                string str = "";
                if (IsCombobox)
                {
                    string uiEditor = "";
                    string typeName = "";

                    uiEditor = "PTComboBoxUI";
                    typeName = "ComboBox";

                    string controlDef =
                        "protected Pharmatechnik.Apotheke.XTplus.Framework.Core.GUI.Controls." + uiEditor + " " +
                        Parent.PTName + BandInfo.TOBandName + Key + typeName + "_";
                    
                    if (SubControlCount > 0)
                    {
                        for (int cnt = 0; cnt < SubControlCount; cnt++)
                        {
                            str += controlDef + cnt.ToString() + ";\r\n";
                        }
                    }
                }
                return str;
            }
        }

         [XmlIgnore]
        public string SubControlDispose {
            get {
                string str = "";
                if (IsCombobox)
                {
                    string typeName = "";

                    typeName = "ComboBox";

                    string controlDef = Parent.PTName + BandInfo.TOBandName + Key + typeName + "_";
                    
                    if (SubControlCount > 0)
                    {
                        for (int cnt = 0; cnt < SubControlCount; cnt++)
                        {
                            str += "if(" + controlDef+cnt + "!=null) " + controlDef + cnt + ".Dispose();\r\n";
                        }
                    }
                }
                return str;
            }
        }



        [XmlIgnore]
        public string SubControlDefinition {
            get {
                //$gridName$$it.BandInfo.TOBandName$$it.Key$ComboBox = new Pharmatechnik.Apotheke.XTplus.Framework.Core.GUI.Controls.PTComboBoxUI();
                //Infragistics.Win.UltraWinGrid.UltraGridColumn grdDemoGridDemoGridDropdownbox_ControlName = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Dropdownbox_ControlName");
                string str = "";
                if (IsCombobox) {
                    string uiEditor = "";
                    string typeName = "";

                    uiEditor = "PTComboBoxUI()";
                    typeName = "ComboBox";

                    string controlDef_Pre = Parent.PTName + BandInfo.TOBandName + Key + typeName + "_";
                    string controlDef_Post = " = new Pharmatechnik.Apotheke.XTplus.Framework.Core.GUI.Controls." + uiEditor;
                    string controlColumnDef = Parent.PTName + BandInfo.Key + Key + "_ControlName";

                    
                    if (SubControlCount > 0) {
                        for (int cnt = 0; cnt < SubControlCount; cnt++) {
                            str += controlDef_Pre + cnt.ToString() + controlDef_Post + ";\r\n";
                        }
                    }
                    str += "Infragistics.Win.UltraWinGrid.UltraGridColumn " + controlColumnDef + " = new Infragistics.Win.UltraWinGrid.UltraGridColumn(\"" + Key + "_ControlName\");\r\n";
                }
                return str;
            }
        }

        [XmlIgnore]
        public string SubControlInitialization {
            get {
                //$GuiElement.PTName$$it.Band$$it.Key$_ControlName.Header.VisiblePosition = $i$;  - geht nicht
                //$GuiElement.PTName$$it.Band$$it.Key$_ControlName.Hidden = true; 
                //$GuiElement.PTName$$it.Band$$it.Key$_ControlName.Key = "$it.Key$_ControlName";
                //$GuiElement.PTName$$it.Band$$it.Key$_ControlName.Nullable = Infragistics.Win.UltraWinGrid.Nullable.EmptyString;
                string str = "";
                if (IsCombobox)
                {
                    string controlDef = Parent.PTName + BandInfo.Key + Key + "_ControlName";
                    
                    if (SubControlCount > 0)
                    {
                        str = controlDef + ".Hidden = true;\r\n";
                        str += controlDef + ".Key = \"" + Key + "_ControlName\";\r\n";
                        str += controlDef + ".Nullable = Infragistics.Win.UltraWinGrid.Nullable.EmptyString;\r\n";
                    }
                }
                return str;
            }
        }

        [XmlIgnore]
        public string SubControlInitialization2 {
            get {
                //$gui.PTName$$it.BandInfo.TOBandName$$it.Key$ComboBox.Name = "$it.Key$"; 
                //$gui.PTName$$it.BandInfo.TOBandName$$it.Key$ComboBox.ValueMember = "Oid"; 
                //$gui.PTName$$it.BandInfo.TOBandName$$it.Key$ComboBox.DisplayMember = "$it.Key$"; 
                //$gui.PTName$$it.BandInfo.TOBandName$$it.Key$ComboBox.DropDownWidth = $gui.PTNAme$.DisplayLayout.Bands[$it.BandInfo.BandIndex$].Columns["$it.Key$"].Width;
                //$gui.PTName$$it.BandInfo.TOBandName$$it.Key$ComboBox.DisplayLayout.Override.DefaultColWidth = $gui.PTNAme$.DisplayLayout.Bands[$it.BandInfo.BandIndex$].Columns["$it.Key$"].Width;
                //$gui.PTName$$it.BandInfo.TOBandName$$it.Key$ComboBox.DisplayLayout.Bands[0].ColHeadersVisible = false; 
                //$gridInstance$.DisplayLayout.Bands[$it.BandInfo.BandIndex$].Columns["$it.Key$"].ValueList = $gui.PTName$$it.BandInfo.TOBandName$$it.Key$ComboBox;


                //grdDemoGridDropdownbox_1ComboBox.Name = "Dropdownbox_1";
                //grdDemoGridDropdownbox_1ComboBox.ValueMember = "Oid";
                //grdDemoGridDropdownbox_1ComboBox.DisplayMember = "Dropdownbox";
                //grdDemoGridDropdownbox_1ComboBox.DropDownWidth = grdDemoGrid.DisplayLayout.Bands[0].Columns["Dropdownbox"].Width;
                //grdDemoGridDropdownbox_1ComboBox.DisplayLayout.Override.DefaultColWidth = grdDemoGrid.DisplayLayout.Bands[0].Columns["Dropdownbox"].Width;
                //grdDemoGridDropdownbox_1ComboBox.DisplayLayout.Bands[0].ColHeadersVisible = false;
                //grdDemoGrid.GridColumnsControls.Add(new PTGridColumnsControl("Dropdownbox_1", "Dropdownbox", "DemoGrid", grdDemoGridDropdownbox_1ComboBox));

                string str = "";
                if (IsCombobox) {
                    string typeName = "";

                    typeName = "ComboBox";
                    string controlDef = Parent.PTName + BandInfo.TOBandName + Key + typeName + "_";

                    if (SubControlCount > 0) {
                        for (int cnt = 0; cnt < SubControlCount; cnt++) {
                            str += controlDef + cnt.ToString() + ".Name = \"" + Key + "_" + cnt.ToString() + "\";\r\n";
                            str += controlDef + cnt.ToString() + ".ValueMember = \"Oid\";\r\n";
                            str += controlDef + cnt.ToString() + ".DisplayMember = \"" + Key + "\";\r\n";
                            str += controlDef + cnt.ToString() + ".DropDownWidth = " + Parent.PTName + ".DisplayLayout.Bands[" + BandInfo.BandIndex + "].Columns[\"" + Key + "\"].Width;\r\n";
                            str += controlDef + cnt.ToString() + ".DisplayLayout.Override.DefaultColWidth = " + Parent.PTName + ".DisplayLayout.Bands[" + BandInfo.BandIndex + "].Columns[\"" + Key + "\"].Width;\r\n";
                            str += controlDef + cnt.ToString() + ".DisplayLayout.Bands[" + BandInfo.BandIndex + "].ColHeadersVisible = false;\r\n";
                            str += Parent.PTName + ".GridColumnsControls.Add(new PTGridColumnsControl(\"" + Key + "_" + cnt.ToString() + "\", \"" + Key + "\", \"" + BandInfo.Key + "\", " + controlDef + cnt.ToString() + "));\r\n";
                        }
                    }
                }
                return str;
            }
        }
        
        [XmlIgnore]
        public string SubControlDataDeclaration {
            get {
                //private ComboBoxUIData<$gui.Name$$it.BandInfo.TOBandName$$it.Key$ComboboxTO> _$gui.PTName$$it.BandInfo.TOBandName$$it.Key$Combobox_X;

                string str = "";
                if (IsCombobox)
                {
                    string controlTODef = Parent.Name + BandInfo.TOBandName + Key;
                    string controlDef = Parent.PTName + BandInfo.TOBandName + Key;
                    
                    if (SubControlCount > 0)
                    {
                        for (int cnt = 0; cnt < SubControlCount; cnt++)
                        {
                            str += "private ComboBoxUIData<" + controlTODef + "ComboboxTO> _" + controlDef + "Combobox_" + cnt.ToString() + ";\r\n";
                        }
                    }
                }
                return str;
            }
        }

        [XmlIgnore]
        public string SubControlDataDefinition {
            get {
                //_$gui.PTName$$it.BandInfo.TOBandName$$it.Key$Combobox_1 = new ComboBoxUIData<$gui.Name$$it.BandInfo.TOBandName$$it.Key$ComboboxTO>(this);
                //_$gui.MemberVariableName$.GridControls.Add(new GridControlsData("$it.Key$_1", "$it.Key$", "$it.Band$",  _$gui.PTName$$it.BandInfo.TOBandName$$it.Key$Combobox_1));
                //_$gui.MemberVariableName$.ResetModified();

                string str = "";
                if (IsCombobox) {
                    string controlTODef = Parent.Name + BandInfo.TOBandName + Key;
                    string controlDef = "_" + Parent.PTName + BandInfo.TOBandName + Key + "Combobox_";

                    if (SubControlCount > 0) {
                        for (int cnt = 0; cnt < SubControlCount; cnt++) {
                            str += controlDef + cnt.ToString() + " = new ComboBoxUIData<" + controlTODef + "ComboboxTO>(this);\r\n";
                            str += "_" + Parent.MemberVariableName + ".GridControls.Add( new GridControlsData(" + "_" + Parent.MemberVariableName + ", " + FixedGuid + ",\"" + Key + "_" + cnt.ToString() + "\", \"" + Key + "\", \"" + 
                                   Band + "\", " + controlDef + cnt.ToString() + "));\r\n";
                            str += "_" + Parent.MemberVariableName + ".ResetModified();\r\n";
                        }
                    }
                }
                return str;
            }
        }

        public override int GetHashCode() {
            return Key.GetHashCode();
        }

        /// <summary>
        /// Compares Keys
        /// </summary>
        /// <param name="obj">object to compare with</param>
        /// <returns>true if equal else false</returns>
        public override bool Equals(object obj) {
            return Key.Equals(((ColumnInfoType) obj).Key, StringComparison.CurrentCultureIgnoreCase);
        }

        [XmlIgnore]
        public string TOBandName {
            get { return null; }
        }
    }

    [SupportedProperties("*Key, *Type, Caption, IsTooltipColumn, IsIdColumn, IsTaskTitleColumn, Legend")]
    public partial class ChartColumnType {
       

        public Chart Parent {
            get { return _parent; }
        }

        private readonly Chart _parent;

        [XmlIgnore]
        public bool IsDataType {
            get {
                return Type != "DateTime";
            }
        }

       
        public ChartColumnType(Chart parent) {
            _parent = parent;
        }

        [XmlIgnore]
        public string ElementType {
            get { return Type.Replace("?",""); }
        }


        [XmlIgnore]
        public string ColumnCaption {
            get { return Caption ?? Key; }
        }

        
        private static int _guidCounter = 0;

        public string FixedGuid {
            get {
                //3-Stellen variabel.
                return string.Format("new Guid(\"00000000-0000-0000-0000-000000000{0:000}\")", ++_guidCounter);
            }
        }


        [XmlIgnore]
        public string MemberVariableName {
            get { return HelperMethods.FormatVariableName(Key); }
        }

        

        public override int GetHashCode() {
            return Key.GetHashCode();
        }

        /// <summary>
        /// Compares Keys
        /// </summary>
        /// <param name="obj">object to compare with</param>
        /// <returns>true if equal else false</returns>
        public override bool Equals(object obj) {
            return Key.Equals(((ColumnInfoType)obj).Key, StringComparison.CurrentCultureIgnoreCase);
        }
    }

    public class EditorUIFactory {
        private readonly ColumnInfoType _currentCol;
        private readonly List<Event> _events = new List<Event>();

        public EditorUIFactory(ColumnInfoType currentCol) {
            _currentCol = currentCol;
        }

        public EditorUIControl Create() {
            EditorUIControl editor =
                GuiModelFactory.CreateEditor(_currentCol.Type, _currentCol.Parent.RootContainer, _currentCol.Parent, _currentCol);

            if (editor == null) {
                return editor;
            }

            editor.Events = _events.ToArray();
            return editor;
        }

        public void AddEvent(string name, string methodName, CallType methodCallType) {
            _events.Add(
                new Event(name, methodName, methodCallType,
                          new EventDelegateInfo(name, "System.EventHandler", "System.Void",
                                                new string[] {"System.Object", "System.EventArgs"})));
        }
    }

    [SupportedProperties("Text")]
    public partial class DynamicButton {
        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "btn"; }
        }

        [XmlIgnore]
        public bool IsBindable {
            get { return true; }
        }

      
    }

    [SupportedProperties("Text, DefaultButton, ImageLibrary, ImageKey, ImageSize")]
    public partial class Button {
        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "btn"; }
        }

        [XmlIgnore]
        public bool IsBindable {
            get { return false; }
        }

        [XmlIgnore]
        public bool HasImageSize {
            get { return imageSizeField > 0; }
        }

        [XmlIgnore]
        public bool IsCancelButton
        {
            get { return DefaultButton == ButtonDefaultButton.Cancel || DefaultButton == ButtonDefaultButton.AcceptCancel || DefaultButton == ButtonDefaultButton.OnlyCancel;  }
        }

        [XmlIgnore]
        public bool IsAcceptButton
        {
            get { return DefaultButton == ButtonDefaultButton.Accept || DefaultButton==ButtonDefaultButton.AcceptCancel; }
        }
    }

    [SupportedProperties("InfoButtonVisible, MaxLength, MaxDropDownItems")]
    [SupportedGuiEvents("Leave, ButtonInfoClick, SelectionChanged")]
    public partial class Combobox {
        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "cmb"; }
        }

        [XmlIgnore]
        public override bool AutoWriteTOValue {
            get { return true; }
        }

        public virtual bool IsCombo {
            get { return true; }
        }

        [XmlIgnore]
        public bool HasMaxLength {
            get { return MaxLength > -1; }
        }

        [XmlIgnore]
        public bool HasMaxDropDownItems {
            get { return MaxDropDownItems > 0; }
        }

        [XmlIgnore]
        public string Text {
            get { return null; }
        }

    }


    public partial class Dropdownbox {
        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "ddb"; }
        }

        [XmlIgnore]
        public override bool AutoWriteTOValue {
            get { return true; }
        }


        public override bool IsCombo {
            get { return false; }
        }
    }

    [SupportedProperties("ButtonText, InfoButtonVisible, BrowseButtonVisible")]
    [SupportedGuiEvents("EditorButtonClick, ButtonClick, ButtonInfoClick")]
    public partial class BrowsableTextbox {
        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "btb"; }
        }
    }

    [SupportedProperties("*Text, Checked")]
    [SupportedGuiEvents("CheckedValueChanged,CheckedChanged")]
    public partial class Checkbox {
        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "cbx"; }
        }

        [XmlIgnore]
        public override bool AutoWriteTOValue {
            get { return true; }
        }
    }


    [SupportedProperties("*Text, *GroupName")]
    public partial class Radiobutton {

        [XmlIgnore]
        public override bool ApplyBinding
        {
            get { return DataBinding == BooleanControlState.@true; }
        }



        [XmlIgnore]
        public override bool AutoWriteTOValue {
            get { return true; }
        }

        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "rbn"; }
        }
    }

    public partial class RadiobuttonGroup {
        private string _keyList;

        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "rbn"; }
        }

        public string KeysString {
            get {
                if (_keyList == null) {
                    for (int iRadioButtonIndex = 0; iRadioButtonIndex < Radiobuttons.Length; iRadioButtonIndex++) {
                        _keyList += Radiobuttons[iRadioButtonIndex].Key + ", ";
                    }
                }
                if (_keyList.EndsWith(", ", StringComparison.CurrentCultureIgnoreCase)) {
                    _keyList = _keyList.Substring(0, _keyList.Length - 2);
                }
                return _keyList;
            }
        }

        [XmlIgnore]
        public override bool AutoWriteTOValue {
            get { return true; }
        }
    }


    public interface IVirtualGuiElement {
        IList<GuiElement> IternalGuiElements { get; }
    }


    public interface INonVisualGuiElement {}

    public partial class RadiobuttonContainer : IContainer, IVirtualGuiElement, IGuiElementParent {
        private string _keyList;


        public RadiobuttonContainer() {}

        public RadiobuttonContainer(string name) {
            Name = name;
            HotKeys = new GuiElementHotKeys();
        }


        public string KeysString {
            get {
                if (_keyList == null) {
                    for (int iRadioButtonIndex = 0; iRadioButtonIndex < Radiobuttons.Length; iRadioButtonIndex++) {
                        _keyList += Radiobuttons[iRadioButtonIndex].Key + ", ";
                    }
                }
                if (_keyList.EndsWith(", ", StringComparison.CurrentCultureIgnoreCase)) {
                    _keyList = _keyList.Substring(0, _keyList.Length - 2);
                }
                return _keyList;
            }
        }

        private Layout _layout;

        public Layout Layout {
            get { return _layout; }
            set { _layout = value; }
        }

        public void AddGuiElement(GuiElement element) {
            List<Radiobutton> tmpRadiobuttons = new List<Radiobutton>(Radiobuttons);
            if (element is Radiobutton) {
                tmpRadiobuttons.Add((Radiobutton) element);
            }
            Radiobuttons = tmpRadiobuttons.ToArray();
        }

        public void RemoveGuiElement(GuiElement element) {
            List<Radiobutton> tmpRadiobuttons = new List<Radiobutton>(Radiobuttons);
            if (element is Radiobutton) {
                tmpRadiobuttons.Remove((Radiobutton) element);
            }
            Radiobuttons = tmpRadiobuttons.ToArray();
        }

        [XmlIgnore]
        public GuiElement[] Controls {
            get { return Radiobuttons; }
            set {
                List<Radiobutton> tmpRadiobuttons = new List<Radiobutton>();
                foreach (GuiElement element in value) {
                    if (element is Radiobutton) {
                        tmpRadiobuttons.Add((Radiobutton) element);
                    }
                }

                Radiobuttons = tmpRadiobuttons.ToArray();
            }
        }

        public IList<GuiElement> IternalGuiElements {
            get { return Controls; }
        }


        private List<GuiElement> _allGuiElements = null;

        public GuiElement[] GetAllGuiElements() {
            if (_allGuiElements == null) {
                _allGuiElements = new List<GuiElement>();
                foreach (Radiobutton btnRadio in Radiobuttons) {
                    _allGuiElements.Add(btnRadio);
                }
            }
            return _allGuiElements.ToArray();
        }

        [XmlIgnore]
        public bool EnabledIfModified {
            get { return false; }
        }

        [XmlIgnore]
        public bool HasToolTip {
            get { return false; }
        }
    }


    [SupportedGuiEvents("AfterNodeSelect, BeforeNodeSelect, AfterDataNodesCollectionPopulated")]
    [SupportedProperties("EnableDragDrop, ImageLibrary")]
    public partial class Tree {
        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "trv"; }
        }

        public override bool AutoWriteTOValue {
            get { return true; }
        }
    }

    [SupportedProperties("*Key, *Text, Hidden")]
    public partial class NodeInfo {}

    [SupportedProperties("*Text, Sorted")]
    public partial class Listbox {
        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "lbx"; }
        }
    }

    [SupportedProperties("Value,MaskInput, ReadOnly, MinValue, MaxValue, Nullable, NullText")]
    public partial class NumericTextbox {
        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "ntb"; }
        }

        [XmlIgnore]
        public virtual bool IsNumeric {
            get { return true; }
        }

      
        [XmlIgnore]
        public string MaskInputType {
            get {
                if (IsIntegerMask) {
                    return "Integer";
                } else {
                    return "Decimal";
                }
            }
        }

       
        [XmlIgnore]
        public string MaskInputTOType {
            get {
                string nullable = nullableField ? "?" : "";

                if (IsIntegerMask) {
                    return "int" + nullable;
                } else {
                    return "decimal" + nullable;
                }
            }
        }

        [XmlIgnore]
        public string MaskInputTOValue {
            get {
                if (nullableField) {
                    return "null";
                } else {
                    return "0";
                }
            }
        }

        [XmlIgnore]
        public bool IsValueAvailable {
            get { return !string.IsNullOrEmpty(Value); }
        }

        private bool IsIntegerMask {
            get {

                if (Style == "PZN") {
                    return true;
                }

                //evtl. könnte man das durch eine RegEx ersetzen
                if (((MaskInput.IndexOf("Double", StringComparison.OrdinalIgnoreCase) > -1) ||
                    ((MaskInput.IndexOf("Decimal", StringComparison.OrdinalIgnoreCase) > -1))) ||
                    ((MaskInput.IndexOf("n", StringComparison.OrdinalIgnoreCase) > -1) &&
                    (MaskInput.IndexOf(".", StringComparison.OrdinalIgnoreCase) > -1))){
                    return false;
                }
                return true;
            }
        }
       


        [XmlIgnore]
        public override bool AutoWriteTOValue {
            get { return true; }
        }


        [XmlIgnore]
        public bool IsDefaultMinValue {
            get { return MinValue == "-2147483648"; }
        }

        [XmlIgnore]
        public bool IsDefaultMaxValue {
            get { return MaxValue == "2147483647"; }
        }
    }

    [SupportedProperties("MaxDate, MinDate, Mask, ReadOnly, Value")]
    public partial class DateTextbox {
        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "dtb"; }
        }

        [XmlIgnore]
        public override string StyleName {
            get { return "PTDateEditorStyles.PT_StyleDef"; }
        }

        [XmlIgnore]
        public bool IsValueAvailable {
            get {
                DateTime dateTime;
                return !string.IsNullOrEmpty(Value) && DateTime.TryParse(Value, out dateTime);
            }
        }

        public override bool AutoWriteTOValue {
            get { return true; }
        }
    }

    [SupportedProperties("Mask, ReadOnly, Value")]
    public partial class TimeTextbox {
        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "ttb"; }
        }

        [XmlIgnore]
        public bool IsValueAvailable {
            get {

                DateTime dateTime;
                return !string.IsNullOrEmpty(Value) && DateTime.TryParse(Value,out dateTime);
            }
        }

        public override bool AutoWriteTOValue {
            get { return true; }
        }
    }

    [SupportedProperties("AreaCode, CountryCode, CityCode, ReadOnly, BrowseButtonVisible")]
    public partial class PhoneTextbox {
        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "ptb"; }
        }

        [XmlIgnore]
        public override bool AutoWriteTOValue {
            get { return true; }
        }

        [XmlIgnore]
        public bool ReadOnly {
            get { return false; }
        }
    }

        [SupportedProperties("Value, ReadOnly, MaskInput")]
    public partial class MaskTextbox {
        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "mtb"; }
        }

        [XmlIgnore]
        public override bool AutoWriteTOValue {
            get { return true; }
        }
    }

    [SupportedProperties("ShowCurrency")]
    public partial class AmountTextbox {
        private readonly string _nameIdentifier = Guid.NewGuid().ToString().Replace("-", "");

        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "atb"; }
        }

        [XmlIgnore]
        public GuiElement CurrencyText {
            get {
                NumericTextbox ptValueText = new NumericTextbox();
                ptValueText.ReadOnly = ReadOnly;
                ptValueText.LayoutInfo = LayoutInfo;
                ptValueText.MaskInput = MaskInput;
                ptValueText.Value = Value;

                ptValueText.Name = Name + _nameIdentifier + "PT_ValueText";

                ptValueText.PTName = PTName + ".PT_ValueText";

                return ptValueText;
            }
        }


        [XmlIgnore]
        public GuiElement CurrencySymbol {
            get {
                Label ptCurrencySymbol = new Label();
                ptCurrencySymbol.Name = Name + _nameIdentifier + "PT_CurrencySymbol";
                ptCurrencySymbol.PTName = PTName + ".PT_CurrencySymbol";
                return ptCurrencySymbol;
            }
        }


        public override bool IsNumeric {
            get { return false; }
        }
    }

    public class CustomControl : Control {
        public Dictionary<string, string> CustomProperties {
            get { return ComposeCustomProperties(); }
        }

        private Dictionary<string, string> ComposeCustomProperties() {
            Dictionary<string, string> customProperties = new Dictionary<string, string>();
            return customProperties;
        }
    }

    public class UC1 : CustomControl {}

    #region FunctionButton, Bar, BarManager

    [SupportedProperties("Align, Position, ButtonBarName, BeginGroup, HotKeyText, ButtonBarHotKey, WordWrap, IgnoreValidation")]
    public partial class FunctionButton {
        public FunctionButton(string name) {
            Name = name;
        }

        [XmlIgnore]
        public override bool SupportThemesProperty {
            get { return false; }
        }

        [XmlIgnore]
        public override bool AllowTabStop {
            get { return true; }
        }

        [XmlIgnore]
        public virtual bool HasButtonBarName {
            get { return ButtonBarName != null && ButtonBarName != string.Empty; }
        }


        [XmlIgnore]
        public virtual bool HasHotKeyText {
            get { return !string.IsNullOrEmpty(HotKeyText); }
        }

        [XmlIgnore]
        public virtual string[] HotKeyTextList {
            get {
                if (string.IsNullOrEmpty(HotKeyText)) {
                    return new string[0];
                }

                return HotKeyText.Split(',');
            }
        }

        [XmlIgnore]
        public override bool UseAddFormHotKey {
            get { return true; }
        }
    }

    public partial class DynamicFunctionButton {
        [XmlIgnore]
        public override bool SupportThemesProperty {
            get { return false; }
        }

        [XmlIgnore]
        public override bool AllowTabStop {
            get { return base.AllowTabStop; }
        }
    }


    public class FormFunctionBar : FunctionButtonBar {
        public FormFunctionBar(string name, BarHotKey hotkey) : base(true, name, hotkey) {
            ParentFormStyle = FunctionButtonBarParentFormStyle.Form;
        }
    }

    public class DialogFunctionBar : FunctionButtonBar {
        public DialogFunctionBar(string name, BarHotKey hotkey) : base(name, hotkey) {
            ParentFormStyle = FunctionButtonBarParentFormStyle.ModalForm;
        }

        public override void InitDefaultButtons() {
            //do nothing  
        }
    }

    [SupportedProperties("IsSelected")]
    public partial class FunctionButtonBar : IGuiElementParent, IContainer {
        private bool defaultButtonsInitialized = false;


        private Layout item1Field;

        [XmlIgnore]
        public Layout Layout {
            get { return item1Field; }
            set { item1Field = value; }
        }

        [XmlIgnore]
        public override bool AllowTabStop {
            get { return false; }
        }


        public void AddGuiElement(GuiElement element) {
            if (element is FunctionButton) {
                _allControls.Add(element);
                AddButton((FunctionButton) element);
            }
        }

        public void RemoveGuiElement(GuiElement element) {
            if (element is FunctionButton) {
                _allControls.Remove(element);
                RemoveButton((FunctionButton) element);
            }
        }


        private List<GuiElement> _allControls = new List<GuiElement>();

        [XmlIgnore]
        public GuiElement[] Controls {
            get { return _allControls.ToArray(); }
            set { _allControls = new List<GuiElement>(value); }
        }

        public FunctionButtonBar(bool formIsRootContainer, string name, BarHotKey hotkey) //initDefaultButtons, string name)
            : this() {
            Name = name;
            ButtonBarHotKey = hotkey;
            Events = new Event[0] {};
            Description = string.Empty;
            ParentFormStyle = formIsRootContainer ? FunctionButtonBarParentFormStyle.Form : FunctionButtonBarParentFormStyle.ModalForm;
            //on dialogs we have zero buttons. formular contains default 12 buttons 
            DefaultButtonsCount = formIsRootContainer ? 12 : 0;
            InitDefaultButtons();
        }

        public FunctionButtonBar(string name, BarHotKey hotkey) : this(false, name, hotkey) {}

        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "fbg"; }
        }

        [XmlIgnore]
        public string DockStyle {
            get { return "Fill"; }
        }

        [XmlIgnore]
        public virtual bool HasButtonBarHotKey {
            get { return ButtonBarHotKey != BarHotKey.NotDefined; }
        }


        [XmlIgnore]
        public virtual string ButtonBarHotKeyConverted {
            get {
                if (ButtonBarHotKey == BarHotKey.Alt) {
                    return "Menu";
                } else if (ButtonBarHotKey == BarHotKey.Ctrl) {
                    return "ControlKey";
                }

                throw new NotSupportedException("Key is not Supported");
            }
        }

        private void AddButton(FunctionButton newButton) {
            List<FunctionButton> listButtons = new List<FunctionButton>(FunctionButtons);
            listButtons.Add(newButton);
            FunctionButtons = listButtons.ToArray();
        }

        private void RemoveButton(FunctionButton newButton) {
            List<FunctionButton> listButtons = new List<FunctionButton>(FunctionButtons);
            listButtons.Remove(newButton);
            FunctionButtons = listButtons.ToArray();
        }

        public virtual void SetButton(FunctionButton btnAdd) {
            //TODO: Refactor. Evtl. können die Buttons später sortiert werden
            if (ParentFormStyle == FunctionButtonBarParentFormStyle.ModalForm) {
                btnAdd.BeginGroup = true;
                AddButton(btnAdd);
                return;
            }
            if (btnAdd.Position > 0 && btnAdd.Position <= 12) {
                FunctionButtons[btnAdd.Position - 1] = btnAdd;
            } else {
                for (int i = 0; i < FunctionButtons.Length; i++) {
                    if (FunctionButtons[i].Position < 1) {
                        btnAdd.Position = i + 1;
                        FunctionButtons[i] = btnAdd;
                        break;
                    }
                }
            }
        }


        public virtual void InitDefaultButtons() {
            if (!defaultButtonsInitialized) {
                FunctionButtons = new FunctionButton[DefaultButtonsCount];
                //Create default buttons
                for (int buttonIndex = 0; buttonIndex < DefaultButtonsCount; buttonIndex++) {
                    FunctionButtons[buttonIndex] = new FunctionButton(string.Format("{0}Button{1}", Name, buttonIndex));
                    FunctionButtons[buttonIndex].Enabled = BooleanControlState.@false;
                    FunctionButtons[buttonIndex].BeginGroup = false;

                    //Default settings for function bar is to group buttons in 
                    //three sections (4 Buttons, 3 Buttons and 5+ Buttons depending on DefaultButtonsCount)
                    if (buttonIndex == 4 || buttonIndex == 7) {
                        FunctionButtons[buttonIndex].BeginGroup = true;
                    }
                }
                defaultButtonsInitialized = true;
            }
        }

        #region IGuiElementParent Members

        public GuiElement[] GetAllGuiElements() {
            List<GuiElement> elements = new List<GuiElement>();
            foreach (FunctionButton element in FunctionButtons) {
                elements.Add(element);

                IGuiElementParent parent = element as IGuiElementParent;
                if (parent != null) {
                    elements.AddRange(parent.GetAllGuiElements());
                }
            }
            return elements.ToArray();
        }

        #endregion
    }


    [SupportedKeyWords("CONTROLS")]
    [SupportedControls("FunctionButton, FunctionButtonBar")]
    public partial class ButtonBarManager : IContainer, IGuiElementParent {
        private Layout layout;

        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "bbm"; }
        }

        [XmlIgnore]
        public bool SelectedButtonBarAssigned {
            get { return SelectedButtonBar != null; }
        }

        [XmlIgnore]
        public FunctionButtonBar SelectedButtonBar {
            get { return GetSelectedBar(); }
        }

        [XmlIgnore]
        public FunctionButtonBar StandardBar {
            get { return FunctionButtonBars[0]; }
        }


        private FunctionButtonBar GetSelectedBar() {
            foreach (FunctionButtonBar bar in FunctionButtonBars) {
                if (bar.IsSelected) {
                    return bar;
                }
            }
            if (FunctionButtonBars.Length > 0) {
                return FunctionButtonBars[0];
            } else {
                return null;
            }
        }

        #region IContainer Members

        [XmlIgnore]
        private List<FunctionButtonBar> ButtonBars {
            get { return new List<FunctionButtonBar>(FunctionButtonBars); }
        }

        public void AddGuiElement(GuiElement element) {
            _allControls.Add(element);
            if (element is FunctionButtonBar) {
                //TODO: PRÜFEN Zugriff auf ButtonBars, etwas ist hier faul, da Buttonbars immer eine neue Liste zurückgibt
                ButtonBars.Add(element as FunctionButtonBar);
                FunctionButtonBars = ButtonBars.ToArray();
            }
        }

        public void RemoveGuiElement(GuiElement element) {
            _allControls.Remove(element);
            if (element is FunctionButtonBar) {
                //TODO: PRÜFEN Zugriff auf ButtonBars, etwas ist hier faul, da Buttonbars immer eine neue Liste zurückgibt
                ButtonBars.Remove(element as FunctionButtonBar);
                FunctionButtonBars = ButtonBars.ToArray();
            }
        }

        private List<GuiElement> _allControls = new List<GuiElement>();

        [XmlIgnore]
        public GuiElement[] Controls {
            get { return _allControls.ToArray(); }
            set { _allControls = new List<GuiElement>(value); }
        }

        [XmlIgnore]
        public Layout Layout {
            get { return layout; }
            set { layout = value; }
        }

        #endregion

        #region IGuiElementParent Members

        private List<GuiElement> _allGuiElements = null;


        public GuiElement[] GetAllGuiElements() {
            if (_allGuiElements == null) {
                _allGuiElements = new List<GuiElement>();
                foreach (FunctionButtonBar bar in FunctionButtonBars) {
                    _allGuiElements.Add(bar);
                    _allGuiElements.AddRange(bar.FunctionButtons);
                }
            }
            return _allGuiElements.ToArray();
        }

        #endregion

        public override Container RootContainer {
            get { return base.RootContainer; }
            set {
                base.RootContainer = value;
                foreach (GuiElement element in GetAllGuiElements()) {
                    element.RootContainer = value;
                }
            }
        }

        [XmlIgnore]
        public bool EnabledIfModified {
            get { return false; }
        }

        [XmlIgnore]
        public bool HasToolTip {
            get { return false; }
        }

    }

    public partial class AbsolutLayout : Layout {
        [XmlIgnore]
        public bool IsContent { get { return false; } }
        [XmlIgnore]
        public bool IsFooter { get { return false; } }
        [XmlIgnore]
        public bool IsHeader { get { return false; } }
    }

    [SupportedKeyWords("TABS")]
    [SupportedControls("UserControlReference")]
    [SupportedProperties("Caption, ImageLibrary, ImageKey, ImageKeyRight, LeftCaption, RightCaption, NewGroup, Shared")]
    public partial class TabPage : IContainer, IGuiElementParent {
        private List<GuiElement> _allGuiElements = null;

        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "tbp"; }
        }

        [XmlIgnore]
        public string NewGroupFlag {
            get {
                if (NewGroup) {
                    return "true";
                } else {
                    return "false";
                }
            }
        }

        #region IGuiElementParent Members

        private Layout _layout = new AbsolutLayout();

        [XmlIgnore]
        public Layout Layout {
            get { return _layout; }
            set { _layout = value; }
        }

        [XmlIgnore]
        private string TabCaption {
            get { return Caption ?? Name; }
        }

        public bool ContainsUserControl(GuiElement guielement) {
            foreach (GuiElement element in GuiElements) {
                if (element.Equals(guielement)) {
                    return true;
                }
            }
            return false;
        }

        public GuiElement[] GetAllGuiElements() {
            if (_allGuiElements == null) {
                _allGuiElements = new List<GuiElement>();
                foreach (GuiElement guiElement in GuiElements) {
                    IGuiElementParent childContainer = guiElement as IGuiElementParent;
                    if (childContainer != null) {
                        _allGuiElements.AddRange(childContainer.GetAllGuiElements());
                    }
                }
            }
            return _allGuiElements.ToArray();
        }

        #endregion

        [XmlIgnore]
        public bool HasUserControl {
            get { return GuiElements.Length > 0; }
        }


        [XmlIgnore]
        public bool IsAdvanced {
            get { return ((TabNavigation) this.ParentContainer).NavigationType == TabNavigationNavigationType.Advanced; }
        }

        [XmlIgnore]
        public bool IsImageDefined {
            get { return (!string.IsNullOrEmpty(ImageLibrary) && !string.IsNullOrEmpty(ImageKey)); }
        }

        [XmlIgnore]
        public bool IsImageAdvancedDefined {
            get { return !string.IsNullOrEmpty(ImageLibrary) && (!string.IsNullOrEmpty(ImageKey) || !string.IsNullOrEmpty(ImageKeyRight)); }
        }

        [XmlIgnore]
        public string UserControlTOName {
            get { return HasUserControl ? ((UserControlReference) GuiElements[0]).TOTypeFullName : string.Empty; }
        }

        [XmlIgnore]
        public string UserControlPTName {
            get { return HasUserControl ? ((UserControlReference) GuiElements[0]).PTName : null; }
        }

        [XmlIgnore]
        public string UserControlSharedPTName {
            get {
                if (!HasUserControl) {
                    return null;
                }

                if (!Shared) {
                    return GuiElements[0].PTName;
                }

                string sharedPTName = GuiElements[0].PTName;
                foreach (TabPage p in ((TabNavigation) ParentContainer).TabPages) {
                    if (
                        ((UserControlReference) p.GuiElements[0]).TypeName.Equals(((UserControlReference) GuiElements[0]).TypeName,
                                                                                  StringComparison.InvariantCultureIgnoreCase)) {
                        return p.GuiElements[0].PTName;
                    }
                }

                return sharedPTName;
            }
        }

        [XmlIgnore]
        public string UserControlType {
            get { return HasUserControl ? ((UserControlReference) GuiElements[0]).TypeFullName : null; }
        }

        #region IContainer Members

        public void AddGuiElement(GuiElement element) {
            List<GuiElement> elements = new List<GuiElement>();
            if (guiElementsField != null) {
                elements.AddRange(guiElementsField);
            }
            //jedes TabPage darf nur ein einziges UserControl beinhalten.
            //Usercontrol wird mit dem gleichen Namen benamt
            element.Name = Name;
            elements.Add(element);
            guiElementsField = elements.ToArray();
        }

        public void RemoveGuiElement(GuiElement element) {
            List<GuiElement> elements = new List<GuiElement>(guiElementsField);
            elements.Remove(element);
            guiElementsField = elements.ToArray();
        }

        [XmlIgnore]
        public GuiElement[] Controls {
            get { return guiElementsField; }
            set { guiElementsField = value; }
        }

        #endregion
    }

    [SupportedKeyWords("TABS")]
    [SupportedControls("TabPage")]
    [SupportedProperties("NavigationType, TabPosition, TabWidth")]
    [SupportedGuiEvents("SelectedTabChanged")]
    public partial class TabNavigation : IContainer, IGuiElementParent, IUserControlReferenceContainer {
        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "tbc"; }
        }

        [XmlIgnore]
        public override bool AutoWriteTOValue {
            get { return true; }
        }

        [XmlIgnore]
        public bool IsMenuNavigation {
            get { return NavigationType == TabNavigationNavigationType.Menu; }
        }

        [XmlIgnore]
        public bool IsAdvanced {
            get { return NavigationType == TabNavigationNavigationType.Advanced; }
        }

        [XmlIgnore]
        public bool HasTabWidth {
            get { return TabWidth > 0; }
        }


        [XmlIgnore]
        public string ClassName {
            get {
                if (NavigationType == TabNavigationNavigationType.Menu) {
                    return "PTNavigationTabControl";
                } else if (NavigationType == TabNavigationNavigationType.Advanced) {
                    return "PTAdvancedTabControl";
                } else {
                    return "PTTabControl";
                }
            }
        }

        #region IContainer Members

        private Layout _layout = new AbsolutLayout();

        [XmlIgnore]
        public Layout Layout {
            get { return _layout; }
            set { _layout = value; }
        }


        [XmlIgnore]
        private List<TabPage> TabPagesList {
            get { return new List<TabPage>(TabPages); }
        }


        /// <summary>
        /// Used for GUI Designer to add a range of tab pages
        /// </summary>
        [XmlIgnore]
        public string TabPageRangeString {
            get {
                List<string> range = new List<string>();
                foreach (TabPage page in TabPages) {
                    range.Add(page.PTName + "Page");
                }
                return string.Join(",", range.ToArray());
            }
        }

        /// <summary>
        /// Default Tabs key
        /// </summary>
        [XmlIgnore]
        public string SelectedTabKey {
            get {
                if (TabPagesList.Count > 0) {
                    return TabPagesList[0].Name;
                }
                return null;
            }
        }


        /// <summary>
        /// Used for GUI Designer to add a range of tabs
        /// </summary>
        [XmlIgnore]
        public string TabRangeString {
            get {
                List<string> range = new List<string>();
                foreach (TabPage page in TabPages) {
                    range.Add(page.PTName);
                }
                return string.Join(",", range.ToArray());
            }
        }


        /// <summary>
        /// Used for GUI Designer to add a range of tab keys
        /// </summary>
        [XmlIgnore]
        public string TabKeysString {
            get {
                List<string> range = new List<string>();
                foreach (TabPage page in TabPages) {
                    range.Add(page.Name);
                }
                return string.Join(",", range.ToArray());
            }
        }


        public void AddGuiElement(GuiElement element) {
            _allControls.Add(element);
            List<TabPage> pages = TabPagesList;
            pages.Add(element as TabPage);
            TabPages = pages.ToArray();
        }

        public void RemoveGuiElement(GuiElement element) {
            _allControls.Remove(element);
            List<TabPage> pages = TabPagesList;
            pages.Remove(element as TabPage);
            TabPages = pages.ToArray();
        }

        private List<GuiElement> _allControls = new List<GuiElement>();


        [XmlIgnore]
        public GuiElement[] Controls {
            get {
                List<GuiElement> tempList = new List<GuiElement>(TabPages);
                tempList.AddRange(GetSharedControls());
                return tempList.ToArray();
            }
            set { _allControls = new List<GuiElement>(value); }
        }

        #endregion

        public bool ContainsUserControl(GuiElement guiElement) {
            foreach (TabPage page in TabPages) {
                if (page.ContainsUserControl(guiElement)) {
                    return true;
                }
            }
            return false;
        }

        #region IGuiElementParent Members

        private List<GuiElement> _allGuiElements = null;

        public GuiElement[] GetAllGuiElements() {
            if (_allGuiElements == null) {
                _allGuiElements = new List<GuiElement>();
                foreach (TabPage page in TabPages) {
                    _allGuiElements.Add(page);
                    _allGuiElements.AddRange(page.GuiElements);
                }
                //_allGuiElements.AddRange(GetSharedControls());
            }
            return _allGuiElements.ToArray();
        }

        private GuiElement[] GetSharedControls() {
            return SharedControls ?? new GuiElement[0];
        }

        #endregion

        /// <summary>
        /// Used for GUI Designer to add a range of shared controls
        /// </summary>
        [XmlIgnore]
        public string SharedControlsNameString {
            get {
                List<string> range = new List<string>();
                foreach (GuiElement element in SharedControls) {
                    range.Add(element.PTName);
                }
                return string.Join(",", range.ToArray());
            }
        }

        [XmlIgnore]
        public bool HasSharedControl {
            get { return SharedControls != null && SharedControls.Length > 0; }
        }

        [XmlIgnore]
        public string SharedControlTOName {
            get { return HasSharedControl ? ((UserControlReference) GetSharedControls()[0]).TOTypeFullName : string.Empty; }
        }

        [XmlIgnore]
        public GuiElement SharedControl {
            get { return GetSharedControls().Length > 0 ? GetSharedControls()[0] : null; }
        }

        [XmlIgnore]
        public string SharedControlTOMemberVariableName {
            get { return HasSharedControl ? GetSharedControls()[0].MemberVariableName : string.Empty; }
        }

        [XmlIgnore]
        public string SharedControlTOMemberPropertyName {
            get { return HasSharedControl ? GetSharedControls()[0].MemberPropertyName : string.Empty; }
        }

        public void AddSharedGuiElement(GuiElement element) {
            List<GuiElement> sharedControls = new List<GuiElement>(SharedControls ?? new GuiElement[0]);
            sharedControls.Add(element);
            SharedControls = sharedControls.ToArray();
        }


        public IEnumerable<UserControlReference> GetAllUserControlReferences() {
            foreach (GuiElement control in GetAllGuiElements()) {
                if (control is UserControlReference) {
                    yield return control as UserControlReference;
                }
            }
            if (HasSharedControl) {
                yield return SharedControl as UserControlReference;
            }
        }
    }

    #endregion

    internal interface IGuiElementParent {
        GuiElement[] GetAllGuiElements();
    }


    public partial class EditorUIControl {
        [XmlIgnore]
        public Table Parent {
            get { return _parent; }
            set { _parent = value; }
        }

        [XmlIgnore]
        public ColumnInfoType CurrentCol {
            get { return _currentCol; }
            set { _currentCol = value; }
        }

        protected Table _parent;
        protected ColumnInfoType _currentCol;

        public void AddInfo(Table parent, ColumnInfoType currentCol) {
            _parent = parent;
            _currentCol = currentCol;
        }

        public override string PTName {
            get { return "Schwachsinn."; }
        }
    }


    public partial class BrowseEditorUI {
        /// <summary>
        /// Gets the name of the Control with the prefix
        ///
        /// </summary>
        [XmlIgnore]
        public override string PTName {
            get { return string.Format("{0}{1}{2}BrowsableTextbox", Parent.PTName, CurrentCol.BandInfo.TOBandName, CurrentCol.Key); }
        }
    }

    public partial class PhoneEditorUI {
        /// <summary>
        /// Gets the name of the Control with the prefix
        ///
        /// </summary>
        [XmlIgnore]
        public override string PTName {
            get { return string.Format("{0}{1}{2}PhoneControl", Parent.PTName, CurrentCol.BandInfo.TOBandName, CurrentCol.Key); }
        }
    }


    public partial class ComboBoxEditorUI {
        [XmlIgnore]
        public override string PTName {
            get { return _parent.PTName + _currentCol.BandInfo.TOBandName + _currentCol.Key + "ComboBox"; }
        }
    }


    public partial class TimeEditorUI {
        [XmlIgnore]
        public override string PTName {
            get { return _parent.PTName + _currentCol.BandInfo.TOBandName + _currentCol.Key + "TimeControl"; }
        }

        [XmlIgnore]
        public string PTType {
            get { return "PTTimeEditorUI"; }
        }
    }


    public partial class DefaultEditorUI {
        [XmlIgnore]
        public override string PTName {
            get {

                string postfix = "";
                
                if (_currentCol.IsAmountbox) {
                    postfix = "AmountEditorControl";
                }else if (_currentCol.IsAutoSuggestion) {
                    postfix = "AutoSuggestion";
                } else if (_currentCol.IsBrowsableTextbox) {
                    postfix = "BrowsableTextbox";
                } else if (_currentCol.IsCombobox) {
                    postfix = "ComboBox";
                } else if (_currentCol.IsDateControl) {
                    postfix = "DateEditorControl";
                } else if (_currentCol.IsDouble || _currentCol.IsInteger || _currentCol.IsReal || _currentCol.IsPercent || _currentCol.IsNumeric) {
                    postfix = "NumericEditorControl";
                } else if (_currentCol.IsIcon) {
                    postfix = "IconEditorControl";
                } else if (_currentCol.IsMaskedTextbox) {
                    postfix = "MaskedTextboxControl";
                } else if (_currentCol.IsPhoneControl) {
                    postfix = "PhoneControl";
                } else if (_currentCol.IsTimeControl) {
                    postfix = "TimeControl";
                } else if (_currentCol.IsImageTextEditor) {
                    postfix = "ImageTextEditorControl";
                }else {
                    postfix = "NotImplemented";
                }

               
                return _parent.PTName + _currentCol.BandInfo.TOBandName + _currentCol.Key + postfix;
            }
        }
    }

    [SupportedProperties("*ImageLibrary, Text")]
    public partial class ExplorerBar {
        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "epb"; }
        }
    }


    public partial class Cave {
        [XmlIgnore]
        public override string ControlNamePrefix {
            get { return "cav"; }
        }
    }

    [SupportedProperties("DecodedType")]
    public partial class Scanner : INonVisualGuiElement {
        [XmlIgnore]
        public virtual Layout Layout {
            get { return null; }
        }
    }

    [SupportedProperties("Text")]
    public partial class WebBrowser {
        public override string ControlNamePrefix {
            get { return "brw"; }
        }
    }

    [SupportedProperties("*ToolsCount, Style, ToolItems")]
    public partial class ContextMenu : IItemWithKey {
        private readonly GuiElement _parent;

        public ContextMenu() {
        }

        public ContextMenu(GuiElement parent) {
            _parent = parent;
        }

        private int _toolsCount;
        [XmlIgnore]
        public int ToolsCount {
            get { return _toolsCount; }

            set {
                _toolsCount = value;
                ToolItems = new ToolItemType[_toolsCount];
                for (int i = 0; i < ToolItems.Length; i++) {
                    ToolItems[i] = new ToolItemType(this);
                }
            }
        }
        [XmlIgnore]
        public string Key {
            get { return _parent.PTName; }
        }

        

        [XmlIgnore]
        public string KeysTOEnum {
            get {
                var enums = new List<string>();
                GenerateEnum(ToolItems, enums,0);

                var result="";
                foreach (var e in enums) {
                    result += e + "\n";
                }
                return result;
            }
        }

        private void GenerateEnum(ToolItemType[] toolItemTypes, List<string> enums, int level) {
            var result = string.Format("public enum {0}ContextMenuItemsLevel{1}", Key, level) + "{";

            foreach (var toolItemType in toolItemTypes) {
                result += string.Format("{0},",toolItemType.Key);
                if (toolItemType.ToolsCount>0) {
                    GenerateEnum(toolItemType.ToolItems, enums, ++level);
                }
            }
            result = result.Substring(0, result.Length - 1);
            result += "}";
            enums.Add(result);
        }

        [XmlIgnore]
        public bool HasStyle {
            get { return !string.IsNullOrEmpty(Style); }
        }

        List<RegisteredMethodInfo> _eventList;

        [XmlIgnore]
        public List<RegisteredMethodInfo> RegisteredMethodInfos {
            get {
                if (_eventList == null) {
                    _eventList = new List<RegisteredMethodInfo>();
                    GenerateRegisteredMethodInfos(_eventList, ToolItems);
                }
                return _eventList;
            }
        }
        [XmlIgnore]
        public List<Event> Events {
            get {
                var eventList = new List<Event>();
                GenerateEvents(eventList, ToolItems);
                return eventList;
            }
        }

        private void GenerateEvents(List<Event> eventList, ToolItemType[] currentToolItems) {
            if (toolItemsField==null) {
                return;
            }

            for (int i = 0; i < currentToolItems.Length; i++) {
                var toolItem = currentToolItems[i];
                if (toolItem.HasToolClick) {
                    eventList.Add(new Event("Click", toolItem.Key + "Click",
                                      new EventDelegateInfo(toolItem.ToolClick, "EventHandler", "void",
                                                            new[] { "System.Object", "EventArgs" })));
                }
                if (toolItem.HasSubItems) {
                     GenerateEvents(eventList, toolItem.ToolItems);
                }
            }
        }

        private bool IsAlreadyDefined(RegisteredMethodInfo methodInfo) {
            foreach (var allDefinedEventsAndHotkey in _parent.RootContainer.AllDefinedEventsAndHotkeys) {
                if(allDefinedEventsAndHotkey.MethodName == methodInfo.MethodName) {
                    return true;
                }
            }
            return false;
        }

        private void GenerateRegisteredMethodInfos(List<RegisteredMethodInfo> eventList, ToolItemType[] currentToolItems) {
            if (currentToolItems==null) {
                return;
            }
            for (int i = 0; i < currentToolItems.Length; i++) {
                var toolItem = currentToolItems[i];
                if (toolItem.HasToolClick) {
                    var registeredMethodInfo = new RegisteredMethodInfo(toolItem.ToolClick,CallType.WFSCall);
                    if (!IsAlreadyDefined(registeredMethodInfo)) {
                        eventList.Add(registeredMethodInfo);
                        _parent.RootContainer.AllDefinedEventsAndHotkeys.Add(registeredMethodInfo);
                    }
                }
                if (toolItem.HasSubItems) {
                    GenerateRegisteredMethodInfos(eventList, toolItem.ToolItems);
                }
            }
        }
    }
    public interface IItemWithKey {
        string Key { get;}
    }
    [SupportedProperties("*Key, Caption, ToolClick, Type, ToolsCount, ToolItems, Shortcut, ImageLibrary, ImageKey, IsFirstInGroup, Enabled, Visible")]
    public partial class ToolItemType : IItemWithKey {
        private readonly IItemWithKey _parent;

        public ToolItemType(IItemWithKey parent) : this() {
            _parent = parent;
        }

        private int _toolsCount;
        [XmlIgnore]
        public int ToolsCount {
            get { return _toolsCount; }

            set {
                _toolsCount = value;
                ToolItems = new ToolItemType[_toolsCount];
                for (int i = 0; i < ToolItems.Length; i++) {
                    ToolItems[i] = new ToolItemType(this);
                }
            }
        }

        [XmlIgnore]
        public string ParentToolKey {
            get { return _parent.Key; }
        }

        [XmlIgnore]
        public string TypeClass {
            get {
                if (Type == ToolItemTypeType.Button) {
                    return "ButtonTool";
                } else if (Type == ToolItemTypeType.Popup) {
                    return "PopupMenuTool";
                }

                throw new NotSupportedException(string.Format("Type {0} not supported", Type));
            }
        }

        [XmlIgnore]
        public string CaptionOrKey {
            get {
                return string.IsNullOrEmpty(Caption) ? Key : Caption;
            }
        }

        [XmlIgnore]
        public bool HasSubItems {
            get {
                return ToolsCount > 0;
            }
        }

        [XmlIgnore]
        public string AllSubItemsInstances {
            get {
                string result="";
                foreach (var toolItemType in ToolItems) {
                    result += "tool" + toolItemType.Key + ",";
                }

                return result; // .Substring(0,result.Length-1);
            }
        }
        

        [XmlIgnore]
        public bool HasToolClick {
            get {
                return !string.IsNullOrEmpty(ToolClick);
            }
        }

        [XmlIgnore]
        public bool HasShortcut {
            get {
                return !string.IsNullOrEmpty(Shortcut);
            }
        }

        [XmlIgnore]
        public bool HasImage {
            get {
                return !string.IsNullOrEmpty(ImageLibrary) && !string.IsNullOrEmpty(ImageKey);
            }
        }
    }
}
