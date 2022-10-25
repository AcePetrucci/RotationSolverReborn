using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.Melee;

internal sealed class MNKCombo : JobGaugeCombo<MNKGauge>
{
    internal override uint JobID => 20;

    internal struct Actions
    {
        public static readonly BaseAction
            //˫����
            DragonKick = new (74)
            {
                BuffsProvide = new [] { ObjectStatus.LeadenFist },
            },

            //����
            Bootshine = new (53),

            //�ƻ���� aoe
            ArmoftheDestroyer = new (62),

            //˫�ƴ� �˺����
            TwinSnakes = new (61),

            //��ȭ
            TrueStrike = new (54),

            //����� aoe
            FourpointFury = new (16473),

            //����ȭ
            Demolish = new(66, isDot:true)
            {
                TargetStatus = new ushort[] { ObjectStatus.Demolish },
            },

            //��ȭ
            SnapPunch = new (56),

            //���Ҿ� aoe
            Rockbreaker = new (70),

            //����
            Meditation = new (3546),

            //��ɽ��
            SteelPeak = new (25761)
            {
                OtherCheck = b => InBattle,
            },

            //����ȭ
            HowlingFist = new (25763)
            {
                OtherCheck = b => InBattle,
            },

            //������
            Brotherhood = new (7396, true),

            //�������� ���dps
            RiddleofFire = new (7395),

            //ͻ������
            Thunderclap = new (25762, shouldEndSpecial: true)
            {
                ChoiceTarget = TargetFilter.FindMoveTarget,
            },

            //����
            Mantra = new (65, true),

            //���
            PerfectBalance = new (69)
            {
                BuffsNeed = new ushort[] { ObjectStatus.RaptorForm },
                OtherCheck = b => InBattle,
            },

            //������ ��
            ElixirField = new (3545),

            //���ѽ� ��
            FlintStrike = new (25882),
            //�����
            RisingPhoenix = new (25768),


            //��������� ����
            TornadoKick = new (3543),
            PhantomRush = new (25769),

            //����
            FormShift = new(4262)
            {
                BuffsProvide = new[] { ObjectStatus.FormlessFist, ObjectStatus.PerfectBalance },
            },

            //��ռ��� ��
            RiddleofEarth = new (7394, shouldEndSpecial: true)
            {
                BuffsProvide = new[] {ObjectStatus.RiddleofEarth }, 
            },

            //���缫��
            RiddleofWind = new (25766);
    }

    internal override SortedList<DescType, string> Description => new ()
    {
        {DescType.��Χ����, $"{Actions.Mantra.Action.Name}"},
        {DescType.�������, $"{Actions.RiddleofEarth.Action.Name}"},
        {DescType.�ƶ�, $"{Actions.Thunderclap.Action.Name}"},
    };

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("AutoFormShift", true, "�Զ�����");
    }

    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.RiddleofFire.ShouldUse(out act)) return true;
        if (Actions.Brotherhood.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.Mantra.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.RiddleofEarth.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        if (GeneralActions.Feint.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.Thunderclap.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }


    private bool OpoOpoForm(out IAction act)
    {
        if (Actions.ArmoftheDestroyer.ShouldUse(out act)) return true;
        if (Actions.DragonKick.ShouldUse(out act)) return true;
        if (Actions.Bootshine.ShouldUse(out act)) return true;
        return false;
    }


    private bool RaptorForm(out IAction act)
    {
        if (Actions.FourpointFury.ShouldUse(out act)) return true;

        //ȷ��Buff
        if (LocalPlayer.WillStatusEndGCD(3, 0, true, ObjectStatus.DisciplinedFist) && Actions.TwinSnakes.ShouldUse(out act)) return true;

        if (Actions.TrueStrike.ShouldUse(out act)) return true;
        return false;
    }

    private bool CoerlForm(out IAction act)
    {
        if (Actions.Rockbreaker.ShouldUse(out act)) return true;
        if (Actions.Demolish.ShouldUse(out act)) return true;
        if (Actions.SnapPunch.ShouldUse(out act)) return true;
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

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        bool havesolar = (JobGauge.Nadi & Dalamud.Game.ClientState.JobGauge.Enums.Nadi.SOLAR) != 0;
        bool havelunar = (JobGauge.Nadi & Dalamud.Game.ClientState.JobGauge.Enums.Nadi.LUNAR) != 0;

        //���˵Ļ�������������
        if (!JobGauge.BeastChakra.Contains(Dalamud.Game.ClientState.JobGauge.Enums.BeastChakra.NONE))
        {
            if (havesolar && havelunar)
            {
                if (Actions.PhantomRush.ShouldUse(out act, mustUse: true)) return true;
                if (Actions.TornadoKick.ShouldUse(out act, mustUse: true)) return true;
            }
            if (JobGauge.BeastChakra.Contains(Dalamud.Game.ClientState.JobGauge.Enums.BeastChakra.RAPTOR))
            {
                if (Actions.RisingPhoenix.ShouldUse(out act, mustUse: true)) return true;
                if (Actions.FlintStrike.ShouldUse(out act, mustUse: true)) return true;
            }
            else
            {
                if (Actions.ElixirField.ShouldUse(out act, mustUse: true)) return true;
            }
        }
        //����ž�����
        else if (LocalPlayer.HaveStatus(ObjectStatus.PerfectBalance))
        {
            if (havesolar && LunarNadi(out act)) return true;
            if (SolarNadi(out act)) return true;
        }

        if (LocalPlayer.HaveStatus(ObjectStatus.CoerlForm))
        {
            if (CoerlForm(out act)) return true;
        }
        else if (LocalPlayer.HaveStatus(ObjectStatus.RaptorForm))
        {
            if (RaptorForm(out act)) return true;
        }
        else
        {
            if (OpoOpoForm(out act)) return true;
        }

        if (IconReplacer.Move && MoveAbility(1, out act)) return true;
        if (JobGauge.Chakra < 5 && Actions.Meditation.ShouldUse(out act)) return true;
        if (Config.GetBoolByName("AutoFormShift") && Actions.FormShift.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        //���
        if (JobGauge.BeastChakra.Contains(Dalamud.Game.ClientState.JobGauge.Enums.BeastChakra.NONE))
        {
            //��������
            if ((JobGauge.Nadi & Dalamud.Game.ClientState.JobGauge.Enums.Nadi.SOLAR) != 0)
            {
                //����Buff����6s����
                var dis = LocalPlayer.WillStatusEndGCD(3, 0, true, ObjectStatus.DisciplinedFist);

                Actions.Demolish.ShouldUse(out _);
                var demo = Actions.Demolish.Target.WillStatusEndGCD(3, 0, true, ObjectStatus.Demolish);

                if (!dis && (!demo || !Actions.PerfectBalance.IsCoolDown))
                {
                    if (Actions.PerfectBalance.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
                }
            }
            else
            {
                if (Actions.PerfectBalance.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
            }
        }

        if (Actions.RiddleofWind.ShouldUse(out act)) return true;

        if (JobGauge.Chakra == 5)
        {
            if (Actions.HowlingFist.ShouldUse(out act)) return true;
            if (Actions.SteelPeak.ShouldUse(out act)) return true;
            if (Actions.HowlingFist.ShouldUse(out act, mustUse: true)) return true;
        }

        return false;
    }
}
