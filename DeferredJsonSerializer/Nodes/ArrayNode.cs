using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace de.alivedevil.Nodes
{
    public class ArrayNode : Node
    {
        public List<Node> Nodes { get; } = new List<Node>();

        public override void WriteOut(DeferredJsonSerializer serializer, JTokenWriter writer)
        {
            writer.WriteStartArray();
            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].WriteOut(serializer, writer);
            }
            writer.WriteEndArray();
        }
    }
}
