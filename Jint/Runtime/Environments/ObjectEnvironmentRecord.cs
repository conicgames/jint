﻿using System;
using System.Linq;
using Jint.Native;
using Jint.Native.Object;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Descriptors.Specialized;

namespace Jint.Runtime.Environments
{
    /// <summary>
    /// Represents an object environment record
    /// http://www.ecma-international.org/ecma-262/5.1/#sec-10.2.1.2
    /// </summary>
    public sealed class ObjectEnvironmentRecord : EnvironmentRecord
    {
        private readonly Engine _engine;
        private readonly ObjectInstance _bindingObject;
        private readonly bool _provideThis;

        public ObjectEnvironmentRecord(Engine engine, ObjectInstance bindingObject, bool provideThis) : base(engine)
        {
            _engine = engine;
            _bindingObject = bindingObject;
            _provideThis = provideThis;
        }

        public override bool HasBinding(string name)
        {
            return _bindingObject.HasProperty(name);
        }

        /// <summary>
        /// http://www.ecma-international.org/ecma-262/5.1/#sec-10.2.1.2.2
        /// </summary>
        public override void CreateMutableBinding(string name, JsValue value, bool configurable = true)
        {
            var propertyDescriptor = configurable
                ? (IPropertyDescriptor) new ConfigurableEnumerableWritablePropertyDescriptor(value)
                : new NonConfigurablePropertyDescriptor(value);

            _bindingObject.SetOwnProperty(name, propertyDescriptor);
        }

        public override void SetMutableBinding(string name, JsValue value, bool strict)
        {
            _bindingObject.Put(name, value, strict);
        }

        public override JsValue GetBindingValue(string name, bool strict)
        {
            var desc = _bindingObject.GetProperty(name);
            if (strict && desc == PropertyDescriptor.Undefined)
            {
                throw new JavaScriptException(_engine.ReferenceError);
            }

            return UnwrapJsValue(desc);
        }

        public override bool DeleteBinding(string name)
        {
            return _bindingObject.Delete(name, false);
        }

        public override JsValue ImplicitThisValue()
        {
            if (_provideThis)
            {
                return _bindingObject;
            }

            return Undefined;
        }

        public override string[] GetAllBindingNames()
        {
            if (_bindingObject != null)
            {
                return _bindingObject.GetOwnProperties().Select( x=> x.Key).ToArray();
            }

            return Array.Empty<string>();
        }
    }
}
