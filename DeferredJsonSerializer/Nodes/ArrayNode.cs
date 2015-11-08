using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace de.alivedevil.Nodes
{
    public class ArrayNode : Node
    {
        private List<Node> nodes = new List<Node>();

        public List<Node> Nodes
        {
            get { return nodes; }
        }

        public override void ReadOut(DeferredJsonSerializer serializer)
        {
            for (int i = 0; i < Nodes.Count; i++)
                Nodes[i].ReadOut(serializer);
        }

        public override void WriteOut(DeferredJsonSerializer serializer)
        {
            for (int i = 0; i < Nodes.Count; i++)
                Nodes[i].WriteOut(serializer);
        }
    }
}
