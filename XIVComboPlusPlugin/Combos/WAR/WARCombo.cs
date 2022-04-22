using Dalamud.Game.ClientState.JobGauge.Types;
using System.Linq;
using System.Numerics;

namespace XIVComboPlus.Combos;

internal abstract class WARCombo : CustomComboJob<WARGauge>
{
    internal static bool HaveShield => BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Defiance);
    internal static float BuffTime 
    {
        get
        {
            var time = BaseAction.FindStatusSelfFromSelf(ObjectStatus.SurgingTempest);
            if (time.Length == 0) return 0;
            return time[0];
        }
    }

    internal struct Actions
    {
        public static readonly BaseAction
            //����
            HeavySwing = new BaseAction(31),

            //�ײ���
            Maim = new BaseAction(37),

            //����ն �̸�
            StormsPath = new BaseAction(42),

            //������ �츫
            StormsEye = new BaseAction(45)
            {
                OtherCheck = () => BuffTime < 10,
            },

            //�ɸ�
            Tomahawk = new BaseAction(46),

            //�͹�
            Onslaught = new BaseAction(7386)
            {
                OtherCheck = () => TargetHelper.DistanceToPlayer(Service.TargetManager.Target) > 5,
            },

            //����    
            Upheaval = new BaseAction(7387),

            //��ѹ��
            Overpower = new BaseAction(41),

            //��������
            MythrilTempest = new BaseAction(16462),

            //Ⱥɽ¡��
            Orogeny = new BaseAction(25752),

            //ԭ��֮��
            InnerBeast = new BaseAction(49),

            //��������
            SteelCyclone = new BaseAction(51),

            //ս��
            Infuriate = new BaseAction(52)
            {
                BuffsProvide = new ushort[] { ObjectStatus.InnerRelease },
                OtherCheck = () => TargetHelper.GetObjectInRadius(TargetHelper.HostileTargets, 5).Length > 0 && JobGauge.BeastGauge <= 50,
            },

            //��
            Berserk = new BaseAction(38)
            {
                OtherCheck = () => TargetHelper.GetObjectInRadius(TargetHelper.HostileTargets, 5).Length > 0,
            },

            //ս��
            ThrillofBattle = new BaseAction(40),

            //̩Ȼ����
            Equilibrium = new BaseAction(3552),

            //ԭ��������
            //NascentFlash = new BaseAction(16464),

            ////ԭ����Ѫ��
            //Bloodwhetting = new BaseAction(25751),

            //�ػ�
            Defiance = new BaseAction(48),

            //����
            Vengeance = new BaseAction(44)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
            },

            //ԭ����ֱ��
            RawIntuition = new BaseAction(3551)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
            },

            //����
            ShakeItOff = new BaseAction(7388),

            //����
            Holmgang = new BaseAction(43)
            {
                OtherCheck = () => (float)Service.ClientState.LocalPlayer.CurrentHp / Service.ClientState.LocalPlayer.MaxHp < 0.15,
            },

            ////ԭ���Ľ��
            //InnerRelease = new BaseAction(7389),

            //���ı���
            PrimalRend = new BaseAction(25753)
            {
                BuffsNeed = new ushort[] { ObjectStatus.PrimalRendReady },
            };
    }

    private protected override bool EmergercyGCD(byte level, uint lastComboActionID, out BaseAction act, byte actabilityRemain)
    {
        //�������һ��T���ǻ��������ˣ�
        if (!HaveShield && TargetHelper.PartyTanks.Length < 2)
        {
            if (Actions.Defiance.TryUseAction(level, out act)) return true;
        }
        act = null;
        return false;
    }

    private protected override bool GeneralGCD(byte level, uint lastComboActionID, out BaseAction act)
    {
        //�޻����
        if (JobGauge.BeastGauge >= 50 || BaseAction.HaveStatusSelfFromSelf(ObjectStatus.InnerRelease))
        {
            //��������
            if (Actions.SteelCyclone.TryUseAction(level, out act)) return true;
            //ԭ��֮��
            if (Actions.InnerBeast.TryUseAction(level, out act)) return true;
        }

        //Ⱥ��
        if (Actions.MythrilTempest.TryUseAction(level, out act, lastComboActionID)) return true;
        if (Actions.Overpower.TryUseAction(level, out act, lastComboActionID)) return true;

        //����
        if (Actions.StormsEye.TryUseAction(level, out act, lastComboActionID)) return true;
        if (Actions.StormsPath.TryUseAction(level, out act, lastComboActionID)) return true;
        if (Actions.Maim.TryUseAction(level, out act, lastComboActionID)) return true;
        if (Actions.HeavySwing.TryUseAction(level, out act, lastComboActionID)) return true;

        //�����ţ�����һ���ɡ�
        if (Actions.Tomahawk.TryUseAction(level, out act)) return true;

        return false;
    }

    private protected override bool ForAttachAbility(byte level, byte abilityRemain, out BaseAction act)
    {
        TargetHelper.ProvokeTarget(out bool haveTargetOnme);
        if (!IsMoving && haveTargetOnme)
        {
            //���� ���л�����ˡ�
            if (Actions.Holmgang.TryUseAction(level, out act)) return true;

            //ԭ����ֱ��������10%��
            if (Actions.RawIntuition.TryUseAction(level, out act)) return true;

            //�����˺�
            //���𣨼���30%��
            if (Actions.Vengeance.TryUseAction(level, out act)) return true;

            //���ڣ�����20%��
            if (GeneralActions.Rampart.TryUseAction(level, out act)) return true;

            //���͹���
            //ѩ��
            if (GeneralActions.Reprisal.TryUseAction(level, out act)) return true;

            //��������
            if (GeneralActions.ArmsLength.TryUseAction(level, out act)) return true;

            //����Ѫ��
            ////���� �����׶�
            //if (Actions.ShakeItOff.TryUseAction(level, out act)) return true;

        }

        //����
        if (BuffTime > 3 || level < Actions.MythrilTempest.Level)
        {
            //ս��
            if (Actions.Infuriate.TryUseAction(level, out act)) return true;
            //��
            if (!new BaseAction(7389).CoolDown.IsCooldown && Actions.Berserk.TryUseAction(level, out act)) return true;
            //ս��
            if (Actions.Infuriate.TryUseAction(level, out act, Empty: true)) return true;
        }

        if ((float)Service.ClientState.LocalPlayer.CurrentHp / Service.ClientState.LocalPlayer.MaxHp < 0.6)
        {
            //ս��
            if (Actions.ThrillofBattle.TryUseAction(level, out act)) return true;
            //̩Ȼ���� ���̰���
            if (Actions.Equilibrium.TryUseAction(level, out act)) return true;
        }

        //�̸����Ѱ���
        //if (!haveTargetOnme && Actions.NascentFlash.TryUseAction(level, out act)) return true;

        //��ͨ����
        //Ⱥɽ¡��
        if (Actions.Orogeny.TryUseAction(level, out act)) return true;
        //���� 
        if (Actions.Upheaval.TryUseAction(level, out act)) return true;

        //��㹥��
        var target = Service.TargetManager.Target;
        if(Vector3.Distance( Service.ClientState.LocalPlayer.Position, target.Position) - target.HitboxRadius < 3)
        {
            if (Actions.Onslaught.TryUseAction(level, out act)) return true;
        }

        return false;
    }

}
