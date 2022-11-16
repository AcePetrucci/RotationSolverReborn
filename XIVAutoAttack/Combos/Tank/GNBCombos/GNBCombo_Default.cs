using Dalamud.Game.ClientState.JobGauge.Types;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Attributes;
using XIVAutoAttack.Combos.Basic;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.Tank.GNBCombos.GNBCombo_Default;

namespace XIVAutoAttack.Combos.Tank.GNBCombos;

[ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/XIVAutoAttack/Combos/Tank/GNBCombos/GNBCombo_Default.cs")]
internal sealed class GNBCombo_Default : GNBCombo_Base<CommandType>
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
    protected override bool CanHealSingleSpell => false;
    protected override bool CanHealAreaSpell => false;

    /// <summary>
    /// ��4�˱��ĵ����Ѿ��ۺùֿ���ʹ����ؼ���(���ƶ�������д���3ֻС��)
    /// </summary>
    private static bool CanUseSpellInDungeonsMiddle => TargetUpdater.PartyMembers.Length is > 1 and <= 4 && !Target.IsBoss() && !IsMoving
                                                    && TargetFilter.GetObjectInRadius(TargetUpdater.HostileTargets, 5).Length >= 3;

    /// <summary>
    /// ��4�˱��ĵ���
    /// </summary>
    private static bool InDungeonsMiddle => TargetUpdater.PartyMembers.Length is > 1 and <= 4 && !Target.IsBoss();

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.��������, $"{Aurora}"},
        {DescType.��Χ����, $"{HeartofLight}"},
        {DescType.�������, $"{HeartofStone}, {Nebula}, {Camouflage}"},
        {DescType.�ƶ�����, $"{RoughDivide}"},
    };

    private protected override bool GeneralGCD(out IAction act)
    {
        //����
        if (CanUseGnashingFang(out act)) return true;

        //������
        if (CanUseSonicBreak(out act)) return true;

        //����
        if (CanUseDoubleDown(out act)) return true;

        //���������
        if (WickedTalon.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        if (SavageClaw.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //����֮�� AOE
        if (FatedCircle.ShouldUse(out act)) return true;

        //������   
        if (CanUseBurstStrike(out act)) return true;

        //AOE
        if (DemonSlaughter.ShouldUse(out act)) return true;
        if (DemonSlice.ShouldUse(out act)) return true;

        //��������
        //�������ʣ0.5����ȴ��,���ͷŻ�������,��Ҫ��Ϊ���ٲ�ͬ���ܻ�ʹ�����Ӻ�̫�������ж�һ��
        if (GnashingFang.IsCoolDown && GnashingFang.WillHaveOneCharge(0.5f) && GnashingFang.EnoughLevel) return false;
        if (SolidBarrel.ShouldUse(out act)) return true;
        if (BrutalShell.ShouldUse(out act)) return true;
        if (KeenEdge.ShouldUse(out act)) return true;

        if (CommandController.Move && MoveAbility(1, out act)) return true;

        if (LightningShot.ShouldUse(out act))
        {
            if (InDungeonsMiddle && LightningShot.Target.DistanceToPlayer() > 3) return true;
        }
        return false;
    }



    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        //����,Ŀǰֻ��4GCD���ֵ��ж�
        if (SettingBreak && abilityRemain == 1 && CanUseNoMercy(out act)) return true;

        //Σ������
        if (DangerZone.ShouldUse(out act))
        {
            if (InDungeonsMiddle) return true;

            //�ȼ�С������,
            if (!GnashingFang.EnoughLevel && (Player.HasStatus(true, StatusID.NoMercy) || !NoMercy.WillHaveOneCharge(15))) return true;

            //������,����֮��
            if (Player.HasStatus(true, StatusID.NoMercy) && GnashingFang.IsCoolDown) return true;

            //�Ǳ�����
            if (!Player.HasStatus(true, StatusID.NoMercy) && !GnashingFang.WillHaveOneCharge(20)) return true;
        }

        //���γ岨
        if (CanUseBowShock(out act)) return true;

        //����
        if (JugularRip.ShouldUse(out act)) return true;
        if (AbdomenTear.ShouldUse(out act)) return true;
        if (EyeGouge.ShouldUse(out act)) return true;
        if (Hypervelocity.ShouldUse(out act)) return true;

        //Ѫ��
        if (GnashingFang.IsCoolDown && Bloodfest.ShouldUse(out act)) return true;

        //��㹥��,�ַ�ն
        if (RoughDivide.Target.DistanceToPlayer() < 1 && !IsMoving)
        {
            if (RoughDivide.ShouldUse(out act)) return true;
            if (Player.HasStatus(true, StatusID.NoMercy) && RoughDivide.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        }
        act = null;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        if (HeartofLight.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        if (Reprisal.ShouldUse(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //ͻ��
        if (RoughDivide.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }
    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (abilityRemain == 2)
        {

            //����10%��
            if (HeartofStone.ShouldUse(out act)) return true;

            //���ƣ�����30%��
            if (Nebula.ShouldUse(out act)) return true;

            //���ڣ�����20%��
            if (Rampart.ShouldUse(out act)) return true;

            //αװ������10%��
            if (Camouflage.ShouldUse(out act)) return true;
        }
        //���͹���
        //ѩ��
        if (Reprisal.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        if (Aurora.ShouldUse(out act, emptyOrSkipCombo: true) && abilityRemain == 1) return true;

        return false;
    }

    private bool CanUseNoMercy(out IAction act)
    {
        if (InDungeonsMiddle && NoMercy.ShouldUse(out act))
        {
            if (CanUseSpellInDungeonsMiddle) return true;
            return false;
        }
        //�ȼ����ڱ��������ж�
        if (!BurstStrike.EnoughLevel && NoMercy.ShouldUse(out act)) return true;

        if (BurstStrike.EnoughLevel && NoMercy.ShouldUse(out act))
        {
            //4GCD�����ж�
            if (IsLastWeaponSkill(KeenEdge.ID) && Ammo == 1 && !GnashingFang.IsCoolDown && !Bloodfest.IsCoolDown) return true;

            //3��������
            else if (Ammo == (Level >= 88 ? 3 : 2)) return true;

            //2��������
            else if (Ammo == 2 && GnashingFang.IsCoolDown) return true;
        }


        act = null;
        return false;
    }

    private bool CanUseGnashingFang(out IAction act)
    {
        //�����ж�
        if (GnashingFang.ShouldUse(out act))
        {
            //��4�˱�����ʹ��
            if (InDungeonsMiddle) return true;

            //������3������
            if (Ammo == (Level >= 88 ? 3 : 2) && (Player.HasStatus(true, StatusID.NoMercy) || !NoMercy.WillHaveOneCharge(55))) return true;

            //����������
            if (Ammo > 0 && !NoMercy.WillHaveOneCharge(17) && NoMercy.WillHaveOneCharge(35)) return true;

            //3���ҽ�������ӵ������,��ǰ������ǰ������
            if (Ammo == 3 && IsLastWeaponSkill(BrutalShell.ID) && NoMercy.WillHaveOneCharge(3)) return true;

            //1����Ѫ������ȴ����
            if (Ammo == 1 && !NoMercy.WillHaveOneCharge(55) && Bloodfest.WillHaveOneCharge(5)) return true;

            //4GCD���������ж�
            if (Ammo == 1 && !NoMercy.WillHaveOneCharge(55) && (!Bloodfest.IsCoolDown && Bloodfest.EnoughLevel || !Bloodfest.EnoughLevel)) return true;
        }
        return false;
    }

    /// <summary>
    /// ������
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool CanUseSonicBreak(out IAction act)
    {
        //�����ж�
        if (SonicBreak.ShouldUse(out act))
        {
            //��4�˱����в�ʹ��
            if (InDungeonsMiddle) return false;

            if (!GnashingFang.EnoughLevel && Player.HasStatus(true, StatusID.NoMercy)) return true;

            //����������ʹ��������
            if (GnashingFang.IsCoolDown && Player.HasStatus(true, StatusID.NoMercy)) return true;

            //�����ж�
            if (!DoubleDown.EnoughLevel && Player.HasStatus(true, StatusID.ReadyToRip)
                && GnashingFang.IsCoolDown) return true;

        }
        return false;
    }

    /// <summary>
    /// ����
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool CanUseDoubleDown(out IAction act)
    {
        //�����ж�
        if (DoubleDown.ShouldUse(out act, mustUse: true))
        {
            //��4�˱�����
            if (InDungeonsMiddle)
            {
                //��4�˱��ĵ����Ѿ��ۺùֿ���ʹ����ؼ���(���ƶ�������д���3ֻС��),������buff
                if (Player.HasStatus(true, StatusID.NoMercy)) return true;

                return false;
            }

            //�������ƺ�ʹ�ñ���
            if (SonicBreak.IsCoolDown && Player.HasStatus(true, StatusID.NoMercy)) return true;

            //2������������ж�,��ǰʹ�ñ���
            if (Player.HasStatus(true, StatusID.NoMercy) && !NoMercy.WillHaveOneCharge(55) && Bloodfest.WillHaveOneCharge(5)) return true;

        }
        return false;
    }

    /// <summary>
    /// ������
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool CanUseBurstStrike(out IAction act)
    {
        if (BurstStrike.ShouldUse(out act))
        {
            //��4�˱�������AOEʱ��ʹ��
            if (InDungeonsMiddle && DemonSlice.ShouldUse(out _)) return false;

            //�������ʣ0.5����ȴ��,���ͷű�����,��Ҫ��Ϊ���ٲ�ͬ���ܻ�ʹ�����Ӻ�̫�������ж�һ��
            if (SonicBreak.IsCoolDown && SonicBreak.WillHaveOneCharge(0.5f) && GnashingFang.EnoughLevel) return false;

            //�����б������ж�
            if (Player.HasStatus(true, StatusID.NoMercy) &&
                AmmoComboStep == 0 &&
                !GnashingFang.WillHaveOneCharge(1)) return true;
            if (Level < 88 && Ammo == 2) return true;
            //�������ֹ���
            if (IsLastWeaponSkill(BrutalShell.ID) &&
                (Ammo == (Level >= 88 ? 3 : 2) ||
                Bloodfest.WillHaveOneCharge(6) && Ammo <= 2 && !NoMercy.WillHaveOneCharge(10) && Bloodfest.EnoughLevel)) return true;

        }
        return false;
    }

    private bool CanUseBowShock(out IAction act)
    {
        if (BowShock.ShouldUse(out act, mustUse: true))
        {
            if (InDungeonsMiddle) return true;

            if (!SonicBreak.EnoughLevel && Player.HasStatus(true, StatusID.NoMercy)) return true;

            //������,������������������ȴ��
            if (Player.HasStatus(true, StatusID.NoMercy) && SonicBreak.IsCoolDown) return true;

        }
        return false;
    }
}

