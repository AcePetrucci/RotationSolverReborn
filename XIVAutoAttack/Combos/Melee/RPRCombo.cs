using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.CustomCombo;

namespace XIVAutoAttack.Combos.Melee;

internal class RPRCombo : JobGaugeCombo<RPRGauge>
{
    internal override uint JobID => 39;
    protected override bool ShouldSayout => !StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Enshrouded);
    internal struct Actions
    {
        public static readonly BaseAction
            //����֮Ӱ
            ShadowofDeath = new (24378)
            {
                TargetStatus = new [] { ObjectStatus.DeathsDesign },
            },

            //�и�
            Slice = new (24373),

            //��ӯ�и�
            WaxingSlice = new (24374),

            //�����и�
            InfernalSlice = new (24375),

            //����Ӹ�
            BloodStalk = new (24389),

            //����
            Harpe = new (24386) { BuffsProvide = new [] { ObjectStatus.SoulReaver } },

            //�ʾ�
            Gibbet = new (24382) { EnermyLocation = EnemyLocation.Side },

            //��ɱ
            Gallows = new (24383) { EnermyLocation = EnemyLocation.Back },

            //����и�
            SoulSlice = new (24380),

            //����֮��
            WhorlofDeath = new (24379)
            {
                TargetStatus = new [] { ObjectStatus.DeathsDesign },
            },

            //��ת�̸�
            SpinningScythe = new (24376),

            //ج���̸�
            NightmareScythe = new (24377),

            //�����Ӹ�
            GrimSwathe = new (24392),

            //��ʳ
            Gluttony = new (24393),

            //����
            Guillotine = new (24384),

            //����̸�
            SoulScythe = new (24381),

            //ҹ�λ��� ����
            Enshroud = new (24394),

            //����
            Communio = new (24398),

            //������ �Ӷ�
            ArcaneCrest = new (24404, true),

            //���ػ� ��Buff
            ArcaneCircle = new (24405, true),

            //������
            Soulsow = new (24387)
            {
                BuffsProvide = new [] {ObjectStatus.Soulsow},
            },

            //�ջ���
            HarvestMoon = new (24388)
            {
                BuffsNeed = new [] { ObjectStatus.Soulsow },
            },

            //�����뾳
            HellsIngress = new (24401),

            //�����
            PlentifulHarvest = new (24385);
    }
    internal override SortedList<DescType, string> Description => new ()
    {
        {DescType.�������, $"{Actions.ArcaneCrest.Action.Name}"},
        {DescType.�ƶ�, $"{Actions.HellsIngress.Action.Name}"},
    };
    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        if(!TargetHelper.InBattle && Actions.Soulsow.ShouldUseAction(out act)) return true;

        //���ڱ���״̬��
        if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Enshrouded))
        {
            if (JobGauge.LemureShroud == 1)
            {
                if (Actions.Communio.ShouldUseAction(out act, mustUse: true)) return true;
            }

            if (Actions.Guillotine.ShouldUseAction(out act)) return true;

            if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.EnhancedVoidReaping))
            {
                if (Actions.Gibbet.ShouldUseAction(out act)) return true;
            }
            else
            {
                if (Actions.Gallows.ShouldUseAction(out act)) return true;
            }
        }
        //���ڲ���״̬���Ͻ���������
        else if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.SoulReaver))
        {
            if (Actions.Guillotine.ShouldUseAction(out act)) return true;

            if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.EnhancedGibbet))
            {
                if (Actions.Gibbet.ShouldUseAction(out act)) return true;
            }
            else
            {
                if (Actions.Gallows.ShouldUseAction(out act)) return true;
            }
        }

        //��Debuff
        if (Actions.WhorlofDeath.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.ShadowofDeath.ShouldUseAction(out act, lastComboActionID)) return true;


        if (JobGauge.Shroud <= 50 && !StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.CircleofSacrifice)
            && StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.ImmortalSacrifice) &&
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
        if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Enshrouded))
        {
            if (JobGauge.VoidShroud > 1)
            {
                if (Actions.GrimSwathe.ShouldUseAction(out act)) return true;
                if (Actions.BloodStalk.ShouldUseAction(out act)) return true;
            }
        }

        if (!StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.SoulReaver))
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
        if (Actions.HellsIngress.ShouldUseAction(out act) && !StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Threshold)) return true;
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        //������
        if (Actions.ArcaneCrest.ShouldUseAction(out act)) return true;
        return false;
    }


}
