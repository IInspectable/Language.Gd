using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Pharmatechnik.Apotheke.XTplus.Framework.Tools.Generators.GuiModelGenerator.GuiMetaModel {

    /// <summary>
    /// Helper class to find control states definitions
    /// </summary>
    public class ControlStateReflector {

        /// <summary>
        /// Check all properties for control state definition and retrieve the list.
        /// 
        /// only active (not Undefined) control state will be retrieved.
        /// </summary>
        /// <returns>List with active control states</returns>
        public static List<ControlStateInfo> Find(object obj) {
            List<ControlStateInfo> csInfos = new List<ControlStateInfo>();
            foreach (MemberInfo mem in obj.GetType().FindMembers(MemberTypes.Property, BindingFlags.Public | BindingFlags.Instance,
                new MemberFilter(IsControlStateDefined), "IControlState")) {
                object objVal = ((PropertyInfo)mem).GetValue(obj, null);
                if (!((IControlState)objVal).Undefined) {
                    csInfos.Add(new ControlStateInfo(mem.Name, ((PropertyInfo)mem).GetValue(obj, null).ToString(),
                        ((IControlState)objVal).TypeName, obj as GuiElement));
                }
            }
            return csInfos;
        }


        private static bool IsControlStateDefined(System.Reflection.MemberInfo objMemberInfo, Object objSearchFor) {
            return (((PropertyInfo)objMemberInfo).PropertyType.GetInterface(objSearchFor.ToString(), true) != null);
        }
    }

    /// <summary>
    /// Info class describes control state property.
    /// 
    /// Used in gui generator ( stringtemplates ) to define control states in ControlStateTO
    /// </summary>
    public class ControlStateInfo {

        private string _controlStateName = null;
        /// <summary>
        /// Name of the control state ( like enabled, visible, readonly )
        /// 
        /// </summary>
        public string ControlStateName {
            get {
                return _controlStateName;
            }
            set {
                _controlStateName = value;
            }
        }

        private string _controlStateType = null;
        /// <summary>
        /// Get/Set Control state type( like bool?, string)
        /// </summary>
        public string ControlStateTypeName {
            get {
                return _controlStateType;
            }
            set {
                _controlStateType = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsStringType {
            get {
                return ControlStateTypeName.Equals("String", StringComparison.CurrentCultureIgnoreCase);
            }
        }


        private string _controlStateValue = null;

        /// <summary>
        /// Value of the control state (true, false)
        /// </summary>
        public string ControlStateValue {
            get {
                return _controlStateValue;
            }
            set {
                _controlStateValue = value;
            }
        }

        private GuiElement _guiElement;

        public GuiElement GuiElement {
            get { return _guiElement; }

        }

        /// <summary>
        /// Empty ctor
        /// </summary>
        private ControlStateInfo() {

        }

        /// <summary>
        /// Control state info ctor
        /// </summary>
        /// <param name="stateName">Name of the property defines the control state.</param>
        /// <param name="stateValue">value of the control state</param>
        public ControlStateInfo(string stateName, string stateValue, string typeName, GuiElement guiElement) {
            this._controlStateName = stateName != null ? stateName.Replace("ControlState", "") : stateName;
            this._controlStateValue = stateValue;
            this._controlStateType = typeName;
            this._guiElement = guiElement;
        }

    }

    /// <summary>
    /// Control state definition
    /// </summary>
    public interface IControlState {

        /// <summary>
        /// Test if control state is undefined
        /// </summary>
        bool Undefined {
            get;
            set;
        }
        /// <summary>
        /// Type name
        /// </summary>
        string TypeName {
            get;
            set;
        }

    }

    /// <summary>
    /// Defines control state properties
    /// </summary>
    /// <typeparam name="T">Type of the control state</typeparam>
    public class ControlState<T> : IControlState {
        private bool _isUndefined = true;
        private string _typeName = null;
        protected T _value;

        public T Value {
            get { return _value; }
            set { _value = value; }
        }

        internal ControlState(T obj)
            : this(obj, null) {

        }

        internal ControlState(T obj, string typeName) {

            _isUndefined = (obj == null);
            _value = obj;
            _typeName = typeName;

        }

        public ControlState() { }

        public static implicit operator T(ControlState<T> obj) {
            return obj;
        }

        /// <summary>
        /// Implicit operators the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static implicit operator ControlState<T>(T obj) {
            return new ControlState<T>(obj);
        }
        /// <summary>
        /// ToString on the value of this control state
        /// </summary>
        /// <returns>value as string</returns>
        public override string ToString() {
            if (_value == null) {
                return string.Empty;
            }

            return _value.ToString();

        }

        #region IControlState Members

        public bool Undefined {
            get {
                return _isUndefined;
            }
            set {
                _isUndefined = value;
            }
        }

        public string TypeName {
            get {
                return _typeName == null ? typeof(T).Name : _typeName;
            }
            set {
                _typeName = value;
            }

        }

        #endregion


    }

    /// <summary>
    /// Defines string control state
    /// 
    /// Control state is undefined if string value is null or empty
    /// </summary>
    public class StringValueControlState : ControlState<string> {
        /// <summary>
        /// String Value control state constructor
        /// </summary>
        /// <param name="value">initial value of the control state</param>
        public StringValueControlState(string value)
            : base(value) {

            Undefined = value == null || value == string.Empty;
        }

    }

    /// <summary>
    /// Defines an enumeration ControlState
    /// 
    /// Control state is defined if the value of the enum is unlike UndefinedControlState
    /// </summary>
    public class EnumValueControlState<T> : ControlState<T> {
        /// <summary>
        /// Enum Value control state constructor
        /// </summary>
        /// <param name="value">initial value of the control state</param>
        public EnumValueControlState(T value)
            : this(value, null) {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">Enumeration value</param>
        /// <param name="internalTypeName">type name used in gui code generator</param>
        public EnumValueControlState(T value, string internalTypeName)
            : base(value) {
            TypeName = internalTypeName;
            Undefined = IsUndefined(value);
        }



        private bool IsUndefined(T value) {
            if (Enum.IsDefined(value.GetType(), "UndefinedControlState")) {
                return value.ToString().Equals("UndefinedControlState", StringComparison.CurrentCultureIgnoreCase);
            }
            else {
                throw new Exception("Enumeration type is not a control state enumeration! Missing <UndefinedControlState> value!");
            }
        }

        /// <summary>
        /// ToString on the value of this control state
        /// </summary>
        /// <returns>value as string</returns>
        public override string ToString() {
            if (Enum.IsDefined(_value.GetType(), "modified")) {
                if (_value.ToString().Equals("modified", StringComparison.CurrentCultureIgnoreCase))
                    return "false";
            }

            return base.ToString();

        }

    }

    /// <summary>
    /// Defines an int ControlState
    /// 
    /// Control state is not defined if the value is -9999
    /// </summary>
    public class IntValueControlState : ControlState<int> {
        /// <summary>
        /// Int Value control state constructor
        /// </summary>
        /// <param name="value">initial value of the control state</param>
        public IntValueControlState(int value)
            : base(value) {
            Undefined = value == -9999;
        }
    }

    /// <summary>
    /// Defines an decimal ControlState
    /// 
    /// Control state is not defined if the value is -9999
    /// </summary>
    public class DecimalValueControlState : ControlState<decimal> {
        /// <summary>
        /// Decimal Value control state constructor
        /// </summary>
        /// <param name="value">initial value of the control state</param>
        public DecimalValueControlState(decimal value)
            : base(value) {
            Undefined = value == -9999;
        }
    }


}
