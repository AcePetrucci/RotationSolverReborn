using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Combos.Script;
using XIVAutoAttack.Data;
using XIVAutoAttack.Localization;
using XIVAutoAttack.SigReplacers;

namespace XIVAutoAttack.Windows.ComboConfigWindow;

internal partial class ComboConfigWindow : Window
{
    public ComboConfigWindow()
        : base(LocalizationManager.RightLang.ConfigWindow_Header + typeof(ComboConfigWindow).Assembly.GetName().Version.ToString(), 0, false)
    {
        SizeCondition = ImGuiCond.FirstUseEver;
        Size = new Vector2(740f, 490f);
        RespectCloseHotkey = true;

        this.WindowName = "Hello";
    }

    private static readonly Dictionary<JobRole, string> _roleDescriptionValue = new Dictionary<JobRole, string>()
    {
        {JobRole.Tank, $"{DescType.DefenseSingle.ToName()} �� {CustomCombo<Enum>.Rampart}, {CustomCombo<Enum>.Reprisal}" },
        {JobRole.Melee, $"{DescType.DefenseArea.ToName()} �� {CustomCombo<Enum>.Feint}" },
        {JobRole.RangedMagicial, $"{DescType.DefenseArea.ToName()} �� {CustomCombo<Enum>.Addle}" },
    };



    public override unsafe void Draw()
    {
        if (ImGui.BeginTabBar("AutoAttackSettings"))
        {
#if DEBUG
            if (Service.ClientState.LocalPlayer != null && ImGui.BeginTabItem("Debug")
            {
                DrawDebug();
                ImGui.EndTabItem();
            }
#endif

            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_AboutItem))
            {
                DrawAbout();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_AttackItem))
            {
                DrawAttack();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_ParamItem))
            {
                DrawParam();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_EventsItem))
            {
                DrawEvent();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_ActionsItem))
            {
                // "��������ڣ�������趨ÿ�����ܵ��ͷ�������"
                ImGui.Text(LocalizationManager.RightLang.ConfigWindow_ActionItem_Description);

                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));


                if (ImGui.BeginChild("Action List", new Vector2(0f, -1f), true))
                {
                    foreach (var pair in IconReplacer.RightComboBaseActions.GroupBy(a => a.CateName).OrderBy(g => g.Key))
                    {
                        if (ImGui.CollapsingHeader(pair.Key))
                        {
                            foreach (var item in pair)
                            {
                                DrawAction(item);
                                ImGui.Separator();
                            }
                        }
                    }
                    ImGui.EndChild();
                }
                ImGui.PopStyleVar();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem(LocalizationManager.RightLang.ConfigWindow_HelpItem))
            {
                //��������ڣ�����Կ���ս���ú꣬������������������в鿴��
                ImGui.Text(LocalizationManager.RightLang.ConfigWindow_HelpItem_Description);

                if (ImGui.BeginChild("Help Infomation", new Vector2(0f, -1f), true))
                {
                    ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

                    //��ʼ������������ڽ����оͿ�ʼ����������ڽ������л�ѡ��ж�Ŀ��������
                    CommandHelp("AttackSmart", LocalizationManager.RightLang.Configwindow_HelpItem_AttackSmart);
                    ImGui.Separator();

                    //��ʼ��������������Ϊ�ֶ�ѡ��
                    CommandHelp("AttackManual", LocalizationManager.RightLang.Configwindow_HelpItem_AttackManual);
                    ImGui.Separator();

                    //ֹͣ�������ǵ�һ��Ҫ�����ص���
                    CommandHelp("AttackCancel", LocalizationManager.RightLang.Configwindow_HelpItem_AttackCancel);
                    ImGui.Separator();

                    //����һ�η�Χ���ƵĴ�����
                    CommandHelp("HealArea", LocalizationManager.RightLang.Configwindow_HelpItem_HealArea);
                    ImGui.Separator();

                    //����һ�ε������ƵĴ�����
                    CommandHelp("HealSingle", LocalizationManager.RightLang.Configwindow_HelpItem_HealSingle);
                    ImGui.Separator();

                    //����һ�η�Χ��Χ�����Ĵ����ڡ�
                    CommandHelp("DefenseArea", LocalizationManager.RightLang.Configwidow_HelpItem_DefenseArea);
                    ImGui.Separator();

                    //����һ�ε�������Ĵ�����
                    CommandHelp("DefenseSingle", LocalizationManager.RightLang.Configwidow_HelpItem_DefenseSingle);
                    ImGui.Separator();

                    //����һ�ο������߶��˻����汱�Ĵ�����
                    CommandHelp("EsunaShield", LocalizationManager.RightLang.Configwidow_HelpItem_EsunaShield);
                    ImGui.Separator();

                    //����ǿ�ƾ��˻��˱ܵĴ�����
                    CommandHelp("RaiseShirk", LocalizationManager.RightLang.Configwidow_HelpItem_RaiseShirk);
                    ImGui.Separator();

                    //����һ�η����˵Ĵ�����
                    CommandHelp("AntiRepulsion", LocalizationManager.RightLang.Configwidow_HelpItem_AntiRepulsion);
                    ImGui.Separator();

                    //����һ�α��������ƵĴ�����
                    CommandHelp("BreakProvoke", LocalizationManager.RightLang.Configwidow_HelpItem_BreakProvoke);
                    ImGui.Separator();

                    //����һ��λ�ƵĴ�����
                    CommandHelp("Move", LocalizationManager.RightLang.Configwidow_HelpItem_Move);
                    ImGui.Separator();
                }
                ImGui.PopStyleVar();

                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
        ImGui.End();
    }



    internal static void DrawTexture<T>(T texture, Action otherThing, ClassJobID jobId = 0, string[] authors = null) where T : class, IEnableTexture
    {
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(3f, 3f));

        ImGui.Columns(2, texture.Name, false);

        var t = texture.GetTexture();

        ImGui.SetColumnWidth(0, t.Width + 5);

        var str = texture.Description;

        ImGui.Image(t.ImGuiHandle, new Vector2(t.Width, t.Height));
        if (ImGui.IsItemHovered())
        {
            if (!string.IsNullOrEmpty(str)) ImGui.SetTooltip(str);
        }

        ImGui.NextColumn();

        bool enable = texture.IsEnabled;

        if (ImGui.Checkbox(texture.Name, ref enable))
        {
            texture.IsEnabled = enable;
            Service.Configuration.Save();
        }
        if (ImGui.IsItemHovered())
        {
            if (!string.IsNullOrEmpty(str)) ImGui.SetTooltip(str);
        }


        ImGui.SameLine();

        if (!string.IsNullOrEmpty(texture.Author))
        {
            authors ??= new string[] { texture.Author };

            int i;
            for (i = 0; i < authors.Length; i++)
            {
                if (authors[i] == texture.Author)
                {
                    break;
                }
            }

            Spacing();
            ImGui.TextDisabled("-  ");
            ImGui.SameLine();
            ImGui.SetNextItemWidth(ImGui.CalcTextSize(authors[i]).X + 30);
            if (ImGui.Combo("##" + texture.Name + "Author", ref i, authors, authors.Length))
            {
                Service.Configuration.ComboChoices[(uint)jobId] = authors[i];
                Service.Configuration.Save();
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("������л�����");
            }
        }

        ImGui.SameLine();
        Spacing();

        if (texture is ICustomCombo com)
        {
            if (texture is IScriptCombo script)
            {
                if (ImGuiComponents.IconButton(texture.GetHashCode(), FontAwesomeIcon.Edit))
                {
                    XIVAutoAttackPlugin.OpenScriptWindow(script);
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("����Ա༭���Զ���Combo");
                }
            }
            else
            {
                if (ImGuiComponents.IconButton(texture.GetHashCode(), FontAwesomeIcon.Globe))
                {
                    var url = @"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/" + texture.GetType().FullName.Replace(".", @"/") + ".cs";
                    Process.Start("cmd", $"/C start {url}");
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("��Դ����ַ");
                }
            }

            ImGui.SameLine();
            Spacing();

            if (Directory.Exists(Service.Configuration.ScriptComboFolder)
                && ImGuiComponents.IconButton(texture.GetHashCode() + 1, FontAwesomeIcon.Plus))
            {
                var newCom = IconReplacer.AddScripCombo(com.JobIDs[0]);
                if(newCom != null)
                {
                    Service.Configuration.ComboChoices[(uint)jobId] = newCom.Author;
                    Service.Configuration.Save();
                }
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("���һ���Զ���Combo");
            }
        }

        if (enable)
        {
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(1f, 1f));
            otherThing?.Invoke();
            ImGui.PopStyleVar();
        }
        ImGui.Columns(1);
        ImGui.PopStyleVar();
    }

    internal static void Spacing(byte count = 1)
    {
        string s = string.Empty;
        for (int i = 0; i < count; i++)
        {
            s += "    ";
        }
        ImGui.Text(s);
        ImGui.SameLine();
    }

    private static void CommandHelp(string command, string help = null)
    {
        command = XIVAutoAttackPlugin._autoCommand + " " + command;
        if (ImGui.Button(command))
        {
            Service.CommandManager.ProcessCommand(command);
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip($"������ִ������: {command}");
        }

        if (!string.IsNullOrEmpty(help))
        {
            ImGui.SameLine();
            ImGui.Text(" �� " + help);
        }
    }
    private unsafe static void DrawAction(BaseAction act)
    {
        if (act == null) return;

        DrawTexture(act, () =>
        {
            if(act.IsTimeline) CommandHelp($"Insert{act}-{5}", string.Format("5s��������Ȳ���\"{0}\"", act));
#if DEBUG
            ImGui.NewLine();
            ImGui.Text("Have One:" + act.HaveOneChargeDEBUG.ToString());
            ImGui.Text("Is Real GCD: " + act.IsRealGCD.ToString());
            ImGui.Text("Recast One: " + act.RecastTimeOneChargeDEBUG.ToString());
            ImGui.Text("Recast Elapsed: " + act.RecastTimeElapsedDEBUG.ToString());
            ImGui.Text("Recast Remain: " + act.RecastTimeRemainDEBUG.ToString());
            ImGui.Text("Status: " + ActionManager.Instance()->GetActionStatus(ActionType.Spell, act.AdjustedID).ToString());

            ImGui.Text("Cast Time: " + act.CastTime.ToString());
            ImGui.Text("MP: " + act.MPNeed.ToString());
            ImGui.Text($"Can Use: {act.ShouldUse(out _)} {act.ShouldUse(out _, mustUse: true)}");

            ImGui.Text("IsUnlocked: " + UIState.Instance()->IsUnlockLinkUnlocked(act.AdjustedID).ToString());
#endif
        });
    }
}
