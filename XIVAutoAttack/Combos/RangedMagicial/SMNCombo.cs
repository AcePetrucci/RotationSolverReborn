using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.RangedMagicial.SMNCombo;

namespace XIVAutoAttack.Combos.RangedMagicial;

internal sealed class SMNCombo : JobGaugeCombo<SMNGauge, CommandType>
{
    public override ComboAuthor[] Authors => new ComboAuthor[] { ComboAuthor.None };

    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //д��ע�Ͱ���������ʾ�û��ġ�
    };
    public class SMNAction : BaseAction
    {
        public SMNAction(uint actionID, bool isFriendly = false, bool shouldEndSpecial = false)
            : base(actionID, isFriendly, shouldEndSpecial)
        {

        }
    }
    public override uint[] JobIDs => new uint[] { 27, 26 };
    protected override bool CanHealSingleSpell => false;
    private protected override BaseAction Raise => Resurrection;

    private static bool InBahamut => Service.IconReplacer.OriginalHook(25822) == Deathflare.ID;
    private static bool InPhoenix => Service.IconReplacer.OriginalHook(25822) == Rekindle.ID;
    private static bool InBreak => InBahamut || InPhoenix || !SummonBahamut.EnoughLevel;

    public static readonly SMNAction
        //��ʯҫ
        Gemshine = new(25883)
        {
            OtherCheck = b => JobGauge.Attunement > 0,
        },

        //��ʯ��
        PreciousBrilliance = new(25884)
        {
            OtherCheck = b => JobGauge.Attunement > 0,
        },

        //���� ���幥��
        Ruin = new(163),

        //���� ��Χ�˺�
        Outburst = new(16511);

    public static readonly BaseAction
        //��ʯ���ٻ�
        SummonCarbuncle = new(25798)
        {
            OtherCheck = b => !TargetUpdater.HavePet,
        },

        //����֮�� �Ÿ�
        SearingLight = new(25801)
        {
            OtherCheck = b => InCombat && !InBahamut && !InPhoenix
        },

        //�ػ�֮�� ���Լ�����
        RadiantAegis = new(25799),

        //ҽ��
        Physick = new(16230, true),

        //��̫���� 
        Aethercharge = new(25800)
        {
            OtherCheck = b => InCombat,
        },

        //�����ٻ�
        SummonBahamut = new(7427),

        //�챦ʯ�ٻ�
        SummonRuby = new(25802)
        {
            OtherCheck = b => JobGauge.IsIfritReady && !IsMoving,
        },

        //�Ʊ�ʯ�ٻ�
        SummonTopaz = new(25803)
        {
            OtherCheck = b => JobGauge.IsTitanReady,
        },

        //�̱�ʯ�ٻ�
        SummonEmerald = new(25804)
        {
            OtherCheck = b => JobGauge.IsGarudaReady,
        },


        //����
        Resurrection = new(173, true),

        //��������
        EnergyDrain = new(16508),

        //������ȡ
        EnergySiphon = new(16510),

        //���ñ���
        Fester = new(181),

        //ʹ��˱�
        Painflare = new(3578),

        //�پ�
        RuinIV = new(7426)
        {
            BuffsNeed = new[] { ObjectStatus.FurtherRuin },
        },

        //����ŷ�
        EnkindleBahamut = new(7429)
        {
            OtherCheck = b => InBahamut || InPhoenix,
        },

        //���Ǻ˱�
        Deathflare = new(3582)
        {
            OtherCheck = b => InBahamut,
        },

        //����֮��
        Rekindle = new(25830, true)
        {
            OtherCheck = b => InPhoenix,
        },

        //�������
        CrimsonCyclone = new(25835)
        {
            BuffsNeed = new[] { ObjectStatus.IfritsFavor },
        },

        //���ǿϮ
        CrimsonStrike = new(25885),

        //ɽ��
        MountainBuster = new(25836)
        {
            BuffsNeed = new[] { ObjectStatus.TitansFavor },
        },

        //��������
        Slipstream = new(25837)
        {
            BuffsNeed = new[] { ObjectStatus.GarudasFavor },
        };
    public override SortedList<DescType, string> Description => new()
    {
        {DescType.�������, $"{RadiantAegis}"},
        {DescType.��������, $"{Physick}"},
    };

    private protected override bool MoveGCD(out IAction act)
    {
        if (CrimsonCyclone.ShouldUse(out act, mustUse: true)) return true;
        return base.MoveGCD(out act);
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //��ʯ���ٻ�
        if (SummonCarbuncle.ShouldUse(out act)) return true;

        //����
        if (!InBahamut && !InPhoenix)
        {
            if (RuinIV.ShouldUse(out act, mustUse: true)) return true;
            if (CrimsonStrike.ShouldUse(out act, mustUse: true)) return true;
            if (CrimsonCyclone.ShouldUse(out act, mustUse: true))
            {
                if (CrimsonCyclone.Target.DistanceToPlayer() < 2)
                {
                    return true;
                }
            }
            if (Slipstream.ShouldUse(out act, mustUse: true)) return true;
        }


        //�ٻ�
        if (JobGauge.Attunement == 0)
        {
            if (SummonBahamut.ShouldUse(out act))
            {
                if (SearingLight.IsCoolDown || !SearingLight.EnoughLevel)
                    return true;
            }
            else if (Aethercharge.ShouldUse(out act)) return true;

            if (JobGauge.IsIfritReady && JobGauge.IsGarudaReady && JobGauge.IsTitanReady ? JobGauge.SummonTimerRemaining == 0 : true)
            {
                switch (Config.GetComboByName("SummonOrder"))
                {
                    default:
                        //�� ��
                        if (SummonRuby.ShouldUse(out act)) return true;
                        //�� ��
                        if (SummonTopaz.ShouldUse(out act)) return true;
                        //�� ��
                        if (SummonEmerald.ShouldUse(out act)) return true;
                        break;
                    case 1:
                        //�� ��
                        if (SummonRuby.ShouldUse(out act)) return true;
                        //�� ��
                        if (SummonEmerald.ShouldUse(out act)) return true;
                        //�� ��
                        if (SummonTopaz.ShouldUse(out act)) return true;
                        break;
                    case 2:
                        //�� ��
                        if (SummonTopaz.ShouldUse(out act)) return true;
                        //�� ��
                        if (SummonEmerald.ShouldUse(out act)) return true;
                        //�� ��
                        if (SummonRuby.ShouldUse(out act)) return true;
                        break;
                    case 3:
                        //�� ��
                        if (SummonTopaz.ShouldUse(out act)) return true;
                        //�� ��
                        if (SummonRuby.ShouldUse(out act)) return true;
                        //�� ��
                        if (SummonEmerald.ShouldUse(out act)) return true;
                        break;
                    case 4:
                        //�� ��
                        if (SummonEmerald.ShouldUse(out act)) return true;
                        //�� ��
                        if (SummonRuby.ShouldUse(out act)) return true;
                        //�� ��
                        if (SummonTopaz.ShouldUse(out act)) return true;
                        break;
                    case 5:
                        //�� ��
                        if (SummonEmerald.ShouldUse(out act)) return true;
                        //�� ��
                        if (SummonTopaz.ShouldUse(out act)) return true;
                        //�� ��
                        if (SummonRuby.ShouldUse(out act)) return true;
                        break;
                }
            }
        }

        //AOE
        if (PreciousBrilliance.ShouldUse(out act)) return true;
        if (Outburst.ShouldUse(out act)) return true;

        //����
        if (Gemshine.ShouldUse(out act)) return true;
        if (Ruin.ShouldUse(out act)) return true;
        return false;
    }
    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetCombo("SummonOrder", 0, new string[]
        {
            "��-��-��", "��-��-��", "��-��-��", "��-��-��", "��-��-��", "��-��-��",

        }, "�����ٻ�˳��");
    }
    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak)
        {
            //����֮��
            if (SearingLight.ShouldUse(out act, mustUse: true)) return true;
        }

        if (EnkindleBahamut.ShouldUse(out act, mustUse: true)) return true;
        if (Deathflare.ShouldUse(out act, mustUse: true)) return true;
        if (Rekindle.ShouldUse(out act, mustUse: true)) return true;
        if (MountainBuster.ShouldUse(out act, mustUse: true)) return true;


        //��������
        if (JobGauge.HasAetherflowStacks && InBreak)
        {
            if (Painflare.ShouldUse(out act)) return true;
            if (Fester.ShouldUse(out act)) return true;
        }
        else
        {
            if (EnergySiphon.ShouldUse(out act)) return true;
            if (EnergyDrain.ShouldUse(out act)) return true;
        }

        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        //�ػ�֮��
        if (RadiantAegis.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(out IAction act)
    {
        //ҽ��
        if (Physick.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //����
        if (GeneralActions.Addle.ShouldUse(out act)) return true;
        return false;
    }
}
