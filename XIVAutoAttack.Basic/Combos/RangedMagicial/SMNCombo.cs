using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.RangedMagicial;

public  abstract class SMNCombo<TCmd> : JobGaugeCombo<SMNGauge, TCmd> where TCmd : Enum
{
    public sealed override uint[] JobIDs => new uint[] { 27, 26 };
    protected override bool CanHealSingleSpell => false;
    private protected  override BaseAction Raise => Resurrection;

    protected static bool InBahamut => Service.IconReplacer.OriginalHook(25822) == Deathflare.ID;
    protected static bool InPhoenix => Service.IconReplacer.OriginalHook(25822) == Rekindle.ID;
    protected static bool InBreak => InBahamut || InPhoenix || !SummonBahamut.EnoughLevel;


    public static readonly BaseAction
        //��ʯҫ
        Gemshine = new(25883)
        {
            OtherCheck = b => JobGauge.Attunement > 0,
        },

        //��ʯ��
        PreciousBrilliance = new(25884)
        {
            OtherCheck = b => JobGauge.Attunement > 0,
        },

        //���� ���幥��
        Ruin = new(163),

        //���� ��Χ�˺�
        Outburst = new(16511),

        //��ʯ���ٻ�
        SummonCarbuncle = new(25798)
        {
            OtherCheck = b => !TargetUpdater.HavePet,
        },

        //����֮�� �Ÿ�
        SearingLight = new(25801)
        {
            OtherCheck = b => InCombat && !InBahamut && !InPhoenix
        },

        //�ػ�֮�� ���Լ�����
        RadiantAegis = new(25799),

        //ҽ��
        Physick = new(16230, true),

        //��̫���� 
        Aethercharge = new(25800)
        {
            OtherCheck = b => InCombat,
        },

        //�����ٻ�
        SummonBahamut = new(7427),

        //�챦ʯ�ٻ�
        SummonRuby = new(25802)
        {
            OtherCheck = b => JobGauge.IsIfritReady && !IsMoving,
        },

        //�Ʊ�ʯ�ٻ�
        SummonTopaz = new(25803)
        {
            OtherCheck = b => JobGauge.IsTitanReady,
        },

        //�̱�ʯ�ٻ�
        SummonEmerald = new(25804)
        {
            OtherCheck = b => JobGauge.IsGarudaReady,
        },


        //����
        Resurrection = new(173, true),

        //��������
        EnergyDrain = new(16508),

        //������ȡ
        EnergySiphon = new(16510),

        //���ñ���
        Fester = new(181),

        //ʹ��˱�
        Painflare = new(3578),

        //�پ�
        RuinIV = new(7426)
        {
            BuffsNeed = new[] { ObjectStatus.FurtherRuin },
        },

        //����ŷ�
        EnkindleBahamut = new(7429)
        {
            OtherCheck = b => InBahamut || InPhoenix,
        },

        //���Ǻ˱�
        Deathflare = new(3582)
        {
            OtherCheck = b => InBahamut,
        },

        //����֮��
        Rekindle = new(25830, true)
        {
            OtherCheck = b => InPhoenix,
        },

        //�������
        CrimsonCyclone = new(25835)
        {
            BuffsNeed = new[] { ObjectStatus.IfritsFavor },
        },

        //���ǿϮ
        CrimsonStrike = new(25885),

        //ɽ��
        MountainBuster = new(25836)
        {
            BuffsNeed = new[] { ObjectStatus.TitansFavor },
        },

        //��������
        Slipstream = new(25837)
        {
            BuffsNeed = new[] { ObjectStatus.GarudasFavor },
        };
}
