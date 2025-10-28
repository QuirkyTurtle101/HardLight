using Content.Client.Medical.Cryogenics;
using Content.Shared._HL.Vacbed;
using Content.Shared.Medical.Cryogenics;
using Content.Shared.Verbs;
using Robust.Client.GameObjects;
using System.Numerics;
using DrawDepth = Content.Shared.DrawDepth.DrawDepth;

namespace Content.Client._HL.Vacbed;

public sealed class VacbedSystem : SharedVacbedSystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VacbedComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<VacbedComponent, GetVerbsEvent<AlternativeVerb>>(AddAlternativeVerbs);
        SubscribeLocalEvent<VacbedComponent, AppearanceChangeEvent>(OnAppearanceChange);
    }

    private void OnAppearanceChange(EntityUid uid, VacbedComponent component, ref AppearanceChangeEvent args)
    {
        //todo figure out appearances and finish this

        if (args.Sprite == null) { return; }

        if (!_appearance.TryGetData<bool>(uid, VacbedComponent.VacbedVisuals.ContainsEntity, out var isFull, args.Component))
        {
            return;
        }

        args.Sprite.LayerSetState(VacbedVisualLayers.Base, "atmos");
        args.Sprite.LayerSetState(VacbedVisualLayers.Door, "atmos_door");

        if (!isFull)
        {
            args.Sprite.LayerSetVisible(VacbedVisualLayers.Door, true);
            args.Sprite.DrawDepth = (int)DrawDepth.Mobs;
        }
        else
        {
            args.Sprite.LayerSetVisible(VacbedVisualLayers.Door, false);
            args.Sprite.DrawDepth = (int)DrawDepth.Objects;
        }
    }
}

public enum VacbedVisualLayers : byte
{
    Base,
    Door,
}
