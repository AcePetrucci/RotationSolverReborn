using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Linq;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Updaters;
using RotationSolver.Helpers;
using RotationSolver.Data;
using RotationSolver.Actions;

namespace RotationSolver.Rotations.Basic;

internal abstract class SGE_Base : CustomRotation.CustomRotation
{
    private static SGEGauge JobGauge => Service.JobGauges.Get<SGEGauge>();

    protected static bool HasEukrasia => JobGauge.Eukrasia;
    protected static byte Addersgall => JobGauge.Addersgall;

    protected static byte Addersting => JobGauge.Addersting;

    /// <summary>
    /// ���ӵ���ʱ���ж������һ�Ű�
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool AddersgallEndAfter(float time)
    {
        return EndAfter(JobGauge.AddersgallTimer / 1000f, time);
    }

    /// <summary>
    /// ���ӵ���ʱ���ж������һ�Ű�
    /// </summary>
    /// <param name="abilityCount"></param>
    /// <param name="gctCount"></param>
    /// <returns></returns>
    protected static bool AddersgallEndAfterGCD(uint gctCount = 0, uint abilityCount = 0)
    {
        return EndAfterGCD(JobGauge.AddersgallTimer / 1000f, gctCount, abilityCount);
    }

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Sage };
    private sealed protected override IBaseAction Raise => Egeiro;

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction Egeiro { get; } = new BaseAction(ActionID.Egeiro, true);

    /// <summary>
    /// עҩ
    /// </summary>
    public static IBaseAction Dosis { get; } = new BaseAction(ActionID.Dosis);

    /// <summary>
    /// ����עҩ
    /// </summary>
    public static IBaseAction EukrasianDosis { get; } = new BaseAction(ActionID.EukrasianDosis, isEot: true)
    {
        TargetStatus = new StatusID[]
        {
             StatusID.EukrasianDosis,
             StatusID.EukrasianDosis2,
             StatusID.EukrasianDosis3
        },
    };

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction Phlegma { get; } = new BaseAction(ActionID.Phlegma);

    /// <summary>
    /// ����2
    /// </summary>
    public static IBaseAction Phlegma2 { get; } = new BaseAction(ActionID.Phlegma2);

    /// <summary>
    /// ����3
    /// </summary>
    public static IBaseAction Phlegma3 { get; } = new BaseAction(ActionID.Phlegma3);

    /// <summary>
    /// ���
    /// </summary>
    public static IBaseAction Diagnosis { get; } = new BaseAction(ActionID.Diagnosis, true);

    /// <summary>
    /// �Ĺ�
    /// </summary>
    public static IBaseAction Kardia { get; } = new BaseAction(ActionID.Kardia, true)
    {
        StatusProvide = new StatusID[] { StatusID.Kardia },
        ChoiceTarget = (Targets, mustUse) =>
        {
            var targets = Targets.GetJobCategory(JobRole.Tank);
            targets = targets.Any() ? targets : Targets;

            if (!targets.Any()) return null;

            return TargetFilter.FindAttackedTarget(targets, mustUse);
        },
        ActionCheck = b => !b.HasStatus(true, StatusID.Kardion),
    };

    /// <summary>
    /// Ԥ��
    /// </summary>
    public static IBaseAction Prognosis { get; } = new BaseAction(ActionID.Prognosis, true, shouldEndSpecial: true, isTimeline: true);

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction Physis { get; } = new BaseAction(ActionID.Physis, true, isTimeline: true);

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction Eukrasia { get; } = new BaseAction(ActionID.Eukrasia, true, isTimeline: true)
    {
        ActionCheck = b => !JobGauge.Eukrasia,
    };

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction Soteria { get; } = new BaseAction(ActionID.Soteria, true, isTimeline: true);

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction Icarus { get; } = new BaseAction(ActionID.Icarus, shouldEndSpecial: true)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    /// <summary>
    /// ������֭
    /// </summary>
    public static IBaseAction Druochole { get; } = new BaseAction(ActionID.Druochole, true, isTimeline: true)
    {
        ActionCheck = b => JobGauge.Addersgall > 0,
    };

    /// <summary>
    /// ʧ��
    /// </summary>
    public static IBaseAction Dyskrasia { get; } = new BaseAction(ActionID.Dyskrasia);

    /// <summary>
    /// �����֭
    /// </summary>
    public static IBaseAction Kerachole { get; } = new BaseAction(ActionID.Kerachole, true, isTimeline: true)
    {
        ActionCheck = b => JobGauge.Addersgall > 0,
    };

    /// <summary>
    /// ������֭
    /// </summary>
    public static IBaseAction Ixochole { get; } = new BaseAction(ActionID.Ixochole, true, isTimeline: true)
    {
        ActionCheck = b => JobGauge.Addersgall > 0,
    };

    /// <summary>
    /// �
    /// </summary>
    public static IBaseAction Zoe { get; } = new BaseAction(ActionID.Zoe, isTimeline: true);

    /// <summary>
    /// ��ţ��֭
    /// </summary>
    public static IBaseAction Taurochole { get; } = new BaseAction(ActionID.Taurochole, true, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
        ActionCheck = b => JobGauge.Addersgall > 0,
    };

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction Toxikon { get; } = new BaseAction(ActionID.Toxikon)
    {
        ActionCheck = b => JobGauge.Addersting > 0,
    };

    /// <summary>
    /// ��Ѫ
    /// </summary>
    public static IBaseAction Haima { get; } = new BaseAction(ActionID.Haima, true, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// �������
    /// </summary>
    public static IBaseAction EukrasianDiagnosis { get; } = new BaseAction(ActionID.EukrasianDiagnosis, true, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// ����Ԥ��
    /// </summary>
    public static IBaseAction EukrasianPrognosis { get; } = new BaseAction(ActionID.EukrasianPrognosis, true, isTimeline: true);

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction Rhizomata { get; } = new BaseAction(ActionID.Rhizomata, isTimeline: true)
    {
        ActionCheck = b => JobGauge.Addersgall < 3,
    };

    /// <summary>
    /// ������
    /// </summary>
    public static IBaseAction Holos { get; } = new BaseAction(ActionID.Holos, true, isTimeline: true);

    /// <summary>
    /// ����Ѫ
    /// </summary>
    public static IBaseAction Panhaima { get; } = new BaseAction(ActionID.Panhaima, true, isTimeline: true);

    /// <summary>
    /// ���
    /// </summary>
    public static IBaseAction Krasis { get; } = new BaseAction(ActionID.Krasis, true, isTimeline: true);

    /// <summary>
    /// �����Ϣ
    /// </summary>
    public static IBaseAction Pneuma { get; } = new BaseAction(ActionID.Pneuma, isTimeline: true);

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction Pepsis { get; } = new BaseAction(ActionID.Pepsis, true, isTimeline: true)
    {
        ActionCheck = b =>
        {
            foreach (var chara in TargetUpdater.PartyMembers)
            {
                if (chara.HasStatus(true, StatusID.EukrasianDiagnosis, StatusID.EukrasianPrognosis)
                && b.WillStatusEndGCD(2, 0, true, StatusID.EukrasianDiagnosis, StatusID.EukrasianPrognosis)
                && chara.GetHealthRatio() < 0.9) return true;
            }

            return false;
        },
    };
}
