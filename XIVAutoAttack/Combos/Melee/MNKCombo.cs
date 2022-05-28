using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Linq;

namespace XIVComboPlus.Combos;

internal class MNKCombo : CustomComboJob<MNKGauge>
{
    internal override uint JobID => 20;
    protected override bool ShouldSayout => true;

    internal struct Actions
    {
        public static readonly BaseAction
            //˫����
            DragonKick = new BaseAction(74)
            {
                BuffsProvide = new ushort[] { ObjectStatus.LeadenFist },
            },

            //����
            Bootshine = new BaseAction(53),

            //�ƻ���� aoe
            ArmoftheDestroyer = new BaseAction(62),

            //˫�ƴ� �˺����
            TwinSnakes = new BaseAction(61),

            //��ȭ
            TrueStrike = new BaseAction(54),

            //����� aoe
            FourpointFury = new BaseAction(16473),

            //����ȭ
            Demolish = new BaseAction(66)
            {
                EnermyLocation = EnemyLocation.Back,
            },

            //��ȭ
            SnapPunch = new BaseAction(56)
            {
                EnermyLocation = EnemyLocation.Side,
            },

            //���Ҿ� aoe
            Rockbreaker = new BaseAction(70),

            //����
            Meditation = new BaseAction(3546),

            //��ɽ��
            SteelPeak = new BaseAction(25761),

            //����ȭ
            HowlingFist = new BaseAction(25763),

            //������
            Brotherhood = new BaseAction(7396, true),

            //�������� ���dps
            RiddleofFire = new BaseAction(7395),

            //ͻ������
            Thunderclap = new BaseAction(25762, shouldEndSpecial:true)
            {
                ChoiceFriend = BaseAction.FindMoveTarget,
            },

            //����
            Mantra = new BaseAction(65, true),

            //���
            PerfectBalance = new BaseAction(69)
            {
                OtherCheck = b => BaseAction.HaveStatusSelfFromSelf(ObjectStatus.RaptorForm) && TargetHelper.InBattle,
            },

            //������ ��
            ElixirField = new BaseAction(3545),

            //���ѽ� ��
            FlintStrike = new BaseAction(25882),

            //��������� ����
            TornadoKick = new BaseAction(3543),

            //����
            FormShift = new BaseAction(4262)
            {
                BuffsProvide = new ushort[] { ObjectStatus.FormlessFist, ObjectStatus.PerfectBalance },
            },

            //��ռ��� ��
            RiddleofEarth = new BaseAction(7394),

            //���缫��
            RiddleofWind = new BaseAction(25766);
    }

    private protected override bool BreakAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.RiddleofFire.ShouldUseAction(out act)) return true;
        if (Actions.Brotherhood.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool HealAreaAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.Mantra.ShouldUseAction(out act)) return true;
        return false;
    }


    private protected override bool DefenceSingleAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.RiddleofEarth.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out BaseAction act)
    {
        if (GeneralActions.Feint.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.Thunderclap.ShouldUseAction(out act, Empty:true)) return true;
        return false;
    }


    private bool OpoOpoForm(out BaseAction act)
    {
        if(Actions.ArmoftheDestroyer.ShouldUseAction(out act)) return true; 
        if (Actions.DragonKick.ShouldUseAction(out act)) return true;
        if (Actions.Bootshine.ShouldUseAction(out act)) return true;
        return false;
    }


    private bool RaptorForm(out BaseAction act)
    {
        if (Actions.FourpointFury.ShouldUseAction(out act)) return true;

        //ȷ��Buff����6s
        var times = BaseAction.FindStatusSelfFromSelf(ObjectStatus.DisciplinedFist);
        if ((times.Length == 0 || times[0] < 4 + WeaponRemain) && Actions.TwinSnakes.ShouldUseAction(out act)) return true;

        if (Actions.TrueStrike.ShouldUseAction(out act)) return true;
        return false;
    }

    private bool CoerlForm(out BaseAction act)
    {
        if (Actions.Rockbreaker.ShouldUseAction(out act)) return true;
        if (Actions.Demolish.ShouldUseAction(out act))
        {
            var times = BaseAction.FindStatusFromSelf(Actions.Demolish.Target, ObjectStatus.Demolish);
            if (times.Length == 0 || times[0] < 4 + WeaponRemain || Math.Abs(times[0] - 4 + WeaponRemain) < 0.1) return true;
        }
        if (Actions.SnapPunch.ShouldUseAction(out act)) return true;
        return false;
    }

    private bool LunarNadi(out BaseAction act)
    {
        if (OpoOpoForm(out act)) return true;
        return false;
    }

    private bool SolarNadi(out BaseAction act)
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

    private protected override bool GeneralGCD(uint lastComboActionID, out BaseAction act)
    {
        bool havesolar = (JobGauge.Nadi & Dalamud.Game.ClientState.JobGauge.Enums.Nadi.SOLAR) != 0;
        bool havelunar = (JobGauge.Nadi & Dalamud.Game.ClientState.JobGauge.Enums.Nadi.LUNAR) != 0;

        //���˵Ļ�������������
        if (!JobGauge.BeastChakra.Contains(Dalamud.Game.ClientState.JobGauge.Enums.BeastChakra.NONE))
        {
            if (havesolar && havelunar && Actions.TornadoKick.ShouldUseAction(out act, mustUse: true)) return true;
            if (JobGauge.BeastChakra.Contains(Dalamud.Game.ClientState.JobGauge.Enums.BeastChakra.RAPTOR))
            {
                if (Actions.FlintStrike.ShouldUseAction(out act, mustUse: true)) return true;
            }
            else
            {
                if (Actions.ElixirField.ShouldUseAction(out act, mustUse: true)) return true;
            }
        }
        //����ž�����
        else if(BaseAction.HaveStatusSelfFromSelf(ObjectStatus.PerfectBalance))
        {
            if (havesolar && LunarNadi(out act)) return true;
            if (SolarNadi(out act)) return true;
        }

        if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.CoerlForm))
        {
            if (CoerlForm(out act)) return true;
        }
        else if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.RaptorForm))
        {
            if (RaptorForm(out act)) return true;
        }
        else
        {
            if (OpoOpoForm(out act)) return true;
        }

        if (IconReplacer.Move && MoveAbility(1, out act)) return true;
        if (JobGauge.Chakra < 5 && Actions.Meditation.ShouldUseAction(out act)) return true;
        if (Actions.FormShift.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out BaseAction act)
    {
        //���
        if (JobGauge.BeastChakra.Contains(Dalamud.Game.ClientState.JobGauge.Enums.BeastChakra.NONE))
        {
            //��������
            if ((JobGauge.Nadi & Dalamud.Game.ClientState.JobGauge.Enums.Nadi.SOLAR) != 0)
            {
                //����Buff����6s����
                var dis = BaseAction.FindStatusSelfFromSelf(ObjectStatus.DisciplinedFist);
                Actions.Demolish.ShouldUseAction(out _);
                var demo = BaseAction.FindStatusFromSelf(Actions.Demolish.Target, ObjectStatus.Demolish);
                if (dis.Length != 0 && dis[0] > 6 && ((demo.Length != 0 && demo[0] > 6) || !Actions.PerfectBalance.IsCoolDown))
                {
                    if (Actions.PerfectBalance.ShouldUseAction(out act, Empty:true)) return true;
                }
            }
            else
            {
                if (Actions.PerfectBalance.ShouldUseAction(out act, Empty: true)) return true;
            }

        }

        if (Actions.RiddleofWind.ShouldUseAction(out act)) return true;

        if(JobGauge.Chakra == 5)
        {
            if (Actions.HowlingFist.ShouldUseAction(out act)) return true;
            if (Actions.SteelPeak.ShouldUseAction(out act)) return true;
            if (Actions.HowlingFist.ShouldUseAction(out act, mustUse: true)) return true;
        }

        return false;
    }
}
