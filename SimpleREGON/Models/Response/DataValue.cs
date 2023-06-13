using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SimpleREGON.Models.Response;

internal class DataValue
{
    [JsonPropertyName("d")]
    public string? Data { get; set; }
}
