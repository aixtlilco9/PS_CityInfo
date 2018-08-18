using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Entities
{
    public class PointOfInterest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        [ForeignKey("CityId")]//to my belief this was added to show that if we did not want to name
        //the foreignkey in the convention base of xxxID we could add this and state wat it was 
        //the the 2 below propertes to link the two entities as pointofinterest is dependent on city 
        public City City { get; set; }

        public int CityId { get; set; }

    }
}
