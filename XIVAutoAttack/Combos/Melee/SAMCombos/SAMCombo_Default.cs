using Dalamud.Game.ClientState.JobGauge.Types;
using Lumina.Data.Parsing.Layer;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Attributes;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.SigReplacers;
using static XIVAutoAttack.Combos.Melee.SAMCombos.SAMCombo_Default;

namespace XIVAutoAttack.Combos.Melee.SAMCombos;

[ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/XIVAutoAttack/Combos/Melee/SAMCombos/SAMCombo_Default.cs")]
internal sealed class SAMCombo_Default : SAMCombo_Base<CommandType>
{
    public override string Author => "����";

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
        {DescType.�������, $"{ThirdEye}"},
        {DescType.�ƶ�����, $"{HissatsuGyoten}"},
    };

    private protected override bool GeneralGCD(out IAction act)
    {
        bool haveMeikyoShisui = Player.HaveStatus(StatusID.MeikyoShisui);

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
        if (Feint.ShouldUse(out act)) return true;
        return false;
    }
}