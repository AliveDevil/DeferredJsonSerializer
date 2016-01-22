using System;

namespace de.alivedevil.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class ReferenceAttribute : Attribute
    {
    }
}
