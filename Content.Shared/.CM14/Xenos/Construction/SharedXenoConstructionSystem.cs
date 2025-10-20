using Content.Shared.Coordinates.Helpers;
using Content.Shared.Popups;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Network;

namespace Content.Shared.CM14.Xenos.Construction;

public abstract class SharedXenoConstructionSystem : EntitySystem
{
    [Dependency] private readonly IMapManager _map = default!;
    [Dependency] private readonly SharedMapSystem _mapSystem = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly XenoSystem _xeno = default!;

    public override void Initialize()
    {
        base.Initialize();

        Log.Info($"[XenoWeeds] ({(_net.IsServer ? "server" : "client")}) SharedXenoConstructionSystem.Initialize()");
        SubscribeLocalEvent<XenoComponent, XenoPlantWeedsEvent>(OnXenoPlantWeeds);
        SubscribeLocalEvent<XenoWeedsComponent, AnchorStateChangedEvent>(OnWeedsAnchorChanged);
    }

    private void OnXenoPlantWeeds(Entity<XenoComponent> ent, ref XenoPlantWeedsEvent args)
    {
        var coordinates = _transform.GetMoverCoordinates(ent).SnapToGrid(EntityManager, _map);
        Log.Info("[XenoWeeds] (" + (_net.IsServer ? "server" : "client") + ") Plant attempt by " + ToPrettyString(ent) + " at " + coordinates);

        // Do not spawn on the client. Server handler will perform authoritative spawn and set Handled.
        // On server, let the server-specific system handle it to avoid double-processing.
        if (_net.IsServer)
            return;
    }

    private void OnWeedsAnchorChanged(Entity<XenoWeedsComponent> ent, ref AnchorStateChangedEvent args)
    {
        if (!args.Anchored)
            QueueDel(ent);
    }
}
