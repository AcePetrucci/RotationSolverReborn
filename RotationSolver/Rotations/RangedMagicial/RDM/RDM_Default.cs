using RotationSolver.Actions;
using RotationSolver.Configuration.RotationConfig;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.Basic;
using RotationSolver.Rotations.CustomRotation;
using System.Collections.Generic;

namespace RotationSolver.Rotations.RangedMagicial.RDM;

internal sealed class RDM_Default : RDM_Base
{
    public override string GameVersion => "6.0";

    public override string RotationName => "Default";

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.HealSingle, $"{Vercure}"},
        {DescType.DefenseArea, $"{MagickBarrier}"},
        {DescType.MoveAction, $"{CorpsAcorps}"},
    };

    static RDM_Default()
    {
        Acceleration.RotationCheck = b => InCombat;
    }

    private protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetBool("UseVercure", true, "Use Vercure for Dualcast");
    }

    private protected override bool EmergencyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        act = null;
        //����Ҫ�ŵ�ħ�ش̻���ħZն��ħ��Բն֮��
        if (nextGCD.IsTheSameTo(true, Zwerchhau, Redoublement, Moulinet))
        {
            if (Service.Configuration.AutoBurst && Embolden.CanUse(out act, mustUse: true)) return true;
        }
        //����������ʱ���ͷš�
        if (Service.Configuration.AutoBurst && GetRightValue(WhiteMana) && GetRightValue(BlackMana))
        {
            if (!canUseMagic(act) && Manafication.CanUse(out act)) return true;
            if (Embolden.CanUse(out act, mustUse: true)) return true;
        }
        //����Ҫ�ŵ�ħ������֮��
        if (ManaStacks == 3 || Level < 68 && !nextGCD.IsTheSameTo(true, Zwerchhau, Riposte))
        {
            if (!canUseMagic(act) && Manafication.CanUse(out act)) return true;
        }

        act = null;
        return false;
    }

    private bool GetRightValue(byte value)
    {
        return value >= 6 && value <= 12;
    }

    private protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        act = null;
        if (InBurst)
        {
            if (!canUseMagic(act) && Manafication.CanUse(out act)) return true;
            if (Embolden.CanUse(out act, mustUse: true)) return true;
        }

        if (ManaStacks == 0 && (BlackMana < 50 || WhiteMana < 50) && !Manafication.WillHaveOneChargeGCD(1, 1))
        {
            //�ٽ�������Ԥ��buffû�����á� 
            if (abilitiesRemaining == 2 && Acceleration.CanUse(out act, emptyOrSkipCombo: true)
                && (!Player.HasStatus(true, StatusID.VerfireReady) || !Player.HasStatus(true, StatusID.VerstoneReady))) return true;

            //����ӽ��
            if (!Player.HasStatus(true, StatusID.Acceleration)
                && Swiftcast.CanUse(out act, mustUse: true)
                && (!Player.HasStatus(true, StatusID.VerfireReady) || !Player.HasStatus(true, StatusID.VerstoneReady))) return true;
        }

        //�����ĸ���������
        if (ContreSixte.CanUse(out act, mustUse: true)) return true;
        if (Fleche.CanUse(out act)) return true;
        //Empty: BaseAction.HaveStatusSelfFromSelf(1239)
        if (Engagement.CanUse(out act, emptyOrSkipCombo: true)) return true;

        if (CorpsAcorps.CanUse(out act, mustUse: true) && !IsMoving) return true;

        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        act = null;
        if (ManaStacks == 3) return false;

        #region �������
        if (!Verthunder2.CanUse(out _))
        {
            if (Verfire.CanUse(out act)) return true;
            if (Verstone.CanUse(out act)) return true;
        }

        //���Կ�ɢ��
        if (Scatter.CanUse(out act)) return true;
        //ƽ��ħԪ
        if (WhiteMana < BlackMana)
        {
            if (Veraero2.CanUse(out act)) return true;
            if (Veraero.CanUse(out act)) return true;
        }
        else
        {
            if (Verthunder2.CanUse(out act)) return true;
            if (Verthunder.CanUse(out act)) return true;
        }
        if (Jolt.CanUse(out act)) return true;
        #endregion

        //��ˢ���׺ͷ�ʯ


        //�����ƣ��Ӽ��̣�������ӽ�����߼��̵Ļ��Ͳ�����
        if (Configs.GetBool("UseVercure") && Vercure.CanUse(out act)
            ) return true;

        return false;
    }


    private protected override bool DefenceAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        //����
        if (Addle.CanUse(out act)) return true;
        if (MagickBarrier.CanUse(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool EmergencyGCD(out IAction act)
    {
        byte level = Level;
        #region Զ������
        //���ħԪ�ᾧ���ˡ�
        if (ManaStacks == 3)
        {
            if (BlackMana > WhiteMana && level >= 70)
            {
                if (Verholy.CanUse(out act, mustUse: true)) return true;
            }
            if (Verflare.CanUse(out act, mustUse: true)) return true;
        }

        //����
        if (Scorch.CanUse(out act, mustUse: true)) return true;

        //����
        if (Resolution.CanUse(out act, mustUse: true)) return true;
        #endregion

        #region ��ս����


        if (IsLastGCD(true, Moulinet) && Moulinet.CanUse(out act, mustUse: true)) return true;
        if (Zwerchhau.CanUse(out act)) return true;
        if (Redoublement.CanUse(out act)) return true;

        //����������ˣ�����ħԪ���ˣ��������ڱ��������ߴ��ڿ�������״̬���������ã�
        bool mustStart = Player.HasStatus(true, StatusID.Manafication) ||
                         BlackMana == 100 || WhiteMana == 100 || !Embolden.IsCoolingDown;

        //��ħ��Ԫû�����������£�Ҫ���С��ħԪ����������Ҳ����ǿ��Ҫ�������жϡ�
        if (!mustStart)
        {
            if (BlackMana == WhiteMana) return false;

            //Ҫ���С��ħԪ����������Ҳ����ǿ��Ҫ�������жϡ�
            if (WhiteMana < BlackMana)
            {
                if (Player.HasStatus(true, StatusID.VerstoneReady))
                {
                    return false;
                }
            }
            if (WhiteMana > BlackMana)
            {
                if (Player.HasStatus(true, StatusID.VerfireReady))
                {
                    return false;
                }
            }

            //������û�м�����صļ��ܡ�
            if (Player.HasStatus(true, Vercure.StatusProvide))
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

        if (Player.HasStatus(true, StatusID.Dualcast)) return false;

        #region ��������
        //Ҫ������ʹ�ý�ս�����ˡ�
        if (Moulinet.CanUse(out act))
        {
            if (BlackMana >= 60 && WhiteMana >= 60) return true;
        }
        else
        {
            if (BlackMana >= 50 && WhiteMana >= 50 && Riposte.CanUse(out act)) return true;
        }
        if (ManaStacks > 0 && Riposte.CanUse(out act)) return true;
        #endregion

        return false;
    }

    //�ж����Ⱦ����ܲ���ʹ��
    private bool canUseMagic(IAction act)
    {
        //return IsLastAction(true, Scorch) || IsLastAction(true, Resolution) || IsLastAction(true, Verholy) || IsLastAction(true, Verflare);
        return Scorch.CanUse(out act) || Resolution.CanUse(out act);
    }
}

