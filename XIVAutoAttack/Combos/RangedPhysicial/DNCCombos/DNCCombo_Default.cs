using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Attributes;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.RangedPhysicial.DNCCombos.DNCCombo_Default;

namespace XIVAutoAttack.Combos.RangedPhysicial.DNCCombos;

[ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/XIVAutoAttack/Combos/RangedPhysicial/DNCCombos/DNCCombo_Default.cs")]
internal sealed class DNCCombo_Default : DNCCombo<CommandType>
{
    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //д��ע�Ͱ���������ʾ�û��ġ�
    };

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.��Χ����, $"{ShieldSamba}"},
        {DescType.��Χ����, $"{CuringWaltz}, {Improvisation}"},
        {DescType.�ƶ�����, $"{EnAvant}"},
    };

    public override string Author => "��ˮ";

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak && !TechnicalStep.EnoughLevel
    && Devilment.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //Ӧ�������
        if (Player.HaveStatus(StatusIDs.ClosedPosition1))
        {
            foreach (var friend in TargetUpdater.PartyMembers)
            {
                if (friend.HaveStatus(StatusIDs.ClosedPosition2))
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

        //���Ա���
        if (Player.HaveStatus(StatusIDs.TechnicalFinish)
        && Devilment.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //�ٻ�
        if (Flourish.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //���衤��
        if (FanDance4.ShouldUse(out act, mustUse: true)) return true;
        if (FanDance3.ShouldUse(out act, mustUse: true)) return true;

        //����
        if (Player.HaveStatus(StatusIDs.Devilment) || JobGauge.Feathers > 3 || !TechnicalStep.EnoughLevel)
        {
            if (FanDance2.ShouldUse(out act)) return true;
            if (FanDance.ShouldUse(out act)) return true;
        }

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
        if (!InCombat && !Player.HaveStatus(StatusIDs.ClosedPosition1)
            && ClosedPosition.ShouldUse(out act)) return true;

        if (StepGCD(out act)) return true;

        if (SettingBreak)
        {
            if (TechnicalStep.ShouldUse(out act, mustUse: true)) return true;
        }

        if (AttackGCD(out act, Player.HaveStatus(StatusIDs.Devilment))) return true;

        return false;
    }

    private bool StepGCD(out IAction act)
    {
        act = null;
        if (!Player.HaveStatus(StatusIDs.StandardStep, StatusIDs.TechnicalStep)) return false;

        if (Player.HaveStatus(StatusIDs.StandardStep) && JobGauge.CompletedSteps == 2)
        {
            act = StandardStep;
            return true;
        }
        else if (Player.HaveStatus(StatusIDs.TechnicalStep) && JobGauge.CompletedSteps == 4)
        {
            act = TechnicalStep;
            return true;
        }
        else
        {
            if (Emboite.ShouldUse(out act)) return true;
            if (Entrechat.ShouldUse(out act)) return true;
            if (Jete.ShouldUse(out act)) return true;
            if (Pirouette.ShouldUse(out act)) return true;
        }

        return false;
    }

    private bool AttackGCD(out IAction act, bool breaking)
    {

        //����
        if ((breaking || JobGauge.Esprit >= 80) &&
            SaberDance.ShouldUse(out act, mustUse: true)) return true;

        //������
        if (Tillana.ShouldUse(out act, mustUse: true)) return true;
        if (StarfallDance.ShouldUse(out act, mustUse: true)) return true;

        if (JobGauge.IsDancing) return false;

        bool canstandard = !TechnicalStep.WillHaveOneChargeGCD(2);

        if (!Player.HaveStatus(StatusIDs.TechnicalFinish))
        {
            //��׼�貽
            if (StandardStep.ShouldUse(out act, mustUse: true)) return true;
        }

        //�õ�Buff
        if (Bloodshower.ShouldUse(out act)) return true;
        if (Fountainfall.ShouldUse(out act)) return true;

        if (RisingWindmill.ShouldUse(out act)) return true;
        if (ReverseCascade.ShouldUse(out act)) return true;


        //��׼�貽
        if (canstandard && StandardStep.ShouldUse(out act, mustUse: true)) return true;


        //aoe
        if (Bladeshower.ShouldUse(out act)) return true;
        if (Windmill.ShouldUse(out act)) return true;

        //single
        if (Fountain.ShouldUse(out act)) return true;
        if (Cascade.ShouldUse(out act)) return true;

        return false;
    }
}
