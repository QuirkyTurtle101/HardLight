using System.Numerics;
using Robust.Shared.GameStates;

namespace Content.Shared._HL.Vacbed;

public sealed partial class InsideVacbedComponent : Component
{
    [DataField("previousOffset")]
    public Vector2 PreviousOffset { get; set; } = new(0, 0);
}
