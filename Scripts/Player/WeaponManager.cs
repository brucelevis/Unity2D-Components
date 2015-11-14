﻿using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class WeaponManager : CacheBehaviour {

    private ProjectileManager projectile;
    private MeleeManager melee;
    private ArmAnimation arm;
    private Weapon equippedWeapon;

    void Start()
    {
        arm        = GetComponentInChildren<ArmAnimation>();
        projectile = transform.parent.GetComponent<ProjectileManager>();
        melee      = transform.parent.GetComponent<MeleeManager>();
    }

    // ACTION DISPATCHER
    public void ActionDispatcher(int action)
    {
        switch (equippedWeapon.weaponType)
        {
            // swinging weapons
            case Weapon.WeaponType.Axe:
            case Weapon.WeaponType.Sword:
            {
                SwingWeapon(action);
                break;
            }

            // hurled weapons
            case Weapon.WeaponType.Hammer:
            case Weapon.WeaponType.Dagger:
            {
                HurlProjectile(action);
                break;
            }

            // magic projectile weapons
            case Weapon.WeaponType.MagicProjectile:
            {
                CastMagicProjectile(action);
                break;
            }

            default:
            {
                Assert.IsTrue(false, "** Default Case Reached **");
                break;
            }
        }
    }

    // SWINGING WEAPONS — swords, axes, etc
    // mix & match animations and attacks for various activity states
    public void SwingWeapon(int action)
    {
        switch (action)
        {
            case IDLE:
            {
                equippedWeapon.PlayIdleAnimation(0, 0);
                arm.PlayIdleAnimation(0, 0);
                break;
            }

            case RUN:
            {
                equippedWeapon.PlayRunAnimation(0, 0);
                arm.PlayRunAnimation(0, 0);
                break;
            }

            case JUMP:
            {
                equippedWeapon.PlayJumpAnimation(0, 0);
                arm.PlayJumpAnimation(0, 0);
                break;
            }

            case FALL:
            {
                equippedWeapon.PlayJumpAnimation(0, 0);
                arm.PlayJumpAnimation(0, 0);
                break;
            }

            case ATTACK:
            {
                equippedWeapon.PlayAttackAnimation(0, 0);
                arm.PlayAttackAnimation(0, 0);
                melee.Attack(equippedWeapon);
                break;
            }

            case RUN_ATTACK:
            {
                equippedWeapon.PlayAttackAnimation(0, ONE_PIXEL);
                arm.PlayAttackAnimation(0, ONE_PIXEL);
                melee.Attack(equippedWeapon);
                break;
            }

            case JUMP_ATTACK:
            {
                equippedWeapon.PlayAttackAnimation(0, ONE_PIXEL * 2);
                arm.PlayAttackAnimation(0, ONE_PIXEL * 2);
                melee.Attack(equippedWeapon);
                break;
            }

            default:
            {
                Assert.IsTrue(false, "** Default Case Reached **");
                break;
            }
        }
    }

    // HURLED WEAPONS — hammers, daggers, etc
    // mix & match animations and attacks for various activity states
    public void HurlProjectile(int action)
    {
        switch (action)
        {
            case IDLE:
            {
                equippedWeapon.PlayIdleAnimation(0, 0);
                arm.PlayIdleAnimation(0, 0);
                break;
            }

            case RUN:
            {
                equippedWeapon.PlayRunAnimation(0, 0);
                arm.PlayRunAnimation(0, 0);
                break;
            }

            case JUMP:
            {
                equippedWeapon.PlayJumpAnimation(0, 0);
                arm.PlayJumpAnimation(0, 0);
                break;
            }

            case FALL:
            {
                equippedWeapon.PlayJumpAnimation(0, 0);
                arm.PlayJumpAnimation(0, 0);
                break;
            }

            case ATTACK:
            {
                equippedWeapon.PlayAttackAnimation(0, 0);
                arm.PlayHurlAnimation(0, 0);
                projectile.Fire(equippedWeapon);
                break;
            }

            case RUN_ATTACK:
            {
                equippedWeapon.PlayAttackAnimation(0, ONE_PIXEL);
                arm.PlayHurlAnimation(0, ONE_PIXEL);
                projectile.Fire(equippedWeapon);
                break;
            }

            case JUMP_ATTACK:
            {
                equippedWeapon.PlayAttackAnimation(0, ONE_PIXEL * 2);
                arm.PlayHurlAnimation(0, ONE_PIXEL * 2);
                projectile.Fire(equippedWeapon);
                break;
            }

            default:
            {
                Assert.IsTrue(false, "** Default Case Reached **");
                break;
            }
        }
    }

    // MAGIC PROJECTILE WEAPONS — fireballs, flaming skulls, etc
    // mix & match animations and attacks for various activity states
    public void CastMagicProjectile(int action)
    {
        switch (action)
        {
            case IDLE:
            {
                equippedWeapon.PlayIdleAnimation(0, 0);
                arm.PlayIdleAnimation(0, 0);
                break;
            }

            case RUN:
            {
                equippedWeapon.PlayRunAnimation(0, 0);
                arm.PlayRunAnimation(0, 0);
                break;
            }

            case JUMP:
            {
                equippedWeapon.PlayJumpAnimation(0, 0);
                arm.PlayJumpAnimation(0, 0);
                break;
            }

            case FALL:
            {
                equippedWeapon.PlayJumpAnimation(0, 0);
                arm.PlayJumpAnimation(0, 0);
                break;
            }

            case ATTACK:
            {
                equippedWeapon.PlayIdleAnimation(0, 0);
                arm.PlayAttackAnimation(0, 0);
                projectile.Fire(equippedWeapon);
                break;
            }

            case RUN_ATTACK:
            {
                equippedWeapon.PlayAttackAnimation(0, ONE_PIXEL);
                arm.PlayAttackAnimation(0, ONE_PIXEL);
                projectile.Fire(equippedWeapon);
                break;
            }

            case JUMP_ATTACK:
            {
                equippedWeapon.PlayAttackAnimation(0, ONE_PIXEL * 2);
                arm.PlayAttackAnimation(0, ONE_PIXEL * 2);
                projectile.Fire(equippedWeapon);
                break;
            }

            default:
            {
                Assert.IsTrue(false, "** Default Case Reached **");
                break;
            }
        }
    }

    void OnNewEquippedWeapon(Weapon newEquippedWeapon)
    {
        equippedWeapon = newEquippedWeapon;
    }

    void OnEnable()
    {
        Messenger.AddListener<Weapon>("new equipped weapon", OnNewEquippedWeapon);
    }

    void OnDestroy()
    {
        Messenger.RemoveListener<Weapon>("new equipped weapon", OnNewEquippedWeapon);
    }
}
