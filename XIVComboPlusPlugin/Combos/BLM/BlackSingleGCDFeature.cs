using Dalamud.Game.ClientState.JobGauge.Types;
using XIVComboPlus;
using XIVComboPlus.Combos;

namespace XIVComboPlus.Combos.BLM;

internal class BlackSingleGCDFeature : BLMCombo
{
    public override string ComboFancyName => "����Ŀ��GCD";

    public override string Description => "�滻��1Ϊ������GCDѭ����";

    protected internal override uint[] ActionIDs => new uint[] { Actions.Fire };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {

        if(CanAddAbility(level, out uint act)) return act;

        //��״̬
        if (JobGauge.InUmbralIce)
        {
            // 35�����£�{��1������} ����
            if (level < Levels.Fire3)
            {
                if (TargetBuffDuration(Debuffs.Thunder) < 10f)
                {
                    return Actions.Thunder;
                }
                else if (HaveEnoughMP)
                {
                    return Actions.Transpose;
                }
                return Actions.Blizzard;
            }
            // 60�����£���3 {��û�����Դ��1} ��3/1
            else if (level < Levels.Fire4)
            {
                //�� Dot
                if (level < Levels.Thunder3)
                {
                    if (TargetBuffDuration(Debuffs.Thunder) < 10f && lastComboMove != Actions.Thunder)
                    {
                        return Actions.Thunder;
                    }
                }
                else
                {
                    if (TargetBuffDuration(Debuffs.Thunder3) < 10f && lastComboMove != Actions.Thunder3)
                    {
                        return Actions.Thunder3;
                    }
                }

                if (HaveEnoughMP)
                {
                    return Actions.Fire3;
                }


                if (level > Levels.Blizzard4)
                    return Actions.Blizzard4;

                return Actions.Blizzard;
            }
            // 89�����£���4 ��3 ��3
            else if (level < Levels.Paradox)
            {
                if (TargetBuffDuration(Debuffs.Thunder3) < 10f && lastComboMove != Actions.Thunder3)
                {
                    return Actions.Thunder3;
                }

                if (HaveEnoughMP && JobGauge.UmbralHearts == 3)
                {
                    return Actions.Fire3;
                }


                return Actions.Blizzard4;
            }
            //90 ��
            else
            {
                if (HaveEnoughMP)
                {
                    return Actions.Fire3;
                }

                if (JobGauge.UmbralHearts < 3)
                {
                    return Actions.Blizzard4;
                }

                if (JobGauge.IsParadoxActive)
                {
                    return Actions.Paradox;
                }

                return Actions.Thunder3;
            }
        }
        //��״̬
        else if (JobGauge.InAstralFire)
        {
            // 35�����£�{��1��û��} ����
            if (level < Levels.Fire3)
            {
                if (TargetBuffDuration(Debuffs.Thunder) < 10f)
                {
                    return Actions.Thunder;
                }
                else if (LocalPlayer.CurrentMp < 800)
                {
                    return Actions.Transpose;
                }
                return Actions.Fire;
            }
            // 60�����£���3 {��1�����������ٴ��1}
            else if (level < Levels.Fire4)
            {
                if (LocalPlayer.CurrentMp < 1600)
                {
                    return Actions.Blizzard3;
                }

                if (BuffDuration(Buffs.Firestarter) > 0)
                {
                    return Actions.Fire3;
                }

                return Actions.Fire;
            }
            // 89�����£���4 x 3 ��1 ��4 x 3 ��3
            else if (level < Levels.Paradox)
            {
                //ʱ�䲻�����Ͻ���1
                if (JobGauge.ElementTimeRemaining < 4000)
                {
                    if (LocalPlayer.CurrentMp > 3000)
                        return Actions.Fire;

                    else if (level < Levels.Despair && LocalPlayer.CurrentMp < 800)
                        return Actions.Blizzard3;

                    else return Actions.Despair;
                }

                //���ͨ��̫�࣬�Ͷ�����
                switch (JobGauge.PolyglotStacks)
                {
                    case 1:
                        if(level < Levels.Xenoglossy)
                        {
                            return Actions.Foul;
                        }
                        break;
                    case 2:
                        return Actions.Xenoglossy;
                }

                if (LocalPlayer.CurrentMp < 1600)
                {
                    if (level >= Levels.Despair && LocalPlayer.CurrentMp >= 800)
                    {
                        return Actions.Despair;
                    }
                    return Actions.Blizzard3;
                }

                //���û�����ˣ��Ͳ��ϣ�
                if (TargetBuffDuration(Debuffs.Thunder3) < 10f && lastComboMove != Actions.Thunder3)
                {
                    return Actions.Thunder3;
                }

                return Actions.Fire4;
            }

            //90����
            else
            {
                if (JobGauge.PolyglotStacks == 2)
                {
                    return Actions.Xenoglossy;
                }

                if (LocalPlayer.CurrentMp < 1600)
                {
                    if (level >= Levels.Despair)
                    {
                        return Actions.Despair;
                    }
                    return Actions.Blizzard3;
                }

                //ʱ�䲻�����Ͻ���ۻ��1
                if (JobGauge.ElementTimeRemaining < 5000)
                {
                    if (JobGauge.IsParadoxActive)
                    {
                        return Actions.Paradox;
                    }
                    return Actions.Fire;
                }

                if (TargetBuffDuration(Debuffs.Thunder3) < 10f)
                {
                    return Actions.Thunder3;
                }

                return Actions.Fire4;
            }
        }

        if (level > Levels.Blizzard3)
        {
            return Actions.Blizzard3;
        }
        else
        {
            return Actions.Blizzard;
        }
        return actionID;
    }
}
