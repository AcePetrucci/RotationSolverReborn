using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboPlus.Combos;

internal abstract class BRDCombo : CustomComboJob<BRDGauge>
{
    //����������û�п�����ǿ��
    protected static bool IsBreaking => BaseAction.HaveStatus(BaseAction.FindStatusSelfFromSelf(125));

    internal struct Actions
    {
        private static bool AddOnDot(ushort status1, ushort  status2)
        {
            var sta1 = BaseAction.FindStatusTargetFromSelf(status1);
            var sta2 = BaseAction.FindStatusTargetFromSelf(status2);

            return BaseAction.HaveStatus(sta1) && BaseAction.HaveStatus(sta2)
                 && (!BaseAction.EnoughStatus(sta1) || !BaseAction.EnoughStatus(sta2));
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
                BuffsProvide = new ushort[] { ObjectStatus.StraightShotReady } ,
                OtherCheck = () =>
                {
                    if (!Service.Configuration.IsTargetBoss) return true;
                    return !VenomousBite.TryUseAction(Service.ClientState.LocalPlayer.Level, out _)
                    && !Windbite.TryUseAction(Service.ClientState.LocalPlayer.Level, out _)
                                        && !IronJaws.TryUseAction(Service.ClientState.LocalPlayer.Level, out _);
                },
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
            WardensPaean = new BaseAction(3561),

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
            };
    }

    protected bool CanAddAbility(byte level, out uint action)
    {
        action = 0;

        if (CanInsertAbility)
        {
            //�������С������
            if (Actions.WanderersMinuet.TryUseAction(level, out action)) return true;

            //��������
            if (Actions.PitchPerfect.TryUseAction(level, out action)) return true;

            //���ߵ�����ҥ
            if (JobGauge.SongTimer < 3000 && Actions.MagesBallad.TryUseAction(level, out action)) return true;

            //�����������
            if (JobGauge.SongTimer < 9000 && (JobGauge.Song == Dalamud.Game.ClientState.JobGauge.Enums.Song.MAGE
                || JobGauge.Song == Dalamud.Game.ClientState.JobGauge.Enums.Song.NONE)&& Actions.ArmysPaeon.TryUseAction(level, out action)) return true;

            //����ǿ��
            if (Actions.RagingStrikes.TryUseAction(level, out action)) return true;

            //ս��֮��
            if (Actions.BattleVoice.TryUseAction(level, out action, mustUse:true)) return true;

            //���������������
            if (Actions.RadiantFinale.TryUseAction(level, out action)) return true;

            //���Ҽ�
            if (Actions.Barrage.TryUseAction(level, out action)) return true;

            //��������
            if (Actions.EmpyrealArrow.TryUseAction(level, out action)) return true;

            //����յ���
            if (Actions.Sidewinder.TryUseAction(level, out action)) return true;

            //��������
            if (Actions.RainofDeath.TryUseAction(level, out action, Empty: level == 90 ? IsBreaking : true)) return true;

            //ʧѪ��
            if (Actions.Bloodletter.TryUseAction(level, out action, Empty: level == 90 ? IsBreaking : true)) return true;


            //��������������
            if (Actions.NaturesMinne.TryUseAction(level, out action)) return true;

            //����������޿���
            if (Actions.WardensPaean.TryUseAction(level, out action)) return true;

            //����
            if (GeneralActions.FootGraze.TryUseAction(level, out action)) return true;

            //����
            if (GeneralActions.LegGraze.TryUseAction(level, out action)) return true;

            //�ڵ�
            if (GeneralActions.SecondWind.TryUseAction(level, out action)) return true;
        }
        return false;
    }

}
