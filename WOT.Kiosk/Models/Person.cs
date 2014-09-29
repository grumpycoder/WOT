using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WOT.Kiosk.Models
{
  public  class Person
    {
      public string Firstname { get; set; }
      public string Lastname { get; set; }
      public string EmailAddress { get; set; }
      public string ZipCode { get; set; }

      public override string ToString()
      {
          return string.Format("{0} {1}", Firstname, Lastname);
      }
    }
}
