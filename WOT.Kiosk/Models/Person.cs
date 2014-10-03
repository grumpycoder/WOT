using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WOT.Kiosk.Models
{
    [Table("Contributors")]
    public class Person
    {
        public int Id { get; set; }
        [StringLength(100)]
        public string Firstname { get; set; }
        [StringLength(100)]
        public string Lastname { get; set; }
        [StringLength(250)]
        public string EmailAddress { get; set; }
        [StringLength(8)]
        public string ZipCode { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", Firstname, Lastname);
        }
    }
}
