using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace de.alivedevil.Nodes
{
    public class ReferenceNode : Node
    {
        public override void WriteOut(DeferredJsonSerializer serializer)
        {
            ((JObject)Token)["$ref"] = serializer.ObjectIdLookup[Reference];
        }

        public override void ReadOut(DeferredJsonSerializer serializer)
        {
            Reference = serializer.IdObjectLookup[((JObject)Token)["$ref"].Value<int>()];
        }
    }
}
