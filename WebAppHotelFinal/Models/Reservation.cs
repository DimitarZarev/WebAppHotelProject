using System.ComponentModel.DataAnnotations;

namespace WebAppHotelFinal.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }
        public int RoomId {  get; set; }
        public Room? Room { get; set; }
        public int ClientId {  get; set; }
        public Client? Client { get; set; }
        public DateTime DateIn { get; set; }
        public DateTime DateOut { get; set; }
        public decimal TotalPrice {  get; set; }
    }
}
