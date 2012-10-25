namespace Demo.CompositeField
{
    using System;

    public class Range
    {
        public Func<string, string> Name { get; set; }

        public Func<int, string> Value { get; set; }

        public int FloorValue { get; set; }

        public int CeilingValue { get; set; }
        
        public int Start { get; set; }

        public int End { get; set; }

        public bool IsFloorValue
        {
            get
            {
                return FloorValue == Start;
            }
        }        
        
        public bool IsCeilingValue
        {
            get
            {
                return CeilingValue == End;
            }
        }

        public bool IsRange
        {
            get
            {
                return Start == End;
            }
        }

    }
}