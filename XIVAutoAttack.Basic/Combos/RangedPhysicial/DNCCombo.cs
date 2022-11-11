using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.RangedPhysicial;
public abstract class DNCCombo<TCmd> : JobGaugeCombo<DNCGauge, TCmd> where TCmd : Enum
{

    public override uint[] JobIDs => new uint[] { 38 };

    public static readonly BaseAction

        //��к
        Cascade = new(15989)
        {
            BuffsProvide = new[] { ObjectStatus.SilkenSymmetry }
        },

        //��Ȫ
        Fountain = new(15990)
        {
            BuffsProvide = new[] { ObjectStatus.SilkenFlow }
        },

        //����к
        ReverseCascade = new(15991)
        {
            BuffsNeed = new[] { ObjectStatus.SilkenSymmetry, ObjectStatus.SilkenSymmetry2 },
        },

        //׹��Ȫ
        Fountainfall = new(15992)
        {
            BuffsNeed = new[] { ObjectStatus.SilkenFlow, ObjectStatus.SilkenFlow2 }
        },

        //���衤��
        FanDance = new(16007)
        {
            OtherCheck = b => JobGauge.Feathers > 0,
            BuffsProvide = new[] { ObjectStatus.ThreefoldFanDance },
        },

        //�糵
        Windmill = new(15993)
        {
            BuffsProvide = Cascade.BuffsProvide,
        },

        //������
        Bladeshower = new(15994)
        {
            BuffsProvide = Fountain.BuffsProvide,
        },

        //���糵
        RisingWindmill = new(15995)
        {
            BuffsNeed = ReverseCascade.BuffsNeed,
        },

        //��Ѫ��
        Bloodshower = new(15996)
        {
            BuffsNeed = Fountainfall.BuffsNeed,
        },

        //���衤��
        FanDance2 = new(16008)
        {
            OtherCheck = b => JobGauge.Feathers > 0,
            BuffsProvide = new[] { ObjectStatus.ThreefoldFanDance },
        },

        //���衤��
        FanDance3 = new(16009)
        {
            BuffsNeed = FanDance2.BuffsProvide,
        },

        //���衤��
        FanDance4 = new(25791)
        {
            BuffsNeed = new[] { ObjectStatus.FourfoldFanDance },
        },

        //����
        SaberDance = new(16005)
        {
            OtherCheck = b => JobGauge.Esprit >= 50,
        },

        //������
        StarfallDance = new(25792)
        {
            BuffsNeed = new[] { ObjectStatus.FlourishingStarfall },
        },

        //ǰ�岽
        EnAvant = new(16010, shouldEndSpecial: true),

        //Ǿޱ���Ų�
        Emboite = new(15999)
        {
            OtherCheck = b => JobGauge.NextStep == 15999,
        },

        //С�񽻵���
        Entrechat = new(16000)
        {
            OtherCheck = b => JobGauge.NextStep == 16000,
        },

        //��ҶС����
        Jete = new(16001)
        {
            OtherCheck = b => JobGauge.NextStep == 16001,
        },

        //���ֺ��ת
        Pirouette = new(16002)
        {
            OtherCheck = b => JobGauge.NextStep == 16002,
        },

        //��׼�貽
        StandardStep = new(15997)
        {
            BuffsProvide = new[]
            {
                    ObjectStatus.StandardStep,
                    ObjectStatus.TechnicalStep,
            },
        },

        //�����貽
        TechnicalStep = new(15998)
        {
            BuffsNeed = new[]
            {
                    ObjectStatus.StandardFinish,
            },
            BuffsProvide = StandardStep.BuffsProvide,
        },

        //����֮ɣ��
        ShieldSamba = new(16012, true)
        {
            BuffsProvide = new[]
            {
                    ObjectStatus.Troubadour,
                    ObjectStatus.Tactician1,
                    ObjectStatus.Tactician2,
                    ObjectStatus.ShieldSamba,
            },
        },

        //����֮������
        CuringWaltz = new(16015, true),

        //��ʽ����
        ClosedPosition = new(16006, true)
        {
            ChoiceTarget = Targets =>
            {
                Targets = Targets.Where(b => b.ObjectId != Player.ObjectId && b.CurrentHp != 0 &&
                //Remove Weak
                b.StatusList.Select(status => status.StatusId).Intersect(new uint[] { ObjectStatus.Weakness, ObjectStatus.BrinkofDeath }).Count() == 0 &&
                //Remove other partner.
                b.StatusList.Where(s => s.StatusId == ObjectStatus.ClosedPosition2 && s.SourceID != Player.ObjectId).Count() == 0).ToArray();

                var targets = TargetFilter.GetJobCategory(Targets, Role.��ս);
                if (targets.Length > 0) return targets[0];

                targets = TargetFilter.GetJobCategory(Targets, Role.Զ��);
                if (targets.Length > 0) return targets[0];

                targets = Targets;
                if (targets.Length > 0) return targets[0];

                return null;
            },
        },

        //����֮̽��
        Devilment = new(16011, true),

        //�ٻ�����
        Flourish = new(16013)
        {
            BuffsNeed = new[] { ObjectStatus.StandardFinish },
            BuffsProvide = new[]
            {
                    ObjectStatus.ThreefoldFanDance,
                    ObjectStatus.FourfoldFanDance,
            },
            OtherCheck = b => InCombat,
        },

        //���˱���
        Improvisation = new(16014, true),

        //������
        Tillana = new(25790)
        {
            BuffsNeed = new[] { ObjectStatus.FlourishingFinish },
        };

}
