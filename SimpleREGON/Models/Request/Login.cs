using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SimpleREGON.Models.Request;

internal class Login
{
    [JsonPropertyName("pKluczUzytkownika")]
    public string Key { get; set; }
}
