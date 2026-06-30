using System.ComponentModel.DataAnnotations;

namespace WebAppHotelFinal.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(10)]
        public string NumberRoom { get; set; }=string.Empty;
        [Range(1,10)]
        public int Capacity { get; set; }
        public RoomType RoomType { get; set; }
        [Range(10, 1000)]
        public decimal Price { get; set; }

    }
    public enum RoomType
    {
        Single=1,
        Double=2,
        Apartament=3
    }
}
