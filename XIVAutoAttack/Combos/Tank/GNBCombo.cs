using Dalamud.Game.ClientState.JobGauge.Types;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;

namespace XIVAutoAttack.Combos.Tank;

internal class GNBCombo : JobGaugeCombo<GNBGauge>
{
    internal override uint JobID => 37;
    internal override bool HaveShield => LocalPlayer.HaveStatus(ObjectStatus.RoyalGuard);
    private protected override BaseAction Shield => Actions.RoyalGuard;

    protected override bool CanHealSingleSpell => false;
    protected override bool CanHealAreaSpell => false;

    /// <summary>
    /// ��4�˱��ĵ����Ѿ��ۺùֿ���ʹ����ؼ���(���ƶ�������д���3ֻС��)
    /// </summary>
    private static bool CanUseSpellInDungeonsMiddle => TargetHelper.PartyMembers.Length is > 1 and <= 4 && !Target.IsBoss() && !IsMoving 
                                                    && TargetFilter.GetObjectInRadius(TargetHelper.HostileTargets, 5).Length >= 3;

    /// <summary>
    /// ��4�˱��ĵ���
    /// </summary>
    private static bool InDungeonsMiddle => TargetHelper.PartyMembers.Length is > 1 and <= 4 && !Target.IsBoss();

    internal struct Actions
    {
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
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
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
            Nebula = new (16148)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
                OtherCheck = BaseAction.TankDefenseSelf,
            },

            //��ħɱ
            DemonSlaughter = new (16149),

            //����
            Aurora = new BaseAction(16151, true)
            {
                BuffsProvide = new [] { ObjectStatus.Aurora },
            },

            //��������
            Superbolide = new (16152)
            {
                OtherCheck = BaseAction.TankBreakOtherCheck,
            },

            //������
            SonicBreak = new (16153),

            //�ַ�ն
            RoughDivide = new (16154, shouldEndSpecial: true)
            {
                ChoiceTarget = TargetFilter.FindMoveTarget
            },

            //����
            GnashingFang = new (16146)
            {
                OtherCheck = b => JobGauge.AmmoComboStep == 0 && JobGauge.Ammo > 0,
            },

            //���γ岨
            BowShock = new (16159),

            //��֮��
            HeartofLight = new (16160, true),

            //ʯ֮��
            HeartofStone = new (16161, true)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
                ChoiceTarget = TargetFilter.FindAttackedTarget,
            },

            //����֮��
            FatedCircle = new (16163)
            {
                OtherCheck = b => JobGauge.Ammo > (Level >= 88 ? 2 : 1),
            },

            //Ѫ��
            Bloodfest = new (16164)
            {
                OtherCheck = b => JobGauge.Ammo == 0,
            },

            //����
            DoubleDown = new (25760)
            {
                OtherCheck = b => JobGauge.Ammo >= 2,
            },

            //����צ
            SavageClaw = new (16147)
            {
                OtherCheck = b => Service.IconReplacer.OriginalHook(GnashingFang.ID) == SavageClaw.ID,
            },

            //����צ
            WickedTalon = new (16150)
            {
                OtherCheck = b => Service.IconReplacer.OriginalHook(GnashingFang.ID) == WickedTalon.ID,
            },

            //˺��
            JugularRip = new (16156)
            {
                OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == JugularRip.ID,
            },

            //����
            AbdomenTear = new (16157)
            {
                OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == AbdomenTear.ID,
            },

            //��Ŀ
            EyeGouge = new (16158)
            {
                OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == EyeGouge.ID,
            },

            //������
            Hypervelocity = new (25759)
            {
                OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == Hypervelocity.ID,
            };
    }
    internal override SortedList<DescType, string> Description => new ()
    {
        {DescType.��������, $"{Actions.Aurora.Action.Name}"},
        {DescType.��Χ����, $"{Actions.HeartofLight.Action.Name}"},
        {DescType.�������, $"{Actions.HeartofStone.Action.Name}, {Actions.Nebula.Action.Name}, {Actions.Camouflage.Action.Name}"},
        {DescType.�ƶ�, $"{Actions.RoughDivide.Action.Name}"},
    };

    //private protected override ActionConfiguration CreateConfiguration()
    //{
    //    return base.CreateConfiguration().SetCombo("GNB_Opener", 0, new string[]
    //    {
    //        "4GCD����",
    //    }, "����ѡ��");
    //}

    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        //����,Ŀǰֻ��4GCD���ֵ��ж�
        if (abilityRemain == 1 && CanUseNoMercy(out act))  return true;

        act = null;
        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //����
        if (CanUseGnashingFang(out act)) return true;
     
        //������
        if (CanUseSonicBreak(out act)) return true;

        //����
        if (CanUseDoubleDown(out act)) return true;

        //���������
        if (Actions.WickedTalon.ShouldUse(out act)) return true;
        if (Actions.SavageClaw.ShouldUse(out act)) return true;

        //����֮�� AOE
        if (Actions.FatedCircle.ShouldUse(out act)) return true;

        //������   
        if (CanUseBurstStrike(out act)) return true;

        //AOE
        if (Actions.DemonSlaughter.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.DemonSlice.ShouldUse(out act, lastComboActionID)) return true;

        //��������
        //�������ʣ0.5����ȴ��,���ͷŻ�������,��Ҫ��Ϊ���ٲ�ͬ���ܻ�ʹ�����Ӻ�̫�������ж�һ��
        if (Actions.GnashingFang.RecastTimeRemain > 0 && Actions.GnashingFang.RecastTimeRemain < 0.5) return false;
        if (Actions.SolidBarrel.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.BrutalShell.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.KeenEdge.ShouldUse(out act, lastComboActionID)) return true;

        
        if (IconReplacer.Move && MoveAbility(1, out act)) return true;

        if (Actions.LightningShot.ShouldUse(out act))
        {
            if (InDungeonsMiddle && Actions.LightningShot.Target.DistanceToPlayer() > 3) return true;
        }

        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //�������� ���л�����ˡ�
        if (Actions.Superbolide.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        //Σ������
        if (Actions.DangerZone.ShouldUse(out act))
        {
            if (InDungeonsMiddle) return true;

            //������,����֮��
            if (LocalPlayer.HaveStatus(ObjectStatus.NoMercy) && Actions.GnashingFang.RecastTimeRemain > 0) return true;

            //�Ǳ�����
            if (!LocalPlayer.HaveStatus(ObjectStatus.NoMercy) && Actions.GnashingFang.RecastTimeRemain > 20) return true;

            //�ȼ�С������,
            if (Level < Actions.GnashingFang.Level && (LocalPlayer.HaveStatus(ObjectStatus.NoMercy) || Actions.NoMercy.RecastTimeRemain > 15)) return true;
        }

        //���γ岨
        if (CanUseBowShock(out act)) return true;

        //����
        if (Actions.JugularRip.ShouldUse(out act)) return true;
        if (Actions.AbdomenTear.ShouldUse(out act)) return true;
        if (Actions.EyeGouge.ShouldUse(out act)) return true;
        if (Actions.Hypervelocity.ShouldUse(out act)) return true;

        //Ѫ��
        if (Actions.GnashingFang.RecastTimeRemain > 0 && Actions.Bloodfest.ShouldUse(out act)) return true;

        //��㹥��,�ַ�ն
        if (Actions.RoughDivide.Target.DistanceToPlayer() < 1 && !IsMoving)
        {  
            if (Actions.RoughDivide.ShouldUse(out act)) return true;
            if (LocalPlayer.HaveStatus(ObjectStatus.NoMercy) && Actions.RoughDivide.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        }
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.HeartofLight.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        if (GeneralActions.Reprisal.ShouldUse(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //ͻ��
        if (Actions.RoughDivide.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }
    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (abilityRemain == 2)
        {

            //����10%��
            if (Actions.HeartofStone.ShouldUse(out act)) return true;

            //���ƣ�����30%��
            if (Actions.Nebula.ShouldUse(out act)) return true;

            //���ڣ�����20%��
            if (GeneralActions.Rampart.ShouldUse(out act)) return true;

            //αװ������10%��
            if (Actions.Camouflage.ShouldUse(out act)) return true;
        }
        //���͹���
        //ѩ��
        if (GeneralActions.Reprisal.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.Aurora.ShouldUse(out act, emptyOrSkipCombo: true) && abilityRemain == 1) return true;

        return false;
    }

    private bool CanUseNoMercy(out IAction act)
    {
        if (InDungeonsMiddle && Actions.NoMercy.ShouldUse(out act))
        {
            if (CanUseSpellInDungeonsMiddle) return true;
            return false;
        }

        if (Level >= Actions.BurstStrike.Level && Actions.NoMercy.ShouldUse(out act))
        {
            //4GCD�����ж�
            if (IsLastWeaponSkill(Actions.KeenEdge.ID) && JobGauge.Ammo == 1 && Actions.GnashingFang.RecastTimeRemain == 0 && !Actions.Bloodfest.IsCoolDown) return true;

            //3��������
            else if (JobGauge.Ammo == (Level >= 88 ? 3 : 2)) return true;

            //2��������
            else if (JobGauge.Ammo == 2 && Actions.GnashingFang.RecastTimeRemain > 0) return true;
        }
        //�ȼ����ڱ��������ж�
        if (Level < Actions.BurstStrike.Level && Actions.NoMercy.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private bool CanUseGnashingFang(out IAction act)
    {
        //�����ж�
        if (Actions.GnashingFang.ShouldUse(out act))
        {
            //��4�˱����в�ʹ��
            if (InDungeonsMiddle) return false;

            //������3������
            if (JobGauge.Ammo == (Level >= 88 ? 3 : 2) && (LocalPlayer.HaveStatus(ObjectStatus.NoMercy) || Actions.NoMercy.RecastTimeRemain > 55)) return true;

            //����������
            if (JobGauge.Ammo > 0 && Actions.NoMercy.RecastTimeRemain > 17 && Actions.NoMercy.RecastTimeRemain < 35) return true;

            //3���ҽ�������ӵ������,��ǰ������ǰ������
            if (JobGauge.Ammo == 3 && IsLastWeaponSkill(Actions.BrutalShell.ID) && Actions.NoMercy.RecastTimeRemain < 3) return true;

            //1����Ѫ������ȴ����
            if (JobGauge.Ammo == 1 && Actions.NoMercy.RecastTimeRemain > 55 && Actions.Bloodfest.RecastTimeRemain < 5) return true;

            //4GCD���������ж�
            if (JobGauge.Ammo == 1 && Actions.NoMercy.RecastTimeRemain > 55 && ((!Actions.Bloodfest.IsCoolDown && Level >= Actions.Bloodfest.Level) || Level < Actions.Bloodfest.Level)) return true;
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
        if (Actions.SonicBreak.ShouldUse(out act))
        {
            //��4�˱����в�ʹ��
            if (InDungeonsMiddle) return false;

            //����������ʹ��������
            if (Actions.GnashingFang.RecastTimeRemain > 0 && LocalPlayer.HaveStatus(ObjectStatus.NoMercy)) return true;

            //�����ж�
            if (Level < Actions.DoubleDown.Level && LocalPlayer.HaveStatus(ObjectStatus.ReadyToRip)
                && Actions.GnashingFang.RecastTimeRemain > 0) return true;
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
        if (Actions.DoubleDown.ShouldUse(out act, mustUse: true))
        {
            //��4�˱�����
            if (InDungeonsMiddle)
            {
                //��4�˱��ĵ����Ѿ��ۺùֿ���ʹ����ؼ���(���ƶ�������д���3ֻС��),������buff
                if (LocalPlayer.HaveStatus(ObjectStatus.NoMercy)) return true;

                return false;
            }

            //�������ƺ�ʹ�ñ���
            if (Actions.SonicBreak.RecastTimeRemain > 0 && LocalPlayer.HaveStatus(ObjectStatus.NoMercy)) return true;

            //2������������ж�,��ǰʹ�ñ���
            if (LocalPlayer.HaveStatus(ObjectStatus.NoMercy) && Actions.NoMercy.RecastTimeRemain > 55 && Actions.Bloodfest.RecastTimeRemain < 5) return true;

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
        if (Actions.BurstStrike.ShouldUse(out act))
        {
            //��4�˱��������ƶ�ʱ��ʹ��
            if (InDungeonsMiddle && IsMoving) return false;

            //�������ʣ0.5����ȴ��,���ͷű�����,��Ҫ��Ϊ���ٲ�ͬ���ܻ�ʹ�����Ӻ�̫�������ж�һ��
            if (Actions.SonicBreak.RecastTimeRemain > 0 && Actions.SonicBreak.RecastTimeRemain < 0.5) return false;

            //�����б������ж�
            if (LocalPlayer.HaveStatus(ObjectStatus.NoMercy) &&
                JobGauge.AmmoComboStep == 0 &&
                Actions.GnashingFang.RecastTimeRemain > 1) return true;

            //�������ֹ���
            if (IsLastWeaponSkill(Actions.BrutalShell.ID) &&
                (JobGauge.Ammo == (Level >= 88 ? 3 : 2) ||
                (Actions.Bloodfest.RecastTimeRemain < 6 && JobGauge.Ammo <= 2 && Actions.NoMercy.RecastTimeRemain > 10 && Level >= Actions.Bloodfest.Level))) return true;
        }
        return false;
    }
    
    private bool CanUseBowShock(out IAction act)
    {
        if (Actions.BowShock.ShouldUse(out act, mustUse: true))
        {
            if (InDungeonsMiddle) return true;

            //������,������������������ȴ��
            if (LocalPlayer.HaveStatus(ObjectStatus.NoMercy) && Actions.SonicBreak.RecastTimeRemain > 0) return true;
        }
        return false;     
    }
}
    
