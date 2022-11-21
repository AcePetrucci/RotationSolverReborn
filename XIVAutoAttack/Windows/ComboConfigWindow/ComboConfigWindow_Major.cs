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
using XIVAutoAttack.SigReplacers;

namespace XIVAutoAttack.Windows.ComboConfigWindow;

internal partial class ComboConfigWindow : Window
{
    public ComboConfigWindow()
        : base("�Զ��������� (��Դ���) v" + typeof(ComboConfigWindow).Assembly.GetName().Version.ToString(), 0, false)
    {
        SizeCondition = ImGuiCond.FirstUseEver;
        Size = new Vector2(740f, 490f);
        RespectCloseHotkey = true;
    }

    private static readonly Dictionary<JobRole, string> _roleDescriptionValue = new Dictionary<JobRole, string>()
    {
        {JobRole.Tank, $"{DescType.�������} �� {CustomCombo<Enum>.Rampart}, {CustomCombo<Enum>.Reprisal}" },
        {JobRole.Melee, $"{DescType.��Χ����} �� {CustomCombo<Enum>.Feint}" },
        {JobRole.RangedMagicial, $"��ϵ{DescType.��Χ����} �� {CustomCombo<Enum>.Addle}" },
    };



    public override unsafe void Draw()
    {
        if (ImGui.BeginTabBar("AutoAttackSettings"))
        {
#if DEBUG
            if (Service.ClientState.LocalPlayer != null && ImGui.BeginTabItem("Debug�鿴"))
            {
                DrawDebug();
                ImGui.EndTabItem();
            }
#endif

            if (ImGui.BeginTabItem("����"))
            {
                DrawAbout();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("�����趨"))
            {
                DrawAttack();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("�����趨"))
            {
                DrawParam();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("�����ͷ��¼�"))
            {
                DrawEvent();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("�����ͷ�����"))
            {
                ImGui.Text("��������ڣ�������趨ÿ�����ܵ��ͷ�������");

                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));


                if (ImGui.BeginChild("�����б�", new Vector2(0f, -1f), true))
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

            if (ImGui.BeginTabItem("�����ĵ�"))
            {
                ImGui.Text("��������ڣ�����Կ���ս���ú꣬������������������в鿴��");

                if (ImGui.BeginChild("����", new Vector2(0f, -1f), true))
                {
                    ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));
                    CommandHelp("AttackSmart", "������ڽ����оͿ�ʼ����������ڽ������л�ѡ��ж�Ŀ��������");
                    ImGui.Separator();
                    CommandHelp("AttackManual", "��ʼ��������������Ϊ�ֶ�ѡ�񣬴�ʱ�����ͷ�AOE��");
                    ImGui.Separator();
                    CommandHelp("AttackCancel", "ֹͣ�������ǵ�һ��Ҫ�����ص���");
                    ImGui.Separator();
                    CommandHelp("HealArea", "����һ�η�Χ���ƵĴ����ڡ�");
                    ImGui.Separator();
                    CommandHelp("HealSingle", "����һ�ε������ƵĴ����ڡ�");
                    ImGui.Separator();
                    CommandHelp("DefenseArea", "����һ�η�Χ�����Ĵ����ڡ�");
                    ImGui.Separator();
                    CommandHelp("DefenseSingle", "����һ�ε�������Ĵ����ڡ�");
                    ImGui.Separator();
                    CommandHelp("EsunaShield", "����һ�ο������߶��˻����汱�Ĵ����ڡ�");
                    ImGui.Separator();
                    CommandHelp("RaiseShirk", "����ǿ�ƾ��˻��˱ܵĴ����ڡ�");
                    ImGui.Separator();
                    CommandHelp("AntiRepulsion", "����һ�η����˵Ĵ����ڡ�");
                    ImGui.Separator();
                    CommandHelp("BreakProvoke", "����һ�α��������ƵĴ����ڡ�");
                    ImGui.Separator();
                    CommandHelp("Move", "����һ��λ�ƵĴ����ڡ�");
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
            if (ImGui.Combo("##" + texture.Name + "����", ref i, authors, authors.Length))
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
                if (ImGuiComponents.IconButton(FontAwesomeIcon.Edit))
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
                IconReplacer.AddScripCombo(com.JobIDs[0]);
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
