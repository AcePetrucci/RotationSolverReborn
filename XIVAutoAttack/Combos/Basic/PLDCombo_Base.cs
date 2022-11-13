using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Basic;

internal abstract class PLDCombo_Base<TCmd> : JobGaugeCombo<PLDGauge, TCmd> where TCmd : Enum
{

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Paladin, ClassJobID.Gladiator };

    internal sealed override bool HaveShield => Player.HaveStatus(true, StatusID.IronWill);

    private sealed protected override BaseAction Shield => IronWill;

    protected override bool CanHealSingleSpell => TargetUpdater.PartyMembers.Length == 1 && base.CanHealSingleSpell;

    /// <summary>
    /// ��������
    /// </summary>
    public static BaseAction IronWill { get; } = new(ActionID.IronWill, shouldEndSpecial: true);

    /// <summary>
    /// �ȷ潣
    /// </summary>
    public static BaseAction FastBlade { get; } = new(ActionID.FastBlade);

    /// <summary>
    /// ���ҽ�
    /// </summary>
    public static BaseAction RiotBlade { get; } = new(ActionID.RiotBlade);

    /// <summary>
    /// ��Ѫ��
    /// </summary>
    public static BaseAction GoringBlade { get; } = new(ActionID.GoringBlade, isEot: true)
    {
        TargetStatus = new[]
        {
                    StatusID.GoringBlade,
                    StatusID.BladeofValor,
            }
    };

    /// <summary>
    /// սŮ��֮ŭ
    /// </summary>
    public static BaseAction RageofHalone { get; } = new(ActionID.RageofHalone);

    /// <summary>
    /// ��Ȩ��
    /// </summary>
    public static BaseAction RoyalAuthority { get; } = new(ActionID.RoyalAuthority);

    /// <summary>
    /// Ͷ��
    /// </summary>
    public static BaseAction ShieldLob { get; } = new(ActionID.ShieldLob)
    {
        FilterForTarget = b => TargetFilter.ProvokeTarget(b),
    };

    /// <summary>
    /// ս�ӷ�Ӧ
    /// </summary>
    public static BaseAction FightorFlight { get; } = new(ActionID.FightorFlight)
    {
        OtherCheck = b =>
        {
            return true;
        },
    };

    /// <summary>
    /// ȫʴն
    /// </summary>
    public static BaseAction TotalEclipse { get; } = new(ActionID.TotalEclipse);

    /// <summary>
    /// ����ն
    /// </summary>
    public static BaseAction Prominence { get; } = new(ActionID.Prominence);

    /// <summary>
    /// Ԥ��
    /// </summary>
    public static BaseAction Sentinel { get; } = new(ActionID.Sentinel)
    {
        BuffsProvide = Rampart.BuffsProvide,
        OtherCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// ������ת
    /// </summary>
    public static BaseAction CircleofScorn { get; } = new(ActionID.CircleofScorn)
    {
        //OtherCheck = b =>
        //{
        //    if (LocalPlayer.HaveStatus(ObjectStatus.FightOrFlight)) return true;

        //    if (FightorFlight.IsCoolDown) return true;

        //    return false;
        //}
    };

    /// <summary>
    /// ���֮��
    /// </summary>
    public static BaseAction SpiritsWithin { get; } = new(ActionID.SpiritsWithin)
    {
        //OtherCheck = b =>
        //{
        //    if (LocalPlayer.HaveStatus(ObjectStatus.FightOrFlight)) return true;

        //    if (FightorFlight.IsCoolDown) return true;

        //    return false;
        //}
    };

    /// <summary>
    /// ��ʥ����
    /// </summary>
    public static BaseAction HallowedGround { get; } = new(ActionID.HallowedGround)
    {
        OtherCheck = BaseAction.TankBreakOtherCheck,
    };
    
    /// <summary>
    /// ʥ��Ļ��
    /// </summary>
    public static BaseAction DivineVeil { get; } = new(ActionID.DivineVeil);

    /// <summary>
    /// ���ʺ���
    /// </summary>
    public static BaseAction Clemency { get; } = new(ActionID.Clemency, true, true);

    /// <summary>
    /// ��Ԥ
    /// </summary>
    public static BaseAction Intervention { get; } = new(ActionID.Intervention, true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// ��ͣ
    /// </summary>
    public static BaseAction Intervene { get; } = new(ActionID.Intervene, shouldEndSpecial: true)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    /// <summary>
    /// ���｣
    /// </summary>
    public static BaseAction Atonement { get; } = new(ActionID.Atonement)
    {
        BuffsNeed = new[] { StatusID.SwordOath },
    };

    /// <summary>
    /// ���꽣
    /// </summary>
    public static BaseAction Expiacion { get; } = new(ActionID.Expiacion);

    /// <summary>
    /// Ӣ��֮��
    /// </summary>
    public static BaseAction BladeofValor { get; } = new(ActionID.BladeofValor);

    /// <summary>
    /// ����֮��
    /// </summary>
    public static BaseAction BladeofTruth { get; } = new(ActionID.BladeofTruth);

    /// <summary>
    /// ����֮��
    /// </summary>
    public static BaseAction BladeofFaith { get; } = new(ActionID.BladeofFaith)
    {
        BuffsNeed = new[] { StatusID.ReadyForBladeofFaith },
    };

    /// <summary>
    /// ������
    /// </summary>
    public static BaseAction Requiescat { get; } = new(ActionID.Requiescat);

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Confiteor { get; } = new(ActionID.Confiteor)
    {
        OtherCheck = b => Player.CurrentMp >= 1000,
    };

    /// <summary>
    /// ʥ��
    /// </summary>
    public static BaseAction HolyCircle { get; } = new(ActionID.HolyCircle)
    {
        OtherCheck = b => Player.CurrentMp >= 2000,
    };

    /// <summary>
    /// ʥ��
    /// </summary>
    public static BaseAction HolySpirit { get; } = new(ActionID.HolySpirit)
    {
        OtherCheck = b => Player.CurrentMp >= 2000,
    };

    /// <summary>
    /// ��װ����
    /// </summary>
    public static BaseAction PassageofArms { get; } = new(ActionID.PassageofArms);

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Cover { get; } = new(ActionID.Cover, true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Sheltron { get; } = new(ActionID.Sheltron);
    //�����ͻ�
    //ShieldBash = new BaseAction(16),
}
