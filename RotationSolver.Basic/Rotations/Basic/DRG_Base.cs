using Dalamud.Game.ClientState.JobGauge.Types;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Attributes;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using RotationSolver.Rotations.CustomRotation;

namespace RotationSolver.Basic.Rotations.Basic;

public abstract class DRG_Base : CustomRotation
{
    private static DRGGauge JobGauge => Service.JobGauges.Get<DRGGauge>();

    public override MedicineType MedicineType => MedicineType.Strength;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Dragoon, ClassJobID.Lancer };

    /// <summary>
    /// ��׼��
    /// </summary>
    public static IBaseAction TrueThrust { get; } = new BaseAction(ActionID.TrueThrust);

    /// <summary>
    /// ��ͨ��
    /// </summary>
    public static IBaseAction VorpalThrust { get; } = new BaseAction(ActionID.VorpalThrust)
    {
        ComboIds = new[] { ActionID.RaidenThrust }
    };

    /// <summary>
    /// ֱ��
    /// </summary>
    public static IBaseAction FullThrust { get; } = new BaseAction(ActionID.FullThrust);

    /// <summary>
    /// ����ǹ
    /// </summary>
    public static IBaseAction Disembowel { get; } = new BaseAction(ActionID.Disembowel)
    {
        ComboIds = new[] { ActionID.RaidenThrust }
    };

    /// <summary>
    /// ӣ��ŭ��
    /// </summary>
    public static IBaseAction ChaosThrust { get; } = new BaseAction(ActionID.ChaosThrust);

    /// <summary>
    /// ������צ
    /// </summary>
    public static IBaseAction FangandClaw { get; } = new BaseAction(ActionID.FangandClaw)
    {
        StatusNeed = new StatusID[] { StatusID.SharperFangandClaw },
    };

    /// <summary>
    /// ��β�����
    /// </summary>
    public static IBaseAction WheelingThrust { get; } = new BaseAction(ActionID.WheelingThrust)
    {
        StatusNeed = new StatusID[] { StatusID.EnhancedWheelingThrust },
    };

    /// <summary>
    /// �ᴩ��
    /// </summary>
    public static IBaseAction PiercingTalon { get; } = new BaseAction(ActionID.PiercingTalon)
    {
        FilterForHostiles = TargetFilter.MeleeRangeTargetFilter,
        ActionCheck = b => !IsLastAction(IActionHelper.MovingActions),
    };

    /// <summary>
    /// ����ǹ
    /// </summary>
    public static IBaseAction DoomSpike { get; } = new BaseAction(ActionID.DoomSpike);

    /// <summary>
    /// ���ٴ�
    /// </summary>
    public static IBaseAction SonicThrust { get; } = new BaseAction(ActionID.SonicThrust)
    {
        ComboIds = new[] { ActionID.DraconianFury }
    };

    /// <summary>
    /// ɽ������
    /// </summary>
    public static IBaseAction CoerthanTorment { get; } = new BaseAction(ActionID.CoerthanTorment);

    /// <summary>
    /// �����
    /// </summary>
    public static IBaseAction SpineshatterDive { get; } = new BaseAction(ActionID.SpineShatterDive);

    /// <summary>
    /// ���׳�
    /// </summary>
    public static IBaseAction DragonfireDive { get; } = new BaseAction(ActionID.DragonFireDive);

    /// <summary>
    /// ��Ծ
    /// </summary>
    public static IBaseAction Jump { get; } = new BaseAction(ActionID.Jump)
    {
        StatusProvide = new StatusID[] { StatusID.DiveReady },
    };

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction HighJump { get; } = new BaseAction(ActionID.HighJump)
    {
        StatusProvide = Jump.StatusProvide,
    };

    /// <summary>
    /// �����
    /// </summary>
    public static IBaseAction MirageDive { get; } = new BaseAction(ActionID.MirageDive)
    {
        StatusNeed = Jump.StatusProvide,
    };

    /// <summary>
    /// ����ǹ
    /// </summary>
    public static IBaseAction Geirskogul { get; } = new BaseAction(ActionID.Geirskogul);

    /// <summary>
    /// ����֮��
    /// </summary>
    public static IBaseAction Nastrond { get; } = new BaseAction(ActionID.Nastrond)
    {
        ActionCheck = b => JobGauge.IsLOTDActive,
    };

    /// <summary>
    /// ׹�ǳ�
    /// </summary>
    public static IBaseAction Stardiver { get; } = new BaseAction(ActionID.StarDiver)
    {
        ActionCheck = b => JobGauge.IsLOTDActive,
    };

    /// <summary>
    /// �����㾦
    /// </summary>
    public static IBaseAction WyrmwindThrust { get; } = new BaseAction(ActionID.WyrmwindThrust)
    {
        ActionCheck = b => JobGauge.FirstmindsFocusCount == 2,
    };

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction LifeSurge { get; } = new BaseAction(ActionID.LifeSurge, true)
    {
        StatusProvide = new[] { StatusID.LifeSurge },

        ActionCheck = b => !IsLastAbility(true, LifeSurge),
    };

    /// <summary>
    /// ��ǹ
    /// </summary>
    public static IBaseAction LanceCharge { get; } = new BaseAction(ActionID.LanceCharge, true);

    /// <summary>
    /// ��������
    /// </summary>
    public static IBaseAction DragonSight { get; } = new BaseAction(ActionID.DragonSight, true)
    {
        ChoiceTarget = (Targets, mustUse) =>
        {
            Targets = Targets.Where(b => b.ObjectId != Player.ObjectId &&
            !b.HasStatus(false, StatusID.Weakness, StatusID.BrinkOfDeath)).ToArray();

            if (Targets.Count() == 0) return Player;

            return Targets.GetJobCategory(JobRole.Melee, JobRole.RangedMagical, JobRole.RangedPhysical, JobRole.Tank).FirstOrDefault();
        },
    };

    /// <summary>
    /// ս������
    /// </summary>
    public static IBaseAction BattleLitany { get; } = new BaseAction(ActionID.BattleLitany, true)
    {
        StatusNeed = new[] { StatusID.PowerSurge },
    };


    [RotationDesc(ActionID.Feint)]
    protected sealed override bool DefenseAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (Feint.CanUse(out act)) return true;
        return false;
    }
}
