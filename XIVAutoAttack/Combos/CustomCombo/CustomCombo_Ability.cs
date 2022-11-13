using System;
using System.Data;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.RangedPhysicial;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.CustomCombo;

internal abstract partial class CustomCombo<TCmd> where TCmd : Enum
{
    private bool Ability(byte abilityRemain, IAction nextGCD, out IAction act, bool helpDefenseAOE, bool helpDefenseSingle)
    {
        if (Service.Configuration.OnlyGCD || Player.TotalCastTime - Player.CurrentCastTime > Service.Configuration.WeaponInterval)
        {
            act = null;
            return false;
        }

        if (EmergercyAbility(abilityRemain, nextGCD, out act)) return true;
        Role role = Role;

        if (TargetUpdater.CanInterruptTargets.Length > 0)
        {
            switch (role)
            {
                case Role.����:
                    if (Interject.ShouldUse(out act)) return true;
                    break;

                case Role.��ս:
                    if (LegSweep.ShouldUse(out act)) return true;
                    break;
                case Role.Զ��:
                    if (RangePhysicial.Contains((ClassJobID)Service.ClientState.LocalPlayer.ClassJob.Id))
                    {
                        if (HeadGraze.ShouldUse(out act)) return true;
                    }
                    break;
            }
        }
        if (role == Role.����)
        {
            if (CommandController.RaiseOrShirk)
            {
                if (Shirk.ShouldUse(out act)) return true;
                if (HaveShield && Shield.ShouldUse(out act)) return true;
            }

            if (CommandController.EsunaOrShield && Shield.ShouldUse(out act)) return true;

            if (Service.Configuration.AutoShield)
            {
                var defenses = new StatusID[] { StatusID.Grit, StatusID.RoyalGuard, StatusID.IronWill, StatusID.Defiance };
                //Alive Tanks with shield.
                var defensesTanks = TargetUpdater.AllianceTanks.Where(t => t.CurrentHp != 0 && t.HaveStatus(true, defenses));
                if (defensesTanks == null || defensesTanks.Count() == 0)
                {
                    if (!HaveShield && Shield.ShouldUse(out act)) return true;
                }
            }
        }

        if (CommandController.AntiRepulsion)
        {
            switch (role)
            {
                case Role.����:
                case Role.��ս:
                    if (ArmsLength.ShouldUse(out act)) return true;
                    break;
                case Role.����:
                    if (Surecast.ShouldUse(out act)) return true;
                    break;
                case Role.Զ��:
                    if (RangePhysicial.Contains((ClassJobID)Service.ClientState.LocalPlayer.ClassJob.Id))
                    {
                        if (ArmsLength.ShouldUse(out act)) return true;
                    }
                    else
                    {
                        if (Surecast.ShouldUse(out act)) return true;
                    }
                    break;
            }
        }
        if (CommandController.EsunaOrShield && role == Role.��ս)
        {
            if (TrueNorth.ShouldUse(out act)) return true;
        }


        if (CommandController.DefenseArea && DefenceAreaAbility(abilityRemain, out act)) return true;
        if (CommandController.DefenseSingle && DefenceSingleAbility(abilityRemain, out act)) return true;
        if (TargetUpdater.HPNotFull || Service.ClientState.LocalPlayer.ClassJob.Id == 25)
        {
            if (ShouldUseHealAreaAbility(abilityRemain, out act)) return true;
            if (ShouldUseHealSingleAbility(abilityRemain, out act)) return true;
        }

        //����
        if (HaveHostilesInRange)
        {
            //��AOE
            if (helpDefenseAOE && !Service.Configuration.NoDefenceAbility)
            {
                if (DefenceAreaAbility(abilityRemain, out act)) return true;
                if (role == Role.��ս || role == Role.Զ��)
                {
                    //����
                    if (DefenceSingleAbility(abilityRemain, out act)) return true;
                }
            }

            //������
            if (role == Role.����)
            {
                var haveTargets = TargetFilter.ProvokeTarget(TargetUpdater.HostileTargets);
                if ((Service.Configuration.AutoProvokeForTank || TargetUpdater.AllianceTanks.Length < 2) 
                    && haveTargets.Length != TargetUpdater.HostileTargets.Length
                    || CommandController.BreakorProvoke)

                {
                    //��������
                    if (!HaveShield && Shield.ShouldUse(out act)) return true;
                    if (Provoke.ShouldUse(out act, mustUse: true)) return true;
                }

                if (Service.Configuration.AutoDefenseForTank && HaveShield
                    && !Service.Configuration.NoDefenceAbility)
                {
                    //��ȺŹ��
                    if (TargetUpdater.TarOnMeTargets.Length > 1 && !IsMoving)
                    {
                        if (ArmsLength.ShouldUse(out act)) return true;
                        if (DefenceSingleAbility(abilityRemain, out act)) return true;
                    }

                    //��һ�����ң���Ҫ���ڶ��Ҹ����顣
                    if (TargetUpdater.TarOnMeTargets.Length == 1)
                    {
                        var tar = TargetUpdater.TarOnMeTargets[0];
                        if (TargetUpdater.IsHostileTank)
                        {
                            //����
                            if (DefenceSingleAbility(abilityRemain, out act)) return true;
                        }
                    }
                }
            }

            //��������
            if (helpDefenseSingle && DefenceSingleAbility(abilityRemain, out act)) return true;
        }

        if (CommandController.Move && MoveAbility(abilityRemain, out act))
        {
            if (act is BaseAction b && TargetFilter.DistanceToPlayer(b.Target) > 5) return true;
        }


        //�ָ�/����
        switch (role)
        {
            case Role.����:
                if (Service.Configuration.AlwaysLowBlow &&
                    LowBlow.ShouldUse(out act)) return true;
                break;
            case Role.��ս:
                if (SecondWind.ShouldUse(out act)) return true;
                if (Bloodbath.ShouldUse(out act)) return true;
                break;
            case Role.����:
                if (LucidDreaming.ShouldUse(out act)) return true;
                break;
            case Role.Զ��:
                if (RangePhysicial.Contains((ClassJobID)Service.ClientState.LocalPlayer.ClassJob.Id))
                {
                    if (SecondWind.ShouldUse(out act)) return true;
                }
                else
                {
                    if (Service.ClientState.LocalPlayer.ClassJob.Id != 25
                        && LucidDreaming.ShouldUse(out act)) return true;
                }
                break;
        }

        if (GeneralAbility(abilityRemain, out act)) return true;
        if (HaveHostilesInRange && AttackAbility(abilityRemain, out act)) return true;
        return false;
    }

    private bool ShouldUseHealAreaAbility(byte abilityRemain, out IAction act)
    {
        act = null;
        return (CommandController.HealArea || CanHealAreaAbility) && ActionUpdater.InCombat && HealAreaAbility(abilityRemain, out act);
    }

    private bool ShouldUseHealSingleAbility(byte abilityRemain, out IAction act)
    {
        act = null;
        return (CommandController.HealSingle || CanHealSingleAbility) && ActionUpdater.InCombat && HealSingleAbility(abilityRemain, out act);
    }

    /// <summary>
    /// ����дһЩ���ڹ�������������ֻ�и����е��˵�ʱ��Ż���Ч��
    /// </summary>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected abstract bool AttackAbility(byte abilityRemain, out IAction act);
    /// <summary>
    /// ����дһЩ������Ϊ�����GCD���ܶ�Ҫ��Ӧ����������
    /// </summary>
    /// <param name="abilityRemain"></param>
    /// <param name="nextGCD"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (nextGCD is BaseAction action)
        {
            if (Role != Role.��ս &&
            action.CastTime >= 5 && Swiftcast.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

            if (Service.Configuration.AutoUseTrueNorth && abilityRemain == 1 && action.EnermyLocation != EnemyLocation.None && action.Target != null)
            {
                if (action.EnermyLocation != action.Target.FindEnemyLocation() && action.Target.HasLocationSide())
                {
                    if (TrueNorth.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
                }
            }
        }

        act = null;
        return false;
    }

    /// <summary>
    /// �������������ɶʱ����ʹ�á�
    /// </summary>
    /// <param name="level"></param>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool GeneralAbility(byte abilityRemain, out IAction act)
    {
        act = null; return false;
    }

    /// <summary>
    /// �ƶ�������
    /// </summary>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool MoveAbility(byte abilityRemain, out IAction act)
    {
        act = null; return false;
    }

    /// <summary>
    /// �������Ƶ�������
    /// </summary>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        act = null; return false;
    }

    /// <summary>
    /// �������������
    /// </summary>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        act = null; return false;
    }
    /// <summary>
    /// ��Χ����������
    /// </summary>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        act = null; return false;
    }
    /// <summary>
    /// ��Χ���Ƶ�������
    /// </summary>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        act = null; return false;
    }
}
