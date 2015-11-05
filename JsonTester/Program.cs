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

    public struct Test
    {
        public int X;
        public int Y;
    }

    [Reference]
    public class Item
    {
        [KeepReference]
        public Container Container { get; set; }

        public Depend Depend { get; set; }

        public Test Test { get; set; }
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
        const string json = "{\"$id\":0,\"$type\":\"JsonTester.Container, JsonTester\",\"Items\":[{\"$id\":1,\"$type\":\"JsonTester.Item, JsonTester\",\"Container\":{\"$ref\":0},\"Depend\":{\"$type\":\"JsonTester.Depend, JsonTester\",\"Item\":{\"$ref\":1}},\"Test\":{\"$type\":\"JsonTester.Test, JsonTester\",\"X\":2136749948,\"Y\":73797350}},{\"$id\":2,\"$type\":\"JsonTester.Item, JsonTester\",\"Container\":{\"$ref\":0},\"Depend\":{\"$type\":\"JsonTester.Depend, JsonTester\",\"Item\":{\"$ref\":2}},\"Test\":{\"$type\":\"JsonTester.Test, JsonTester\",\"X\":1379563029,\"Y\":139796753}},{\"$id\":3,\"$type\":\"JsonTester.Item, JsonTester\",\"Container\":{\"$ref\":0},\"Depend\":{\"$type\":\"JsonTester.Depend, JsonTester\",\"Item\":{\"$ref\":3}},\"Test\":{\"$type\":\"JsonTester.Test, JsonTester\",\"X\":2139544086,\"Y\":1129007368}},{\"$id\":4,\"$type\":\"JsonTester.Item, JsonTester\",\"Container\":{\"$ref\":0},\"Depend\":{\"$type\":\"JsonTester.Depend, JsonTester\",\"Item\":{\"$ref\":4}},\"Test\":{\"$type\":\"JsonTester.Test, JsonTester\",\"X\":830835170,\"Y\":1726835406}},{\"$id\":5,\"$type\":\"JsonTester.Item, JsonTester\",\"Container\":{\"$ref\":0},\"Depend\":{\"$type\":\"JsonTester.Depend, JsonTester\",\"Item\":{\"$ref\":5}},\"Test\":{\"$type\":\"JsonTester.Test, JsonTester\",\"X\":891816016,\"Y\":1727474472}}]}";

        private static void Main(string[] args)
        {
            //Random random = new Random();
            //Container container = new Container();
            //for (int i = 0; i < 5; i++)
            //{
            //    container.Items.Add(new Item()._(_ =>
            //    {
            //        _.Test = new Test() { X = random.Next(), Y = random.Next() };
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
