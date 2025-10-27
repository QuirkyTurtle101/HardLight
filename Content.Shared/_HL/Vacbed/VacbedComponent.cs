using Robust.Shared.Containers;

namespace Content.Shared._HL.Vacbed;

[RegisterComponent]
public sealed partial class VacbedComponent
{
    public ContainerSlot BodyContainer = default!;

    [DataField]
    public bool Locked { get; set; }

    public enum VacbedVisuals : byte
    {
        ContainsEntity
    }
}
