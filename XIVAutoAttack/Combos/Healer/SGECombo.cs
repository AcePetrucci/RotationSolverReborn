using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Attributes;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.Healer.SGECombo;

namespace XIVAutoAttack.Combos.Healer;

[ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/XIVAutoAttack/Combos/Healer/SGECombo.cs",
   ComboAuthor.Armolion)]
internal sealed class SGECombo : JobGaugeCombo<SGEGauge, CommandType>
{
    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //д��ע�Ͱ���������ʾ�û��ġ�
    };
    public override uint[] JobIDs => new uint[] { 40 };
    internal static byte level => Service.ClientState.LocalPlayer!.Level;

    private protected override BaseAction Raise => Egeiro;
    protected override bool CanHealSingleSpell => base.CanHealSingleSpell && (Config.GetBoolByName("GCDHeal") || TargetUpdater.PartyHealers.Length < 2);
    protected override bool CanHealAreaSpell => base.CanHealAreaSpell && (Config.GetBoolByName("GCDHeal") || TargetUpdater.PartyHealers.Length < 2);

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
                ChoiceTarget = TargetFilter.FindTargetForMoving,
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
                        && b.WillStatusEndGCD(2, 0, true, ObjectStatus.EukrasianDiagnosis, ObjectStatus.EukrasianPrognosis)
                        && chara.GetHealthRatio() < 0.9) return true;
                    }

                    return false;
                },
            };
    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("GCDHeal", false, "�Զ���GCD��");
    }

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.��Χ����, $"GCD: {Prognosis}\n                     ����: {Holos}, {Ixochole}, {Physis2}, {Physis}"},
        {DescType.��������, $"GCD: {Diagnosis}\n                     ����: {Druochole}"},
        {DescType.��Χ����, $"{Panhaima}, {Kerachole}, {Prognosis}"},
        {DescType.�������, $"GCD: {Diagnosis}\n                     ����: {Haima}, {Taurochole}"},
        {DescType.�ƶ�����, $"{Icarus}��Ŀ��Ϊ����н�С��30������ԶĿ�ꡣ"},
    };
    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        act = null!;
        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (base.EmergercyAbility(abilityRemain, nextGCD, out act)) return true;

        //�¸�������
        if (nextGCD.IsAnySameAction(false, Pneuma , EukrasianDiagnosis, 
            EukrasianPrognosis , Diagnosis , Prognosis))
        {
            //�
            if (Zoe.ShouldUse(out act)) return true;
        }

        if (nextGCD == Diagnosis)
        {
            //���
            if (Krasis.ShouldUse(out act)) return true;
        }

        act = null;
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {

        if (JobGauge.Addersgall == 0)
        {
            //��Ѫ
            if (Haima.ShouldUse(out act)) return true;
        }

        //��ţ��֭
        if (Taurochole.ShouldUse(out act)) return true;

        act = null!;
        return false;
    }

    private protected override bool DefenseSingleGCD(out IAction act)
    {
        //���
        if (EukrasianDiagnosis.ShouldUse(out act))
        {
            if (EukrasianDiagnosis.Target.StatusList.Select(s => s.StatusId).Intersect(new uint[]
            {
                ObjectStatus.EukrasianDiagnosis,
                ObjectStatus.EukrasianPrognosis,
                ObjectStatus.Galvanize,
            }).Count() > 0) return false;

            //����
            if (Eukrasia.ShouldUse(out act)) return true;

            act = EukrasianDiagnosis;
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
            if (Panhaima.ShouldUse(out act)) return true;
        }

        //�����֭
        if (Kerachole.ShouldUse(out act)) return true;

        //������
        if (Holos.ShouldUse(out act)) return true;

        act = null!;
        return false;
    }

    private protected override bool DefenseAreaGCD(out IAction act)
    {
        //Ԥ��
        if (EukrasianPrognosis.ShouldUse(out act))
        {
            if (EukrasianPrognosis.Target.StatusList.Select(s => s.StatusId).Intersect(new uint[]
            {
                ObjectStatus.EukrasianDiagnosis,
                ObjectStatus.EukrasianPrognosis,
                ObjectStatus.Galvanize,
            }).Count() > 0) return false;

            //����
            if (Eukrasia.ShouldUse(out act)) return true;

            act = EukrasianPrognosis;
            return true;
        }

        act = null!;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //����
        if (Icarus.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool GeneralAbility(byte abilityRemain, out IAction act)
    {
        //�Ĺ�
        if (Kardia.ShouldUse(out act)) return true;

        //����
        if (JobGauge.Addersgall == 0 && Rhizomata.ShouldUse(out act)) return true;

        //����
        if (Soteria.ShouldUse(out act)) return true;

        //����
        if (Pepsis.ShouldUse(out act)) return true;

        act = null!;
        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //����
        if (JobGauge.Addersting == 3 && Toxikon.ShouldUse(out act, mustUse: true)) return true;

        var level = Level;
        //����
        if (Phlegma3.ShouldUse(out act, mustUse: Phlegma3.WillHaveOneChargeGCD(2), emptyOrSkipCombo: true)) return true;
        if (!Phlegma3.EnoughLevel && Phlegma2.ShouldUse(out act, mustUse: Phlegma2.WillHaveOneChargeGCD(2), emptyOrSkipCombo: true)) return true;
        if (!Phlegma2.EnoughLevel && Phlegma.ShouldUse(out act, mustUse: Phlegma.WillHaveOneChargeGCD(2), emptyOrSkipCombo: true)) return true;

        //ʧ��
        if (Dyskrasia.ShouldUse(out act)) return true;

        if(EukrasianDosis.ShouldUse(out var enAct))
        {
            //����Dot
            if (Eukrasia.ShouldUse(out act)) return true;
            act = enAct;
            return true;
        }
        else if (JobGauge.Eukrasia)
        {
            if (DefenseAreaGCD(out act)) return true;
            if (DefenseSingleGCD(out act)) return true;
        }

        //עҩ
        if (Dosis.ShouldUse(out act)) return true;

        //����
        if (Phlegma3.ShouldUse(out act, mustUse: true)) return true;
        if (!Phlegma3.EnoughLevel && Phlegma2.ShouldUse(out act, mustUse: true)) return true;
        if (!Phlegma2.EnoughLevel && Phlegma.ShouldUse(out act, mustUse: true)) return true;

        //����
        if (JobGauge.Addersting > 0 && Toxikon.ShouldUse(out act, mustUse: true)) return true;

        //��ս��Tˢ�����ζ���
        if (!InCombat)
        {
            var tank = TargetUpdater.PartyTanks;
            if (tank.Length == 1 && EukrasianDiagnosis.Target == tank.First() && EukrasianDiagnosis.ShouldUse(out act))
            {
                if (tank.First().StatusList.Select(s => s.StatusId).Intersect(new uint[]
                {
                ObjectStatus.EukrasianDiagnosis,
                ObjectStatus.EukrasianPrognosis,
                ObjectStatus.Galvanize,
                }).Any()) return false;

                //����
                if (Eukrasia.ShouldUse(out act)) return true;

                act = EukrasianDiagnosis;
                return true;
            }
            if (Eukrasia.ShouldUse(out act)) return true;
        }

        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        //��ţ��֭
        if (Taurochole.ShouldUse(out act)) return true;

        //������֭
        if (Druochole.ShouldUse(out act)) return true;

        //����Դ����ʱ���뷶Χ���ƻ���ѹ��
        var tank = TargetUpdater.PartyTanks;
        var isBoss = Dosis.Target.IsBoss();
        if (JobGauge.Addersgall == 0 && tank.Length == 1 && tank.Any(t => t.GetHealthRatio() < 0.6f) && !isBoss)
        {
            //������
            if (Holos.ShouldUse(out act)) return true;

            //����2
            if (Physis2.ShouldUse(out act)) return true;
            //����
            if (!Physis2.EnoughLevel && Physis.ShouldUse(out act)) return true;

            //����Ѫ
            if (Panhaima.ShouldUse(out act)) return true;
        }

        act = null!;
        return false;
    }

    private protected override bool HealSingleGCD(out IAction act)
    {
        //���
        if (EukrasianDiagnosis.ShouldUse(out act))
        {
            if (EukrasianDiagnosis.Target.StatusList.Select(s => s.StatusId).Intersect(new uint[]
            {
                //�������
                ObjectStatus.EukrasianDiagnosis,
                ObjectStatus.EukrasianPrognosis,
                ObjectStatus.Galvanize,
            }).Count() > 0)
            {
                if (Diagnosis.ShouldUse(out act)) return true;
            }

            //����
            if (Eukrasia.ShouldUse(out act)) return true;

            act = EukrasianDiagnosis;
            return true;
        }

        //���
        if (Diagnosis.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool HealAreaGCD(out IAction act)
    {
        if (TargetUpdater.PartyMembersAverHP < 0.55f)
        {
            //�����Ϣ
            if (Pneuma.ShouldUse(out act, mustUse: true)) return true;
        }

        if (EukrasianPrognosis.ShouldUse(out act))
        {
            if (EukrasianPrognosis.Target.StatusList.Select(s => s.StatusId).Intersect(new uint[]
            {
                //�������
                ObjectStatus.EukrasianDiagnosis,
                ObjectStatus.EukrasianPrognosis,
                ObjectStatus.Galvanize,
            }).Count() > 0)
            {
                if (Prognosis.ShouldUse(out act)) return true;
            }

            //����
            if (Eukrasia.ShouldUse(out act)) return true;

            act = EukrasianPrognosis;
            return true;
        }

        //Ԥ��
        if (Prognosis.ShouldUse(out act)) return true;
        return false;
    }
    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        //�����֭
        if (Kerachole.ShouldUse(out act) && level >= Level) return true;

        //����2
        if (Physis2.ShouldUse(out act)) return true;
        //����
        if (!Physis2.EnoughLevel && Physis.ShouldUse(out act)) return true;

        //������
        if (Holos.ShouldUse(out act) && TargetUpdater.PartyMembersAverHP < 0.65f) return true;

        //������֭
        if (Ixochole.ShouldUse(out act)) return true;

        return false;
    }
}
