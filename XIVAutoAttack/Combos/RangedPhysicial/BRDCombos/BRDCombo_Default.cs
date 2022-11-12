using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Attributes;
using XIVAutoAttack.Combos.Basic;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using static XIVAutoAttack.Combos.RangedPhysicial.BRDCombos.BRDCombo_Default;

namespace XIVAutoAttack.Combos.RangedPhysicial.BRDCombos;

[ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/XIVAutoAttack/Combos/RangedPhysicial/BRDCombos/BRDCombo_Default.cs")]
internal sealed class BRDCombo_Default : BRDCombo_Base<CommandType>
{
    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //д��ע�Ͱ���������ʾ�û��ġ�
    };

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.��Χ����, $"{Troubadour}"},
        {DescType.��������, $"{NaturesMinne}"},
    };

    public override string Author => "ϫ��Moon";

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
        if (IronJaws.ShouldUse(out act)) return true;

        //�Ŵ��У�
        if (CanUseApexArrow(out act)) return true;

        //Ⱥ��GCD
        if (Shadowbite.ShouldUse(out act)) return true;
        if (QuickNock.ShouldUse(out act)) return true;

        //ֱ�����
        if (StraitShoot.ShouldUse(out act)) return true;

        //�϶�
        if (VenomousBite.ShouldUse(out act)) return true;
        if (Windbite.ShouldUse(out act)) return true;

        //ǿ�����
        if (HeavyShoot.ShouldUse(out act)) return true;

        return false;
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
            (!RagingStrikes.EnoughLevel || Player.HaveStatus(StatusID.RagingStrikes)) &&
            (!BattleVoice.EnoughLevel || Player.HaveStatus(StatusID.BattleVoice)))
        {
            if (EmpyrealArrow.IsCoolDown || !EmpyrealArrow.WillHaveOneChargeGCD() || JobGauge.Repertoire != 3 || !EmpyrealArrow.EnoughLevel)
            {
                //���Ҽ�
                if (Barrage.ShouldUse(out act)) return true;
            }      
        }

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak && JobGauge.Song != Song.NONE && MagesBallad.EnoughLevel)
        {

            //����ǿ��
            if (RagingStrikes.ShouldUse(out act))
            {
                if (JobGauge.Song != Song.NONE) return true;
            }

            //���������������
            if (abilityRemain == 2 && RadiantFinale.ShouldUse(out act, mustUse: true))
            {
                if (RagingStrikes.IsCoolDown && Player.HaveStatus(StatusID.RagingStrikes) && RagingStrikes.ElapsedAfterGCD(1)) return true;
            }

            //ս��֮��
            if (abilityRemain == 1 && BattleVoice.ShouldUse(out act, mustUse: true))
            {
                if (RagingStrikes.IsCoolDown && Player.HaveStatus(StatusID.RagingStrikes) && RagingStrikes.ElapsedAfterGCD(1)) return true;
            }
        }

        if (RadiantFinale.IsCoolDown && !RadiantFinale.ElapsedAfterGCD())
        {
            act = null;
            return false;
        }
        //�������С������
        if ((JobGauge.Song == Song.NONE || (JobGauge.Song != Song.NONE || Player.HaveStatus(StatusID.ArmyEthos)) && abilityRemain == 1)
            && JobGauge.SongTimer < 3000)
        {
            if (WanderersMinuet.ShouldUse(out act)) return true;
        }

        //��������
        if (JobGauge.Song != Song.NONE && EmpyrealArrow.ShouldUse(out act)) return true;

        //��������
        if (PitchPerfect.ShouldUse(out act))
        {
            if (JobGauge.SongTimer < 3000 && JobGauge.Repertoire > 0) return true;

            if (JobGauge.Repertoire == 3 || JobGauge.Repertoire == 2 && EmpyrealArrow.WillHaveOneChargeGCD(1)) return true;
        }

        //���ߵ�����ҥ
        if (JobGauge.SongTimer < 3000 && MagesBallad.ShouldUse(out act)) return true;

        //�����������
        if (JobGauge.SongTimer < 12000 && (JobGauge.Song == Song.MAGE
            || JobGauge.Song == Song.NONE) && ArmysPaeon.ShouldUse(out act)) return true;

        //����յ���
        if (Sidewinder.ShouldUse(out act))
        {
            if (Player.HaveStatus(StatusID.BattleVoice) && (Player.HaveStatus(StatusID.RadiantFinale) || !RadiantFinale.EnoughLevel)) return true;

            if (!BattleVoice.WillHaveOneCharge(10, false) && !RadiantFinale.WillHaveOneCharge(10, false)) return true;

            if (RagingStrikes.IsCoolDown && !Player.HaveStatus(StatusID.RagingStrikes)) return true;
        }

        //����������û�п�����ǿ����ս��֮��
        bool empty = Player.HaveStatus(StatusID.RagingStrikes)
            && (Player.HaveStatus(StatusID.BattleVoice)
            || !BattleVoice.EnoughLevel) || JobGauge.Song == Song.MAGE;

        if (EmpyrealArrow.IsCoolDown || !EmpyrealArrow.WillHaveOneChargeGCD() || JobGauge.Repertoire != 3 || !EmpyrealArrow.EnoughLevel)
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

        if (Player.HaveStatus(StatusID.BlastArrowReady) || (QuickNock.ShouldUse(out _) && JobGauge.SoulVoice == 100)) return true;

        //�챬����,���ŵȱ���
        if (JobGauge.SoulVoice == 100 && BattleVoice.WillHaveOneCharge(25)) return false;

        //���������,������ﻹ�о����,�ͰѾ�������ȥ
        if (JobGauge.SoulVoice >= 80 && Player.HaveStatus(StatusID.RagingStrikes) && Player.WillStatusEnd(10, false, StatusID.RagingStrikes)) return true;

        if (JobGauge.SoulVoice == 100
            && Player.HaveStatus(StatusID.RagingStrikes)
            && Player.HaveStatus(StatusID.BattleVoice)
            && (Player.HaveStatus(StatusID.RadiantFinale) || !RadiantFinale.EnoughLevel)) return true;

        if (JobGauge.Song == Song.MAGE && JobGauge.SoulVoice >= 80 && JobGauge.SongTimer < 22 && JobGauge.SongTimer > 18) return true;

        //����֮������100�����ڱ�����Ԥ��״̬
        if (!Player.HaveStatus(StatusID.RagingStrikes) && JobGauge.SoulVoice == 100) return true;

        return false;
    }
}
