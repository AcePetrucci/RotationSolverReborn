using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.Basic;

//[ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/XIVAutoAttack/Combos/Melee/SAMCombo.cs",
//   ComboAuthor.fatinghenji)]
internal abstract class SAMCombo_Base<TCmd> : JobGaugeCombo<SAMGauge, TCmd> where TCmd : Enum
{
    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Samurai };

    protected static byte SenCount => (byte)((JobGauge.HasGetsu ? 1 : 0) + (JobGauge.HasSetsu ? 1 : 0) + (JobGauge.HasKa ? 1 : 0));

    protected static bool HaveMoon => Player.HaveStatus(true, StatusID.Moon);
    protected static bool HaveFlower => Player.HaveStatus(true, StatusID.Flower);


    public static readonly BaseAction
        //�з�
        Hakaze = new(7477),

        //���
        Jinpu = new(7478),

        //����
        ThirdEye = new(7498),

        //���
        Enpi = new(7486),

        //ʿ��
        Shifu = new(7479),

        //����
        Fuga = new(7483),

        //�¹�
        Gekko = new(ActionIDs.Gekko),

        //�˰���
        Higanbana = new(7489, isEot: true)
        {
            OtherCheck = b => !IsMoving && SenCount == 1 && HaveMoon && HaveFlower,
            TargetStatus = new[] { StatusID.Higanbana },
        },

        //�����彣
        TenkaGoken = new(7488)
        {
            OtherCheck = b => !IsMoving,
        },

        //����ѩ�»�
        MidareSetsugekka = new(7487)
        {
            OtherCheck = b => !IsMoving && SenCount == 3,
        },

        //����
        Mangetsu = new(7484),

        //����
        Kasha = new(ActionIDs.Kasha),

        //ӣ��
        Oka = new(7485),

        //����ֹˮ
        MeikyoShisui = new(7499)
        {
            BuffsProvide = new[] { StatusID.MeikyoShisui },
            OtherCheck = b => JobGauge.HasSetsu && !JobGauge.HasKa && !JobGauge.HasGetsu,
        },

        //ѩ��
        Yukikaze = new(7480),

        //��ɱ��������
        HissatsuGyoten = new(7492),

        //��ɱ��������
        HissatsuShinten = new(7490),

        //��ɱ��������
        HissatsuKyuten = new(7491),

        //��������
        Ikishoten = new(16482),

        //��ɱ��������
        HissatsuGuren = new(7496),

        //��ɱ������Ӱ
        HissatsuSenei = new(16481),

        //�ط��彣
        KaeshiGoken = new(16485),

        //�ط�ѩ�»�
        KaeshiSetsugekka = new(16486),

        //����
        Shoha = new(16487),

        //��������
        Shoha2 = new(25779),

        //����ն��
        OgiNamikiri = new(25781)
        {
            OtherCheck = b => HaveFlower && HaveMoon,
            BuffsNeed = new[] { StatusID.OgiNamikiriReady },
        },

        //�ط�ն��
        KaeshiNamikiri = new(25782);

}