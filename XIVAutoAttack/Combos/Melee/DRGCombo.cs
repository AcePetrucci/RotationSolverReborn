using Dalamud.Game.ClientState.JobGauge.Types;
using System.Numerics;

namespace XIVComboPlus.Combos;

internal class DRGCombo : CustomComboJob<DRGGauge>
{
    internal override uint JobID => 22;

    protected override bool ShouldSayout => true;
    internal struct Actions
    {
        public static readonly BaseAction
            //��׼��
            TrueThrust = new BaseAction(75),

            //��ͨ��
            VorpalThrust = new BaseAction(78) { OtherIDsCombo = new uint[] { 16479 } },

            //ֱ��
            FullThrust = new BaseAction(84),

            //����ǹ
            Disembowel = new BaseAction(87) { OtherIDsCombo = new uint[] { 16479 } },

            //ӣ��ŭ��
            ChaosThrust = new BaseAction(88),

            //������צ
            FangandClaw = new BaseAction(3554)
            {
                EnermyLocation = EnemyLocation.Side,
                BuffsNeed = new ushort[] { ObjectStatus.SharperFangandClaw },
            },

            //��β�����
            WheelingThrust = new BaseAction(3556)
            {
                EnermyLocation = EnemyLocation.Back,
                BuffsNeed = new ushort[] { ObjectStatus.EnhancedWheelingThrust },
            },

            //�ᴩ��
            PiercingTalon = new BaseAction(90),

            //����ǹ
            DoomSpike = new BaseAction(86),

            //���ٴ�
            SonicThrust = new BaseAction(7397) { OtherIDsCombo = new uint[] { 25770 } },

            //ɽ������
            CoerthanTorment = new BaseAction(16477),

            //�����
            SpineshatterDive = new BaseAction(95),

            //���׳�
            DragonfireDive = new BaseAction(96),

            //��Ծ
            Jump = new BaseAction(92),

            //�����
            MirageDive = new BaseAction(7399) { BuffsNeed = new ushort[] { ObjectStatus.DiveReady }, },

            //����ǹ
            Geirskogul = new BaseAction(3555),

            //����֮��
            Nastrond = new BaseAction(7400),

            //׹�ǳ�
            Stardiver = new BaseAction(16480),

            //�����㾦
            WyrmwindThrust = new BaseAction(25773),

            //����
            LifeSurge = new BaseAction(83) { BuffsProvide = new ushort[] { ObjectStatus.LifeSurge } },

            //��ǹ
            LanceCharge = new BaseAction(85),

            //��������
            DragonSight = new BaseAction(7398),

            //ս������
            BattleLitany = new BaseAction(3557);
    }

    private protected override bool EmergercyAbility(byte abilityRemain, BaseAction nextGCD, out BaseAction act)
    {
        if (nextGCD == Actions.FullThrust || nextGCD == Actions.CoerthanTorment || (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.LanceCharge) && nextGCD == Actions.WheelingThrust))
        {
            if (Actions.LifeSurge.ShouldUseAction(out act, Empty: true)) return true;
        }
        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool BreakAbility(byte abilityRemain, out BaseAction act)
    {
        //3 BUff
        if (Actions.LanceCharge.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.DragonSight.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.BattleLitany.ShouldUseAction(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out BaseAction act)
    {

        if (JobGauge.IsLOTDActive)
        {
            if (abilityRemain > 1 && Actions.Stardiver.ShouldUseAction(out act, mustUse: true)) return true;
            if (JobGauge.FirstmindsFocusCount == 2 && Actions.WyrmwindThrust.ShouldUseAction(out act, mustUse: true)) return true;
            if (Actions.Nastrond.ShouldUseAction(out act, mustUse: true)) return true;
        }

        //���Խ������Ѫ
        if (Actions.Geirskogul.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.MirageDive.ShouldUseAction(out act, mustUse: true)) return true;

        if (abilityRemain > 1 && Vector3.Distance(LocalPlayer.Position, Target.Position) - Target.HitboxRadius < 2)
        {
            if (Actions.Jump.ShouldUseAction(out act)) return true;
            if (Actions.SpineshatterDive.ShouldUseAction(out act, Empty: true)) return true;
            if (Actions.DragonfireDive.ShouldUseAction(out act, mustUse: true)) return true;
        }

        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out BaseAction act)
    {
        if (GeneralActions.SecondWind.ShouldUseAction(out act)) return true;
        if (GeneralActions.Bloodbath.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out BaseAction act)
    {
        #region Ⱥ��
        if (Actions.CoerthanTorment.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.SonicThrust.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.DoomSpike.ShouldUseAction(out act, lastComboActionID)) return true;

        #endregion

        #region ����
        if (Actions.WheelingThrust.ShouldUseAction(out act)) return true;
        if (Actions.FangandClaw.ShouldUseAction(out act)) return true;

        //�����Ƿ���Ҫ��Buff
        var time = BaseAction.FindStatusSelfFromSelf(ObjectStatus.PowerSurge);
        if (time.Length > 0 && time[0] > 13)
        {
            if (Actions.FullThrust.ShouldUseAction(out act, lastComboActionID)) return true;
            if (Actions.VorpalThrust.ShouldUseAction(out act, lastComboActionID)) return true;
            if (Actions.ChaosThrust.ShouldUseAction(out act, lastComboActionID)) return true;
        }
        else
        {
            if (Actions.Disembowel.ShouldUseAction(out act, lastComboActionID)) return true;
        }


        if (Actions.TrueThrust.ShouldUseAction(out act)) return true;
        if (Actions.PiercingTalon.ShouldUseAction(out act)) return true;

        return false;

        #endregion
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out BaseAction act)
    {
        //ǣ��
        if (GeneralActions.Feint.ShouldUseAction(out act)) return true;
        return false;
    }
}
