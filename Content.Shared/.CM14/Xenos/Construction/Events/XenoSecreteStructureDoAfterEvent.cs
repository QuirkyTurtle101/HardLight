﻿using Content.Shared.DoAfter;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.CM14.Xenos.Construction.Events;

[Serializable, NetSerializable]
public sealed partial class XenoSecreteStructureDoAfterEvent : DoAfterEvent
{
    public NetCoordinates Coordinates;
    public EntProtoId StructureId;

    public XenoSecreteStructureDoAfterEvent(NetCoordinates coordinates, EntProtoId structureId)
    {
        Coordinates = coordinates;
        StructureId = structureId;
    }

    public override DoAfterEvent Clone()
    {
        return this;
    }
}
