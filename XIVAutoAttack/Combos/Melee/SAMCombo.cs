using Dalamud.Game.ClientState.JobGauge.Types;
using Lumina.Data.Parsing.Layer;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.SigReplacers;
using static XIVAutoAttack.Combos.Melee.SAMCombo;

namespace XIVAutoAttack.Combos.Melee;

internal sealed class SAMCombo : JobGaugeCombo<SAMGauge, CommandType>
{
    public override ComboAuthor[] Authors => new ComboAuthor[] { ComboAuthor.fatinghenji };

    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //д��ע�Ͱ���������ʾ�û��ġ�
    };
    public override uint[] JobIDs => new uint[] { 34 };

    private static byte SenCount => (byte)((JobGauge.HasGetsu ? 1 : 0) + (JobGauge.HasSetsu ? 1 : 0) + (JobGauge.HasKa ? 1 : 0));

    private static bool HaveMoon => Player.HaveStatus(ObjectStatus.Moon);
    private static bool HaveFlower => Player.HaveStatus(ObjectStatus.Flower);


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
        Gekko = new(7481),

        //�˰���
        Higanbana = new(7489, isDot: true)
        {
            OtherCheck = b => !IsMoving && SenCount == 1 && HaveMoon && HaveFlower,
            TargetStatus = new[] { ObjectStatus.Higanbana },
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
        Kasha = new(7482),

        //ӣ��
        Oka = new(7485),

        //����ֹˮ
        MeikyoShisui = new(7499)
        {
            BuffsProvide = new[] { ObjectStatus.MeikyoShisui },
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
            BuffsNeed = new[] { ObjectStatus.OgiNamikiriReady },
        },

        //�ط�ն��
        KaeshiNamikiri = new(25782);

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.�������, $"{ThirdEye}"},
        {DescType.�ƶ�����, $"{HissatsuGyoten}"},
    };
    private protected override bool GeneralGCD(out IAction act)
    {
        bool haveMeikyoShisui = Player.HaveStatus(ObjectStatus.MeikyoShisui);

        //�Ͻ��ط���
        if (Service.IconReplacer.OriginalHook(OgiNamikiri.ID) == KaeshiNamikiri.ID)
        {
            if (KaeshiNamikiri.ShouldUse(out act, mustUse: true)) return true;
        }
        if (Service.IconReplacer.OriginalHook(16483) == KaeshiGoken.ID)
        {
            if (KaeshiGoken.ShouldUse(out act, mustUse: true)) return true;
        }
        if (Service.IconReplacer.OriginalHook(16483) == KaeshiSetsugekka.ID)
        {
            if (KaeshiSetsugekka.ShouldUse(out act, mustUse: true)) return true;
        }

        if (!haveMeikyoShisui && OgiNamikiri.ShouldUse(out act, mustUse: true)) return true;
        if (TenkaGoken.ShouldUse(out act))
        {
            if (SenCount == 2) return true;
            if (MidareSetsugekka.ShouldUse(out act)) return true;
        }
        else
        {
            if (MidareSetsugekka.ShouldUse(out act)) return true;
            //�����˱˰����߼���Ӧ����ӵ��ѩ����ʱ����
            if (Higanbana.ShouldUse(out act) && JobGauge.HasSetsu) return true;
        }

        //123
        //����ǵ��壬������ֹˮ����ȴʱ��С��3�롣
        if (!JobGauge.HasSetsu && !Fuga.ShouldUse(out _))
        {
            if (Yukikaze.ShouldUse(out act)) return true;
        }
        if (!HaveMoon)//�жϷ���buff����buff�ṩ10%�˺��ӳ�
        {
            if (GetsuGCD(out act, haveMeikyoShisui)) return true;
        }
        if (!HaveFlower)//�жϷ绨buff����buff�ṩ10%���ټӳ�
        {
            if (KaGCD(out act, haveMeikyoShisui)) return true;
        }
        if (!JobGauge.HasGetsu) //����
        {
            if (GetsuGCD(out act, haveMeikyoShisui)) return true;
        }
        if (!JobGauge.HasKa) //����
        {
            if (KaGCD(out act, haveMeikyoShisui)) return true;
        }

        //�����£�
        if (GetsuGCD(out act, haveMeikyoShisui)) return true;
        if (Yukikaze.ShouldUse(out act, emptyOrSkipCombo: haveMeikyoShisui)) return true;

        if (Fuga.ShouldUse(out act)) return true;
        if (Hakaze.ShouldUse(out act)) return true;



        if (CommandController.Move && MoveAbility(1, out act)) return true;
        if (Enpi.ShouldUse(out act)) return true;

        return false;
    }

    //����ӣ������
    private bool KaGCD(out IAction act, bool haveMeikyoShisui)
    {
        if (Oka.ShouldUse(out act, emptyOrSkipCombo: haveMeikyoShisui)) return true;
        if (Kasha.ShouldUse(out act, emptyOrSkipCombo: haveMeikyoShisui)) return true;
        if (Shifu.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    //�����¹�����
    private bool GetsuGCD(out IAction act, bool haveMeikyoShisui)
    {
        if (Mangetsu.ShouldUse(out act, emptyOrSkipCombo: haveMeikyoShisui)) return true;
        if (Gekko.ShouldUse(out act, emptyOrSkipCombo: haveMeikyoShisui)) return true;
        if (Jinpu.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (JobGauge.Kenki >= 30 && HissatsuGyoten.ShouldUse(out act)) return true;
        act = null;
        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (JobGauge.MeditationStacks == 3)
        {
            if (Shoha2.ShouldUse(out act)) return true;
            if (Shoha.ShouldUse(out act)) return true;
        }

        if (JobGauge.Kenki >= 25)
        {
            if (HissatsuGuren.ShouldUse(out act)) return true;
            if (HissatsuKyuten.ShouldUse(out act)) return true;

            if (HissatsuSenei.ShouldUse(out act)) return true;
            if (HissatsuShinten.ShouldUse(out act)) return true;
        }

        if (InCombat && Ikishoten.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (HaveHostileInRange &&
            !nextGCD.IsAnySameAction(false, Higanbana, OgiNamikiri, KaeshiNamikiri) &&
            MeikyoShisui.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (ThirdEye.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //ǣ��
        if (GeneralActions.Feint.ShouldUse(out act)) return true;
        return false;
    }
}