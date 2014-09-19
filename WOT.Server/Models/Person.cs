using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WOT.Server.Models
{
    [Table("Contributors")]
    public class Person
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(100)]
        public string Firstname { get; set; }
        [StringLength(100)]
        public string Lastname { get; set; }

        public bool? IsVIP { get; set; }
        public bool? IsDonor { get; set; }
        public int? Priority { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", Firstname, Lastname);
        }
    }
}
