namespace IntelHexFormatReader.Model
{
    /// <summary>
    /// Logical representation of a MemoryCell.
    /// </summary>
    public class MemoryCell
    {
        public uint Address { get; private set; }

        public byte Value { get; set; }

        public MemoryCell(uint address, byte value)
        {
            Address = address;
            Value = value;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1})", Address.ToString("X8"), Value.ToString("X2"));
        }
    }
}
