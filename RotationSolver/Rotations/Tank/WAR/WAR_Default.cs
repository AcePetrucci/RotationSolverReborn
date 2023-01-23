using RotationSolver.Actions;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Rotations.Basic;
using RotationSolver.Rotations.CustomRotation;
using RotationSolver.Updaters;
using System.Collections.Generic;
using System.Linq;

namespace RotationSolver.Rotations.Tank.WAR;

internal sealed class WAR_Default : WAR_Base
{
    public override string GameVersion => "6.0";

    public override string RotationName => "Default";

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.DefenseArea, $"{ShakeItOff}"},
        {DescType.DefenseSingle, $"{RawIntuition}, {Vengeance}"},
        {DescType.MoveAction, $"GCD: {PrimalRend}\n{Onslaught}"},
    };

    static WAR_Default()
    {
        InnerBeast.RotationCheck = b => !Player.WillStatusEndGCD(3, 0, true, StatusID.SurgingTempest);
    }

    private protected override bool DefenceAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        //���� �����׶�
        if (ShakeItOff.CanUse(out act, mustUse: true)) return true;

        if (Reprisal.CanUse(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool MoveGCD(out IAction act)
    {
        //�Ÿ��� ���ı��� ����ǰ��
        if (PrimalRend.CanUse(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //��㹥��
        if (PrimalRend.CanUse(out act, mustUse: true) && !IsMoving)
        {
            if (PrimalRend.Target.DistanceToPlayer() < 1)
            {
                return true;
            }
        }

        //�޻����
        //��������
        if (SteelCyclone.CanUse(out act)) return true;
        //ԭ��֮��
        if (InnerBeast.CanUse(out act)) return true;

        //Ⱥ��
        if (MythrilTempest.CanUse(out act)) return true;
        if (Overpower.CanUse(out act)) return true;

        //����
        if (StormsEye.CanUse(out act)) return true;
        if (StormsPath.CanUse(out act)) return true;
        if (Maim.CanUse(out act)) return true;
        if (HeavySwing.CanUse(out act)) return true;

        //�����ţ�����һ���ɡ�
        if (RSCommands.SpecialType == SpecialCommandType.MoveForward && MoveForwardAbility(1, out act)) return true;
        if (Tomahawk.CanUse(out act)) return true;

        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        if (abilitiesRemaining == 2)
        {
            if (TargetUpdater.HostileTargets.Count() > 1)
            {
                //ԭ����ֱ��������10%��
                if (RawIntuition.CanUse(out act)) return true;
            }

            //���𣨼���30%��
            if (Vengeance.CanUse(out act)) return true;

            //���ڣ�����20%��
            if (Rampart.CanUse(out act)) return true;

            //ԭ����ֱ��������10%��
            if (RawIntuition.CanUse(out act)) return true;
        }
        //���͹���
        //ѩ��
        if (Reprisal.CanUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        //����
        if (!Player.WillStatusEndGCD(3, 0, true, StatusID.SurgingTempest) || !MythrilTempest.EnoughLevel)
        {
            //��
            if (!InnerRelease.IsCoolingDown && Berserk.CanUse(out act)) return true;
        }

        if (Player.GetHealthRatio() < 0.6f)
        {
            //ս��
            if (ThrillofBattle.CanUse(out act)) return true;
            //̩Ȼ���� ���̰���
            if (Equilibrium.CanUse(out act)) return true;
        }

        //�̸����Ѱ���
        if (!HasShield && NascentFlash.CanUse(out act)) return true;

        //ս��
        if (Infuriate.CanUse(out act, emptyOrSkipCombo: true)) return true;

        //��ͨ����
        //Ⱥɽ¡��
        if (Orogeny.CanUse(out act)) return true;
        //���� 
        if (Upheaval.CanUse(out act)) return true;

        //��㹥��
        if (Onslaught.CanUse(out act, mustUse: true) && !IsMoving) return true;

        return false;
    }
}
