using System.Numerics;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.Inventory;
using Content.Shared.Slippery;
using Content.Shared.Tag;
using Content.Shared.Throwing;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Map;

//linq
using System.Linq;
using Content.Shared.Examine;

namespace Content.Shared.Starlight.Knockback;

public abstract partial class SharedKnockbackSystem : EntitySystem
{
    [Dependency] private readonly TagSystem _tagSystem = default!;
    [Dependency] protected readonly SharedTransformSystem _transformSystem = default!;
    [Dependency] private readonly ThrowingSystem _throwing = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly StaminaSystem _stamina = default!;
    public override void Initialize()
    {
        SubscribeLocalEvent<KnockbackByUserTagComponent, GunShotEvent>(OnGunShot);
        SubscribeLocalEvent<KnockbackByUserTagComponent, ExaminedEvent>(OnExamined);
    }

    private void OnExamined(Entity<KnockbackByUserTagComponent> ent, ref ExaminedEvent args)
    {
        if (args.IsInDetailsRange)
        {
            //check if the examiner has any tags that match the component's tags
            if (GetKnockbackData(ent, args.Examiner, out KnockbackData data))
            {
                var knockback = CalculateKnockback(args.Examiner, data);
                //figure out the forwards/backwards direction
                var direction = knockback < 0 ? "forwards" : "backwards";
                args.PushMarkup(Loc.GetString("knockback-by-user-tag-component-examine-distance", ("knockback", String.Format("{0:0.###}", MathF.Abs(knockback))), ("direction", direction)));
                args.PushMarkup(Loc.GetString("knockback-by-user-tag-component-examine-stamina", ("stamina", String.Format("{0:0.###}", CalculateStaminaDamage(data, knockback)))));
            }
        }
    }

    private void OnGunShot(Entity<KnockbackByUserTagComponent> ent, ref GunShotEvent args)
    {
        //make sure the ammo is shootable
        foreach (var ammo in args.Ammo)
        {
            if (ammo.Uid != null && TryComp<CartridgeAmmoComponent>(ammo.Uid, out var cartridge))
            {
                //check if its spent
                if (cartridge.Spent)
                {
                    return;
                }
            }
        }

        EntityUid user = args.User;

        //check for tags
        if (GetKnockbackData(ent, user, out var data))
        {
            //get the gun component
            if (TryComp<GunComponent>(ent, out var gunComponent))
            {
                var toCoordinates = gunComponent.ShootCoordinates;

                if (toCoordinates == null)
                    return;

                var knockback = CalculateKnockback(user, data);

                if (knockback == 0.0f)
                    return;

                //make a clone, not a reference
                Vector2 modifiedCoords = toCoordinates.Value.Position;
                //flip the direction
                if (knockback > 0)
                    modifiedCoords = -modifiedCoords;

                //absolute knockback now
                knockback = Math.Abs(knockback);
                //normalize them
                modifiedCoords = Vector2.Normalize(modifiedCoords);
                //multiply by the knockback value
                modifiedCoords *= knockback;
                //set the new coordinates
                var flippedDirection = new EntityCoordinates(user, modifiedCoords);

                _throwing.TryThrow(user, flippedDirection, knockback * 5, user, 0, doSpin: false, compensateFriction: true);

                //deal stamina damage
                if (TryComp<StaminaComponent>(user, out var stamina))
                {
                    _stamina.TakeStaminaDamage(user, CalculateStaminaDamage(data, knockback), component: stamina);
                }
            }
        }
    }

    private bool GetKnockbackData(Entity<KnockbackByUserTagComponent> ent, EntityUid user, out KnockbackData data)
    {
        KnockbackData totalData = new();
        bool hadAnyMatches = false;
        //get all matching tags
        foreach (var tag in ent.Comp.DoestContain.Keys)
        {
            if (_tagSystem.HasTag(user, tag))
            {
                var tagdata = ent.Comp.DoestContain[tag];
                totalData.Knockback += tagdata.Knockback;
                totalData.StaminaMultiplier += tagdata.StaminaMultiplier;
                hadAnyMatches = true;
            }
        }

        data = totalData;

        return hadAnyMatches;
    }

    private float CalculateKnockback(EntityUid user, KnockbackData data)
    {
        float knockback = data.Knockback;
        //If we have no slips, cut the knockback in half
        if (CheckForNoSlips(user))
        {
            knockback *= 0.5f;
        }

        return knockback;
    }

    private static float CalculateStaminaDamage(KnockbackData data, float knockback) => MathF.Abs(knockback) * data.StaminaMultiplier;


    private bool CheckForNoSlips(EntityUid uid)
    {
        if (EntityManager.TryGetComponent(uid, out NoSlipComponent? flashImmunityComponent))
        {
            return true;
        }

        if (TryComp<InventoryComponent>(uid, out var inventoryComp))
        {
            //get all worn items
            var slots = _inventory.GetSlotEnumerator((uid, inventoryComp), SlotFlags.WITHOUT_POCKET);
            while (slots.MoveNext(out var slot))
            {
                if (slot.ContainedEntity != null && EntityManager.TryGetComponent(slot.ContainedEntity, out NoSlipComponent? wornNoSlipComponent))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
