using Dalamud.Game.ClientState.Objects.Types;
using System;
using System.Data;
using System.Linq;
using System.Numerics;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.RangedPhysicial;

namespace XIVAutoAttack.Combos.CustomCombo;

public abstract partial class CustomCombo
{
    private bool Ability(byte abilityRemain, IAction nextGCD, out IAction act, bool helpDefenseAOE, bool helpDefenseSingle)
    {
        if (Service.Configuration.OnlyGCD)
        {
            act = null;
            return false;
        }

        //��ĳЩ�ǳ�Σ�յ�״̬��
        if (JobID == 23)
        {
            if (IconReplacer.EsunaOrShield && TargetHelper.WeakenPeople.Length > 0 || TargetHelper.DyingPeople.Length > 0)
            {
                if (BRDCombo.Actions.WardensPaean.ShouldUse(out act, mustUse: true)) return true;
            }
        }


        if (EmergercyAbility(abilityRemain, nextGCD, out act)) return true;
        Role role = (Role)XIVAutoAttackPlugin.AllJobs.First(job => job.RowId == JobID).Role;

        if (TargetHelper.CanInterruptTargets.Length > 0)
        {
            switch (role)
            {
                case Role.����:
                    if (GeneralActions.Interject.ShouldUse(out act)) return true;
                    break;

                case Role.��ս:
                    if (GeneralActions.LegSweep.ShouldUse(out act)) return true;
                    break;
                case Role.Զ��:
                    if (RangePhysicial.Contains(Service.ClientState.LocalPlayer.ClassJob.Id))
                    {
                        if (GeneralActions.HeadGraze.ShouldUse(out act)) return true;
                    }
                    break;
            }
        }
        if (role == Role.����)
        {
            if (IconReplacer.RaiseOrShirk)
            {
                if (GeneralActions.Shirk.ShouldUse(out act)) return true;
                if (HaveShield && Shield.ShouldUse(out act)) return true;
            }

            if (IconReplacer.EsunaOrShield && Shield.ShouldUse(out act)) return true;

            var defenses = new uint[] { ObjectStatus.Grit, ObjectStatus.RoyalGuard, ObjectStatus.IronWill, ObjectStatus.Defiance };
            //Alive Tanks with shield.
            var defensesTanks = TargetHelper.AllianceTanks.Where(t => t.CurrentHp != 0 && t.StatusList.Select(s => s.StatusId).Intersect(defenses).Count() > 0);
            if (defensesTanks == null || defensesTanks.Count() == 0)
            {
                if (!HaveShield && Shield.ShouldUse(out act)) return true;
            }
        }

        if (IconReplacer.AntiRepulsion)
        {
            switch (role)
            {
                case Role.����:
                case Role.��ս:
                    if (GeneralActions.ArmsLength.ShouldUse(out act)) return true;
                    break;
                case Role.����:
                    if (GeneralActions.Surecast.ShouldUse(out act)) return true;
                    break;
                case Role.Զ��:
                    if (RangePhysicial.Contains(Service.ClientState.LocalPlayer.ClassJob.Id))
                    {
                        if (GeneralActions.ArmsLength.ShouldUse(out act)) return true;
                    }
                    else
                    {
                        if (GeneralActions.Surecast.ShouldUse(out act)) return true;
                    }
                    break;
            }
        }
        if (IconReplacer.EsunaOrShield && role == Role.��ս)
        {
            if (GeneralActions.TrueNorth.ShouldUse(out act)) return true;
        }


        if (IconReplacer.DefenseArea && DefenceAreaAbility(abilityRemain, out act)) return true;
        if (IconReplacer.DefenseSingle && DefenceSingleAbility(abilityRemain, out act)) return true;
        if (TargetHelper.HPNotFull || Service.ClientState.LocalPlayer.ClassJob.Id == 25)
        {
            if (ShouldUseHealAreaAbility(abilityRemain, out act)) return true;
            if (ShouldUseHealSingleAbility(abilityRemain, out act)) return true;
        }

        //����
        if (HaveHostileInRange)
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
                var haveTargets = TargetFilter.ProvokeTarget(TargetHelper.HostileTargets);
                if ((Service.Configuration.AutoProvokeForTank || TargetHelper.AllianceTanks.Length < 2) 
                    && haveTargets.Length != TargetHelper.HostileTargets.Length
                    || IconReplacer.BreakorProvoke)

                {
                    //��������
                    if (!HaveShield && Shield.ShouldUse(out act)) return true;
                    if (GeneralActions.Provoke.ShouldUse(out act, mustUse: true)) return true;
                }

                if (Service.Configuration.AutoDefenseForTank && HaveShield
                    && !Service.Configuration.NoDefenceAbility)
                {
                    //��ȺŹ��
                    if (TargetHelper.TarOnMeTargets.Length > 1 && !IsMoving)
                    {
                        if (GeneralActions.ArmsLength.ShouldUse(out act)) return true;
                        if (DefenceSingleAbility(abilityRemain, out act)) return true;
                    }

                    //��һ�����ң���Ҫ���ڶ��Ҹ����顣
                    if (TargetHelper.TarOnMeTargets.Length == 1)
                    {
                        var tar = TargetHelper.TarOnMeTargets[0];
                        if (TargetHelper.IsHostileTank)
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

        if (HaveHostileInRange && SettingBreak && BreakAbility(abilityRemain, out act)) return true;
        if (IconReplacer.Move && MoveAbility(abilityRemain, out act))
        {
            if (act is BaseAction b && TargetFilter.DistanceToPlayer(b.Target) > 5) return true;
        }


        //�ָ�/����
        switch (role)
        {
            case Role.����:
                if (Service.Configuration.AlwaysLowBlow &&
                    GeneralActions.LowBlow.ShouldUse(out act)) return true;
                break;
            case Role.��ս:
                if (GeneralActions.SecondWind.ShouldUse(out act)) return true;
                if (GeneralActions.Bloodbath.ShouldUse(out act)) return true;
                break;
            case Role.����:
                if (GeneralActions.LucidDreaming.ShouldUse(out act)) return true;
                break;
            case Role.Զ��:
                if (RangePhysicial.Contains(Service.ClientState.LocalPlayer.ClassJob.Id))
                {
                    if (GeneralActions.SecondWind.ShouldUse(out act)) return true;
                }
                else
                {
                    if (Service.ClientState.LocalPlayer.ClassJob.Id != 25
                        && GeneralActions.LucidDreaming.ShouldUse(out act)) return true;
                }
                break;
        }

        if (GeneralAbility(abilityRemain, out act)) return true;
        if (HaveHostileInRange && ForAttachAbility(abilityRemain, out act)) return true;
        return false;
    }

    private bool ShouldUseHealAreaAbility(byte abilityRemain, out IAction act)
    {
        act = null;
        return (IconReplacer.HealArea || CanHealAreaAbility) && HealAreaAbility(abilityRemain, out act);
    }

    private bool ShouldUseHealSingleAbility(byte abilityRemain, out IAction act)
    {
        act = null;
        return (IconReplacer.HealSingle || CanHealSingleAbility) && HealSingleAbility(abilityRemain, out act);
    }

    /// <summary>
    /// ����дһЩ���ڹ�������������ֻ�и����е��˵�ʱ��Ż���Ч��
    /// </summary>
    /// <param name="level"></param>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected abstract bool ForAttachAbility(byte abilityRemain, out IAction act);
    /// <summary>
    /// ����дһЩ������Ϊ�����GCD���ܶ�Ҫ��Ӧ����������
    /// </summary>
    /// <param name="level"></param>
    /// <param name="abilityRemain"></param>
    /// <param name="nextGCD"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (nextGCD is BaseAction action)
        {
            if ((Role)XIVAutoAttackPlugin.AllJobs.First(job => job.RowId == JobID).Role != Role.��ս &&
            action.Cast100 >= 50 && GeneralActions.Swiftcast.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

            if (Service.Configuration.AutoUseTrueNorth && abilityRemain == 1 && action.EnermyLocation != EnemyLocation.None && action.Target != null)
            {
                if (action.EnermyLocation != action.Target.FindEnemyLocation() && action.Target.HasLocationSide())
                {
                    if (GeneralActions.TrueNorth.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
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

    private protected virtual bool MoveAbility(byte abilityRemain, out IAction act)
    {
        act = null; return false;
    }

    /// <summary>
    /// �������Ƶ�������
    /// </summary>
    /// <param name="level"></param>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        act = null; return false;
    }

    private protected virtual bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        act = null; return false;
    }
    private protected virtual bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        act = null; return false;
    }
    /// <summary>
    /// ��Χ���Ƶ�������
    /// </summary>
    /// <param name="level"></param>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        act = null; return false;
    }

    private protected virtual bool BreakAbility(byte abilityRemain, out IAction act)
    {
        act = null; return false;
    }
}
