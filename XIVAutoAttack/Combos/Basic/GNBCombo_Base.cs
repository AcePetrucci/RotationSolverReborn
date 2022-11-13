using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.Basic;


internal abstract class GNBCombo_Base<TCmd> : JobGaugeCombo<GNBGauge, TCmd> where TCmd : Enum
{
    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Gunbreaker };
    internal sealed override bool HaveShield => Player.HaveStatus(true, StatusID.RoyalGuard);
    private sealed protected override BaseAction Shield => RoyalGuard;

    protected override bool CanHealSingleSpell => false;
    protected override bool CanHealAreaSpell => false;

    /// <summary>
    /// ��������
    /// </summary>
    public static BaseAction RoyalGuard { get; } = new(ActionID.RoyalGuard, shouldEndSpecial: true);

    /// <summary>
    /// ����ն
    /// </summary>
    public static BaseAction KeenEdge { get; } = new(ActionID.KeenEdge);

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction NoMercy { get; } = new(ActionID.NoMercy);

    /// <summary>
    /// �б���
    /// </summary>
    public static BaseAction BrutalShell { get; } = new(ActionID.BrutalShell);

    /// <summary>
    /// αװ
    /// </summary>
    public static BaseAction Camouflage { get; } = new(ActionID.Camouflage)
    {
        BuffsProvide = Rampart.BuffsProvide,
        OtherCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// ��ħ��
    /// </summary>
    public static BaseAction DemonSlice { get; } = new(ActionID.DemonSlice);

    /// <summary>
    /// ���׵�
    /// </summary>
    public static BaseAction LightningShot { get; } = new(ActionID.LightningShot);

    /// <summary>
    /// Σ������
    /// </summary>
    public static BaseAction DangerZone { get; } = new(ActionID.DangerZone);

    /// <summary>
    /// Ѹ��ն
    /// </summary>
    public static BaseAction SolidBarrel { get; } = new(ActionID.SolidBarrel);

    /// <summary>
    /// ������
    /// </summary>
    public static BaseAction BurstStrike { get; } = new(ActionID.BurstStrike)
    {
        OtherCheck = b => JobGauge.Ammo > 0,
    };

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Nebula { get; } = new(ActionID.Nebula)
    {
        BuffsProvide = Rampart.BuffsProvide,
        OtherCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// ��ħɱ
    /// </summary>
    public static BaseAction DemonSlaughter { get; } = new(ActionID.DemonSlaughter);

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Aurora { get; } = new BaseAction(ActionID.Aurora, true);

    /// <summary>
    /// ��������
    /// </summary>
    public static BaseAction Superbolide { get; } = new(ActionID.Superbolide)
    {
        OtherCheck = BaseAction.TankBreakOtherCheck,
    };

    /// <summary>
    /// ������
    /// </summary>
    public static BaseAction SonicBreak { get; } = new(ActionID.SonicBreak);

    /// <summary>
    /// �ַ�ն
    /// </summary>
    public static BaseAction RoughDivide { get; } = new(ActionID.RoughDivide, shouldEndSpecial: true)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving
    };

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction GnashingFang { get; } = new(ActionID.GnashingFang)
    {
        OtherCheck = b => JobGauge.AmmoComboStep == 0 && JobGauge.Ammo > 0,
    };

    /// <summary>
    /// ���γ岨
    /// </summary>
    public static BaseAction BowShock { get; } = new(ActionID.BowShock);

    /// <summary>
    /// ��֮��
    /// </summary>
    public static BaseAction HeartofLight { get; } = new(ActionID.HeartofLight, true);

    /// <summary>
    /// ʯ֮��
    /// </summary>
    public static BaseAction HeartofStone { get; } = new(ActionID.HeartofStone, true)
    {
        BuffsProvide = Rampart.BuffsProvide,
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// ����֮��
    /// </summary>
    public static BaseAction FatedCircle { get; } = new(ActionID.FatedCircle)
    {
        OtherCheck = b => JobGauge.Ammo > (Level >= 88 ? 2 : 1),
    };

    /// <summary>
    /// Ѫ��
    /// </summary>
    public static BaseAction Bloodfest { get; } = new(ActionID.Bloodfest)
    {
        OtherCheck = b => JobGauge.Ammo == 0,
    };

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction DoubleDown { get; } = new(ActionID.DoubleDown)
    {
        OtherCheck = b => JobGauge.Ammo >= 2,
    };

    /// <summary>
    /// ����צ
    /// </summary>
    public static BaseAction SavageClaw { get; } = new(ActionID.SavageClaw)
    {
        OtherCheck = b => Service.IconReplacer.OriginalHook(GnashingFang.ID) == SavageClaw.ID,
    };

    /// <summary>
    /// ����צ
    /// </summary>
    public static BaseAction WickedTalon { get; } = new(ActionID.WickedTalon)
    {
        OtherCheck = b => Service.IconReplacer.OriginalHook(GnashingFang.ID) == WickedTalon.ID,
    };

    /// <summary>
    /// ˺��
    /// </summary>
    public static BaseAction JugularRip { get; } = new(ActionID.JugularRip)
    {
        OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == JugularRip.ID,
    };

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction AbdomenTear { get; } = new(ActionID.AbdomenTear)
    {
        OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == AbdomenTear.ID,
    };

    /// <summary>
    /// ��Ŀ
    /// </summary>
    public static BaseAction EyeGouge { get; } = new(ActionID.EyeGouge)
    {
        OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == EyeGouge.ID,
    };

    /// <summary>
    /// ������
    /// </summary>
    public static BaseAction Hypervelocity { get; } = new(ActionID.Hypervelocity)
    {
        OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == Hypervelocity.ID,
    };
}

