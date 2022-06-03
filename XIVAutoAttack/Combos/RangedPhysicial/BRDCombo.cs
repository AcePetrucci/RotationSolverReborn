using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using System.Linq;
namespace XIVComboPlus.Combos;

internal class BRDCombo : CustomComboJob<BRDGauge>
{
    //����������û�п�����ǿ��
    protected static bool IsBreaking => BaseAction.HaveStatusSelfFromSelf(125);

    internal override uint JobID => 23;

    protected override bool CanHealSingleAbility => false;
    internal struct Actions
    {
        private static bool AddOnDot(BattleChara b, ushort status1, ushort status2)
        {
            var results = BaseAction.FindStatusFromSelf(b, status1, status2);
            if (results.Length != 2) return false;
            return results.Min() < 6f;
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
                OtherCheck = b =>
                {
                    bool needLow = AddOnDot(b, ObjectStatus.VenomousBite, ObjectStatus.Windbite);
                    bool needHigh = AddOnDot(b, ObjectStatus.CausticBite, ObjectStatus.Stormbite);
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
                OtherCheck = b =>
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
                OtherCheck = b => TargetHelper.WeakenPeople.Length > 0,
            },

            //ս��֮��
            BattleVoice = new BaseAction(118, true),

            //��������������
            NaturesMinne = new BaseAction(7408),

            //����յ���
            Sidewinder = new BaseAction(3562),

            //�����
            ApexArrow = new BaseAction(16496),



            //���������������
            RadiantFinale = new BaseAction(25785, true),

            //����
            Troubadour = new BaseAction(7405, true)
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

    private protected override bool DefenceAreaAbility(byte abilityRemain, out BaseAction act)
    {
        //����
        if (Actions.Troubadour.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool HealSingleAbility( byte abilityRemain, out BaseAction act)
    {
        //��������������
        if (Actions.NaturesMinne.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out BaseAction act)
    {
        //�Ŵ��У�
        if (JobGauge.SoulVoice == 100 || BaseAction.HaveStatusSelfFromSelf(ObjectStatus.BlastArrowReady))
        {
            if (Actions.ApexArrow.ShouldUseAction(out act, mustUse: true)) return true;
        }

        //Ⱥ��GCD
        if (Actions.Shadowbite.ShouldUseAction(out act)) return true;
        if (Actions.QuickNock.ShouldUseAction(out act)) return true;

        //ֱ�����
        if (Actions.StraitShoot.ShouldUseAction(out act)) return true;

        //�϶�
        if (Actions.IronJaws.ShouldUseAction(out act)) return true;
        if (Actions.VenomousBite.ShouldUseAction(out act)) return true;
        if (Actions.Windbite.ShouldUseAction(out act)) return true;

        //ǿ�����
        if (Actions.HeavyShoot.ShouldUseAction(out act)) return true;


        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, BaseAction nextGCD, out BaseAction act)
    {
        if (base.EmergercyAbility(abilityRemain, nextGCD, out act)) return true;

        //���������Ҫ�϶�����Ҫֱ������������ˡ�
        if (nextGCD.ActionID == Actions.StraitShoot.ActionID || nextGCD.ActionID == Actions.VenomousBite.ActionID ||
            nextGCD.ActionID == Actions.Windbite.ActionID || nextGCD.ActionID == Actions.IronJaws.ActionID)
        {
            act = null;
            return false;
        }
        else if(abilityRemain != 0 && 
            (Service.ClientState.LocalPlayer.Level < Actions.RagingStrikes.Level ||  BaseAction.HaveStatusSelfFromSelf(ObjectStatus.RagingStrikes)) &&
            (Service.ClientState.LocalPlayer.Level < Actions.BattleVoice.Level || BaseAction.HaveStatusSelfFromSelf(ObjectStatus.BattleVoice)))
        {
            //���Ҽ�
            if (Actions.Barrage.ShouldUseAction(out act)) return true;
        }

        act = null;
        return false;
    }

    private protected override bool BreakAbility(byte abilityRemain, out BaseAction act)
    {
        if(JobGauge.Song == Dalamud.Game.ClientState.JobGauge.Enums.Song.NONE || Service.ClientState.LocalPlayer.Level < Actions.MagesBallad.Level)
        {
            act = null;
            return false;
        }

        //����ǿ��
        if (Actions.RagingStrikes.ShouldUseAction(out act)) return true;

        //������ֻ����ս��֮���ͷ�֮ǰ0.6s�ڣ��Ż��ͷš�
        if (Actions.BattleVoice.RecastTimeRemain < 0.6)
        {
            //���������������
            if (Actions.RadiantFinale.ShouldUseAction(out act, mustUse: true)) return true;

            //ս��֮��
            if (Actions.BattleVoice.ShouldUseAction(out act, mustUse: true)) return true;
        }
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out BaseAction act)
    {
        //�������С������
        if (JobGauge.SongTimer < 3000 && Actions.WanderersMinuet.ShouldUseAction(out act)) return true;

        //��������
        if (Actions.PitchPerfect.ShouldUseAction(out act)) return true;

        //���ߵ�����ҥ
        if (JobGauge.SongTimer < 3000 && Actions.MagesBallad.ShouldUseAction(out act)) return true;

        //�����������
        if (JobGauge.SongTimer < 12000 && (JobGauge.Song == Dalamud.Game.ClientState.JobGauge.Enums.Song.MAGE
            || JobGauge.Song == Dalamud.Game.ClientState.JobGauge.Enums.Song.NONE) && Actions.ArmysPaeon.ShouldUseAction(out act)) return true;


        //��������
        if (Actions.EmpyrealArrow.ShouldUseAction(out act)) return true;

        //����յ���
        if (Actions.Sidewinder.ShouldUseAction(out act)) return true;

        byte level = Service.ClientState.LocalPlayer.Level;

        //��������
        if (Actions.RainofDeath.ShouldUseAction(out act, emptyOrSkipCombo: level == 90 ? IsBreaking : true)) return true;

        //ʧѪ��
        if (Actions.Bloodletter.ShouldUseAction(out act, emptyOrSkipCombo: level == 90 ? IsBreaking : true)) return true;

        //����������޿��� ����Debuff
        if (Actions.WardensPaean.ShouldUseAction(out act)) return true;

        return false;
    }

}
