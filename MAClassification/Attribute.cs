using System.Collections.Generic;

namespace MAClassification
{
    public class Attribute
    {
        public string AttributeName { get; set; }
        public List<string> AttributeValues { get; set; }
        public bool IsUsed { get; set; }
    }
}
