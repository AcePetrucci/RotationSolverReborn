using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using System.Numerics;

namespace XIVAutoAttack.Combos.Tank;

internal class GNBCombo : CustomComboJob<GNBGauge>
{
    internal override uint JobID => 37;
    internal override bool HaveShield => BaseAction.HaveStatusSelfFromSelf(ObjectStatus.RoyalGuard);
    private protected override BaseAction Shield => Actions.RoyalGuard;

    protected override bool CanHealSingleSpell => false;
    protected override bool CanHealAreaSpell => false;

    internal struct Actions
    {
        public static readonly BaseAction
            //��������
            RoyalGuard = new (16142, shouldEndSpecial: true),

            //����ն
            KeenEdge = new (16137),

            //����
            NoMercy = new (16138),

            //�б���
            BrutalShell = new (16139),

            //αװ
            Camouflage = new (16140)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
            },

            //��ħ��
            DemonSlice = new (16141),

            //���׵�
            LightningShot = new (16143),

            //Σ������
            DangerZone = new (16144),

            //Ѹ��ն
            SolidBarrel = new (16145),

            //������
            BurstStrike = new (16162),

            //����
            Nebula = new (16148)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
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
                OtherCheck = b => (float)Service.ClientState.LocalPlayer.CurrentHp / Service.ClientState.LocalPlayer.MaxHp < Service.Configuration.HealthForDyingTank,
            },

            //������
            SonicBreak = new (16153),

            //�ַ�ն
            RoughDivide = new (16154, shouldEndSpecial: true),

            //����
            GnashingFang = new (16146),

            //���γ岨
            BowShock = new (16159),

            //��֮��
            HeartofLight = new (16160, true),

            //ʯ֮��
            HeartofStone = new (16161, true)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
                ChoiceFriend = BaseAction.FindAttackedTarget,
            },

            //����֮��
            FatedCircle = new (16163),

            //Ѫ��
            Bloodfest = new (16164)
            {
                OtherCheck = b => JobGauge.Ammo == 0,
            },

            //����
            DoubleDown = new (25760),

            //����צ
            SavageClaw = new (16147),

            //����צ
            WickedTalon = new (16150),

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
    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //ʹ�þ���
        bool useAmmo = JobGauge.Ammo > (Service.ClientState.LocalPlayer.Level >= Actions.DoubleDown.Level ? 2 : 0);

        uint remap = Service.IconReplacer.OriginalHook(Actions.GnashingFang.ID);
        if (remap == Actions.WickedTalon.ID && Actions.WickedTalon.ShouldUseAction(out act)) return true;
        if (remap == Actions.SavageClaw.ID && Actions.SavageClaw.ShouldUseAction(out act)) return true;

        //AOE
        if (useAmmo)
        {
            if (Actions.DoubleDown.ShouldUseAction(out act, mustUse: true)) return true;
            if (Actions.FatedCircle.ShouldUseAction(out act)) return true;
        }
        if ( Actions.DemonSlaughter.ShouldUseAction(out act, lastComboActionID)) return true;
        if ( Actions.DemonSlice.ShouldUseAction(out act, lastComboActionID)) return true;

        //����
        if (useAmmo)
        {
            if (Actions.GnashingFang.ShouldUseAction(out act)) return true;
            if (Actions.BurstStrike.ShouldUseAction(out act)) return true;
        }
        if (Actions.SonicBreak.ShouldUseAction(out act)) return true;

        //��������
        if (Actions.SolidBarrel.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.BrutalShell.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.KeenEdge.ShouldUseAction(out act, lastComboActionID)) return true;

        if (IconReplacer.Move && MoveAbility(1, out act)) return true;
        if (Actions.LightningShot.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //��ʥ���� ���л�����ˡ�
        if (Actions.Superbolide.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.JugularRip.ShouldUseAction(out act)) return true;
        if (Actions.AbdomenTear.ShouldUseAction(out act)) return true;
        if (Actions.EyeGouge.ShouldUseAction(out act)) return true;
        if (Actions.Hypervelocity.ShouldUseAction(out act)) return true;


        if (Actions.NoMercy.ShouldUseAction(out act)) return true;
        if (Actions.Bloodfest.ShouldUseAction(out act)) return true;
        if (Actions.BowShock.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.DangerZone.ShouldUseAction(out act)) return true;

        //��㹥��
        if (Actions.RoughDivide.ShouldUseAction(out act) && !IsMoving)
        {
            if (BaseAction.DistanceToPlayer(Actions.RoughDivide.Target) < 1)
            {
                return true;
            }
        }
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.HeartofLight.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //ͻ��
        if (Actions.RoughDivide.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }
    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (abilityRemain == 1)
        {

            //����10%��
            if (Actions.HeartofStone.ShouldUseAction(out act)) return true;

            //���ƣ�����30%��
            if (Actions.Nebula.ShouldUseAction(out act)) return true;

            //���ڣ�����20%��
            if (GeneralActions.Rampart.ShouldUseAction(out act)) return true;

            //αװ������10%��
            if (Actions.Camouflage.ShouldUseAction(out act)) return true;
        }
        //���͹���
        //ѩ��
        if (GeneralActions.Reprisal.ShouldUseAction(out act)) return true;


        act = null;
        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.Aurora.ShouldUseAction(out act, emptyOrSkipCombo: true) && abilityRemain == 1) return true;

        return false;
    }
}
