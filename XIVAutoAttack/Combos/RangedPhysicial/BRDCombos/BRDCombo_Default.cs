using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Attributes;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using static XIVAutoAttack.Combos.RangedPhysicial.BRDCombos.BRDCombo_Default;

namespace XIVAutoAttack.Combos.RangedPhysicial.BRDCombos;

[ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/XIVAutoAttack/Combos/RangedPhysicial/BRDCombos/BRDCombo_Default.cs")]
internal sealed class BRDCombo_Default : BRDCombo<CommandType>
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
        if (ApexArrow.ShouldUse(out act, mustUse: true)) return true;

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
            (!RagingStrikes.EnoughLevel || Player.HaveStatusFromSelf(StatusID.RagingStrikes)) &&
            (!BattleVoice.EnoughLevel || Player.HaveStatusFromSelf(StatusID.BattleVoice)))
        {
            //���Ҽ�
            if (Barrage.ShouldUse(out act)) return true;
        }

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak && JobGauge.Song != Song.NONE && MagesBallad.EnoughLevel)
        {

            //����ǿ��
            if (RagingStrikes.ShouldUse(out act)) return true;

            //���������������
            if (abilityRemain == 2 && RadiantFinale.ShouldUse(out act, mustUse: true)) return true;

            //ս��֮��
            if (BattleVoice.ShouldUse(out act, mustUse: true))
            {
                if (RadiantFinale.IsCoolDown && RadiantFinale.EnoughLevel) return true;
                if (RagingStrikes.IsCoolDown && RagingStrikes.ElapsedAfterGCD(1) && !RadiantFinale.EnoughLevel) return true;
            }
        }

        if (RadiantFinale.IsCoolDown && !RadiantFinale.ElapsedAfterGCD())
        {
            act = null;
            return false;
        }
        //�������С������
        if ((JobGauge.Song == Song.NONE || (JobGauge.Song != Song.NONE || Player.HaveStatusFromSelf(StatusID.ArmyEthos)) && abilityRemain == 1)
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
            if (Player.HaveStatusFromSelf(StatusID.BattleVoice) && (Player.HaveStatusFromSelf(StatusID.RadiantFinale) || !RadiantFinale.EnoughLevel)) return true;

            if (!BattleVoice.WillHaveOneCharge(10, false) && !RadiantFinale.WillHaveOneCharge(10, false)) return true;

            if (RagingStrikes.IsCoolDown && !Player.HaveStatusFromSelf(StatusID.RagingStrikes)) return true;
        }

        //����������û�п�����ǿ����ս��֮��
        bool empty = Player.HaveStatusFromSelf(StatusID.RagingStrikes)
            && (Player.HaveStatusFromSelf(StatusID.BattleVoice)
            || !BattleVoice.EnoughLevel) || JobGauge.Song == Song.MAGE;
        //��������
        if (RainofDeath.ShouldUse(out act, emptyOrSkipCombo: empty)) return true;

        //ʧѪ��
        if (Bloodletter.ShouldUse(out act, emptyOrSkipCombo: empty)) return true;

        return false;
    }
}
