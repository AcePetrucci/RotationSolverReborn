using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Attributes;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using RotationSolver.Rotations.CustomRotation;

namespace RotationSolver.Basic.Rotations.Basic;

public abstract class BRD_Base : CustomRotation
{
    private static BRDGauge JobGauge => Service.JobGauges.Get<BRDGauge>();

    public override MedicineType MedicineType => MedicineType.Dexterity;

    /// <summary>
    /// ʫ������
    /// </summary>
    protected static byte Repertoire => JobGauge.Repertoire;

    /// <summary>
    /// ��ǰ���ڳ��ĸ�
    /// </summary>
    protected static Song Song => JobGauge.Song;

    /// <summary>
    /// ��һ�׳��ĸ�
    /// </summary>
    protected static Song LastSong => JobGauge.LastSong;

    /// <summary>
    /// ���֮��
    /// </summary>
    protected static byte SoulVoice => JobGauge.SoulVoice;

    /// <summary>
    /// ���׸谡�ڶ�ú��ڳ���(�Ƿ��Ѿ�����)
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool SongEndAfter(float time) => EndAfter(SongTime, time);

    /// <summary>
    /// ���׸谡�ڶ�ú��ڳ���
    /// </summary>
    /// <param name="abilityCount"></param>
    /// <param name="gctCount"></param>
    /// <returns></returns>
    protected static bool SongEndAfterGCD(uint gctCount = 0, uint abilityCount = 0) => EndAfterGCD(SongTime, gctCount, abilityCount);

    private static float SongTime => JobGauge.SongTimer / 1000f;

    public sealed override ClassJobID[] JobIDs => new[] { ClassJobID.Bard, ClassJobID.Archer };

    /// <summary>
    /// ǿ�����
    /// </summary>
    public static IBaseAction HeavyShoot { get; } = new BaseAction(ActionID.HeavyShoot) { StatusProvide = new[] { StatusID.StraightShotReady } };

    /// <summary>
    /// ֱ�����
    /// </summary>
    public static IBaseAction StraitShoot { get; } = new BaseAction(ActionID.StraitShoot) { StatusNeed = new[] { StatusID.StraightShotReady } };

    /// <summary>
    /// ��ҧ��
    /// </summary>
    public static IBaseAction VenomousBite { get; } = new BaseAction(ActionID.VenomousBite, isEot: true)
    {
        TargetStatus = new[] { StatusID.VenomousBite, StatusID.CausticBite }
    };

    /// <summary>
    /// ��ʴ��
    /// </summary>
    public static IBaseAction WindBite { get; } = new BaseAction(ActionID.WindBite, isEot: true)
    {
        TargetStatus = new[] { StatusID.WindBite, StatusID.StormBite }
    };

    /// <summary>
    /// ��������
    /// </summary>
    public static IBaseAction IronJaws { get; } = new BaseAction(ActionID.IronJaws, isEot: true)
    {
        TargetStatus = VenomousBite.TargetStatus.Union(WindBite.TargetStatus).ToArray(),
        ActionCheck = b => b.HasStatus(true, VenomousBite.TargetStatus) & b.HasStatus(true, WindBite.TargetStatus),
    };

    /// <summary>
    /// �������С������
    /// </summary>
    public static IBaseAction WanderersMinuet { get; } = new BaseAction(ActionID.WanderersMinuet);

    /// <summary>
    /// ���ߵ�����ҥ
    /// </summary>
    public static IBaseAction MagesBallad { get; } = new BaseAction(ActionID.MagesBallad);

    /// <summary>
    /// �����������
    /// </summary>
    public static IBaseAction ArmysPaeon { get; } = new BaseAction(ActionID.ArmysPaeon);

    /// <summary>
    /// ս��֮��
    /// </summary>
    public static IBaseAction BattleVoice { get; } = new BaseAction(ActionID.BattleVoice, true);

    /// <summary>
    /// ����ǿ��
    /// </summary>
    public static IBaseAction RagingStrikes { get; } = new BaseAction(ActionID.RagingStrikes, true);

    /// <summary>
    /// ���������������
    /// </summary>
    public static IBaseAction RadiantFinale { get; } = new BaseAction(ActionID.RadiantFinale, true)
    {
        ActionCheck = b => JobGauge.Coda.Any(s => s != Song.NONE),
    };

    /// <summary>
    /// ���Ҽ�
    /// </summary>
    public static IBaseAction Barrage { get; } = new BaseAction(ActionID.Barrage);

    /// <summary>
    /// ��������
    /// </summary>
    public static IBaseAction EmpyrealArrow { get; } = new BaseAction(ActionID.EmpyrealArrow);

    /// <summary>
    /// ��������
    /// </summary>
    public static IBaseAction PitchPerfect { get; } = new BaseAction(ActionID.PitchPerfect)
    {
        ActionCheck = b => JobGauge.Song == Song.WANDERER,
    };

    /// <summary>
    /// ʧѪ��
    /// </summary>
    public static IBaseAction Bloodletter { get; } = new BaseAction(ActionID.Bloodletter);

    /// <summary>
    /// ��������
    /// </summary>
    public static IBaseAction RainOfDeath { get; } = new BaseAction(ActionID.RainOfDeath)
    {
        AOECount = 2,
    };

    /// <summary>
    /// �����
    /// </summary>
    public static IBaseAction QuickNock { get; } = new BaseAction(ActionID.QuickNock)
    {
        StatusProvide = new[] { StatusID.ShadowBiteReady }
    };

    /// <summary>
    /// Ӱ�ɼ�
    /// </summary>
    public static IBaseAction ShadowBite { get; } = new BaseAction(ActionID.ShadowBite)
    {
        StatusNeed = new[] { StatusID.ShadowBiteReady }
    };

    /// <summary>
    /// ����������޿���
    /// </summary>
    public static IBaseAction WardensPaean { get; } = new BaseAction(ActionID.WardensPaean, true, isTimeline: true);

    /// <summary>
    /// ��������������
    /// </summary>
    public static IBaseAction NaturesMinne { get; } = new BaseAction(ActionID.NaturesMinne, true, isTimeline: true);

    /// <summary>
    /// ����յ���
    /// </summary>
    public static IBaseAction Sidewinder { get; } = new BaseAction(ActionID.Sidewinder);

    /// <summary>
    /// �����
    /// </summary>
    public static IBaseAction ApexArrow { get; } = new BaseAction(ActionID.ApexArrow)
    {
        ActionCheck = b => JobGauge.SoulVoice >= 20 && !Player.HasStatus(true, StatusID.BlastArrowReady),
    };

    /// <summary>
    /// ���Ƽ�
    /// </summary>
    public static IBaseAction BlastArrow { get; } = new BaseAction(ActionID.BlastArrow)
    {
        ActionCheck = b => Player.HasStatus(true, StatusID.BlastArrowReady),
    };

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction Troubadour { get; } = new BaseAction(ActionID.Troubadour, true, isTimeline: true)
    {
        ActionCheck = b => !Player.HasStatus(false, StatusID.Troubadour,
            StatusID.Tactician1,
            StatusID.Tactician2,
            StatusID.ShieldSamba),
    };

    protected override bool EmergencyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //��ĳЩ�ǳ�Σ�յ�״̬��
        if (DataCenter.SpecialType == SpecialCommandType.EsunaStanceNorth && DataCenter.WeakenPeople.Any() || DataCenter.DyingPeople.Any())
        {
            if (WardensPaean.CanUse(out act, CanUseOption.MustUse)) return true;
        }
        return base.EmergencyAbility(abilityRemain, nextGCD, out act);
    }

    [RotationDesc(ActionID.Troubadour)]
    protected sealed override bool DefenseAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (Troubadour.CanUse(out act)) return true;
        return false;
    }

    [RotationDesc(ActionID.NaturesMinne)]
    protected sealed override bool HealSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        if (NaturesMinne.CanUse(out act)) return true;
        return false;
    }
}
