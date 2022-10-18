using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.CustomCombo;

namespace XIVAutoAttack.Combos.Tank;

internal class GNBCombo : JobGaugeCombo<GNBGauge>
{
    internal override uint JobID => 37;
    internal override bool HaveShield => StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.RoyalGuard);
    private protected override PVEAction Shield => Actions.RoyalGuard;

    protected override bool CanHealSingleSpell => false;
    protected override bool CanHealAreaSpell => false;

    internal struct Actions
    {
        public static readonly PVEAction
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
                OtherCheck = PVEAction.TankDefenseSelf,
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
                OtherCheck = PVEAction.TankDefenseSelf,
            },

            //��ħɱ
            DemonSlaughter = new (16149),

            //����
            Aurora = new PVEAction(16151, true)
            {
                BuffsProvide = new [] { ObjectStatus.Aurora },
            },

            //��������
            Superbolide = new (16152)
            {
                OtherCheck = PVEAction.TankBreakOtherCheck,
            },

            //������
            SonicBreak = new (16153),

            //�ַ�ն
            RoughDivide = new (16154, shouldEndSpecial: true)
            {
                ChoiceTarget = TargetFilter.FindMoveTarget
            },

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
                ChoiceTarget = TargetFilter.FindAttackedTarget,
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
        bool useAmmo = JobGauge.Ammo > (Level >= Actions.DoubleDown.Level ? 2 : 1);
        bool breakUseAmmo = JobGauge.Ammo >= (Level >= Actions.DoubleDown.Level ? 2 : 1);

        //AOE
        if (breakUseAmmo && Actions.DoubleDown.ShouldUse(out act, mustUse: true)) return true;
        if (useAmmo && Actions.FatedCircle.ShouldUse(out act)) return true;

        if ( Actions.DemonSlaughter.ShouldUse(out act, lastComboActionID)) return true;
        if ( Actions.DemonSlice.ShouldUse(out act, lastComboActionID)) return true;

        uint remap = Service.IconReplacer.OriginalHook(Actions.GnashingFang.ID);
        if (remap == Actions.WickedTalon.ID && Actions.WickedTalon.ShouldUse(out act)) return true;
        if (remap == Actions.SavageClaw.ID && Actions.SavageClaw.ShouldUse(out act)) return true;


        //����
        if (breakUseAmmo && Actions.GnashingFang.ShouldUse(out act)) return true;
        if (useAmmo && Actions.BurstStrike.ShouldUse(out act)) return true;

        if (Actions.SonicBreak.ShouldUse(out act)) return true;

        //��������
        if (Actions.SolidBarrel.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.BrutalShell.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.KeenEdge.ShouldUse(out act, lastComboActionID)) return true;

        if (IconReplacer.Move && MoveAbility(1, out act)) return true;
        if (Actions.LightningShot.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //��ʥ���� ���л�����ˡ�
        if (Actions.Superbolide.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.JugularRip.ShouldUse(out act)) return true;
        if (Actions.AbdomenTear.ShouldUse(out act)) return true;
        if (Actions.EyeGouge.ShouldUse(out act)) return true;
        if (Actions.Hypervelocity.ShouldUse(out act)) return true;


        if (Actions.NoMercy.ShouldUse(out act)) return true;
        if (Actions.Bloodfest.ShouldUse(out act)) return true;
        if (Actions.BowShock.ShouldUse(out act, mustUse: true)) return true;
        if (Actions.DangerZone.ShouldUse(out act)) return true;

        //��㹥��
        if (Actions.RoughDivide.ShouldUse(out act) && !IsMoving)
        {
            if (Actions.RoughDivide.Target.DistanceToPlayer() < 1)
            {
                return true;
            }
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
}
