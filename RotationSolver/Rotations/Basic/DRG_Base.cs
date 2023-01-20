using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Linq;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Helpers;
using RotationSolver.Data;
using RotationSolver.Actions;

namespace RotationSolver.Rotations.Basic;

internal abstract class DRG_Base : CustomRotation.CustomRotation

{
    private static DRGGauge JobGauge => Service.JobGauges.Get<DRGGauge>();

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
        OtherIDsCombo = new[] { ActionID.RaidenThrust }
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
        OtherIDsCombo = new[] { ActionID.RaidenThrust }
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
    public static IBaseAction PiercingTalon { get; } = new BaseAction(ActionID.PiercingTalon);

    /// <summary>
    /// ����ǹ
    /// </summary>
    public static IBaseAction DoomSpike { get; } = new BaseAction(ActionID.DoomSpike);

    /// <summary>
    /// ���ٴ�
    /// </summary>
    public static IBaseAction SonicThrust { get; } = new BaseAction(ActionID.SonicThrust)
    {
        OtherIDsCombo = new[] { ActionID.DraconianFury }
    };

    /// <summary>
    /// ɽ������
    /// </summary>
    public static IBaseAction CoerthanTorment { get; } = new BaseAction(ActionID.CoerthanTorment);

    /// <summary>
    /// �����
    /// </summary>
    public static IBaseAction SpineshatterDive { get; } = new BaseAction(ActionID.SpineshatterDive);

    /// <summary>
    /// ���׳�
    /// </summary>
    public static IBaseAction DragonfireDive { get; } = new BaseAction(ActionID.DragonfireDive);

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
    public static IBaseAction Stardiver { get; } = new BaseAction(ActionID.Stardiver)
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
            Targets = Targets.Where(b => b.ObjectId != Service.ClientState.LocalPlayer.ObjectId &&
            !b.HasStatus(false, StatusID.Weakness, StatusID.BrinkofDeath)).ToArray();

            if (Targets.Count() == 0) return Player;

            return Targets.GetJobCategory(JobRole.Melee, JobRole.RangedMagicial, JobRole.RangedPhysical, JobRole.Tank).FirstOrDefault();
        },
    };

    /// <summary>
    /// ս������
    /// </summary>
    public static IBaseAction BattleLitany { get; } = new BaseAction(ActionID.BattleLitany, true)
    {
        StatusNeed = new[] { StatusID.PowerSurge },
    };
}
