using Newtonsoft.Json.Linq;

namespace de.alivedevil.Nodes
{
    public class ValueNode : Node
    {
        public override void WriteOut(DeferredJsonSerializer serializer)
        {
            ((JValue)Token).Value = Reference;
        }
    }
}
