using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using  library.Models.Enums;

namespace library.Models.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        
        public int ItemId { get; set; }
        
        public string UserId { get; set; } = string.Empty;
        public DateTime ReservationDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiryDate { get; set; }
        public DateTime? NotificationSentAt { get; set; }
        public DateTime? PickupDate { get; set; }
        
        public ReservationStatus Status { get; set; } = ReservationStatus.Waiting;
        public int QueuePosition { get; set; }
        
        public Item Item { get; set; } = null!;
        public User User { get; set; } = null!; 
        
        
    }
}