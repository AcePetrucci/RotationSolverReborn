using XIVComboPlus;
using XIVComboPlus.Combos;

namespace XIVComboPlus.Combos.BLM;

internal class BLMLeyLinesFeature : BLMCombo
{
    public override string ComboFancyName => "�滻��ħ��Ϊħ�Ʋ�";

    public override string Description => "����ħ����������ʱ���ͰѺ�ħ�Ʊ��ħ�Ʋ���";

    protected internal override uint[] ActionIDs => new uint[] { Actions.Leylines.ActionID };

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if(Actions.BetweenTheLines.TryUseAction(level, out uint act)) return act;

        bool haveSwift = false;

        

        foreach (var buff in GeneralActions.Swiftcast.BuffsProvide)
        {
            if (BaseAction.HaveStatus(BaseAction.FindStatusSelfFromSelf(buff)))
            {
                haveSwift = true;
                break;
            }
        }
        if (haveSwift)
        {
            if(Actions.Leylines.TryUseAction(level, out act)) return act;
        }
        return Actions.BetweenTheLines.ActionID;
    }
}
