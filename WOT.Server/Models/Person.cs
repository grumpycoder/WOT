using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WOT.Server.Models
{
    [Table("Persons")]
    public class    Person
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(100)]
        public string Firstname { get; set; }
        [StringLength(100)]
        public string Lastname { get; set; }

        [StringLength(250)]
        public string EmailAddress { get; set; }
        [StringLength(8)]
        public string ZipCode { get; set; }
        
        public bool? IsDonor { get; set; }
        public bool? IsVIP { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", Firstname,Lastname);
        }
    }
}
