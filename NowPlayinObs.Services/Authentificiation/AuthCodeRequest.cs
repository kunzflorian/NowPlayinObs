using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NowPlayinObs.Authentificiation;

public class AuthCodeRequest() : AuthRequestBase("authorization_code")
{
    [JsonPropertyName("scope")]
    public required string Scope { get; set; }
    [JsonPropertyName("code")]
    public required string Code { get; set; }
    [JsonPropertyName("redirect_uri")]
    public required string RedirectUrl { get; set; }
}
