using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.Basic;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.RangedPhysicial.DNCCombos.DNCCombo_Default;

namespace XIVAutoAttack.Combos.RangedPhysicial.DNCCombos;

internal sealed class DNCCombo_Default : DNCCombo_Base<CommandType>
{
    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //д��ע�Ͱ���������ʾ�û��ġ�
    };

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetFloat("Technical_over", 3, "������ȴ��֮ǰ����GCD��ʼ���ӹ����ܻ���,����", min: 0, max: 5, speed: 0.02f);
    }

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.��Χ����, $"{ShieldSamba}"},
        {DescType.��Χ����, $"{CuringWaltz}, {Improvisation}"},
        {DescType.�ƶ�����, $"{EnAvant}"},
    };

    public override string Author => "��ˮ";

    //���贰��
    private static bool _TechnicalFinish => Player.HasStatus(true, StatusID.TechnicalFinish);
    private float Technical_over => Config.GetFloatByName("Technical_over");


    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        //Ӧ������� ����鼼���Ȳ���
        if (Player.HasStatus(true, StatusID.ClosedPosition1) &&
            !Player.HasStatus(true, StatusID.StandardFinish) &&
            !Player.HasStatus(true, StatusID.Devilment))
        {
            foreach (var friend in TargetUpdater.PartyMembers)
            {
                if (friend.HasStatus(true, StatusID.ClosedPosition2))
                {
                    if (ClosedPosition.ShouldUse(out act) && ClosedPosition.Target != friend)
                    {
                        return true;
                    }
                    break;
                }
            }
        }
        else if (ClosedPosition.ShouldUse(out act)) return true;

        //���Դ�����̽��
        if (Player.HasStatus(true, StatusID.TechnicalFinish)
        && Devilment.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //���衤��
        if (FanDance3.ShouldUse(out act, mustUse: true)) return true;

        //�ٻ�
        if (Flourish.ShouldUse(out act, emptyOrSkipCombo: true)) return true;


        //���� ���������һ��
        if (!TechnicalStep.WillHaveOneChargeGCD((uint)Technical_over) && (Player.HasStatus(true, StatusID.Devilment) || Feathers > 3 || !TechnicalStep.EnoughLevel))
        {
            if (FanDance2.ShouldUse(out act)) return true;
            if (FanDance.ShouldUse(out act)) return true;
        }
        if (FanDance4.ShouldUse(out act, mustUse: true)) return true; //ʱ�䳤�����Ӻ� ���ȴ���������

        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (EnAvant.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        if (CuringWaltz.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        if (Improvisation.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        if (ShieldSamba.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //�����
        if (!InCombat && !Player.HasStatus(true, StatusID.ClosedPosition1)
            && ClosedPosition.ShouldUse(out act)) return true;
        //����
        if (FinishStepGCD(out act)) return true;

        //��ʼ����
        if (StandardStep.ShouldUse(out act, mustUse: true)) return true;
        if (SettingBreak && TechnicalStep.ShouldUse(out act, mustUse: true)) return true;
        #region ���豬��
        if (SettingBreak && Player.HasStatus(true, StatusID.TechnicalFinish))
        {
            //������
            if (Tillana.ShouldUse(out act, mustUse: true)) return true;
            //����
            if (SaberDance.ShouldUse(out act, mustUse: true)) return true;
            //������
            if (StarfallDance.ShouldUse(out act, mustUse: true)) return true;
        }
        #endregion
        #region ��ͨ����Դ
        #region �����߼�
        if (!TechnicalStep.WillHaveOneChargeGCD((uint)Technical_over) && Esprit >= 85 && SaberDance.ShouldUse(out act, mustUse: true)) return true;
        #endregion


        if (Bloodshower.ShouldUse(out act)) return true;
        if (Fountainfall.ShouldUse(out act)) return true;

        if (RisingWindmill.ShouldUse(out act)) return true;
        if (ReverseCascade.ShouldUse(out act)) return true;

        //aoe
        if (Bladeshower.ShouldUse(out act)) return true;
        if (Windmill.ShouldUse(out act)) return true;
        //single
        if (Fountain.ShouldUse(out act)) return true;
        if (Cascade.ShouldUse(out act)) return true;
        #endregion

        return false;
    }

    //����15sʹ��С��
    private protected override IAction CountDownAction(float remainTime)
    {
        IAction act = null;
        if (remainTime <= 15)
        {
            if (StandardStep.ShouldUse(out _)) return StandardStep;
            if (Player.HasStatus(true, StatusID.StandardStep) && CompletedSteps < 2)
            {
                if (excutionStepGCD(out act)) return act;
            }
        }
        return base.CountDownAction(remainTime);
    }
}
