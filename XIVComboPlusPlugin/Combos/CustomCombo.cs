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

public abstract class CustomCombo
{
    #region Job

    internal abstract uint JobID { get; }

    internal abstract string JobName { get; }

    internal struct GeneralActions
    {
        internal static readonly BaseAction
            //����
            Addle = new BaseAction(7560u),

            //����ӽ��
            Swiftcast = new BaseAction(7561u)
            {
                BuffsProvide = new ushort[]
            {
                ObjectStatus.Swiftcast1,
                ObjectStatus.Swiftcast2,
                ObjectStatus.Swiftcast3,
                ObjectStatus.Triplecast,
                ObjectStatus.Dualcast,
                ObjectStatus.Acceleration,
            }
            },

            //����
            Esuna = new BaseAction(7568)
            {
                OtherCheck = () => TargetHelper.WeakenPeople.Length > 0,
            },

            //���Σ����MP����6000��ôʹ�ã�
            LucidDreaming = new BaseAction(7562u)
            {
                OtherCheck = () => Service.ClientState.LocalPlayer.CurrentMp < 6000,
            },

            //����
            LegGraze = new BaseAction(7554)
            {
                BuffsProvide = new ushort[]
                {
                    13,564,1345,
                },
            },

            //�ڵ�
            SecondWind = new BaseAction(7541)
            {
                OtherCheck = () => (double)Service.ClientState.LocalPlayer.CurrentHp / Service.ClientState.LocalPlayer.MaxHp < 0.6,
            },

            //����
            FootGraze = new BaseAction(7553),

            //��������
            ArmsLength = new BaseAction(7548),

            //����
            Rampart = new BaseAction(7531)
            {
                BuffsProvide = new ushort[]
                {
                    ObjectStatus.Rampart1, ObjectStatus.Rampart2, ObjectStatus.Rampart3,
                    //ԭ����ֱ����Ѫ��
                    ObjectStatus.RawIntuition, ObjectStatus.Bloodwhetting,
                    //����
                    ObjectStatus.Vengeance,
                },
            },

            //����
            Provoke = new BaseAction(7533),

            //ѩ��
            Reprisal = new BaseAction(7535),

            //�˱�
            Shirk = new BaseAction(7537);

    }
    #endregion

    #region Combo
    protected internal abstract uint[] ActionIDs { get; }
    public abstract string ComboFancyName { get; }

    public abstract string Description { get; }

    public virtual string[] ConflictingCombos => new string[0];
    public virtual string ParentCombo => string.Empty;

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
    protected static bool HaveTargetAngle => TargetHelper.GetObjectInRadius(TargetHelper.HostileTargets, 25).Length > 0;

    protected static PlayerCharacter LocalPlayer => Service.ClientState.LocalPlayer;
    protected static GameObject Target => Service.TargetManager.Target;
    protected static float WeaponRemain => Service.IconReplacer.GetCooldown(141u).CooldownRemaining;
    protected static bool CanInsertAbility => !LocalPlayer.IsCasting && WeaponRemain > 0.6;
    protected static bool CanHealAreaAbility => TargetHelper.PartyMembersDifferHP < Service.Configuration.HealthDifference && TargetHelper.PartyMembersAverHP < Service.Configuration.HealthAreaAbility;
    protected static bool CanHealAreaSpell => TargetHelper.PartyMembersDifferHP < Service.Configuration.HealthDifference && TargetHelper.PartyMembersAverHP < Service.Configuration.HealthAreafSpell;

    protected static bool CanHealSingleAbility => TargetHelper.PartyMembersHP.Min() < Service.Configuration.HealthSingleAbility;
    protected static bool CanHealSingleSpell => TargetHelper.PartyMembersHP.Min() < Service.Configuration.HealthSingleSpell;
    protected CustomCombo()
    {
    }

    internal bool TryInvoke(uint actionID, uint lastComboActionID, float comboTime, byte level, out uint newActionID)
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
        if (actionID == num2)
        {
            return false;
        }
        else if (num2 == 0)
        {
            SortedSet<byte> validJobs = new SortedSet<byte>(ClassJob.AllJobs.Where(job => job.Type == JobType.MagicalRanged || job.Type == JobType.Healer).Select(job => job.Index));

            newActionID = TargetHelper.GetJobCategory( Service.ClientState.LocalPlayer, validJobs) ? GeneralActions.SecondWind.ActionID : GeneralActions.LucidDreaming.ActionID;
            return true;
        }
        newActionID = num2;
        return true;
    }

    protected abstract uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level);

}
