using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.Basic;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.Tank.PLDCombos.PLDCombo_Default;

namespace XIVAutoAttack.Combos.Tank.PLDCombos;

internal sealed class PLDCombo_Default : PLDCombo_Base<CommandType>
{
    public override string Author => "ϫ��Moon";

    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //д��ע�Ͱ���������ʾ�û��ġ�
    };


    protected override bool CanHealSingleSpell => TargetUpdater.PartyMembers.Length == 1 && base.CanHealSingleSpell;

    //private bool SlowLoop = false;

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.��������, $"{Clemency}"},
        {DescType.��Χ����, $"{DivineVeil}, {PassageofArms}"},
        {DescType.�������, $"{Sentinel}, {Sheltron}"},
        {DescType.�ƶ�����, $"{Intervene}"},
    };

    private protected override bool GeneralGCD(out IAction act)
    {
        //��������
        if (BladeofValor.ShouldUse(out act, mustUse: true)) return true;
        if (BladeofFaith.ShouldUse(out act, mustUse: true)) return true;
        if (BladeofTruth.ShouldUse(out act, mustUse: true)) return true;

        //ħ����������
        if (CanUseConfiteor(out act)) return true;

        //AOE ����
        if (Prominence.ShouldUse(out act)) return true;
        if (TotalEclipse.ShouldUse(out act)) return true;

        //���｣
        if (Atonement.ShouldUse(out act))
        {
            if (Player.HasStatus(true, StatusID.FightOrFlight)
                   && IsLastWeaponSkill(true, Atonement, RageofHalone)
                   && !Player.WillStatusEndGCD(2, 0, true, StatusID.FightOrFlight)) return true;

            if (Player.StatusStack(true, StatusID.SwordOath) > 1) return true;
        }
        //��������
        if (GoringBlade.ShouldUse(out act)) return true;
        if (RageofHalone.ShouldUse(out act)) return true;
        if (RiotBlade.ShouldUse(out act)) return true;
        if (FastBlade.ShouldUse(out act)) return true;

        //Ͷ��
        if (CommandController.Move && MoveAbility(1, out act)) return true;
        if (ShieldLob.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //��ͣ
        if (Intervene.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(out IAction act)
    {
        //���ʺ���
        if (Clemency.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //ʥ��Ļ��
        if (DivineVeil.ShouldUse(out act)) return true;

        //��װ����
        if (PassageofArms.ShouldUse(out act)) return true;

        if (Reprisal.ShouldUse(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak)
        {
            //ս�ӷ�Ӧ ��Buff
            if (abilityRemain == 1 && CanUseFightorFlight(out act)) return true;

            //������
            //if (SlowLoop && CanUseRequiescat(out act)) return true;
            if (abilityRemain == 1 && CanUseRequiescat(out act)) return true;
        }


        //������ת
        if (CircleofScorn.ShouldUse(out act, mustUse: true))
        {
            if (!IsFullParty) return true;

            if (FightorFlight.ElapsedAfterGCD(2)) return true;

            //if (SlowLoop && inOpener && IsLastWeaponSkill(false, Actions.RiotBlade)) return true;

            //if (!SlowLoop && inOpener && OpenerStatus && IsLastWeaponSkill(true, Actions.RiotBlade)) return true;

        }

        //���֮��
        if (SpiritsWithin.ShouldUse(out act, mustUse: true))
        {
            //if (SlowLoop && inOpener && IsLastWeaponSkill(true, Actions.RiotBlade)) return true;

            if (!IsFullParty) return true;

            if (FightorFlight.ElapsedAfterGCD(3)) return true;
        }

        //��ͣ
        if (Intervene.Target.DistanceToPlayer() < 1 && !IsMoving && Target.HasStatus(true, StatusID.GoringBlade))
        {
            if (FightorFlight.ElapsedAfterGCD(2) && Intervene.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

            if (Intervene.ShouldUse(out act)) return true;
        }

        //Special Defense.
        if (OathGauge == 100 && OathDefense(out act) && Player.CurrentHp < Player.MaxHp) return true;

        act = null;
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (OathDefense(out act)) return true;

        if (abilityRemain == 1)
        {
            //Ԥ��������30%��
            if (Sentinel.ShouldUse(out act)) return true;

            //���ڣ�����20%��
            if (Rampart.ShouldUse(out act)) return true;
        }
        //���͹���
        //ѩ��
        if (Reprisal.ShouldUse(out act)) return true;

        //��Ԥ������10%��
        if (!HaveShield && Intervention.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    /// <summary>
    /// �ж��ܷ�ʹ��ս�ӷ�Ӧ
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool CanUseFightorFlight(out IAction act)
    {
        if (FightorFlight.ShouldUse(out act))
        {
            //��4�˱�����
            if (!IsFullParty)
            {
                if (!Player.HasStatus(true, StatusID.Requiescat)
                    && !Player.HasStatus(true, StatusID.ReadyForBladeofFaith)
                    && Player.CurrentMp < 2000) return true;

                return false;
            }
            //�������ȷ潣��
            return true;
        }

        act = null;
        return false;
    }

    /// <summary>
    /// �ж��ܷ�ʹ�ð�����
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool CanUseRequiescat(out IAction act)
    {
        //������
        if (Requiescat.ShouldUse(out act, mustUse: true))
        {
            //��ս��buffʱ��ʣ17������ʱ�ͷ�
            if (Player.HasStatus(true, StatusID.FightOrFlight) && Player.WillStatusEnd(17, true, StatusID.FightOrFlight) && Target.HasStatus(true, StatusID.GoringBlade))
            {
                //��������ʱ,��Ȩ�����ͷ�
                return true;
            }
        }

        act = null;
        return false;
    }


    /// <summary>
    /// ����,ʥ��,ʥ��
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool CanUseConfiteor(out IAction act)
    {
        act = null;
        if (Player.HasStatus(true, StatusID.SwordOath)) return false;

        //�а�����buff,��û��ս����
        if (Player.HasStatus(true, StatusID.Requiescat) && !Player.HasStatus(true, StatusID.FightOrFlight))
        {
            //if (SlowLoop && !IsLastWeaponSkill(true, GoringBlade) && !IsLastWeaponSkill(true, Atonement)) return false;

            var statusStack = Player.StatusStack(true, StatusID.Requiescat);
            if (statusStack == 1 || Player.HasStatus(true, StatusID.Requiescat) && Player.WillStatusEnd(3, false, StatusID.Requiescat) || Player.CurrentMp <= 2000)
            {
                if (Confiteor.ShouldUse(out act, mustUse: true)) return true;
            }
            else
            {
                if (HolyCircle.ShouldUse(out act)) return true;
                if (HolySpirit.ShouldUse(out act)) return true;
            }
        }

        act = null;
        return false;
    }


    private bool OathDefense(out IAction act)
    {
        if (HaveShield)
        {
            //����
            if (Sheltron.ShouldUse(out act)) return true;
        }
        else
        {
            //����
            if (Cover.ShouldUse(out act)) return true;
        }

        return false;
    }
}
