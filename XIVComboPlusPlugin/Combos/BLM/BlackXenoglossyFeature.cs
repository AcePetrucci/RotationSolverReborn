using Dalamud.Game.ClientState.JobGauge.Types;
using XIVComboPlus;
using XIVComboPlus.Combos;

namespace XIVComboPlus.Combos.BLM;

internal class BlackXenoglossyFeature : BLMCombo
{
    public override string ComboFancyName => "�滻����Ϊ����";

    public override string Description => "����ȼ������ߣ��Ǿ��滻����Ϊ���ǡ�";

    protected internal override uint[] ActionIDs => new uint[] { Actions.Xenoglossy };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if(level < Levels.Xenoglossy)
        {
            return Actions.Foul;
        }
        return actionID;
    }
}
