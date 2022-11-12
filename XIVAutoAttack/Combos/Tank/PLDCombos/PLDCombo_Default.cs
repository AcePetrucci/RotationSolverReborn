using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Attributes;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.Tank.PLDCombos.PLDCombo_Default;

namespace XIVAutoAttack.Combos.Tank.PLDCombos;

[ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/XIVAutoAttack/Combos/Tank/PLDCombos/PLDCombo_Default.cs")]
internal sealed class PLDCombo_Default : PLDCombo<CommandType>
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

    /// <summary>
    /// ��4�˱��ĵ����Ѿ��ۺùֿ���ʹ����ؼ���(���ƶ�������д���3ֻС��)
    /// </summary>
    private static bool CanUseSpellInDungeonsMiddle => TargetUpdater.PartyMembers.Length is > 1 and <= 4 && !Target.IsBoss() && !IsMoving
                                                    && TargetFilter.GetObjectInRadius(TargetUpdater.HostileTargets, 5).Length >= 3;

    /// <summary>
    /// ��4�˱��ĵ���
    /// </summary>
    private static bool InDungeonsMiddle => TargetUpdater.PartyMembers.Length is > 1 and <= 4 && !Target.IsBoss();

    private bool SlowLoop = false;

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
            if (!SlowLoop && Player.HaveStatus(StatusIDs.FightOrFlight)
                   && IsLastWeaponSkill(true, Atonement, RoyalAuthority)
                   && !Player.WillStatusEndGCD(2, 0, true, StatusIDs.FightOrFlight)) return true;
            if (!SlowLoop && Player.FindStatusStack(StatusIDs.SwordOath) > 1) return true;

            if (SlowLoop) return true;
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
            if (abilityRemain == 1 && CanUseFightorFlight(out act))
            {
                return true;
            }

            //������
            //if (SlowLoop && CanUseRequiescat(out act)) return true;
            if (abilityRemain == 1 && CanUseRequiescat(out act)) return true;
        }


        //������ת
        if (CircleofScorn.ShouldUse(out act, mustUse: true))
        {
            if (InDungeonsMiddle) return true;

            if (FightorFlight.ElapsedAfterGCD(2)) return true;

            //if (SlowLoop && inOpener && IsLastWeaponSkill(false, Actions.RiotBlade)) return true;

            //if (!SlowLoop && inOpener && OpenerStatus && IsLastWeaponSkill(true, Actions.RiotBlade)) return true;

        }

        //���֮��
        if (SpiritsWithin.ShouldUse(out act, mustUse: true))
        {
            //if (SlowLoop && inOpener && IsLastWeaponSkill(true, Actions.RiotBlade)) return true;

            if (InDungeonsMiddle) return true;

            if (FightorFlight.ElapsedAfterGCD(3)) return true;
        }

        //��ͣ
        if (Intervene.Target.DistanceToPlayer() < 1 && !IsMoving && Target.HaveStatus(StatusIDs.GoringBlade))
        {
            if (FightorFlight.ElapsedAfterGCD(2) && Intervene.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

            if (Intervene.ShouldUse(out act)) return true;
        }

        //Special Defense.
        if (JobGauge.OathGauge == 100 && Defense(out act) && Player.CurrentHp < Player.MaxHp) return true;

        act = null;
        return false;
    }
    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //��ʥ���� ���л�����ˡ�
        if (HallowedGround.ShouldUse(out act)) return true;
        return false;
    }
    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (Defense(out act)) return true;

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
            if (InDungeonsMiddle)
            {
                if (CanUseSpellInDungeonsMiddle && !Player.HaveStatus(StatusIDs.Requiescat)
                    && !Player.HaveStatus(StatusIDs.ReadyForBladeofFaith)
                    && Player.CurrentMp < 2000) return true;

                return false;
            }

            if (SlowLoop)
            {
                //if (openerFinished && Actions.Requiescat.ElapsedAfterGCD(12)) return true;

            }
            else
            {
                //�������ȷ潣��
                return true;

            }


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
            //��4�˱�����
            if (InDungeonsMiddle)
            {
                if (CanUseSpellInDungeonsMiddle) return true;

                return false;
            }

            //��ѭ��
            if (SlowLoop)
            {
                //if (inOpener && IsLastWeaponSkill(true, Actions.FastBlade)) return true;

                //if (openerFinished && Actions.FightorFlight.ElapsedAfterGCD(12)) return true;
            }
            else
            {
                //��ս��buffʱ��ʣ17������ʱ�ͷ�
                if (Player.HaveStatus(StatusIDs.FightOrFlight) && Player.WillStatusEnd(17, false, StatusIDs.FightOrFlight) && Target.HaveStatus(StatusIDs.GoringBlade))
                {
                    //��������ʱ,��Ȩ�����ͷ�
                    return true;
                }
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
        if (Player.HaveStatus(StatusIDs.SwordOath)) return false;

        //�а�����buff,��û��ս����
        if (Player.HaveStatus(StatusIDs.Requiescat) && !Player.HaveStatus(StatusIDs.FightOrFlight))
        {
            if (SlowLoop && !IsLastWeaponSkill(true, GoringBlade) && !IsLastWeaponSkill(true, Atonement)) return false;

            var statusStack = Player.FindStatusStack(StatusIDs.Requiescat);
            if (statusStack == 1 || Player.HaveStatus(StatusIDs.Requiescat) && Player.WillStatusEnd(3, false, StatusIDs.Requiescat) || Player.CurrentMp <= 2000)
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

    private bool Defense(out IAction act)
    {
        act = null;
        if (JobGauge.OathGauge < 50) return false;

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
