using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.RangedMagicial;

public abstract class RDMCombo<TCmd> : JobGaugeCombo<RDMGauge, TCmd> where TCmd : Enum
{

    public sealed override uint[] JobIDs => new uint[] { 35 };
    protected override bool CanHealSingleSpell => TargetUpdater.PartyMembers.Length == 1 && base.CanHealSingleSpell;
    //����������û�дٽ�

    private protected override BaseAction Raise => Verraise;

    protected static bool StartLong = false;

    public class RDMAction : BaseAction
    {
        public override ushort[] BuffsNeed 
        {
            get => NeedBuffNotCast ? base.BuffsNeed : null;
            set => base.BuffsNeed = value; 
        }
        public bool NeedBuffNotCast => !StartLong || InCombat;

        internal RDMAction(uint actionID, bool isFriendly = false, bool shouldEndSpecial = false) : base(actionID, isFriendly, shouldEndSpecial)
        {
            BuffsNeed = Swiftcast.BuffsProvide.Union(new[] { ObjectStatus.Acceleration }).ToArray();
        }
    }
    public static readonly BaseAction
        //�ิ��
        Verraise = new(7523, true),

        //��
        Jolt = new(7503)
        {
            BuffsProvide = Swiftcast.BuffsProvide.Union(new[] { ObjectStatus.Acceleration }).ToArray(),
        },

        //�ش�
        Riposte = new(7504)
        {
            OtherCheck = b => JobGauge.BlackMana >= 20 && JobGauge.WhiteMana >= 20,
        },

        //������
        Verthunder = new RDMAction(7505),

        //�̱����
        CorpsAcorps = new(7506, shouldEndSpecial: true)
        {
            BuffsProvide = new[]
            {
                    ObjectStatus.Bind1,
                    ObjectStatus.Bind2,
            }
        },

        //�༲��
        Veraero = new RDMAction(7507),

        //ɢ��
        Scatter = new RDMAction(7509),

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
            BuffsNeed = new[] { ObjectStatus.VerfireReady },
            BuffsProvide = Jolt.BuffsProvide,
        },

        //���ʯ
        Verstone = new(7511)
        {
            BuffsNeed = new[] { ObjectStatus.VerstoneReady },
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
            BuffsProvide = new[] { ObjectStatus.Acceleration },
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
