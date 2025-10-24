using Content.Shared.DoAfter;
using Content.Shared.Humanoid.Markings;
using Robust.Shared.Serialization;

namespace Content.Shared.Floofstation.ModifyUndies;

[Serializable, NetSerializable]
public sealed partial class ModifyUndiesDoAfterEvent : SimpleDoAfterEvent
{
    [DataField(required: true)]
    public Marking Marking;

    [DataField(required: true)]
    public string MarkingPrototypeName;

    [DataField(required: true)]
    public bool IsVisible;

    public ModifyUndiesDoAfterEvent(Marking marking, string markingPrototypeName, bool isVisible)
    {
        Marking = marking;
        MarkingPrototypeName = markingPrototypeName;
        IsVisible = isVisible;
    }
}
