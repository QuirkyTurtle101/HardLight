using Content.Shared.Administration.Logs;
using Content.Shared.Body.Components;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Standing;
using Content.Shared.Stunnable;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.Serialization;

namespace Content.Shared._HL.Vacbed;

public sealed partial class SharedVacbedSystem
{
    [Dependency] private readonly SharedContainerSystem _containerSystem = default!;
    [Dependency] private readonly StandingStateSystem _standingStateSystem = default!;
    [Dependency] private readonly SharedPopupSystem _popupSystem = default!;
    [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearanceSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VacbedComponent, CanDropTargetEvent>(OnVacbedCanDropOn);
    }

    private void OnVacbedCanDropOn(EntityUid uid, VacbedComponent component, ref CanDropTargetEvent args)
    {
        if (args.Handled)
            return;

        args.CanDrop = HasComp<BodyComponent>(args.Dragged);
        args.Handled = true;
    }

    protected void UpdateAppearance(EntityUid uid, VacbedComponent? vacbed = null, AppearanceComponent? appearance = null)
    {
        if (!Resolve(uid, ref cryoPod))
            return;

        if (!Resolve(uid, ref appearance))
            return;

        _appearanceSystem.SetData(uid, VacbedComponent.VacbedVisuals.ContainsEntity, vacbed.BodyContainer.ContainedEntity == null, appearance);
    }

    protected void OnComponentInit(EntityUid uid, VacbedComponent vacbedComponent, ComponentInit args)
    {
        vacbedComponent.BodyContainer = _containerSystem.EnsureContainer<ContainerSlot>(uid, "insidebody");
    }

    public bool InsertBody(EntityUid uid, EntityUid target, VacbedComponent vacbedComponent)
    {
        if (vacbedComponent.BodyContainer.ContainedEntity != null)
        {
            return false;
        }
        if (!HasComp<MobStateComponent>(target))
        {
            return false;
        }

        var xform = Transform(target);
        _containerSystem.Insert((target, xform), vacbedComponent.BodyContainer);

        EnsureComp<InsideVacbedComponent>(target);
        _standingStateSystem.Stand(target, force: true);

        UpdateAppearance(uid, vacbedComponent);
        return true;
    }

    public void TryEjectBody(EntityUid uid, EntityUid userId, vacbedComponent? vacbedComponent)
    {
        if (!Resolve(uid, ref vacbedComponent))
        {
            return;
        }
        if (vacbedComponent.Locked)
        {
            _popupSystem.PopupEntity("locked placeholder", uid, UserId); //todo loc string
            return;
        }

        var ejected = EjectBody(uid, vacbedComponent);
        if (ejected != null)
        {
            //todo admin log
        }
    }

    public virtual EntityUid? EjectBody(EntityUid uid, VacbedComponent? vacbedComponent)
    {
        if (!Resolve(uid, ref vacbedComponent))
            return null;

        if (vacbedComponent.BodyContainer.ContainedEntity is not { Valid: true } contained)
            return null;

        _containerSystem.Remove(contained, vacbedComponent.BodyContainer);
        _standingStateSystem.Down(contained);

        UpdateAppearance(uid, vacbedComponent);
        return contained;
    }

    protected void AddAlternativeVerbs(EntityUid uid, VacbedComponent vacbedComponent, GetVerbsEvent<AlternativeVerb> args)
    {
        if (!args.CanAccess || !args.CanInteract)
            return;

        if(vacbedComponent.BodyContainer.ContainedEntity != null)
        {
            args.Verbs.Add(new AlternativeVerb
            {
                Text = "Eject", //todo loc string
                Category = VerbCategory.Eject,
                Priority = 1,
                Act = () => TryEjectBody(uid, args.User, vacbedComponent)
            });
        }
    }
}
