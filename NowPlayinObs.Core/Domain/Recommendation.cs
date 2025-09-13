using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NowPlayinObs.Domain;

public class Recommendation
{
    public string Name { get; set; } = string.Empty;
    public string ImageFile { get; set; } = string.Empty;
    public string? CheckUrl { get; set; }
    public string? CustomShoutText { get; set; }
    public bool IsFirst { get; set; }

    public override string ToString() => Name;
}
