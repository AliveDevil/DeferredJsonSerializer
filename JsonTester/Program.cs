using System;
using System.Collections.Generic;
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
        private static void Main(string[] args)
        {
            Container container = new Container();
            for (int i = 0; i < 10; i++)
            {
                container.Items.Add(new Item()._(_ =>
                {
                    _.Container = container;
                    _.Depend = new Depend() { Item = _ };
                }));
            }
            DeferredJsonSerializer serializer = new DeferredJsonSerializer();
            serializer.Serialize(container, Console.OpenStandardOutput());
            Console.ReadLine();
        }
    }
}
