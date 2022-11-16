using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game.Gauge;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Basic;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using static XIVAutoAttack.Combos.RangedPhysicial.BRDCombos.BRDCombo_Default;

namespace XIVAutoAttack.Combos.RangedPhysicial.BRDCombos;

internal sealed class BRDCombo_Default : BRDCombo_Base<CommandType>
{
    public override string Author => "ϫ��Moon";

    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //д��ע�Ͱ���������ʾ�û��ġ�
    };

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetCombo("FirstSong", 0, "��һ�׸�", "�����", "���߸�", "�����")
            .SetFloat("WANDTime", 43, "�����ʱ��", min: 0, max: 45, speed: 1)
            .SetFloat("MAGETime", 34, "���߸�ʱ��", min: 0, max: 45, speed: 1)
            .SetFloat("ARMYTime", 43, "���߸�ʱ��", min: 0, max: 45, speed: 1);
    }

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.ѭ��˵��, $"��ȷ�����׸�ʱ�����һ�����120��!"},
        {DescType.��Χ����, $"{Troubadour}"},
        {DescType.��������, $"{NaturesMinne}"},
    };

    private int FirstSong => Config.GetComboByName("FirstSong");
    private float WANDRemainTime => 45 - Config.GetFloatByName("WANDTime");
    private float MAGERemainTime => 45 - Config.GetFloatByName("MAGETime");
    private float ARMYRemainTime => 45 - Config.GetFloatByName("ARMYTime");

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //����
        if (Troubadour.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        //��������������
        if (NaturesMinne.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //��������
        if (IronJaws.ShouldUse(out act))
        {
            var b = IronJaws.Target;
            if (b.HasStatus(true, VenomousBite.TargetStatus) & b.HasStatus(true, Windbite.TargetStatus)
            & (b.WillStatusEndGCD((uint)Service.Configuration.AddDotGCDCount, 0, true, VenomousBite.TargetStatus)
            | b.WillStatusEndGCD((uint)Service.Configuration.AddDotGCDCount, 0, true, Windbite.TargetStatus))) return true;

            if (Player.HasStatus(true, StatusID.RagingStrikes) && Player.WillStatusEndGCD(1, 0, true, StatusID.RagingStrikes)) return true;
        }

        //�Ŵ��У�
        if (CanUseApexArrow(out act)) return true;

        //Ⱥ��GCD
        if (Shadowbite.ShouldUse(out act)) return true;
        if (QuickNock.ShouldUse(out act)) return true;

        //�϶�
        if (VenomousBite.ShouldUse(out act)) return true;
        if (Windbite.ShouldUse(out act)) return true;

        //ֱ�����
        if (StraitShoot.ShouldUse(out act)) return true;

        //ǿ�����
        if (HeavyShoot.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool GeneralAbility(byte abilityRemain, out IAction act)
    {
        //��ս����
        if (!InCombat && Player.WillStatusEndGCD(1) && Peloton.ShouldUse(out act)) return true;
        return base.GeneralAbility(abilityRemain, out act);
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //���������Ҫ�϶�����Ҫֱ������������ˡ�
        if (nextGCD.IsAnySameAction(true, StraitShoot, VenomousBite,
            Windbite, IronJaws))
        {
            return base.EmergercyAbility(abilityRemain, nextGCD, out act);
        }
        else if (abilityRemain != 0 &&
            (!RagingStrikes.EnoughLevel || Player.HasStatus(true, StatusID.RagingStrikes)) &&
            (!BattleVoice.EnoughLevel || Player.HasStatus(true, StatusID.BattleVoice)))
        {
            if (EmpyrealArrow.IsCoolDown || !EmpyrealArrow.WillHaveOneChargeGCD() || Repertoire != 3 || !EmpyrealArrow.EnoughLevel)
            {
                //���Ҽ�
                if (Barrage.ShouldUse(out act)) return true;
            }
        }

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak && Song != Song.NONE && MagesBallad.EnoughLevel)
        {

            //����ǿ��
            if (RagingStrikes.ShouldUse(out act))
            {
                if (Song != Song.NONE) return true;
            }

            //���������������
            if (abilityRemain == 2 && RadiantFinale.ShouldUse(out act, mustUse: true))
            {
                if (RagingStrikes.IsCoolDown && Player.HasStatus(true, StatusID.RagingStrikes) && RagingStrikes.ElapsedAfterGCD(1)) return true;
            }

            //ս��֮��
            if (abilityRemain == 1 && BattleVoice.ShouldUse(out act, mustUse: true))
            {
                if (RagingStrikes.IsCoolDown && Player.HasStatus(true, StatusID.RagingStrikes) && RagingStrikes.ElapsedAfterGCD(1)) return true;
            }
        }

        if (RadiantFinale.IsCoolDown && !RadiantFinale.ElapsedAfterGCD())
        {
            act = null;
            return false;
        }

        if (Song == Song.NONE)
        {
            if (FirstSong == 0 && WanderersMinuet.ShouldUse(out act)) return true;
            if (FirstSong == 1 && MagesBallad.ShouldUse(out act)) return true;
            if (FirstSong == 2 && ArmysPaeon.ShouldUse(out act)) return true;

        }

        //�������С������
        if ((Song != Song.NONE || Player.HasStatus(true, StatusID.ArmyEthos)) && abilityRemain == 1 && SongEndAfter(ARMYRemainTime))
        {
            if (WanderersMinuet.ShouldUse(out act)) return true;
        }

        //���ߵ�����ҥ
        if (SongEndAfter(WANDRemainTime) && MagesBallad.ShouldUse(out act)) return true;

        //�����������
        if (SongEndAfter(MAGERemainTime) && (Song == Song.MAGE || Song == Song.NONE) && ArmysPaeon.ShouldUse(out act)) return true;

        //��������
        if (Song != Song.NONE && EmpyrealArrow.ShouldUse(out act)) return true;

        //��������
        if (PitchPerfect.ShouldUse(out act))
        {
            if (SongEndAfter(3) && Repertoire > 0) return true;

            if (Repertoire == 3 || Repertoire == 2 && EmpyrealArrow.WillHaveOneChargeGCD(1)) return true;
        }

        //����յ���
        if (Sidewinder.ShouldUse(out act))
        {
            if (Player.HasStatus(true, StatusID.BattleVoice) && (Player.HasStatus(true, StatusID.RadiantFinale) || !RadiantFinale.EnoughLevel)) return true;

            if (!BattleVoice.WillHaveOneCharge(10) && !RadiantFinale.WillHaveOneCharge(10)) return true;

            if (RagingStrikes.IsCoolDown && !Player.HasStatus(true, StatusID.RagingStrikes)) return true;
        }

        //����������û�п�����ǿ����ս��֮��
        bool empty = Player.HasStatus(true, StatusID.RagingStrikes)
            && (Player.HasStatus(true, StatusID.BattleVoice)
            || !BattleVoice.EnoughLevel) || Song == Song.MAGE;

        if (EmpyrealArrow.IsCoolDown || !EmpyrealArrow.WillHaveOneChargeGCD() || Repertoire != 3 || !EmpyrealArrow.EnoughLevel)
        {
            //��������
            if (RainofDeath.ShouldUse(out act, emptyOrSkipCombo: empty)) return true;

            //ʧѪ��
            if (Bloodletter.ShouldUse(out act, emptyOrSkipCombo: empty)) return true;
        }

        return false;
    }

    private bool CanUseApexArrow(out IAction act)
    {
        //�Ŵ��У�
        if (!ApexArrow.ShouldUse(out act, mustUse: true)) return false;

        if (Player.HasStatus(true, StatusID.BlastArrowReady) || (QuickNock.ShouldUse(out _) && SoulVoice == 100)) return true;

        //�챬����,���ŵȱ���
        if (SoulVoice == 100 && BattleVoice.WillHaveOneCharge(25)) return false;

        //���������,������ﻹ�о����,�ͰѾ�������ȥ
        if (SoulVoice >= 80 && Player.HasStatus(true, StatusID.RagingStrikes) && Player.WillStatusEnd(10, false, StatusID.RagingStrikes)) return true;

        if (SoulVoice == 100
            && Player.HasStatus(true, StatusID.RagingStrikes)
            && Player.HasStatus(true, StatusID.BattleVoice)
            && (Player.HasStatus(true, StatusID.RadiantFinale) || !RadiantFinale.EnoughLevel)) return true;

        if (Song == Song.MAGE && SoulVoice >= 80 && SongEndAfter(22) && SongEndAfter(18)) return true;

        //����֮������100�����ڱ�����Ԥ��״̬
        if (!Player.HasStatus(true, StatusID.RagingStrikes) && SoulVoice == 100) return true;

        return false;
    }
}
