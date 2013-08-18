﻿using Jint.Native.Object;
using Jint.Runtime.Descriptors.Specialized;

namespace Jint.Native.Json
{
    public sealed class JsonInstance : ObjectInstance
    {
        private readonly Engine _engine;

        private JsonInstance(Engine engine, ObjectInstance prototype)
            : base(engine, prototype)
        {
            _engine = engine;
            Extensible = true;
        }

        public override string Class
        {
            get
            {
                return "JSON";
            }
        }

        public static JsonInstance CreateJsonObject(Engine engine)
        {
            var json = new JsonInstance(engine, engine.Object.Prototype);
            json.DefineOwnProperty("parse", new ClrDataDescriptor<JsonInstance, object>(engine, json.Parse), false);
            json.DefineOwnProperty("stringify", new ClrDataDescriptor<JsonInstance, object>(engine, json.Stringify), false);

            return json;
        }

        public object Parse(JsonInstance thisObject, object[] arguments)
        {
            var parser = new JsonParser(_engine);

            return parser.Parse(arguments[0].ToString());
        }

        public object Stringify(JsonInstance thisObject, object[] arguments)
        {
            object 
                value = Undefined.Instance, 
                replacer = Undefined.Instance,
                space = Undefined.Instance;

            if (arguments.Length > 2)
            {
                space = arguments[2];
            }

            if (arguments.Length > 1)
            {
                replacer = arguments[1];
            }

            if (arguments.Length > 0)
            {
                value = arguments[0];
            }

            var serializer = new JsonSerializer(_engine);
            return serializer.Serialize(value, replacer, space);
        }
    }
}
