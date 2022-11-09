using Dalamud.Game.ClientState.JobGauge.Types;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Attributes;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.Tank.GNBCombo;

namespace XIVAutoAttack.Combos.Tank;

[ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/XIVAutoAttack/Combos/Tank/GNBCombo.cs",
   ComboAuthor.Armolion)]
internal sealed class GNBCombo : JobGaugeCombo<GNBGauge, CommandType>
{
    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //д��ע�Ͱ���������ʾ�û��ġ�
    };
    public override uint[] JobIDs => new uint[] { 37 };
    internal override bool HaveShield => Player.HaveStatus(ObjectStatus.RoyalGuard);
    private protected override BaseAction Shield => RoyalGuard;

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

    public static readonly BaseAction
        //��������
        RoyalGuard = new(16142, shouldEndSpecial: true),

        //����ն
        KeenEdge = new(16137),

        //����
        NoMercy = new(16138),

        //�б���
        BrutalShell = new(16139),

        //αװ
        Camouflage = new(16140)
        {
            BuffsProvide = Rampart.BuffsProvide,
            OtherCheck = BaseAction.TankDefenseSelf,
        },

        //��ħ��
        DemonSlice = new(16141),

        //���׵�
        LightningShot = new(16143),

        //Σ������
        DangerZone = new(16144),

        //Ѹ��ն
        SolidBarrel = new(16145),

        //������
        BurstStrike = new(16162)
        {
            OtherCheck = b => JobGauge.Ammo > 0,
        },

        //����
        Nebula = new(16148)
        {
            BuffsProvide = Rampart.BuffsProvide,
            OtherCheck = BaseAction.TankDefenseSelf,
        },

        //��ħɱ
        DemonSlaughter = new(16149),

        //����
        Aurora = new BaseAction(16151, true)
        {
            BuffsProvide = new[] { ObjectStatus.Aurora },
        },

        //��������
        Superbolide = new(16152)
        {
            OtherCheck = BaseAction.TankBreakOtherCheck,
        },

        //������
        SonicBreak = new(16153),

        //�ַ�ն
        RoughDivide = new(16154, shouldEndSpecial: true)
        {
            ChoiceTarget = TargetFilter.FindTargetForMoving
        },

        //����
        GnashingFang = new(16146)
        {
            OtherCheck = b => JobGauge.AmmoComboStep == 0 && JobGauge.Ammo > 0,
        },

        //���γ岨
        BowShock = new(16159),

        //��֮��
        HeartofLight = new(16160, true),

        //ʯ֮��
        HeartofStone = new(16161, true)
        {
            BuffsProvide = Rampart.BuffsProvide,
            ChoiceTarget = TargetFilter.FindAttackedTarget,
        },

        //����֮��
        FatedCircle = new(16163)
        {
            OtherCheck = b => JobGauge.Ammo > (Level >= 88 ? 2 : 1),
        },

        //Ѫ��
        Bloodfest = new(16164)
        {
            OtherCheck = b => JobGauge.Ammo == 0,
        },

        //����
        DoubleDown = new(25760)
        {
            OtherCheck = b => JobGauge.Ammo >= 2,
        },

        //����צ
        SavageClaw = new(16147)
        {
            OtherCheck = b => Service.IconReplacer.OriginalHook(GnashingFang.ID) == SavageClaw.ID,
        },

        //����צ
        WickedTalon = new(16150)
        {
            OtherCheck = b => Service.IconReplacer.OriginalHook(GnashingFang.ID) == WickedTalon.ID,
        },

        //˺��
        JugularRip = new(16156)
        {
            OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == JugularRip.ID,
        },

        //����
        AbdomenTear = new(16157)
        {
            OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == AbdomenTear.ID,
        },

        //��Ŀ
        EyeGouge = new(16158)
        {
            OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == EyeGouge.ID,
        },

        //������
        Hypervelocity = new(25759)
        {
            OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == Hypervelocity.ID,
        };
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
        if (GnashingFang.IsCoolDown && GnashingFang.WillHaveOneCharge((float)0.5, false) && GnashingFang.EnoughLevel) return false;
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

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //�������� ���л�����ˡ�
        if (Superbolide.ShouldUse(out act)) return true;
        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
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
            if (!GnashingFang.EnoughLevel && (Player.HaveStatus(ObjectStatus.NoMercy) || !NoMercy.WillHaveOneCharge(15, false))) return true;

            //������,����֮��
            if (Player.HaveStatus(ObjectStatus.NoMercy) && GnashingFang.IsCoolDown) return true;

            //�Ǳ�����
            if (!Player.HaveStatus(ObjectStatus.NoMercy) && !GnashingFang.WillHaveOneCharge(20, false)) return true;
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
            if (Player.HaveStatus(ObjectStatus.NoMercy) && RoughDivide.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
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
            if (IsLastWeaponSkill(KeenEdge.ID) && JobGauge.Ammo == 1 && !GnashingFang.IsCoolDown && !Bloodfest.IsCoolDown) return true;

            //3��������
            else if (JobGauge.Ammo == (Level >= 88 ? 3 : 2)) return true;

            //2��������
            else if (JobGauge.Ammo == 2 && GnashingFang.IsCoolDown) return true;
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
            if (JobGauge.Ammo == (Level >= 88 ? 3 : 2) && (Player.HaveStatus(ObjectStatus.NoMercy) || !NoMercy.WillHaveOneCharge(55, false))) return true;

            //����������
            if (JobGauge.Ammo > 0 && !NoMercy.WillHaveOneCharge(17, false) && NoMercy.WillHaveOneCharge(35, false)) return true;

            //3���ҽ�������ӵ������,��ǰ������ǰ������
            if (JobGauge.Ammo == 3 && IsLastWeaponSkill(BrutalShell.ID) && NoMercy.WillHaveOneCharge(3, false)) return true;

            //1����Ѫ������ȴ����
            if (JobGauge.Ammo == 1 && !NoMercy.WillHaveOneCharge(55, false) && Bloodfest.WillHaveOneCharge(5, false)) return true;

            //4GCD���������ж�
            if (JobGauge.Ammo == 1 && !NoMercy.WillHaveOneCharge(55, false) && ((!Bloodfest.IsCoolDown && Bloodfest.EnoughLevel) || !Bloodfest.EnoughLevel)) return true;
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

            if (!GnashingFang.EnoughLevel && Player.HaveStatus(ObjectStatus.NoMercy)) return true;

            //����������ʹ��������
            if (GnashingFang.IsCoolDown && Player.HaveStatus(ObjectStatus.NoMercy)) return true;

            //�����ж�
            if (!DoubleDown.EnoughLevel && Player.HaveStatus(ObjectStatus.ReadyToRip)
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
                if (Player.HaveStatus(ObjectStatus.NoMercy)) return true;

                return false;
            }

            //�������ƺ�ʹ�ñ���
            if (SonicBreak.IsCoolDown && Player.HaveStatus(ObjectStatus.NoMercy)) return true;

            //2������������ж�,��ǰʹ�ñ���
            if (Player.HaveStatus(ObjectStatus.NoMercy) && !NoMercy.WillHaveOneCharge(55, false) && Bloodfest.WillHaveOneCharge(5, false)) return true;

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
            if (SonicBreak.IsCoolDown && SonicBreak.WillHaveOneCharge((float)0.5, false) && GnashingFang.EnoughLevel) return false;

            //�����б������ж�
            if (Player.HaveStatus(ObjectStatus.NoMercy) &&
                JobGauge.AmmoComboStep == 0 &&
                !GnashingFang.WillHaveOneCharge(1, false)) return true;
            if (Level < 88 && JobGauge.Ammo == 2) return true;
            //�������ֹ���
            if (IsLastWeaponSkill(BrutalShell.ID) &&
                (JobGauge.Ammo == (Level >= 88 ? 3 : 2) ||
                (Bloodfest.WillHaveOneCharge(6, false) && JobGauge.Ammo <= 2 && !NoMercy.WillHaveOneCharge(10, false) && Bloodfest.EnoughLevel))) return true;

        }
        return false;
    }

    private bool CanUseBowShock(out IAction act)
    {
        if (BowShock.ShouldUse(out act, mustUse: true))
        {
            if (InDungeonsMiddle) return true;

            if (!SonicBreak.EnoughLevel && Player.HaveStatus(ObjectStatus.NoMercy)) return true;

            //������,������������������ȴ��
            if (Player.HaveStatus(ObjectStatus.NoMercy) && SonicBreak.IsCoolDown) return true;

        }
        return false;
    }
}

