using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Attributes;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using static XIVAutoAttack.Combos.Melee.MNKCombo;

namespace XIVAutoAttack.Combos.Melee;

[ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/XIVAutoAttack/Combos/Melee/MNKCombo.cs")]
internal sealed class MNKCombo : JobGaugeCombo<MNKGauge, CommandType>
{
    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //д��ע�Ͱ���������ʾ�û��ġ�
    };
    public override uint[] JobIDs => new uint[] { 20, 2 };


    public static readonly BaseAction
        //˫����
        DragonKick = new(74)
        {
            BuffsProvide = new[] { ObjectStatus.LeadenFist },
        },

        //����
        Bootshine = new(53),

        //�ƻ���� aoe
        ArmoftheDestroyer = new(62),

        //˫�ƴ� �˺����
        TwinSnakes = new(61),

        //��ȭ
        TrueStrike = new(54),

        //����� aoe
        FourpointFury = new(16473),

        //����ȭ
        Demolish = new(66, isDot: true)
        {
            TargetStatus = new ushort[] { ObjectStatus.Demolish },
        },

        //��ȭ
        SnapPunch = new(56),

        //���Ҿ� aoe
        Rockbreaker = new(70),

        //����
        Meditation = new(3546),

        //��ɽ��
        SteelPeak = new(25761)
        {
            OtherCheck = b => InCombat,
        },

        //����ȭ
        HowlingFist = new(25763)
        {
            OtherCheck = b => InCombat,
        },

        //������
        Brotherhood = new(7396, true),

        //�������� ���dps
        RiddleofFire = new(7395),

        //ͻ������
        Thunderclap = new(25762, shouldEndSpecial: true)
        {
            ChoiceTarget = TargetFilter.FindTargetForMoving,
        },

        //����
        Mantra = new(65, true),

        //���
        PerfectBalance = new(69)
        {
            BuffsNeed = new ushort[] { ObjectStatus.RaptorForm },
            OtherCheck = b => InCombat,
        },

        //������ ��
        ElixirField = new(3545),

        //���ѽ� ��
        FlintStrike = new(25882),

        //�����
        RisingPhoenix = new(25768),

        //��������� ����
        TornadoKick = new(3543),
        PhantomRush = new(25769),

        //����
        FormShift = new(4262)
        {
            BuffsProvide = new[] { ObjectStatus.FormlessFist, ObjectStatus.PerfectBalance },
        },

        //��ռ��� ��
        RiddleofEarth = new(7394, shouldEndSpecial: true)
        {
            BuffsProvide = new[] { ObjectStatus.RiddleofEarth },
        },

        //���缫��
        RiddleofWind = new(25766);

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.��Χ����, $"{Mantra}"},
        {DescType.�������, $"{RiddleofEarth}"},
        {DescType.�ƶ�����, $"{Thunderclap}"},
    };

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("AutoFormShift", true, "�Զ�����");
    }

    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        if (Mantra.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (RiddleofEarth.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        if (GeneralActions.Feint.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (Thunderclap.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }


    private bool OpoOpoForm(out IAction act)
    {
        if (ArmoftheDestroyer.ShouldUse(out act)) return true;
        if (DragonKick.ShouldUse(out act)) return true;
        if (Bootshine.ShouldUse(out act)) return true;
        return false;
    }


    private bool RaptorForm(out IAction act)
    {
        if (FourpointFury.ShouldUse(out act)) return true;

        //ȷ��Buff
        if (Player.WillStatusEndGCD(3, 0, true, ObjectStatus.DisciplinedFist) && TwinSnakes.ShouldUse(out act)) return true;

        if (TrueStrike.ShouldUse(out act)) return true;
        return false;
    }

    private bool CoerlForm(out IAction act)
    {
        if (Rockbreaker.ShouldUse(out act)) return true;
        if (Demolish.ShouldUse(out act)) return true;
        if (SnapPunch.ShouldUse(out act)) return true;
        return false;
    }

    private bool LunarNadi(out IAction act)
    {
        if (OpoOpoForm(out act)) return true;
        return false;
    }

    private bool SolarNadi(out IAction act)
    {
        if (!JobGauge.BeastChakra.Contains(Dalamud.Game.ClientState.JobGauge.Enums.BeastChakra.RAPTOR))
        {
            if (RaptorForm(out act)) return true;
        }
        else if (!JobGauge.BeastChakra.Contains(Dalamud.Game.ClientState.JobGauge.Enums.BeastChakra.OPOOPO))
        {
            if (OpoOpoForm(out act)) return true;
        }
        else
        {
            if (CoerlForm(out act)) return true;
        }

        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        bool havesolar = (JobGauge.Nadi & Dalamud.Game.ClientState.JobGauge.Enums.Nadi.SOLAR) != 0;
        bool havelunar = (JobGauge.Nadi & Dalamud.Game.ClientState.JobGauge.Enums.Nadi.LUNAR) != 0;

        //���˵Ļ�������������
        if (!JobGauge.BeastChakra.Contains(Dalamud.Game.ClientState.JobGauge.Enums.BeastChakra.NONE))
        {
            if (havesolar && havelunar)
            {
                if (PhantomRush.ShouldUse(out act, mustUse: true)) return true;
                if (TornadoKick.ShouldUse(out act, mustUse: true)) return true;
            }
            if (JobGauge.BeastChakra.Contains(Dalamud.Game.ClientState.JobGauge.Enums.BeastChakra.RAPTOR))
            {
                if (RisingPhoenix.ShouldUse(out act, mustUse: true)) return true;
                if (FlintStrike.ShouldUse(out act, mustUse: true)) return true;
            }
            else
            {
                if (ElixirField.ShouldUse(out act, mustUse: true)) return true;
            }
        }
        //����ž�����
        else if (Player.HaveStatus(ObjectStatus.PerfectBalance))
        {
            if (havesolar && LunarNadi(out act)) return true;
            if (SolarNadi(out act)) return true;
        }

        if (Player.HaveStatus(ObjectStatus.CoerlForm))
        {
            if (CoerlForm(out act)) return true;
        }
        else if (Player.HaveStatus(ObjectStatus.RaptorForm))
        {
            if (RaptorForm(out act)) return true;
        }
        else
        {
            if (OpoOpoForm(out act)) return true;
        }

        if (CommandController.Move && MoveAbility(1, out act)) return true;
        if (JobGauge.Chakra < 5 && Meditation.ShouldUse(out act)) return true;
        if (Config.GetBoolByName("AutoFormShift") && FormShift.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak)
        {
            if (RiddleofFire.ShouldUse(out act)) return true;
            if (Brotherhood.ShouldUse(out act)) return true;
        }

        //���
        if (JobGauge.BeastChakra.Contains(Dalamud.Game.ClientState.JobGauge.Enums.BeastChakra.NONE))
        {
            //��������
            if ((JobGauge.Nadi & Dalamud.Game.ClientState.JobGauge.Enums.Nadi.SOLAR) != 0)
            {
                //����Buff����6s����
                var dis = Player.WillStatusEndGCD(3, 0, true, ObjectStatus.DisciplinedFist);

                Demolish.ShouldUse(out _);
                var demo = Demolish.Target.WillStatusEndGCD(3, 0, true, ObjectStatus.Demolish);

                if (!dis && (!demo || !PerfectBalance.IsCoolDown))
                {
                    if (PerfectBalance.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
                }
            }
            else
            {
                if (PerfectBalance.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
            }
        }

        if (RiddleofWind.ShouldUse(out act)) return true;

        if (JobGauge.Chakra == 5)
        {
            if (HowlingFist.ShouldUse(out act)) return true;
            if (SteelPeak.ShouldUse(out act)) return true;
            if (HowlingFist.ShouldUse(out act, mustUse: true)) return true;
        }

        return false;
    }
}
