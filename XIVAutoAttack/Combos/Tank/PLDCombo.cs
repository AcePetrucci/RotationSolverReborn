using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using System.Linq;
using System.Numerics;

namespace XIVComboPlus.Combos;

internal class PLDCombo : CustomComboJob<PLDGauge>
{
    internal override uint JobID => 19;

    internal override  bool HaveShield => BaseAction.HaveStatusSelfFromSelf(ObjectStatus.IronWill);

    private protected override BaseAction Shield => Actions.IronWill;

    protected override bool CanHealSingleSpell => false;
    protected override bool CanHealAreaSpell => false;

    internal struct Actions
    {
        public static readonly BaseAction
            //��������
            IronWill =  new BaseAction(28, shouldEndSpecial:true),

            //�ȷ潣
            FastBlade = new BaseAction(9),

            //���ҽ�
            RiotBlade = new BaseAction(15),

            //��Ѫ��
            GoringBlade = new BaseAction(3538)
            {
                TargetStatus = new ushort[]
                {
                    ObjectStatus.GoringBlade,
                    ObjectStatus.BladeofValor,
                }
            },

            //սŮ��֮ŭ
            RageofHalone = new BaseAction(21),

            //Ͷ��
            ShieldLob = new BaseAction(24)
            {
                FilterForHostile = b => BaseAction.ProvokeTarget(b, out _),
            },

            //ս�ӷ�Ӧ
            FightorFlight = new BaseAction(20),

            //ȫʴն
            TotalEclipse = new BaseAction(7381),

            //����ն
            Prominence = new BaseAction(16457),

            //Ԥ��
            Sentinel = new BaseAction(17)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
            },

            //������ת
            CircleofScorn = new BaseAction(23),

            //���֮��
            SpiritsWithin = new BaseAction(29),

            //��ʥ����
            HallowedGround = new BaseAction(30)
            {
                OtherCheck = b => (float)Service.ClientState.LocalPlayer.CurrentHp / Service.ClientState.LocalPlayer.MaxHp < Service.Configuration.HealthForDyingTank,
            },

            //ʥ��Ļ��
            DivineVeil = new BaseAction(3540),

            //���ʺ���
            Clemency = new BaseAction(3541, true, true),

            //��Ԥ
            Intervention = new BaseAction(7382, true)
            {
                ChoiceFriend = BaseAction.FindAttackedTarget,
            },

            //��ͣ
            Intervene = new BaseAction(16461),

            //���｣
            Atonement = new BaseAction(16460)
            {
                BuffsNeed = new ushort[] { ObjectStatus.SwordOath },
            },

            //���꽣
            Expiacion = new BaseAction(25747),

            //Ӣ��֮��
            BladeofValor = new BaseAction(25750),

            //����֮��
            BladeofTruth = new BaseAction(25749),

            //����֮��
            BladeofFaith = new BaseAction(25748),

            //������
            Requiescat = new BaseAction(7383),

            //����
            Confiteor = new BaseAction(16459),

            //ʥ��
            HolyCircle = new BaseAction(16458),

            //ʥ��
            HolySpirit = new BaseAction(7384),

            //��װ����
            PassageofArms = new BaseAction(7385),

            //����
            //Cover = new BaseAction(27, true),

            //����
            Sheltron = new BaseAction(3542);
        //�����ͻ�
        //ShieldBash = new BaseAction(16),
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out BaseAction act)
    {
        //��������
        if (Actions.BladeofValor.ShouldUseAction(out act, lastComboActionID, mustUse:true)) return true;
        if (Actions.BladeofTruth.ShouldUseAction(out act, lastComboActionID, mustUse: true)) return true;
        if (Actions.BladeofFaith.ShouldUseAction(out act, lastComboActionID, mustUse: true)) return true;

        //ħ����������
        var status = BaseAction.FindStatusFromSelf(Service.ClientState.LocalPlayer).Where(status => status.StatusId == ObjectStatus.Requiescat);
        if(status != null && status.Count() > 0)
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

    private protected override bool MoveAbility(byte abilityRemain, out BaseAction act)
    {
        //��ͣ
        if (Actions.Intervene.ShouldUseAction(out act, emptyOrSkipCombo:true)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(uint lastComboActionID, out BaseAction act)
    {
        //���ʺ���
        if (Actions.Clemency.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out BaseAction act)
    {
        //ʥ��Ļ��
        if (Actions.DivineVeil.ShouldUseAction(out act)) return true;

        //��װ����
        if (Actions.PassageofArms.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out BaseAction act)
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
        if (Actions.Intervene.ShouldUseAction(out act) && ! IsMoving)
        {
            if (BaseAction.DistanceToPlayer(Actions.Intervene.Target, true) < 1)
            {
                return true;
            }
        }

        return false;
    }
    private protected override bool EmergercyAbility(byte abilityRemain, BaseAction nextGCD, out BaseAction act)
    {
        //��ʥ���� ���л�����ˡ�
        if (Actions.HallowedGround.ShouldUseAction(out act)) return true;
        return false;
    }
    private protected override bool DefenceSingleAbility(byte abilityRemain, out BaseAction act)
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

            //���͹���
            //ѩ��
            if (GeneralActions.Reprisal.ShouldUseAction(out act)) return true;
        }

        //��Ԥ������10%��
        if (!HaveShield && Actions.Intervention.ShouldUseAction(out act)) return true;

        act = null;
        return false;
    }
}
