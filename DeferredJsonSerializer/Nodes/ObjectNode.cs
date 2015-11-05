using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            Type referenceType = Reference.GetType();
            foreach (var property in Nodes)
            {
                property.Value.ReadOut(serializer);

                MemberInfo memberInfo = referenceType.GetMember(property.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty).FirstOrDefault();
                PropertyInfo propertyInfo = memberInfo as PropertyInfo;
                FieldInfo fieldInfo = memberInfo as FieldInfo;

                if (property.Value is ValueNode)
                {
                    if (propertyInfo != null)
                        propertyInfo.SetValue(Reference, property.Value.Token.ToObject(propertyInfo.PropertyType), null);
                    else if (fieldInfo != null)
                        fieldInfo.SetValue(Reference, property.Value.Token.ToObject(fieldInfo.FieldType));
                }
                else if (property.Value is ArrayNode)
                {
                    if (propertyInfo != null)
                    {
                        if (typeof(IList).IsAssignableFrom(propertyInfo.PropertyType))
                        {
                            IList list = (IList)propertyInfo.GetValue(Reference, null);
                            foreach (var item in ((ArrayList)property.Value.Reference))
                            {
                                list.Add(item);
                            }
                        }
                        else if (propertyInfo.PropertyType.IsArray && propertyInfo.CanWrite)
                        {
                            propertyInfo.SetValue(Reference, ((ArrayList)property.Value.Reference).ToArray(), null);
                        }
                    }
                    else if (fieldInfo != null)
                    {
                        if (typeof(IList).IsAssignableFrom(fieldInfo.FieldType))
                        {
                            IList list = (IList)fieldInfo.GetValue(Reference);
                            foreach (var item in ((ArrayList)property.Value.Reference))
                            {
                                list.Add(item);
                            }
                        }
                        else if (fieldInfo.FieldType.IsArray && !fieldInfo.IsInitOnly)
                        {
                            propertyInfo.SetValue(Reference, ((ArrayList)property.Value.Reference).ToArray(), null);
                        }
                    }
                }
                else
                {
                    if (propertyInfo != null)
                        propertyInfo.SetValue(Reference, property.Value.Reference, null);
                    else if (fieldInfo != null)
                        fieldInfo.SetValue(Reference, property.Value.Reference);
                }
            }
        }
    }
}
