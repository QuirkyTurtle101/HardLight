//includes go here

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

    private void OnDragFinished(Entity<CryoPodComponent> entity, ref VacbedDragFinished args)
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
