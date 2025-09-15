using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NowPlayinObs.Authentificiation;

public class DeviceCode
{
    [JsonPropertyName("device_code")]
    public required string Code { get; set; }
    [JsonPropertyName("expires_in")]
    public required int ExpiresIn { get; set; }
    [JsonPropertyName("interval")]
    public required int Interval { get; set; }
    [JsonPropertyName("user_code")]
    public required string UserCode { get; set; }
    [JsonPropertyName("verification_uri")]
    public required string VericifactionUrl { get; set; }
}
