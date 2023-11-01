using System;

namespace Inventory.Models
{
    public class MaterialsModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateAdd { get; set; }
        public byte[] Image { get; set; }
        public int Count { get; set; }
        public string ResponsibleUser { get; set; }
        public string TemporaryUser { get; set; }
        public string Type { get; set; }
    }
}
