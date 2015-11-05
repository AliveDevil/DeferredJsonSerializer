using Newtonsoft.Json.Linq;

namespace de.alivedevil.Nodes
{
    public class PropertyNode : Node
    {
        public string Name { get; set; }

        public Node Value { get; set; }
    }
}
