using System.Numerics;
using Robust.Shared.GameStates;

namespace Content.Shared._HL.Vacbed;

public sealed class InsideVacbedComponent
{
    [DataField("previousOffset")]
    public Vector2 PreviousOffset { get; set; } = new(0, 0);
}
