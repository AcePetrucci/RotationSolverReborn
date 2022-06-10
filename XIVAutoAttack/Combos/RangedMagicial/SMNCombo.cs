using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVAutoAttack.Combos.RangedMagicial;

internal class SMNCombo : CustomComboJob<SMNGauge>
{
    public class SMNAction : BaseAction
    {
        internal override int Cast100 => InBahamut || InPhoenix || !JobGauge.IsIfritAttuned ? 0 : base.Cast100;
        public SMNAction(uint actionID, bool isFriendly = false, bool shouldEndSpecial = false)
            : base(actionID, isFriendly, shouldEndSpecial)
        {

        }
    }
    internal override uint JobID => 27;
    protected override bool CanHealSingleSpell => false;
    private protected override BaseAction Raise => Actions.Resurrection;

    private static bool InBahamut => Service.IconReplacer.OriginalHook(25822) == Actions.Deathflare.ID;
    private static bool InPhoenix => Service.IconReplacer.OriginalHook(25822) == Actions.Rekindle.ID;
    private static bool InBreak => InBahamut || InPhoenix || Service.ClientState.LocalPlayer.Level < Actions.SummonBahamut.Level;
    internal struct Actions
    {
        public static readonly BaseAction
            //��ʯ���ٻ�
            SummonCarbuncle = new BaseAction(25798)
            {
                OtherCheck = b => !TargetHelper.HavePet,
            },

            //����֮�� �Ÿ�
            SearingLight = new BaseAction(25801)
            {
                OtherCheck = b => TargetHelper.InBattle && !InBahamut && !InPhoenix &&
                JobGauge.ReturnSummon == Dalamud.Game.ClientState.JobGauge.Enums.SummonPet.NONE,
            },

            //�ػ�֮�� ���Լ�����
            RadiantAegis = new BaseAction(25799),

            //ҽ��
            Physick = new BaseAction(16230, true),

            //��̫���� 
            Aethercharge = new BaseAction(25800)
            {
                OtherCheck = b => TargetHelper.InBattle,
            },

            //�����ٻ�
            SummonBahamut = new BaseAction(7427),

            //�챦ʯ�ٻ�
            SummonRuby = new BaseAction(25802)
            {
                OtherCheck = b => JobGauge.IsIfritReady,
            },

            //�Ʊ�ʯ�ٻ�
            SummonTopaz = new BaseAction(25803)
            {
                OtherCheck = b => JobGauge.IsTitanReady,
            },

            //�̱�ʯ�ٻ�
            SummonEmerald = new BaseAction(25804)
            {
                OtherCheck = b => JobGauge.IsGarudaReady,
            },

            //��ʯҫ
            Gemshine = new SMNAction(25883)
            {
                OtherCheck = b => JobGauge.Attunement > 0,
            },

            //��ʯ��
            PreciousBrilliance = new SMNAction(25884)
            {
                OtherCheck = b => JobGauge.Attunement > 0,
            },

            //���� ���幥��
            Ruin = new SMNAction(163),

            //���� ��Χ�˺�
            Outburst = new SMNAction(16511),

            //����
            Resurrection = new BaseAction(173, true),

            //��������
            EnergyDrain = new BaseAction(16508),

            //������ȡ
            EnergySiphon = new BaseAction(16510),

            //���ñ���
            Fester = new BaseAction(181),

            //ʹ��˱�
            Painflare = new BaseAction(3578),

            //�پ�
            RuinIV = new BaseAction(7426)
            {
                BuffsNeed = new ushort[] { ObjectStatus.FurtherRuin },
            },

            //����ŷ�
            EnkindleBahamut = new BaseAction(7429)
            {
                OtherCheck = b => InBahamut || InPhoenix,
            },

            //���Ǻ˱�
            Deathflare = new BaseAction(3582)
            {
                OtherCheck = b => InBahamut,
            },

            //����֮��
            Rekindle = new BaseAction(25830, true)
            {
                OtherCheck = b => InPhoenix,
            },

            //�������
            CrimsonCyclone = new BaseAction(25835)
            {
                BuffsNeed = new ushort[] { ObjectStatus.IfritsFavor },
            },

            //���ǿϮ
            CrimsonStrike = new BaseAction(25885),

            //ɽ��
            MountainBuster = new BaseAction(25836)
            {
                BuffsNeed = new ushort[] { ObjectStatus.TitansFavor },
            },

            //��������
            Slipstream = new BaseAction(25837)
            {
                BuffsNeed = new ushort[] { ObjectStatus.GarudasFavor },
            };
    }

    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        //����֮��
        if (Actions.SearingLight.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //��ʯ���ٻ�
        if (Actions.SummonCarbuncle.ShouldUseAction(out act)) return true;

        //����
        if (!InBahamut && !InPhoenix)
        {
            if (Actions.RuinIV.ShouldUseAction(out act, mustUse: true)) return true;
            if (Actions.CrimsonStrike.ShouldUseAction(out act, lastComboActionID, mustUse: true)) return true;
            if (Actions.CrimsonCyclone.ShouldUseAction(out act, mustUse: true)) return true;
            if (Actions.Slipstream.ShouldUseAction(out act, mustUse: true)) return true;
        }


        //�ٻ�
        if (JobGauge.Attunement == 0)
        {
            if (Actions.SummonBahamut.ShouldUseAction(out act))
            {
                if (Actions.SearingLight.IsCoolDown || Service.ClientState.LocalPlayer.Level < Actions.SearingLight.Level)
                    return true;
            }
            else if (Actions.Aethercharge.ShouldUseAction(out act)) return true;

            if (JobGauge.IsIfritReady && JobGauge.IsGarudaReady && JobGauge.IsTitanReady ? JobGauge.SummonTimerRemaining == 0 : true)
            {
                //��
                if (!TargetHelper.IsMoving && Actions.SummonRuby.ShouldUseAction(out act)) return true;

                //��
                if (Actions.SummonEmerald.ShouldUseAction(out act)) return true;
                //��
                if (Actions.SummonTopaz.ShouldUseAction(out act)) return true;
            }
        }

        //AOE
        if (Actions.PreciousBrilliance.ShouldUseAction(out act)) return true;
        if (Actions.Outburst.ShouldUseAction(out act)) return true;

        //����
        if (Actions.Gemshine.ShouldUseAction(out act)) return true;
        if (Actions.Ruin.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.EnkindleBahamut.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.Deathflare.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.Rekindle.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.MountainBuster.ShouldUseAction(out act, mustUse: true)) return true;


        //��������
        if (JobGauge.HasAetherflowStacks && InBreak)
        {
            if (Actions.Painflare.ShouldUseAction(out act)) return true;
            if (Actions.Fester.ShouldUseAction(out act)) return true;
        }
        else
        {
            if (Actions.EnergySiphon.ShouldUseAction(out act)) return true;
            if (Actions.EnergyDrain.ShouldUseAction(out act)) return true;
        }

        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        //�ػ�֮��
        if (Actions.RadiantAegis.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(uint lastComboActionID, out IAction act)
    {
        //ҽ��
        if (Actions.Physick.ShouldUseAction(out act)) return true;

        return false;
    }

}
