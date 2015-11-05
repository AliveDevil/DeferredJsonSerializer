using Newtonsoft.Json.Linq;

namespace de.alivedevil
{
    public abstract class Node
    {
        public JToken Token { get; set; }

        public object Reference { get; set; }

        public virtual void WriteOut(DeferredJsonSerializer serializer)
        {
        }

        public virtual void ReadOut(DeferredJsonSerializer serializer)
        {
        }
    }
}
