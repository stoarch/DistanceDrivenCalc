using System;

namespace DistanceDrivenCalc
{
    internal class JsonPropertyAttribute : Attribute
    {
        public string PropertyName { get; set; }
    }
}