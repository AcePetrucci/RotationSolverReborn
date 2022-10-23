using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.CustomCombo;

namespace XIVAutoAttack.Combos.RangedPhysicial;

internal class BRDCombo : JobGaugeCombo<BRDGauge>
{

    internal override uint JobID => 23;
    private static bool initFinished = false;
    private static float RagingStrikes1GCDDelayTime = 0;
    private static float RadiantFinaleCurrent0GCDRemain = 0;
    private static DateTime RagingStrikesNowTime;

    internal struct Actions
    {
        private static bool AddOnDot(BattleChara b, ushort status1, ushort status2, float duration)
        {
            var results = StatusHelper.FindStatusTimes(b, status1, status2);
            if (results.Length != 2) return false;
            return results.Min() < duration;
        }

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
                    bool needLow = AddOnDot(b, ObjectStatus.VenomousBite, ObjectStatus.Windbite, 4);
                    bool needHigh = AddOnDot(b, ObjectStatus.CausticBite, ObjectStatus.Stormbite, 4);

                    bool needLow1 = AddOnDot(b, ObjectStatus.VenomousBite, ObjectStatus.Windbite, 40);
                    bool needHigh1 = AddOnDot(b, ObjectStatus.CausticBite, ObjectStatus.Stormbite, 40);

                    if (LocalPlayer.HaveStatus(ObjectStatus.RagingStrikes) && 
                        LocalPlayer.FindStatusTime(ObjectStatus.RagingStrikes) < 4 
                        && (needLow1 || needHigh1)) return true;

                    return needLow || needHigh;
                },
            },

            //���ߵ�����ҥ
            MagesBallad = new(114),

            //�����������
            ArmysPaeon = new(116),

            //�������С������
            WanderersMinuet = new(3559),

            //ս��֮��
            BattleVoice = new(118, true)
            {
                OtherCheck = b =>
                {
                    if (IsLastAbility(true, RadiantFinale)) return true;

                    return false;
                },
            },

            //����ǿ��
            RagingStrikes = new(101)
            {
                OtherCheck = b =>
                {
                    if (JobGauge.Song == Song.WANDERER || Level < WanderersMinuet.Level && BattleVoice.RecastTimeRemain <= 5.38f 
                        || Level < BattleVoice.Level) return true;

                    return false;
                },
                AfterUse = () =>
                {
                    RagingStrikes1GCDDelayTime = WeaponRemain(1);
                    RagingStrikesNowTime = DateTime.Now;
                }
            },

            //���������������
            RadiantFinale = new(25785, true)
            {
                OtherCheck = b =>
                {
                    var canUse = DateTime.Now - RagingStrikesNowTime >= new TimeSpan(0, 0, 0, 0, (int)(RagingStrikes1GCDDelayTime * 1000));
                    static bool SongIsNotNone(Song value) => value != Song.NONE;
                    static bool SongIsWandererMinuet(Song value) => value == Song.WANDERER;
                    if ((Array.TrueForAll(JobGauge.Coda, SongIsNotNone) || Array.Exists(JobGauge.Coda, SongIsWandererMinuet))
                        && BattleVoice.RecastTimeRemain < 1f
                        && RagingStrikes.IsCoolDown 
                        && LocalPlayer.HaveStatus(ObjectStatus.RagingStrikes)
                        && canUse) return true;

                    return false;
                },
                AfterUse = () =>
                {
                    RadiantFinaleCurrent0GCDRemain = WeaponRemain();
                }
            },

            //���Ҽ�
            Barrage = new(107)
            {
                BuffsProvide = new[] { ObjectStatus.StraightShotReady },
                OtherCheck = b =>
                {
                    if (!EmpyrealArrow.IsCoolDown || EmpyrealArrow.RecastTimeRemain < 2) return false;
                    return true;
                }
            },

            //��������
            EmpyrealArrow = new(3558)
            {
                OtherCheck = b =>
                {
                    if (!initFinished || (initFinished && BattleVoice.RecastTimeRemain >= WeaponRemain(1))) return true;

                    return false;
                },
            },

            //��������
            PitchPerfect = new(7404)
            {
                OtherCheck = b =>
                {
                    if (JobGauge.Song != Song.WANDERER) return false;

                    if (initFinished && (!initFinished || BattleVoice.RecastTimeRemain < 3.5f)) return false;

                    if (JobGauge.SongTimer < 3000 && JobGauge.Repertoire > 0) return true;
                    if (JobGauge.Repertoire == 3 || JobGauge.Repertoire == 2 && EmpyrealArrow.RecastTimeRemain < 2) return true;
                    return false;
                },
            },

            //ʧѪ��
            Bloodletter = new(110)
            {
                OtherCheck = b =>
                {
                    if (!EmpyrealArrow.IsCoolDown || EmpyrealArrow.RecastTimeRemain < 1) return false;
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
            Sidewinder = new(3562)
            {
                OtherCheck = b =>
                {
                    if (!initFinished || (initFinished && BattleVoice.RecastTimeRemain >= 3.5f))
                    {
                        if (LocalPlayer.HaveStatus(ObjectStatus.BattleVoice) || BattleVoice.RecastTimeRemain > 10
                        && LocalPlayer.HaveStatus(ObjectStatus.RadiantFinale) || RadiantFinale.RecastTimeRemain > 10
                        || Level < RadiantFinale.Level) return true;
                    }

                    return false;
                },
            },

            //�����
            ApexArrow = new(16496)
            {
                OtherCheck = b =>
                {
                    //�챬����,���ŵȱ���
                    if (JobGauge.SoulVoice == 100 && BattleVoice.RecastTimeRemain <= 25) return false;

                    //���������,������ﻹ�о����,�ͰѾ�������ȥ
                    if (JobGauge.SoulVoice >= 80 && LocalPlayer.HaveStatus(ObjectStatus.RagingStrikes) && LocalPlayer.FindStatusTime(ObjectStatus.RagingStrikes) < 10) return true;

                    if (JobGauge.SoulVoice == 100
                        && LocalPlayer.HaveStatus(ObjectStatus.RagingStrikes)
                        && LocalPlayer.HaveStatus(ObjectStatus.BattleVoice)
                        && (LocalPlayer.HaveStatus(ObjectStatus.RadiantFinale) || Level < RadiantFinale.Level)) return true;

                    if (JobGauge.Song == Song.MAGE && JobGauge.SoulVoice >= 80 && JobGauge.SongTimer < 22 && JobGauge.SongTimer > 18) return true;

                    //����֮������100�����ڱ�����Ԥ��״̬
                    if (JobGauge.SoulVoice == 100 || LocalPlayer.HaveStatus(ObjectStatus.BlastArrowReady)) return true;

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
    }
    internal override SortedList<DescType, string> Description => new()
    {
        {DescType.��Χ����, $"{Actions.Troubadour.Action.Name}"},
        {DescType.��������, $"{Actions.NaturesMinne.Action.Name}"},
    };
    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //����
        if (Actions.Troubadour.ShouldUse(out act)) return true;


        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        //��������������
        if (Actions.NaturesMinne.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        if (!InBattle)
        {
            //if (UseBreakItem(out act)) return true;
            initFinished = false;
        }

        //��������
        if (Actions.IronJaws.ShouldUse(out act))
        {
            initFinished = true;
            return true;
        }

        //�Ŵ��У�
        if (Actions.ApexArrow.ShouldUse(out act, mustUse: true)) return true;

        //Ⱥ��GCD
        if (Actions.Shadowbite.ShouldUse(out act)) return true;
        if (Actions.QuickNock.ShouldUse(out act)) return true;

        //ֱ�����
        if (Actions.StraitShoot.ShouldUse(out act)) return true;

        //�϶�
        if (Actions.VenomousBite.ShouldUse(out act)) return true;
        if (Actions.Windbite.ShouldUse(out act)) return true;

        //ǿ�����
        if (Actions.HeavyShoot.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //���������Ҫ�϶�����Ҫֱ������������ˡ�
        if (nextGCD == Actions.StraitShoot || nextGCD == Actions.VenomousBite ||
            nextGCD == Actions.Windbite || nextGCD == Actions.IronJaws)
        {
            return base.EmergercyAbility(abilityRemain, nextGCD, out act);
        }
        else if (abilityRemain != 0 &&
            (Level < Actions.RagingStrikes.Level || LocalPlayer.HaveStatus(ObjectStatus.RagingStrikes)) &&
            (Level < Actions.BattleVoice.Level || LocalPlayer.HaveStatus(ObjectStatus.BattleVoice)))
        {
            //���Ҽ�
            if (Actions.Barrage.ShouldUse(out act)) return true;
        }

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);

    }

    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        if (JobGauge.Song == Song.NONE || Level < Actions.MagesBallad.Level)
        {
            act = null!;
            return false;
        }

        //����ǿ��
        if (Actions.RagingStrikes.ShouldUse(out act)) return true;

        //���������������
        if (abilityRemain == 2 && Actions.RadiantFinale.ShouldUse(out act, mustUse: true)) return true;

        //ս��֮��
        if (abilityRemain == 1 && IsLastAbility(true, Actions.RadiantFinale) && Actions.BattleVoice.ShouldUse(out act, mustUse: true)) return true;


        act = null!;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.RadiantFinale.IsCoolDown && LocalPlayer.FindStatusTime(ObjectStatus.RadiantFinale) > RadiantFinaleCurrent0GCDRemain)
        {
            act = null;
            return false;
        }
        //�������С������
        if ((JobGauge.Song == Song.NONE || ((JobGauge.Song != Song.NONE || LocalPlayer.HaveStatus(ObjectStatus.ArmyEthos)) && abilityRemain == 1))
            && JobGauge.SongTimer < 3000)
        {
            if (Actions.WanderersMinuet.ShouldUse(out act)) return true;
        }

        //��������
        if (JobGauge.Song != Song.NONE && Actions.EmpyrealArrow.ShouldUse(out act)) return true;

        //��������
        if (Actions.PitchPerfect.ShouldUse(out act)) return true;

        //���ߵ�����ҥ
        if (JobGauge.SongTimer < 3000 && Actions.MagesBallad.ShouldUse(out act)) return true;

        //�����������
        if (JobGauge.SongTimer < 12000 && (JobGauge.Song == Song.MAGE
            || JobGauge.Song == Song.NONE) && Actions.ArmysPaeon.ShouldUse(out act)) return true;

        //����յ���
        if (Actions.Sidewinder.ShouldUse(out act)) return true;

        //����������û�п�����ǿ����ս��֮��
        bool empty = (LocalPlayer.HaveStatus(ObjectStatus.RagingStrikes) 
            && (LocalPlayer.HaveStatus(ObjectStatus.BattleVoice) 
            || Level < Actions.BattleVoice.Level)) || JobGauge.Song == Song.MAGE;
        //��������
        if (Actions.RainofDeath.ShouldUse(out act, emptyOrSkipCombo: empty)) return true;

        //ʧѪ��
        if (Actions.Bloodletter.ShouldUse(out act, emptyOrSkipCombo: empty)) return true;

        return false;
    }
}
