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

    internal struct Actions
    {
        private static bool AddOnDot(BattleChara b, ushort status1, ushort status2, float duration)
        {
            var results = StatusHelper.FindStatusFromSelf(b, status1, status2);
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

                    if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.RagingStrikes) && 
                        StatusHelper.FindStatusTimeSelfFromSelf(ObjectStatus.RagingStrikes) < 4 
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
                    if (StatusHelper.FindStatusTimeSelfFromSelf(ObjectStatus.RagingStrikes) <= 16.5f
                    || initFinished) return true;

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
            },

            //���������������
            RadiantFinale = new(25785, true)
            {
                OtherCheck = b =>
                {
                    static bool SongIsNotNone(Song value) => value != Song.NONE;
                    static bool SongIsWandererMinuet(Song value) => value == Song.WANDERER;
                    if ((Array.TrueForAll(JobGauge.Coda, SongIsNotNone) || Array.Exists(JobGauge.Coda, SongIsWandererMinuet))
                        && BattleVoice.RecastTimeRemain < 0.7f
                        && (StatusHelper.FindStatusTimeSelfFromSelf(ObjectStatus.RagingStrikes) <= 16.5f || initFinished)
                        && RagingStrikes.IsCoolDown
                        && RagingStrikes.RecastTimeElapsed > 3) return true;

                    return false;
                },
            },

            //���Ҽ�
            Barrage = new(107)
            {
                BuffsProvide = new[] { ObjectStatus.StraightShotReady },
            },

            //��������
            EmpyrealArrow = new(3558)
            {
                OtherCheck = b =>
                {
                    if (!initFinished || (initFinished && BattleVoice.RecastTimeRemain >= 3.5f)) return true;

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
            Bloodletter = new(110),

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
                        if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.BattleVoice) || BattleVoice.RecastTimeRemain > 10
                        && StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.RadiantFinale) || RadiantFinale.RecastTimeRemain > 10
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
                    if (JobGauge.SoulVoice == 100 && BattleVoice.RecastTimeRemain <= 25) return false;

                    if (JobGauge.SoulVoice >= 80 && StatusHelper.FindStatusTimeSelfFromSelf(ObjectStatus.RagingStrikes) < 10) return true;

                    if (JobGauge.SoulVoice == 100
                        && StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.RagingStrikes)
                        && StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.BattleVoice)
                        && (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.RadiantFinale) || Level < RadiantFinale.Level)) return true;

                    //����֮������100�����ڱ�����Ԥ��״̬
                    if (JobGauge.SoulVoice == 100 || StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.BlastArrowReady)) return true;

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
        if (!TargetHelper.InBattle)
        {
            //if (UseBreakItem(out act)) return true;
            initFinished = false;
        }

        //�Ŵ��У�
        if (JobGauge.SoulVoice == 100 || StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.BlastArrowReady))
        {
            if (Actions.ApexArrow.ShouldUse(out act, mustUse: true)) return true;
        }

        //Ⱥ��GCD
        if (Actions.Shadowbite.ShouldUse(out act)) return true;
        if (Actions.QuickNock.ShouldUse(out act)) return true;

        //ֱ�����
        if (Actions.StraitShoot.ShouldUse(out act)) return true;

        //�϶�
        if (Actions.IronJaws.ShouldUse(out act))
        {
            initFinished = true;
            return true;
        }
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
            (Level < Actions.RagingStrikes.Level || StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.RagingStrikes)) &&
            (Level < Actions.BattleVoice.Level || StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.BattleVoice)))
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
        if (abilityRemain == 1 && LastAbility == Actions.RadiantFinale.ID && Actions.BattleVoice.ShouldUse(out act, mustUse: true)) return true;


        act = null!;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        //�������С������
        if ((TargetHelper.CombatEngageDuration.Minutes == 0 || (TargetHelper.CombatEngageDuration.Minutes > 0 && abilityRemain == 1))
            && JobGauge.SongTimer < 3000)
        {
            if (Actions.WanderersMinuet.ShouldUse(out act)) return true;
        }

        //��������
        if (Actions.PitchPerfect.ShouldUse(out act)) return true;

        //��������
        if (JobGauge.Song != Song.NONE && Actions.EmpyrealArrow.ShouldUse(out act)) return true;


        //���ߵ�����ҥ
        if (JobGauge.SongTimer < 3000 && Actions.MagesBallad.ShouldUse(out act)) return true;

        //�����������
        if (JobGauge.SongTimer < 12000 && (JobGauge.Song == Song.MAGE
            || JobGauge.Song == Song.NONE) && Actions.ArmysPaeon.ShouldUse(out act)) return true;

        //����յ���
        if (Actions.Sidewinder.ShouldUse(out act)) return true;

        //����������û�п�����ǿ����ս��֮��
        bool empty = (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.RagingStrikes) 
            && (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.BattleVoice) 
            || Level < Actions.BattleVoice.Level)) || JobGauge.Song == Song.MAGE;
        //��������
        if (Actions.RainofDeath.ShouldUse(out act, emptyOrSkipCombo: empty)) return true;

        //ʧѪ��
        if (Actions.Bloodletter.ShouldUse(out act, emptyOrSkipCombo: empty)) return true;

        return false;
    }
}
