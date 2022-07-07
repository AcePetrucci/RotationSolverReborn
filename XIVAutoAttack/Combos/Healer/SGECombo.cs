using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Configuration;

namespace XIVAutoAttack.Combos.Healer;

internal class SGECombo : CustomComboJob<SGEGauge>
{
    internal override uint JobID => 40;

    private protected override BaseAction Raise => Actions.Egeiro;
    protected override bool CanHealSingleSpell => Config.GetBoolByName("GCDHeal");
    protected override bool CanHealAreaSpell => Config.GetBoolByName("GCDHeal");
    internal struct Actions
    {
        public static readonly BaseAction
            //����
            Egeiro = new BaseAction(24287),

            //עҩ
            Dosis = new BaseAction(24283),

            //����
            Phlegma = new BaseAction(24289),
            //����2
            Phlegma2 = new BaseAction(24307),
            //����3
            Phlegma3 = new BaseAction(24313),

            //���
            Diagnosis = new BaseAction(24284, true),

            //�Ĺ�
            Kardia = new BaseAction(24285, true)
            {
                ChoiceFriend = Targets =>
                {
                    var targets = TargetHelper.GetJobCategory(Targets, Role.����);
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
            Prognosis = new BaseAction(24286, true, shouldEndSpecial: true),

            //����
            Physis = new BaseAction(24288, true),

            //����2
            Physis2 = new BaseAction(24302, true),

            ////����
            Eukrasia = new BaseAction(24290)
            {
                OtherCheck = b => !JobGauge.Eukrasia,
            },

            //����
            Soteria = new BaseAction(24294, true)
            {
                ChoiceFriend = Targets =>
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
            Icarus = new BaseAction(24295, shouldEndSpecial: true)
            {
                ChoiceFriend = BaseAction.FindMoveTarget,
            },

            //������֭
            Druochole = new BaseAction(24296, true),

            //ʧ��
            Dyskrasia = new BaseAction(24297),

            //�����֭
            Kerachole = new BaseAction(24298, true),

            //������֭
            Ixochole = new BaseAction(24299, true),

            //�
            Zoe = new BaseAction(24300),

            //��ţ��֭
            Taurochole = new BaseAction(24303, true)
            {
                ChoiceFriend = BaseAction.FindBeAttacked,
            },

            //����
            Toxikon = new BaseAction(24304),

            //��Ѫ
            Haima = new BaseAction(24305, true)
            {
                ChoiceFriend = BaseAction.FindBeAttacked,
            },

            //����
            Rhizomata = new BaseAction(24309),

            //������
            Holos = new BaseAction(24310, true),

            //����Ѫ
            Panhaima = new BaseAction(24311, true),

            //���
            Krasis = new BaseAction(24317, true),

            //�����Ϣ
            Pneuma = new BaseAction(24318);
    }

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("GCDHeal", false, "�Զ���GCD��");
    }

    internal override SortedList<DescType, string> Description => new SortedList<DescType, string>()
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
        if (Actions.Diagnosis.ShouldUseAction(out act))
        {
            if (Actions.Diagnosis.Target.StatusList.Select(s => s.StatusId).Intersect(new uint[]
            {
                ObjectStatus.EukrasianDiagnosis,
                ObjectStatus.EukrasianPrognosis,
                ObjectStatus.Galvanize,
            }).Count() > 0) return false;

            //����
            if (Actions.Eukrasia.ShouldUseAction(out act)) return true;

            act = Actions.Diagnosis;
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
        if (Actions.Prognosis.ShouldUseAction(out act))
        {
            if (Actions.Diagnosis.Target.StatusList.Select(s => s.StatusId).Intersect(new uint[]
            {
                ObjectStatus.EukrasianDiagnosis,
                ObjectStatus.EukrasianPrognosis,
                ObjectStatus.Galvanize,
            }).Count() > 0) return false;

            //����
            if (Actions.Eukrasia.ShouldUseAction(out act)) return true;

        }

        act = null;
        return false;
    }


    private protected override bool DefenseAreaGCD(uint lastComboActionID, out IAction act)
    {
        //Ԥ��
        if (Actions.Prognosis.ShouldUseAction(out act) && JobGauge.Eukrasia) return true;
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

        if (JobGauge.Addersting > 0)
        {
            //����
            if (Actions.Toxikon.ShouldUseAction(out act, mustUse: true)) return true;
        }
        var level = Service.ClientState.LocalPlayer.Level;
        //����
        if (Actions.Phlegma3.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
        if (level < Actions.Phlegma3.Level && Actions.Phlegma2.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
        if (level < Actions.Phlegma2.Level && Actions.Phlegma.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;

        //ʧ��
        if (Actions.Dyskrasia.ShouldUseAction(out act)) return true;

        Actions.Dosis.ShouldUseAction(out _);
        if(Actions.Dosis.Target != null)
        {
            var times = BaseAction.FindStatusFromSelf(Actions.Dosis.Target,
                new ushort[] { ObjectStatus.EukrasianDosis, ObjectStatus.EukrasianDosis2, ObjectStatus.EukrasianDosis3 });
            if (times.Length == 0 || times.Max() < 3)
            {
                //����Dot
                if (Actions.Eukrasia.ShouldUseAction(out act)) return true;
            }
        }

        //עҩ
        if (Actions.Dosis.ShouldUseAction(out act)) return true;

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
