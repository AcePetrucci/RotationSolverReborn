using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using RotationSolver.Actions;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Data;
using RotationSolver.Helpers;

namespace RotationSolver.Rotations.Basic;


internal abstract class GNB_Base : CustomRotation.CustomRotation
{
    private static GNBGauge JobGauge => Service.JobGauges.Get<GNBGauge>();

    /// <summary>
    /// ��������
    /// </summary>
    protected static byte Ammo => JobGauge.Ammo;

    /// <summary>
    /// �����ĵڼ���combo
    /// </summary>
    protected static byte AmmoComboStep => JobGauge.AmmoComboStep;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Gunbreaker };
    private sealed protected override IBaseAction Shield => RoyalGuard;

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
    public static IBaseAction LightningShot { get; } = new BaseAction(ActionID.LightningShot);

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
    public static IBaseAction Aurora { get; } = new BaseAction(ActionID.Aurora, true, isTimeline: true);

    /// <summary>
    /// ��������
    /// </summary>
    public static IBaseAction Superbolide { get; } = new BaseAction(ActionID.Superbolide, true, isTimeline: true);

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
    public static IBaseAction HeartofLight { get; } = new BaseAction(ActionID.HeartofLight, true, isTimeline: true);

    /// <summary>
    /// ʯ֮��
    /// </summary>
    public static IBaseAction HeartofStone { get; } = new BaseAction(ActionID.HeartofStone, true, isTimeline: true)
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
    public static IBaseAction Bloodfest { get; } = new BaseAction(ActionID.Bloodfest, true)
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
        ActionCheck = b => Service.IconReplacer.OriginalHook(ActionID.GnashingFang) == ActionID.SavageClaw,
    };

    /// <summary>
    /// ����צ
    /// </summary>
    public static IBaseAction WickedTalon { get; } = new BaseAction(ActionID.WickedTalon)
    {
        ActionCheck = b => Service.IconReplacer.OriginalHook(ActionID.GnashingFang) == ActionID.WickedTalon,
    };

    /// <summary>
    /// ˺��
    /// </summary>
    public static IBaseAction JugularRip { get; } = new BaseAction(ActionID.JugularRip)
    {
        ActionCheck = b => Service.IconReplacer.OriginalHook(ActionID.Continuation) == ActionID.JugularRip,
    };

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction AbdomenTear { get; } = new BaseAction(ActionID.AbdomenTear)
    {
        ActionCheck = b => Service.IconReplacer.OriginalHook(ActionID.Continuation) == ActionID.AbdomenTear,
    };

    /// <summary>
    /// ��Ŀ
    /// </summary>
    public static IBaseAction EyeGouge { get; } = new BaseAction(ActionID.EyeGouge)
    {
        ActionCheck = b => Service.IconReplacer.OriginalHook(ActionID.Continuation) == ActionID.EyeGouge,
    };

    /// <summary>
    /// ������
    /// </summary>
    public static IBaseAction Hypervelocity { get; } = new BaseAction(ActionID.Hypervelocity)
    {
        ActionCheck = b => Service.IconReplacer.OriginalHook(ActionID.Continuation)
        == ActionID.Hypervelocity,
    };

    private protected override bool EmergencyAbility(byte abilitiesRemaining, IAction nextGCD, out IAction act)
    {
        //�������� ���л�����ˡ�
        if (Superbolide.CanUse(out act) && BaseAction.TankBreakOtherCheck(JobIDs[0], Superbolide.Target)) return true;
        return base.EmergencyAbility(abilitiesRemaining, nextGCD, out act);
    }

    private protected sealed override bool MoveForwardAbility(byte abilitiesRemaining, out IAction act)
    {
        //ͻ��
        if (RoughDivide.CanUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }
}

