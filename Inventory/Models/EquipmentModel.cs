namespace Inventory.Models
{
    public class EquipmentModel
    {
        public int Id { get; set; }
        public string EquipmentName { get; set; }
        public byte[] EquipmentImage { get; set; }
        public string InventoryNumber { get; set; }
        public string Auditorium { get; set; }
        public string ResponsibleUser { get; set; }
        public string TemporaryUser { get; set; }
        public decimal EquipmentCost { get; set; }
        public string EquipmentDirection { get; set; }
        public string EquipmentStatus { get; set; }
        public string EquipmentType { get; set; }
        public string EquipmentComment { get; set; }
    }
}
