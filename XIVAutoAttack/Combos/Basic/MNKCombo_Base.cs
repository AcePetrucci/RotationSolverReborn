using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.Basic;

internal abstract class MNKCombo_Base<TCmd> : JobGaugeCombo<MNKGauge, TCmd> where TCmd : Enum
{
    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Monk, ClassJobID.Pugilist };

    /// <summary>
    /// ˫����
    /// </summary>
    public static BaseAction DragonKick { get; } = new(ActionID.DragonKick)
    {
        BuffsProvide = new[] { StatusID.LeadenFist },
    };

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Bootshine { get; } = new(ActionID.Bootshine);

    /// <summary>
    /// �ƻ���� aoe
    /// </summary>
    public static BaseAction ArmoftheDestroyer { get; } = new(ActionID.ArmoftheDestroyer);

    /// <summary>
    /// ˫�ƴ� �˺����
    /// </summary>
    public static BaseAction TwinSnakes { get; } = new(ActionID.TwinSnakes);

    /// <summary>
    /// ��ȭ
    /// </summary>
    public static BaseAction TrueStrike { get; } = new(ActionID.TrueStrike);

    /// <summary>
    /// ����� aoe
    /// </summary>
    public static BaseAction FourpointFury { get; } = new(ActionID.FourpointFury);

    /// <summary>
    /// ����ȭ
    /// </summary>
    public static BaseAction Demolish { get; } = new(ActionID.Demolish, isEot: true)
    {
        TargetStatus = new StatusID[] { StatusID.Demolish },
    };

    /// <summary>
    /// ��ȭ
    /// </summary>
    public static BaseAction SnapPunch { get; } = new(ActionID.SnapPunch);

    /// <summary>
    /// ���Ҿ� aoe
    /// </summary>
    public static BaseAction Rockbreaker { get; } = new(ActionID.Rockbreaker);

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Meditation { get; } = new(ActionID.Meditation);

    /// <summary>
    /// ��ɽ��
    /// </summary>
    public static BaseAction SteelPeak { get; } = new(ActionID.SteelPeak)
    {
        OtherCheck = b => InCombat,
    };

    /// <summary>
    /// ����ȭ
    /// </summary>
    public static BaseAction HowlingFist { get; } = new(ActionID.HowlingFist)
    {
        OtherCheck = b => InCombat,
    };

    /// <summary>
    /// ������
    /// </summary>
    public static BaseAction Brotherhood { get; } = new(ActionID.Brotherhood, true);

    /// <summary>
    /// �������� ���dps
    /// </summary>
    public static BaseAction RiddleofFire { get; } = new(ActionID.RiddleofFire);

    /// <summary>
    /// ͻ������
    /// </summary>
    public static BaseAction Thunderclap { get; } = new(ActionID.Thunderclap, shouldEndSpecial: true)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Mantra { get; } = new(ActionID.Mantra, true);

    /// <summary>
    /// ���
    /// </summary>
    public static BaseAction PerfectBalance { get; } = new(ActionID.PerfectBalance)
    {
        BuffsNeed = new StatusID[] { StatusID.RaptorForm },
        OtherCheck = b => InCombat,
    };

    /// <summary>
    /// ������ ��
    /// </summary>
    public static BaseAction ElixirField { get; } = new(ActionID.ElixirField);

    /// <summary>
    /// ���ѽ� ��
    /// </summary>
    public static BaseAction FlintStrike { get; } = new(ActionID.FlintStrike);

    /// <summary>
    /// �����
    /// </summary>
    public static BaseAction RisingPhoenix { get; } = new(ActionID.RisingPhoenix);

    /// <summary>
    /// ��������� ����
    /// </summary>
    public static BaseAction TornadoKick { get; } = new(ActionID.TornadoKick);
    public static BaseAction PhantomRush { get; } = new(ActionID.PhantomRush);

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction FormShift { get; } = new(ActionID.FormShift)
    {
        BuffsProvide = new[] { StatusID.FormlessFist, StatusID.PerfectBalance },
    };

    /// <summary>
    /// ��ռ��� ��
    /// </summary>
    public static BaseAction RiddleofEarth { get; } = new(ActionID.RiddleofEarth, shouldEndSpecial: true)
    {
        BuffsProvide = new[] { StatusID.RiddleofEarth },
    };

    /// <summary>
    /// ���缫��
    /// </summary>
    public static BaseAction RiddleofWind { get; } = new(ActionID.RiddleofWind);

}
