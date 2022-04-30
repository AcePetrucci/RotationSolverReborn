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
    internal static readonly uint[] RangePhysicial = new uint[] { 23, 31, 38 };
    internal abstract uint JobID { get; }
    internal string RoleName => ((Role)XIVComboPlusPlugin.AllJobs.First(job => job.RowId == JobID).Role).ToString();

    internal string JobName => XIVComboPlusPlugin.AllJobs.First(job => job.RowId == JobID).Name;

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
                ChoiceFriend = (tars) =>
                {
                    HashSet<BattleChara> dying = new (tars.Length);
                    HashSet<BattleChara> weaken = new (tars.Length);
                    foreach (var p in tars)
                    {
                        foreach (var status in p.StatusList)
                        {
                            if (status.StatusId == ObjectStatus.Doom) dying.Add(p);
                            if (status.GameData.CanDispel) weaken.Add(p);
                        }
                    }
                    if(dying.Count > 0)
                    {
                        return dying.OrderBy(b => BaseAction.DistanceToPlayer(b)).First();
                    }
                    else if(weaken.Count > 0)
                    {
                        return weaken.OrderBy(b => BaseAction.DistanceToPlayer(b)).First();
                    }
                    return null;
                },
            },

            //Ӫ��
            Rescue = new BaseAction(7571),

            //����
            Repose = new BaseAction(16560),

            //���Σ����MP����6000��ôʹ�ã�
            LucidDreaming = new BaseAction(7562u)
            {
                OtherCheck = b => Service.ClientState.LocalPlayer.CurrentMp < 6000,
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
                OtherCheck = b => (float)Service.ClientState.LocalPlayer.CurrentHp / Service.ClientState.LocalPlayer.MaxHp < 0.6,
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
            Provoke = new BaseAction(7533)
            {

            },

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
            HeadGraze = new BaseAction(7551),

            //����ӽ��
            Surecast = new BaseAction(7559);

    }
    #endregion

    #region Combo
    protected internal uint ActionID => GeneralActions.Repose.ActionID;

    public bool IsEnabled
    {
        get => Service.Configuration.EnabledActions.Contains(JobName);
        set
        {
            if (value)
            {
                Service.Configuration.EnabledActions.Add(JobName);
            }
            else
            {
                Service.Configuration.EnabledActions.Remove(JobName);
            }
        }
    }

    #endregion

    protected static PlayerCharacter LocalPlayer => Service.ClientState.LocalPlayer;
    protected static GameObject Target => Service.TargetManager.Target;

    protected static bool IsMoving => TargetHelper.IsMoving;
    protected static bool HaveTargetAngle => TargetHelper.HaveTargetAngle;
    protected static float WeaponRemain => TargetHelper.WeaponRemain;

    protected virtual bool CanHealAreaAbility => TargetHelper.CanHealAreaAbility;
    protected virtual bool CanHealAreaSpell => TargetHelper.CanHealAreaSpell;

    protected virtual bool CanHealSingleAbility => TargetHelper.CanHealSingleAbility;
    protected virtual bool CanHealSingleSpell => TargetHelper.CanHealSingleSpell;

    /// <summary>
    /// Only one feature can set it to true!
    /// </summary>
    protected virtual bool ShouldSayout => false;
    private protected virtual BaseAction Raise => null;
    private protected CustomCombo()
    {
    }

    internal bool TryInvoke(uint actionID, uint lastComboActionID, float comboTime, byte level, out BaseAction newAction)
    {

        newAction = null;
        if (!IsEnabled)
        {
            return false;
        }
        if (ActionID != actionID)
        {
            return false;
        }

        newAction = Invoke(actionID, lastComboActionID, comboTime);

        //û��ö���
        if (newAction == null) return false;

        //��֮ǰһ��
        if (actionID == newAction.ActionID) return false;
        //else if (actNew == null)
        //{
        //    //SortedSet<byte> validJobs = new SortedSet<byte>(ClassJob.AllJobs.Where(job => job.Type == JobType.MagicalRanged || job.Type == JobType.Healer).Select(job => job.Index));

        //    //newActionID = TargetHelper.GetJobCategory(Service.ClientState.LocalPlayer, validJobs) ? GeneralActions.SecondWind.ActionID : GeneralActions.LucidDreaming.ActionID;
        //    return true;
        //}


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

    internal static void Speak(string text, bool wait = false)
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

    private BaseAction Invoke(uint actionID, uint lastComboActionID, float comboTime)
    {

        byte abilityRemain = TargetHelper.AbilityRemainCount;
        BaseAction GCDaction = GCD(lastComboActionID, abilityRemain);
        //Sayout!
        if (GCDaction != null)
        {
            if(CheckAction(GCDaction.ActionID) && GCDaction.EnermyLocation != EnemyLocation.None)
            {
                string text = GCDaction.EnermyLocation.ToString();
                //Service.ChatGui.PrintChat(new Dalamud.Game.Text.XivChatEntry()
                //{
                //    Message = text,
                //    Type = Dalamud.Game.Text.XivChatType.Notice,
                //});
                Speak(text);
            }

            switch (abilityRemain)
            {
                case 0:
                    return GCDaction;
                default:
                    if (Ability(abilityRemain, GCDaction, out BaseAction ability)) return ability;
                    return GCDaction;
            }
        }
        else
        {
            if (Ability(abilityRemain, GeneralActions.Addle, out BaseAction ability)) return ability;
            return null;
        }
    }

    private BaseAction GCD(uint lastComboActionID, byte abilityRemain)
    {
        if (EmergercyGCD(out BaseAction act, abilityRemain)) return act;
        if (IconReplacer.RaiseorMove && MoveGCD(lastComboActionID, out act)) return act;
        if (TargetHelper.HPNotFull)
        {
            if ((IconReplacer.HealArea || CanHealAreaSpell) && HealAreaGCD(lastComboActionID, out act)) return act;
            if ((IconReplacer.HealSingle || CanHealSingleSpell) && HealSingleGCD(lastComboActionID, out act)) return act;
        }
        if (GeneralGCD(lastComboActionID, out act)) return act;
        return null;
    }

    private bool Ability(byte abilityRemain, BaseAction nextGCD, out BaseAction act)
    {
        if (EmergercyAbility(abilityRemain, nextGCD, out act)) return true;
        if (IconReplacer.AntiRepulsion)
        {
            switch( XIVComboPlusPlugin.AllJobs.First(job => job.RowId == JobID).Role)
            {
                case (byte)Role.����:
                case (byte)Role.��ս:
                    if(GeneralActions.ArmsLength.ShouldUseAction(out act)) return true;
                    break;
                case (byte)Role.����:
                    if (GeneralActions.Surecast.ShouldUseAction(out act)) return true;
                    break;
                case (byte)Role.Զ��:
                    if (RangePhysicial.Contains(Service.ClientState.LocalPlayer.ClassJob.Id))
                    {
                        if (GeneralActions.ArmsLength.ShouldUseAction(out act)) return true;
                    }
                    else
                    {
                        if (GeneralActions.Surecast.ShouldUseAction(out act)) return true;
                    }
                    break;
            }
        }
        if (TargetHelper.CanInterruptTargets.Length > 0)
        {
            switch (XIVComboPlusPlugin.AllJobs.First(job => job.RowId == JobID).Role)
            {
                case (byte)Role.����:
                    if (GeneralActions.Interject.ShouldUseAction(out act)) return true;
                    if (GeneralActions.LowBlow.ShouldUseAction(out act)) return true;
                    break;

                case (byte)Role.��ս:
                    if (GeneralActions.LegSweep.ShouldUseAction(out act)) return true;
                    break;
                case (byte)Role.Զ��:
                    if (RangePhysicial.Contains(Service.ClientState.LocalPlayer.ClassJob.Id))
                    {
                        if (GeneralActions.HeadGraze.ShouldUseAction(out act)) return true;
                    }
                    break;
            }
        }
        if (IconReplacer.RaiseorMove && MoveAbility(abilityRemain, out act)) return true;
        if (IconReplacer.DefenseArea && DefenceAreaAbility(abilityRemain, out act)) return true;
        if (IconReplacer.DefenseSingle && DefenceSingleAbility(abilityRemain, out act)) return true;
        if (TargetHelper.HPNotFull)
        {
            if ((IconReplacer.HealArea || CanHealAreaAbility) && HealAreaAbility(abilityRemain, out act)) return true;
            if ((IconReplacer.HealSingle || CanHealSingleAbility) && HealSingleAbility(abilityRemain, out act)) return true;
        }
        if (GeneralAbility(abilityRemain, out act)) return true;
        if (HaveTargetAngle && ForAttachAbility(abilityRemain, out act)) return true;
        return false;
    }
    /// <summary>
    /// ����дһЩ���ڹ�������������ֻ�и����е��˵�ʱ��Ż���Ч��
    /// </summary>
    /// <param name="level"></param>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected abstract bool ForAttachAbility(byte abilityRemain, out BaseAction act);
    /// <summary>
    /// ����дһЩ������Ϊ�����GCD���ܶ�Ҫ��Ӧ����������
    /// </summary>
    /// <param name="level"></param>
    /// <param name="abilityRemain"></param>
    /// <param name="nextGCD"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool EmergercyAbility(byte abilityRemain, BaseAction nextGCD, out BaseAction act)
    {
        if (nextGCD.Cast100 > 70 && GeneralActions.Swiftcast.ShouldUseAction(out act, mustUse: true)) return true;
        act = null; return false;
    }
    /// <summary>
    /// �������������ɶʱ����ʹ�á�
    /// </summary>
    /// <param name="level"></param>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool GeneralAbility(byte abilityRemain, out BaseAction act)
    {
        act = null; return false;
    }

    private protected virtual bool MoveAbility(byte abilityRemain, out BaseAction act)
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
    private protected virtual bool HealSingleAbility(byte abilityRemain, out BaseAction act)
    {
        act = null; return false;
    }

    private protected virtual bool DefenceSingleAbility(byte abilityRemain, out BaseAction act)
    {
        act = null; return false;
    }
    private protected virtual bool DefenceAreaAbility(byte abilityRemain, out BaseAction act)
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
    private protected virtual bool HealAreaAbility(byte abilityRemain, out BaseAction act)
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
    private bool EmergercyGCD(out BaseAction act, byte actabilityRemain)
    {
        if (Raise == null)
        {
            act = null;
            return false;
        }

        //��ĳЩ�ǳ�Σ�յ�״̬��
        if ((IconReplacer.Esuna && TargetHelper.WeakenPeople.Length > 0) ||TargetHelper.DyingPeople.Length > 0)
        {
            if (GeneralActions.Esuna.ShouldUseAction(out act, mustUse: true)) return true;
        }

        if (TargetHelper.DeathPeopleParty.Length > 0)
        {
            if (Service.ClientState.LocalPlayer.ClassJob.Id == 35)
            {
                if (HaveSwift && Raise.ShouldUseAction(out act)) return true;
            }
            else
            {
                if (IconReplacer.RaiseorMove || HaveSwift || (!GeneralActions.Swiftcast.IsCoolDown && actabilityRemain > 0))
                {
                    if (Raise.ShouldUseAction(out act)) return true;
                }
            }
        }
        //�������ˣ������ܲ��ܾȡ�


        act = null;
        return false;
    }
    /// <summary>
    /// ����GCD����
    /// </summary>
    /// <param name="level"></param>
    /// <param name="lastComboActionID"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected abstract bool GeneralGCD(uint lastComboActionID, out BaseAction act);
    /// <summary>
    /// ��������GCD
    /// </summary>
    /// <param name="level"></param>
    /// <param name="lastComboActionID"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool HealSingleGCD(uint lastComboActionID, out BaseAction act)
    {
        act = null; return false;
    }

    private protected virtual bool MoveGCD(uint lastComboActionID, out BaseAction act)
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
    private protected virtual bool HealAreaGCD(uint lastComboActionID, out BaseAction act)
    {
        act = null; return false;
    }
}
