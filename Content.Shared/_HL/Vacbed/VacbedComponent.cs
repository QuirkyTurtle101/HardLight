using Robust.Shared.Containers;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._HL.Vacbed;

[RegisterComponent, NetworkedComponent]
public sealed partial class VacbedComponent : Component
{
    public ContainerSlot BodyContainer = default!;

    [ViewVariables]
    [DataField]
    public bool Locked { get; set; }

    [ViewVariables]
    [DataField("entryDelay")]
    public float EntryDelay = 2f;

    [Serializable, NetSerializable]
    public enum VacbedVisuals : byte
    {
        ContainsEntity
    }
}
