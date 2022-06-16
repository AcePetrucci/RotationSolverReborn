using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using System.Linq;

namespace XIVAutoAttack.Combos.Melee;

internal class RPRCombo : CustomComboJob<RPRGauge>
{
    internal override uint JobID => 39;
    protected override bool ShouldSayout => !BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Enshrouded);
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
            Harpe = new BaseAction(24386) { BuffsProvide = new ushort[] { ObjectStatus.SoulReaver } },

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
            ArcaneCrest = new BaseAction(24404, true),

            //���ػ� ��Buff
            ArcaneCircle = new BaseAction(24405, true),

            //������
            Soulsow = new BaseAction(24387)
            {
                BuffsProvide = new ushort[] {ObjectStatus.Soulsow},
            },

            //�ջ���
            HarvestMoon = new BaseAction(24388)
            {
                BuffsNeed = new ushort[] { ObjectStatus.Soulsow },
            },

            //�����뾳
            HellsIngress = new BaseAction(24401),

            //�����
            PlentifulHarvest = new BaseAction(24385);
    }
    internal override SortedList<DescType, string> Description => new SortedList<DescType, string>()
    {
        {DescType.�������, $"{Actions.ArcaneCrest.Action.Name}"},
        {DescType.�ƶ�, $"{Actions.HellsIngress.Action.Name}"},
    };
    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        if(!TargetHelper.InBattle && Actions.Soulsow.ShouldUseAction(out act)) return true;

        //��Debuff
        if (Actions.WhorlofDeath.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.ShadowofDeath.ShouldUseAction(out act, lastComboActionID)) return true;

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

        if (JobGauge.Shroud <= 50 && !BaseAction.HaveStatusSelfFromSelf(ObjectStatus.CircleofSacrifice)
            && BaseAction.HaveStatusSelfFromSelf(ObjectStatus.ImmortalSacrifice) &&
             Actions.PlentifulHarvest.ShouldUseAction(out act, mustUse: true)) return true;


        //������ 50.
        if (JobGauge.Soul <= 50)
        {
            if (Actions.SoulScythe.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
            if (Actions.SoulSlice.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
        }


        //Ⱥ�����
        if (Actions.NightmareScythe.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.SpinningScythe.ShouldUseAction(out act, lastComboActionID)) return true;


        //��������
        if (Actions.InfernalSlice.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.WaxingSlice.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.Slice.ShouldUseAction(out act, lastComboActionID)) return true;

        //��������
        if (IconReplacer.Move && MoveAbility(1, out act)) return true;

        if (Actions.HarvestMoon.ShouldUseAction(out act, mustUse:true)) return true;
        if (Actions.Harpe.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
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

        if (!BaseAction.HaveStatusSelfFromSelf(ObjectStatus.SoulReaver))
        {
            //�������ˣ�����
            if (JobGauge.Shroud >= 50 && Actions.Enshroud.ShouldUseAction(out act)) return true;

            //��깻�ˣ�������״̬��
            if (JobGauge.Soul >= 50)
            {
                if (Actions.Gluttony.ShouldUseAction(out act, mustUse: true)) return true;
                if (Actions.GrimSwathe.ShouldUseAction(out act)) return true;
                if (Actions.BloodStalk.ShouldUseAction(out act)) return true;
            }
        }

        act = null;
        return false;
    }

    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        //�����Ÿ�
        if (Actions.ArcaneCircle.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //ǣ��
        if (GeneralActions.Feint.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //�����뾳
        if (Actions.HellsIngress.ShouldUseAction(out act) && !BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Threshold)) return true;
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        //������
        if (Actions.ArcaneCrest.ShouldUseAction(out act)) return true;
        return false;
    }


}
