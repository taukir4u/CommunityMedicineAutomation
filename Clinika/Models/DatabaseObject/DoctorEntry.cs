using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Clinika.Models.DatabaseObject
{
    public class DoctorEntry
    {
        [Key]
        public int DoctorId { get; set; }

        
        [Display(Name = "Doctor Name")]
        public string Name { get; set; }
        [DisplayName("Degree")]
        public string Degree { get; set; }

        [DisplayName("Specialization")]
        public string Specialization { get; set; }
        public string ServiceCenterCode { set; get; }
      
    }

}