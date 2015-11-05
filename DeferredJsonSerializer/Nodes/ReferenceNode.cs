using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace de.alivedevil.Nodes
{
    public class ReferenceNode : Node
    {
        public object Reference { get; set; }

        public override void WriteOut(DeferredJsonSerializer serializer, JTokenWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("$ref");
            writer.WriteValue(serializer.ObjectIdLookup[Reference]);
            writer.WriteEndObject();
        }
    }
}
