using Newtonsoft.Json.Linq;

namespace de.alivedevil
{
    public abstract class Node
    {
        public JToken Token { get; set; }

        public virtual void ReadOut(DeferredJsonSerializer serializer, JTokenReader reader)
        {
        }

        public virtual void WriteOut(DeferredJsonSerializer serializer, JTokenWriter writer)
        {
        }
    }
}
