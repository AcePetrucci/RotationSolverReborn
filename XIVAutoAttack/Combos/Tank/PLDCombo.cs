using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using XIVAutoAttack;
using XIVAutoAttack.Combos;

namespace XIVAutoAttack.Combos.Tank;

internal class PLDCombo : CustomComboJob<PLDGauge>
{
    internal override uint JobID => 19;

    internal override bool HaveShield => BaseAction.HaveStatusSelfFromSelf(ObjectStatus.IronWill);

    private protected override BaseAction Shield => Actions.IronWill;

    protected override bool CanHealSingleSpell => false;
    protected override bool CanHealAreaSpell => false;

    internal struct Actions
    {
        public static readonly BaseAction
            //��������
            IronWill = new (28, shouldEndSpecial: true),

            //�ȷ潣
            FastBlade = new (9),

            //���ҽ�
            RiotBlade = new (15),

            //��Ѫ��
            GoringBlade = new (3538)
            {
                TargetStatus = new []
                {
                    ObjectStatus.GoringBlade,
                    ObjectStatus.BladeofValor,
                }
            },

            //սŮ��֮ŭ
            RageofHalone = new (21),

            //Ͷ��
            ShieldLob = new (24)
            {
                FilterForHostile = b => BaseAction.ProvokeTarget(b, out _),
            },

            //ս�ӷ�Ӧ
            FightorFlight = new (20),

            //ȫʴն
            TotalEclipse = new (7381),

            //����ն
            Prominence = new (16457),

            //Ԥ��
            Sentinel = new (17)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
            },

            //������ת
            CircleofScorn = new (23),

            //���֮��
            SpiritsWithin = new (29),

            //��ʥ����
            HallowedGround = new (30)
            {
                OtherCheck = b => (float)Service.ClientState.LocalPlayer.CurrentHp / Service.ClientState.LocalPlayer.MaxHp < Service.Configuration.HealthForDyingTank,
            },

            //ʥ��Ļ��
            DivineVeil = new (3540),

            //���ʺ���
            Clemency = new (3541, true, true),

            //��Ԥ
            Intervention = new (7382, true)
            {
                ChoiceFriend = BaseAction.FindAttackedTarget,
            },

            //��ͣ
            Intervene = new (16461, shouldEndSpecial:true),

            //���｣
            Atonement = new (16460)
            {
                BuffsNeed = new [] { ObjectStatus.SwordOath },
            },

            //���꽣
            Expiacion = new (25747),

            //Ӣ��֮��
            BladeofValor = new (25750),

            //����֮��
            BladeofTruth = new (25749),

            //����֮��
            BladeofFaith = new (25748)
            {
                BuffsNeed = new [] { ObjectStatus.ReadyForBladeofFaith },
            },

            //������
            Requiescat = new (7383),

            //����
            Confiteor = new (16459),

            //ʥ��
            HolyCircle = new (16458),

            //ʥ��
            HolySpirit = new (7384),

            //��װ����
            PassageofArms = new (7385),

            //����
            //Cover = new BaseAction(27, true),

            //����
            Sheltron = new (3542);
        //�����ͻ�
        //ShieldBash = new BaseAction(16),
    }
    internal override SortedList<DescType, string> Description => new ()
    {
        {DescType.��������, $"{Actions.Clemency.Action.Name}"},
        {DescType.��Χ����, $"{Actions.DivineVeil.Action.Name}, {Actions.PassageofArms.Action.Name}"},
        {DescType.�������, $"{Actions.Sentinel.Action.Name}, {Actions.Sheltron.Action.Name}"},
        {DescType.�ƶ�, $"{Actions.Intervene.Action.Name}"},
    };
    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //��������
        if (Actions.BladeofValor.ShouldUseAction(out act, lastComboActionID, mustUse: true)) return true;
        if (Actions.BladeofFaith.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.BladeofTruth.ShouldUseAction(out act, lastComboActionID, mustUse: true)) return true;

        //ħ����������
        var status = BaseAction.FindStatusFromSelf(Service.ClientState.LocalPlayer).Where(status => status.StatusId == ObjectStatus.Requiescat);
        if (status != null && status.Count() > 0)
        {
            var s = status.First();
            if ((s.StackCount == 1 || s.RemainingTime < 2.5) &&
                Actions.Confiteor.ShouldUseAction(out act, mustUse: true)) return true;
            if (Actions.HolyCircle.ShouldUseAction(out act)) return true;
            if (Actions.HolySpirit.ShouldUseAction(out act)) return true;
        }

        //AOE ����
        if (Actions.Prominence.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.TotalEclipse.ShouldUseAction(out act, lastComboActionID)) return true;

        //���｣
        if (Actions.Atonement.ShouldUseAction(out act)) return true;

        //��������
        if (Actions.GoringBlade.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.RageofHalone.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.RiotBlade.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.FastBlade.ShouldUseAction(out act, lastComboActionID)) return true;

        //Ͷ��
        if (IconReplacer.Move && MoveAbility(1, out act)) return true;
        if (Actions.ShieldLob.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //��ͣ
        if (Actions.Intervene.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(uint lastComboActionID, out IAction act)
    {
        //���ʺ���
        if (Actions.Clemency.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //ʥ��Ļ��
        if (Actions.DivineVeil.ShouldUseAction(out act)) return true;

        //��װ����
        if (Actions.PassageofArms.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        //ս�ӷ�Ӧ ��Buff
        if (Actions.FightorFlight.ShouldUseAction(out act)) return true;

        //������ת
        if (Actions.CircleofScorn.ShouldUseAction(out act, mustUse: true)) return true;

        //���꽣
        if (Actions.Expiacion.ShouldUseAction(out act, mustUse: true)) return true;

        //������
        if (Service.TargetManager.Target is BattleChara b && BaseAction.FindStatusFromSelf(b, ObjectStatus.GoringBlade, ObjectStatus.BladeofValor) is float[] times &&
            times != null && times.Length > 0 && times.Max() > 10 &&
            Actions.Requiescat.ShouldUseAction(out act, mustUse: true)) return true;

        //���֮��
        if (Actions.SpiritsWithin.ShouldUseAction(out act)) return true;

        //��㹥��
        if (Actions.Intervene.ShouldUseAction(out act) && !IsMoving)
        {
            if (BaseAction.DistanceToPlayer(Actions.Intervene.Target) < 1)
            {
                return true;
            }
        }

        return false;
    }
    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //��ʥ���� ���л�����ˡ�
        if (Actions.HallowedGround.ShouldUseAction(out act)) return true;
        return false;
    }
    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (abilityRemain == 1)
        {

            //Ԥ��������30%��
            if (Actions.Sentinel.ShouldUseAction(out act)) return true;

            //���ڣ�����20%��
            if (GeneralActions.Rampart.ShouldUseAction(out act)) return true;

            if (JobGauge.OathGauge >= 50)
            {
                //����
                if (Actions.Sheltron.ShouldUseAction(out act)) return true;
            }
        }
        //���͹���
        //ѩ��
        if (GeneralActions.Reprisal.ShouldUseAction(out act)) return true;

        //��Ԥ������10%��
        if (!HaveShield && Actions.Intervention.ShouldUseAction(out act)) return true;

        act = null;
        return false;
    }
}
