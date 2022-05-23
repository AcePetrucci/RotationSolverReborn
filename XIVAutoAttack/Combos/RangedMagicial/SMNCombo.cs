using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboPlus.Combos;

internal class SMNCombo : CustomComboJob<SMNGauge>
{
    internal override uint JobID => 27;
    protected override bool CanHealSingleSpell => false;
    private protected override BaseAction Raise => Actions.Resurrection;
    internal struct Actions
    {
        public static readonly BaseAction
            //��ʯ���ٻ�
            SummonCarbuncle = new BaseAction(25798)
            {
                OtherCheck = b => JobGauge.ReturnSummon == Dalamud.Game.ClientState.JobGauge.Enums.SummonPet.NONE,
            },

            //����֮�� �Ÿ�
            SearingLight = new BaseAction(25801),

            //�ػ�֮�� ���Լ�����
            RadiantAegis = new BaseAction(25799),

            //ҽ��
            Physick = new BaseAction(16230, true),

            //��̫���� 
            Aethercharge = new BaseAction(25800),

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
            Gemshine = new BaseAction(25883)
            {
                OtherCheck = b => JobGauge.Attunement > 0,
            },

            //���� ���幥��
            Ruin = new BaseAction(163),

            //��ʯ��
            PreciousBrilliance = new BaseAction(25884)
            {
                OtherCheck = b => JobGauge.Attunement > 0,
            },

            //���� ��Χ�˺�
            Outburst = new BaseAction(16511),

            //����
            Resurrection = new BaseAction(173, true),

            //��������
            EnergyDrain = new BaseAction(16508)
            {
                BuffsProvide = new ushort[] { ObjectStatus.FurtherRuin },
            },

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
                OtherCheck = b => JobGauge.ReturnSummonGlam == Dalamud.Game.ClientState.JobGauge.Enums.PetGlam.CARBUNCLE,
            },

            //���Ǻ˱�
            Deathflare = new BaseAction(3582)
            {
                OtherCheck = b => (byte)JobGauge.ReturnSummon == 10,
            },

            //����֮��
            Rekindle = new BaseAction(25830, true)
            {
                OtherCheck = b => (byte)JobGauge.ReturnSummon == 20,
            },

            //�������
            CrimsonCyclone = new BaseAction(25835)
            {
                BuffsNeed = new ushort[] { ObjectStatus.IfritsFavor },
            },

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

    private protected override bool BreakAbility(byte abilityRemain, out BaseAction act)
    {
        //����֮��
        if (Actions.SearingLight.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out BaseAction act)
    {
        //��ʯ���ٻ�
        if (Actions.SummonCarbuncle.ShouldUseAction(out act)) return true;

        //����
        if (Actions.CrimsonCyclone.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.Slipstream.ShouldUseAction(out act, mustUse: true)) return true;

        if (Actions.RuinIV.ShouldUseAction(out act, mustUse:true)) return true;
        if (Actions.EnkindleBahamut.ShouldUseAction(out act, mustUse:true)) return true;

        //�ٻ�
        if (Actions.Aethercharge.ShouldUseAction(out act)) return true;
        if (JobGauge.Attunement == 0)
        {
            if (Actions.SummonEmerald.ShouldUseAction(out act)) return true;
            if (Actions.SummonRuby.ShouldUseAction(out act)) return true;
            if (Actions.SummonTopaz.ShouldUseAction(out act)) return true;
        }

        //AOE
        if (Actions.PreciousBrilliance.ShouldUseAction(out act)) return true;
        if (Actions.Outburst.ShouldUseAction(out act)) return true;

        //����
        if (Actions.Gemshine.ShouldUseAction(out act)) return true;
        if (Actions.Ruin.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.Deathflare.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.Rekindle.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.MountainBuster.ShouldUseAction(out act, mustUse: true)) return true;


        //��������
        if (JobGauge.HasAetherflowStacks)
        {
            if (Actions.Painflare.ShouldUseAction(out act)) return true;
            if (Actions.Fester.ShouldUseAction(out act)) return true;
        }
        else if (Actions.EnergyDrain.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out BaseAction act)
    {
        //�ػ�֮��
        if (Actions.RadiantAegis.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out BaseAction act)
    {
        //ҽ��
        if (Actions.Physick.ShouldUseAction(out act)) return true;

        return false;
    }
}
