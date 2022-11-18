using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.Basic;

internal abstract class MNKCombo_Base<TCmd> : CustomCombo<TCmd> where TCmd : Enum
{
    private static MNKGauge JobGauge => Service.JobGauges.Get<MNKGauge>();

    /// <summary>
    /// �������
    /// </summary>
    protected static BeastChakra[] BeastChakras => JobGauge.BeastChakra;

    /// <summary>
    /// ���������
    /// </summary>
    protected static byte Chakra => JobGauge.Chakra;

    /// <summary>
    /// ������ɱ
    /// </summary>
    protected static Nadi Nadi => JobGauge.Nadi;

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
    public static BaseAction TwinSnakes { get; } = new(ActionID.TwinSnakes, isEot: true);

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
    public static BaseAction Meditation { get; } = new(ActionID.Meditation, true);

    /// <summary>
    /// ��ɽ��
    /// </summary>
    public static BaseAction SteelPeak { get; } = new(ActionID.SteelPeak)
    {
        ActionCheck = b => InCombat && Chakra == 5,
    };

    /// <summary>
    /// ����ȭ
    /// </summary>
    public static BaseAction HowlingFist { get; } = new(ActionID.HowlingFist)
    {
        ActionCheck = SteelPeak.ActionCheck,
    };

    /// <summary>
    /// ������
    /// </summary>
    public static BaseAction Brotherhood { get; } = new(ActionID.Brotherhood, true);

    /// <summary>
    /// �������� ���dps
    /// </summary>
    public static BaseAction RiddleofFire { get; } = new(ActionID.RiddleofFire, true);

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
        ActionCheck = b => InCombat,
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
    public static BaseAction FormShift { get; } = new(ActionID.FormShift, true)
    {
        BuffsProvide = new[] { StatusID.FormlessFist, StatusID.PerfectBalance },
    };

    /// <summary>
    /// ��ռ��� ��
    /// </summary>
    public static BaseAction RiddleofEarth { get; } = new(ActionID.RiddleofEarth, true, shouldEndSpecial: true)
    {
        BuffsProvide = new[] { StatusID.RiddleofEarth },
    };

    /// <summary>
    /// ���缫��
    /// </summary>
    public static BaseAction RiddleofWind { get; } = new(ActionID.RiddleofWind, true);
}
