using Dalamud.Game.ClientState.JobGauge.Types;
using System.Linq;
namespace XIVComboPlus.Combos;

internal abstract class BRDCombo : CustomComboJob<BRDGauge>
{
    //����������û�п�����ǿ��
    protected static bool IsBreaking => BaseAction.HaveStatusSelfFromSelf(125);

    internal struct Actions
    {
        private static bool AddOnDot(ushort status1, ushort status2)
        {
            var results = BaseAction.FindStatusTargetFromSelf(status1, status2);
            if(results.Length != 2) return false;
            return results.Min() < 5.5f;
        }

        public static readonly BaseAction
            //ǿ�����
            HeavyShoot = new BaseAction(97u) { BuffsProvide = new ushort[] { ObjectStatus.StraightShotReady } },

            //ֱ�����
            StraitShoot = new BaseAction(98u) { BuffsNeed = new ushort[] { ObjectStatus.StraightShotReady } },

            //����ǿ��
            RagingStrikes = new BaseAction(101),

            //��ҩ��
            VenomousBite = new BaseAction(100) { TargetStatus = new ushort[] { ObjectStatus.VenomousBite, ObjectStatus.CausticBite } },

            //��ʴ��
            Windbite = new BaseAction(113) { TargetStatus = new ushort[] { ObjectStatus.Windbite, ObjectStatus.Stormbite } },

            //��������
            IronJaws = new BaseAction(3560)
            {
                OtherCheck = () =>
                {
                    bool needLow = AddOnDot(ObjectStatus.VenomousBite, ObjectStatus.Windbite);
                    bool needHigh = AddOnDot(ObjectStatus.CausticBite, ObjectStatus.Stormbite);
                    return needLow || needHigh;
                },
            },

            //���Ҽ�
            Barrage = new BaseAction(107)
            {
                BuffsProvide = new ushort[] { ObjectStatus.StraightShotReady },
            },

            //��������
            EmpyrealArrow = new BaseAction(3558),

            //ʧѪ��
            Bloodletter = new BaseAction(110),

            //��������
            RainofDeath = new BaseAction(117),

            //�����
            QuickNock = new BaseAction(106) { BuffsProvide = new ushort[] { ObjectStatus.ShadowbiteReady } },

            //Ӱ�ɼ�
            Shadowbite = new BaseAction(16494) { BuffsNeed = new ushort[] { ObjectStatus.ShadowbiteReady } },

            //���ߵ�����ҥ
            MagesBallad = new BaseAction(114),

            //�����������
            ArmysPaeon = new BaseAction(116),

            //�������С������
            WanderersMinuet = new BaseAction(3559),

            //��������
            PitchPerfect = new BaseAction(7404)
            {
                OtherCheck = () =>
            {
                if (JobGauge.Song != Dalamud.Game.ClientState.JobGauge.Enums.Song.WANDERER) return false;
                if (JobGauge.SongTimer < 3000 && JobGauge.Repertoire > 0) return true;
                if (JobGauge.Repertoire == 3) return true;
                return false;
            }
            },

            //����������޿���
            WardensPaean = new BaseAction(3561)
            {
                OtherCheck = () => TargetHelper.WeakenPeople.Length > 0,
            },

            //ս��֮��
            BattleVoice = new BaseAction(118),

            //��������������
            NaturesMinne = new BaseAction(7408),

            //����յ���
            Sidewinder = new BaseAction(3562),

            //�����
            ApexArrow = new BaseAction(16496),



            //���������������
            RadiantFinale = new BaseAction(25785)
            {
                OtherCheck = () =>
                {
                    return JobGauge.Coda.Length > 2 || MagesBallad.CoolDown.CooldownRemaining < 0.1;
                },
            },

            //����
            Troubadour = new BaseAction(7405)
            {
                BuffsProvide = new ushort[]
                {
                    ObjectStatus.Troubadour,
                    ObjectStatus.Tactician1,
                    ObjectStatus.Tactician2,
                    ObjectStatus.ShieldSamba,
                },
            };
    }

    private protected override bool HealSingleAbility(byte level, byte abilityRemain, out BaseAction act)
    {
        //��������������
        if (Actions.NaturesMinne.TryUseAction(level, out act)) return true;

        return false;
    }

    private protected override bool GeneralGCD(byte level, uint lastComboActionID, out BaseAction act)
    {
        //�Ŵ��У�
        if (JobGauge.SoulVoice >= 80 || BaseAction.HaveStatusSelfFromSelf(ObjectStatus.BlastArrowReady))
        {
            if (Actions.ApexArrow.TryUseAction(level, out act, mustUse: true)) return true;
        }

        //Ⱥ��GCD
        if (Actions.Shadowbite.TryUseAction(level, out act)) return true;
        if (Actions.QuickNock.TryUseAction(level, out act)) return true;

        //ֱ�����
        if (Actions.StraitShoot.TryUseAction(level, out act)) return true;

        //�϶�
        if (Actions.IronJaws.TryUseAction(level, out act)) return true;
        if (Actions.VenomousBite.TryUseAction(level, out act)) return true;
        if (Actions.Windbite.TryUseAction(level, out act)) return true;

        //ǿ�����
        if (Actions.HeavyShoot.TryUseAction(level, out act)) return true;

        
        return false;
    }

    private protected override bool FirstActionAbility(byte level, byte abilityRemain, BaseAction nextGCD, out BaseAction act)
    {
        //���������Ҫ�϶�����Ҫֱ������������ˡ�
        if(nextGCD.ActionID == Actions.StraitShoot.ActionID || nextGCD.ActionID == Actions.VenomousBite.ActionID ||
            nextGCD.ActionID == Actions.Windbite.ActionID || nextGCD.ActionID == Actions.IronJaws.ActionID)
        {
            act= null;
            return false;
        }
        else
        {
            //���Ҽ�
            if (Actions.Barrage.TryUseAction(level, out act)) return true;
        }

        act = null;
        return false;
    }

    private protected override bool ForAttachAbility(byte level, byte abilityRemain, out BaseAction act)
    {
        //�������С������
        if (Actions.WanderersMinuet.TryUseAction(level, out act)) return true;

        //��������
        if (Actions.PitchPerfect.TryUseAction(level, out act)) return true;

        //���ߵ�����ҥ
        if (JobGauge.SongTimer < 3000 && Actions.MagesBallad.TryUseAction(level, out act)) return true;

        //�����������
        if (JobGauge.SongTimer < 12000 && (JobGauge.Song == Dalamud.Game.ClientState.JobGauge.Enums.Song.MAGE
            || JobGauge.Song == Dalamud.Game.ClientState.JobGauge.Enums.Song.NONE) && Actions.ArmysPaeon.TryUseAction(level, out act)) return true;

        //����ǿ��
        if (Actions.RagingStrikes.TryUseAction(level, out act)) return true;

        //ս��֮��
        if (Actions.BattleVoice.TryUseAction(level, out act, mustUse: true)) return true;

        //���������������
        if (Actions.RadiantFinale.TryUseAction(level, out act)) return true;

        //��������
        if (Actions.EmpyrealArrow.TryUseAction(level, out act)) return true;

        //����յ���
        if (Actions.Sidewinder.TryUseAction(level, out act)) return true;

        //��������
        if (Actions.RainofDeath.TryUseAction(level, out act, Empty: level == 90 ? IsBreaking : true)) return true;

        //ʧѪ��
        if (Actions.Bloodletter.TryUseAction(level, out act, Empty: level == 90 ? IsBreaking : true)) return true;

        //����������޿��� ����Debuff
        if (Actions.WardensPaean.TryUseAction(level, out act)) return true;

        //����
        if (GeneralActions.FootGraze.TryUseAction(level, out act)) return true;

        //����
        if (GeneralActions.LegGraze.TryUseAction(level, out act)) return true;

        //�ڵ�
        if (GeneralActions.SecondWind.TryUseAction(level, out act)) return true;

        return false;
    }

}
