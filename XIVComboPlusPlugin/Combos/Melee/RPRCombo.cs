using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboPlus.Combos;

internal class RPRCombo : CustomComboJob<RPRGauge>
{
    internal override uint JobID => 39;

    internal struct Actions
    {
        public static readonly BaseAction
            //����֮Ӱ
            ShadowofDeath = new BaseAction(24378)
            {
                TargetStatus = new ushort[] { ObjectStatus.DeathsDesign },
            },

            //�и�
            Slice = new BaseAction(24373),

            //��ӯ�и�
            WaxingSlice = new BaseAction(24374),

            //�����и�
            InfernalSlice = new BaseAction(24375),

            //����Ӹ�
            BloodStalk = new BaseAction(24389),

            //����
            Harpe = new BaseAction(24386),

            //�ʾ�
            Gibbet = new BaseAction(24382) { EnermyLocation = EnemyLocation.Side },

            //��ɱ
            Gallows = new BaseAction(24383) { EnermyLocation = EnemyLocation.Back },

            //����и�
            SoulSlice = new BaseAction(24380),

            //����֮��
            WhorlofDeath = new BaseAction(24379)
            {
                TargetStatus = new ushort[] { ObjectStatus.DeathsDesign },
            },

            //��ת�̸�
            SpinningScythe = new BaseAction(24376),

            //ج���̸�
            NightmareScythe = new BaseAction(24377),

            //�����Ӹ�
            GrimSwathe = new BaseAction(24392),

            //��ʳ
            Gluttony = new BaseAction(24393),

            //����
            Guillotine = new BaseAction(24384),

            //����̸�
            SoulScythe = new BaseAction(24381),

            //ҹ�λ��� ����
            Enshroud = new BaseAction(24394),

            //����
            Communio = new BaseAction(24398),

            //������ �Ӷ�
            ArcaneCrest = new BaseAction(24404),

            //���ػ� ��Buff
            ArcaneCircle = new BaseAction(24405),

            //������
            Soulsow = new BaseAction(24387)
            {
                OtherCheck = b =>
                {
                    if (!HaveTargetAngle) return true;
                    return false;
                },
            },

            //�����뾳
            HellsIngress = new BaseAction(24401),

            //�����
            PlentifulHarvest = new BaseAction(24385);
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out BaseAction act)
    {
        //���ڱ���״̬��
        if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Enshrouded))
        {
            if (JobGauge.LemureShroud == 1)
            {
                if (Actions.Communio.ShouldUseAction(out act, mustUse: true)) return true;
            }

            if (Actions.Guillotine.ShouldUseAction(out act)) return true;

            if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.EnhancedVoidReaping))
            {
                if (Actions.Gibbet.ShouldUseAction(out act)) return true;
            }
            else
            {
                if (Actions.Gallows.ShouldUseAction(out act)) return true;
            }
        }
        //���ڲ���״̬���Ͻ���������
        else if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.SoulReaver))
        {
            if (Actions.Guillotine.ShouldUseAction(out act)) return true;

            if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.EnhancedGibbet))
            {
                if (Actions.Gibbet.ShouldUseAction(out act)) return true;
            }
            else
            {
                if (Actions.Gallows.ShouldUseAction(out act)) return true;
            }
        }

        if (JobGauge.Shroud <= 50 && BaseAction.HaveStatusSelfFromSelf(ObjectStatus.ImmortalSacrifice) &&
            !BaseAction.HaveStatusSelfFromSelf(ObjectStatus.CircleofSacrifice))
        {
            //����գ�
            if (Actions.PlentifulHarvest.ShouldUseAction(out act)) return true;
        }

        //������ 50.
        if (JobGauge.Soul <= 50)
        {
            if (Actions.SoulScythe.ShouldUseAction(out act, Empty: true)) return true;
            if (Actions.SoulSlice.ShouldUseAction(out act, Empty: true)) return true;
        }

        //Ⱥ�����
        if (Actions.NightmareScythe.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.SpinningScythe.ShouldUseAction(out act, lastComboActionID)) return true;

        //��������
        if (Actions.InfernalSlice.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.WaxingSlice.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.Slice.ShouldUseAction(out act, lastComboActionID)) return true;

        //��������
        if (Actions.Harpe.ShouldUseAction(out act, lastComboActionID)) return true;

        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out BaseAction act)
    {
        //����������
        if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Enshrouded))
        {
            if (JobGauge.VoidShroud > 1)
            {
                if (Actions.GrimSwathe.ShouldUseAction(out act)) return true;
                if (Actions.BloodStalk.ShouldUseAction(out act)) return true;
            }
        }
        //�������ˣ�����
        if (JobGauge.Shroud >= 50 && Actions.Enshroud.ShouldUseAction(out act)) return true;

        //��깻�ˣ�������״̬��
        if (JobGauge.Soul >= 50)
        {
            if (Actions.Gluttony.ShouldUseAction(out act, mustUse: true)) return true;
            if (Actions.GrimSwathe.ShouldUseAction(out act)) return true;
            if (Actions.BloodStalk.ShouldUseAction(out act)) return true;
        }
        //�����Ÿ�
        if (Actions.ArcaneCircle.ShouldUseAction(out act)) return true;

        //��Ѫ
        if (GeneralActions.Bloodbath.ShouldUseAction(out act)) return true;
        if (GeneralActions.SecondWind.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out BaseAction act)
    {
        //ǣ��
        if (GeneralActions.Feint.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out BaseAction act)
    {
        //�����뾳
        if (Actions.HellsIngress.ShouldUseAction(out act) && !BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Threshold)) return true;
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out BaseAction act)
    {
        //������
        if (Actions.ArcaneCrest.ShouldUseAction(out act)) return true;
        return false;
    }
}
