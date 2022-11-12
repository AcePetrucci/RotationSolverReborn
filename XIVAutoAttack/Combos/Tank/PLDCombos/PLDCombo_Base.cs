using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Tank.PLDCombos;

internal abstract class PLDCombo_Base<TCmd> : JobGaugeCombo<PLDGauge, TCmd> where TCmd : Enum
{

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Paladin, ClassJobID.Gladiator };

    internal sealed override bool HaveShield => Player.HaveStatus(StatusID.IronWill);

    private sealed protected override BaseAction Shield => IronWill;

    protected override bool CanHealSingleSpell => TargetUpdater.PartyMembers.Length == 1 && base.CanHealSingleSpell;


    public static readonly BaseAction
        //��������
        IronWill = new(28, shouldEndSpecial: true),

        //�ȷ潣
        FastBlade = new(9),

        //���ҽ�
        RiotBlade = new(15),

        //��Ѫ��
        GoringBlade = new(3538, isEot: true)
        {
            TargetStatus = new[]
            {
                    StatusID.GoringBlade,
                    StatusID.BladeofValor,
            }
        },

        //սŮ��֮ŭ
        RageofHalone = new(21),

        //��Ȩ��
        RoyalAuthority = new(3539),

        //Ͷ��
        ShieldLob = new(24)
        {
            FilterForTarget = b => TargetFilter.ProvokeTarget(b),
        },

        //ս�ӷ�Ӧ
        FightorFlight = new(20)
        {
            OtherCheck = b =>
            {
                return true;
            },
        },

        //ȫʴն
        TotalEclipse = new(7381),

        //����ն
        Prominence = new(16457),

        //Ԥ��
        Sentinel = new(17)
        {
            BuffsProvide = Rampart.BuffsProvide,
            OtherCheck = BaseAction.TankDefenseSelf,
        },

        //������ת
        CircleofScorn = new(23)
        {
            //OtherCheck = b =>
            //{
            //    if (LocalPlayer.HaveStatus(ObjectStatus.FightOrFlight)) return true;

            //    if (FightorFlight.IsCoolDown) return true;

            //    return false;
            //}
        },

        //���֮��
        SpiritsWithin = new(29)
        {
            //OtherCheck = b =>
            //{
            //    if (LocalPlayer.HaveStatus(ObjectStatus.FightOrFlight)) return true;

            //    if (FightorFlight.IsCoolDown) return true;

            //    return false;
            //}
        },

        //��ʥ����
        HallowedGround = new(30)
        {
            OtherCheck = BaseAction.TankBreakOtherCheck,
        },

        //ʥ��Ļ��
        DivineVeil = new(3540),

        //���ʺ���
        Clemency = new(3541, true, true),

        //��Ԥ
        Intervention = new(7382, true)
        {
            ChoiceTarget = TargetFilter.FindAttackedTarget,
        },

        //��ͣ
        Intervene = new(16461, shouldEndSpecial: true)
        {
            ChoiceTarget = TargetFilter.FindTargetForMoving,
        },

        //���｣
        Atonement = new(16460)
        {
            BuffsNeed = new[] { StatusID.SwordOath },
        },

        //���꽣
        Expiacion = new(25747),

        //Ӣ��֮��
        BladeofValor = new(25750),

        //����֮��
        BladeofTruth = new(25749),

        //����֮��
        BladeofFaith = new(25748)
        {
            BuffsNeed = new[] { StatusID.ReadyForBladeofFaith },
        },

        //������
        Requiescat = new(7383),

        //����
        Confiteor = new(16459)
        {
            OtherCheck = b => Player.CurrentMp >= 1000,
        },

        //ʥ��
        HolyCircle = new(16458)
        {
            OtherCheck = b => Player.CurrentMp >= 2000,
        },

        //ʥ��
        HolySpirit = new(7384)
        {
            OtherCheck = b => Player.CurrentMp >= 2000,
        },

        //��װ����
        PassageofArms = new(7385),

        //����
        Cover = new BaseAction(27, true)
        {
            ChoiceTarget = TargetFilter.FindAttackedTarget,
        },

        //����
        Sheltron = new(3542);
    //�����ͻ�
    //ShieldBash = new BaseAction(16),
}
