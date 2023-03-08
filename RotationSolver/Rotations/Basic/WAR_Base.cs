using Dalamud.Game.ClientState.JobGauge.Types;
using RotationSolver.Actions;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Attributes;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.CustomRotation;

namespace RotationSolver.Rotations.Basic;

internal abstract class WAR_Base : CustomRotation.CustomRotation
{
    private static WARGauge JobGauge => Service.JobGauges.Get<WARGauge>();
    public override MedicineType MedicineType => MedicineType.Strength;


    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Warrior, ClassJobID.Marauder };
    private sealed protected override IBaseAction Shield => Defiance;

    /// <summary>
    /// �ػ�
    /// </summary>
    public static IBaseAction Defiance { get; } = new BaseAction(ActionID.Defiance, shouldEndSpecial: true);

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction HeavySwing { get; } = new BaseAction(ActionID.HeavySwing);

    /// <summary>
    /// �ײ���
    /// </summary>
    public static IBaseAction Maim { get; } = new BaseAction(ActionID.Maim);

    /// <summary>
    /// ����ն �̸�
    /// </summary>
    public static IBaseAction StormsPath { get; } = new BaseAction(ActionID.StormsPath);

    /// <summary>
    /// ������ �츫
    /// </summary>
    public static IBaseAction StormsEye { get; } = new BaseAction(ActionID.StormsEye)
    {
        ActionCheck = b => Player.WillStatusEndGCD(3, 0, true, StatusID.SurgingTempest),
    };

    /// <summary>
    /// �ɸ�
    /// </summary>
    public static IBaseAction Tomahawk { get; } = new BaseAction(ActionID.Tomahawk)
    {
        FilterForHostiles = TargetFilter.TankRangeTarget,
    };

    /// <summary>
    /// �͹�
    /// </summary>
    public static IBaseAction Onslaught { get; } = new BaseAction(ActionID.Onslaught, shouldEndSpecial: true)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    /// <summary>
    /// ����    
    /// </summary>
    public static IBaseAction Upheaval { get; } = new BaseAction(ActionID.Upheaval)
    {
        StatusNeed = new StatusID[] { StatusID.SurgingTempest },
    };

    /// <summary>
    /// ��ѹ��
    /// </summary>
    public static IBaseAction Overpower { get; } = new BaseAction(ActionID.Overpower);

    /// <summary>
    /// ��������
    /// </summary>
    public static IBaseAction MythrilTempest { get; } = new BaseAction(ActionID.MythrilTempest);

    /// <summary>
    /// Ⱥɽ¡��
    /// </summary>
    public static IBaseAction Orogeny { get; } = new BaseAction(ActionID.Orogeny);

    /// <summary>
    /// ԭ��֮��
    /// </summary>
    public static IBaseAction InnerBeast { get; } = new BaseAction(ActionID.InnerBeast)
    {
        ActionCheck = b => JobGauge.BeastGauge >= 50 || Player.HasStatus(true, StatusID.InnerRelease),
    };

    /// <summary>
    /// ԭ���Ľ��
    /// </summary>
    public static IBaseAction InnerRelease { get; } = new BaseAction(ActionID.InnerRelease)
    {
        ActionCheck = InnerBeast.ActionCheck,
    };

    /// <summary>
    /// ��������
    /// </summary>
    public static IBaseAction SteelCyclone { get; } = new BaseAction(ActionID.SteelCyclone)
    {
        ActionCheck = InnerBeast.ActionCheck,
    };

    /// <summary>
    /// ս��
    /// </summary>
    public static IBaseAction Infuriate { get; } = new BaseAction(ActionID.Infuriate)
    {
        StatusProvide = new[] { StatusID.InnerRelease },
        ActionCheck = b => HasHostilesInRange && JobGauge.BeastGauge < 50 && InCombat,
    };

    /// <summary>
    /// ��
    /// </summary>
    public static IBaseAction Berserk { get; } = new BaseAction(ActionID.Berserk)
    {
        ActionCheck = b => HasHostilesInRange && !InnerRelease.IsCoolingDown,
    };

    /// <summary>
    /// ս��
    /// </summary>
    public static IBaseAction ThrillofBattle { get; } = new BaseAction(ActionID.ThrillofBattle, true, isTimeline: true);

    /// <summary>
    /// ̩Ȼ����
    /// </summary>
    public static IBaseAction Equilibrium { get; } = new BaseAction(ActionID.Equilibrium, true, isTimeline: true);

    /// <summary>
    /// ԭ��������
    /// </summary>
    public static IBaseAction NascentFlash { get; } = new BaseAction(ActionID.NascentFlash, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction Vengeance { get; } = new BaseAction(ActionID.Vengeance, isTimeline: true)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// ԭ����ֱ��
    /// </summary>
    public static IBaseAction RawIntuition { get; } = new BaseAction(ActionID.RawIntuition, isTimeline: true)
    {
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction ShakeItOff { get; } = new BaseAction(ActionID.ShakeItOff, true, isTimeline: true);

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction Holmgang { get; } = new BaseAction(ActionID.Holmgang, isTimeline: true)
    {
        ChoiceTarget = (tars, mustUse) => Player,
    };

    /// <summary>
    /// ���ı���
    /// </summary>
    public static IBaseAction PrimalRend { get; } = new BaseAction(ActionID.PrimalRend)
    {
        StatusNeed = new[] { StatusID.PrimalRendReady }
    };

    protected override bool EmergencyAbility(byte abilitiesRemaining, IAction nextGCD, out IAction act)
    {
        //���� ���Ѫ�����ˡ�
        if (Holmgang.CanUse(out act) && BaseAction.TankBreakOtherCheck(JobIDs[0])) return true;
        return base.EmergencyAbility(abilitiesRemaining, nextGCD, out act);
    }

    [RotationDesc(ActionID.Onslaught)]
    protected sealed override bool MoveForwardAbility(byte abilitiesRemaining, out IAction act, bool recordTarget = true)
    {
        if (Onslaught.CanUse(out act, emptyOrSkipCombo: true, recordTarget: recordTarget)) return true;
        return false;
    }
}
