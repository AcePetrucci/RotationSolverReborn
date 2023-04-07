namespace RotationSolver.Basic.Rotations.Basic;

public abstract class GNB_Base : CustomRotation
{
    private static GNBGauge JobGauge => Service.JobGauges.Get<GNBGauge>();

    public override MedicineType MedicineType => MedicineType.Strength;

    /// <summary>
    /// ��������
    /// </summary>
    protected static byte Ammo => JobGauge.Ammo;

    /// <summary>
    /// �����ĵڼ���combo
    /// </summary>
    protected static byte AmmoComboStep => JobGauge.AmmoComboStep;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Gunbreaker };
    private sealed protected override IBaseAction TankStance => RoyalGuard;

    protected override bool CanHealSingleSpell => false;
    protected override bool CanHealAreaSpell => false;

    protected static byte MaxAmmo => Level >= 88 ? (byte)3 : (byte)2;

    /// <summary>
    /// ��������
    /// </summary>
    public static IBaseAction RoyalGuard { get; } = new BaseAction(ActionID.RoyalGuard, shouldEndSpecial: true);

    /// <summary>
    /// ����ն
    /// </summary>
    public static IBaseAction KeenEdge { get; } = new BaseAction(ActionID.KeenEdge);

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction NoMercy { get; } = new BaseAction(ActionID.NoMercy);

    /// <summary>
    /// �б���
    /// </summary>
    public static IBaseAction BrutalShell { get; } = new BaseAction(ActionID.BrutalShell);

    /// <summary>
    /// αװ
    /// </summary>
    public static IBaseAction Camouflage { get; } = new BaseAction(ActionID.Camouflage, true, isTimeline: true)
    {
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// ��ħ��
    /// </summary>
    public static IBaseAction DemonSlice { get; } = new BaseAction(ActionID.DemonSlice)
    {
        AOECount = 2,
    };

    /// <summary>
    /// ���׵�
    /// </summary>
    public static IBaseAction LightningShot { get; } = new BaseAction(ActionID.LightningShot)
    {
        FilterForHostiles = TargetFilter.TankRangeTarget,
        ActionCheck = b => !IsLastAction(IActionHelper.MovingActions),
    };

    /// <summary>
    /// Σ������
    /// </summary>
    public static IBaseAction DangerZone { get; } = new BaseAction(ActionID.DangerZone);

    /// <summary>
    /// Ѹ��ն
    /// </summary>
    public static IBaseAction SolidBarrel { get; } = new BaseAction(ActionID.SolidBarrel);

    /// <summary>
    /// ������
    /// </summary>
    public static IBaseAction BurstStrike { get; } = new BaseAction(ActionID.BurstStrike)
    {
        ActionCheck = b => JobGauge.Ammo > 0,
    };

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction Nebula { get; } = new BaseAction(ActionID.Nebula, true, isTimeline: true)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// ��ħɱ
    /// </summary>
    public static IBaseAction DemonSlaughter { get; } = new BaseAction(ActionID.DemonSlaughter)
    {
        AOECount = 2,
    };

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction Aurora { get; } = new BaseAction(ActionID.Aurora, true, isTimeline: true)
    {
        ActionCheck = b => b.HasStatus(true, StatusID.Aurora),
    };

    /// <summary>
    /// ��������
    /// </summary>
    public static IBaseAction SuperBolide { get; } = new BaseAction(ActionID.SuperBolide, true, isTimeline: true);

    /// <summary>
    /// ������
    /// </summary>
    public static IBaseAction SonicBreak { get; } = new BaseAction(ActionID.SonicBreak);

    /// <summary>
    /// �ַ�ն
    /// </summary>
    public static IBaseAction RoughDivide { get; } = new BaseAction(ActionID.RoughDivide, shouldEndSpecial: true)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction GnashingFang { get; } = new BaseAction(ActionID.GnashingFang)
    {
        ActionCheck = b => JobGauge.AmmoComboStep == 0 && JobGauge.Ammo > 0,
    };

    /// <summary>
    /// ���γ岨
    /// </summary>
    public static IBaseAction BowShock { get; } = new BaseAction(ActionID.BowShock);

    /// <summary>
    /// ��֮��
    /// </summary>
    public static IBaseAction HeartOfLight { get; } = new BaseAction(ActionID.HeartOfLight, true, isTimeline: true);

    /// <summary>
    /// ʯ֮��
    /// </summary>
    public static IBaseAction HeartOfStone { get; } = new BaseAction(ActionID.HeartOfStone, true, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// ����֮��
    /// </summary>
    public static IBaseAction FatedCircle { get; } = new BaseAction(ActionID.FatedCircle)
    {
        ActionCheck = b => JobGauge.Ammo > 0,
    };

    /// <summary>
    /// Ѫ��
    /// </summary>
    public static IBaseAction BloodFest { get; } = new BaseAction(ActionID.BloodFest, true)
    {
        ActionCheck = b => MaxAmmo - JobGauge.Ammo > 1,
    };

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction DoubleDown { get; } = new BaseAction(ActionID.DoubleDown)
    {
        ActionCheck = b => JobGauge.Ammo > 1,
    };

    /// <summary>
    /// ����צ
    /// </summary>
    public static IBaseAction SavageClaw { get; } = new BaseAction(ActionID.SavageClaw)
    {
        ActionCheck = b => Service.GetAdjustedActionId(ActionID.GnashingFang) == ActionID.SavageClaw,
    };

    /// <summary>
    /// ����צ
    /// </summary>
    public static IBaseAction WickedTalon { get; } = new BaseAction(ActionID.WickedTalon)
    {
        ActionCheck = b => Service.GetAdjustedActionId(ActionID.GnashingFang) == ActionID.WickedTalon,
    };

    /// <summary>
    /// ˺��
    /// </summary>
    public static IBaseAction JugularRip { get; } = new BaseAction(ActionID.JugularRip)
    {
        ActionCheck = b => Service.GetAdjustedActionId(ActionID.Continuation) == ActionID.JugularRip,
    };

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction AbdomenTear { get; } = new BaseAction(ActionID.AbdomenTear)
    {
        ActionCheck = b => Service.GetAdjustedActionId(ActionID.Continuation) == ActionID.AbdomenTear,
    };

    /// <summary>
    /// ��Ŀ
    /// </summary>
    public static IBaseAction EyeGouge { get; } = new BaseAction(ActionID.EyeGouge)
    {
        ActionCheck = b => Service.GetAdjustedActionId(ActionID.Continuation) == ActionID.EyeGouge,
    };

    /// <summary>
    /// ������
    /// </summary>
    public static IBaseAction Hypervelocity { get; } = new BaseAction(ActionID.Hypervelocity)
    {
        ActionCheck = b => Service.GetAdjustedActionId(ActionID.Continuation)
        == ActionID.Hypervelocity,
    };

    protected override bool EmergencyAbility(byte abilitiesRemaining, IAction nextGCD, out IAction act)
    {
        if (SuperBolide.CanUse(out act) && BaseAction.TankBreakOtherCheck(JobIDs[0])) return true;
        return base.EmergencyAbility(abilitiesRemaining, nextGCD, out act);
    }

    [RotationDesc(ActionID.RoughDivide)]
    protected sealed override bool MoveForwardAbility(byte abilitiesRemaining, out IAction act, CanUseOption option = CanUseOption.None)
    {
        if (RoughDivide.CanUse(out act, CanUseOption.EmptyOrSkipCombo | option)) return true;
        return false;
    }
}

