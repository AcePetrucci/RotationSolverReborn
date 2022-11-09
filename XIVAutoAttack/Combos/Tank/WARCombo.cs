using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Attributes;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.Tank.WARCombo;

namespace XIVAutoAttack.Combos.Tank;

[ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/XIVAutoAttack/Combos/Tank/WARCombo.cs")]
internal sealed class WARCombo : JobGaugeCombo<WARGauge, CommandType>
{
    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //д��ע�Ͱ���������ʾ�û��ġ�
    };
    public override uint[] JobIDs => new uint[] { 21, 3 };
    internal override bool HaveShield => Player.HaveStatus(ObjectStatus.Defiance);
    private protected override BaseAction Shield => Defiance;

    public static readonly BaseAction
        //�ػ�
        Defiance = new(48, shouldEndSpecial: true),

        //����
        HeavySwing = new(31),

        //�ײ���
        Maim = new(37),

        //����ն �̸�
        StormsPath = new(42),

        //������ �츫
        StormsEye = new(45)
        {
            OtherCheck = b => Player.WillStatusEndGCD(3, 0, true, ObjectStatus.SurgingTempest),
        },

        //�ɸ�
        Tomahawk = new(46)
        {
            FilterForTarget = b => TargetFilter.ProvokeTarget(b),
        },

        //�͹�
        Onslaught = new(7386, shouldEndSpecial: true)
        {
            ChoiceTarget = TargetFilter.FindTargetForMoving,
        },

        //����    
        Upheaval = new(7387)
        {
            BuffsNeed = new ushort[] { ObjectStatus.SurgingTempest },
        },

        //��ѹ��
        Overpower = new(41),

        //��������
        MythrilTempest = new(16462),

        //Ⱥɽ¡��
        Orogeny = new(25752),

        //ԭ��֮��
        InnerBeast = new(49)
        {
            OtherCheck = b => !Player.WillStatusEndGCD(3, 0, true, ObjectStatus.SurgingTempest) && (JobGauge.BeastGauge >= 50 || Player.HaveStatus(ObjectStatus.InnerRelease)),
        },

        //��������
        SteelCyclone = new(51)
        {
            OtherCheck = InnerBeast.OtherCheck,
        },

        //ս��
        Infuriate = new(52)
        {
            BuffsProvide = new[] { ObjectStatus.InnerRelease },
            OtherCheck = b => TargetFilter.GetObjectInRadius(TargetUpdater.HostileTargets, 5).Length > 0 && JobGauge.BeastGauge < 50,
        },

        //��
        Berserk = new(38)
        {
            OtherCheck = b => TargetFilter.GetObjectInRadius(TargetUpdater.HostileTargets, 5).Length > 0,
        },

        //ս��
        ThrillofBattle = new(40),

        //̩Ȼ����
        Equilibrium = new(3552),

        //ԭ��������
        NascentFlash = new(16464)
        {
            ChoiceTarget = TargetFilter.FindAttackedTarget,
        },

        ////ԭ����Ѫ��
        //Bloodwhetting = new BaseAction(25751),

        //����
        Vengeance = new(44)
        {
            BuffsProvide = Rampart.BuffsProvide,
            OtherCheck = BaseAction.TankDefenseSelf,
        },

        //ԭ����ֱ��
        RawIntuition = new(3551)
        {
            BuffsProvide = Rampart.BuffsProvide,
            OtherCheck = BaseAction.TankDefenseSelf,
        },

        //����
        ShakeItOff = new(7388, true),

        //����
        Holmgang = new(43)
        {
            OtherCheck = BaseAction.TankBreakOtherCheck,
        },

        ////ԭ���Ľ��
        //InnerRelease = new BaseAction(7389),

        //���ı���
        PrimalRend = new(25753)
        {
            BuffsNeed = new[] { ObjectStatus.PrimalRendReady },
        };
    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.��Χ����, $"{ShakeItOff}"},
        {DescType.�������, $"{RawIntuition}, {Vengeance}"},
        {DescType.�ƶ�����, $"GCD: {PrimalRend}��Ŀ��Ϊ����н�С��30������ԶĿ�ꡣ\n                     ����: {Onslaught}, "},
    };
    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //���� �����׶�
        if (ShakeItOff.ShouldUse(out act, mustUse: true)) return true;

        if (Reprisal.ShouldUse(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool MoveGCD(out IAction act)
    {
        //�Ÿ��� ���ı��� ����ǰ��
        if (PrimalRend.ShouldUse(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //ͻ��
        if (Onslaught.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //��㹥��
        if (PrimalRend.ShouldUse(out act, mustUse: true) && !IsMoving)
        {
            if (PrimalRend.Target.DistanceToPlayer() < 1)
            {
                return true;
            }
        }

        //�޻����
        //��������
        if (SteelCyclone.ShouldUse(out act)) return true;
        //ԭ��֮��
        if (InnerBeast.ShouldUse(out act)) return true;

        //Ⱥ��
        if (MythrilTempest.ShouldUse(out act)) return true;
        if (Overpower.ShouldUse(out act)) return true;

        //����
        if (StormsEye.ShouldUse(out act)) return true;
        if (StormsPath.ShouldUse(out act)) return true;
        if (Maim.ShouldUse(out act)) return true;
        if (HeavySwing.ShouldUse(out act)) return true;

        //�����ţ�����һ���ɡ�
        if (CommandController.Move && MoveAbility(1, out act)) return true;
        if (Tomahawk.ShouldUse(out act)) return true;

        return false;
    }
    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //���� ���Ѫ�����ˡ�
        if (Holmgang.ShouldUse(out act)) return true;

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (abilityRemain == 2)
        {
            if (TargetUpdater.HostileTargets.Length == 1)
            {
                //���𣨼���30%��
                if (Vengeance.ShouldUse(out act)) return true;
            }

            //ԭ����ֱ��������10%��
            if (RawIntuition.ShouldUse(out act)) return true;

            //���𣨼���30%��
            if (Vengeance.ShouldUse(out act)) return true;

            //���ڣ�����20%��
            if (Rampart.ShouldUse(out act)) return true;
        }
        //���͹���
        //ѩ��
        if (Reprisal.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {

        //����
        if (!Player.WillStatusEndGCD(3, 0, true, ObjectStatus.SurgingTempest) || !MythrilTempest.EnoughLevel)
        {
            //��
            if (!new BaseAction(7389).IsCoolDown && Berserk.ShouldUse(out act)) return true;
        }

        if (Player.GetHealthRatio() < 0.6f)
        {
            //ս��
            if (ThrillofBattle.ShouldUse(out act)) return true;
            //̩Ȼ���� ���̰���
            if (Equilibrium.ShouldUse(out act)) return true;
        }

        //�̸����Ѱ���
        if (!HaveShield && NascentFlash.ShouldUse(out act)) return true;

        //ս��
        if (Infuriate.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //��ͨ����
        //Ⱥɽ¡��
        if (Orogeny.ShouldUse(out act)) return true;
        //���� 
        if (Upheaval.ShouldUse(out act)) return true;

        //��㹥��
        if (Onslaught.ShouldUse(out act) && !IsMoving)
        {
            if (Onslaught.Target.DistanceToPlayer() < 1)
            {
                return true;
            }
        }

        return false;
    }
}
