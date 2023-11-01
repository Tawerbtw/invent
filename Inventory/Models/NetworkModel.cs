namespace Inventory.Models
{
    public class NetworkModel
    {
        public int Id { get; set; }
        public string Ip { get; set; }
        public string Mask { get; set; }
        public string Gateway { get; set; }
        public string Dns { get; set; }
    }
}
