using System;

namespace Inventory.Models
{
    public class InventoryModel
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Name { get; set; }
        public string InventoriedEquipment { get; set; }
        public string InventoryComment { get; set; }
        public string AuthorizedUser { get; set; }
    }
}
