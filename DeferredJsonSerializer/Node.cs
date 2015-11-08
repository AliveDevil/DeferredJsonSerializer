using Newtonsoft.Json.Linq;

namespace de.alivedevil
{
    public abstract class Node
    {
        public object Reference { get; set; }

        public JToken Token { get; set; }

        public virtual void ReadOut(DeferredJsonSerializer serializer)
        {
        }

        public virtual void WriteOut(DeferredJsonSerializer serializer)
        {
        }
    }
}
