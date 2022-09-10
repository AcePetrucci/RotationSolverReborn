using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using XIVAutoAttack;
using XIVAutoAttack.Combos;

namespace XIVAutoAttack.Combos.Tank;

internal class WARCombo : JobGaugeCombo<WARGauge>
{
    internal override uint JobID => 21;
    internal override bool HaveShield => BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Defiance);
    private protected override BaseAction Shield => Actions.Defiance;
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
            //�ػ�
            Defiance = new (48, shouldEndSpecial: true),

            //����
            HeavySwing = new (31),

            //�ײ���
            Maim = new (37),

            //����ն �̸�
            StormsPath = new (42),

            //������ �츫
            StormsEye = new (45)
            {
                OtherCheck = b => BuffTime < 10,
            },

            //�ɸ�
            Tomahawk = new (46)
            {
                FilterForHostile = b => BaseAction.ProvokeTarget(b),
            },

            //�͹�
            Onslaught = new (7386, shouldEndSpecial: true),

            //����    
            Upheaval = new (7387),

            //��ѹ��
            Overpower = new (41),

            //��������
            MythrilTempest = new (16462),

            //Ⱥɽ¡��
            Orogeny = new (25752),

            //ԭ��֮��
            InnerBeast = new (49),

            //��������
            SteelCyclone = new (51),

            //ս��
            Infuriate = new (52)
            {
                BuffsProvide = new [] { ObjectStatus.InnerRelease },
                OtherCheck = b => BaseAction.GetObjectInRadius(TargetHelper.HostileTargets, 5).Length > 0 && JobGauge.BeastGauge <= 50,
            },

            //��
            Berserk = new (38)
            {
                OtherCheck = b => BaseAction.GetObjectInRadius(TargetHelper.HostileTargets, 5).Length > 0,
            },

            //ս��
            ThrillofBattle = new (40),

            //̩Ȼ����
            Equilibrium = new (3552),

            //ԭ��������
            NascentFlash = new (16464)
            {
                ChoiceFriend = BaseAction.FindAttackedTarget,
            },

            ////ԭ����Ѫ��
            //Bloodwhetting = new BaseAction(25751),

            //����
            Vengeance = new (44)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
            },

            //ԭ����ֱ��
            RawIntuition = new (3551)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
            },

            //����
            ShakeItOff = new (7388),

            //����
            Holmgang = new (43)
            {
                OtherCheck = BaseAction.TankBreakOtherCheck,
            },

            ////ԭ���Ľ��
            //InnerRelease = new BaseAction(7389),

            //���ı���
            PrimalRend = new (25753)
            {
                BuffsNeed = new [] { ObjectStatus.PrimalRendReady },
            };
    }
    internal override SortedList<DescType, string> Description => new ()
    {
        {DescType.��Χ����, $"{Actions.ShakeItOff.Action.Name}"},
        {DescType.�������, $"{Actions.RawIntuition.Action.Name}, {Actions.Vengeance.Action.Name}"},
        {DescType.�ƶ�, $"GCD: {Actions.PrimalRend.Action.Name}��Ŀ��Ϊ����н�С��30������ԶĿ�ꡣ\n                     ����: {Actions.Onslaught.Action.Name}, "},
    };
    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //���� �����׶�
        if (Actions.ShakeItOff.ShouldUseAction(out act)) return true;

        if (GeneralActions.Reprisal.ShouldUseAction(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool MoveGCD(uint lastComboActionID, out IAction act)
    {
        //�Ÿ��� ���ı��� ����ǰ��
        if (Actions.PrimalRend.ShouldUseAction(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //ͻ��
        if (Actions.Onslaught.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
        return false;

    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //��㹥��
        if (Actions.PrimalRend.ShouldUseAction(out act, mustUse: true) && !IsMoving)
        {
            if (BaseAction.DistanceToPlayer(Actions.PrimalRend.Target) < 1)
            {
                return true;
            }
        }

        //�޻����
        if (JobGauge.BeastGauge >= 50 || BaseAction.HaveStatusSelfFromSelf(ObjectStatus.InnerRelease))
        {
            //��������
            if (Actions.SteelCyclone.ShouldUseAction(out act)) return true;
            //ԭ��֮��
            if (Actions.InnerBeast.ShouldUseAction(out act)) return true;
        }

        //Ⱥ��
        if (Actions.MythrilTempest.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.Overpower.ShouldUseAction(out act, lastComboActionID)) return true;

        //����
        if (Actions.StormsEye.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.StormsPath.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.Maim.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.HeavySwing.ShouldUseAction(out act, lastComboActionID)) return true;

        //�����ţ�����һ���ɡ�
        if (IconReplacer.Move && MoveAbility(1, out act)) return true;
        if (Actions.Tomahawk.ShouldUseAction(out act)) return true;

        return false;
    }
    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //���� ���Ѫ�����ˡ�
        if (Actions.Holmgang.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (abilityRemain == 2)
        {
            //ԭ����ֱ��������10%��
            if (Actions.RawIntuition.ShouldUseAction(out act)) return true;

            //���𣨼���30%��
            if (Actions.Vengeance.ShouldUseAction(out act)) return true;

            //���ڣ�����20%��
            if (GeneralActions.Rampart.ShouldUseAction(out act)) return true;

        }
        //���͹���
        //ѩ��
        if (GeneralActions.Reprisal.ShouldUseAction(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        //����
        if (BuffTime > 3 || Service.ClientState.LocalPlayer.Level < Actions.MythrilTempest.Level)
        {
            //ս��
            if (Actions.Infuriate.ShouldUseAction(out act)) return true;
            //��
            if (!new BaseAction(7389).IsCoolDown && Actions.Berserk.ShouldUseAction(out act)) return true;
            //ս��
            if (Actions.Infuriate.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
        }

        if ((float)Service.ClientState.LocalPlayer.CurrentHp / Service.ClientState.LocalPlayer.MaxHp < 0.6)
        {
            //ս��
            if (Actions.ThrillofBattle.ShouldUseAction(out act)) return true;
            //̩Ȼ���� ���̰���
            if (Actions.Equilibrium.ShouldUseAction(out act)) return true;
        }

        //�̸����Ѱ���
        if (!HaveShield && Actions.NascentFlash.ShouldUseAction(out act)) return true;

        //��ͨ����
        //Ⱥɽ¡��
        if (Actions.Orogeny.ShouldUseAction(out act)) return true;
        //���� 
        if (Actions.Upheaval.ShouldUseAction(out act)) return true;

        //��㹥��
        if (Actions.Onslaught.ShouldUseAction(out act) && !IsMoving)
        {
            if (BaseAction.DistanceToPlayer(Actions.Onslaught.Target) < 2)
            {
                return true;
            }
        }

        return false;
    }
}
