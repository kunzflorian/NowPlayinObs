using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NowPlayinObs.Authentificiation;

public class StatusResponse
{
    [JsonPropertyName("status")]
    public int Status { get; set; }
    [JsonPropertyName("message")]
    public required string Message { get; set; }

    public override string ToString()
    {
        return $"Status:{Status} Message: {Message}";
    }
}
