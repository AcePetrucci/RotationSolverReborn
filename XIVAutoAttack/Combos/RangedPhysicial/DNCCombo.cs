using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboPlus.Combos;

internal class DNCCombo : CustomComboJob<DNCGauge>
{
    internal override uint JobID => 38;

    internal struct Actions
    {
        public static readonly BaseAction

            //��к
            Cascade = new BaseAction(15989)
            {
                BuffsProvide = new ushort[] { ObjectStatus.SilkenSymmetry }
            },

            //��Ȫ
            Fountain = new BaseAction(15990)
            {
                BuffsProvide = new ushort[] { ObjectStatus.SilkenFlow }
            },

            //����к
            ReverseCascade = new BaseAction(15991)
            {
                BuffsNeed = new ushort[] { ObjectStatus.SilkenSymmetry },
            },

            //׹��Ȫ
            Fountainfall = new BaseAction(15992)
            {
                BuffsNeed = new ushort[] { ObjectStatus.SilkenFlow }
            },

            //���衤��
            FanDance = new BaseAction(16007)
            {
                OtherCheck = b => JobGauge.Feathers > 0,
                BuffsProvide = new ushort[] { ObjectStatus.ThreefoldFanDance },
            },

            //�糵
            Windmill = new BaseAction(15993)
            {
                BuffsProvide = new ushort[] { ObjectStatus.SilkenSymmetry }
            },

            //������
            Bladeshower = new BaseAction(15994)
            {
                BuffsProvide = new ushort[] { ObjectStatus.SilkenFlow }
            },

            //���糵
            RisingWindmill = new BaseAction(15995)
            {
                BuffsNeed = new ushort[] { ObjectStatus.SilkenSymmetry },
            },

            //��Ѫ��
            Bloodshower = new BaseAction(15996)
            {
                BuffsNeed = new ushort[] { ObjectStatus.SilkenFlow }
            },

            //���衤��
            FanDance2 = new BaseAction(16008)
            {
                OtherCheck = b => JobGauge.Feathers > 0,
                BuffsProvide = new ushort[] { ObjectStatus.ThreefoldFanDance },
            },

            //���衤��
            FanDance3 = new BaseAction(16009)
            {
                BuffsNeed = new ushort[] { ObjectStatus.ThreefoldFanDance },
            },

            //���衤��
            FanDance4 = new BaseAction(25791)
            {
                BuffsNeed = new ushort[] { ObjectStatus.FourfoldFanDance },
            },

            //����
            SaberDance = new BaseAction(16005)
            {
                OtherCheck = b => JobGauge.Esprit >= 50,
            },

            //������
            StarfallDance = new BaseAction(25792)
            {
                BuffsNeed = new ushort[] { ObjectStatus.FlourishingStarfall },
            },

            //ǰ�岽
            EnAvant = new BaseAction(16010, shouldEndSpecial: true),

            //Ǿޱ���Ų�
            Emboite = new BaseAction(15999)
            {
                OtherCheck = b => JobGauge.NextStep == 15999,
            },

            //С�񽻵���
            Entrechat = new BaseAction(16000)
            {
                OtherCheck = b => JobGauge.NextStep == 16000,
            },

            //��ҶС����
            Jete = new BaseAction(16001)
            {
                OtherCheck = b => JobGauge.NextStep == 16001,
            },

            //���ֺ��ת
            Pirouette = new BaseAction(16002)
            {
                OtherCheck = b => JobGauge.NextStep == 16002,
            },

            //��׼�貽
            StandardStep = new BaseAction(15997)
            {
                BuffsProvide = new ushort[]
                {
                    ObjectStatus.StandardStep,
                    ObjectStatus.TechnicalStep,
                },
            },

            //�����貽
            TechnicalStep = new BaseAction(15998)
            {
                BuffsNeed = new ushort[]
                {
                    ObjectStatus.StandardFinish,
                },
                BuffsProvide = new ushort[]
                {
                    ObjectStatus.StandardStep,
                    ObjectStatus.TechnicalStep,
                },
            },

            //����֮ɣ��
            ShieldSamba = new BaseAction(16012, true)
            {
                BuffsProvide = new ushort[]
                {
                    ObjectStatus.Troubadour,
                    ObjectStatus.Tactician1,
                    ObjectStatus.Tactician2,
                    ObjectStatus.ShieldSamba,
                },
            },

            //����֮������
            CuringWaltz = new BaseAction(16015, true),

            //��ʽ����
            ClosedPosition = new BaseAction(16006, true)
            {
                ChoiceFriend = Targets =>
                {
                    var targets = TargetHelper.GetJobCategory(Targets, Role.��ս);
                    if (targets.Length > 0) return ASTCombo.RandomObject(targets);

                    targets = TargetHelper.GetJobCategory(Targets, Role.Զ��);
                    if (targets.Length > 0) return ASTCombo.RandomObject(targets);

                    targets = Targets;
                    if (targets.Length > 0) return ASTCombo.RandomObject(targets);

                    return null;
                },
                BuffsProvide = new ushort[]
                {
                    ObjectStatus.ClosedPosition1,
                    ObjectStatus.ClosedPosition2,
                },
            },

            //����֮̽��
            Devilment = new BaseAction(16011, true)
            {
                BuffsNeed = new ushort[] { ObjectStatus.TechnicalFinish },
            },

            //�ٻ�����
            Flourish = new BaseAction(16013)
            {
                BuffsNeed = new ushort[] { ObjectStatus.StandardFinish },
                BuffsProvide = new ushort[]
                {
                    ObjectStatus.SilkenSymmetry,
                    ObjectStatus.SilkenFlow,
                    ObjectStatus.ThreefoldFanDance,
                    ObjectStatus.FourfoldFanDance,
                }
            },

            //���˱���
            Improvisation = new BaseAction(16014, true),

            //������
            Tillana = new BaseAction(25790)
            {
                BuffsNeed = new ushort[] { ObjectStatus.FlourishingFinish },
            },

            //��׼�貽����
            StandardFinish = new BaseAction(16003),

            //�����貽����
            TechnicalFinish = new BaseAction(16004);
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out BaseAction act)
    {
        //���Ա���
        if (Actions.Devilment.ShouldUseAction(out act, Empty: true)) return true;

        //�ٻ�
        if (Actions.Flourish.ShouldUseAction(out act, Empty: true)) return true;

        //���衤��
        if (Actions.FanDance4.ShouldUseAction(out act)) return true;
        if (Actions.FanDance3.ShouldUseAction(out act)) return true;

        //����
        if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Devilment) || JobGauge.Feathers > 3)
        {
            if (Actions.FanDance2.ShouldUseAction(out act)) return true;
            if (Actions.FanDance.ShouldUseAction(out act)) return true;
        }

        //����
        if (GeneralActions.FootGraze.ShouldUseAction(out act)) return true;

        //����
        if (GeneralActions.LegGraze.ShouldUseAction(out act)) return true;

        //�ڵ�
        if (GeneralActions.SecondWind.ShouldUseAction(out act)) return true;


        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.EnAvant.ShouldUseAction(out act, Empty: true)) return true;
        return false;
    }

    private protected override bool HealAreaAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.CuringWaltz.ShouldUseAction(out act, Empty: true)) return true;
        if (Actions.Improvisation.ShouldUseAction(out act, Empty: true)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.ShieldSamba.ShouldUseAction(out act, Empty: true)) return true;
        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out BaseAction act)
    {
        if (SettingBreak)
        {
            if (Actions.TechnicalStep.ShouldUseAction(out act)) return true;
        }

        if (StepGCD(out act)) return true;
        if (AttackGCD(out act, BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Devilment), lastComboActionID)) return true;

        return false;
    }

    private bool StepGCD(out BaseAction act)
    {
        act = null;
        if (!BaseAction.HaveStatusSelfFromSelf(ObjectStatus.StandardStep, ObjectStatus.TechnicalStep)) return false;

        if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.StandardStep) && JobGauge.CompletedSteps == 2)
        {
            if (Actions.StandardFinish.ShouldUseAction(out act, mustUse:true)) return true;
        }
        else if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.TechnicalStep) && JobGauge.CompletedSteps == 4)
        {
            if (Actions.TechnicalFinish.ShouldUseAction(out act, mustUse: true)) return true;
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

    private protected override bool GeneralAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.ClosedPosition.ShouldUseAction(out act)) return true;
        return false;
    }

    private bool AttackGCD(out BaseAction act, bool breaking, uint lastComboActionID)
    {
        //������
        if (Actions.Tillana.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.StarfallDance.ShouldUseAction(out act)) return true;

        //�õ�Buff
        if (Actions.Bloodshower.ShouldUseAction(out act)) return true;
        if (Actions.Fountainfall.ShouldUseAction(out act)) return true;

        if (Actions.RisingWindmill.ShouldUseAction(out act)) return true;
        if (Actions.ReverseCascade.ShouldUseAction(out act)) return true;

        //����
        if ((breaking || JobGauge.Esprit >= 75) &&
            Actions.SaberDance.ShouldUseAction(out act)) return true;

        //��׼�貽
        if (Actions.StandardStep.ShouldUseAction(out act)) return true;

        //aoe
        if (Actions.Bladeshower.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.Windmill.ShouldUseAction(out act)) return true;

        //single
        if (Actions.Fountain.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.Cascade.ShouldUseAction(out act)) return true;

        return false;
    }

    //public static class Buffs
    //{
    //    public const ushort FlourishingSymmetry = 2693;

    //    public const ushort FlourishingFlow = 2694;

    //    public const ushort FlourishingFinish = 2698;

    //    public const ushort FlourishingStarfall = 2700;

    //    public const ushort StandardStep = 1818;

    //    public const ushort TechnicalStep = 1819;

    //    public const ushort ThreefoldFanDance = 1820;

    //    public const ushort FourfoldFanDance = 2699;
    //}

}
