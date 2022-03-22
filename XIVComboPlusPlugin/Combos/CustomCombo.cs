using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using Dalamud.Utility;
using XIVComboPlus.Attributes;

namespace XIVComboPlus.Combos;

internal abstract class CustomCombo
{
    #region Job

    internal abstract uint JobID { get; }

    internal abstract string JobName { get; }

    protected struct GeneralActions
    {
        public static readonly BaseAction
            //����
            Addle = new BaseAction(8, 7560u, ability: true),

            //����ӽ��
            Swiftcast = new BaseAction(18, 7561u, ability: true) { BuffsProvide = new ushort[]
            {
                ObjectStatus.Swiftcast1,
                ObjectStatus.Swiftcast2,
                ObjectStatus.Triplecast,
            }},

            //���Σ����MP����5000��ôʹ�ã�
            LucidDreaming = new BaseAction(24, 7562u, ability: true)
            {
                OtherCheck = () => Service.ClientState.LocalPlayer.CurrentMp < 6000,
            };

    }
    #endregion

    #region Combo
    protected internal abstract uint[] ActionIDs { get; }
    public abstract string ComboFancyName { get; }

    public abstract string Description { get; }

    public virtual string[] ConflictingCombos => new string[0];
    public virtual string ParentCombo => string.Empty;

    public virtual bool SecretCombo => false;
    /// <summary>
    /// ���ȼ���Խ���ʹ�õ��ĸ���Խ�ߣ�
    /// </summary>
    public virtual byte Priority => 0;
    public bool IsEnabled
    {
        get => Service.Configuration.EnabledActions.Contains(ComboFancyName);
        set
        {
            if (value)
            {
                Service.Configuration.EnabledActions.Add(ComboFancyName);
            }
            else
            {
                Service.Configuration.EnabledActions.Remove(ComboFancyName);
            }
        }
    }

    #endregion
    protected static bool IsMoving => TargetHelper.IsMoving;
    protected static bool HaveValidTarget => Target != null && Target.ObjectKind == ObjectKind.BattleNpc && ((BattleNpc)Target).CurrentHp != 0;
    protected static PlayerCharacter LocalPlayer => Service.ClientState.LocalPlayer;
    protected static GameObject Target => Service.TargetManager.Target;
    protected static bool CanInsertAbility => !LocalPlayer.IsCasting && Service.IconReplacer.GetCooldown(141u).CooldownRemaining > 0.67;
    protected CustomCombo()
    {
    }

    public bool TryInvoke(uint actionID, uint lastComboActionID, float comboTime, byte level, out uint newActionID)
    {
        newActionID = 0u;
        if (!IsEnabled)
        {
            return false;
        }
        if (!ActionIDs.Contains(actionID))
        {
            return false;
        }
        uint num2 = Invoke(actionID, lastComboActionID, comboTime, level);
        if (num2 == 0 || actionID == num2)
        {
            return false;
        }
        newActionID = num2;
        return true;
    }

    //protected static uint CalcBestAction(uint original, params uint[] actions)
    //{
    //    return actions.Select(new Func<uint, (uint, IconReplacer.CooldownData)>(Selector)).Aggregate(((uint ActionID, IconReplacer.CooldownData Data) a1, (uint ActionID, IconReplacer.CooldownData Data) a2) => Compare(original, a1, a2)).Item1;
        
    //    static (uint ActionID, IconReplacer.CooldownData Data) Compare(uint original, (uint ActionID, IconReplacer.CooldownData Data) a1, (uint ActionID, IconReplacer.CooldownData Data) a2)
    //    {
    //        if (!a1.Data.IsCooldown && !a2.Data.IsCooldown)
    //        {
    //            if (original != a1.ActionID)
    //            {
    //                return a2;
    //            }
    //            return a1;
    //        }
    //        if (a1.Data.IsCooldown && a2.Data.IsCooldown)
    //        {
    //            if (!(a1.Data.CooldownRemaining < a2.Data.CooldownRemaining))
    //            {
    //                return a2;
    //            }
    //            return a1;
    //        }
    //        if (!a1.Data.IsCooldown)
    //        {
    //            return a1;
    //        }
    //        return a2;
    //    }

    //    static (uint ActionID, IconReplacer.CooldownData Data) Selector(uint actionID)
    //    {
    //        return (actionID, GetCooldown(actionID));
    //    }
    //}

    protected abstract uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level);

    protected static uint OriginalHook(uint actionID)
    {
        return Service.IconReplacer.OriginalHook(actionID);
    }

    protected static bool HasCondition(ConditionFlag flag)
    {
        return Service.Conditions[flag];
    }
}
