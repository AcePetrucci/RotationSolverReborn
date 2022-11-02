using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using ImGuiNET;
using Lumina.Data.Parsing.Uld;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Combos.Melee;
using XIVAutoAttack.Combos.RangedPhysicial;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.SigReplacers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Windows;

internal class ConfigWindow : Window
{
    private readonly Vector4 shadedColor = new Vector4(0.68f, 0.68f, 0.68f, 1f);

    public ConfigWindow()
        : base("�Զ��������� (��Դ���)", 0, false)
    {
        RespectCloseHotkey = true;

        SizeCondition = (ImGuiCond)4;
        Size = new Vector2(740f, 490f);
    }
    private static readonly Dictionary<Role, string> _roleDescriptionValue = new Dictionary<Role, string>()
    {
        {Role.����, $"{CustomCombo.DescType.�������} �� {CustomCombo.GeneralActions.Rampart}, {CustomCombo.GeneralActions.Reprisal}" },
        {Role.��ս, $"{CustomCombo.DescType.��Χ����} �� {CustomCombo.GeneralActions.Feint}" },
        {Role.Զ��, $"��ϵ{CustomCombo.DescType.��Χ����} �� {CustomCombo.GeneralActions.Addle}" },
    };

    private static string ToName(VirtualKey k)
    {
        return k switch
        {
            VirtualKey.SHIFT => "SHIFT",
            VirtualKey.CONTROL => "CTRL",
            VirtualKey.MENU => "ALT",
            _ => k.ToString(),
        };
    }

    public override unsafe void Draw()
    {
        if (ImGui.BeginTabBar("##tabbar"))
        {
            if (ImGui.BeginTabItem("�����趨"))
            {
                ImGui.Text("�����ѡ������Ҫ��ְҵ������GCDս�������ܡ�");

                ImGui.BeginChild("����", new Vector2(0f, -1f), true);
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));
                int num = 1;


                foreach (Role key in IconReplacer.CustomCombosDict.Keys)
                {
                    var combos = IconReplacer.CustomCombosDict[key];
                    if (combos == null || combos.Length == 0) continue;

                    if (ImGui.CollapsingHeader(key.ToString()))
                    {
                        if (ImGui.IsItemHovered() && _roleDescriptionValue.TryGetValue(key, out string roleDesc))
                        {
                            ImGui.SetTooltip(roleDesc);
                        }
                        for (int i = 0; i < combos.Length; i++)
                        {
                            if (i > 0) ImGui.Separator();
                            var combo = combos[i];

                            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(3f, 3f));

                            ImGui.Columns(2, i.ToString(), false);
                            int size = Math.Min(combo.Texture.Width, 45);
                            ImGui.SetColumnWidth(0, size + 5);

                            var str = string.Join('\n', combo.Description.Select(pair => pair.Key.ToString() + " �� " + pair.Value));

                            ImGui.Image(combo.Texture.ImGuiHandle, new Vector2(size, size));
                            if (ImGui.IsItemHovered())
                            {
                                if (!string.IsNullOrEmpty(str)) ImGui.SetTooltip(str);
                            }

                            ImGui.NextColumn();

                            //ImGui.Spacing();

                            bool enable = combo.IsEnabled;
                            if (ImGui.Checkbox(combo.JobName, ref enable))
                            {
                                combo.IsEnabled = enable;
                                Service.Configuration.Save();
                            }
                            if (ImGui.IsItemHovered())
                            {
                                if (!string.IsNullOrEmpty(str)) ImGui.SetTooltip(str);
                            }
                            string text = $"#{num}: Ϊ{combo.JobName}������GCDս�������ܡ�";
                            ImGui.TextColored(shadedColor, text);

                            if (enable)
                            {
                                ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(1f, 1f));
                                var actions = combo.Config;
                                foreach (var boolean in actions.bools)
                                {
                                    Spacing();
                                    bool val = boolean.value;
                                    if (ImGui.Checkbox($"#{num}: {boolean.description}", ref val))
                                    {
                                        boolean.value = val;
                                        Service.Configuration.Save();
                                    }
                                    if (ImGui.IsItemHovered())
                                    {
                                        ImGui.SetTooltip("�ؼ�����Ϊ��" + boolean.name);
                                    }
                                }
                                foreach (var doubles in actions.doubles)
                                {
                                    Spacing();
                                    float val = doubles.value;
                                    if (ImGui.DragFloat($"#{num}: {doubles.description}", ref val, doubles.speed, doubles.min, doubles.max))
                                    {
                                        doubles.value = val;
                                        Service.Configuration.Save();
                                    }
                                }
                                foreach (var textItem in actions.texts)
                                {
                                    Spacing();
                                    string val = textItem.value;
                                    if (ImGui.InputText($"#{num}: {textItem.description}", ref val, 15))
                                    {
                                        textItem.value = val;
                                        Service.Configuration.Save();
                                    }
                                }
                                foreach (var comboItem in actions.combos)
                                {
                                    Spacing();
                                    int val = comboItem.value;
                                    if (ImGui.Combo($"#{num}: {comboItem.description}", ref val, comboItem.items, comboItem.items.Length))
                                    {
                                        comboItem.value = val;
                                        Service.Configuration.Save();
                                    }
                                }
                                ImGui.PopStyleVar();

                            }
                            ImGui.Columns(1);

                            num++;
                        }
                    }
                    else
                    {
                        if (ImGui.IsItemHovered() && _roleDescriptionValue.TryGetValue(key, out string roleDesc))
                        {
                            ImGui.SetTooltip(roleDesc);
                        }
                        num += combos.Length;
                    }
                }

                ImGui.PopStyleVar();
                ImGui.EndChild();

                ImGui.EndTabItem();
            }


            if (ImGui.BeginTabItem("�����趨"))
            {

                ImGui.Text("��������ڣ�������趨�ͷż�������Ĳ�����");

                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

                if (ImGui.BeginChild("����", new Vector2(0f, -1f), true))
                {
                    bool neverReplaceIcon = Service.Configuration.NeverReplaceIcon;
                    if (ImGui.Checkbox("���滻ͼ��", ref neverReplaceIcon))
                    {
                        Service.Configuration.NeverReplaceIcon = neverReplaceIcon;
                        Service.Configuration.Save();
                    }
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip("������滻ͼ�꣬��ǰ�ж������й��ܽ�ʧЧ������ǰ��ʾ��λ");
                    }
                    if (neverReplaceIcon)
                    {
                        ImGui.TextColored(new Vector4(1, 0, 0, 1), "���滻ͼ�꣬��ǰ�ж������й��ܽ�ʧЧ������ǰ��ʾ��λ");
                    }

                    bool useOverlayWindow = Service.Configuration.UseOverlayWindow;
                    if (ImGui.Checkbox("ʹ����ߴ󸲸Ǵ���", ref useOverlayWindow))
                    {
                        Service.Configuration.UseOverlayWindow = useOverlayWindow;
                        Service.Configuration.Save();
                    }
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip("�������Ŀǰ��������ǰ��ʾ��λ");
                    }

                    if (ImGui.CollapsingHeader("��������"))
                    {

                        float weaponDelay = Service.Configuration.WeaponDelay;
                        if (ImGui.DragFloat("��ҪGCD����ֲж�����", ref weaponDelay, 0.002f, 0, 1))
                        {
                            Service.Configuration.WeaponDelay = weaponDelay;
                            Service.Configuration.Save();
                        }

                        float weaponFaster = Service.Configuration.WeaponFaster;
                        if (ImGui.DragFloat("��Ҫ��ǰ���밴�¼���", ref weaponFaster, 0.002f, 0, 0.1f))
                        {
                            Service.Configuration.WeaponFaster = weaponFaster;
                            Service.Configuration.Save();
                        }

                        float weaponInterval = Service.Configuration.WeaponInterval;
                        if (ImGui.DragFloat("�������ͷ�������", ref weaponInterval, 0.002f, 0.5f, 0.7f))
                        {
                            Service.Configuration.WeaponInterval = weaponInterval;
                            Service.Configuration.Save();
                        }

                        float interruptibleTime = Service.Configuration.InterruptibleTime;
                        if (ImGui.DragFloat("����༼���ӳٶ�ú��ͷ�", ref interruptibleTime, 0.002f, 0, 2))
                        {
                            Service.Configuration.InterruptibleTime = interruptibleTime;
                            Service.Configuration.Save();
                        }

                        float specialDuration = Service.Configuration.SpecialDuration;
                        if (ImGui.DragFloat("����״̬�������", ref specialDuration, 0.02f, 1, 20))
                        {
                            Service.Configuration.SpecialDuration = specialDuration;
                            Service.Configuration.Save();
                        }

                        int addDotGCDCount = Service.Configuration.AddDotGCDCount;
                        if (ImGui.DragInt("�����GCD�Ϳ��Բ�DOT��", ref addDotGCDCount, 0.2f, 0, 3))
                        {
                            Service.Configuration.AddDotGCDCount = addDotGCDCount;
                            Service.Configuration.Save();
                        }
                    }

                    ImGui.Separator();

                    if (ImGui.CollapsingHeader("��ʾ��ǿ"))
                    {
                        bool poslockCasting = Service.Configuration.PoslockCasting;
                        VirtualKey poslockModifier = Service.Configuration.PoslockModifier;
                        if (ImGui.Checkbox("ʹ��ӽ���ƶ���", ref poslockCasting))
                        {
                            Service.Configuration.PoslockCasting = poslockCasting;
                            Service.Configuration.Save();
                        }
                         var modifierChoices = new VirtualKey[]{ VirtualKey.CONTROL, VirtualKey.SHIFT, VirtualKey.MENU };
                        if(poslockCasting && ImGui.BeginCombo("����ӽ�����ȼ�", ToName(poslockModifier)))
                         {
                             foreach (VirtualKey k in modifierChoices)
                             {
                                 if (ImGui.Selectable(ToName(k)))
                                 {
                                     Service.Configuration.PoslockModifier = k;
                                     Service.Configuration.Save();
                                 }
                             }
                            ImGui.EndCombo();
                         }

                        bool usecheckCasting = Service.Configuration.CheckForCasting;
                        if (ImGui.Checkbox("ʹ��ӽ��������ʾ", ref usecheckCasting))
                        {
                            Service.Configuration.CheckForCasting = usecheckCasting;
                            Service.Configuration.Save();
                        }

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

                        bool sayoutLocationWrong = Service.Configuration.SayoutLocationWrong;
                        if (useOverlayWindow && ImGui.Checkbox("��ʾ��λ����", ref sayoutLocationWrong))
                        {
                            Service.Configuration.SayoutLocationWrong = sayoutLocationWrong;
                            Service.Configuration.Save();
                        }

                        var str = Service.Configuration.LocationText;
                        if (ImGui.InputText("��λ������ʾ��", ref str, 15))
                        {
                            Service.Configuration.LocationText = str;
                            Service.Configuration.Save();
                        }
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.SetTooltip("�����λ����������ô����!");
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
                    }

                    ImGui.Separator();

                    if (ImGui.CollapsingHeader("����ʹ��"))
                    {
                        bool useAOEWhenManual = Service.Configuration.UseAOEWhenManual;
                        if (ImGui.Checkbox("���ֶ�ѡ���ʱ��ʹ��AOE����", ref useAOEWhenManual))
                        {
                            Service.Configuration.UseAOEWhenManual = useAOEWhenManual;
                            Service.Configuration.Save();
                        }

                        bool autoBreak = Service.Configuration.AutoBreak;
                        if (ImGui.Checkbox("�Զ����б���", ref autoBreak))
                        {
                            Service.Configuration.AutoBreak = autoBreak;
                            Service.Configuration.Save();
                        }

                        bool isOnlyGCD = Service.Configuration.OnlyGCD;
                        if (ImGui.Checkbox("ֻʹ��GCDѭ������ȥ������", ref isOnlyGCD))
                        {
                            Service.Configuration.OnlyGCD = isOnlyGCD;
                            Service.Configuration.Save();
                        }

                        if (!isOnlyGCD)
                        {
                            Spacing();
                            bool noHealOrDefenceAbility = Service.Configuration.NoDefenceAbility;
                            if (ImGui.Checkbox("��ʹ�÷���������", ref noHealOrDefenceAbility))
                            {
                                Service.Configuration.NoDefenceAbility = noHealOrDefenceAbility;
                                Service.Configuration.Save();
                            }
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.SetTooltip("���Ҫ����ѣ����鹴��������Լ��������ƺ����ᡣ");
                            }

                            Spacing();
                            bool autoDefenseforTank = Service.Configuration.AutoDefenseForTank;
                            if (ImGui.Checkbox("�Զ��ϼ���(��̫׼)", ref autoDefenseforTank))
                            {
                                Service.Configuration.AutoDefenseForTank = autoDefenseforTank;
                                Service.Configuration.Save();
                            }
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.SetTooltip("�Զ����������ʶ������Ϊ0��AOE���ܣ���ע�⡣");
                            }

                            Spacing();
                            bool autoShieled = Service.Configuration.AutoShield;
                            if (ImGui.Checkbox("T�Զ��϶�", ref autoShieled))
                            {
                                Service.Configuration.AutoShield = autoShieled;
                                Service.Configuration.Save();
                            }

                            Spacing();
                            bool autoProvokeforTank = Service.Configuration.AutoProvokeForTank;
                            if (ImGui.Checkbox("T�Զ�����", ref autoProvokeforTank))
                            {
                                Service.Configuration.AutoProvokeForTank = autoProvokeforTank;
                                Service.Configuration.Save();
                            }
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.SetTooltip("���й����ڴ��T��ʱ�򣬻��Զ����ơ�");
                            }

                            Spacing();
                            bool alwaysLowBlow = Service.Configuration.AlwaysLowBlow;
                            if (ImGui.Checkbox("T��Զ����", ref alwaysLowBlow))
                            {
                                Service.Configuration.AlwaysLowBlow = alwaysLowBlow;
                                Service.Configuration.Save();
                            }

                            Spacing();
                            bool autoUseTrueNorth = Service.Configuration.AutoUseTrueNorth;
                            if (ImGui.Checkbox("��ս�Զ����汱", ref autoUseTrueNorth))
                            {
                                Service.Configuration.AutoUseTrueNorth = autoUseTrueNorth;
                                Service.Configuration.Save();
                            }

                            Spacing();
                            bool raiseSwift = Service.Configuration.RaisePlayerBySwift;
                            if (ImGui.Checkbox("��������", ref raiseSwift))
                            {
                                Service.Configuration.RaisePlayerBySwift = raiseSwift;
                                Service.Configuration.Save();
                            }
                        }

                        bool raiseCasting = Service.Configuration.RaisePlayerByCasting;
                        if (ImGui.Checkbox("��Ŀ��ʱӲ��������", ref raiseCasting))
                        {
                            Service.Configuration.RaisePlayerByCasting = raiseCasting;
                            Service.Configuration.Save();
                        }

                        bool useItem = Service.Configuration.UseItem;
                        if (ImGui.Checkbox("ʹ�õ���", ref useItem))
                        {
                            Service.Configuration.UseItem = useItem;
                            Service.Configuration.Save();
                        }
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.SetTooltip("ʹ�ñ���ҩ��Ŀǰ��δдȫ");
                        }
                    }

                    ImGui.Separator();

                    if (ImGui.CollapsingHeader("��������"))
                    {
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
                    }

                    ImGui.Separator();

                    if (ImGui.CollapsingHeader("Ŀ��ѡ��"))
                    {
                        int isAllTargetAsHostile = Service.Configuration.TargetToHostileType;
                        if (ImGui.Combo("�ж�Ŀ��ɸѡ����", ref isAllTargetAsHostile, new string[]
                        {
                                "�����ܴ��Ŀ�궼�ǵжԵ�Ŀ��",
                                "������ڴ��˵�Ŀ������Ϊ�㣬�����ܴ�Ķ��ǵжԵ�",
                                "ֻ�д��˵�Ŀ����ǵжԵ�Ŀ��",
                        }, 3))
                        {
                            Service.Configuration.TargetToHostileType = isAllTargetAsHostile;
                            Service.Configuration.Save();
                        }

                        bool addEnemyListToHostile = Service.Configuration.AddEnemyListToHostile;
                        if (ImGui.Checkbox("���ж��б�Ķ�����Ϊ�ж�", ref addEnemyListToHostile))
                        {
                            Service.Configuration.AddEnemyListToHostile = addEnemyListToHostile;
                            Service.Configuration.Save();
                        }

                        bool chooseAttackMark = Service.Configuration.ChooseAttackMark;
                        if (ImGui.Checkbox("����ѡ���й�����ǵ�Ŀ��", ref chooseAttackMark))
                        {
                            Service.Configuration.ChooseAttackMark = chooseAttackMark;
                            Service.Configuration.Save();
                        }

                        if (chooseAttackMark)
                        {
                            Spacing();
                            bool attackMarkAOE = Service.Configuration.AttackMarkAOE;

                            if (ImGui.Checkbox("�Ƿ�Ҫʹ��AOE", ref attackMarkAOE))
                            {
                                Service.Configuration.AttackMarkAOE = attackMarkAOE;
                                Service.Configuration.Save();
                            }
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.SetTooltip("�����ѡ�ˣ���ô�������AOE�򲻵�����Ŀ��Ķ�����ΪΪ��׷��򵽸����Ŀ�ꡣ");
                            }
                        }

                        bool filterStopMark = Service.Configuration.FilterStopMark;
                        if (ImGui.Checkbox("ȥ����ֹͣ��ǵ�Ŀ��", ref filterStopMark))
                        {
                            Service.Configuration.FilterStopMark = filterStopMark;
                            Service.Configuration.Save();
                        }

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

                        float minradius = Service.Configuration.ObjectMinRadius;
                        if (ImGui.DragFloat("����������С��Ȧ��С", ref minradius, 0.02f, 0, 10))
                        {
                            Service.Configuration.ObjectMinRadius = minradius;
                            Service.Configuration.Save();
                        }

                        //bool changeTargetForFate = Service.Configuration.ChangeTargetForFate;
                        //if (ImGui.Checkbox("��Fate��ֻѡ��Fate��", ref changeTargetForFate))
                        //{
                        //    Service.Configuration.ChangeTargetForFate = changeTargetForFate;
                        //    Service.Configuration.Save();
                        //}

                        bool moveToScreen = Service.Configuration.MoveTowardsScreen;
                        if (ImGui.Checkbox("�ƶ�����ѡ��Ļ���ĵĶ���", ref moveToScreen))
                        {
                            Service.Configuration.MoveTowardsScreen = moveToScreen;
                            Service.Configuration.Save();
                        }
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.SetTooltip("��Ϊ��ʱ�ƶ��Ķ���Ϊ��Ļ���ĵ��Ǹ�����Ϊ��Ϸ��ɫ�泯�Ķ���");
                        }

                        bool raiseAll = Service.Configuration.RaiseAll;
                        if (ImGui.Checkbox("���������ܸ�����ˣ�����С��", ref raiseAll))
                        {
                            Service.Configuration.RaiseAll = raiseAll;
                            Service.Configuration.Save();
                        }
                    }

                    ImGui.Separator();

                    if (ImGui.CollapsingHeader("�ж�ѡ��"))
                    {
                        if (ImGui.Button("���ѡ������"))
                        {
                            Service.Configuration.TargetingTypes.Add(TargetingType.Big);
                        }
                        ImGui.SameLine();
                        Spacing();
                        ImGui.Text("������趨�жԵ�ѡ���Ա�����ս��������л�ѡ��жԵ��߼���");
                        for (int i = 0; i < Service.Configuration.TargetingTypes.Count; i++)
                        {

                            ImGui.Separator();

                            var names = Enum.GetNames(typeof(TargetingType));
                            var targingType = (int)Service.Configuration.TargetingTypes[i];
                            if (ImGui.Combo("�ж�Ŀ��ѡ������" + i.ToString(), ref targingType, names, names.Length))
                            {
                                Service.Configuration.TargetingTypes[i] = (TargetingType)targingType;
                                Service.Configuration.Save();
                            }

                            if (ImGui.Button("��������" + i.ToString()))
                            {
                                if (i != 0)
                                {
                                    var value = Service.Configuration.TargetingTypes[i];
                                    Service.Configuration.TargetingTypes.RemoveAt(i);
                                    Service.Configuration.TargetingTypes.Insert(i - 1, value);
                                }
                            }
                            ImGui.SameLine();
                            Spacing();
                            if (ImGui.Button("��������" + i.ToString()))
                            {
                                if (i < Service.Configuration.TargetingTypes.Count - 1)
                                {
                                    var value = Service.Configuration.TargetingTypes[i];
                                    Service.Configuration.TargetingTypes.RemoveAt(i);
                                    Service.Configuration.TargetingTypes.Insert(i + 1, value);
                                }
                            }

                            ImGui.SameLine();
                            Spacing();

                            if (ImGui.Button("ɾ������" + i.ToString()))
                            {
                                Service.Configuration.TargetingTypes.RemoveAt(i);
                            }
                        }
                    }

                    ImGui.EndChild();
                }
                ImGui.PopStyleVar();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("�����ͷ��¼�"))
            {

                if (ImGui.Button("����¼�"))
                {
                    Service.Configuration.Events.Add(new ActionEventInfo());
                }
                ImGui.SameLine();
                Spacing();
                ImGui.Text("��������ڣ�������趨һЩ�����ͷź�ʹ��ʲô�ꡣ");

                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));


                if (ImGui.BeginChild("�¼��б�", new Vector2(0f, -1f), true))
                {
                    for (int i = 0; i < Service.Configuration.Events.Count; i++)
                    {
                        string name = Service.Configuration.Events[i].Name;
                        if (ImGui.InputText("��������" + i.ToString(), ref name, 50))
                        {
                            Service.Configuration.Events[i].Name = name;
                            Service.Configuration.Save();
                        }

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
                        if (ImGui.Button("ɾ���¼�" + i.ToString()))
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
                    CommandHelp("/aauto HealArea", "����һ�η�Χ���ƵĴ����ڡ�");
                    ImGui.Separator();
                    CommandHelp("/aauto HealSingle", "����һ�ε������ƵĴ����ڡ�");
                    ImGui.Separator();
                    CommandHelp("/aauto DefenseArea", "����һ�η�Χ�����Ĵ����ڡ�");
                    ImGui.Separator();
                    CommandHelp("/aauto DefenseSingle", "����һ�ε�������Ĵ����ڡ�");
                    ImGui.Separator();
                    CommandHelp("/aauto EsunaShield", "����һ�ο������߶��˻����汱�Ĵ����ڡ�");
                    ImGui.Separator();
                    CommandHelp("/aauto RaiseShirk", "����ǿ�ƾ��˻��˱ܵĴ����ڡ�");
                    ImGui.Separator();
                    CommandHelp("/aauto AntiRepulsion", "����һ�η����˵Ĵ����ڡ�");
                    ImGui.Separator();
                    CommandHelp("/aauto BreakProvoke", "����һ�α��������ƵĴ����ڡ�");
                    ImGui.Separator();
                    CommandHelp("/aauto Move", "����һ��λ�ƵĴ����ڡ�");
                    ImGui.Separator();
                    CommandHelp("/aauto AutoBreak", "�����Ƿ��Զ�������");
                    ImGui.Separator();
                    CommandHelp("/aauto AttackSmart", "������ڽ����оͿ�ʼ����������ڽ������л�ѡ��ж�Ŀ��������");
                    ImGui.Separator();
                    CommandHelp("/aauto AttackManual", "��ʼ��������������Ϊ�ֶ�ѡ�񣬴�ʱ�����ͷ�AOE��");
                    ImGui.Separator();
                    CommandHelp("/aauto AttackCancel", "ֹͣ�������ǵ�һ��Ҫ�����ص���");
                    ImGui.Separator();
                }
                ImGui.PopStyleVar();

                ImGui.EndTabItem();
            }

#if DEBUG
            if (ImGui.BeginTabItem("Debug�鿴") && Service.ClientState.LocalPlayer != null)
            {
                if (ImGui.CollapsingHeader("�����Ӹ��Լ���״̬"))
                {
                    foreach (var item in Service.ClientState.LocalPlayer.StatusList)
                    {

                        if (item.SourceID == Service.ClientState.LocalPlayer.ObjectId)
                        {
                            ImGui.Text(item.GameData.Name + item.StatusId);
                        }
                    }
                }

                if (ImGui.CollapsingHeader("Ŀ����Ϣ"))
                {
                    if (Service.TargetManager.Target is BattleChara b)
                    {
                        ImGui.Text("Is Boss: " + b.IsBoss().ToString());
                        ImGui.Text("Has Side: " + b.HasLocationSide().ToString());
                        ImGui.Text("Is Dying: " + b.IsDying().ToString());

                        foreach (var status in b.StatusList)
                        {
                            if (status.SourceID == Service.ClientState.LocalPlayer.ObjectId)
                            {
                                ImGui.Text(status.GameData.Name + status.StatusId);
                            }
                        }
                    }
                }

                if (ImGui.CollapsingHeader("��һ������"))
                {
                    BaseAction baseAction = null;
                    baseAction ??= IconReplacer.nextAction;
                    if (baseAction != null)
                    {
                        ImGui.Text(baseAction.ToString());
                        ImGui.Text("Have One:" + baseAction.HaveOneChargeDEBUG.ToString());
                        ImGui.Text("Is General GCD: " + baseAction.IsGeneralGCD.ToString());
                        ImGui.Text("Is Real GCD: " + baseAction.IsRealGCD.ToString());
                        ImGui.Text("Recast One: " + baseAction.RecastTimeOneChargeDEBUG.ToString());
                        ImGui.Text("Recast Elapsed: " + baseAction.RecastTimeElapsedDEBUG.ToString());
                        ImGui.Text("Recast Remain: " + baseAction.RecastTimeRemainDEBUG.ToString());
                        //ImGui.Text("Status: " + ActionManager.Instance()->GetActionStatus(ActionType.Spell, baseAction.AdjustedID).ToString());

                        ImGui.Text("Cast Cost: " + baseAction.CastTime.ToString());
                        ImGui.Text($"Can Use: {baseAction.ShouldUse(out _)} {baseAction.ShouldUse(out _, mustUse:true)}");
                    }
                }
            }
#endif

            ImGui.EndTabBar();
        }
        ImGui.End();
    }

    private static void Spacing(byte count = 1)
    {
        string s = string.Empty;
        for (int i = 0; i < count; i++)
        {
            s += "    ";
        }
        ImGui.Text(s);
        ImGui.SameLine();
    }
    private static void CommandHelp(string command,string help)
    {
        if (ImGui.Button(command))
        {
            Service.CommandManager.ProcessCommand(command);
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip($"������ִ������: {command}");
        }

        ImGui.SameLine();
        ImGui.Text(" �� " + help);
    }
}
