using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Basic;

internal abstract class BRDCombo_Base<TCmd> : JobGaugeCombo<BRDGauge, TCmd> where TCmd : Enum
{
    public sealed override ClassJobID[] JobIDs => new [] { ClassJobID.Bard, ClassJobID.Archer };

    public static readonly BaseAction
        //ǿ�����
        HeavyShoot = new(ActionID.HeavyShoot) { BuffsProvide = new[] { StatusID.StraightShotReady } },

        //ֱ�����
        StraitShoot = new(ActionID.StraitShoot) { BuffsNeed = new[] { StatusID.StraightShotReady } },

        //��ҧ��
        VenomousBite = new(ActionID.VenomousBite, isEot: true) 
        { 
            TargetStatus = new[] { StatusID.VenomousBite, StatusID.CausticBite } 
        },

        //��ʴ��
        Windbite = new(ActionID.Windbite, isEot: true) 
        { 
            TargetStatus = new[] { StatusID.Windbite, StatusID.Stormbite } 
        },

        //��������
        IronJaws = new(ActionID.IronJaws, isEot: true)
        {
            OtherCheck = b =>
            {
                return b.HaveStatus(true, VenomousBite.TargetStatus) 
                    & b.HaveStatus(true, Windbite.TargetStatus)

                & (b.WillStatusEndGCD((uint)Service.Configuration.AddDotGCDCount, 0, true, VenomousBite.TargetStatus)
                | b.WillStatusEndGCD((uint)Service.Configuration.AddDotGCDCount, 0, true, Windbite.TargetStatus));
            },
        },

        //�������С������
        WanderersMinuet = new(ActionID.WanderersMinuet),

        //���ߵ�����ҥ
        MagesBallad = new(ActionID.MagesBallad),

        //�����������
        ArmysPaeon = new(ActionID.ArmysPaeon),

        //ս��֮��
        BattleVoice = new(ActionID.BattleVoice, true),

        //����ǿ��
        RagingStrikes = new(ActionID.RagingStrikes, true),

        //���������������
        RadiantFinale = new(ActionID.RadiantFinale, true)
        {
            OtherCheck = b => JobGauge.Coda.Any(s => s != Song.NONE),
        },

        //���Ҽ�
        Barrage = new(ActionID.Barrage),

        //��������
        EmpyrealArrow = new(ActionID.EmpyrealArrow),

        //��������
        PitchPerfect = new(ActionID.PitchPerfect)
        {
            OtherCheck = b => JobGauge.Song == Song.WANDERER,
        },

        //ʧѪ��
        Bloodletter = new(ActionID.Bloodletter),

        //��������
        RainofDeath = new(ActionID.RainofDeath),

        //�����
        QuickNock = new(106) { BuffsProvide = new[] { StatusID.ShadowbiteReady } },

        //Ӱ�ɼ�
        Shadowbite = new(16494) { BuffsNeed = new[] { StatusID.ShadowbiteReady } },

        //����������޿���
        WardensPaean = new(3561),

        //��������������
        NaturesMinne = new(7408),

        //����յ���
        Sidewinder = new(3562),

        //�����
        ApexArrow = new(16496)
        {
            //    OtherCheck = b =>
            //    {
            //        if (Player.HaveStatus(StatusIDs.BlastArrowReady) || (QuickNock.ShouldUse(out _) && JobGauge.SoulVoice == 100)) return true;

            //        //�챬����,���ŵȱ���
            //        if (JobGauge.SoulVoice == 100 && BattleVoice.WillHaveOneCharge(25)) return false;

            //        //���������,������ﻹ�о����,�ͰѾ�������ȥ
            //        if (JobGauge.SoulVoice >= 80 && Player.HaveStatus(StatusIDs.RagingStrikes) && Player.WillStatusEnd(10, false, StatusIDs.RagingStrikes)) return true;

            //        if (JobGauge.SoulVoice == 100
            //            && Player.HaveStatus(StatusIDs.RagingStrikes)
            //            && Player.HaveStatus(StatusIDs.BattleVoice)
            //            && (Player.HaveStatus(StatusIDs.RadiantFinale) || !RadiantFinale.EnoughLevel)) return true;

            //        if (JobGauge.Song == Song.MAGE && JobGauge.SoulVoice >= 80 && JobGauge.SongTimer < 22 && JobGauge.SongTimer > 18) return true;

            //        //����֮������100�����ڱ�����Ԥ��״̬
            //        if (!Player.HaveStatus(StatusIDs.RagingStrikes) && JobGauge.SoulVoice == 100) return true;

            //        return false;
            //    },
        },

            //����
            Troubadour = new(7405, true)
            {
                BuffsProvide = new[]
            {
                    StatusID.Troubadour,
                    StatusID.Tactician1,
                    StatusID.Tactician2,
                    StatusID.ShieldSamba,
            },
            };
    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //��ĳЩ�ǳ�Σ�յ�״̬��
        if (CommandController.EsunaOrShield && TargetUpdater.WeakenPeople.Length > 0 || TargetUpdater.DyingPeople.Length > 0)
        {
            if (WardensPaean.ShouldUse(out act, mustUse: true)) return true;
        }
        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }
}
