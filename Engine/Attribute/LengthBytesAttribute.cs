namespace Engine.Attribute
{
    class LengthBytesAttribute : System.Attribute
    {
        public int Length { get; set; }

        public LengthBytesAttribute(int length)
        {
            Length = length;
        }
    }
}
