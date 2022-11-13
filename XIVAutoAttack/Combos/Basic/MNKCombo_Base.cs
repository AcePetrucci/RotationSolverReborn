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


    public static readonly BaseAction
        //˫����
        DragonKick = new(74)
        {
            BuffsProvide = new[] { StatusID.LeadenFist },
        },

        //����
        Bootshine = new(53),

        //�ƻ���� aoe
        ArmoftheDestroyer = new(62),

        //˫�ƴ� �˺����
        TwinSnakes = new(61),

        //��ȭ
        TrueStrike = new(54),

        //����� aoe
        FourpointFury = new(16473),

        //����ȭ
        Demolish = new(66, isEot: true)
        {
            TargetStatus = new StatusID[] { StatusID.Demolish },
        },

        //��ȭ
        SnapPunch = new(ActionID.SnapPunch),

        //���Ҿ� aoe
        Rockbreaker = new(70),

        //����
        Meditation = new(3546),

        //��ɽ��
        SteelPeak = new(25761)
        {
            OtherCheck = b => InCombat,
        },

        //����ȭ
        HowlingFist = new(25763)
        {
            OtherCheck = b => InCombat,
        },

        //������
        Brotherhood = new(7396, true),

        //�������� ���dps
        RiddleofFire = new(7395),

        //ͻ������
        Thunderclap = new(25762, shouldEndSpecial: true)
        {
            ChoiceTarget = TargetFilter.FindTargetForMoving,
        },

        //����
        Mantra = new(65, true),

        //���
        PerfectBalance = new(69)
        {
            BuffsNeed = new StatusID[] { StatusID.RaptorForm },
            OtherCheck = b => InCombat,
        },

        //������ ��
        ElixirField = new(3545),

        //���ѽ� ��
        FlintStrike = new(25882),

        //�����
        RisingPhoenix = new(25768),

        //��������� ����
        TornadoKick = new(3543),
        PhantomRush = new(25769),

        //����
        FormShift = new(4262)
        {
            BuffsProvide = new[] { StatusID.FormlessFist, StatusID.PerfectBalance },
        },

        //��ռ��� ��
        RiddleofEarth = new(7394, shouldEndSpecial: true)
        {
            BuffsProvide = new[] { StatusID.RiddleofEarth },
        },

        //���缫��
        RiddleofWind = new(25766);

}
