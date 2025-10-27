using Content.Shared.Standing;

namespace Content.Shared._HL.Vacbed;

public sealed partial class SharedVacbedSystem
{
    public virtual void InitializeInsideCryopod()
    {
        SubscribeLocalEvent<InsideVacbedComponent, DownAttemptEvent>(HandleDown);
        SubscribeLocalEvent<InsideVacbedComponent, EntGotRemovedFromContainerMessage>(OnEntGotRemovedFromContainer);
    }

    private void HandleDown(EntityUid uid, InsideVacbedComponent component, DownAttemptEvent args)
    {
        args.Cancel(); //keeps person inside standing
    }

    private void OnEntGotRemovedFromContainer(EntityUid uid, InsideVacbedComponent component, EntGotRemovedFromContainerMessage args)
    {
        if (Terminating(uid))
        {
            return;
        }

        RemComp<InsideVacbedComponent>(uid);
    }
}
