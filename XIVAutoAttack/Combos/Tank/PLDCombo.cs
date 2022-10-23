using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.CustomCombo;

namespace XIVAutoAttack.Combos.Tank;

internal class PLDCombo : JobGaugeCombo<PLDGauge>
{
    internal override uint JobID => 19;

    internal override bool HaveShield => StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.IronWill);

    private protected override BaseAction Shield => Actions.IronWill;

    protected override bool CanHealSingleSpell => TargetHelper.PartyHealers.Length == 0 && base.CanHealSingleSpell;

    /// <summary>
    /// ��4�˱��ĵ����Ѿ��ۺùֿ���ʹ����ؼ���(���ƶ�������д���3ֻС��)
    /// </summary>
    private static bool CanUseSpellInDungeonsMiddle => TargetHelper.PartyMembers.Length is > 1 and <= 4 && !Target.IsBoss() && !IsMoving
                                                    && TargetFilter.GetObjectInRadius(TargetHelper.HostileTargets, 5).Length >= 3;

    /// <summary>
    /// ��4�˱��ĵ���
    /// </summary>
    private static bool InDungeonsMiddle => TargetHelper.PartyMembers.Length is > 1 and <= 4 && !Target.IsBoss();

    private static bool inOpener = false;
    private static bool openerFinished = false;

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
            GoringBlade = new (3538, isDot:true)
            {
                TargetStatus = new []
                {
                    ObjectStatus.GoringBlade,
                    ObjectStatus.BladeofValor,
                }
            },

            //սŮ��֮ŭ
            RageofHalone = new (21),

            //��Ȩ��
            RoyalAuthority = new (3539),

            //Ͷ��
            ShieldLob = new (24)
            {
                FilterForTarget = b => TargetFilter.ProvokeTarget(b),
            },

            //ս�ӷ�Ӧ
            FightorFlight = new (20)
            {
                OtherCheck = b =>
                {
                    return true;
                }
            },

            //ȫʴն
            TotalEclipse = new (7381),

            //����ն
            Prominence = new (16457),

            //Ԥ��
            Sentinel = new (17)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
                OtherCheck = BaseAction.TankDefenseSelf,
            },

            //������ת
            CircleofScorn = new (23)
            {
                OtherCheck = b =>
                {
                    if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.FightOrFlight)) return true;

                    if (FightorFlight.IsCoolDown) return true;

                    return false;
                }
            },

            //���֮��
            SpiritsWithin = new (29)
            {
                OtherCheck = b =>
                {
                    if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.FightOrFlight)) return true;

                    if (FightorFlight.IsCoolDown) return true;

                    return false;
                }
            },

            //��ʥ����
            HallowedGround = new (30)
            {
                OtherCheck = BaseAction.TankBreakOtherCheck,
            },

            //ʥ��Ļ��
            DivineVeil = new (3540),

            //���ʺ���
            Clemency = new (3541, true, true),

            //��Ԥ
            Intervention = new (7382, true)
            {
                ChoiceTarget = TargetFilter.FindAttackedTarget,
            },

            //��ͣ
            Intervene = new (16461, shouldEndSpecial: true)
            {
                ChoiceTarget = TargetFilter.FindMoveTarget,
            },

            //���｣
            Atonement = new (16460)
            {
                BuffsNeed = new [] { ObjectStatus.SwordOath },
                OtherCheck = b =>
                {
                    if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.FightOrFlight) 
                    && (IsLastWeaponSkill(true, Atonement, RoyalAuthority)
                    && StatusHelper.FindStatusTimeSelfFromSelf(ObjectStatus.FightOrFlight) >= WeaponRemain(2))) return true;

                    var status = StatusHelper.FindStatusFromSelf(LocalPlayer).Where(status => status.StatusId == ObjectStatus.SwordOath);
                    if (status != null && status.Any())
                    {
                        var s = status.First();
                        if (s.StackCount > 1) return true;
                    }
                    
                    return false;
                }
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
            Cover = new BaseAction(27, true)
            {
                ChoiceTarget = TargetFilter.FindAttackedTarget,
            },

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

    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        //ս�ӷ�Ӧ ��Buff
        if (abilityRemain == 1 && Actions.FightorFlight.ShouldUse(out act))
        {
            return true;
        }

        //������
        if (abilityRemain == 1 && CanUseRequiescat(out act)) return true;
  

        act = null;
        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        if (!InBattle)
        {
            inOpener = false;
            openerFinished = false;
        }
        else if (Level > Actions.Requiescat.Level && !openerFinished && !inOpener)
        {
            inOpener = true;
        }

        //��������
        if (Actions.BladeofValor.ShouldUse(out act, lastComboActionID, mustUse: true)) return true;
        if (Actions.BladeofFaith.ShouldUse(out act, mustUse: true)) return true;
        if (Actions.BladeofTruth.ShouldUse(out act, lastComboActionID, mustUse: true)) return true;

        //ħ����������
        if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Requiescat) && (!StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.FightOrFlight)) && LocalPlayer.CurrentMp >= 1000)
        {
            var status = StatusHelper.FindStatusFromSelf(LocalPlayer).Where(status => status.StatusId == ObjectStatus.Requiescat);
            if (status != null && status.Any())
            {
                var s = status.First();
                if (s.StackCount == 1 || (s.RemainingTime < 3 && s.RemainingTime > 0) || LocalPlayer.CurrentMp <= 2000)
                {
                    if (Actions.Confiteor.ShouldUse(out act, mustUse: true)) return true;
                }
                else
                {
                    if (Actions.HolyCircle.ShouldUse(out act)) return true;
                    if (Actions.HolySpirit.ShouldUse(out act)) return true;
                }
            }
        }

        //AOE ����
        if (Actions.Prominence.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.TotalEclipse.ShouldUse(out act, lastComboActionID)) return true;


        //���｣
        if (Actions.Atonement.ShouldUse(out act)) return true;

        //��������
        if (Actions.GoringBlade.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.RageofHalone.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.RiotBlade.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.FastBlade.ShouldUse(out act, lastComboActionID)) return true;

        //Ͷ��
        if (IconReplacer.Move && MoveAbility(1, out act)) return true;
        if (Actions.ShieldLob.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //��ͣ
        if (Actions.Intervene.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(uint lastComboActionID, out IAction act)
    {
        //���ʺ���
        if (Actions.Clemency.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //ʥ��Ļ��
        if (Actions.DivineVeil.ShouldUse(out act)) return true;

        //��װ����
        if (Actions.PassageofArms.ShouldUse(out act)) return true;

        if (GeneralActions.Reprisal.ShouldUse(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        if (inOpener || !Actions.TotalEclipse.ShouldUse(out _))
        {
            if (IsLastWeaponSkill(true, Actions.Confiteor) || (!StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Requiescat) && Actions.Requiescat.IsCoolDown && Actions.Requiescat.RecastTimeRemain <= 59))
            {
                inOpener = false;
                openerFinished = true;
            }
        }

            if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.FightOrFlight) && StatusHelper.FindStatusTimeSelfFromSelf(ObjectStatus.FightOrFlight) <= 19)
            {
                if (IsLastWeaponSkill(true, Actions.FastBlade) && StatusHelper.HaveStatusFromSelf(Target, ObjectStatus.GoringBlade))
                {
        var OpenerStatus = StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.FightOrFlight) && StatusHelper.FindStatusTimeSelfFromSelf(ObjectStatus.FightOrFlight) <= 19 && LastWeaponskill != Actions.FastBlade.ID && StatusHelper.HaveStatusFromSelf(Target, ObjectStatus.GoringBlade);

                    //������ת
                    if (Actions.CircleofScorn.ShouldUse(out act, mustUse: true)) return true;
                    //��ͣ
                    if (Actions.Intervene.ShouldUse(out act) && Actions.Intervene.RecastTimeRemain == 0) return true;
                    //���֮��
                    if (Actions.SpiritsWithin.ShouldUse(out act)) return true;
                    //��ͣ
                    if (Actions.Intervene.ShouldUse(out act,emptyOrSkipCombo: true)) return true;

            //������ת
            if (Actions.CircleofScorn.ShouldUse(out act, mustUse: true))
            {
                return true;
            }

            //���֮��
            if (Actions.SpiritsWithin.ShouldUse(out act))
            {
                return true;
            }


            //���꽣
            //if (Actions.Expiacion.ShouldUse(out act, mustUse: true)) return true;

            //��㹥��
            if (Actions.Intervene.ShouldUse(out act) && !IsMoving)
            {
                if (Actions.Intervene.Target.DistanceToPlayer() < 1)
                {
                    return true;
                }
            }



        //Special Defense.
        if (JobGauge.OathGauge == 100 && Defense(out act) && LocalPlayer.CurrentHp < LocalPlayer.MaxHp) return true;

        act = null;
        return false;
    }
    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //��ʥ���� ���л�����ˡ�
        if (Actions.HallowedGround.ShouldUse(out act)) return true;
        return false;
    }
    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (Defense(out act)) return true;

        if (abilityRemain == 1)
        {
            //Ԥ��������30%��
            if (Actions.Sentinel.ShouldUse(out act)) return true;

            //���ڣ�����20%��
            if (GeneralActions.Rampart.ShouldUse(out act)) return true;
        }
        //���͹���
        //ѩ��
        if (GeneralActions.Reprisal.ShouldUse(out act)) return true;

        //��Ԥ������10%��
        if (!HaveShield && Actions.Intervention.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    /// <summary>
    /// �ж��ܷ�ʹ��ս�ӷ�Ӧ
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool CanUseFightorFlight(out IAction act)
    {
        if (Actions.FightorFlight.ShouldUse(out act))
        {
            //��4�˱�����
            if (InDungeonsMiddle)
            {
                if (CanUseSpellInDungeonsMiddle && !StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Requiescat) 
                    && !StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.ReadyForBladeofFaith)) return true;

                return false;
            }

            //�������ȷ潣��
            if (inOpener && LastWeaponskill == Actions.FastBlade.ID) return true;

            //û������,��ȴ���˾���
            if (!inOpener) return true;

        }

        act = null;
        return false;
    }

    /// <summary>
    /// �ж��ܷ�ʹ�ð�����
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool CanUseRequiescat(out IAction act)
    {
        //������
        if (Actions.Requiescat.ShouldUse(out act, mustUse: true))
        {
            //��4�˱�����
            if (InDungeonsMiddle)
            {
                if (CanUseSpellInDungeonsMiddle) return true;

                return false;
            }

            //��ս��buffʱ��ʣ17������ʱ�ͷ�
            if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.FightOrFlight) && StatusHelper.FindStatusTimeSelfFromSelf(ObjectStatus.FightOrFlight) < 17 && StatusHelper.HaveStatusFromSelf(Target, ObjectStatus.GoringBlade))
            {
                //��������ʱ,��Ȩ�����ͷ�
                if (inOpener && LastWeaponskill == Actions.RoyalAuthority.ID) return true;

                //û������ʱ��
                if (!inOpener) return true;
            }
        }
                       
        act = null;
        return false;
    }
    private bool Defense(out IAction act)
    {
        act = null;
        if (JobGauge.OathGauge < 50) return false;

        if (HaveShield)
        {
            //����
            if (Actions.Sheltron.ShouldUse(out act)) return true;
        }
        else
        {
            if(Actions.Cover.ShouldUse(out act)) return true;
        }

        return false;
    }
}
