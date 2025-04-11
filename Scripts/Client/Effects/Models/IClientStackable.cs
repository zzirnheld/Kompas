using Kompas.Effects.Models;

namespace Kompas.Client.Effects.Models;

/// <summary>
/// Marker interface.
/// Has at various points in history contained actual properties,
/// but I've usually soon after factored them out into shared client/server logic
/// </summary>
public interface IClientStackable : IStackable
{ }