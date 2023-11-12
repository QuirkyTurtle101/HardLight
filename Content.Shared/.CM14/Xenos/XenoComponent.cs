using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Content.Shared.Damage;

namespace Content.Shared.CM14.Xenos;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class XenoComponent : Component
{
    // Evolution
    [DataField]
    public TimeSpan EvolveIn;

    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public List<EntProtoId> EvolvesTo = new();

    [DataField]
    public EntProtoId EvolveActionId = "ActionXenoEvolve";

    // Weeds prototype to spawn; use EntProtoId instead of a custom serializer to avoid missing type errors.
    [ViewVariables(VVAccess.ReadWrite), DataField("Weedprototype")]
    public EntProtoId Weedprototype = "XenoWeeds";
    [DataField]
    public EntityUid? EvolveAction;

    // Actions
    [DataField, AutoNetworkedField]
    public List<EntProtoId> ActionIds = new();

    [DataField]
    public Dictionary<EntProtoId, EntityUid> Actions = new();

    // Plasma/effects
    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public int Plasma;

    // Make MaxPlasma optional; default to 300 if not specified on the prototype.
    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public int MaxPlasma = 300;

    // PlasmaRegen was previously required across all prototypes, which caused
    // RequiredFieldNotMappedException for any xeno that didn't explicitly set it.
    // Make it optional with a conservative default (no passive regen) and let
    // specific prototypes override as needed.
    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public int PlasmaRegen = 0;

    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan PlasmaRegenCooldown = TimeSpan.FromSeconds(1);

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan NextPlasmaRegenTime;

    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public int? OriginalDrawDepth;

    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan AcidDelay = TimeSpan.FromSeconds(5);

    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public string DevourContainerId = "cm_xeno_devour";

    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan DevourDelay = TimeSpan.FromSeconds(5);

    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public SoundSpecifier RegurgitateSound = new SoundPathSpecifier("/Audio/.CM14/Xeno/alien_drool2.ogg");

    // Resting regen
    [DataField]
    public DamageSpecifier? RestHealing;

    [DataField]
    public float RestHealingCritMultiplier = 1.5f;

    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public EntProtoId TailAnimationId = "WeaponArcThrust";

    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public float TailRange = 3;

    [DataField]
    [ViewVariables(VVAccess.ReadWrite)]
    public DamageSpecifier TailDamage;

    [DataField]
    [ViewVariables(VVAccess.ReadWrite)]
    public SoundSpecifier TailHitSound = new SoundCollectionSpecifier("XenoTailSwipe");
}
