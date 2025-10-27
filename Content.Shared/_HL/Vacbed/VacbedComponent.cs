using Robust.Shared.Containers;

namespace Content.Shared._HL.Vacbed;

[RegisterComponent]
public sealed partial class VacbedComponent : Component
{
    public ContainerSlot BodyContainer = default!;

    [DataField]
    public bool Locked { get; set; }

    [DataField("entryDelay")]
    public float EntryDelay = 2f;

    public enum VacbedVisuals : byte
    {
        ContainsEntity
    }
}
