using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Healer.SGECombos;

internal abstract class SGECombo<TCmd> : JobGaugeCombo<SGEGauge, TCmd> where TCmd : Enum
{
    public sealed override uint[] JobIDs => new uint[] { 40 };
    private sealed protected override BaseAction Raise => Egeiro;

    public static readonly BaseAction
        //����
        Egeiro = new(24287),

        //עҩ
        Dosis = new(24283),

        //����עҩ
        EukrasianDosis = new(24283, isEot: true)
        {
            TargetStatus = new ushort[]
            {
                StatusIDs.EukrasianDosis,
                StatusIDs.EukrasianDosis2,
                StatusIDs.EukrasianDosis3
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
            BuffsProvide = new ushort[] { StatusIDs.Kardia },
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
                        && status.StatusId == StatusIDs.Kardion)
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
                    if (friend.HaveStatus(StatusIDs.Kardion))
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
                            StatusIDs.EukrasianDiagnosis,
                            StatusIDs.EukrasianPrognosis,
                    }).Any()
                    && b.WillStatusEndGCD(2, 0, true, StatusIDs.EukrasianDiagnosis, StatusIDs.EukrasianPrognosis)
                    && chara.GetHealthRatio() < 0.9) return true;
                }

                return false;
            },
        };
}
