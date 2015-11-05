using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using de.alivedevil.Attributes;
using de.alivedevil.Nodes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace de.alivedevil
{
    public class DeferredJsonSerializer
    {
        private int currentId = 0;

        internal Dictionary<int, object> IdObjectLookup { get; } = new Dictionary<int, object>();

        internal Dictionary<object, int> ObjectIdLookup { get; } = new Dictionary<object, int>();

        public T Deserialize<T>(Stream stream) where T : new()
        {
            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            using (var jtokenReader = new JTokenReader(JToken.ReadFrom(jsonReader)))
            {
                return default(T);
            }
        }

        public void Serialize<T>(T graph, Stream stream) where T : class, new()
        {
            Node startNode = FindNode(typeof(T));
            Serialize(startNode, typeof(T), graph, true);
            using (JTokenWriter writer = new JTokenWriter())
            {
                startNode.WriteOut(this, writer);

                using (var streamWriter = new StreamWriter(stream))
                using (var jsonWriter = new JsonTextWriter(streamWriter) { Formatting = Formatting.Indented })
                    writer.Token.WriteTo(jsonWriter);
            }
        }

        private static Type GetEnumerableType(Type type)
        {
            foreach (Type interfaceType in type.GetInterfaces())
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    return interfaceType.GetGenericArguments()[0];
            return null;
        }

        private Node FindNode(Type type)
        {
            if (type == typeof(string) || type == typeof(char)) return new ValueNode();
            else if (type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong) || type == typeof(short) || type == typeof(int) || type == typeof(long)) return new ValueNode();
            else if (type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(TimeSpan)) return new ValueNode();
            else if (type.IsArray || GetEnumerableType(type) != null) return new ArrayNode();
            else if (type.IsClass || type.IsValueType)
                if (type.HasAttribute<ReferenceAttribute>()) return new ReferenceObjectNode();
                else return new ObjectNode();
            else return null;
        }

        private int GetId()
        {
            return currentId++;
        }

        private void Serialize(Node node, Type graphType, object graph, bool keepReferences)
        {
            if (node is ValueNode)
            {
                ((ValueNode)node).Value = graph;
            }
            else if (node is ArrayNode)
            {
                IEnumerable ienumerable = (IEnumerable)graph;
                foreach (var item in ienumerable)
                {
                    bool keepReference = false;
                    Node itemNode;
                    if (keepReferences && item.GetType().HasAttribute<ReferenceAttribute>())
                    {
                        itemNode = new ReferenceNode();
                        keepReference = true;
                    }
                    else
                    {
                        itemNode = FindNode(item.GetType());
                    }
                    Serialize(itemNode, item.GetType(), item, keepReference);
                    ((ArrayNode)node).Nodes.Add(itemNode);
                }
            }
            else if (node is ReferenceNode)
            {
                ((ReferenceNode)node).Reference = graph;
            }
            else if (node is ObjectNode)
            {
                if (node is ReferenceObjectNode)
                    ObjectIdLookup[graph] = ((ReferenceObjectNode)node).Id = GetId();
                ((ObjectNode)node).Reference = graph;

                PropertyInfo[] properties = graphType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
                for (int i = 0; i < properties.Length; i++)
                {
                    PropertyInfo propertyInfo = properties[i];
                    Type propertyType = propertyInfo.PropertyType;
                    PropertyNode property = new PropertyNode() { Name = propertyInfo.Name };
                    bool keepReference = false;

                    if (propertyInfo.HasAttribute<KeepReferenceAttribute>())
                    {
                        if (propertyType.HasAttribute<ReferenceAttribute>())
                        {
                            property.Value = new ReferenceNode();
                            keepReference = true;
                        }
                        else
                        {
                            property.Value = FindNode(propertyType);
                            Type enumerableType = GetEnumerableType(propertyType);
                            keepReferences = enumerableType != null && enumerableType.HasAttribute<ReferenceAttribute>();
                        }
                    }
                    else
                    {
                        property.Value = FindNode(propertyType);
                    }
                    Serialize(property.Value, propertyType, propertyInfo.GetValue(graph, null), keepReference);

                    ((ObjectNode)node).Nodes.Add(property);
                }
            }
        }
    }
}
