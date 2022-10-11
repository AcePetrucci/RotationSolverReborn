using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;

namespace XIVAutoAttack.Combos.Healer;

internal class SGECombo : JobGaugeCombo<SGEGauge>
{
    internal override uint JobID => 40;

    private protected override BaseAction Raise => Actions.Egeiro;
    protected override bool CanHealSingleSpell => base.CanHealSingleSpell && (Config.GetBoolByName("GCDHeal") || TargetHelper.PartyHealers.Length < 2);
    protected override bool CanHealAreaSpell => base .CanHealAreaSpell && ( Config.GetBoolByName("GCDHeal") || TargetHelper.PartyHealers.Length < 2);
    internal struct Actions
    {
        public static readonly BaseAction
            //����
            Egeiro = new (24287),

            //עҩ
            Dosis = new (24283),

            //����
            Phlegma = new (24289),
            //����2
            Phlegma2 = new (24307),
            //����3
            Phlegma3 = new (24313),

            //���
            Diagnosis = new (24284, true),

            //�Ĺ�
            Kardia = new(24285, true)
            {
                BuffsProvide = new ushort[] {ObjectStatus.Kardia},
                ChoiceTarget = Targets =>
                {
                    var targets = TargetFilter.GetJobCategory(Targets, Role.����);
                    targets = targets.Length == 0 ? Targets : targets;

                    if (targets.Length == 0) return null;

                    foreach (var tar in targets)
                    {
                        if (tar.TargetObject?.TargetObject?.ObjectId == tar.ObjectId)
                        {
                            return tar;
                        }
                    }

                    return targets[0];
                },
                OtherCheck = b =>
                {
                    foreach (var status in b.StatusList)
                    {
                        if (status.SourceID == Service.ClientState.LocalPlayer.ObjectId
                            && status.StatusId == ObjectStatus.Kardion)
                        {
                            return false;
                        }
                    }
                    return true;
                },
            },

            //Ԥ��
            Prognosis = new (24286, true, shouldEndSpecial: true),

            //����
            Physis = new (24288, true),

            //����2
            Physis2 = new (24302, true),

            //����
            Eukrasia = new (24290)
            {
                OtherCheck = b => !JobGauge.Eukrasia,
            },

            //����
            Soteria = new (24294, true)
            {
                ChoiceTarget = Targets =>
                {
                    foreach (var friend in Targets)
                    {
                        var statuses = friend.StatusList.Select(status => status.StatusId);
                        if (statuses.Contains(ObjectStatus.Kardion))
                        {
                            return friend;
                        }
                    }
                    return null;
                },
                OtherCheck = b => (float)b.CurrentHp / b.MaxHp < 0.7,
            },

            //����
            Icarus = new (24295, shouldEndSpecial: true)
            {
                ChoiceTarget = TargetFilter.FindMoveTarget,
            },

            //������֭
            Druochole = new (24296, true),

            //ʧ��
            Dyskrasia = new (24297),

            //�����֭
            Kerachole = new (24298, true),

            //������֭
            Ixochole = new (24299, true),

            //�
            Zoe = new (24300),

            //��ţ��֭
            Taurochole = new (24303, true)
            {
                ChoiceTarget = TargetFilter.FindAttackedTarget,
            },

            //����
            Toxikon = new (24304),

            //��Ѫ
            Haima = new (24305, true)
            {
                ChoiceTarget = TargetFilter.FindAttackedTarget,
            },

            //�������
            EukrasianDiagnosis = new (24291, true) 
            {
                ChoiceTarget = TargetFilter.FindAttackedTarget,
            },

            //�������
            EukrasianPrognosis = new (24292, true)
            {
                ChoiceTarget = TargetFilter.FindAttackedTarget,
            },

            //����
            Rhizomata = new (24309),

            //������
            Holos = new (24310, true),

            //����Ѫ
            Panhaima = new (24311, true),

            //���
            Krasis = new (24317, true),

            //�����Ϣ
            Pneuma = new (24318);
    }

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("GCDHeal", false, "�Զ���GCD��");
    }

    internal override SortedList<DescType, string> Description => new ()
    {
        {DescType.��Χ����, $"GCD: {Actions.Prognosis.Action.Name}\n                     ����: {Actions.Holos.Action.Name}, {Actions.Ixochole.Action.Name}, {Actions.Physis2.Action.Name}, {Actions.Physis.Action.Name}"},
        {DescType.��������, $"GCD: {Actions.Diagnosis.Action.Name}\n                     ����: {Actions.Druochole.Action.Name}"},
        {DescType.��Χ����, $"{Actions.Panhaima.Action.Name}, {Actions.Kerachole.Action.Name}, {Actions.Prognosis.Action.Name}"},
        {DescType.�������, $"GCD: {Actions.Diagnosis.Action.Name}\n                     ����: {Actions.Haima.Action.Name}, {Actions.Taurochole.Action.Name}"},
        {DescType.�ƶ�, $"{Actions.Icarus.Action.Name}��Ŀ��Ϊ����н�С��30������ԶĿ�ꡣ"},
    };
    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        act = null;
        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (base.EmergercyAbility(abilityRemain, nextGCD, out act)) return true;

        if (nextGCD.ID == Actions.Diagnosis.ID ||
            nextGCD.ID == Actions.Prognosis.ID)
        {
            //�
            if (Actions.Zoe.ShouldUseAction(out act)) return true;
        }

        if (nextGCD.ID == Actions.Diagnosis.ID)
        {
            //���
            if (Actions.Krasis.ShouldUseAction(out act)) return true;
        }

        act = null;
        return false;

    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        //��Ѫ
        if (Actions.Haima.ShouldUseAction(out act)) return true;

        if (JobGauge.Addersgall > 0)
        {
            //��ţ��֭
            if (Actions.Taurochole.ShouldUseAction(out act)) return true;
        }

        return false;
    }

    private protected override bool DefenseSingleGCD(uint lastComboActionID, out IAction act)
    {
        //���
        if (Actions.EukrasianDiagnosis.ShouldUseAction(out act))
        {
            if (Actions.EukrasianDiagnosis.Target.StatusList.Select(s => s.StatusId).Intersect(new uint[]
            {
                ObjectStatus.EukrasianDiagnosis,
                ObjectStatus.EukrasianPrognosis,
                ObjectStatus.Galvanize,
            }).Count() > 0) return false;

            //����
            if (Actions.Eukrasia.ShouldUseAction(out act)) return true;

            act = Actions.EukrasianDiagnosis;
            return true;
        }

        act = null;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //����Ѫ
        if (Actions.Panhaima.ShouldUseAction(out act)) return true;

        if (JobGauge.Addersgall > 0)
        {
            //�����֭
            if (Actions.Kerachole.ShouldUseAction(out act)) return true;
        }

        //Ԥ��
        if (Actions.EukrasianPrognosis.ShouldUseAction(out act))
        {
            if (Actions.EukrasianPrognosis.Target.StatusList.Select(s => s.StatusId).Intersect(new uint[]
            {
                ObjectStatus.EukrasianDiagnosis,
                ObjectStatus.EukrasianPrognosis,
                ObjectStatus.Galvanize,
            }).Count() > 0) return false;

            //����
            if (Actions.Eukrasia.ShouldUseAction(out act)) return true;

            act = Actions.EukrasianPrognosis;
            return true;
        }

        act = null;
        return false;
    }

    private protected override bool DefenseAreaGCD(uint lastComboActionID, out IAction act)
    {
        //Ԥ��
        if (Actions.EukrasianPrognosis.ShouldUseAction(out act))
        {
            if (Actions.EukrasianPrognosis.Target.StatusList.Select(s => s.StatusId).Intersect(new uint[]
            {
                ObjectStatus.EukrasianDiagnosis,
                ObjectStatus.EukrasianPrognosis,
                ObjectStatus.Galvanize,
            }).Count() > 0) return false;

            //����
            if (Actions.Eukrasia.ShouldUseAction(out act)) return true;

            act = Actions.EukrasianPrognosis;
            return true;
        }

        act = null;
        return false;
    }
    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {

        if (JobGauge.Addersgall > 1)
        {
            //������֭
            if (Actions.Druochole.ShouldUseAction(out act)) return true;
        }

        act = null;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //����
        if (Actions.Icarus.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool GeneralAbility(byte abilityRemain, out IAction act)
    {
        //�Ĺ�
        if (Actions.Kardia.ShouldUseAction(out act)) return true;



        //����
        if (JobGauge.Addersgall < 2 && Actions.Rhizomata.ShouldUseAction(out act)) return true;

        if (Actions.Soteria.ShouldUseAction(out act)) return true;
        act = null;
        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //�����Ϣ
        if (Actions.Pneuma.ShouldUseAction(out act, mustUse: true)) return true;

        //����
        if (JobGauge.Addersting == 3 && Actions.Toxikon.ShouldUseAction(out act, mustUse: true)) return true;

        var level = Service.ClientState.LocalPlayer.Level;
        //����
        if (Actions.Phlegma3.ShouldUseAction(out act, mustUse : Actions.Phlegma3.RecastTimeRemain < 4, emptyOrSkipCombo: true)) return true;
        if (level < Actions.Phlegma3.Level && Actions.Phlegma2.ShouldUseAction(out act, mustUse: Actions.Phlegma2.RecastTimeRemain < 4, emptyOrSkipCombo: true)) return true;
        if (level < Actions.Phlegma2.Level && Actions.Phlegma.ShouldUseAction(out act, mustUse: Actions.Phlegma.RecastTimeRemain < 4, emptyOrSkipCombo: true)) return true;

        //ʧ��
        if (Actions.Dyskrasia.ShouldUseAction(out act)) return true;

        Actions.Dosis.ShouldUseAction(out _);
        var times = StatusHelper.FindStatusFromSelf(Actions.Dosis.Target,
            new ushort[] { ObjectStatus.EukrasianDosis, ObjectStatus.EukrasianDosis2, ObjectStatus.EukrasianDosis3 });
        if (times.Length == 0 || times.Max() < 3)
        {
            if (Actions.Dosis.Target != null)
            {
                //����Dot
                if (Actions.Eukrasia.ShouldUseAction(out act)) return true;
            }
        }
        else if (JobGauge.Eukrasia)
        {
            if (DefenseAreaGCD(lastComboActionID, out act)) return true;
            if (DefenseSingleGCD(lastComboActionID, out act)) return true;
        }


        //עҩ
        if (Actions.Dosis.ShouldUseAction(out act)) return true;

        //����
        if (JobGauge.Addersting > 0 && Actions.Toxikon.ShouldUseAction(out act, mustUse: true)) return true;

        //����
        if (Actions.Phlegma3.ShouldUseAction(out act, mustUse: true)) return true;
        if (level < Actions.Phlegma3.Level && Actions.Phlegma2.ShouldUseAction(out act, mustUse: true)) return true;
        if (level < Actions.Phlegma2.Level && Actions.Phlegma.ShouldUseAction(out act, mustUse: true)) return true;

        return false;
    }
    private protected override bool HealSingleGCD(uint lastComboActionID, out IAction act)
    {
        //���
        if (Actions.Diagnosis.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool HealAreaGCD(uint lastComboActionID, out IAction act)
    {
        //Ԥ��
        if (Actions.Prognosis.ShouldUseAction(out act)) return true;
        return false;
    }
    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        //������
        if (Actions.Holos.ShouldUseAction(out act)) return true;

        if (JobGauge.Addersgall > 0)
        {
            //������֭
            if (Actions.Ixochole.ShouldUseAction(out act)) return true;
        }
        //����2
        if (Actions.Physis2.ShouldUseAction(out act)) return true;
        //����
        if (Service.ClientState.LocalPlayer.Level < Actions.Physis2.Level && Actions.Physis.ShouldUseAction(out act)) return true;
        return false;
    }
}
