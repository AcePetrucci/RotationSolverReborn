using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Enums;
using System.Linq;
using XIVComboPlus;
using XIVComboPlus.Combos;

namespace XIVComboPlus.Combos.BLM;

internal class BLM_SingleFeature : BLMCombo
{
    public override string ComboFancyName => "����Ŀ��GCD";

    public override string Description => "�滻��1Ϊ������GCDѭ����";

    protected internal override uint[] ActionIDs => new uint[] { Actions.Fire.ActionID };

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        uint act;
        if (IsMoving)
        {
            //������ƶ�������Ŀ�ꡣ
            if (HaveValidTarget)
            {
                if (Actions.Xenoglossy.TryUseAction(level, out act)) return act;
                if (Actions.Triplecast.TryUseAction(level, out act)) return act;
                if (GeneralActions.Swiftcast.TryUseAction(level, out act)) return act;
            }
            //������ƶ�������û��Ŀ�ꡣ
            else
            {
                if (Actions.UmbralSoul.TryUseAction(level, out act))
                {
                    if (level < Actions.Paradox.Level)
                    {
                        return act;
                    }
                    else
                    {
                        if (JobGauge.UmbralIceStacks > 2 && JobGauge.UmbralHearts > 2)
                        {
                            return act;
                        }
                    }
                }
                return Actions.Transpose.ActionID;
            }
        }

        if (CanAddAbility(level, out act)) return act;
        if (MantainceState(level, lastComboMove, out act)) return act;
        if (AttackAndExchange(level, out act)) return act;
        return actionID;
    }

    private bool AttackAndExchange(byte level, out uint act)
    {
        if (JobGauge.InUmbralIce)
        {
            if (Actions.Blizzard4.TryUseAction(level, out act)) return true;
            if (Actions.Blizzard.TryUseAction(level, out act)) return true;
        }
        else if (JobGauge.InAstralFire)
        {
            //���û���ˣ���ֱ�ӱ�״̬��
            if (Service.ClientState.LocalPlayer.CurrentMp == 0)
            {
                if (AddUmbralIceStacks(level, out act)) return true;
            }
            //����������ˣ��Ͻ�һ��������
            if (Service.ClientState.LocalPlayer.CurrentMp < Actions.Fire4.MPNeed + Actions.Despair.MPNeed)
            {
                if (Actions.Despair.TryUseAction(level, out act)) return true;
            }

            //���MP����һ���˺���
            if (Service.ClientState.LocalPlayer.CurrentMp >= AttackAstralFire(level, out act))
            {
                return true;
            }
            //����ת���״̬��
            else
            {
                if (AddUmbralIceStacks(level, out act)) return true;
            }
        }

        act = 0;
        return false;
    }

    /// <summary>
    /// In AstralFire, maintain the time.
    /// </summary>
    /// <param name="level"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private uint AttackAstralFire(byte level, out uint act)
    {
        uint addition = level < Actions.Despair.Level ? 0u : 800u;

        //���ͨ�����ˣ��ͷŵ���
        if (IsPolyglotStacksFull)
        {
            if (Actions.Xenoglossy.TryUseAction(level, out act)) return addition;
            if (Actions.Foul.TryUseAction(level, out act)) return addition;
        }

        if (Actions.Fire4.TryUseAction(level, out act)) return Actions.Fire4.MPNeed + addition;
        if (Actions.Paradox.TryUseAction(level, out act)) return Actions.Paradox.MPNeed + addition;
        //����л����ˣ��Ǿ�����3
        if (BaseAction.HaveStatus(BaseAction.FindStatusSelfFromSelf(ObjectStatus.Firestarter)))
        {
            act = Actions.Fire3.ActionID;
            return addition;
        }
        if (Actions.Fire.TryUseAction(level, out act)) return Actions.Fire.MPNeed + addition;
        return uint.MaxValue;
    }

    /// <summary>
    /// ��֤���������������֤���ף�������������Ͻ�ת��
    /// </summary>
    /// <param name="level"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool MantainceState(byte level, uint lastAct, out uint act)
    {
        if (JobGauge.InUmbralIce)
        {
            if (HaveEnoughMP && (JobGauge.UmbralHearts == 3 || level < 58))
            {
                if (AddAstralFireStacks(level, out act)) return true;
            }

            if (AddUmbralIceStacks(level, out act)) return true;
            if (AddUmbralHeartsSingle(level, out act)) return true;
            if (AddThunderSingle(level, lastAct, out act)) return true;
        }
        else if (JobGauge.InAstralFire)
        {
            if (AddAstralFireStacks(level, out act)) return true;
            if (AddThunderSingle(level, lastAct, out act)) return true;
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
        if (JobGauge.UmbralIceStacks > 2)return false;

        //���Կ���3
        if (Actions.Blizzard3.TryUseAction(level, out act)) return true;

        //���Կ���1
        if (Actions.Blizzard.TryUseAction(level, out act)) return true;

        act = Actions.Transpose.ActionID;
        return true;
    }

    private bool AddAstralFireStacks(byte level, out uint act)
    {
        //��������ˣ��ͱ���ˡ�
        act = 0;
        if (JobGauge.AstralFireStacks > 2) return false;

        if(Service.ClientState.LocalPlayer.CurrentMp < 5000)
        {
            if(AddUmbralIceStacks(level, out act)) return true;
        }

        //���Կ���3
        if (Actions.Fire3.TryUseAction(level, out act)) return true;

        //���ʱ�乻��1
        if (JobGauge.ElementTimeRemaining > 2500 && level >= Actions.Blizzard3.Level)
        {
            if (Actions.Fire.TryUseAction(level, out act)) return true;
        }
        else
        {
            if (Actions.Blizzard3.TryUseAction(level, out act)) return true;
        }

        act = Actions.Transpose.ActionID;
        return true;
    }

    private bool AddThunderSingle(byte level, uint lastAct, out uint act)
    {
        //���Կ���1
        if (Actions.Thunder.TryUseAction(level, out act, lastAct)) return true;

        return false;
    }

    private bool AddUmbralHeartsSingle(byte level, out uint act)
    {
        //������ˣ����ߵȼ�̫�ͣ�û�б��ģ��ͱ���ˡ�
        act = 0;
        if (JobGauge.UmbralHearts == 3 || level < 58) return false;

        //��4
        if (Actions.Blizzard4.TryUseAction(level, out act)) return true;

        return false;
    }

}
