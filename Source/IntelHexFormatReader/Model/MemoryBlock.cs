using System.Collections.Generic;

namespace IntelHexFormatReader.Model
{
    /// <summary>
    /// Logical representation of a MemoryBlock (an ordered collection of memory cells and registers).
    /// </summary>
    public class MemoryBlock
    {
        /// <summary>
        /// Memory cells in this memory block.
        /// </summary>
        public List<MemoryCell> Cells { get; set; }

        
        /// <summary>
        /// Construct a new MemoryBlock.
        /// </summary>
        /// <param name="memorySize">The size of the MemoryBlock to instantiate.</param>
        /// <param name="fillValue">Default cell initialization / fill value.</param>
        public MemoryBlock()
        {
            Cells = new List<MemoryCell>();
        }

        public void Add(MemoryCell cell)
        {
            this.Cells.Add(cell);
        }

        public List<MemoryCell> GetCells() { return this.Cells; }
    }
}
