using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Linq;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Basic;

internal abstract class RDMCombo_Base<TCmd> : JobGaugeCombo<RDMGauge, TCmd> where TCmd : Enum
{

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.RedMage };
    protected override bool CanHealSingleSpell => TargetUpdater.PartyMembers.Length == 1 && base.CanHealSingleSpell;
    //����������û�дٽ�

    private sealed protected override BaseAction Raise => Verraise;

    public static readonly BaseAction
        //�ิ��
        Verraise = new(7523, true),

        //��
        Jolt = new(7503)
        {
            BuffsProvide = Swiftcast.BuffsProvide.Union(new[] { StatusID.Acceleration }).ToArray(),
        },

        //�ش�
        Riposte = new(7504)
        {
            OtherCheck = b => JobGauge.BlackMana >= 20 && JobGauge.WhiteMana >= 20,
        },

        //������
        Verthunder = new(7505)
        {
            BuffsNeed = Jolt.BuffsProvide,
        },

        //�̱����
        CorpsAcorps = new(7506, shouldEndSpecial: true)
        {
            BuffsProvide = new[]
            {
                 StatusID.Bind1,
                 StatusID.Bind2,
            }
        },

        //�༲��
        Veraero = new(7507)
        {
            BuffsNeed = Jolt.BuffsProvide,
        },

        //ɢ��
        Scatter = new(7509)
        {
            BuffsNeed = Jolt.BuffsProvide,
        },

        //������
        Verthunder2 = new(16524u)
        {
            BuffsProvide = Jolt.BuffsProvide,
        },

        //���ҷ�
        Veraero2 = new(16525u)
        {
            BuffsProvide = Jolt.BuffsProvide,
        },

        //�����
        Verfire = new(7510)
        {
            BuffsNeed = new[] { StatusID.VerfireReady },
            BuffsProvide = Jolt.BuffsProvide,
        },

        //���ʯ
        Verstone = new(7511)
        {
            BuffsNeed = new[] { StatusID.VerstoneReady },
            BuffsProvide = Jolt.BuffsProvide,
        },

        //����ն
        Zwerchhau = new(7512)
        {
            OtherCheck = b => JobGauge.BlackMana >= 15 && JobGauge.WhiteMana >= 15,
        },

        //����
        Engagement = new(16527),

        //�ɽ�
        Fleche = new(7517),

        //����
        Redoublement = new(7516)
        {
            OtherCheck = b => JobGauge.BlackMana >= 15 && JobGauge.WhiteMana >= 15,
        },


        //�ٽ�
        Acceleration = new(7518)
        {
            BuffsProvide = new[] { StatusID.Acceleration },
        },

        //��Բն
        Moulinet = new(7513)
        {
            OtherCheck = b => JobGauge.BlackMana >= 20 && JobGauge.WhiteMana >= 20,
        },

        //������
        Vercure = new(7514, true)
        {
            BuffsProvide = Swiftcast.BuffsProvide.Union(Acceleration.BuffsProvide).ToArray(),
        },

        //���ַ���
        ContreSixte = new(7519u),

        //����
        Embolden = new(7520, true),

        //��ն
        Reprise = new(16529),

        //����
        MagickBarrier = new(25857),

        //��˱�
        Verflare = new(7525),

        //����ʥ
        Verholy = new(7526),

        //����
        Scorch = new(16530)
        {
            OtherIDsCombo = new uint[] { Verholy.ID },
        },

        //����
        Resolution = new(25858),

        //ħԪ��
        Manafication = new(7521)
        {
            OtherCheck = b => JobGauge.WhiteMana <= 50 && JobGauge.BlackMana <= 50 && InCombat && JobGauge.ManaStacks == 0,
            OtherIDsNot = new uint[] { Riposte.ID, Zwerchhau.ID, Scorch.ID, Verflare.ID, Verholy.ID },
        };
}
