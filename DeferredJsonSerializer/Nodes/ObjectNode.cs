using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace de.alivedevil.Nodes
{
    public class ObjectNode : Node
    {
        public List<PropertyNode> Nodes { get; } = new List<PropertyNode>();

        public override void WriteOut(DeferredJsonSerializer serializer)
        {
            JObject jObject = (JObject)Token;
            jObject["$type"] = $"{Reference.GetType().FullName}, {Reference.GetType().Assembly.GetName().Name}";
            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].Value.WriteOut(serializer);
                jObject[Nodes[i].Name] = Nodes[i].Value.Token;
            }
        }

        public override void ReadOut(DeferredJsonSerializer serializer)
        {
            foreach (var property in Nodes)
            {

            }
        }
    }
}
