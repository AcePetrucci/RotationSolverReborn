using Dalamud.Game.ClientState.JobGauge.Types;
using System.Linq;
namespace XIVComboPlus.Combos;

internal abstract class RDMCombo : CustomComboJob<RDMGauge>
{
    internal struct Actions
    {
        public static readonly BaseAction
            //��
            Jolt = new BaseAction(7503)
            {
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //�ش�
            EnchantedRiposte = new BaseAction(7504),

            //������
            Verthunder = new BaseAction(7505)
            {
                BuffsNeed = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] {ObjectStatus.Acceleration}).ToArray(),
            },

            //�̱����
            CorpsAcorps = new BaseAction(7506)
            {
                BuffsCantHave = new ushort[]
                {
                    ObjectStatus.Bind1,
                    ObjectStatus.Bind2,
                }
            },

            //�༲��
            Veraero = new BaseAction(7507)
            {
                BuffsNeed = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //ɢ��
            Scatter = new BaseAction(7509)
            {
                BuffsNeed = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //������
            Verthunder2 = new BaseAction(16524u)
            {
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //���ҷ�
            Veraero2 = new BaseAction(16525u)
            {
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //�����
            Verfire = new BaseAction(7510)
            {
                BuffsNeed = new ushort[] { ObjectStatus.VerfireReady },
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //���ʯ
            Verstone = new BaseAction(7511)
            {
                BuffsNeed = new ushort[] { ObjectStatus.VerstoneReady },
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //����ն
            EnchantedZwerchhau = new BaseAction(7512),

            //����
            Engagement = new BaseAction(16527),

            //�ɽ�
            Fleche = new BaseAction(7517),

            //����
            EnchantedRedoublement = new BaseAction(7516),

            //�ٽ�
            Acceleration = new BaseAction(7518),

            //��Բն
            EnchantedMoulinet = new BaseAction(7513),

            //������
            Vercure = new BaseAction(7514, true)
            {
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //���ַ���
            ContreSixte = new BaseAction(7519u),

            //����
            Embolden = new BaseAction(7520),

            //����
            Manafication = new BaseAction(7521),

            //�ิ��
            Verraise = new BaseAction(7523, true)
            {
                BuffsNeed = GeneralActions.Swiftcast.BuffsProvide,
                OtherCheck = () => TargetHelper.DeathPeopleAll.Length > 0,
                BuffsProvide = new ushort[] { ObjectStatus.Raise },
            },

            //��ն
            Reprise = new BaseAction(16529),

            //����
            MagickBarrier = new BaseAction(25857);
    }

    //public static class Buffs
    //{

    //    public const ushort LostChainspell = 2560;
    //}
    protected bool CanAddAbility(byte level, uint lastComboActionID, out uint act)
    {
        act = 0;

        if (CanInsertAbility)
        {
            //����Ҫ�ŵ�ħ�ش̻���ħZն��ħ��Բն֮��
            if (lastComboActionID == Actions.EnchantedRiposte.ActionID || lastComboActionID == Actions.EnchantedZwerchhau.ActionID || lastComboActionID == Actions.EnchantedMoulinet.ActionID)
            {
                if (Actions.Embolden.TryUseAction(level, out act, mustUse:true)) return true;
            }
            //����Ҫ�ŵ�ħ������֮��
            if (JobGauge.ManaStacks == 3 || lastComboActionID == Actions.EnchantedRedoublement.ActionID)
            {
                if (Actions.Manafication.TryUseAction(level, out act, mustUse:true)) return true;
            }

            //�Ӹ�����
            if (GeneralActions.LucidDreaming.TryUseAction(level, out act)) return true;


            //�ٽ����˾��á� 
            if (Actions.Acceleration.TryUseAction(level, out act)) return true;

            //�����ĸ���������
            if (Actions.ContreSixte.TryUseAction(level, out act, mustUse:true)) return true;
            if (Actions.Fleche.TryUseAction(level, out act)) return true;
            if (Actions.Engagement.TryUseAction(level, out act)) return true;
            //if (Actions.CorpsAcorps.TryUseAction(level, out act)) return true;

            //�ŶӼ��� 
            if(Actions.MagickBarrier.TryUseAction(level, out act)) return true;

            //�Ӹ�����
            if (GeneralActions.Addle.TryUseAction(level, out act)) return true;
        }
        return false;
    }
    public static bool CanBreak(uint lastComboActionID, byte level, out uint act, bool canOpen)
    {
        act = 0;

        #region Զ������
        //���ħԪ�ᾧ���ˡ�
        if (JobGauge.ManaStacks == 3)
        {
            if (JobGauge.BlackMana > JobGauge.WhiteMana && level >= 70)
            {
                if (Actions.Veraero2.TryUseAction(level, out act, mustUse: true)) return true;

            }
            if (Actions.Verthunder2.TryUseAction(level, out act, mustUse: true)) return true;
        }

        //�����һ�δ��˳���ʥ���߳�˱���
        if (level >= 80 && (lastComboActionID == 7525 || lastComboActionID == 7526))
        {
            if (Actions.Jolt.TryUseAction(level, out act, mustUse: true)) return true;
        }

        //�����һ�δ��˽���
        if (level >= 90 && lastComboActionID == 16530)
        {
            if (Actions.Jolt.TryUseAction(level, out act, mustUse: true)) return true;
        }
        #endregion

        #region ��ս����

        if (lastComboActionID == Actions.EnchantedMoulinet.ActionID && JobGauge.BlackMana >= 20 && JobGauge.WhiteMana >= 20)
        {
            if (Actions.EnchantedMoulinet.TryUseAction(level, out act)) return true;
        }
        if (Actions.EnchantedZwerchhau.TryUseAction(level, out act,lastComboActionID)) return true;
        if (Actions.EnchantedRedoublement.TryUseAction(level, out act, lastComboActionID)) return true;


        //��ħ��Ԫû�����������£�Ҫ���С��ħԪ����������Ҳ����ǿ��Ҫ�������жϡ�
        if (JobGauge.BlackMana < 100 & JobGauge.WhiteMana < 100)
        {
            if (JobGauge.BlackMana == JobGauge.WhiteMana) return false;
            if (JobGauge.WhiteMana < JobGauge.BlackMana)
            {
                if (BaseAction.HaveStatus(BaseAction.FindStatusSelfFromSelf(ObjectStatus.VerstoneReady)))
                {
                    return false;
                }
            }
            if (JobGauge.WhiteMana > JobGauge.BlackMana)
            {
                if (BaseAction.HaveStatus(BaseAction.FindStatusSelfFromSelf(ObjectStatus.VerfireReady)))
                {
                    return false;
                }
            }
        }

        #endregion

        //if (!canOpen) return false;

        #region ��������
        //������û�м�����صļ��ܡ�
        bool haveIt = false;
        foreach (var buff in GeneralActions.Swiftcast.BuffsProvide)
        {
            if (BaseAction.HaveStatus(BaseAction.FindStatusSelfFromSelf(buff)))
            {
                haveIt = true;
                break;
            }
        }
        //���û�м�����صģ����ҹ���Ҫô��̫�磬Ҫô�Ѿ��������ˡ��Ϳ��Կ�ʼ�����ˣ�
        if (!haveIt && (Actions.Embolden.CoolDown.CooldownRemaining > 20 || Actions.Embolden.CoolDown.CooldownRemaining < 1))
        {
            //Ҫ������ʹ�ý�ս�����ˡ�
            if (Service.Configuration.IsTargetBoss && JobGauge.BlackMana >= 50 && JobGauge.WhiteMana >= 50)
            {
                if (Actions.EnchantedRiposte.TryUseAction(level, out act)) return true;
                if (canOpen && Actions.CorpsAcorps.TryUseAction(level, out act, mustUse: true)) return true;
            }
            if (JobGauge.BlackMana >= 60 && JobGauge.WhiteMana >= 60)
            {
                if (Actions.EnchantedMoulinet.TryUseAction(level, out act)) return true;
                if (Actions.EnchantedRiposte.TryUseAction(level, out act)) return true;
                if (canOpen && Actions.CorpsAcorps.TryUseAction(level, out act, mustUse: true)) return true;
            }
        }
        #endregion
        return false;
    }

}
