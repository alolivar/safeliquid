using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SafeLiquid
{
    public class DropProxy : DropBase, IValueTypeConvertible
    {
        private readonly string[] _allowedMembers;
        private readonly object _proxiedObject;
        private readonly Func<object, object> _value;
        private readonly bool _allowAllMembers;

        public DropProxy(object theObject, string[] allowedMembers)
        {
            this._proxiedObject = theObject;
            this._allowedMembers = allowedMembers;
            string[] allowedMembers1 = this._allowedMembers;
            this._allowAllMembers = (allowedMembers1 != null ? (allowedMembers1.Length == 1 ? 1 : 0) : 0) != 0 && this._allowedMembers[0] == "*";
        }

        public DropProxy(object theObject, string[] allowedMembers, Func<object, object> value)
          : this(theObject, allowedMembers)
        {
            this._value = value;
        }

        public virtual object ConvertToValueType() => this._value == null ? (object)null : this._value(this._proxiedObject);

        internal override object GetObject() => this._proxiedObject;

        internal override TypeResolution CreateTypeResolution(Type type) => new TypeResolution(type, (Func<MemberInfo, bool>)(mi => this._allowAllMembers || ((IEnumerable<string>)this._allowedMembers).Contains<string>(mi.Name)));
    }
}
