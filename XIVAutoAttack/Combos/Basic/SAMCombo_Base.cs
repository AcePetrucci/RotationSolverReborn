using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.Basic;

internal abstract class SAMCombo_Base<TCmd> : CustomCombo<TCmd> where TCmd : Enum
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
    /// ʲô����
    /// </summary>
    protected static byte Kenki => JobGauge.Kenki;

    /// <summary>
    /// ��ѹ
    /// </summary>
    protected static byte MeditationStacks => JobGauge.MeditationStacks;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Samurai };

    protected static byte SenCount => (byte)((HasGetsu ? 1 : 0) + (HasSetsu ? 1 : 0) + (HasKa ? 1 : 0));

    protected static bool HaveMoon => Player.HasStatus(true, StatusID.Moon);
    protected static bool HaveFlower => Player.HasStatus(true, StatusID.Flower);

    /// <summary>
    /// �з�
    /// </summary>
    public static BaseAction Hakaze { get; } = new(ActionID.Hakaze);

    /// <summary>
    /// ���
    /// </summary>
    public static BaseAction Jinpu { get; } = new(ActionID.Jinpu);

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction ThirdEye { get; } = new(ActionID.ThirdEye, true);

    /// <summary>
    /// ���
    /// </summary>
    public static BaseAction Enpi { get; } = new(ActionID.Enpi);

    /// <summary>
    /// ʿ��
    /// </summary>
    public static BaseAction Shifu { get; } = new(ActionID.Shifu);

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Fuga { get; } = new(ActionID.Fuga);

    /// <summary>
    /// �¹�
    /// </summary>
    public static BaseAction Gekko { get; } = new(ActionID.Gekko);

    /// <summary>
    /// �˰���
    /// </summary>
    public static BaseAction Higanbana { get; } = new(ActionID.Higanbana, isEot: true)
    {
        OtherCheck = b => !IsMoving && SenCount == 1,
        TargetStatus = new[] { StatusID.Higanbana },
    };

    /// <summary>
    /// �����彣
    /// </summary>
    public static BaseAction TenkaGoken { get; } = new(ActionID.TenkaGoken)
    {
        OtherCheck = b => !IsMoving && SenCount == 2,
    };

    /// <summary>
    /// ����ѩ�»�
    /// </summary>
    public static BaseAction MidareSetsugekka { get; } = new(ActionID.MidareSetsugekka)
    {
        OtherCheck = b => !IsMoving && SenCount == 3,
    };

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Mangetsu { get; } = new(ActionID.Mangetsu);

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Kasha { get; } = new(ActionID.Kasha);

    /// <summary>
    /// ӣ��
    /// </summary>
    public static BaseAction Oka { get; } = new(ActionID.Oka);

    /// <summary>
    /// ����ֹˮ
    /// </summary>
    public static BaseAction MeikyoShisui { get; } = new(ActionID.MeikyoShisui)
    {
        BuffsProvide = new[] { StatusID.MeikyoShisui },
    };

    /// <summary>
    /// ѩ��
    /// </summary>
    public static BaseAction Yukikaze { get; } = new(ActionID.Yukikaze);

    /// <summary>
    /// ��ɱ��������
    /// </summary>
    public static BaseAction HissatsuGyoten { get; } = new(ActionID.HissatsuGyoten);

    /// <summary>
    /// ��ɱ��������
    /// </summary>
    public static BaseAction HissatsuShinten { get; } = new(ActionID.HissatsuShinten);

    /// <summary>
    /// ��ɱ��������
    /// </summary>
    public static BaseAction HissatsuKyuten { get; } = new(ActionID.HissatsuKyuten);

    /// <summary>
    /// ��������
    /// </summary>
    public static BaseAction Ikishoten { get; } = new(ActionID.Ikishoten);

    /// <summary>
    /// ��ɱ��������
    /// </summary>
    public static BaseAction HissatsuGuren { get; } = new(ActionID.HissatsuGuren);

    /// <summary>
    /// ��ɱ������Ӱ
    /// </summary>
    public static BaseAction HissatsuSenei { get; } = new(ActionID.HissatsuSenei);

    /// <summary>
    /// �ط��彣
    /// </summary>
    public static BaseAction KaeshiGoken { get; } = new(ActionID.KaeshiGoken);

    /// <summary>
    /// �ط�ѩ�»�
    /// </summary>
    public static BaseAction KaeshiSetsugekka { get; } = new(ActionID.KaeshiSetsugekka);

    /// <summary>
    /// ����
    /// </summary>
    public static BaseAction Shoha { get; } = new(ActionID.Shoha);

    /// <summary>
    /// ��������
    /// </summary>
    public static BaseAction Shoha2 { get; } = new(ActionID.Shoha2);

    /// <summary>
    /// ����ն��
    /// </summary>
    public static BaseAction OgiNamikiri { get; } = new(ActionID.OgiNamikiri)
    {
        OtherCheck = b => HaveFlower && HaveMoon,
        BuffsNeed = new[] { StatusID.OgiNamikiriReady },
    };

    /// <summary>
    /// �ط�ն��
    /// </summary>
    public static BaseAction KaeshiNamikiri { get; } = new(ActionID.KaeshiNamikiri);

}