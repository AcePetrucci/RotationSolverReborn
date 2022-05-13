using Dalamud.Game.ClientState.JobGauge.Types;
using System.Linq;
namespace XIVComboPlus.Combos;

internal class SGECombo : CustomComboJob<SGEGauge>
{
    internal override uint JobID => 40;

    private protected override BaseAction Raise => new BaseAction(24287);

    internal struct Actions
    {
        public static readonly BaseAction
            //עҩ
            Dosis = new BaseAction(24283),

            //����
            Phlegma = new BaseAction(24289),

            //���
            Diagnosis = new BaseAction(24284, true),

            //�Ĺ�
            Kardia = new BaseAction(24285, true)
            {
                BuffsProvide = new ushort[] { ObjectStatus.Kardia},
                ChoiceFriend = Targets =>
                {
                    Targets = Targets.Where(b => b.ObjectId != Service.ClientState.LocalPlayer.ObjectId).ToArray();

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

                    return ASTCombo.RandomObject(targets);
                },
                OtherCheck = b => BaseAction.FindStatusFromSelf(b, ObjectStatus.Kardion).Length == 0,
            },

            //Ԥ��
            Prognosis = new BaseAction(24286, true),

            //����
            Physis = new BaseAction(24288, true),

            //����2
            Physis2 = new BaseAction(24302, true),

            //����
            Eukrasia = new BaseAction(24290)
            {
                OtherCheck = b => !JobGauge.Eukrasia,
            },

            //����
            Soteria = new BaseAction(24294, true),

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

    private protected override bool ForAttachAbility(byte abilityRemain, out BaseAction act)
    {
        act = null;
        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, BaseAction nextGCD, out BaseAction act)
    {
        if (nextGCD.ActionID == Actions.Diagnosis.ActionID ||
            nextGCD.ActionID == Actions.Prognosis.ActionID)
        {
            //�
            if (Actions.Zoe.ShouldUseAction(out act)) return true;
        }

        if (nextGCD.ActionID == Actions.Diagnosis.ActionID)
        {
            //���
            if (Actions.Krasis.ShouldUseAction(out act)) return true;
        }

        act = null;
        return false;

    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out BaseAction act)
    {
        //��Ѫ
        if (Actions.Haima.ShouldUseAction(out act)) return true;

        if (JobGauge.Addersgall > 0)
        {
            //��ţ��֭
            if (Actions.Taurochole.ShouldUseAction(out act)) return true;
        }

        //����
        if (Actions.Eukrasia.ShouldUseAction(out act)) return true;

        //���
        if (Actions.Diagnosis.ShouldUseAction(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out BaseAction act)
    {
        //����Ѫ
        if (Actions.Panhaima.ShouldUseAction(out act)) return true;

        if (JobGauge.Addersgall > 0)
        {
            //�����֭
            if (Actions.Kerachole.ShouldUseAction(out act)) return true;
        }

        //����
        if (Actions.Eukrasia.ShouldUseAction(out act)) return true;

        //Ԥ��
        if (Actions.Prognosis.ShouldUseAction(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out BaseAction act)
    {

        if(JobGauge.Addersgall > 1)
        {
            //������֭
            if (Actions.Druochole.ShouldUseAction(out act)) return true;
        }

        act = null;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out BaseAction act)
    {
        //����
        if (Actions.Icarus.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool GeneralAbility(byte abilityRemain, out BaseAction act)
    {
        //�Ĺ�
        if (Actions.Kardia.ShouldUseAction(out act)) return true;

        //����
        if (JobGauge.Addersgall < 2 && Actions.Rhizomata.ShouldUseAction(out act)) return true;

        foreach (var friend in TargetHelper.PartyMembers)
        {
            var statuses = friend.StatusList.Select(status => status.StatusId);
            if (statuses.Contains(ObjectStatus.Kardion))
            {
                if ((float)friend.CurrentHp/friend.MaxHp < 0.7)
                {
                    Actions.Soteria.ShouldUseAction(out act);
                    return true;
                }
                break;
            }
        }

        act = null;
        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out BaseAction act)
    {
        //�����Ϣ
        if (Actions.Pneuma.ShouldUseAction(out act, mustUse:true)) return true;

        if (JobGauge.Addersting > 0)
        {
            //����
            if (Actions.Toxikon.ShouldUseAction(out act, mustUse:true)) return true;
        }

        //����
        if (Actions.Phlegma.ShouldUseAction(out act)) return true;

        //ʧ��
        if (Actions.Dyskrasia.ShouldUseAction(out act)) return true;

        Actions.Dosis.ShouldUseAction(out _);
        var times = BaseAction.FindStatusFromSelf(Actions.Dosis.Target,
            new ushort[] { ObjectStatus.EukrasianDosis, ObjectStatus.EukrasianDosis2, ObjectStatus.EukrasianDosis3 });
        if (times.Length == 0 || times.Max() < 3)
        {
            //����
            if (Actions.Eukrasia.ShouldUseAction(out act)) return true;
        }
        //עҩ
        if (Actions.Dosis.ShouldUseAction(out act)) return true;

        return false;
    }
    private protected override bool HealSingleGCD(uint lastComboActionID, out BaseAction act)
    {
        //���
        if (Actions.Diagnosis.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool HealAreaGCD(uint lastComboActionID, out BaseAction act)
    {
        //Ԥ��
        if (Actions.Prognosis.ShouldUseAction(out act)) return true;
        return false;
    }
    private protected override bool HealAreaAbility(byte abilityRemain, out BaseAction act)
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
        if (Actions.Physis.ShouldUseAction(out act)) return true;
        return false;
    }
}
