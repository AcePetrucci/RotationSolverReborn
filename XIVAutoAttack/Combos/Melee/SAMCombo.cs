using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using XIVAutoAttack.Actions;

namespace XIVAutoAttack.Combos.Melee;

internal class SAMCombo : JobGaugeCombo<SAMGauge>
{
    internal override uint JobID => 34;

    protected override bool ShouldSayout => true;

    private static byte SenCount => (byte)((JobGauge.HasGetsu ? 1 : 0) + (JobGauge.HasSetsu ? 1 : 0) + (JobGauge.HasKa ? 1 : 0));
    internal struct Actions
    {
        public static readonly BaseAction
            //�з�
            Hakaze = new (7477),

            //���
            Jinpu = new (7478),

            //����
            ThirdEye = new (7498),

            //���
            Enpi = new (7486),

            //ʿ��
            Shifu = new (7479),

            //����
            Fuga = new (7483),

            //�¹�
            Gekko = new (7481)
            {
                EnermyLocation = EnemyLocation.Back,
            },

            //�˰���
            Higanbana = new (7489)
            {
                OtherCheck = b => !TargetHelper.IsMoving,
                TargetStatus = new [] { ObjectStatus.Higanbana },
            },

            //�����彣
            TenkaGoken = new (7488)
            {
                OtherCheck = b => !TargetHelper.IsMoving,
            },

            //����ѩ�»�
            MidareSetsugekka = new (7487)
            {
                OtherCheck = b => !TargetHelper.IsMoving,
            },

            //����
            Mangetsu = new (7484),

            //����
            Kasha = new (7482)
            {
                EnermyLocation = EnemyLocation.Side,
            },

            //ӣ��
            Oka = new (7485),

            //����ֹˮ
            MeikyoShisui = new (7499)
            {
                BuffsProvide = new [] { ObjectStatus.MeikyoShisui },
            },

            //ѩ��
            Yukikaze = new (7480),

            ////��ɱ��������
            //HissatsuKaiten = new BaseAction(7494),

            //��ɱ��������
            HissatsuGyoten = new (7492),

            //��ɱ��������
            HissatsuShinten = new (7490),

            //��ɱ��������
            HissatsuKyuten = new (7491),

            //��������
            Ikishoten = new (16482),

            //��ɱ��������
            HissatsuGuren = new (7496),

            //��ɱ������Ӱ
            HissatsuSenei = new (16481),

            //�ط��彣
            KaeshiGoken = new (16485),

            //�ط�ѩ�»�
            KaeshiSetsugekka = new (16486),

            //����
            Shoha = new (16487),

            //��������
            Shoha2 = new (25779),

            //����ն��
            OgiNamikiri = new (25781)
            {
                BuffsNeed = new [] { ObjectStatus.OgiNamikiriReady },
            },

            //�ط�ն��
            KaeshiNamikiri = new (25782);
    }
    internal override SortedList<DescType, string> Description => new ()
    {
        {DescType.�������, $"{Actions.ThirdEye.Action.Name}"},
        {DescType.�ƶ�, $"{Actions.HissatsuGyoten.Action.Name}"},
    };
    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //�Ͻ��ط���
        if (Service.IconReplacer.OriginalHook(Actions.OgiNamikiri.ID) == Actions.KaeshiNamikiri.ID)
        {
            if (Actions.KaeshiNamikiri.ShouldUseAction(out act, mustUse: true)) return true;
        }
        if (Service.IconReplacer.OriginalHook(16483) == Actions.KaeshiGoken.ID)
        {
            if (Actions.KaeshiGoken.ShouldUseAction(out act, mustUse: true)) return true;
        }
        if (Service.IconReplacer.OriginalHook(16483) == Actions.KaeshiSetsugekka.ID)
        {
            if (Actions.KaeshiSetsugekka.ShouldUseAction(out act, mustUse: true)) return true;
        }

        if (Actions.OgiNamikiri.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.TenkaGoken.ShouldUseAction(out act))
        {
            if (SenCount > 1) return true;
        }
        else if (SenCount > 0)
        {
            if (SenCount == 3 && Actions.MidareSetsugekka.ShouldUseAction(out act)) return true;
            if (Actions.Higanbana.ShouldUseAction(out act)) return true;
        }


        //123
        bool haveMeikyoShisui = StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.MeikyoShisui);
        //����ǵ��壬������ֹˮ����ȴʱ��С��3�롣
        if (!JobGauge.HasSetsu && !Actions.Fuga.ShouldUseAction(out _) && Actions.MeikyoShisui.RecastTimeRemain < 3)
        {
            if (Actions.Yukikaze.ShouldUseAction(out act, lastComboActionID)) return true;
        }
        if (!StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Moon))
        {
            if (Actions.Mangetsu.ShouldUseAction(out act, lastComboActionID, emptyOrSkipCombo: haveMeikyoShisui)) return true;
            if (Actions.Gekko.ShouldUseAction(out act, lastComboActionID, emptyOrSkipCombo: haveMeikyoShisui)) return true;
            if (Actions.Jinpu.ShouldUseAction(out act, lastComboActionID)) return true;
        }
        if (!StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Flower))
        {
            if (Actions.Oka.ShouldUseAction(out act, lastComboActionID, emptyOrSkipCombo: haveMeikyoShisui)) return true;
            if (Actions.Kasha.ShouldUseAction(out act, lastComboActionID, emptyOrSkipCombo: haveMeikyoShisui)) return true;
            if (Actions.Shifu.ShouldUseAction(out act, lastComboActionID)) return true;
        }
        if (!JobGauge.HasGetsu)
        {
            if (Actions.Mangetsu.ShouldUseAction(out act, lastComboActionID, emptyOrSkipCombo: haveMeikyoShisui)) return true;
            if (Actions.Gekko.ShouldUseAction(out act, lastComboActionID, emptyOrSkipCombo: haveMeikyoShisui)) return true;
            if (Actions.Jinpu.ShouldUseAction(out act, lastComboActionID)) return true;
        }
        if (!JobGauge.HasKa)
        {
            if (Actions.Oka.ShouldUseAction(out act, lastComboActionID, emptyOrSkipCombo: haveMeikyoShisui)) return true;
            if (Actions.Kasha.ShouldUseAction(out act, lastComboActionID, emptyOrSkipCombo: haveMeikyoShisui)) return true;
            if (Actions.Shifu.ShouldUseAction(out act, lastComboActionID)) return true;
        }
        if (!JobGauge.HasSetsu)
        {
            if (Actions.Yukikaze.ShouldUseAction(out act, lastComboActionID, emptyOrSkipCombo: haveMeikyoShisui)) return true;
        }
        if (Actions.Fuga.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.Hakaze.ShouldUseAction(out act, lastComboActionID)) return true;



        if (IconReplacer.Move && MoveAbility(1, out act)) return true;
        if (Actions.Enpi.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (JobGauge.Kenki >= 30 && Actions.HissatsuGyoten.ShouldUseAction(out act)) return true;
        act = null;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        if (JobGauge.MeditationStacks == 3)
        {
            if (Actions.Shoha2.ShouldUseAction(out act)) return true;
            if (Actions.Shoha.ShouldUseAction(out act)) return true;
        }

        if (JobGauge.Kenki >= 25)
        {
            if (Actions.HissatsuGuren.ShouldUseAction(out act)) return true;
            if (Actions.HissatsuKyuten.ShouldUseAction(out act)) return true;

            if (Actions.HissatsuSenei.ShouldUseAction(out act)) return true;
            if (Actions.HissatsuShinten.ShouldUseAction(out act)) return true;
        }

        if (TargetHelper.InBattle && Actions.Ikishoten.ShouldUseAction(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (HaveTargetAngle && Actions.MeikyoShisui.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;

        act = null;
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.ThirdEye.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //ǣ��
        if (GeneralActions.Feint.ShouldUseAction(out act)) return true;
        return false;
    }
}
