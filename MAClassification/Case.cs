using System.Collections.Generic;

namespace MAClassification
{
    public class Case
    {
        public int Number { get; set; }
        public List<string> AttributesValuesList { get; set; }
        public string Result { get; set; }
    }
}
