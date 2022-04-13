using Dalamud.Game.ClientState.JobGauge.Types;
using System.Linq;
namespace XIVComboPlus.Combos;

internal abstract class WARCombo : CustomComboJob<WARGauge>
{
    internal static bool HaveShield => BaseAction.FindStatusSelf(ObjectStatus.Defiance) != null;

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
                BuffsProvide = new ushort[] { ObjectStatus.SurgingTempest },
            },

            //�ɸ�
            Tomahawk = new BaseAction(46),

            //�͹�
            Onslaught = new BaseAction(7386),

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
                OtherCheck = () => TargetHelper.GetObjectInRadius(TargetHelper.HostileTargets, 3).Length > 0,
            },

            //��
            Berserk = new BaseAction(38)
            {
                OtherCheck = () => TargetHelper.GetObjectInRadius(TargetHelper.HostileTargets, 3).Length > 0,
            },

            //ս��
            ThrillofBattle = new BaseAction(40),

            //̩Ȼ����
            Equilibrium = new BaseAction(3552)
            {
                OtherCheck = () => (float)Service.ClientState.LocalPlayer.CurrentHp / Service.ClientState.LocalPlayer.MaxHp < 0.6,
            },

            //ԭ��������
            NascentFlash = new BaseAction(16464),

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
                OtherCheck = () => (float)Service.ClientState.LocalPlayer.CurrentHp / Service.ClientState.LocalPlayer.MaxHp < 0.1,
            },

            ////ԭ���Ľ��
            //InnerRelease = new BaseAction(7389),

            //���ı���
            PrimalRend = new BaseAction(25753)
            {
                BuffsNeed = new ushort[] { ObjectStatus.PrimalRendReady},
            };
    }

    protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
    {
        uint act;

        if (HaveShield && TargetHelper.ProvokeTarget().Length > 0)
        {
            //����һ�£�
            if (Actions.Tomahawk.TryUseAction(level, out act)) return act;
            if (GeneralActions.Provoke.TryUseAction(level, out act)) return act;
        }

        //�޻����
        //��������
        if (Actions.SteelCyclone.TryUseAction(level, out act)) return act;
        //ԭ��֮��
        if (Actions.InnerBeast.TryUseAction(level, out act)) return act;
        //�Ÿ��� ���ı��� ����ǰ��
        if (Actions.PrimalRend.TryUseAction(level, out act)) return act;

        //Ⱥ��
        if (Actions.MythrilTempest.TryUseAction(level, out act)) return act;
        if (Actions.Overpower.TryUseAction(level, out act)) return act;

        //����
        if (Actions.StormsEye.TryUseAction(level, out act)) return act;
        if (Actions.StormsPath.TryUseAction(level, out act)) return act;
        if (Actions.Maim.TryUseAction(level, out act)) return act;
        if (Actions.HeavySwing.TryUseAction(level, out act)) return act;

        return 0;
    }

    protected bool CanAddAbility(byte level, out uint act)
    {
        act = 0;

        if (CanInsertAbility)
        {
            if (!IsMoving && CanAddRampart(level, out act)) return true;

            //����
            //��
            if (Actions.Berserk.TryUseAction(level, out act)) return true;
            //ս��
            if (Actions.Infuriate.TryUseAction(level, out act)) return true;

            //̩Ȼ���� ���̰���
            if (Actions.Equilibrium.TryUseAction(level, out act)) return true;

            //��ͨ����
            //Ⱥɽ¡��
            if (Actions.Orogeny.TryUseAction(level, out act)) return true;
            //���� 
            if (Actions.Overpower.TryUseAction(level, out act)) return true;

        }
        return false;
    }

    private bool CanAddRampart(byte level, out uint act)
    {
        act = 0;

        //���� ���л�����ˡ�
        if (Actions.Holmgang.TryUseAction(level, out act)) return true;

        //�����˺�
        //���𣨼���30%��
        if (Actions.Vengeance.TryUseAction(level, out act)) return true;

        //���ڣ�����20%��
        if (GeneralActions.Rampart.TryUseAction(level, out act)) return true;

        //ԭ����ֱ��������10%��
        if (Actions.RawIntuition.TryUseAction(level, out act)) return true;

        //���͹���
        //ѩ��
        if (GeneralActions.Reprisal.TryUseAction(level, out act)) return true;

        //��������
        if (GeneralActions.ArmsLength.TryUseAction(level, out act)) return true;
        
        //����Ѫ��
        //���� �����׶�
        if (Actions.ShakeItOff.TryUseAction(level, out act)) return true;
        //ս��
        if (Actions.ThrillofBattle.TryUseAction(level, out act)) return true;
        return false;
    }

}
