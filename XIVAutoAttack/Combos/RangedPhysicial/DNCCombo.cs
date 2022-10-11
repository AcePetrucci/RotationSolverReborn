using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.CustomCombo;

namespace XIVAutoAttack.Combos.RangedPhysicial;

internal class DNCCombo : JobGaugeCombo<DNCGauge>
{
    internal override uint JobID => 38;

    internal struct Actions
    {
        public static readonly BaseAction

            //��к
            Cascade = new (15989)
            {
                BuffsProvide = new [] { ObjectStatus.SilkenSymmetry}
            },

            //��Ȫ
            Fountain = new (15990)
            {
                BuffsProvide = new [] { ObjectStatus.SilkenFlow }
            },

            //����к
            ReverseCascade = new (15991)
            {
                BuffsNeed = new [] { ObjectStatus.SilkenSymmetry, ObjectStatus.SilkenSymmetry2 },
            },

            //׹��Ȫ
            Fountainfall = new (15992)
            {
                BuffsNeed = new [] { ObjectStatus.SilkenFlow, ObjectStatus.SilkenFlow2 }
            },

            //���衤��
            FanDance = new (16007)
            {
                OtherCheck = b => JobGauge.Feathers > 0,
                BuffsProvide = new [] { ObjectStatus.ThreefoldFanDance },
            },

            //�糵
            Windmill = new (15993)
            {
                BuffsProvide = new [] { ObjectStatus.SilkenSymmetry }
            },

            //������
            Bladeshower = new (15994)
            {
                BuffsProvide = new [] { ObjectStatus.SilkenFlow }
            },

            //���糵
            RisingWindmill = new (15995)
            {
                BuffsNeed = new [] { ObjectStatus.SilkenSymmetry, ObjectStatus.SilkenSymmetry2 },
            },

            //��Ѫ��
            Bloodshower = new (15996)
            {
                BuffsNeed = new [] { ObjectStatus.SilkenFlow, ObjectStatus.SilkenFlow2 }
            },

            //���衤��
            FanDance2 = new (16008)
            {
                OtherCheck = b => JobGauge.Feathers > 0,
                BuffsProvide = new [] { ObjectStatus.ThreefoldFanDance },
            },

            //���衤��
            FanDance3 = new (16009)
            {
                BuffsNeed = new [] { ObjectStatus.ThreefoldFanDance },
            },

            //���衤��
            FanDance4 = new (25791)
            {
                BuffsNeed = new [] { ObjectStatus.FourfoldFanDance },
            },

            //����
            SaberDance = new (16005)
            {
                OtherCheck = b => JobGauge.Esprit >= 50,
            },

            //������
            StarfallDance = new (25792)
            {
                BuffsNeed = new [] { ObjectStatus.FlourishingStarfall },
            },

            //ǰ�岽
            EnAvant = new (16010, shouldEndSpecial: true),

            //Ǿޱ���Ų�
            Emboite = new (15999)
            {
                OtherCheck = b => JobGauge.NextStep == 15999,
            },

            //С�񽻵���
            Entrechat = new (16000)
            {
                OtherCheck = b => JobGauge.NextStep == 16000,
            },

            //��ҶС����
            Jete = new (16001)
            {
                OtherCheck = b => JobGauge.NextStep == 16001,
            },

            //���ֺ��ת
            Pirouette = new (16002)
            {
                OtherCheck = b => JobGauge.NextStep == 16002,
            },

            //��׼�貽
            StandardStep = new (15997)
            {
                BuffsProvide = new []
                {
                    ObjectStatus.StandardStep,
                    ObjectStatus.TechnicalStep,
                },
            },

            //�����貽
            TechnicalStep = new (15998)
            {
                BuffsNeed = new []
                {
                    ObjectStatus.StandardFinish,
                },
                BuffsProvide = new []
                {
                    ObjectStatus.StandardStep,
                    ObjectStatus.TechnicalStep,
                },
            },

            //����֮ɣ��
            ShieldSamba = new (16012, true)
            {
                BuffsProvide = new []
                {
                    ObjectStatus.Troubadour,
                    ObjectStatus.Tactician1,
                    ObjectStatus.Tactician2,
                    ObjectStatus.ShieldSamba,
                },
            },

            //����֮������
            CuringWaltz = new (16015, true),

            //��ʽ����
            ClosedPosition = new (16006, true)
            {
                ChoiceTarget = Targets =>
                {
                    Targets = Targets.Where(b => b.ObjectId != Service.ClientState.LocalPlayer.ObjectId && b.CurrentHp != 0 &&
                    //Remove Weak
                    b.StatusList.Select(status => status.StatusId).Intersect(new uint[] { ObjectStatus.Weakness, ObjectStatus.BrinkofDeath }).Count() == 0 &&
                    //Remove other partner.
                    b.StatusList.Where(s => s.StatusId == ObjectStatus.ClosedPosition2 && s.SourceID != Service.ClientState.LocalPlayer.ObjectId).Count() == 0).ToArray();

                    var targets = TargetFilter.GetJobCategory(Targets, Role.��ս);
                    if (targets.Length > 0) return targets[0];

                    targets = TargetFilter.GetJobCategory(Targets, Role.Զ��);
                    if (targets.Length > 0) return targets[0];

                    targets = Targets;
                    if (targets.Length > 0) return targets[0];

                    return null;
                },
            },

            //����֮̽��
            Devilment = new (16011, true),

            //�ٻ�����
            Flourish = new (16013)
            {
                BuffsNeed = new [] { ObjectStatus.StandardFinish },
                BuffsProvide = new []
                {
                    ObjectStatus.ThreefoldFanDance,
                    ObjectStatus.FourfoldFanDance,
                },
                OtherCheck = b => TargetHelper.InBattle,
            },

            //���˱���
            Improvisation = new (16014, true),

            //������
            Tillana = new (25790)
            {
                BuffsNeed = new [] { ObjectStatus.FlourishingFinish },
            };
    }

    internal override SortedList<DescType, string> Description => new ()
    {
        {DescType.��Χ����, $"{Actions.ShieldSamba.Action.Name}"},
        {DescType.��Χ����, $"{Actions.CuringWaltz.Action.Name}, {Actions.Improvisation.Action.Name}"},
        {DescType.�ƶ�, $"{Actions.EnAvant.Action.Name}"},
    };

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.ClosedPosition1))
        {
            foreach (var friend in TargetHelper.PartyMembers)
            {
                if (StatusHelper.FindStatusFromSelf(friend, ObjectStatus.ClosedPosition2)?.Length > 0)
                {
                    if (Actions.ClosedPosition.ShouldUseAction(out act) && Actions.ClosedPosition.Target != friend)
                    {
                        return true;
                    }
                    break;
                }
            }
        }
        else if (Actions.ClosedPosition.ShouldUseAction(out act)) return true;

        //���Ա���
        if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.TechnicalFinish)
        && Actions.Devilment.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;

        //�ٻ�
        if (Actions.Flourish.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;

        //���衤��
        if (Actions.FanDance4.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.FanDance3.ShouldUseAction(out act, mustUse: true)) return true;

        //����
        if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Devilment) || JobGauge.Feathers > 3 || Service.ClientState.LocalPlayer.Level < 70)
        {
            if (Actions.FanDance2.ShouldUseAction(out act)) return true;
            if (Actions.FanDance.ShouldUseAction(out act)) return true;
        }

        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.EnAvant.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.CuringWaltz.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
        if (Actions.Improvisation.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.ShieldSamba.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        if (!TargetHelper.InBattle && !StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.ClosedPosition1) && Actions.ClosedPosition.ShouldUseAction(out act)) return true;

        if (SettingBreak)
        {
            if (Actions.TechnicalStep.ShouldUseAction(out act)) return true;
        }

        if (StepGCD(out act)) return true;
        if (AttackGCD(out act, StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Devilment), lastComboActionID)) return true;

        return false;
    }
    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        if (Service.ClientState.LocalPlayer.Level < Actions.TechnicalStep.Level
            && Actions.Devilment.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;

        return base.BreakAbility(abilityRemain, out act);
    }

    private bool StepGCD(out IAction act)
    {
        act = null;
        if (!StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.StandardStep, ObjectStatus.TechnicalStep)) return false;

        if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.StandardStep) && JobGauge.CompletedSteps == 2)
        {
            act = Actions.StandardStep;
            return true;
        }
        else if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.TechnicalStep) && JobGauge.CompletedSteps == 4)
        {
            act = Actions.TechnicalStep;
            return true;
        }
        else
        {
            if (Actions.Emboite.ShouldUseAction(out act)) return true;
            if (Actions.Entrechat.ShouldUseAction(out act)) return true;
            if (Actions.Jete.ShouldUseAction(out act)) return true;
            if (Actions.Pirouette.ShouldUseAction(out act)) return true;
        }

        return false;
    }

    private bool AttackGCD(out IAction act, bool breaking, uint lastComboActionID)
    {
        //����
        if ((breaking || JobGauge.Esprit >= 75) &&
            Actions.SaberDance.ShouldUseAction(out act, mustUse: true)) return true;

        //������
        if (Actions.Tillana.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.StarfallDance.ShouldUseAction(out act, mustUse: true)) return true;

        if (JobGauge.IsDancing) return false;

        bool canstandard = Actions.TechnicalStep.RecastTimeRemain == 0 || Actions.TechnicalStep.RecastTimeRemain > 5;

        if (!StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.TechnicalFinish))
        {
            //��׼�貽
            if (canstandard && Actions.StandardStep.ShouldUseAction(out act)) return true;
        }

        //�õ�Buff
        if (Actions.Bloodshower.ShouldUseAction(out act)) return true;
        if (Actions.Fountainfall.ShouldUseAction(out act)) return true;

        if (Actions.RisingWindmill.ShouldUseAction(out act)) return true;
        if (Actions.ReverseCascade.ShouldUseAction(out act)) return true;


        //��׼�貽
        if (canstandard && Actions.StandardStep.ShouldUseAction(out act)) return true;


        //aoe
        if (Actions.Bladeshower.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.Windmill.ShouldUseAction(out act)) return true;

        //single
        if (Actions.Fountain.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.Cascade.ShouldUseAction(out act)) return true;

        return false;
    }
}
