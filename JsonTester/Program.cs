using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using de.alivedevil;
using de.alivedevil.Attributes;

namespace JsonTester
{
    [Reference]
    public class Container
    {
        [KeepReference]
        public List<Item> Items { get; set; } = new List<Item>();
    }

    public class Depend
    {
        [KeepReference]
        public Item Item { get; set; }
    }

    [Reference]
    public class Item
    {
        [KeepReference]
        public Container Container { get; set; }

        public Depend Depend { get; set; }
    }

    internal static class Helper
    {
        public static T _<T>(this T t, Action<T> a)
        {
            a(t);
            return t;
        }
    }

    internal class Program
    {
        const string json = "{\"$id\": 0,\"$type\": \"JsonTester.Container, JsonTester\",\"Items\": [{\"$id\": 1,\"$type\": \"JsonTester.Item, JsonTester\",\"Container\": {\"$ref\": 0},\"Depend\": {\"$type\": \"JsonTester.Depend, JsonTester\",\"Item\": {\"$ref\": 1}}}]}";

        private static void Main(string[] args)
        {
            //Container container = new Container();
            //for (int i = 0; i < 5; i++)
            //{
            //    container.Items.Add(new Item()._(_ =>
            //    {
            //        _.Container = container;
            //        _.Depend = new Depend() { Item = _ };
            //    }));
            //}
            //DeferredJsonSerializer serializer = new DeferredJsonSerializer();
            //serializer.Serialize(container, Console.OpenStandardOutput());
            //Console.ReadLine();
            DeferredJsonSerializer serializer = new DeferredJsonSerializer();
            MemoryStream stream = new MemoryStream(Encoding.Default.GetBytes(json));
            Container container = serializer.Deserialize<Container>(stream);
            Console.ReadLine();
        }
    }
}
