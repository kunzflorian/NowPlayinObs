using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NowPlayinObs.Authentificiation;

public class DeviceCodeRequest
{
    [JsonPropertyName("client_id")]
    public required string ClientId { get; set; }
    [JsonPropertyName("scopes")]
    public required string Scope { get; set; }    
}

