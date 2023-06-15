using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleREGON.Models.Request;

internal class GetFullData
{
    public string pNazwaRaportu { get; set; }
    public string pRegon { get; set; }
    public string pSilosID { get; set; }
}
