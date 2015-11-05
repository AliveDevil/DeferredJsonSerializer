using Newtonsoft.Json.Linq;

namespace de.alivedevil.Nodes
{
    public class ReferenceObjectNode : ObjectNode
    {
        public int Id { get; set; }

        public override void WriteOut(DeferredJsonSerializer serializer)
        {
            JObject jObject = (JObject)Token;
            jObject["$id"] = Id;
            base.WriteOut(serializer);
        }
    }
}
