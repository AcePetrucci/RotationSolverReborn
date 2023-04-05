namespace RotationSolver.Basic.Rotations.Basic;
public abstract class DNC_Base : CustomRotation
{
    private static DNCGauge JobGauge => Service.JobGauges.Get<DNCGauge>();

    public override MedicineType MedicineType => MedicineType.Dexterity;

    /// <summary>
    /// ��������
    /// </summary>
    protected static bool IsDancing => JobGauge.IsDancing;

    /// <summary>
    /// ����
    /// </summary>
    protected static byte Esprit => JobGauge.Esprit;

    /// <summary>
    /// ������
    /// </summary>
    protected static byte Feathers => JobGauge.Feathers;

    protected static byte CompletedSteps => JobGauge.CompletedSteps;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Dancer };

    /// <summary>
    /// ��к
    /// </summary>
    public static IBaseAction Cascade { get; } = new BaseAction(ActionID.Cascade)
    {
        StatusProvide = new[] { StatusID.SilkenSymmetry }
    };

    /// <summary>
    /// ��Ȫ
    /// </summary>
    public static IBaseAction Fountain { get; } = new BaseAction(ActionID.Fountain)
    {
        StatusProvide = new[] { StatusID.SilkenFlow }
    };

    /// <summary>
    /// ����к
    /// </summary>
    public static IBaseAction ReverseCascade { get; } = new BaseAction(ActionID.ReverseCascade)
    {
        StatusNeed = new[] { StatusID.SilkenSymmetry, StatusID.SilkenSymmetry2 },
    };

    /// <summary>
    /// ׹��Ȫ
    /// </summary>
    public static IBaseAction FountainFall { get; } = new BaseAction(ActionID.FountainFall)
    {
        StatusNeed = new[] { StatusID.SilkenFlow, StatusID.SilkenFlow2 }
    };

    /// <summary>
    /// ���衤��
    /// </summary>
    public static IBaseAction FanDance { get; } = new BaseAction(ActionID.FanDance)
    {
        ActionCheck = b => JobGauge.Feathers > 0,
        StatusProvide = new[] { StatusID.ThreefoldFanDance },
    };

    /// <summary>
    /// �糵
    /// </summary>
    public static IBaseAction Windmill { get; } = new BaseAction(ActionID.Windmill)
    {
        StatusProvide = Cascade.StatusProvide,
    };

    /// <summary>
    /// ������
    /// </summary>
    public static IBaseAction BladeShower { get; } = new BaseAction(ActionID.BladeShower)
    {
        StatusProvide = Fountain.StatusProvide,
    };

    /// <summary>
    /// ���糵
    /// </summary>
    public static IBaseAction RisingWindmill { get; } = new BaseAction(ActionID.RisingWindmill)
    {
        StatusNeed = ReverseCascade.StatusNeed,
    };

    /// <summary>
    /// ��Ѫ��
    /// </summary>
    public static IBaseAction BloodShower { get; } = new BaseAction(ActionID.BloodShower)
    {
        AOECount = 2,
        StatusNeed = FountainFall.StatusNeed,
    };

    /// <summary>
    /// ���衤��
    /// </summary>
    public static IBaseAction FanDance2 { get; } = new BaseAction(ActionID.FanDance2)
    {
        ActionCheck = b => Feathers > 0,
        AOECount = 2,
        StatusProvide = new[] { StatusID.ThreefoldFanDance },
    };

    /// <summary>
    /// ���衤��
    /// </summary>
    public static IBaseAction FanDance3 { get; } = new BaseAction(ActionID.FanDance3)
    {
        StatusNeed = FanDance2.StatusProvide,
    };

    /// <summary>
    /// ���衤��
    /// </summary>
    public static IBaseAction FanDance4 { get; } = new BaseAction(ActionID.FanDance4)
    {
        StatusNeed = new[] { StatusID.FourfoldFanDance },
    };

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction SaberDance { get; } = new BaseAction(ActionID.SaberDance)
    {
        ActionCheck = b => Esprit >= 50,
    };

    /// <summary>
    /// ������
    /// </summary>
    public static IBaseAction StarFallDance { get; } = new BaseAction(ActionID.StarFallDance)
    {
        StatusNeed = new[] { StatusID.FlourishingStarfall },
    };

    /// <summary>
    /// ǰ�岽
    /// </summary>
    public static IBaseAction EnAvant { get; } = new BaseAction(ActionID.EnAvant, true, shouldEndSpecial: true);

    /// <summary>
    /// Ǿޱ���Ų�
    /// </summary>
    private static IBaseAction Emboite { get; } = new BaseAction(ActionID.Emboite, true)
    {
        ActionCheck = b => (ActionID)JobGauge.NextStep == ActionID.Emboite,
    };

    /// <summary>
    /// С�񽻵���
    /// </summary>
    private static IBaseAction Entrechat { get; } = new BaseAction(ActionID.Entrechat, true)
    {
        ActionCheck = b => (ActionID)JobGauge.NextStep == ActionID.Entrechat,
    };

    /// <summary>
    /// ��ҶС����
    /// </summary>
    private static IBaseAction Jete { get; } = new BaseAction(ActionID.Jete, true)
    {
        ActionCheck = b => (ActionID)JobGauge.NextStep == ActionID.Jete,
    };

    /// <summary>
    /// ���ֺ��ת
    /// </summary>
    private static IBaseAction Pirouette { get; } = new BaseAction(ActionID.Pirouette, true)
    {
        ActionCheck = b => (ActionID)JobGauge.NextStep == ActionID.Pirouette,
    };

    /// <summary>
    /// ��׼�貽
    /// </summary>
    public static IBaseAction StandardStep { get; } = new BaseAction(ActionID.StandardStep)
    {
        StatusProvide = new[]
        {
            StatusID.StandardStep,
            StatusID.TechnicalStep,
        },
    };

    /// <summary>
    /// �����貽
    /// </summary>
    public static IBaseAction TechnicalStep { get; } = new BaseAction(ActionID.TechnicalStep)
    {
        StatusNeed = new[]
        {
            StatusID.StandardFinish,
        },
        StatusProvide = StandardStep.StatusProvide,
    };

    /// <summary>
    /// ��׼�貽����
    /// </summary>
    protected static IBaseAction StandardFinish { get; } = new BaseAction(ActionID.StandardFinish)
    {
        StatusNeed = new[] { StatusID.StandardStep },
        ActionCheck = b => IsDancing && JobGauge.CompletedSteps == 2,
    };

    /// <summary>
    /// �����貽����
    /// </summary>
    protected static IBaseAction TechnicalFinish { get; } = new BaseAction(ActionID.TechnicalFinish)
    {
        StatusNeed = new[] { StatusID.TechnicalStep },
        ActionCheck = b => IsDancing && JobGauge.CompletedSteps == 4,
    };

    /// <summary>
    /// ����֮ɣ��
    /// </summary>
    public static IBaseAction ShieldSamba { get; } = new BaseAction(ActionID.ShieldSamba, true, isTimeline: true)
    {
        ActionCheck = b => !Player.HasStatus(false, StatusID.Troubadour,
            StatusID.Tactician1,
            StatusID.Tactician2,
            StatusID.ShieldSamba),
    };

    /// <summary>
    /// ����֮������
    /// </summary>
    public static IBaseAction CuringWaltz { get; } = new BaseAction(ActionID.CuringWaltz, true, isTimeline: true);

    /// <summary>
    /// ��ʽ����
    /// </summary>
    public static IBaseAction ClosedPosition { get; } = new BaseAction(ActionID.ClosedPosition, true)
    {
        ChoiceTarget = (Targets, mustUse) =>
        {
            Targets = Targets.Where(b => b.ObjectId != Player.ObjectId && b.CurrentHp != 0 &&
            //Remove Weak
            !b.HasStatus(false, StatusID.Weakness, StatusID.BrinkOfDeath)
            //Remove other partner.
            && !b.HasStatus(false, StatusID.ClosedPosition2) | b.HasStatus(true, StatusID.ClosedPosition2)
            );

            return Targets.GetJobCategory(JobRole.Melee, JobRole.RangedMagical, JobRole.RangedPhysical).FirstOrDefault();
        },
    };

    /// <summary>
    /// ����֮̽��
    /// </summary>
    public static IBaseAction Devilment { get; } = new BaseAction(ActionID.Devilment, true);

    /// <summary>
    /// �ٻ�����
    /// </summary>
    public static IBaseAction Flourish { get; } = new BaseAction(ActionID.Flourish, true)
    {
        StatusNeed = new[] { StatusID.StandardFinish },
        StatusProvide = new[]
        {
            StatusID.ThreefoldFanDance,
            StatusID.FourfoldFanDance,
        },
        ActionCheck = b => InCombat,
    };

    /// <summary>
    /// ���˱���
    /// </summary>
    public static IBaseAction Improvisation { get; } = new BaseAction(ActionID.Improvisation, true);

    /// <summary>
    /// ������
    /// </summary>
    public static IBaseAction Tillana { get; } = new BaseAction(ActionID.Tillana)
    {
        StatusNeed = new[] { StatusID.FlourishingFinish },
    };

    /// <summary>
    /// ִ���貽
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    protected static bool ExecuteStepGCD(out IAction act)
    {
        act = null;
        if (!Player.HasStatus(true, StatusID.StandardStep, StatusID.TechnicalStep)) return false;
        if (Player.HasStatus(true, StatusID.StandardStep) && CompletedSteps == 2) return false;
        if (Player.HasStatus(true, StatusID.TechnicalStep) && CompletedSteps == 4) return false;

        if (Emboite.CanUse(out act)) return true;
        if (Entrechat.CanUse(out act)) return true;
        if (Jete.CanUse(out act)) return true;
        if (Pirouette.CanUse(out act)) return true;
        return false;
    }

    [RotationDesc(ActionID.EnAvant)]
    protected sealed override bool MoveForwardAbility(byte abilitiesRemaining, out IAction act, CanUseOption option = CanUseOption.None)
    {
        if (EnAvant.CanUse(out act, CanUseOption.EmptyOrSkipCombo | option)) return true;
        return false;
    }

    [RotationDesc(ActionID.CuringWaltz, ActionID.Improvisation)]
    protected sealed override bool HealAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (CuringWaltz.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
        if (Improvisation.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
        return false;
    }

    [RotationDesc(ActionID.ShieldSamba)]
    protected sealed override bool DefenseAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (ShieldSamba.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
        return false;
    }
}
