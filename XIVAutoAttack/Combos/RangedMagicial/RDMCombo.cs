using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace XIVAutoAttack.Combos.RangedMagicial;

internal class RDMCombo : JobGaugeCombo<RDMGauge>
{
    internal override uint JobID => 35;
    protected override bool CanHealSingleSpell => false;
    //����������û�дٽ�

    private protected override BaseAction Raise => Actions.Verraise;
    public class RDMAction : BaseAction
    {
        internal override int Cast100 => TargetHelper.InBattle ? 0 : base.Cast100;
        internal override ushort[] BuffsNeed 
        {
            get => TargetHelper.InBattle ? base.BuffsNeed : null;
            set => base.BuffsNeed = value; 
        }
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
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(new[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //���ҷ�
            Veraero2 = new(16525u)
            {
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(new[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //�����
            Verfire = new(7510)
            {
                BuffsNeed = new[] { ObjectStatus.VerfireReady },
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(new[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //���ʯ
            Verstone = new(7511)
            {
                BuffsNeed = new[] { ObjectStatus.VerstoneReady },
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(new[] { ObjectStatus.Acceleration }).ToArray(),
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
                OtherCheck = b => JobGauge.WhiteMana <= 50 && JobGauge.BlackMana <= 50 && TargetHelper.InBattle && JobGauge.ManaStacks == 0,
                OtherIDsNot = new uint[] { Riposte.ID, Zwerchhau.ID, Scorch.ID, Verflare.ID, Verholy.ID },
            };

    }
    internal override SortedList<DescType, string> Description => new ()
    {
        {DescType.��������, $"{Actions.Vercure.Action.Name}"},
        {DescType.��Χ����, $"{Actions.MagickBarrier.Action.Name}"},
        {DescType.�ƶ�, $"{Actions.CorpsAcorps.Action.Name}"},
    };
    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //����Ҫ�ŵ�ħ�ش̻���ħZն��ħ��Բն֮��
        if (nextGCD.ID == Actions.Zwerchhau.ID || nextGCD.ID == Actions.Redoublement.ID || nextGCD.ID == Actions.Moulinet.ID)
        {
            if (Service.Configuration.AutoBreak && Actions.Embolden.ShouldUseAction(out act, mustUse: true)) return true;
        }
        //����������ʱ���ͷš�
        if (Service.Configuration.AutoBreak && GetRightValue(JobGauge.WhiteMana) && GetRightValue(JobGauge.BlackMana))
        {
            if (Actions.Manafication.ShouldUseAction(out act, Service.Address.LastComboAction)) return true;
            if (Actions.Embolden.ShouldUseAction(out act, mustUse: true)) return true;
        }
        //����Ҫ�ŵ�ħ������֮��
        if (JobGauge.ManaStacks == 3 || Service.ClientState.LocalPlayer.Level < 68 && nextGCD.ID != Actions.Zwerchhau.ID && nextGCD.ID != Actions.Riposte.ID)
        {
            if (Actions.Manafication.ShouldUseAction(out act, Service.Address.LastComboAction)) return true;
        }

        act = null;
        return false;
    }

    private bool GetRightValue(byte value)
    {
        return value >= 6 && value <= 12;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        if (JobGauge.ManaStacks == 0 && (JobGauge.BlackMana < 50 || JobGauge.WhiteMana < 50) && Actions.Manafication.RecastTimeRemain > 4)
        {
            //�ٽ����˾��á� 
            if (abilityRemain == 2 && Actions.Acceleration.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;

            //����ӽ��
            if (GeneralActions.Swiftcast.ShouldUseAction(out act, mustUse: true)) return true;
        }

        //�����ĸ���������
        if (Actions.ContreSixte.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.Fleche.ShouldUseAction(out act)) return true;
        //Empty: BaseAction.HaveStatusSelfFromSelf(1239)
        if (Actions.Engagement.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;

        if (Actions.CorpsAcorps.ShouldUseAction(out act) && !IsMoving)
        {
            if (BaseAction.DistanceToPlayer(Actions.CorpsAcorps.Target) < 1)
            {
                return true;
            }
        }

        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        act = null;
        if (JobGauge.ManaStacks == 3) return false;

        #region �������
        if (!Actions.Verthunder2.ShouldUseAction(out _))
        {
            if (Actions.Verfire.ShouldUseAction(out act)) return true;
            if (Actions.Verstone.ShouldUseAction(out act)) return true;
        }

        //���Կ�ɢ��
        if (Actions.Scatter.ShouldUseAction(out act)) return true;
        //ƽ��ħԪ
        if (JobGauge.WhiteMana < JobGauge.BlackMana)
        {
            if (Actions.Veraero2.ShouldUseAction(out act)) return true;
            if (Actions.Veraero.ShouldUseAction(out act)) return true;
        }
        else
        {
            if (Actions.Verthunder2.ShouldUseAction(out act)) return true;
            if (Actions.Verthunder.ShouldUseAction(out act)) return true;
        }
        if (Actions.Jolt.ShouldUseAction(out act)) return true;
        #endregion
        //�����ƣ��Ӽ��̡�
        if (Actions.Vercure.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(uint lastComboActionID, out IAction act)
    {
        if (Actions.Vercure.ShouldUseAction(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.CorpsAcorps.ShouldUseAction(out act, mustUse: true)) return true;
        return false;
    }
    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //����
        if (GeneralActions.Addle.ShouldUseAction(out act)) return true;
        if (Actions.MagickBarrier.ShouldUseAction(out act, mustUse:true)) return true;
        return false;
    }

    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.Manafication.ShouldUseAction(out act, Service.Address.LastComboAction)) return true;
        if (Actions.Embolden.ShouldUseAction(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool EmergercyGCD(uint lastComboActionID, out IAction act)
    {
        byte level = Service.ClientState.LocalPlayer.Level;
        #region Զ������
        //���ħԪ�ᾧ���ˡ�
        if (JobGauge.ManaStacks == 3)
        {
            if (JobGauge.BlackMana > JobGauge.WhiteMana && level >= 70)
            {
                if (Actions.Verholy.ShouldUseAction(out act, mustUse: true)) return true;
            }
            if (Actions.Verflare.ShouldUseAction(out act, mustUse: true)) return true;
        }

        //�����һ�δ��˳���ʥ���߳�˱���
        if (lastComboActionID == Actions.Verholy.ID || lastComboActionID == Actions.Verflare.ID)
        {
            if (Actions.Scorch.ShouldUseAction(out act, mustUse: true)) return true;
        }

        //�����һ�δ��˽���
        if (lastComboActionID == Actions.Scorch.ID)
        {
            if (Actions.Resolution.ShouldUseAction(out act, mustUse: true)) return true;
        }
        #endregion

        #region ��ս����

        if (lastComboActionID == Actions.Moulinet.ID && JobGauge.BlackMana >= 20 && JobGauge.WhiteMana >= 20)
        {
            if (Actions.Moulinet.ShouldUseAction(out act)) return true;
            if (Actions.Riposte.ShouldUseAction(out act)) return true;
        }
        if (Actions.Zwerchhau.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.Redoublement.ShouldUseAction(out act, lastComboActionID)) return true;

        //����������ˣ�����ħԪ���ˣ��������ڱ��������ߴ��ڿ�������״̬���������ã�
        bool mustStart = BaseAction.HaveStatusSelfFromSelf(1971) || JobGauge.BlackMana == 100 || JobGauge.WhiteMana == 100 || !Actions.Embolden.IsCoolDown;

        //��ħ��Ԫû�����������£�Ҫ���С��ħԪ����������Ҳ����ǿ��Ҫ�������жϡ�
        if (!mustStart)
        {
            if (JobGauge.BlackMana == JobGauge.WhiteMana) return false;

            //Ҫ���С��ħԪ����������Ҳ����ǿ��Ҫ�������жϡ�
            if (JobGauge.WhiteMana < JobGauge.BlackMana)
            {
                if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.VerstoneReady))
                {
                    return false;
                }
            }
            if (JobGauge.WhiteMana > JobGauge.BlackMana)
            {
                if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.VerfireReady))
                {
                    return false;
                }
            }

            //������û�м�����صļ��ܡ�
            foreach (var buff in Actions.Vercure.BuffsProvide)
            {
                if (BaseAction.HaveStatusSelfFromSelf(buff))
                {
                    return false;
                }
            }

            //���������ʱ��쵽�ˣ�������û�á�
            float emboldenRemain = Actions.Embolden.RecastTimeRemain;
            if (emboldenRemain < 30 && emboldenRemain > 1)
            {
                return false;
            }
        }
        #endregion

        #region ��������
        //Ҫ������ʹ�ý�ս�����ˡ�
        if (Actions.Moulinet.ShouldUseAction(out act))
        {
            if (JobGauge.BlackMana >= 60 && JobGauge.WhiteMana >= 60) return true;
        }
        else
        {
            if (JobGauge.BlackMana >= 50 && JobGauge.WhiteMana >= 50 && Actions.Riposte.ShouldUseAction(out act)) return true;
        }
        #endregion

        return false;
    }
}
