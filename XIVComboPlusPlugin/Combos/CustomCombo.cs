using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;

namespace XIVComboPlus.Combos;

public abstract class CustomCombo
{
    //private SpeechSynthesizer ssh = new SpeechSynthesizer() { Rate = 0 };
    private uint _lastGCDAction;

    internal static bool HaveSwift
    {
        get
        {
            foreach (var status in Service.ClientState.LocalPlayer.StatusList)
            {
                if (GeneralActions.Swiftcast.BuffsProvide.Contains((ushort)status.StatusId))
                {
                    return true;
                }
            }
            return false;
        }
    }

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

            //Ӫ��
            Rescue = new BaseAction(7571),

            //����
            Repose = new BaseAction(16560),

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
            Shirk = new BaseAction(7537),

            //ԡѪ
            Bloodbath = new BaseAction(7542)
            {
                OtherCheck = SecondWind.OtherCheck,
            },

            //ǣ��
            Feint = new BaseAction(7549),

            //����
            Interject = new BaseAction(7538),

            //����
            LowBlow = new BaseAction(7540),

            //ɨ��
            LegSweep = new BaseAction(7863),

            //��ͷ
            HeadGraze = new BaseAction(7551);

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

    protected static PlayerCharacter LocalPlayer => Service.ClientState.LocalPlayer;
    protected static GameObject Target => Service.TargetManager.Target;

    protected static bool IsMoving => TargetHelper.IsMoving;
    protected static bool HaveTargetAngle => TargetHelper.HaveTargetAngle;
    internal static byte AbilityRemainCount => TargetHelper.AbilityRemainCount;

    protected static float WeaponRemain => TargetHelper.WeaponRemain;

    protected virtual bool CanHealAreaAbility => TargetHelper.CanHealAreaAbility;
    protected virtual bool CanHealAreaSpell => TargetHelper.CanHealAreaSpell;

    protected virtual bool CanHealSingleAbility => TargetHelper.CanHealSingleAbility;
    protected virtual bool CanHealSingleSpell => TargetHelper.CanHealSingleSpell;

    /// <summary>
    /// Only one feature can set it to true!
    /// </summary>
    protected virtual bool ShouldSayout => false;
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

        uint actNew = Invoke(actionID, lastComboActionID, comboTime, level);
        if (actionID == actNew)
        {
            return false;
        }
        else if (actNew == 0)
        {
            SortedSet<byte> validJobs = new SortedSet<byte>(ClassJob.AllJobs.Where(job => job.Type == JobType.MagicalRanged || job.Type == JobType.Healer).Select(job => job.Index));

            newActionID = TargetHelper.GetJobCategory(Service.ClientState.LocalPlayer, validJobs) ? GeneralActions.SecondWind.ActionID : GeneralActions.LucidDreaming.ActionID;
            return true;
        }
        newActionID = actNew;
        return true;
    }

    private bool CheckAction(uint actionID)
    {
        //return false;
        if (ShouldSayout && _lastGCDAction != actionID)
        {
            _lastGCDAction = actionID;
            return true;
        }
        else return false;
    }

    private static void Speak(string text, bool wait = false)
    {
        ExecuteCommand(
            $@"Add-Type -AssemblyName System.speech; 
                $speak = New-Object System.Speech.Synthesis.SpeechSynthesizer; 
                $speak.Speak(""{text}"");");

        void ExecuteCommand(string command)
        {
            string path = Path.GetTempPath() + Guid.NewGuid() + ".ps1";

            // make sure to be using System.Text
            using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                sw.Write(command);

                ProcessStartInfo start = new ProcessStartInfo()
                {
                    FileName = @"C:\Windows\System32\windowspowershell\v1.0\powershell.exe",
                    LoadUserProfile = false,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = $"-executionpolicy bypass -File {path}",
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                Process process = Process.Start(start);

                if (wait)
                    process.WaitForExit();
            }
        }
    }

    private uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
    {

        byte abilityRemain = AbilityRemainCount;
        BaseAction GCDaction = GCD(level, lastComboActionID);
        if (GCDaction == null) return 0;

        //Sayout!
        if (CheckAction(GCDaction.ActionID) && GCDaction.SayoutText != EnemyLocation.None)
        {
            string text = GCDaction.Action.Name + " " + GCDaction.SayoutText.ToString();
            //Service.ChatGui.PrintChat(new Dalamud.Game.Text.XivChatEntry()
            //{
            //    Message = text,
            //    Type = Dalamud.Game.Text.XivChatType.Notice,
            //});
            Speak(text);
        }

        uint GCDact = GCDaction.ActionID;

        //return GCDact;
        switch (AbilityRemainCount)
        {
            case 0:
                return GCDact;
            default:
                BaseAction AbilityAction;
                if (FirstActionAbility(level, abilityRemain, GCDaction, out AbilityAction)) return AbilityAction.ActionID;
                if (!TargetHelper.HPFull)
                {
                    if (CanHealAreaAbility && HealAreaAbility(level, abilityRemain, out AbilityAction)) return AbilityAction.ActionID;
                    if (CanHealSingleAbility && HealSingleAbility(level, abilityRemain, out AbilityAction)) return AbilityAction.ActionID;
                }
                if (GeneralAbility(level, abilityRemain, out AbilityAction)) return AbilityAction.ActionID;
                if (HaveTargetAngle && ForAttachAbility(level, abilityRemain, out AbilityAction)) return AbilityAction.ActionID;
                return GCDact;
        }
    }

    private BaseAction GCD(byte level, uint lastComboActionID)
    {
        if (EmergercyGCD(level, lastComboActionID, out BaseAction act)) return act;
        if (!TargetHelper.HPFull)
        {
            if (CanHealAreaSpell && HealAreaGCD(level, lastComboActionID, out act)) return act;
            if (CanHealSingleSpell && HealSingleGCD(level, lastComboActionID, out act)) return act;
        }
        if (GeneralGCD(level, lastComboActionID, out act)) return act;
        return null;
    }
    /// <summary>
    /// ����дһЩ���ڹ�������������ֻ�и����е��˵�ʱ��Ż���Ч��
    /// </summary>
    /// <param name="level"></param>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected abstract bool ForAttachAbility(byte level, byte abilityRemain, out BaseAction act);
    /// <summary>
    /// ����дһЩ������Ϊ�����GCD���ܶ�Ҫ��Ӧ����������
    /// </summary>
    /// <param name="level"></param>
    /// <param name="abilityRemain"></param>
    /// <param name="nextGCD"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool FirstActionAbility(byte level, byte abilityRemain, BaseAction nextGCD, out BaseAction act)
    {
        if(Target is BattleChara b && b.IsCasting && b.IsCastInterruptible)
        {
            JobType type = JobType.None;
            foreach (var job in ClassJob.AllJobs)
            {
                if (job.Index == JobID)
                {
                    type = job.Type;
                    break;
                }
            }

            switch (type)
            {
                case JobType.Tank:
                    if (GeneralActions.Interject.TryUseAction(level, out act, mustUse: true)) return true;
                    if (GeneralActions.LowBlow.TryUseAction(level, out act, mustUse:true)) return true;
                    break;
                case JobType.Melee:
                    if (GeneralActions.LegSweep.TryUseAction(level, out act, mustUse: true)) return true;
                    break;
                case JobType.PhysicalRanged:
                    if (GeneralActions.HeadGraze.TryUseAction(level, out act, mustUse: true)) return true;
                    break;
            }
        }

        if (nextGCD.CastTime > 7000 && GeneralActions.Swiftcast.TryUseAction(level, out act, mustUse: true)) return true;
        act = null; return false;
    }
    /// <summary>
    /// �������������ɶʱ����ʹ�á�
    /// </summary>
    /// <param name="level"></param>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool GeneralAbility(byte level, byte abilityRemain, out BaseAction act)
    {
        act = null; return false;
    }
    /// <summary>
    /// �������Ƶ�������
    /// </summary>
    /// <param name="level"></param>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool HealSingleAbility(byte level, byte abilityRemain, out BaseAction act)
    {
        act = null; return false;
    }
    /// <summary>
    /// ��Χ���Ƶ�������
    /// </summary>
    /// <param name="level"></param>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool HealAreaAbility(byte level, byte abilityRemain, out BaseAction act)
    {
        act = null; return false;
    }
    /// <summary>
    /// һЩ�ǳ�������GCDս������������ʲô�ġ����ȼ����
    /// </summary>
    /// <param name="level"></param>
    /// <param name="lastComboActionID"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool EmergercyGCD(byte level, uint lastComboActionID, out BaseAction act)
    {
        act = null; return false;
    }
    /// <summary>
    /// ����GCD����
    /// </summary>
    /// <param name="level"></param>
    /// <param name="lastComboActionID"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected abstract bool GeneralGCD(byte level, uint lastComboActionID, out BaseAction act);
    /// <summary>
    /// ��������GCD
    /// </summary>
    /// <param name="level"></param>
    /// <param name="lastComboActionID"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool HealSingleGCD(byte level, uint lastComboActionID, out BaseAction act)
    {
        act = null; return false;
    }
    /// <summary>
    /// ��Χ����GCD
    /// </summary>
    /// <param name="level"></param>
    /// <param name="lastComboActionID"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool HealAreaGCD(byte level, uint lastComboActionID, out BaseAction act)
    {
        act = null; return false;
    }
}
