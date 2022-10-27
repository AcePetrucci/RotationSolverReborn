using Dalamud.Game.ClientState.JobGauge.Types;
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

internal sealed class RDMCombo : JobGaugeCombo<RDMGauge>
{
    internal override uint JobID => 35;
    protected override bool CanHealSingleSpell => TargetUpdater.PartyMembers.Length == 1 && base.CanHealSingleSpell;
    //����������û�дٽ�

    private protected override BaseAction Raise => Actions.Verraise;

    private static bool _startLong = false;

    public class RDMAction : BaseAction
    {
        internal override int Cast100 => NeedBuffNotCast ? 0 : base.Cast100;
        internal override ushort[] BuffsNeed 
        {
            get => NeedBuffNotCast ? base.BuffsNeed : null;
            set => base.BuffsNeed = value; 
        }
        public bool NeedBuffNotCast => !_startLong || InCombat;

        internal RDMAction(uint actionID, bool isFriendly = false, bool shouldEndSpecial = false) : base(actionID, isFriendly, shouldEndSpecial)
        {
            BuffsNeed = GeneralActions.Swiftcast.BuffsProvide.Union(new[] { ObjectStatus.Acceleration }).ToArray();
        }
    }
    internal struct Actions
    {
        public static readonly BaseAction
            //�ิ��
            Verraise = new(7523, true),

            //��
            Jolt = new(7503)
            {
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(new[] { ObjectStatus.Acceleration }).ToArray(),
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
            Moulinet = new(7513),

            //������
            Vercure = new(7514, true)
            {
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(Acceleration.BuffsProvide).ToArray(),
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
            Scorch = new(16530),

            //����
            Resolution = new(25858),

            //ħԪ��
            Manafication = new(7521)
            {
                OtherCheck = b => JobGauge.WhiteMana <= 50 && JobGauge.BlackMana <= 50 && InCombat && JobGauge.ManaStacks == 0,
                OtherIDsNot = new uint[] { Riposte.ID, Zwerchhau.ID, Scorch.ID, Verflare.ID, Verholy.ID },
            };
    }
    internal override SortedList<DescType, string> Description => new ()
    {
        {DescType.��������, $"{Actions.Vercure.Action.Name}"},
        {DescType.��Χ����, $"{Actions.MagickBarrier.Action.Name}"},
        {DescType.�ƶ�����, $"{Actions.CorpsAcorps.Action.Name}"},
    };

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetBool("StartLong", false, "����������")
            .SetBool("UseVercure", true, "ʹ�ó����ƻ�ü���");
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //����Ҫ�ŵ�ħ�ش̻���ħZն��ħ��Բն֮��
        if (nextGCD.IsAnySameAction(true, Actions.Zwerchhau, Actions.Redoublement, Actions.Moulinet))
        {
            if (Service.Configuration.AutoBreak && Actions.Embolden.ShouldUse(out act, mustUse: true)) return true;
        }
        //����������ʱ���ͷš�
        if (Service.Configuration.AutoBreak && GetRightValue(JobGauge.WhiteMana) && GetRightValue(JobGauge.BlackMana))
        {
            if (Actions.Manafication.ShouldUse(out act, Service.Address.LastComboAction)) return true;
            if (Actions.Embolden.ShouldUse(out act, mustUse: true)) return true;
        }
        //����Ҫ�ŵ�ħ������֮��
        if (JobGauge.ManaStacks == 3 || Level < 68 && !nextGCD.IsAnySameAction(true, Actions.Zwerchhau, Actions.Riposte))
        {
            if (Actions.Manafication.ShouldUse(out act, Service.Address.LastComboAction)) return true;
        }

        act = null;
        return false;
    }

    private bool GetRightValue(byte value)
    {
        return value >= 6 && value <= 12;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak)
        {
            if (Actions.Manafication.ShouldUse(out act, Service.Address.LastComboAction)) return true;
            if (Actions.Embolden.ShouldUse(out act, mustUse: true)) return true;
        }

        if (JobGauge.ManaStacks == 0 && (JobGauge.BlackMana < 50 || JobGauge.WhiteMana < 50) && !Actions.Manafication.WillHaveOneChargeGCD(1, 1))
        {
            //�ٽ����˾��á� 
            if (abilityRemain == 2 && Actions.Acceleration.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

            //����ӽ��
            if (GeneralActions.Swiftcast.ShouldUse(out act, mustUse: true)) return true;
        }

        //�����ĸ���������
        if (Actions.ContreSixte.ShouldUse(out act, mustUse: true)) return true;
        if (Actions.Fleche.ShouldUse(out act)) return true;
        //Empty: BaseAction.HaveStatusSelfFromSelf(1239)
        if (Actions.Engagement.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        if (Actions.CorpsAcorps.ShouldUse(out act) && !IsMoving)
        {
            if (Actions.CorpsAcorps.Target.DistanceToPlayer() < 1)
            {
                return true;
            }
        }

        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        _startLong = Config.GetBoolByName("StartLong");

        act = null;
        if (JobGauge.ManaStacks == 3) return false;

        #region �������
        if (!Actions.Verthunder2.ShouldUse(out _))
        {
            if (Actions.Verfire.ShouldUse(out act)) return true;
            if (Actions.Verstone.ShouldUse(out act)) return true;
        }

        //���Կ�ɢ��
        if (Actions.Scatter.ShouldUse(out act)) return true;
        //ƽ��ħԪ
        if (JobGauge.WhiteMana < JobGauge.BlackMana)
        {
            if (Actions.Veraero2.ShouldUse(out act)) return true;
            if (Actions.Veraero.ShouldUse(out act)) return true;
        }
        else
        {
            if (Actions.Verthunder2.ShouldUse(out act)) return true;
            if (Actions.Verthunder.ShouldUse(out act)) return true;
        }
        if (Actions.Jolt.ShouldUse(out act)) return true;
        #endregion

        //�����ƣ��Ӽ��̡�
        if (Config.GetBoolByName("UseVercure") && Actions.Vercure.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(uint lastComboActionID, out IAction act)
    {
        if (Actions.Vercure.ShouldUse(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.CorpsAcorps.ShouldUse(out act, mustUse: true)) return true;
        return false;
    }
    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //����
        if (GeneralActions.Addle.ShouldUse(out act)) return true;
        if (Actions.MagickBarrier.ShouldUse(out act, mustUse:true)) return true;
        return false;
    }

    private protected override bool EmergercyGCD(uint lastComboActionID, out IAction act)
    {
        byte level = Level;
        #region Զ������
        //���ħԪ�ᾧ���ˡ�
        if (JobGauge.ManaStacks == 3)
        {
            if (JobGauge.BlackMana > JobGauge.WhiteMana && level >= 70)
            {
                if (Actions.Verholy.ShouldUse(out act, mustUse: true)) return true;
            }
            if (Actions.Verflare.ShouldUse(out act, mustUse: true)) return true;
        }

        //�����һ�δ��˳���ʥ���߳�˱���
        if (lastComboActionID == Actions.Verholy.ID || lastComboActionID == Actions.Verflare.ID)
        {
            if (Actions.Scorch.ShouldUse(out act, mustUse: true)) return true;
        }

        //�����һ�δ��˽���
        if (lastComboActionID == Actions.Scorch.ID)
        {
            if (Actions.Resolution.ShouldUse(out act, mustUse: true)) return true;
        }
        #endregion

        #region ��ս����

        if (lastComboActionID == Actions.Moulinet.ID && JobGauge.BlackMana >= 20 && JobGauge.WhiteMana >= 20)
        {
            if (Actions.Moulinet.ShouldUse(out act)) return true;
            if (Actions.Riposte.ShouldUse(out act)) return true;
        }
        if (Actions.Zwerchhau.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.Redoublement.ShouldUse(out act, lastComboActionID)) return true;

        //����������ˣ�����ħԪ���ˣ��������ڱ��������ߴ��ڿ�������״̬���������ã�
        bool mustStart = Player.HaveStatus(1971) || JobGauge.BlackMana == 100 || JobGauge.WhiteMana == 100 || !Actions.Embolden.IsCoolDown;

        //��ħ��Ԫû�����������£�Ҫ���С��ħԪ����������Ҳ����ǿ��Ҫ�������жϡ�
        if (!mustStart)
        {
            if (JobGauge.BlackMana == JobGauge.WhiteMana) return false;

            //Ҫ���С��ħԪ����������Ҳ����ǿ��Ҫ�������жϡ�
            if (JobGauge.WhiteMana < JobGauge.BlackMana)
            {
                if (Player.HaveStatus(ObjectStatus.VerstoneReady))
                {
                    return false;
                }
            }
            if (JobGauge.WhiteMana > JobGauge.BlackMana)
            {
                if (Player.HaveStatus(ObjectStatus.VerfireReady))
                {
                    return false;
                }
            }

            //������û�м�����صļ��ܡ�
            foreach (var buff in Actions.Vercure.BuffsProvide)
            {
                if (Player.HaveStatus(buff))
                {
                    return false;
                }
            }

            //���������ʱ��쵽�ˣ�������û�á�
            if (Actions.Embolden.WillHaveOneChargeGCD(10))
            {
                return false;
            }
        }
        #endregion

        #region ��������
        //Ҫ������ʹ�ý�ս�����ˡ�
        if (Actions.Moulinet.ShouldUse(out act))
        {
            if (JobGauge.BlackMana >= 60 && JobGauge.WhiteMana >= 60) return true;
        }
        else
        {
            if (JobGauge.BlackMana >= 50 && JobGauge.WhiteMana >= 50 && Actions.Riposte.ShouldUse(out act)) return true;
        }
        #endregion

        return false;
    }
}
