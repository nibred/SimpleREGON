using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimpleREGON.Services;

internal class DeserializationService
{
    internal async Task<string> DeserializeBaseRequestAsync(Stream response)
    {
        var resultDict = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(response);
        return resultDict?.GetValueOrDefault("d") ?? string.Empty;
    }
}
