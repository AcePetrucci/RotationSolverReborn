using Dalamud.Game.ClientState.JobGauge.Types;
using XIVComboPlus;
using XIVComboPlus.Combos;

namespace XIVComboPlus.Combos.BLM;

internal class BlackAOEGCDFeature : BLMCombo
{
    public override string ComboFancyName => "Ⱥ��GCD";

    public override string Description => "�滻��2�ǳ�ţ�Ƶ�Ⱥ��GCD��";

    protected internal override uint[] ActionIDs => new uint[] { Actions.Fire2.ActionID };

    protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
    {
        if (CanAddAbility(level, out uint act)) return act;
        if (MantainceState(level, lastComboActionID, out act)) return act;
        if (AttackAndExchange(level, out act)) return act;
        return actionID;
    }

    private bool AttackAndExchange(byte level, out uint act)
    {
        if (JobGauge.InUmbralIce)
        {
            if (HaveEnoughMP)
            {
                if (AddAstralFireStacks(level, out act)) return true;
            }

            if (Actions.Freeze.TryUseAction(level, out act)) return true;
            if (Actions.Blizzard2.TryUseAction(level, out act)) return true;
        }
        else if (JobGauge.InAstralFire)
        {
            //���û���ˣ���ֱ�ӱ�״̬��
            if (Service.ClientState.LocalPlayer.CurrentMp == 0)
            {
                if (AddUmbralIceStacks(level, out act)) return true;
            }

            //���ͨ�����ˣ��ͷŵ���
            if (IsPolyglotStacksFull)
            {
                if (Actions.Foul.TryUseAction(level, out act)) return true;
            }

            //����������ˣ��Ͻ�һ��������
            if (level >= 58 && JobGauge.UmbralHearts < 2)
            {
                if (Actions.Flare.TryUseAction(level, out act)) return true;
            }

            //���Կ���2
            if (Actions.Fire2.TryUseAction(level, out act)) return true;

            //�����Կ�����
            if (Actions.Flare.TryUseAction(level, out act)) return true;

            //ɶ���Ų��˵Ļ���ת���״̬��
            if (AddUmbralIceStacks(level, out act)) return true;
        }

        act = 0;
        return false;
    }


    /// <summary>
    /// ��֤���������������֤����
    /// </summary>
    /// <param name="level"></param>
    /// <param name="lastAct"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool MantainceState(byte level, uint lastAct, out uint act)
    {
        if (JobGauge.InUmbralIce)
        {
            if (AddUmbralIceStacks(level, out act)) return true;
            if (AddUmbralHeartsArea(level, out act)) return true;
            if (AddThunderArea(level, lastAct, out act)) return true;
        }
        else if (JobGauge.InAstralFire)
        {
            if (AddAstralFireStacks(level, out act)) return true;
            if (AddThunderArea(level, lastAct, out act)) return true;
        }
        else
        {
            //û״̬���ͼӸ���״̬��
            if (AddUmbralIceStacks(level, out act)) return true;
        }

        return false;
    }

    private bool AddUmbralIceStacks(byte level, out uint act)
    {
        //��������ˣ��ͱ���ˡ�
        act = 0;
        if (JobGauge.UmbralIceStacks > 2) return false;

        //���Կ���2
        if (Actions.Blizzard2.TryUseAction(level, out act)) return true;

        return false;
    }

    private bool AddAstralFireStacks(byte level, out uint act)
    {
        //��������ˣ��ͱ���ˡ�
        act = 0;
        if (JobGauge.AstralFireStacks > 2) return false;

        //���Կ���2
        if (Actions.Fire2.TryUseAction(level, out act)) return true;

        return false;
    }

    private bool AddThunderArea(byte level, uint lastAct, out uint act)
    {
        //���Կ���2
        if (Actions.Thunder2.TryUseAction(level, out act, lastAct)) return true;

        return false;
    }

    private bool AddUmbralHeartsArea(byte level, out uint act)
    {
        //������ˣ��ͱ���ˡ�
        act = 0;
        if (JobGauge.UmbralHearts == 3 && level >= 58) return false;

        //����
        if (Actions.Freeze.TryUseAction(level, out act)) return true;

        return false;
    }
}
