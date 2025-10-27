using Content.Server.Administration.Logs;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Piping.Components;
using Content.Server.Atmos.Piping.Unary.EntitySystems;
using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Server.Medical.Components;
using Content.Server.NodeContainer.EntitySystems;
using Content.Server.NodeContainer.NodeGroups;
using Content.Server.NodeContainer.Nodes;
using Content.Server.Temperature.Components;
using Content.Shared._HL.Vacbed;
using Content.Shared.Atmos;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Climbing.Systems;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.Emag.Systems;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.MedicalScanner;
using Content.Shared.Power;
using Content.Shared.UserInterface;
using Content.Shared.Verbs;
using static Content.Shared.Medical.Cryogenics.SharedCryoPodSystem;


namespace Content.Server._HL.Vacbed;

public sealed partial class VacbedSystem : SharedVacbedSystem
{
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly ClimbSystem _climbSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VacbedComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<VacbedComponent, GetVerbsEvent<AlternativeVerb>>(AddAlternativeVerbs);
        SubscribeLocalEvent<VacbedComponent, VacbedDragFinished>(OnDragFinished);
        SubscribeLocalEvent<VacbedComponent, DragDropTargetEvent>(HandleDragDropOn);
    }

    private void HandleDragDropOn(Entity<VacbedComponent> entity, ref DragDropTargetEvent args)
    {
        if (entity.Comp.BodyContainer.ContainedEntity != null)
            return;

        var doAfterArgs = new DoAfterArgs(EntityManager, args.User, entity.Comp.EntryDelay, new VacbedDragFinished(), entity, target: args.Dragged, used: entity)
        {
            BreakOnDamage = true,
            BreakOnMove = true,
            NeedHand = false,
        };
        _doAfterSystem.TryStartDoAfter(doAfterArgs);
        args.Handled = true;
    }

    private void OnDragFinished(Entity<VacbedComponent> entity, ref VacbedDragFinished args)
    {
        if (args.Cancelled || args.Handled || args.Args.Target == null)
            return;

        if (InsertBody(entity.Owner, args.Args.Target.Value, entity.Comp))
        {
            //todo adminlog
        }
        args.Handled = true;
    }

    public override EntityUid? EjectBody(EntityUid uid, VacbedComponent? vacbedComponent)
    {
        if (!Resolve(uid, ref vacbedComponent))
            return null;
        if (vacbedComponent.BodyContainer.ContainedEntity is not { Valid: true } contained)
            return null;
        base.EjectBody(uid, vacbedComponent);
        _climbSystem.ForciblySetClimbing(contained, uid);
        return contained;
    }
}
