using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace de.alivedevil.Nodes
{
    public class ArrayNode : Node
    {
        public List<Node> Nodes { get; } = new List<Node>();

        public override void WriteOut(DeferredJsonSerializer serializer)
        {
            JArray array = (JArray)Token;
            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].WriteOut(serializer);
                array.Add(Nodes[i].Token);
            }
        }
    }
}
