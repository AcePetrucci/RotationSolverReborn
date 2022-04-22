using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using XIVComboPlus.Attributes;
using XIVComboPlus.Combos;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace XIVComboPlus;

internal class ConfigWindow : Window
{
    private readonly Vector4 shadedColor = new Vector4(0.68f, 0.68f, 0.68f, 1f);

    public ConfigWindow()
        : base("�Զ�����������", 0, false)
    {
        RespectCloseHotkey = true;

        SizeCondition = (ImGuiCond)4;
        Size = new Vector2(740f, 490f);
    }

    public override void Draw()
    {
        if (ImGui.BeginTabBar("##tabbar"))
        {
            if (ImGui.BeginTabItem("�����趨"))
            {
                ImGui.Text("��������ڣ�������趨�Լ�ϲ���������趨��");

                ImGui.BeginChild("scrolling", new Vector2(0f, -1f), true);
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));
                int num = 1;

                foreach (uint key in IconReplacer.CustomCombosDict.Keys)
                {
                    var combos = IconReplacer.CustomCombosDict[key];
                    if (combos == null || combos.Length == 0) continue;

                    string jobName = combos[0].JobName;
                    if (ImGui.CollapsingHeader(jobName))
                    {
                        foreach (var combo in combos)
                        {
                            //ImGui.Text(combo.ComboFancyName);

                            bool enable = combo.IsEnabled;
                            string[] conflicts = combo.ConflictingCombos;
                            string parent = combo.ParentCombo;
                            ImGui.PushItemWidth(200f);
                            if (ImGui.Checkbox(combo.ComboFancyName, ref enable))
                            {
                                if (enable)
                                {
                                    combo.IsEnabled = true;
                                    string[] array = conflicts;
                                    foreach (string item3 in array)
                                    {
                                        IconReplacer.SetEnable(item3, false);
                                    }
                                }
                                else
                                {
                                    combo.IsEnabled = false;
                                }
                                Service.Configuration.Save();
                            }
                            ImGui.PopItemWidth();
                            string text = $"#{num}: {combo.Description}";
                            if (!string.IsNullOrEmpty(parent))
                            {
                                text += "\nRequires " + combo.ComboFancyName;
                            }
                            ImGui.TextColored(shadedColor, text);
                            ImGui.Spacing();
                            if (conflicts.Length != 0)
                            {
                                ImGui.TextColored(shadedColor, "Conflicts with: " + string.Join("\n - ", conflicts));
                                ImGui.Spacing();
                            }
                            //if (item == CustomComboPreset.DancerDanceComboCompatibility && enable)
                            //{
                            //    int[] array2 = Service.Configuration.DancerDanceCompatActionIDs.Cast<int>().ToArray();
                            //    if (false | ImGui.InputInt("Emboite (Red) ActionID", ref array2[0], 0) | ImGui.InputInt("Entrechat (Blue) ActionID", ref array2[1], 0) | ImGui.InputInt("Jete (Green) ActionID", ref array2[2], 0) | ImGui.InputInt("Pirouette (Yellow) ActionID", ref array2[3], 0))
                            //    {
                            //        Service.Configuration.DancerDanceCompatActionIDs = array2.Cast<uint>().ToArray();
                            //        Service.IconReplacer.UpdateEnabledActionIDs();
                            //        Service.Configuration.Save();
                            //    }
                            //    ImGui.Spacing();
                            //}
                            num++;
                        }
                    }
                    else
                    {
                        num += combos.Length;
                    }
                }

                ImGui.PopStyleVar();
                ImGui.EndChild();

                ImGui.EndTabItem();

            }


            if (ImGui.BeginTabItem("�����趨"))
            {
                ImGui.Text(TargetHelper.HostileTargets.Length.ToString());

                int multiCount = Service.Configuration.MultiCount;
                if (ImGui.DragInt("��Χ����������Ҫ������", ref multiCount, 0.02f, 2, 5))
                {
                    Service.Configuration.MultiCount = multiCount;
                    Service.Configuration.Save();
                }
                ImGui.Separator();

                float speed = 0.005f;
                float healthDiff = Service.Configuration.HealthDifference;
                if (ImGui.DragFloat("���ٵ�HP��׼�����£�������Ⱥ��", ref healthDiff, speed * 2, 0, 0.5f))
                {
                    Service.Configuration.HealthDifference = healthDiff;
                    Service.Configuration.Save();
                }



                float healthAreaA = Service.Configuration.HealthAreaAbility;
                if (ImGui.DragFloat("���ٵ�HP��������������Ⱥ��", ref healthAreaA, speed, 0, 1))
                {
                    Service.Configuration.HealthAreaAbility = healthAreaA;
                    Service.Configuration.Save();
                }

                float healthAreaS = Service.Configuration.HealthAreafSpell;
                if (ImGui.DragFloat("���ٵ�HP��������GCDȺ��", ref healthAreaS, speed, 0, 1))
                {
                    Service.Configuration.HealthAreafSpell = healthAreaS;
                    Service.Configuration.Save();
                }

                ImGui.Separator();

                float healthSingleA = Service.Configuration.HealthSingleAbility;
                if (ImGui.DragFloat("���ٵ�HP������������������", ref healthSingleA, speed, 0, 1))
                {
                    Service.Configuration.HealthSingleAbility = healthSingleA;
                    Service.Configuration.Save();
                }

                float healthSingleS = Service.Configuration.HealthSingleSpell;
                if (ImGui.DragFloat("���ٵ�HP��������GCD����", ref healthSingleS, speed, 0, 1))
                {
                    Service.Configuration.HealthSingleSpell = healthSingleS;
                    Service.Configuration.Save();
                }

                ImGui.EndTabItem();

            }

            ImGui.EndTabBar();
        }
        ImGui.End();
    }
}
