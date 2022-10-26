using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Controllers;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Tank;

internal sealed class WARCombo : JobGaugeCombo<WARGauge>
{
    internal override uint JobID => 21;
    internal override bool HaveShield => LocalPlayer.HaveStatus(ObjectStatus.Defiance);
    private protected override BaseAction Shield => Actions.Defiance;
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
                OtherCheck = b => LocalPlayer.WillStatusEndGCD(3, 0, true, ObjectStatus.SurgingTempest),
            },

            //�ɸ�
            Tomahawk = new (46)
            {
                FilterForTarget = b => TargetFilter.ProvokeTarget(b),
            },

            //�͹�
            Onslaught = new (7386, shouldEndSpecial: true)
            {
                ChoiceTarget = TargetFilter.FindMoveTarget,
            },

            //����    
            Upheaval = new(7387)
            {
                BuffsNeed = new ushort[] { ObjectStatus.SurgingTempest },
            },

            //��ѹ��
            Overpower = new (41),

            //��������
            MythrilTempest = new (16462),

            //Ⱥɽ¡��
            Orogeny = new (25752),

            //ԭ��֮��
            InnerBeast = new (49)
            {
                OtherCheck = b => !LocalPlayer.WillStatusEndGCD(3, 0, true, ObjectStatus.SurgingTempest) && ( JobGauge.BeastGauge >= 50 || LocalPlayer.HaveStatus(ObjectStatus.InnerRelease)),
            },

            //��������
            SteelCyclone = new(51)
            {
                OtherCheck = InnerBeast.OtherCheck,
            },

            //ս��
            Infuriate = new (52)
            {
                BuffsProvide = new [] { ObjectStatus.InnerRelease },
                OtherCheck = b => TargetFilter.GetObjectInRadius(TargetUpdater.HostileTargets, 5).Length > 0 && JobGauge.BeastGauge < 50,
            },

            //��
            Berserk = new (38)
            {
                OtherCheck = b => TargetFilter.GetObjectInRadius(TargetUpdater.HostileTargets, 5).Length > 0,
            },

            //ս��
            ThrillofBattle = new (40),

            //̩Ȼ����
            Equilibrium = new (3552),

            //ԭ��������
            NascentFlash = new (16464)
            {
                ChoiceTarget = TargetFilter.FindAttackedTarget,
            },

            ////ԭ����Ѫ��
            //Bloodwhetting = new BaseAction(25751),

            //����
            Vengeance = new (44)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
                OtherCheck = BaseAction.TankDefenseSelf,
            },

            //ԭ����ֱ��
            RawIntuition = new (3551)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
                OtherCheck = BaseAction.TankDefenseSelf,
            },

            //����
            ShakeItOff = new (7388, true),

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
        if (Actions.ShakeItOff.ShouldUse(out act, mustUse:true)) return true;

        if (GeneralActions.Reprisal.ShouldUse(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool MoveGCD(uint lastComboActionID, out IAction act)
    {
        //�Ÿ��� ���ı��� ����ǰ��
        if (Actions.PrimalRend.ShouldUse(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //ͻ��
        if (Actions.Onslaught.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //��㹥��
        if (Actions.PrimalRend.ShouldUse(out act, mustUse: true) && !IsMoving)
        {
            if (Actions.PrimalRend.Target.DistanceToPlayer() < 1)
            {
                return true;
            }
        }

        //�޻����
        //��������
        if (Actions.SteelCyclone.ShouldUse(out act)) return true;
        //ԭ��֮��
        if (Actions.InnerBeast.ShouldUse(out act)) return true;

        //Ⱥ��
        if (Actions.MythrilTempest.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.Overpower.ShouldUse(out act, lastComboActionID)) return true;

        //����
        if (Actions.StormsEye.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.StormsPath.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.Maim.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.HeavySwing.ShouldUse(out act, lastComboActionID)) return true;

        //�����ţ�����һ���ɡ�
        if (CommandController.Move && MoveAbility(1, out act)) return true;
        if (Actions.Tomahawk.ShouldUse(out act)) return true;

        return false;
    }
    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //���� ���Ѫ�����ˡ�
        if (Actions.Holmgang.ShouldUse(out act)) return true;

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (abilityRemain == 2)
        {
            if(TargetUpdater.HostileTargets.Length == 1)
            {
                //���𣨼���30%��
                if (Actions.Vengeance.ShouldUse(out act)) return true;
            }

            //ԭ����ֱ��������10%��
            if (Actions.RawIntuition.ShouldUse(out act)) return true;

            //���𣨼���30%��
            if (Actions.Vengeance.ShouldUse(out act)) return true;

            //���ڣ�����20%��
            if (GeneralActions.Rampart.ShouldUse(out act)) return true;
        }
        //���͹���
        //ѩ��
        if (GeneralActions.Reprisal.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        
        //����
        if (!LocalPlayer.WillStatusEndGCD(3, 0, true, ObjectStatus.SurgingTempest) || !Actions.MythrilTempest.EnoughLevel)
        {
            //��
            if (!new BaseAction(7389).IsCoolDown && Actions.Berserk.ShouldUse(out act)) return true;
        }

        if (LocalPlayer.GetHealthRatio() < 0.6f)
        {
            //ս��
            if (Actions.ThrillofBattle.ShouldUse(out act)) return true;
            //̩Ȼ���� ���̰���
            if (Actions.Equilibrium.ShouldUse(out act)) return true;
        }

        //�̸����Ѱ���
        if (!HaveShield && Actions.NascentFlash.ShouldUse(out act)) return true;

        //ս��
        if (Actions.Infuriate.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //��ͨ����
        //Ⱥɽ¡��
        if (Actions.Orogeny.ShouldUse(out act)) return true;
        //���� 
        if (Actions.Upheaval.ShouldUse(out act)) return true;

        //��㹥��
        if (Actions.Onslaught.ShouldUse(out act) && !IsMoving)
        {
            if (Actions.Onslaught.Target.DistanceToPlayer() < 1)
            {
                return true;
            }
        }

        return false;
    }
}
