using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Actions;
using RotationSolver.Data;
using RotationSolver.Helpers;

namespace RotationSolver.Rotations.Basic;

internal abstract class SAM_Base : CustomRotation.CustomRotation
{
    private static SAMGauge JobGauge => Service.JobGauges.Get<SAMGauge>();

    /// <summary>
    /// ѩ��
    /// </summary>
    protected static bool HasSetsu => JobGauge.HasSetsu;

    /// <summary>
    /// ����
    /// </summary>
    protected static bool HasGetsu => JobGauge.HasGetsu;

    /// <summary>
    /// ����
    /// </summary>
    protected static bool HasKa => JobGauge.HasKa;

    /// <summary>
    /// ����
    /// </summary>
    protected static byte Kenki => JobGauge.Kenki;

    /// <summary>
    /// ��ѹ
    /// </summary>
    protected static byte MeditationStacks => JobGauge.MeditationStacks;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Samurai };

    /// <summary>
    /// ��������
    /// </summary>
    protected static byte SenCount => (byte)((HasGetsu ? 1 : 0) + (HasSetsu ? 1 : 0) + (HasKa ? 1 : 0));

    protected static bool HaveMoon => Player.HasStatus(true, StatusID.Fugetsu);
    protected static float MoonTime => Player.StatusTime(true, StatusID.Fugetsu);
    protected static bool HaveFlower => Player.HasStatus(true, StatusID.Fuka);
    protected static float FlowerTime => Player.StatusTime(true, StatusID.Fuka);

    #region ����
    /// <summary>
    /// �з�
    /// </summary>
    public static IBaseAction Hakaze { get; } = new BaseAction(ActionID.Hakaze);

    /// <summary>
    /// ���
    /// </summary>
    public static IBaseAction Jinpu { get; } = new BaseAction(ActionID.Jinpu);

    /// <summary>
    /// �¹�
    /// </summary>
    public static IBaseAction Gekko { get; } = new BaseAction(ActionID.Gekko);

    /// <summary>
    /// ʿ��
    /// </summary>
    public static IBaseAction Shifu { get; } = new BaseAction(ActionID.Shifu);

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction Kasha { get; } = new BaseAction(ActionID.Kasha);

    /// <summary>
    /// ѩ��
    /// </summary>
    public static IBaseAction Yukikaze { get; } = new BaseAction(ActionID.Yukikaze);

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction Shoha { get; } = new BaseAction(ActionID.Shoha)
    {
        ActionCheck = b => MeditationStacks == 3
    };
    #endregion

    #region AoE

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction Fuga { get; } = new BaseAction(ActionID.Fuga);

    /// <summary>
    /// ���
    /// </summary>
    public static IBaseAction Fuko { get; } = new BaseAction(ActionID.Fuko);

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction Mangetsu { get; } = new BaseAction(ActionID.Mangetsu)
    {
        OtherIDsCombo = new[]
        {
            ActionID.Fuga,ActionID.Fuko
        }
    };
    /// <summary>
    /// ӣ��
    /// </summary>
    public static IBaseAction Oka { get; } = new BaseAction(ActionID.Oka)
    {
        OtherIDsCombo = new[]
        {
            ActionID.Fuga,ActionID.Fuko
        }
    };

    /// <summary>
    /// ��������
    /// </summary>
    public static IBaseAction Shoha2 { get; } = new BaseAction(ActionID.Shoha2)
    {
        ActionCheck = b => MeditationStacks == 3
    };

    /// <summary>
    /// ����ն��
    /// </summary>
    public static IBaseAction OgiNamikiri { get; } = new BaseAction(ActionID.OgiNamikiri)
    {
        StatusNeed = new[] { StatusID.OgiNamikiriReady },
        ActionCheck = b => !IsMoving
    };

    /// <summary>
    /// �ط�ն��
    /// </summary>
    public static IBaseAction KaeshiNamikiri { get; } = new BaseAction(ActionID.KaeshiNamikiri)
    {
        ActionCheck = b => JobGauge.Kaeshi == Dalamud.Game.ClientState.JobGauge.Enums.Kaeshi.NAMIKIRI
    };
    #endregion

    #region �Ӻ���
    /// <summary>
    /// �˰���
    /// </summary>
    public static IBaseAction Higanbana { get; } = new BaseAction(ActionID.Higanbana, isEot: true)
    {
        ActionCheck = b => !IsMoving && SenCount == 1,
        TargetStatus = new[] { StatusID.Higanbana },
    };

    /// <summary>
    /// �����彣
    /// </summary>
    public static IBaseAction TenkaGoken { get; } = new BaseAction(ActionID.TenkaGoken)
    {
        ActionCheck = b => !IsMoving && SenCount == 2,
    };

    /// <summary>
    /// ����ѩ�»�
    /// </summary>
    public static IBaseAction MidareSetsugekka { get; } = new BaseAction(ActionID.MidareSetsugekka)
    {
        ActionCheck = b => !IsMoving && SenCount == 3,
    };

    /// <summary>
    /// ��ط�
    /// </summary>
    public static IBaseAction TsubameGaeshi { get; } = new BaseAction(ActionID.TsubameGaeshi);

    /// <summary>
    /// �ط��彣
    /// </summary>
    public static IBaseAction KaeshiGoken { get; } = new BaseAction(ActionID.KaeshiGoken)
    {
        ActionCheck = b => JobGauge.Kaeshi == Dalamud.Game.ClientState.JobGauge.Enums.Kaeshi.GOKEN
    };

    /// <summary>
    /// �ط�ѩ�»�
    /// </summary>
    public static IBaseAction KaeshiSetsugekka { get; } = new BaseAction(ActionID.KaeshiSetsugekka)
    {
        ActionCheck = b => JobGauge.Kaeshi == Dalamud.Game.ClientState.JobGauge.Enums.Kaeshi.SETSUGEKKA
    };
    #endregion

    #region ����
    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction ThirdEye { get; } = new BaseAction(ActionID.ThirdEye, true, isTimeline: true);

    /// <summary>
    /// ���
    /// </summary>
    public static IBaseAction Enpi { get; } = new BaseAction(ActionID.Enpi);

    /// <summary>
    /// ����ֹˮ
    /// </summary>
    public static IBaseAction MeikyoShisui { get; } = new BaseAction(ActionID.MeikyoShisui)
    {
        StatusProvide = new[] { StatusID.MeikyoShisui },
    };

    /// <summary>
    /// Ҷ��
    /// </summary>
    public static IBaseAction Hagakure { get; } = new BaseAction(ActionID.Hagakure)
    {
        ActionCheck = b => SenCount > 0
    };

    /// <summary>
    /// ��������
    /// </summary>
    public static IBaseAction Ikishoten { get; } = new BaseAction(ActionID.Ikishoten)
    {
        StatusProvide = new[] { StatusID.OgiNamikiriReady },
        ActionCheck = b => InCombat
    };
    #endregion

    #region ��ɱ��
    /// <summary>
    /// ��ɱ��������
    /// </summary>
    public static IBaseAction HissatsuShinten { get; } = new BaseAction(ActionID.HissatsuShinten)
    {
        ActionCheck = b => Kenki >= 25
    };

    /// <summary>
    /// ��ɱ��������
    /// </summary>
    public static IBaseAction HissatsuGyoten { get; } = new BaseAction(ActionID.HissatsuGyoten)
    {
        ActionCheck = b => Kenki >= 10 && !Player.HasStatus(true, StatusID.Bind1, StatusID.Bind2)
    };

    /// <summary>
    /// ��ɱ����ҹ��
    /// </summary>
    public static IBaseAction HissatsuYaten { get; } = new BaseAction(ActionID.HissatsuYaten)
    {
        ActionCheck = HissatsuGyoten.ActionCheck
    };

    /// <summary>
    /// ��ɱ��������
    /// </summary>
    public static IBaseAction HissatsuKyuten { get; } = new BaseAction(ActionID.HissatsuKyuten)
    {
        ActionCheck = b => Kenki >= 25
    };

    /// <summary>
    /// ��ɱ��������
    /// </summary>
    public static IBaseAction HissatsuGuren { get; } = new BaseAction(ActionID.HissatsuGuren)
    {
        ActionCheck = b => Kenki >= 25
    };

    /// <summary>
    /// ��ɱ������Ӱ
    /// </summary>
    public static IBaseAction HissatsuSenei { get; } = new BaseAction(ActionID.HissatsuSenei)
    {
        ActionCheck = b => Kenki >= 25
    };
    #endregion

    private protected override bool MoveForwardAbility(byte abilityRemain, out IAction act)
    {
        if (HissatsuGyoten.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }
}