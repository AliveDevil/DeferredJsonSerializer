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
        private Dictionary<int, object> idObjectLookup = new Dictionary<int, object>();
        private Dictionary<object, int> objectIdLookup = new Dictionary<object, int>();

        internal Dictionary<int, object> IdObjectLookup
        {
            get { return idObjectLookup; }
        }

        internal Dictionary<object, int> ObjectIdLookup
        {
            get { return objectIdLookup; }
        }

        public T Deserialize<T>(Stream stream) where T : new()
        {
            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                JToken token = JToken.ReadFrom(jsonReader);
                Node startNode = FindNode(token);
                Deserialize(startNode, token);
                startNode.ReadOut(this);
                return (T)((ObjectNode)startNode).Reference;
            }
        }

        public void Serialize<T>(T graph, Stream stream) where T : class, new()
        {
            Node startNode = FindNode(typeof(T));
            Serialize(startNode, typeof(T), graph, true);
            startNode.WriteOut(this);

            using (var streamWriter = new StreamWriter(stream))
            using (var jsonWriter = new JsonTextWriter(streamWriter) { Formatting = Formatting.None })
                startNode.Token.WriteTo(jsonWriter);
        }

        private static Type GetEnumerableType(Type type)
        {
            foreach (Type interfaceType in type.GetInterfaces())
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    return interfaceType.GetGenericArguments()[0];
            return null;
        }

        private void Deserialize(Node node, JToken token)
        {
            if (node is ArrayNode)
            {
                ((ArrayNode)node).Reference = new ArrayList();
                foreach (var item in (JArray)token)
                {
                    Node arrayNode = FindNode(item);
                    Deserialize(arrayNode, item);
                    ((ArrayNode)node).Nodes.Add(arrayNode);
                }
            }
            else if (node is ObjectNode)
            {
                Type graphType = Type.GetType(token["$type"].Value<string>());
                object graph = Activator.CreateInstance(graphType);
                if (node is ReferenceObjectNode)
                    IdObjectLookup[ObjectIdLookup[graph] = ((ReferenceObjectNode)node).Id = token["$id"].Value<int>()] = graph;
                ((ObjectNode)node).Reference = graph;

                foreach (var item in (JObject)token)
                {
                    if (item.Key == "$type" || item.Key == "$id") continue;

                    PropertyNode property = new PropertyNode() { Name = item.Key, Token = item.Value };
                    property.Value = FindNode(item.Value);
                    ((ObjectNode)node).Nodes.Add(property);
                    Deserialize(property.Value, property.Token);
                }
            }
        }

        private Node FindNode(JToken token)
        {
            if (token is JValue) return new ValueNode() { Token = token };
            else if (token is JArray) return new ArrayNode() { Token = token };
            else if (token is JObject)
                if (token["$type"] != null)
                    if (token["$id"] != null) return new ReferenceObjectNode() { Token = token };
                    else return new ObjectNode() { Token = token };
                else if (token["$ref"] != null) return new ReferenceNode() { Token = token };
            return null;
        }

        private Node FindNode(Type type)
        {
            if (type == typeof(string) || type == typeof(char)) return new ValueNode() { Token = JValue.CreateUndefined() };
            else if (type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong) || type == typeof(short) || type == typeof(int) || type == typeof(long)) return new ValueNode() { Token = JValue.CreateUndefined() };
            else if (type == typeof(float) || type == typeof(double)) return new ValueNode() { Token = JValue.CreateUndefined() };
            else if (type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(TimeSpan)) return new ValueNode() { Token = JValue.CreateUndefined() };
            else if (type == typeof(bool)) return new ValueNode() { Token = JValue.CreateUndefined() };
            else if (type.IsArray || GetEnumerableType(type) != null) return new ArrayNode() { Token = new JArray() };
            else if (type.IsClass || type.IsValueType)
                if (type.HasAttribute<ReferenceAttribute>()) return new ReferenceObjectNode() { Token = new JObject() };
                else return new ObjectNode() { Token = new JObject() };
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
                ((ValueNode)node).Reference = graph;
            }
            else if (node is ArrayNode)
            {
                IEnumerable ienumerable = (IEnumerable)graph;
                foreach (var item in ienumerable)
                {
                    Node itemNode;
                    bool keepReference = false;
                    if (keepReferences && item.GetType().HasAttribute<ReferenceAttribute>())
                    {
                        itemNode = new ReferenceNode() { Token = new JObject() };
                        keepReference = true;
                    }
                    else
                    {
                        itemNode = FindNode(item.GetType());
                    }
                    Serialize(itemNode, item.GetType(), item, keepReference);
                    ((ArrayNode)node).Nodes.Add(itemNode);
                    ((JArray)((ArrayNode)node).Token).Add(itemNode.Token);
                }
            }
            else if (node is ReferenceNode)
            {
                ((ReferenceNode)node).Reference = graph;
            }
            else if (node is ObjectNode)
            {
                if (node is ReferenceObjectNode)
                    IdObjectLookup[ObjectIdLookup[graph] = ((ReferenceObjectNode)node).Id = GetId()] = graph;
                ((ObjectNode)node).Reference = graph;

                var properties = graphType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                for (int i = 0; i < properties.Length; i++)
                {
                    bool keepReference = false;
                    PropertyInfo propertyInfo = properties[i];

                    if (propertyInfo.GetIndexParameters() != null && propertyInfo.GetIndexParameters().Length > 0)
                    {
                        continue;
                    }

                    var value = propertyInfo.GetValue(graph, null);
                    Type propertyType = propertyInfo.PropertyType;
                    PropertyNode property = new PropertyNode() { Name = propertyInfo.Name };

                    if (value == null)
                    {
                        property.Value = new ValueNode();
                    }
                    else
                    {
                        if (propertyInfo.HasAttribute<KeepReferenceAttribute>())
                        {
                            if (propertyType.HasAttribute<ReferenceAttribute>())
                            {
                                property.Value = new ReferenceNode() { Token = new JObject() };
                                keepReference = true;
                            }
                            else
                            {
                                property.Value = FindNode(propertyType);
                                Type enumerableType = GetEnumerableType(propertyType);
                                keepReference = enumerableType != null && enumerableType.HasAttribute<ReferenceAttribute>();
                            }
                        }
                        else
                        {
                            property.Value = FindNode(propertyType);
                        }
                        if (!(property.Value is ArrayNode) && !propertyInfo.CanWrite) continue;
                    }

                    Serialize(property.Value, propertyType, value, keepReference);
                    ((ObjectNode)node).Nodes.Add(property);
                }

                var fields = graphType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                for (int i = 0; i < fields.Length; i++)
                {
                    bool keepReference = false;
                    FieldInfo fieldInfo = fields[i];

                    var value = fieldInfo.GetValue(graph);
                    Type fieldType = fieldInfo.FieldType;
                    PropertyNode field = new PropertyNode() { Name = fieldInfo.Name };

                    if (value == null)
                    {
                        field.Value = new ValueNode();
                    }
                    else
                    {
                        if (fieldType.HasAttribute<KeepReferenceAttribute>())
                        {
                            if (fieldType.HasAttribute<ReferenceAttribute>())
                            {
                                field.Value = new ReferenceNode() { Token = new JObject() };
                                keepReference = true;
                            }
                            else
                            {
                                field.Value = FindNode(fieldType);
                                Type enumerableType = GetEnumerableType(fieldType);
                                keepReference = enumerableType != null && enumerableType.HasAttribute<ReferenceAttribute>();
                            }
                        }
                        else
                        {
                            field.Value = FindNode(fieldType);
                        }
                        if (!(field.Value is ArrayNode) && fieldInfo.IsInitOnly) continue;
                    }

                    Serialize(field.Value, fieldType, value, keepReference);
                    ((ObjectNode)node).Nodes.Add(field);
                }
            }
        }
    }
}
