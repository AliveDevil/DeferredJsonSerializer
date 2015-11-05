using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace de.alivedevil.Nodes
{
    public class ValueNode : Node
    {
        public object Value { get; set; }

        public override void WriteOut(DeferredJsonSerializer serializer, JTokenWriter writer)
        {
            writer.WriteValue(Value);
        }
    }
}
