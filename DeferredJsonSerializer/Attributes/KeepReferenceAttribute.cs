using System;

namespace de.alivedevil.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class KeepReferenceAttribute : Attribute
    {
    }
}
