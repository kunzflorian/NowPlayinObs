using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NowPlayinObs.Authentificiation;

public abstract class AuthRequestBase(string grantType)
{
    [JsonPropertyName("client_id")]
    public required string ClientId { get; set; }
    [JsonPropertyName("client_secret")]
    public required string ClientSecret { get; set; }
    [JsonPropertyName("grant_type")]
    public string GrantType { get; set; } = grantType;
}
