using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Attributes;
using XIVAutoAttack.Combos.Basic;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.RangedMagicial.RDMCombos.RDMCombo_Default;

namespace XIVAutoAttack.Combos.RangedMagicial.RDMCombos;

[ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/XIVAutoAttack/Combos/RangedMagicial/RDMCombos/RDMCombo_Default.cs")]
internal sealed class RDMCombo_Default : RDMCombo_Base<CommandType>
{
    public override string Author => "��ˮ";

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
        {DescType.��������, $"{Vercure}"},
        {DescType.��Χ����, $"{MagickBarrier}"},
        {DescType.�ƶ�����, $"{CorpsAcorps}"},
    };

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetBool("UseVercure", true, "ʹ�ó����ƻ�ü���");
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //����Ҫ�ŵ�ħ�ش̻���ħZն��ħ��Բն֮��
        if (nextGCD.IsAnySameAction(true, Zwerchhau, Redoublement, Moulinet))
        {
            if (Service.Configuration.AutoBreak && Embolden.ShouldUse(out act, mustUse: true)) return true;
        }
        //����������ʱ���ͷš�
        if (Service.Configuration.AutoBreak && GetRightValue(JobGauge.WhiteMana) && GetRightValue(JobGauge.BlackMana))
        {
            if (Manafication.ShouldUse(out act)) return true;
            if (Embolden.ShouldUse(out act, mustUse: true)) return true;
        }
        //����Ҫ�ŵ�ħ������֮��
        if (JobGauge.ManaStacks == 3 || Level < 68 && !nextGCD.IsAnySameAction(true, Zwerchhau, Riposte))
        {
            if (Manafication.ShouldUse(out act)) return true;
        }

        act = null;
        return false;
    }

    private bool GetRightValue(byte value)
    {
        return value >= 6 && value <= 12;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak)
        {
            if (Manafication.ShouldUse(out act)) return true;
            if (Embolden.ShouldUse(out act, mustUse: true)) return true;
        }

        if (JobGauge.ManaStacks == 0 && (JobGauge.BlackMana < 50 || JobGauge.WhiteMana < 50) && !Manafication.WillHaveOneChargeGCD(1, 1))
        {
            //�ٽ����˾��á� 
            if (abilityRemain == 2 && Acceleration.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

            //����ӽ��
            if (!Player.HaveStatus(true, StatusID.Acceleration)
                && Swiftcast.ShouldUse(out act, mustUse: true)) return true;
        }

        //�����ĸ���������
        if (ContreSixte.ShouldUse(out act, mustUse: true)) return true;
        if (Fleche.ShouldUse(out act)) return true;
        //Empty: BaseAction.HaveStatusSelfFromSelf(1239)
        if (Engagement.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        if (CorpsAcorps.ShouldUse(out act) && !IsMoving)
        {
            if (CorpsAcorps.Target.DistanceToPlayer() < 1)
            {
                return true;
            }
        }

        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        act = null;
        if (JobGauge.ManaStacks == 3) return false;

        #region �������
        if (!Verthunder2.ShouldUse(out _))
        {
            if (Verfire.ShouldUse(out act)) return true;
            if (Verstone.ShouldUse(out act)) return true;
        }

        //���Կ�ɢ��
        if (Scatter.ShouldUse(out act)) return true;
        //ƽ��ħԪ
        if (JobGauge.WhiteMana < JobGauge.BlackMana)
        {
            if (Veraero2.ShouldUse(out act)) return true;
            if (Veraero.ShouldUse(out act)) return true;
        }
        else
        {
            if (Verthunder2.ShouldUse(out act)) return true;
            if (Verthunder.ShouldUse(out act)) return true;
        }
        if (Jolt.ShouldUse(out act)) return true;
        #endregion

        //�����ƣ��Ӽ��̡�
        if (Config.GetBoolByName("UseVercure") && Vercure.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(out IAction act)
    {
        if (Vercure.ShouldUse(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (CorpsAcorps.ShouldUse(out act, mustUse: true)) return true;
        return false;
    }
    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //����
        if (Addle.ShouldUse(out act)) return true;
        if (MagickBarrier.ShouldUse(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool EmergercyGCD(out IAction act)
    {
        byte level = Level;
        #region Զ������
        //���ħԪ�ᾧ���ˡ�
        if (JobGauge.ManaStacks == 3)
        {
            if (JobGauge.BlackMana > JobGauge.WhiteMana && level >= 70)
            {
                if (Verholy.ShouldUse(out act, mustUse: true)) return true;
            }
            if (Verflare.ShouldUse(out act, mustUse: true)) return true;
        }

        //����
        if (Scorch.ShouldUse(out act, mustUse: true)) return true;

        //����
        if (Resolution.ShouldUse(out act, mustUse: true)) return true;
        #endregion

        #region ��ս����


        if (Moulinet.ShouldUse(out act)) return true;
        if (Zwerchhau.ShouldUse(out act)) return true;
        if (Redoublement.ShouldUse(out act)) return true;

        //����������ˣ�����ħԪ���ˣ��������ڱ��������ߴ��ڿ�������״̬���������ã�
        bool mustStart = /*Player.HaveStatus(1971)|| */ JobGauge.BlackMana == 100 || JobGauge.WhiteMana == 100 || !Embolden.IsCoolDown;

        //��ħ��Ԫû�����������£�Ҫ���С��ħԪ����������Ҳ����ǿ��Ҫ�������жϡ�
        if (!mustStart)
        {
            if (JobGauge.BlackMana == JobGauge.WhiteMana) return false;

            //Ҫ���С��ħԪ����������Ҳ����ǿ��Ҫ�������жϡ�
            if (JobGauge.WhiteMana < JobGauge.BlackMana)
            {
                if (Player.HaveStatus(true, StatusID.VerstoneReady))
                {
                    return false;
                }
            }
            if (JobGauge.WhiteMana > JobGauge.BlackMana)
            {
                if (Player.HaveStatus(true, StatusID.VerfireReady))
                {
                    return false;
                }
            }

            //������û�м�����صļ��ܡ�
            if (Player.HaveStatus(true, Vercure.BuffsProvide))
            {
                return false;
            }

            //���������ʱ��쵽�ˣ�������û�á�
            if (Embolden.WillHaveOneChargeGCD(10))
            {
                return false;
            }
        }
        #endregion

        #region ��������
        //Ҫ������ʹ�ý�ս�����ˡ�
        if (Moulinet.ShouldUse(out act))
        {
            if (JobGauge.BlackMana >= 60 && JobGauge.WhiteMana >= 60) return true;
        }
        else
        {
            if (JobGauge.BlackMana >= 50 && JobGauge.WhiteMana >= 50 && Riposte.ShouldUse(out act)) return true;
        }
        if (JobGauge.ManaStacks > 0 && Riposte.ShouldUse(out act)) return true;
        #endregion

        return false;
    }
}
