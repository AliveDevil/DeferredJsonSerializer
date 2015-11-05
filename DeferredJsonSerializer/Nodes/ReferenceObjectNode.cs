using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace de.alivedevil.Nodes
{
    public class ReferenceObjectNode : ObjectNode
    {
        public int Id { get; set; }

        public override void WriteOut(DeferredJsonSerializer serializer, JTokenWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("$id");
            writer.WriteValue(Id);
            writer.WritePropertyName("$type");
            writer.WriteValue($"{Reference.GetType().FullName}, {Reference.GetType().Assembly.GetName().Name}");
            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].WriteOut(serializer, writer);
            }
            writer.WriteEndObject();
        }
    }
}
