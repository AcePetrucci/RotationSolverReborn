using Dalamud.Game.ClientState.JobGauge.Types;
using XIVComboPlus;
using XIVComboPlus.Combos;

namespace XIVComboPlus.Combos.BLM;

internal class BlackAOEGCDFeature : BLMCombo
{
    public override string ComboFancyName => "Ⱥ��GCD";

    public override string Description => "�滻��2�ǳ�ţ�Ƶ�Ⱥ��GCD��";

    protected internal override uint[] ActionIDs => new uint[] { Actions.Fire2 };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (CanAddAbility(level, out uint act)) return act;

        //����19��
        if (level < Levels.Fire2)
        {
            return Actions.Blizzard2;
        }
        //��״̬
        if (JobGauge.InUmbralIce)
        {
            // ����58�������2/�����������������ײ��ϡ�
            if (level < Levels.Blizzard4)
            {
                if (HaveEnoughMP)
                {
                    return Actions.Fire2;
                }

                if (level >= Levels.Thunder2 && TargetBuffDuration((ushort)Debuffs.Thunder2) < 3f)
                {
                    return Actions.Thunder2;
                }

                if (level >= Levels.Freeze)
                {
                    return Actions.Freeze;
                }
                return Actions.Blizzard2;
            }
            // ����82�����������������������ײ��ϡ�
            else if (level < Levels.HighBlizzard2)
            {
                if (HaveEnoughMP)
                {
                    return Actions.Fire2;
                }

                if (JobGauge.UmbralHearts < 3)
                {
                    return Actions.Freeze;
                }

                //����
                if (TargetBuffDuration(Debuffs.Thunder4) < 3f)
                {
                    if (level >= Levels.Thunder4)
                    {
                        return Actions.Thunder4;
                    }
                    else if (TargetBuffDuration(Debuffs.Thunder2) < 3f)
                    {
                        return Actions.Thunder2;
                    }
                }


                return Actions.Freeze;
            }
            // ��߱�2�������������ײ��ϡ�
            else
            {
                if (LocalPlayer.CurrentMp > 9000)
                {
                    return Actions.HighFire2;
                }

                if (JobGauge.UmbralHearts < 3)
                {
                    return Actions.Freeze;
                }

                if (TargetBuffDuration(Debuffs.Thunder4) < 3f)
                {
                    return Actions.Thunder4;
                }

                return Actions.HighBlizzard2;
            }
        }
        //��״̬
        else if (JobGauge.InAstralFire)
        {
            // ����50 �������2��û����
            if (level < Levels.Flare)
            {

                if (LocalPlayer.CurrentMp < 3000)
                {
                    return Actions.Blizzard2;
                }

                return Actions.Fire2;
            }
            // ����58�������2������ת�˱���
            else if (level < Levels.Blizzard4)
            {
                if (LocalPlayer.CurrentMp == 0)
                {
                    return Actions.Blizzard2;
                }
                if (LocalPlayer.CurrentMp < 3800)
                {
                    return Actions.Flare;
                }
                return Actions.Fire2;
            }
            // ��˫�˱�����2/�߻�2��䣬�����ײ��ϡ�
            else
            {
                if (LocalPlayer.CurrentMp == 0)
                {
                    if (level >= Levels.HighBlizzard2)
                    {
                        return Actions.HighBlizzard2;
                    }
                    return Actions.Blizzard2;
                }

                if (JobGauge.UmbralHearts < 2)
                {
                    return Actions.Flare;
                }

                //����
                if (TargetBuffDuration(Debuffs.Thunder4) < 3f)
                {
                    if (level >= Levels.Thunder4)
                    {
                        return Actions.Thunder4;
                    }
                    else if (TargetBuffDuration(Debuffs.Thunder2) < 3f)
                    {
                        return Actions.Thunder2;
                    }
                }
                //�����ǣ��������
                if (level >= Levels.Foul && JobGauge.PolyglotStacks == 2)
                {
                    return Actions.Foul;
                }

                //�ϻ�2/�߻�2
                if (level >= Levels.HighBlizzard2)
                {
                    return Actions.HighFire2;
                }
                return Actions.Fire2;
            }
        }
        else
        {
            return Actions.Blizzard2;
        }

        return actionID;
    }
}
