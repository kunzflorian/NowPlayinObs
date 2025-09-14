using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NowPlayinObs.Authentification;

public class AuthCodeToken : BaseToken
{
    [JsonPropertyName("refresh_token")]
    public required string RefreshToken { get; set; }
    [JsonPropertyName("scope")]
    public required string[] Scope { get; set; }
}
