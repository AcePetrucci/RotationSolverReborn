using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Helpers.TargetHelper;

namespace XIVAutoAttack.Combos.Healer;

internal class SGECombo : JobGaugeCombo<SGEGauge>
{
    internal override uint JobID => 40;
    internal static byte level => Service.ClientState.LocalPlayer!.Level;

    private protected override BaseAction Raise => Actions.Egeiro;
    protected override bool CanHealSingleSpell => base.CanHealSingleSpell && (Config.GetBoolByName("GCDHeal") || TargetUpdater.PartyHealers.Length < 2);
    protected override bool CanHealAreaSpell => base.CanHealAreaSpell && (Config.GetBoolByName("GCDHeal") || TargetUpdater.PartyHealers.Length < 2);
    internal struct Actions
    {
        public static readonly BaseAction
            //����
            Egeiro = new(24287),

            //עҩ
            Dosis = new(24283),

            //����עҩ
            EukrasianDosis = new (24283, isDot: true)
            {
                TargetStatus = new ushort[] 
                { 
                    ObjectStatus.EukrasianDosis, 
                    ObjectStatus.EukrasianDosis2, 
                    ObjectStatus.EukrasianDosis3
                },
            },

            //����
            Phlegma = new(24289),
            //����2
            Phlegma2 = new(24307),
            //����3
            Phlegma3 = new(24313),

            //���
            Diagnosis = new(24284, true),

            //�Ĺ�
            Kardia = new(24285, true)
            {
                BuffsProvide = new ushort[] { ObjectStatus.Kardia },
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
            Prognosis = new(24286, true, shouldEndSpecial: true),

            //����
            Physis = new(24288, true),

            //����2
            Physis2 = new(24302, true),

            //����
            Eukrasia = new(24290)
            {
                OtherCheck = b => !JobGauge.Eukrasia,
            },

            //����
            Soteria = new(24294, true)
            {
                ChoiceTarget = Targets =>
                {
                    foreach (var friend in Targets)
                    {
                        if (friend.HaveStatus(ObjectStatus.Kardion))
                        {
                            return friend;
                        }
                    }
                    return null;
                },
                OtherCheck = b => b.GetHealthRatio() < 0.7,
            },

            //����
            Icarus = new(24295, shouldEndSpecial: true)
            {
                ChoiceTarget = TargetFilter.FindMoveTarget,
            },

            //������֭
            Druochole = new(24296, true)
            {
                OtherCheck = b => JobGauge.Addersgall > 0 && HealHelper.SingleHeal(b, 600, 0.9, 0.85),
            },

            //ʧ��
            Dyskrasia = new(24297),

            //�����֭
            Kerachole = new(24298, true)
            {
                OtherCheck = b => JobGauge.Addersgall > 0,
            },

            //������֭
            Ixochole = new(24299, true)
            {
                OtherCheck = b => JobGauge.Addersgall > 0,
            },

            //�
            Zoe = new(24300),

            //��ţ��֭
            Taurochole = new(24303, true)
            {
                ChoiceTarget = TargetFilter.FindAttackedTarget,
                OtherCheck = b => JobGauge.Addersgall > 0,
            },

            //����
            Toxikon = new(24304),

            //��Ѫ
            Haima = new(24305, true)
            {
                ChoiceTarget = TargetFilter.FindAttackedTarget,
            },

            //�������
            EukrasianDiagnosis = new(24291, true)
            {
                ChoiceTarget = TargetFilter.FindAttackedTarget,
            },

            //����Ԥ��
            EukrasianPrognosis = new(24292, true)
            {
                ChoiceTarget = TargetFilter.FindAttackedTarget,
            },

            //����
            Rhizomata = new(24309),

            //������
            Holos = new(24310, true),

            //����Ѫ
            Panhaima = new(24311, true),

            //���
            Krasis = new(24317, true),

            //�����Ϣ
            Pneuma = new(24318),

            //����
            Pepsis = new(24301, true)
            {
                OtherCheck = b =>
                {
                    foreach (var chara in TargetUpdater.PartyMembers)
                    {
                        if (chara.StatusList.Select(s => s.StatusId).Intersect(new uint[]
                        {
                            ObjectStatus.EukrasianDiagnosis,
                            ObjectStatus.EukrasianPrognosis,
                        }).Any()
                        && StatusHelper.FindStatusTime(b, ObjectStatus.EukrasianDiagnosis, ObjectStatus.EukrasianPrognosis) < 3
                        && chara.GetHealthRatio() < 0.9) return true;
                    }

                    return false;
                },
            };

    }

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("GCDHeal", false, "�Զ���GCD��");
    }

    internal override SortedList<DescType, string> Description => new()
    {
        {DescType.��Χ����, $"GCD: {Actions.Prognosis.Action.Name}\n                     ����: {Actions.Holos.Action.Name}, {Actions.Ixochole.Action.Name}, {Actions.Physis2.Action.Name}, {Actions.Physis.Action.Name}"},
        {DescType.��������, $"GCD: {Actions.Diagnosis.Action.Name}\n                     ����: {Actions.Druochole.Action.Name}"},
        {DescType.��Χ����, $"{Actions.Panhaima.Action.Name}, {Actions.Kerachole.Action.Name}, {Actions.Prognosis.Action.Name}"},
        {DescType.�������, $"GCD: {Actions.Diagnosis.Action.Name}\n                     ����: {Actions.Haima.Action.Name}, {Actions.Taurochole.Action.Name}"},
        {DescType.�ƶ�, $"{Actions.Icarus.Action.Name}��Ŀ��Ϊ����н�С��30������ԶĿ�ꡣ"},
    };
    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        act = null!;
        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (base.EmergercyAbility(abilityRemain, nextGCD, out act)) return true;

        //�¸�������
        if (nextGCD.IsAnySameAction(false, Actions.Pneuma , Actions.EukrasianDiagnosis, 
            Actions.EukrasianPrognosis , Actions.Diagnosis , Actions.Prognosis))
        {
            //�
            if (Actions.Zoe.ShouldUse(out act)) return true;
        }

        if (nextGCD == Actions.Diagnosis)
        {
            //���
            if (Actions.Krasis.ShouldUse(out act)) return true;
        }

        act = null;
        return false;

    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {

        if (JobGauge.Addersgall == 0)
        {
            //��Ѫ
            if (Actions.Haima.ShouldUse(out act)) return true;
        }

        //��ţ��֭
        if (Actions.Taurochole.ShouldUse(out act)) return true;

        act = null!;
        return false;
    }

    private protected override bool DefenseSingleGCD(uint lastComboActionID, out IAction act)
    {
        //���
        if (Actions.EukrasianDiagnosis.ShouldUse(out act))
        {
            if (Actions.EukrasianDiagnosis.Target.StatusList.Select(s => s.StatusId).Intersect(new uint[]
            {
                ObjectStatus.EukrasianDiagnosis,
                ObjectStatus.EukrasianPrognosis,
                ObjectStatus.Galvanize,
            }).Count() > 0) return false;

            //����
            if (Actions.Eukrasia.ShouldUse(out act)) return true;

            act = Actions.EukrasianDiagnosis;
            return true;
        }

        act = null!;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //����Ѫ
        if (JobGauge.Addersgall == 0 && TargetUpdater.PartyMembersAverHP < 0.7)
        {
            if (Actions.Panhaima.ShouldUse(out act)) return true;
        }

        //�����֭
        if (Actions.Kerachole.ShouldUse(out act)) return true;

        //������
        if (Actions.Holos.ShouldUse(out act)) return true;

        act = null!;
        return false;
    }

    private protected override bool DefenseAreaGCD(uint lastComboActionID, out IAction act)
    {
        //Ԥ��
        if (Actions.EukrasianPrognosis.ShouldUse(out act))
        {
            if (Actions.EukrasianPrognosis.Target.StatusList.Select(s => s.StatusId).Intersect(new uint[]
            {
                ObjectStatus.EukrasianDiagnosis,
                ObjectStatus.EukrasianPrognosis,
                ObjectStatus.Galvanize,
            }).Count() > 0) return false;

            //����
            if (Actions.Eukrasia.ShouldUse(out act)) return true;

            act = Actions.EukrasianPrognosis;
            return true;
        }

        act = null!;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //����
        if (Actions.Icarus.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool GeneralAbility(byte abilityRemain, out IAction act)
    {
        //�Ĺ�
        if (Actions.Kardia.ShouldUse(out act)) return true;

        //����
        if (JobGauge.Addersgall == 0 && Actions.Rhizomata.ShouldUse(out act)) return true;

        //����
        if (Actions.Soteria.ShouldUse(out act)) return true;

        //����
        if (Actions.Pepsis.ShouldUse(out act)) return true;

        act = null!;
        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //����
        if (JobGauge.Addersting == 3 && Actions.Toxikon.ShouldUse(out act, mustUse: true)) return true;

        var level = Level;
        //����
        if (Actions.Phlegma3.ShouldUse(out act, mustUse: Actions.Phlegma3.RecastTimeRemain < 4, emptyOrSkipCombo: true)) return true;
        if (level < Actions.Phlegma3.Level && Actions.Phlegma2.ShouldUse(out act, mustUse: Actions.Phlegma2.RecastTimeRemain < 4, emptyOrSkipCombo: true)) return true;
        if (level < Actions.Phlegma2.Level && Actions.Phlegma.ShouldUse(out act, mustUse: Actions.Phlegma.RecastTimeRemain < 4, emptyOrSkipCombo: true)) return true;

        //ʧ��
        if (Actions.Dyskrasia.ShouldUse(out act)) return true;

        if(Actions.EukrasianDosis.ShouldUse(out var enAct))
        {
            //����Dot
            if (Actions.Eukrasia.ShouldUse(out act)) return true;
            act = enAct;
            return true;
        }
        else if (JobGauge.Eukrasia)
        {
            if (DefenseAreaGCD(lastComboActionID, out act)) return true;
            if (DefenseSingleGCD(lastComboActionID, out act)) return true;
        }

        //עҩ
        if (Actions.Dosis.ShouldUse(out act)) return true;

        //����
        if (Actions.Phlegma3.ShouldUse(out act, mustUse: true)) return true;
        if (level < Actions.Phlegma3.Level && Actions.Phlegma2.ShouldUse(out act, mustUse: true)) return true;
        if (level < Actions.Phlegma2.Level && Actions.Phlegma.ShouldUse(out act, mustUse: true)) return true;

        //����
        if (JobGauge.Addersting > 0 && Actions.Toxikon.ShouldUse(out act, mustUse: true)) return true;

        //��ս��Tˢ�����ζ���
        if (!InBattle)
        {
            var tank = TargetUpdater.PartyTanks;
            if (tank.Length == 1 && Actions.EukrasianDiagnosis.Target == tank.First() && Actions.EukrasianDiagnosis.ShouldUse(out act))
            {
                if (tank.First().StatusList.Select(s => s.StatusId).Intersect(new uint[]
                {
                ObjectStatus.EukrasianDiagnosis,
                ObjectStatus.EukrasianPrognosis,
                ObjectStatus.Galvanize,
                }).Any()) return false;

                //����
                if (Actions.Eukrasia.ShouldUse(out act)) return true;

                act = Actions.EukrasianDiagnosis;
                return true;
            }
            if (Actions.Eukrasia.ShouldUse(out act)) return true;
        }

        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        //��ţ��֭
        if (Actions.Taurochole.ShouldUse(out act)) return true;

        //������֭
        if (Actions.Druochole.ShouldUse(out act)) return true;

        //����Դ����ʱ���뷶Χ���ƻ���ѹ��
        var tank = TargetUpdater.PartyTanks;
        var isBoss = Actions.Dosis.Target.IsBoss();
        if (JobGauge.Addersgall == 0 && tank.Length == 1 && tank.Any(t => t.GetHealthRatio() < 0.6f) && !isBoss)
        {
            //������
            if (Actions.Holos.ShouldUse(out act)) return true;

            //����2
            if (Actions.Physis2.ShouldUse(out act)) return true;
            //����
            if (Level < Actions.Physis2.Level && Actions.Physis.ShouldUse(out act)) return true;

            //����Ѫ
            if (Actions.Panhaima.ShouldUse(out act)) return true;
        }

        act = null!;
        return false;
    }

    private protected override bool HealSingleGCD(uint lastComboActionID, out IAction act)
    {
        //���
        if (Actions.EukrasianDiagnosis.ShouldUse(out act))
        {
            if (Actions.EukrasianDiagnosis.Target.StatusList.Select(s => s.StatusId).Intersect(new uint[]
            {
                //�������
                ObjectStatus.EukrasianDiagnosis,
                ObjectStatus.EukrasianPrognosis,
                ObjectStatus.Galvanize,
            }).Count() > 0)
            {
                if (Actions.Diagnosis.ShouldUse(out act)) return true;
            }

            //����
            if (Actions.Eukrasia.ShouldUse(out act)) return true;

            act = Actions.EukrasianDiagnosis;
            return true;
        }

        //���
        if (Actions.Diagnosis.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool HealAreaGCD(uint lastComboActionID, out IAction act)
    {
        if (TargetUpdater.PartyMembersAverHP < 0.55f)
        {
            //�����Ϣ
            if (Actions.Pneuma.ShouldUse(out act, mustUse: true)) return true;
        }

        if (Actions.EukrasianPrognosis.ShouldUse(out act))
        {
            if (Actions.EukrasianPrognosis.Target.StatusList.Select(s => s.StatusId).Intersect(new uint[]
            {
                //�������
                ObjectStatus.EukrasianDiagnosis,
                ObjectStatus.EukrasianPrognosis,
                ObjectStatus.Galvanize,
            }).Count() > 0)
            {
                if (Actions.Prognosis.ShouldUse(out act)) return true;
            }

            //����
            if (Actions.Eukrasia.ShouldUse(out act)) return true;

            act = Actions.EukrasianPrognosis;
            return true;
        }

        //Ԥ��
        if (Actions.Prognosis.ShouldUse(out act)) return true;
        return false;
    }
    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        var level = Level;

        //�����֭
        if (Actions.Kerachole.ShouldUse(out act) && level >= 78) return true;

        //����2
        if (Actions.Physis2.ShouldUse(out act)) return true;
        //����
        if (level < Actions.Physis2.Level && Actions.Physis.ShouldUse(out act)) return true;

        //������
        if (Actions.Holos.ShouldUse(out act) && TargetUpdater.PartyMembersAverHP < 0.65f) return true;

        //������֭
        if (Actions.Ixochole.ShouldUse(out act)) return true;

        return false;
    }
}
