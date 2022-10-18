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

                    if (safeMove && TargetFilter.DistanceToPlayer(Target) > 2) return false;
                    if (LastAction == SpineshatterDive.ID) return false;

                    if (inOpener && (LastWeaponskill == FangandClaw.ID || LastWeaponskill == HeavensThrust.ID)) return true;
                    if (!inOpener) return true;

                    return false;
                }
            },

            //���׳�
            DragonfireDive = new (96)
            {
                OtherCheck = b =>
                {
                    if (safeMove && TargetFilter.DistanceToPlayer(Target) > 2) return false;

                    if (inOpener && LastWeaponskill == RaidenThrust.ID) return true;
                    if (!inOpener) return true;

                    return false;
                }
            },

            //��Ծ
            Jump = new (92)
            {
                OtherCheck = b =>
                {
                    if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.DiveReady)) return false;

                    if (safeMove && Vector3.Distance(LocalPlayer.Position, b.Position) - b.HitboxRadius > 2) return false;

                    if (inOpener && LastWeaponskill == ChaoticSpring.ID) return true;
                    if (!inOpener) return true;

                    return false;
                },
            },
            //����
            HighJump = new (16478)
            {
                OtherCheck = b =>
                {
                    if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.DiveReady)) return false;

                    if (safeMove && Vector3.Distance(LocalPlayer.Position, b.Position) - b.HitboxRadius > 2) return false;

                    if (inOpener && LastWeaponskill == ChaoticSpring.ID) return true;
                    if (!inOpener) return true;

                    return false;
                },
            },
            //�����
            MirageDive = new (7399)
            {
                BuffsNeed = new [] { ObjectStatus.DiveReady },

                OtherCheck = b =>
                {
                    if (Geirskogul.RecastTimeRemain < 12) return false;
                    if (inOpener && LastWeaponskill == FangandClaw.ID) return true;
                    if (!inOpener) return true;

                    return false;
                },
            },

            //����ǹ
            Geirskogul = new (3555)
            {
                OtherCheck = b =>
                {
                    if (inOpener && LastWeaponskill == ChaoticSpring.ID) return true;
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
                OtherCheck = b => JobGauge.FirstmindsFocusCount == 2 && LastAction != Stardiver.ID,
            },

            //����
            LifeSurge = new (83) 
            { 
                BuffsProvide = new [] { ObjectStatus.LifeSurge },

                OtherCheck = b => LastAbility != LifeSurge.ID,
            },

            //��ǹ
            LanceCharge = new (85)
            {
                OtherCheck = b =>
                {
                    if (inOpener && LastWeaponskill == TrueThrust.ID) return true;
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
                    if (targets.Length > 0) return ASTCombo.RandomObject(targets);

                    targets = TargetFilter.GetJobCategory(Targets, Role.Զ��);
                    if (targets.Length > 0) return ASTCombo.RandomObject(targets);

                    targets = Targets;
                    if (targets.Length > 0) return ASTCombo.RandomObject(targets);

                    return LocalPlayer;
                },

                BuffsNeed = new [] {ObjectStatus.PowerSurge},


                OtherCheck = b =>
                {
                    //����buff��
                    if (!StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.LanceCharge)) return false;

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
                    if (!StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.LanceCharge)) return false;

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
            if (Actions.SpineshatterDive.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
            if (Actions.DragonfireDive.ShouldUseAction(out act, mustUse: true)) return true;
        }

        act = null;
        return false;
    }
    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (nextGCD.ID == Actions.FullThrust.ID || nextGCD.ID == Actions.CoerthanTorment.ID || StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.LanceCharge) && nextGCD == Actions.FangandClaw)
        {
            //����
            if (abilityRemain ==1 && Actions.LifeSurge.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
        }

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        //��ǹ
        if (inOpener && abilityRemain == 1 && Actions.LanceCharge.ShouldUseAction(out act, mustUse: true)) return true;
        if (!inOpener && Actions.LanceCharge.ShouldUseAction(out act, mustUse: true)) return true;

        //��������
        if (Actions.DragonSight.ShouldUseAction(out act, mustUse: true)) return true;

        //ս������
        if (Actions.BattleLitany.ShouldUseAction(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        //����֮��
        if (Actions.Nastrond.ShouldUseAction(out act, mustUse: true)) return true;

        //׹�ǳ�
        if (Actions.Stardiver.ShouldUseAction(out act, mustUse: true)) return true;

        //����
        if (Service.ClientState.LocalPlayer.Level >= Actions.HighJump.Level)
        {
            if (Actions.HighJump.ShouldUseAction(out act)) return true;
        }
        else
        {
            if (Actions.Jump.ShouldUseAction(out act)) return true;
        }

        //���Խ������Ѫ
        if (Actions.Geirskogul.ShouldUseAction(out act, mustUse: true)) return true;

        //�����
        if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.RightEye) && Actions.SpineshatterDive.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
        if (Actions.SpineshatterDive.ShouldUseAction(out act)) return true;

        //�����
        if (Actions.MirageDive.ShouldUseAction(out act)) return true;

        //���׳�
        if (Actions.DragonfireDive.ShouldUseAction(out act, mustUse: true)) return true;

        //�����㾦
        if (Actions.WyrmwindThrust.ShouldUseAction(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        safeMove = Config.GetBoolByName("DRG_SafeMove");

        if (!TargetHelper.InBattle && Config.GetBoolByName("DRG_Opener") && Service.ClientState.LocalPlayer!.Level >= 88)
        {
            inOpener = false;

            if (!Actions.LanceCharge.IsCoolDown && !Actions.BattleLitany.IsCoolDown)
            {
                inOpener = true;
            }
        }
        if (Actions.BattleLitany.IsCoolDown && !StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.LanceCharge))
        {
            inOpener = false;
        }
        #region Ⱥ��
        if (Actions.CoerthanTorment.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.SonicThrust.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.DoomSpike.ShouldUseAction(out act, lastComboActionID)) return true;

        #endregion

        #region ����
        if (Config.GetBoolByName("ShouldDelay"))
        {
            if (Actions.WheelingThrust.ShouldUseAction(out act)) return true;
            if (Actions.FangandClaw.ShouldUseAction(out act)) return true;
        }
        else
        {
            if (Actions.FangandClaw.ShouldUseAction(out act)) return true;
            if (Actions.WheelingThrust.ShouldUseAction(out act)) return true;
        }

        //�����Ƿ���Ҫ��Buff
        var time = StatusHelper.FindStatusSelfFromSelf(ObjectStatus.PowerSurge);
        if (time.Length > 0 && time[0] > 13)
        {
            if (Actions.FullThrust.ShouldUseAction(out act, lastComboActionID)) return true;
            if (Actions.VorpalThrust.ShouldUseAction(out act, lastComboActionID)) return true;
            if (Actions.ChaosThrust.ShouldUseAction(out act, lastComboActionID)) return true;
        }
        else
        {
            if (Actions.Disembowel.ShouldUseAction(out act, lastComboActionID)) return true;
        }
        if (Actions.TrueThrust.ShouldUseAction(out act)) return true;

        if (IconReplacer.Move && MoveAbility(1, out act)) return true;
        if (Actions.PiercingTalon.ShouldUseAction(out act)) return true;

        return false;

        #endregion
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //ǣ��
        if (GeneralActions.Feint.ShouldUseAction(out act)) return true;
        return false;
    }
}
