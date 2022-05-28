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
using XIVComboPlus.Combos;

namespace XIVComboPlus;

internal class ConfigWindow : Window
{
    private readonly Vector4 shadedColor = new Vector4(0.68f, 0.68f, 0.68f, 1f);

    public ConfigWindow()
        : base("�Զ���������", 0, false)
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
                ImGui.Text("��������ڣ�������趨�Լ�ϲ�����Զ������趨��");

                ImGui.BeginChild("����", new Vector2(0f, -1f), true);
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));
                int num = 1;


                //ImGui.Text(IconReplacer.CustomCombosDict.Keys.ToString());
                foreach (string key in IconReplacer.CustomCombosDict.Keys)
                {
                    var combos = IconReplacer.CustomCombosDict[key];
                    if (combos == null || combos.Length == 0) continue;

                    if (ImGui.CollapsingHeader(key))
                    {
                        foreach (var combo in combos)
                        {
                            //ImGui.Text(combo.ComboFancyName);

                            bool enable = combo.IsEnabled;
                            ImGui.PushItemWidth(200f);
                            if (ImGui.Checkbox(combo.JobName, ref enable))
                            {
                                combo.IsEnabled = enable;
                                Service.Configuration.Save();
                            }
                            ImGui.PopItemWidth();
                            string text = $"#{num}: �滻����Ϊ{combo.JobName}������GCDս�������ܡ�";
                            if(!string.IsNullOrEmpty(combo.Description))
                            {
                                text += '\n' + combo.Description;
                            }
                            ImGui.TextColored(shadedColor, text);
                            //ImGui.Spacing();
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
#if DEBUG
                //foreach (var item in Service.ClientState.LocalPlayer.StatusList)
                //{
                //    if (item.SourceID == Service.ClientState.LocalPlayer.ObjectId)
                //    {
                //        ImGui.Text(item.GameData.Name + item.StatusId);
                //    }
                //}

                ImGui.Text(Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.Mounted].ToString());


#endif
                ImGui.Text("��������ڣ�������趨�ͷż�������Ĳ�����");

                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

                if (ImGui.BeginChild("����", new Vector2(0f, -1f), true))
                {
                    int voiceVolume = Service.Configuration.VoiceVolume;
                    if (ImGui.DragInt("��������", ref voiceVolume, 0.2f, 0, 100))
                    {
                        Service.Configuration.VoiceVolume = voiceVolume;
                        Service.Configuration.Save();
                    }

                    bool textlocation = Service.Configuration.TextLocation;
                    if (ImGui.Checkbox("д��ս����λ", ref textlocation))
                    {
                        Service.Configuration.TextLocation = textlocation;
                        Service.Configuration.Save();
                    }

                    bool sayingLocation = Service.Configuration.SayingLocation;
                    if (ImGui.Checkbox("����ս����λ", ref sayingLocation))
                    {
                        Service.Configuration.SayingLocation = sayingLocation;
                        Service.Configuration.Save();
                    }

                    bool autoSayingOut = Service.Configuration.AutoSayingOut;
                    if (ImGui.Checkbox("״̬�仯ʱ����", ref autoSayingOut))
                    {
                        Service.Configuration.AutoSayingOut = autoSayingOut;
                        Service.Configuration.Save();
                    }

                    bool useDtr = Service.Configuration.UseDtr;
                    if (ImGui.Checkbox("״̬��ʾ��ϵͳ��Ϣ��", ref useDtr))
                    {
                        Service.Configuration.UseDtr = useDtr;
                        Service.Configuration.Save();
                    }

                    bool useToast = Service.Configuration.UseToast;
                    if (ImGui.Checkbox("״̬��ʾ����Ļ����", ref useToast))
                    {
                        Service.Configuration.UseToast = useToast;
                        Service.Configuration.Save();
                    }

                    ImGui.Separator();

                    bool autoDefenseforTank = Service.Configuration.AutoDefenseForTank;
                    if (ImGui.Checkbox("T�Զ��ϼ���", ref autoDefenseforTank))
                    {
                        Service.Configuration.AutoDefenseForTank = autoDefenseforTank;
                        Service.Configuration.Save();
                    }

                    bool neverReplaceIcon = Service.Configuration.NeverReplaceIcon;
                    if (ImGui.Checkbox("���滻ͼ��", ref neverReplaceIcon))
                    {
                        Service.Configuration.NeverReplaceIcon = neverReplaceIcon;
                        Service.Configuration.Save();
                    }

                    bool isAllTargetAsHostile = Service.Configuration.AllTargeAsHostile;
                    if (ImGui.Checkbox("���п��Թ�����Ŀ���Ϊ�ж�Ŀ��", ref isAllTargetAsHostile))
                    {
                        Service.Configuration.AllTargeAsHostile = isAllTargetAsHostile;
                        Service.Configuration.Save();
                    }

                    bool isOnlyGCD = Service.Configuration.OnlyGCD;
                    if (ImGui.Checkbox("ֻʹ��GCDѭ������ȥ������", ref isOnlyGCD))
                    {
                        Service.Configuration.OnlyGCD = isOnlyGCD;
                        Service.Configuration.Save();
                    }

                    bool autoBreak = Service.Configuration.AutoBreak;
                    if (ImGui.Checkbox("�Զ����б���", ref autoBreak))
                    {
                        Service.Configuration.AutoBreak = autoBreak;
                        Service.Configuration.Save();
                    }

                    ImGui.Separator();

                    float weaponDelay = Service.Configuration.WeaponDelay;
                    if (ImGui.DragFloat("��ҪGCD����ֲж�����", ref weaponDelay, 0.002f, 0, 1))
                    {
                        Service.Configuration.WeaponDelay = weaponDelay;
                        Service.Configuration.Save();
                    }

                    float weaponInterval = Service.Configuration.WeaponInterval;
                    if (ImGui.DragFloat("�������ͷ�������", ref weaponInterval, 0.002f, 0.6f, 0.7f))
                    {
                        Service.Configuration.WeaponInterval = weaponInterval;
                        Service.Configuration.Save();
                    }

                    float specialDuration = Service.Configuration.SpecialDuration;
                    if (ImGui.DragFloat("����״̬�������", ref specialDuration, 0.02f, 1, 20))
                    {
                        Service.Configuration.SpecialDuration = specialDuration;
                        Service.Configuration.Save();
                    }

                    ImGui.Separator();

                    int multiCount = Service.Configuration.HostileCount;
                    if (ImGui.DragInt("��Χ����������Ҫ������", ref multiCount, 0.02f, 2, 5))
                    {
                        Service.Configuration.HostileCount = multiCount;
                        Service.Configuration.Save();
                    }

                    int partyCount = Service.Configuration.PartyCount;
                    if (ImGui.DragInt("��Χ����������Ҫ������", ref partyCount, 0.02f, 2, 5))
                    {
                        Service.Configuration.PartyCount = partyCount;
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

                    float healthTank = Service.Configuration.HealthForDyingTank;
                    if (ImGui.DragFloat("���ڶ��ٵ�HP��̹��Ҫ�Ŵ�����", ref healthTank, speed, 0, 1))
                    {
                        Service.Configuration.HealthForDyingTank = healthTank;
                        Service.Configuration.Save();
                    }
                    ImGui.EndChild();
                }

                ImGui.PopStyleVar();


                ImGui.EndTabItem();

            }

            if (ImGui.BeginTabItem("�����ͷ��¼�"))
            {
                ImGui.Text("��������ڣ�������趨һЩ�����ͷź�ʹ��ʲô�ꡣ");

                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

                if (ImGui.Button("���"))
                {
                    Service.Configuration.Events.Add(new ActionEvents());
                }

                if (ImGui.BeginChild("�¼�", new Vector2(0f, -1f), true))
                {
                    for (int i = 0; i < Service.Configuration.Events.Count; i++)
                    {
                        string name = Service.Configuration.Events[i].Name;
                        if (ImGui.InputText("��������" + i.ToString(), ref name, 50))
                        {
                            Service.Configuration.Events[i].Name = name;
                            Service.Configuration.Save();
                        }

                        //ImGui.SameLine();

                        int macroindex = Service.Configuration.Events[i].MacroIndex;
                        if (ImGui.DragInt("����" + i.ToString(), ref macroindex, 1, 0, 99))
                        {
                            Service.Configuration.Events[i].MacroIndex = macroindex;
                        }


                        bool isShared = Service.Configuration.Events[i].IsShared;
                        if (ImGui.Checkbox("�����" + i.ToString(), ref isShared))
                        {
                            Service.Configuration.Events[i].IsShared = isShared;
                            Service.Configuration.Save();
                        }

                        ImGui.SameLine();
                        if (ImGui.Button("ɾ��" + i.ToString()))
                        {
                            Service.Configuration.Events.RemoveAt(i);
                        }
                        ImGui.Separator();
                    }
                    ImGui.EndChild();
                }
                ImGui.PopStyleVar();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("�����ĵ�"))
            {
                ImGui.Text("��������ڣ�����Կ���һ��Ѱ������ݡ�");

                if (ImGui.BeginChild("����", new Vector2(0f, -1f), true))
                {

                    ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

                    ImGui.Text("/aauto HealArea ��ʾ����һ�η�Χ���ƵĴ����ڡ�");
                    ImGui.Separator();
                    ImGui.Text("/aauto HealSingle ��ʾ����һ�ε������ƵĴ����ڡ�");
                    ImGui.Separator();
                    ImGui.Text("/aauto DefenseArea ��ʾ����һ�η�Χ�����Ĵ����ڡ�");
                    ImGui.Separator();
                    ImGui.Text("/aauto DefenseSingle ��ʾ����һ�ε�������Ĵ����ڡ�");
                    ImGui.Separator();
                    ImGui.Text("/aauto EsunaShield ��ʾ����һ�ο������߶��˵Ĵ����ڡ�");
                    ImGui.Separator();
                    ImGui.Text("/aauto RaiseShirk ��ʾ����ǿ�ƾ��˻��˱ܵĴ����ڡ�");
                    ImGui.Separator();
                    ImGui.Text("/aauto AntiRepulsion ��ʾ����һ�η����˵Ĵ����ڡ�");
                    ImGui.Separator();
                    ImGui.Text("/aauto BreakProvoke ��ʾ����һ�α��������ƵĴ����ڡ�");
                    ImGui.Separator();
                    ImGui.Text("/aauto AttackBig ��ʼ��������������ΪHitBox���ġ�");
                    ImGui.Separator();
                    ImGui.Text("/aauto AttackSmall ��ʼ��������������ΪHitBox��С�ġ�");
                    ImGui.Separator();
                    ImGui.Text("/aauto AttackManual ��ʼ��������������Ϊ�ֶ�ѡ��");
                    ImGui.Separator();
                    ImGui.Text("/aauto AttackCancel ֹͣ�������ǵ�һ��Ҫ�����ص���");
                    ImGui.Separator();
                    ImGui.Text("/aauto EndSpecial ֹͣ����״̬��");
                    ImGui.EndChild();
                }
                ImGui.PopStyleVar();

                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
        ImGui.End();
    }

    //private static uint GetActionsByName(string name)
    //{
    //    var enumerator = Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().GetEnumerator();

    //    while (enumerator.MoveNext())
    //    {
    //        var action = enumerator.Current;
    //        if (action.Name == name && action.ClassJobLevel != 0 && !action.IsPvP)
    //        {
    //            return action.RowId;
    //        }
    //    }
    //    return 0;
    //}

    //private static string GetActionsByName(uint actionID)
    //{
    //    var act = Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().GetRow(actionID);

    //    return act == null ? "" : act.Name;
    //}
}
