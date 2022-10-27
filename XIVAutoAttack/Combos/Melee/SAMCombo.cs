using Dalamud.Game.ClientState.JobGauge.Types;
using Lumina.Data.Parsing.Layer;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Controllers;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.Melee;

internal sealed class SAMCombo : JobGaugeCombo<SAMGauge>
{
    internal override uint JobID => 34;

    private static byte SenCount => (byte)((JobGauge.HasGetsu ? 1 : 0) + (JobGauge.HasSetsu ? 1 : 0) + (JobGauge.HasKa ? 1 : 0));

    internal struct Actions
    {
        public static readonly BaseAction
        #region ����GCD
            //�з�
            Hakaze = new(7477),

            //���
            Jinpu = new(7478),

            //ʿ��
            Shifu = new(7479),

            //�¹�
            Gekko = new(7481),

            //����
            Kasha = new(7482),

            //ѩ��
            Yukikaze = new(7480),

            //���
            Enpi = new(7486),
        #endregion
        #region Ⱥ��GCD

            //����
            Fuga = new(7483),

            //����
            Mangetsu = new(7484),

            //ӣ��
            Oka = new(7485),

            //���
            Fuko = new(2578),

        #endregion
        #region ����������

            //��ɱ��������
            HissatsuShinten = new(7490),

            //��ɱ��������
            HissatsuGyoten = new(7492),

            //��ɱ����ҹ��
            HissatsuYaten = new(7943),

            //��ɱ��������
            HissatsuKyuten = new(7491),

            //��ɱ��������
            HissatsuGuren = new(7496),

            //��ɱ������Ӱ
            HissatsuSenei = new(16481),
        #endregion
        #region �Ӻ���

            //�˰���
            Higanbana = new(7489, isDot: true)
            {
                OtherCheck = b => !IsMoving && SenCount == 1,
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

            //�ط��˰���
            KaeshiHiganbana = new(17741, isDot: true),

            //�ط��彣
            KaeshiGoken = new(16485),

            //�ط�ѩ�»�
            KaeshiSetsugekka = new(16486),
        #endregion
        #region �����˺�������

            //����
            Shoha = new(16487),

            //��������
            Shoha2 = new(25779),

            //����ն��
            OgiNamikiri = new(25781)
            {
                BuffsNeed = new[] { ObjectStatus.OgiNamikiriReady },
            },

            //�ط�ն��
            KaeshiNamikiri = new(25782),
        #endregion
        #region ����������

            //����
            ThirdEye = new(7498),

            //����ֹˮ
            MeikyoShisui = new(7499)
            {
                BuffsProvide = new[] { ObjectStatus.MeikyoShisui },
                OtherCheck = b => JobGauge.HasSetsu && !JobGauge.HasKa && !JobGauge.HasGetsu,
            },

            //Ҷ��
            Hagakure = new(7495)
            {
                OtherCheck = b => SenCount == 1,
            },

            //��������
            Ikishoten = new(16482),

            //��ط�
            Tsubamegaeshi = new(16483),

            //�汱
            TrueNorth = new(7546);
        #endregion
    }
    internal override SortedList<DescType, string> Description => new()
    {
        {DescType.�������, $"{Actions.ThirdEye.Action.Name}"},
        {DescType.�ƶ�����, $"{Actions.HissatsuGyoten.Action.Name}"},
    };

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //�ط�ն��
        if (Service.IconReplacer.OriginalHook(Actions.OgiNamikiri.ID) == Actions.KaeshiNamikiri.ID)
        {
            if (Actions.KaeshiNamikiri.ShouldUse(out act, mustUse: true)) return true;
        }
        //�ط��彣
        if (Service.IconReplacer.OriginalHook(Actions.Tsubamegaeshi.ID) == Actions.KaeshiGoken.ID)
        {
            if (Actions.KaeshiGoken.ShouldUse(out act, mustUse: true)) return true;
        }
        //�ط�ѩ�»�
        if (Service.IconReplacer.OriginalHook(Actions.Tsubamegaeshi.ID) == Actions.KaeshiSetsugekka.ID)
        {
            if (Actions.KaeshiSetsugekka.ShouldUse(out act, mustUse: true)) return true;
        }
        if (Actions.OgiNamikiri.ShouldUse(out act, mustUse: true)) return true;
        //�����彣
        if (Actions.TenkaGoken.ShouldUse(out act))
        {
            if (SenCount == 2) return true;
        }
        else
        {
            //ѩ���£�����
            if (Actions.MidareSetsugekka.ShouldUse(out act)) return true;
            //�˰���������ֹˮ�д����ͬʱӦ�����ĵ�һ������
            if (Player.HaveStatus(ObjectStatus.MeikyoShisui) && Player.HaveStatus(ObjectStatus.Moon) && Actions.Higanbana.ShouldUse(out act)) return true;
        }
        bool haveMeikyoShisui = Player.HaveStatus(ObjectStatus.MeikyoShisui);
        if (!JobGauge.HasSetsu && !Player.HaveStatus(ObjectStatus.Moon))
        {
            if (GetsuGCD(out act, lastComboActionID, haveMeikyoShisui)) return true;
        }
        if (!Player.HaveStatus(ObjectStatus.Flower))
        {
            if (KaGCD(out act, lastComboActionID, haveMeikyoShisui)) return true;
        }
        if (!JobGauge.HasSetsu)//ȷ���ǵ�����û��ѩ��
        {
            if (Actions.Yukikaze.ShouldUse(out act, lastComboActionID)) return true;//��ѩ��
        }
        if (!JobGauge.HasGetsu) //��
        {
            if (GetsuGCD(out act, lastComboActionID, haveMeikyoShisui)) return true;
        }
        if (!JobGauge.HasKa) //��
        {
            if (KaGCD(out act, lastComboActionID, haveMeikyoShisui)) return true;
        }
        if (!JobGauge.HasSetsu) //ѩ
        {
            if (Actions.Yukikaze.ShouldUse(out act, lastComboActionID, emptyOrSkipCombo: haveMeikyoShisui)) return true;
        }

        //�����£�
        if (GetsuGCD(out act, lastComboActionID, haveMeikyoShisui)) return true;
        if (Actions.Fuga.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.Hakaze.ShouldUse(out act, lastComboActionID)) return true;


        if (CommandController.Move && MoveAbility(1, out act)) return true;
        if (Actions.Enpi.ShouldUse(out act)) return true;

        return false;
    }
    private bool GetsuGCD(out IAction act, uint lastComboActionID, bool haveMeikyoShisui)
    {
        if (Actions.Mangetsu.ShouldUse(out act, lastComboActionID, emptyOrSkipCombo: haveMeikyoShisui)) return true;
        if (Actions.Gekko.ShouldUse(out act, lastComboActionID, emptyOrSkipCombo: haveMeikyoShisui)) return true;
        if (Actions.Jinpu.ShouldUse(out act, lastComboActionID)) return true;

        act = null;
        return false;
    }

    private bool KaGCD(out IAction act, uint lastComboActionID, bool haveMeikyoShisui)
    {
        if (Actions.Oka.ShouldUse(out act, lastComboActionID, emptyOrSkipCombo: haveMeikyoShisui)) return true;
        if (Actions.Kasha.ShouldUse(out act, lastComboActionID, emptyOrSkipCombo: haveMeikyoShisui)) return true;
        if (Actions.Shifu.ShouldUse(out act, lastComboActionID)) return true;

        act = null;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (JobGauge.Kenki >= 30 && Actions.HissatsuGyoten.ShouldUse(out act)) return true;
        act = null;
        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        //��������ѹ�ˣ��Ǿʹ����ն��
        if (JobGauge.MeditationStacks == 3)
        {
            if (Actions.Shoha2.ShouldUse(out act)) return true;
            if (Actions.Shoha.ShouldUse(out act)) return true;
        }

        if (JobGauge.Kenki >= 25)
        {
            if (Actions.HissatsuGuren.ShouldUse(out act)) return true;
            if (Actions.HissatsuKyuten.ShouldUse(out act)) return true;

            if (Actions.HissatsuSenei.ShouldUse(out act)) return true;
            if (Actions.HissatsuShinten.ShouldUse(out act)) return true;
        }

        if (InCombat && Actions.Ikishoten.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (HaveHostileInRange &&
            !nextGCD.IsAnySameAction(false, Actions.Higanbana, Actions.OgiNamikiri, Actions.KaeshiNamikiri) &&
            Actions.MeikyoShisui.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.ThirdEye.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //ǣ��
        if (GeneralActions.Feint.ShouldUse(out act)) return true;
        return false;
    }
}
