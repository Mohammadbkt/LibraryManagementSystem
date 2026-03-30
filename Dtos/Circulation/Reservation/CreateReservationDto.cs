using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Dtos.Circulation.Reservation
{
    public class CreateReservationDto
    {
        public string UserId {get; set;} = string.Empty;
        public int BookId {get; set;}
    }
}