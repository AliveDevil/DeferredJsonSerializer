using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace de.alivedevil.Nodes
{
    public class ObjectNode : Node
    {
        public List<PropertyNode> Nodes { get; } = new List<PropertyNode>();

        public object Reference { get; set; }

        public override void WriteOut(DeferredJsonSerializer serializer, JTokenWriter writer)
        {
            writer.WriteStartObject();
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
