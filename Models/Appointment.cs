using System.ComponentModel.DataAnnotations;

namespace StudioMgn.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
        [Phone]
        [Required]
        public string? Phone { get; set; }
        [Required]
        public AppointmentType Type { get; set; } = AppointmentType.ИНДИВИДУАЛЬНОЕ_ЗАНЯТИЕ;
        public string? Comment { get; set; }
        public string Description { get; set; }
    }
   public enum AppointmentType
    {
        ЗАПИСЬ,
        ИНДИВИДУАЛЬНОЕ_ЗАНЯТИЕ,
        УРОК,
        РЕПЕТИЦИЯ_ГРУППЫ
    }
}
