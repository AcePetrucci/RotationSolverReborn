using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Basic;

internal abstract class SMNCombo_Base<TCmd> : JobGaugeCombo<SMNGauge, TCmd> where TCmd : Enum
{
    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Summoner, ClassJobID.Arcanist };
    protected override bool CanHealSingleSpell => false;
    private sealed protected override BaseAction Raise => Resurrection;

    protected static bool InBahamut => Service.IconReplacer.OriginalHook(ActionID.AstralFlow) == ActionID.Deathflare;
    protected static bool InPhoenix => Service.IconReplacer.OriginalHook(ActionID.AstralFlow) == ActionID.Rekindle;
    protected static bool InBreak => InBahamut || InPhoenix || !SummonBahamut.EnoughLevel;

    //��ʯҫ
    public static BaseAction Gemshine { get; } = new(ActionID.Gemshine)
    {
        OtherCheck = b => JobGauge.Attunement > 0,
    };

    //��ʯ��
    public static BaseAction PreciousBrilliance { get; } = new(ActionID.PreciousBrilliance)
    {
        OtherCheck = b => JobGauge.Attunement > 0,
    };

    //���� ���幥��
    public static BaseAction Ruin { get; } = new(ActionID.RuinSMN);

    //���� ��Χ�˺�
    public static BaseAction Outburst { get; } = new(ActionID.Outburst);

    //��ʯ���ٻ�
    public static BaseAction SummonCarbuncle { get; } = new(ActionID.SummonCarbuncle)
    {
        OtherCheck = b => !TargetUpdater.HavePet,
    };

    //����֮�� �Ÿ�
    public static BaseAction SearingLight { get; } = new(ActionID.SearingLight, true)
    {
        OtherCheck = b => InCombat && !InBahamut && !InPhoenix
    };

    //�ػ�֮�� ���Լ�����
    public static BaseAction RadiantAegis { get; } = new(ActionID.RadiantAegis, true);

    //ҽ��
    public static BaseAction Physick { get; } = new(ActionID.Physick, true);

    //��̫���� 
    public static BaseAction Aethercharge { get; } = new(ActionID.Aethercharge)
    {
        OtherCheck = b => InCombat,
    };

    //�����ٻ�
    public static BaseAction SummonBahamut { get; } = new(ActionID.SummonBahamut);

    //�챦ʯ�ٻ�
    public static BaseAction SummonRuby { get; } = new(ActionID.SummonRuby)
    {
        OtherCheck = b => JobGauge.IsIfritReady && !IsMoving,
    };

    //�Ʊ�ʯ�ٻ�
    public static BaseAction SummonTopaz { get; } = new(ActionID.SummonTopaz)
    {
        OtherCheck = b => JobGauge.IsTitanReady,
    };

    //�̱�ʯ�ٻ�
    public static BaseAction SummonEmerald { get; } = new(ActionID.SummonEmerald)
    {
        OtherCheck = b => JobGauge.IsGarudaReady,
    };


    //����
    public static BaseAction Resurrection { get; } = new(ActionID.ResurrectionSMN, true);

    //��������
    public static BaseAction EnergyDrain { get; } = new(ActionID.EnergyDrainSMN);

    //������ȡ
    public static BaseAction EnergySiphon { get; } = new(ActionID.EnergySiphon);

    //���ñ���
    public static BaseAction Fester { get; } = new(ActionID.Fester);

    //ʹ��˱�
    public static BaseAction Painflare { get; } = new(ActionID.Painflare);

    //�پ�
    public static BaseAction RuinIV { get; } = new(ActionID.RuinIV)
    {
        BuffsNeed = new[] { StatusID.FurtherRuin },
    };

    //����ŷ�
    public static BaseAction EnkindleBahamut { get; } = new(ActionID.EnkindleBahamut)
    {
        OtherCheck = b => InBahamut || InPhoenix,
    };

    //���Ǻ˱�
    public static BaseAction Deathflare { get; } = new(ActionID.Deathflare)
    {
        OtherCheck = b => InBahamut,
    };

    //����֮��
    public static BaseAction Rekindle { get; } = new(ActionID.Rekindle, true)
    {
        OtherCheck = b => InPhoenix,
    };

    //�������
    public static BaseAction CrimsonCyclone { get; } = new(ActionID.CrimsonCyclone)
    {
        BuffsNeed = new[] { StatusID.IfritsFavor },
    };

    //���ǿϮ
    public static BaseAction CrimsonStrike { get; } = new(ActionID.CrimsonStrike);

    //ɽ��
    public static BaseAction MountainBuster { get; } = new(ActionID.MountainBuster)
    {
        BuffsNeed = new[] { StatusID.TitansFavor },
    };

    //��������
    public static BaseAction Slipstream { get; } = new(ActionID.Slipstream)
    {
        BuffsNeed = new[] { StatusID.GarudasFavor },
    };
}
