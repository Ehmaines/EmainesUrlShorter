using EmainesUrlShorter.Domain.Entities;

namespace EmainesUrlShorter.Application.Interfaces;

public interface IClickTracker
{
    Task TrackAsync(LinkAccess access, CancellationToken cancellationToken = default);
}
