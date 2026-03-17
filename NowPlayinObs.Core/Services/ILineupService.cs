using NowPlayinObs.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace NowPlayinObs.Services;

public interface ILineupService
{
    public Task<IEnumerable<Slot>> GetSlotsAsync();
}
