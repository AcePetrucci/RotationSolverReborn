using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Linq;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.Basic;
internal abstract class DNCCombo_Base<TCmd> : JobGaugeCombo<DNCGauge, TCmd> where TCmd : Enum
{

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Dancer };

    public static readonly BaseAction

        //��к
        Cascade = new(15989)
        {
            BuffsProvide = new[] { StatusID.SilkenSymmetry }
        },

        //��Ȫ
        Fountain = new(15990)
        {
            BuffsProvide = new[] { StatusID.SilkenFlow }
        },

        //����к
        ReverseCascade = new(15991)
        {
            BuffsNeed = new[] { StatusID.SilkenSymmetry, StatusID.SilkenSymmetry2 },
        },

        //׹��Ȫ
        Fountainfall = new(15992)
        {
            BuffsNeed = new[] { StatusID.SilkenFlow, StatusID.SilkenFlow2 }
        },

        //���衤��
        FanDance = new(16007)
        {
            OtherCheck = b => JobGauge.Feathers > 0,
            BuffsProvide = new[] { StatusID.ThreefoldFanDance },
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
            BuffsProvide = new[] { StatusID.ThreefoldFanDance },
        },

        //���衤��
        FanDance3 = new(16009)
        {
            BuffsNeed = FanDance2.BuffsProvide,
        },

        //���衤��
        FanDance4 = new(25791)
        {
            BuffsNeed = new[] { StatusID.FourfoldFanDance },
        },

        //����
        SaberDance = new(16005)
        {
            OtherCheck = b => JobGauge.Esprit >= 50,
        },

        //������
        StarfallDance = new(25792)
        {
            BuffsNeed = new[] { StatusID.FlourishingStarfall },
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
                    StatusID.StandardStep,
                    StatusID.TechnicalStep,
            },
        },

        //�����貽
        TechnicalStep = new(15998)
        {
            BuffsNeed = new[]
            {
                    StatusID.StandardFinish,
            },
            BuffsProvide = StandardStep.BuffsProvide,
        },

        //����֮ɣ��
        ShieldSamba = new(16012, true)
        {
            BuffsProvide = new[]
            {
                    StatusID.Troubadour,
                    StatusID.Tactician1,
                    StatusID.Tactician2,
                    StatusID.ShieldSamba,
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
                !b.HaveStatus(StatusID.Weakness, StatusID.BrinkofDeath)
                //Remove other partner.
                //&& !b.HaveStatusFromSelf(StatusID.ClosedPosition2)
                ).ToArray();

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
            BuffsNeed = new[] { StatusID.StandardFinish },
            BuffsProvide = new[]
            {
                    StatusID.ThreefoldFanDance,
                    StatusID.FourfoldFanDance,
            },
            OtherCheck = b => InCombat,
        },

        //���˱���
        Improvisation = new(16014, true),

        //������
        Tillana = new(25790)
        {
            BuffsNeed = new[] { StatusID.FlourishingFinish },
        };

}
