using XIVComboPlus;
using XIVComboPlus.Combos;

namespace XIVComboPlus.Combos.BLM;

internal class BlackLeyLinesFeature : BLMCombo
{
    public override string ComboFancyName => "�滻��ħ��Ϊħ�Ʋ�";

    public override string Description => "����ħ����������ʱ���ͰѺ�ħ�Ʊ��ħ�Ʋ���";

    protected internal override uint[] ActionIDs => new uint[] { Actions.Leylines.ActionID };

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if(Actions.BetweenTheLines.TryUseAction(level, out uint act)) return act;
        return actionID;
    }
}
