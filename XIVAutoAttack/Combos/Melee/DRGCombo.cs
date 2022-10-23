using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Combos.Healer;
using XIVAutoAttack.Configuration;

namespace XIVAutoAttack.Combos.Melee;

internal class DRGCombo : JobGaugeCombo<DRGGauge>
{
    internal override uint JobID => 22;
    private static bool inOpener = true;
    private static bool safeMove = false;

    internal struct Actions
    {
        public static readonly BaseAction
            //��׼��
            TrueThrust = new (75),

            //��ͨ��
            VorpalThrust = new (78) { OtherIDsCombo = new [] { 16479u } },

            //ֱ��
            FullThrust = new (84),

            //����
            HeavensThrust = new (25771),

            //����ǹ
            Disembowel = new (87) { OtherIDsCombo = new [] { 16479u } },

            //ӣ��ŭ��
            ChaosThrust = new (88),

            //ӣ��ŭ��
            ChaoticSpring = new(25772),

            //������צ
            FangandClaw = new (3554)
            {
                BuffsNeed = new ushort[] { ObjectStatus.SharperFangandClaw },
            },

            //��β�����
            WheelingThrust = new (3556)
            {
                BuffsNeed = new ushort[] { ObjectStatus.EnhancedWheelingThrust },
            },

            //�����׵�
            RaidenThrust = new (16479),

            //�ᴩ��
            PiercingTalon = new (90),

            //����ǹ
            DoomSpike = new (86),

            //���ٴ�
            SonicThrust = new (7397) { OtherIDsCombo = new [] { 25770u } },

            //ɽ������
            CoerthanTorment = new (16477),

            //�����
            SpineshatterDive = new (95)
            {
                OtherCheck = b =>
                {

                    if (safeMove && b.DistanceToPlayer() > 2) return false;
                    if (IsLastAction(true, SpineshatterDive)) return false;

                    if (inOpener && IsLastWeaponSkill(true,  FangandClaw, HeavensThrust)) return true;
                    if (!inOpener) return true;

                    return false;
                }
            },

            //���׳�
            DragonfireDive = new (96)
            {
                OtherCheck = b =>
                {
                    if (safeMove && b.DistanceToPlayer() > 2) return false;

                    if (inOpener && IsLastWeaponSkill(true, RaidenThrust)) return true;
                    if (!inOpener) return true;

                    return false;
                }
            },

            //��Ծ
            Jump = new (92)
            {
                OtherCheck = b =>
                {
                    if (StatusHelper.HaveStatusFromSelf(ObjectStatus.DiveReady)) return false;

                    if (safeMove && b.DistanceToPlayer() > 2) return false;

                    if (inOpener && IsLastWeaponSkill(true, ChaoticSpring)) return true;
                    if (!inOpener) return true;

                    return false;
                },
            },
            //����
            HighJump = new (16478)
            {
                OtherCheck = Jump.OtherCheck,
            },
            //�����
            MirageDive = new (7399)
            {
                BuffsNeed = new [] { ObjectStatus.DiveReady },

                OtherCheck = b =>
                {
                    if (Geirskogul.RecastTimeRemain < 12) return false;
                    if (inOpener && IsLastWeaponSkill(true, FangandClaw)) return true;
                    if (!inOpener) return true;

                    return false;
                },
            },

            //����ǹ
            Geirskogul = new (3555)
            {
                OtherCheck = b =>
                {
                    if (inOpener && IsLastWeaponSkill(true, ChaoticSpring)) return true;
                    if (!inOpener) return true;

                    return false;
                },
            },

            //����֮��
            Nastrond = new (7400)
            {
                OtherCheck = b => JobGauge.IsLOTDActive,
            },

            //׹�ǳ�
            Stardiver = new (16480)
            {
                OtherCheck = b => JobGauge.IsLOTDActive && JobGauge.LOTDTimer < 25000,
            },

            //�����㾦
            WyrmwindThrust = new (25773)
            {
                OtherCheck = b => JobGauge.FirstmindsFocusCount == 2 && !IsLastAction(true, Stardiver),
            },

            //����
            LifeSurge = new (83) 
            { 
                BuffsProvide = new [] { ObjectStatus.LifeSurge },

                OtherCheck = b => !IsLastAbility(true, LifeSurge),
            },

            //��ǹ
            LanceCharge = new (85)
            {
                OtherCheck = b =>
                {
                    if (inOpener && IsLastWeaponSkill(true, TrueThrust)) return true;
                    if (!inOpener) return true;

                    return false;
                }
            },

            //��������
            DragonSight = new (7398)
            {
                ChoiceTarget = Targets =>
                {
                    Targets = Targets.Where(b => b.ObjectId != Service.ClientState.LocalPlayer.ObjectId &&
                    b.StatusList.Select(status => status.StatusId).Intersect(new uint[] { ObjectStatus.Weakness, ObjectStatus.BrinkofDeath }).Count() == 0).ToArray();

                    var targets = TargetFilter.GetJobCategory(Targets, Role.��ս);
                    if (targets.Length > 0) return TargetFilter.RandomObject(targets);

                    targets = TargetFilter.GetJobCategory(Targets, Role.Զ��);
                    if (targets.Length > 0) return TargetFilter.RandomObject(targets);

                    targets = Targets;
                    if (targets.Length > 0) return TargetFilter.RandomObject(targets);

                    return LocalPlayer;
                },

                BuffsNeed = new [] {ObjectStatus.PowerSurge},


                OtherCheck = b =>
                {
                    //����buff��
                    if (!StatusHelper.HaveStatusFromSelf(ObjectStatus.LanceCharge)) return false;

                    return true;
                }

            },

            //ս������
            BattleLitany = new (3557)
            {
                BuffsNeed = new[] { ObjectStatus.PowerSurge },

                OtherCheck = b =>
                {
                    //����buff��
                    if (!StatusHelper.HaveStatusFromSelf(ObjectStatus.LanceCharge)) return false;

                    return true;
                }
            };
    }

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("DRG_ShouldDelay", true, "�Ӻ����Ѫ")
            .SetBool("DRG_Opener", true, "88������")
            .SetBool("DRG_SafeMove", true, "��ȫλ��");
    }

    internal override SortedList<DescType, string> Description => new SortedList<DescType, string>()
    {
        {DescType.�ƶ�, $"{Actions.SpineshatterDive.Action.Name}, {Actions.DragonfireDive.Action.Name}"},
    };

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (abilityRemain > 1)
        {
            if (Actions.SpineshatterDive.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
            if (Actions.DragonfireDive.ShouldUse(out act, mustUse: true)) return true;
        }

        act = null;
        return false;
    }
    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (nextGCD == Actions.FullThrust || nextGCD == Actions.CoerthanTorment|| StatusHelper.HaveStatusFromSelf(ObjectStatus.LanceCharge) && nextGCD == Actions.FangandClaw)
        {
            //����
            if (abilityRemain ==1 && Actions.LifeSurge.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        }

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        //��ǹ
        if (inOpener && abilityRemain == 1 && Actions.LanceCharge.ShouldUse(out act, mustUse: true)) return true;
        if (!inOpener && Actions.LanceCharge.ShouldUse(out act, mustUse: true)) return true;

        //��������
        if (Actions.DragonSight.ShouldUse(out act, mustUse: true)) return true;

        //ս������
        if (Actions.BattleLitany.ShouldUse(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        //����֮��
        if (Actions.Nastrond.ShouldUse(out act, mustUse: true)) return true;

        //׹�ǳ�
        if (Actions.Stardiver.ShouldUse(out act, mustUse: true)) return true;

        //����
        if (Level >= Actions.HighJump.Level)
        {
            if (Actions.HighJump.ShouldUse(out act)) return true;
        }
        else
        {
            if (Actions.Jump.ShouldUse(out act)) return true;
        }

        //���Խ������Ѫ
        if (Actions.Geirskogul.ShouldUse(out act, mustUse: true)) return true;

        //�����
        if (StatusHelper.HaveStatusFromSelf(ObjectStatus.RightEye) && Actions.SpineshatterDive.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        if (Actions.SpineshatterDive.ShouldUse(out act)) return true;

        //�����
        if (Actions.MirageDive.ShouldUse(out act)) return true;

        //���׳�
        if (Actions.DragonfireDive.ShouldUse(out act, mustUse: true)) return true;

        //�����㾦
        if (Actions.WyrmwindThrust.ShouldUse(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        safeMove = Config.GetBoolByName("DRG_SafeMove");

        if (!InBattle && Config.GetBoolByName("DRG_Opener") && Service.ClientState.LocalPlayer!.Level >= 88)
        {
            inOpener = false;

            if (!Actions.LanceCharge.IsCoolDown && !Actions.BattleLitany.IsCoolDown)
            {
                inOpener = true;
            }
        }
        if (Actions.BattleLitany.IsCoolDown && !StatusHelper.HaveStatusFromSelf(ObjectStatus.LanceCharge))
        {
            inOpener = false;
        }
        #region Ⱥ��
        if (Actions.CoerthanTorment.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.SonicThrust.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.DoomSpike.ShouldUse(out act, lastComboActionID)) return true;

        #endregion

        #region ����
        if (Config.GetBoolByName("ShouldDelay"))
        {
            if (Actions.WheelingThrust.ShouldUse(out act)) return true;
            if (Actions.FangandClaw.ShouldUse(out act)) return true;
        }
        else
        {
            if (Actions.FangandClaw.ShouldUse(out act)) return true;
            if (Actions.WheelingThrust.ShouldUse(out act)) return true;
        }

        //�����Ƿ���Ҫ��Buff
        var time = StatusHelper.FindStatusTimesSelfFromSelf(ObjectStatus.PowerSurge);
        if (time.Length > 0 && time[0] > 13)
        {
            if (Actions.FullThrust.ShouldUse(out act, lastComboActionID)) return true;
            if (Actions.VorpalThrust.ShouldUse(out act, lastComboActionID)) return true;
            if (Actions.ChaosThrust.ShouldUse(out act, lastComboActionID)) return true;
        }
        else
        {
            if (Actions.Disembowel.ShouldUse(out act, lastComboActionID)) return true;
        }
        if (Actions.TrueThrust.ShouldUse(out act)) return true;

        if (IconReplacer.Move && MoveAbility(1, out act)) return true;
        if (Actions.PiercingTalon.ShouldUse(out act)) return true;

        return false;

        #endregion
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //ǣ��
        if (GeneralActions.Feint.ShouldUse(out act)) return true;
        return false;
    }
}
