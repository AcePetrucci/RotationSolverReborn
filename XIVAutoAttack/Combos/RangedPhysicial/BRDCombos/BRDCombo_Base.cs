using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.RangedPhysicial.BRDCombos;

internal abstract class BRDCombo_Base<TCmd> : JobGaugeCombo<BRDGauge, TCmd> where TCmd : Enum
{
    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Bard, ClassJobID.Archer };

    public static readonly BaseAction
        //ǿ�����
        HeavyShoot = new(97) { BuffsProvide = new[] { StatusID.StraightShotReady } },

        //ֱ�����
        StraitShoot = new(98) { BuffsNeed = new[] { StatusID.StraightShotReady } },

        //��ҩ��
        VenomousBite = new(100, isEot: true) { TargetStatus = new[] { StatusID.VenomousBite, StatusID.CausticBite } },

        //��ʴ��
        Windbite = new(113, isEot: true) { TargetStatus = new[] { StatusID.Windbite, StatusID.Stormbite } },

        //��������
        IronJaws = new(3560, isEot: true)
        {
            OtherCheck = b =>
            {
                if (IsLastWeaponSkill(false, IronJaws)) return false;

                if (Player.HaveStatusFromSelf(StatusID.RagingStrikes) &&
                    Player.WillStatusEndGCD(1, 1, true, StatusID.RagingStrikes)) return true;

                return b.HaveStatusFromSelf(StatusID.VenomousBite, StatusID.CausticBite) & b.HaveStatusFromSelf(StatusID.Windbite, StatusID.Stormbite)
                & (b.WillStatusEndGCD((uint)Service.Configuration.AddDotGCDCount, 0, true, StatusID.VenomousBite, StatusID.CausticBite)
                | b.WillStatusEndGCD((uint)Service.Configuration.AddDotGCDCount, 0, true, StatusID.Windbite, StatusID.Stormbite));
            },
        },

        //���ߵ�����ҥ
        MagesBallad = new(114),

        //�����������
        ArmysPaeon = new(116),

        //�������С������
        WanderersMinuet = new(3559),

        //ս��֮��
        BattleVoice = new(118, true),

        //����ǿ��
        RagingStrikes = new(101),

        //���������������
        RadiantFinale = new(25785, true),

        //���Ҽ�
        Barrage = new(107),

        //��������
        EmpyrealArrow = new(3558),

        //��������
        PitchPerfect = new(7404)
        {
            OtherCheck = b => JobGauge.Song == Song.WANDERER,
        },

        //ʧѪ��
        Bloodletter = new(110),

        //��������
        RainofDeath = new(117),

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
