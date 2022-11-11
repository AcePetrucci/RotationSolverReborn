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

namespace XIVAutoAttack.Combos.RangedPhysicial;

public abstract class BRDCombo<TCmd> : JobGaugeCombo<BRDGauge, TCmd> where TCmd : Enum
{
    public  sealed override uint[] JobIDs => new uint[] { 23, 5 };

    public static readonly BaseAction
        //ǿ�����
        HeavyShoot = new(97) { BuffsProvide = new[] { ObjectStatus.StraightShotReady } },

        //ֱ�����
        StraitShoot = new(98) { BuffsNeed = new[] { ObjectStatus.StraightShotReady } },

        //��ҩ��
        VenomousBite = new(100, isDot: true) { TargetStatus = new[] { ObjectStatus.VenomousBite, ObjectStatus.CausticBite } },

        //��ʴ��
        Windbite = new(113, isDot: true) { TargetStatus = new[] { ObjectStatus.Windbite, ObjectStatus.Stormbite } },

        //��������
        IronJaws = new(3560, isDot: true)
        {
            OtherCheck = b =>
            {
                if (IsLastWeaponSkill(false, IronJaws)) return false;

                if (Player.HaveStatus(ObjectStatus.RagingStrikes) &&
                    Player.WillStatusEndGCD(1, 1, true, ObjectStatus.RagingStrikes)) return true;

                return b.HaveStatus(ObjectStatus.VenomousBite, ObjectStatus.CausticBite) & b.HaveStatus(ObjectStatus.Windbite, ObjectStatus.Stormbite)
                & (b.WillStatusEndGCD((uint)Service.Configuration.AddDotGCDCount, 0, true, ObjectStatus.VenomousBite, ObjectStatus.CausticBite)
                | b.WillStatusEndGCD((uint)Service.Configuration.AddDotGCDCount, 0, true, ObjectStatus.Windbite, ObjectStatus.Stormbite));
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
        RagingStrikes = new(101)
        {
            OtherCheck = b =>
            {
                if (JobGauge.Song == Song.WANDERER || !WanderersMinuet.EnoughLevel && BattleVoice.WillHaveOneChargeGCD(1, 1)
                    || !BattleVoice.EnoughLevel) return true;

                return false;
            },
        },

        //���������������
        RadiantFinale = new(25785, true)
        {
            OtherCheck = b =>
            {
                static bool SongIsNotNone(Song value) => value != Song.NONE;
                static bool SongIsWandererMinuet(Song value) => value == Song.WANDERER;
                if ((Array.TrueForAll(JobGauge.Coda, SongIsNotNone) || Array.Exists(JobGauge.Coda, SongIsWandererMinuet))
                    && BattleVoice.WillHaveOneChargeGCD()
                    && RagingStrikes.IsCoolDown
                    && Player.HaveStatus(ObjectStatus.RagingStrikes)
                    && RagingStrikes.ElapsedAfterGCD(1)) return true;
                return false;
            },
        },

        //���Ҽ�
        Barrage = new(107)
        {
            BuffsProvide = new[] { ObjectStatus.StraightShotReady },
            OtherCheck = b =>
            {
                if (!EmpyrealArrow.IsCoolDown || EmpyrealArrow.WillHaveOneChargeGCD() || JobGauge.Repertoire == 3) return false;
                return true;
            }
        },

        //��������
        EmpyrealArrow = new(3558),

        //��������
        PitchPerfect = new(7404)
        {
            OtherCheck = b => JobGauge.Song == Song.WANDERER,
        },

        //ʧѪ��
        Bloodletter = new(110)
        {
            OtherCheck = b =>
            {
                if (EmpyrealArrow.EnoughLevel && (!EmpyrealArrow.IsCoolDown || EmpyrealArrow.WillHaveOneChargeGCD())) return false;
                return true;
            }
        },

        //��������
        RainofDeath = new(117),

        //�����
        QuickNock = new(106) { BuffsProvide = new[] { ObjectStatus.ShadowbiteReady } },

        //Ӱ�ɼ�
        Shadowbite = new(16494) { BuffsNeed = new[] { ObjectStatus.ShadowbiteReady } },

        //����������޿���
        WardensPaean = new(3561),

        //��������������
        NaturesMinne = new(7408),

        //����յ���
        Sidewinder = new(3562),

        //�����
        ApexArrow = new(16496)
        {
            OtherCheck = b =>
            {
                if (Player.HaveStatus(ObjectStatus.BlastArrowReady) || (QuickNock.ShouldUse(out _) && JobGauge.SoulVoice == 100)) return true;

                //�챬����,���ŵȱ���
                if (JobGauge.SoulVoice == 100 && BattleVoice.WillHaveOneCharge(25)) return false;

                //���������,������ﻹ�о����,�ͰѾ�������ȥ
                if (JobGauge.SoulVoice >= 80 && Player.HaveStatus(ObjectStatus.RagingStrikes) && Player.WillStatusEnd(10, false, ObjectStatus.RagingStrikes)) return true;

                if (JobGauge.SoulVoice == 100
                    && Player.HaveStatus(ObjectStatus.RagingStrikes)
                    && Player.HaveStatus(ObjectStatus.BattleVoice)
                    && (Player.HaveStatus(ObjectStatus.RadiantFinale) || !RadiantFinale.EnoughLevel)) return true;

                if (JobGauge.Song == Song.MAGE && JobGauge.SoulVoice >= 80 && JobGauge.SongTimer < 22 && JobGauge.SongTimer > 18) return true;

                //����֮������100�����ڱ�����Ԥ��״̬
                if (!Player.HaveStatus(ObjectStatus.RagingStrikes) && JobGauge.SoulVoice == 100) return true;

                return false;
            },
        },

        //����
        Troubadour = new(7405, true)
        {
            BuffsProvide = new[]
            {
                    ObjectStatus.Troubadour,
                    ObjectStatus.Tactician1,
                    ObjectStatus.Tactician2,
                    ObjectStatus.ShieldSamba,
            },
        };
    protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (CommandController.EsunaOrShield && TargetUpdater.WeakenPeople.Length > 0 || TargetUpdater.DyingPeople.Length > 0)
        {
            if (WardensPaean.ShouldUse(out act, mustUse: true)) return true;
        }
        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }
}
