using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using ImGuiNET;
using Lumina.Data.Parsing;
using Lumina.Data.Parsing.Uld;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos;
using XIVAutoAttack.Combos.Attributes;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Combos.Melee;
using XIVAutoAttack.Combos.RangedMagicial;
using XIVAutoAttack.Combos.RangedPhysicial;
using XIVAutoAttack.Combos.Tank;
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
        : base("�Զ��������� (��Դ���) v"+ typeof(ConfigWindow).Assembly.GetName().Version.ToString(), 0, false)
    {
        RespectCloseHotkey = true;

        SizeCondition = (ImGuiCond)4;
        Size = new Vector2(740f, 490f);
    }
    private static readonly Dictionary<Role, string> _roleDescriptionValue = new Dictionary<Role, string>()
    {
        {Role.����, $"{DescType.�������} �� {CustomCombo<Enum>.GeneralActions.Rampart}, {CustomCombo<Enum>.GeneralActions.Reprisal}" },
        {Role.��ս, $"{DescType.��Χ����} �� {CustomCombo<Enum>.GeneralActions.Feint}" },
        {Role.Զ��, $"��ϵ{DescType.��Χ����} �� {CustomCombo<Enum>.GeneralActions.Addle}" },
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
        if (ImGui.BeginTabBar("AutoAttack"))
        {
            if (ImGui.BeginTabItem("�����趨"))
            {
                ImGui.Text("�����ѡ������Ҫ��ְҵ������GCDս�������ܣ���ְҵ�뵱ǰְҵ��ͬ�����������ʾ��");

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
                            var canAddButton = Service.ClientState.LocalPlayer != null && combo.JobIDs.Contains( Service.ClientState.LocalPlayer.ClassJob.Id);

                            DrawTexture(combo, () =>
                            {
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

                                    //��ʾ�������õİ���
                                    if (canAddButton)
                                    {
                                        ImGui.SameLine();
                                        Spacing();
                                        CommandHelp(boolean.name);
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
                                    if (ImGui.BeginCombo($"#{num}: {comboItem.description}", comboItem.items[comboItem.value]))
                                    {
                                        for (int comboIndex = 0; comboIndex < comboItem.items.Length; comboIndex++)
                                        {
                                            if (ImGui.Selectable(comboItem.items[comboIndex]))
                                            {
                                                comboItem.value = comboIndex;
                                                Service.Configuration.Save();
                                            }
                                            if (canAddButton)
                                            {
                                                ImGui.SameLine();
                                                Spacing();
                                                CommandHelp(comboItem.name + comboIndex.ToString());
                                            }
                                        }
                                        ImGui.EndCombo();
                                    }
                                    if (ImGui.IsItemHovered())
                                    {
                                        ImGui.SetTooltip("�ؼ�����Ϊ��" + comboItem.name);
                                    }

                                    //��ʾ�������õİ���
                                    if (canAddButton)
                                    {
                                        ImGui.SameLine();
                                        Spacing();
                                        CommandHelp(comboItem.name);
                                    }
                                }

                                if (canAddButton)
                                {
                                    ImGui.NewLine();

                                    foreach (var customCMD in combo.CommandShow)
                                    {
                                        Spacing();
                                        CommandHelp(customCMD.Key, customCMD.Value);
                                    }
                                }

                            });

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

                    bool useOverlayWindow = Service.Configuration.UseOverlayWindow;
                    if (ImGui.Checkbox("ʹ����ߴ󸲸Ǵ���", ref useOverlayWindow))
                    {
                        Service.Configuration.UseOverlayWindow = useOverlayWindow;
                        Service.Configuration.Save();
                    }
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip("�������Ŀǰ������ǰ��ʾ��λ��");
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
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.SetTooltip("�ֱ����Ϊ����LT+RT����ӽ����");
                        }

                        bool usecheckCasting = Service.Configuration.CheckForCasting;
                        if (ImGui.Checkbox("ʹ��ӽ��������ʾ", ref usecheckCasting))
                        {
                            Service.Configuration.CheckForCasting = usecheckCasting;
                            Service.Configuration.Save();
                        }

                        bool teachingMode = Service.Configuration.TeachingMode;
                        if (ImGui.Checkbox("ѭ������ģʽ", ref teachingMode))
                        {
                            Service.Configuration.TeachingMode = teachingMode;
                            Service.Configuration.Save();
                        }
                        if (teachingMode)
                        {
                            ImGui.SameLine();
                            Spacing();

                            var teachingColor = Service.Configuration.TeachingModeColor;

                            if(ImGui.ColorEdit3("����ģʽ��ɫ", ref teachingColor))
                            {
                                Service.Configuration.TeachingModeColor = teachingColor;
                                Service.Configuration.Save();
                            }
                        }

                        bool keyBoardNoise = Service.Configuration.KeyBoardNoise;
                        if (ImGui.Checkbox("ģ�ⰴ�¼���Ч��", ref keyBoardNoise))
                        {
                            Service.Configuration.KeyBoardNoise = keyBoardNoise;
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
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.SetTooltip("��λ�����Ǻ�׼�������ο���");
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
                        ImGui.SameLine();
                        Spacing();
                        CommandHelp("AutoBreak");


                        bool isOnlyGCD = Service.Configuration.OnlyGCD;
                        if (ImGui.Checkbox("ֻʹ��GCDѭ������ȥ������", ref isOnlyGCD))
                        {
                            Service.Configuration.OnlyGCD = isOnlyGCD;
                            Service.Configuration.Save();
                        }

                        bool attackSafeMode = Service.Configuration.AttackSafeMode;
                        if (ImGui.Checkbox("������ȫģʽ", ref attackSafeMode))
                        {
                            Service.Configuration.AttackSafeMode = attackSafeMode;
                            Service.Configuration.Save();
                        }
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.SetTooltip("���Ա�֤�ڵ�Ŀ���ʱ�򲻴�AOE���������Ҳ�ǡ���������ֵ������ﵽ��׼��Ȼ��ʹ��AOE��");
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

                            Spacing();
                            bool useAreaAbilityFriendly = Service.Configuration.UseAreaAbilityFriendly;
                            if (ImGui.Checkbox("ʹ���ѷ�������ü���", ref useAreaAbilityFriendly))
                            {
                                Service.Configuration.UseAreaAbilityFriendly = useAreaAbilityFriendly;
                                Service.Configuration.Save();
                            }
                        }

                        bool raiseCasting = Service.Configuration.RaisePlayerByCasting;
                        if (ImGui.Checkbox("��Ŀ��ʱӲ��������", ref raiseCasting))
                        {
                            Service.Configuration.RaisePlayerByCasting = raiseCasting;
                            Service.Configuration.Save();
                        }

                        bool useHealWhenNotAHealer = Service.Configuration.UseHealWhenNotAHealer;
                        if (ImGui.Checkbox("�������Ƿ�Ҫ�����˵ļ���", ref useHealWhenNotAHealer))
                        {
                            Service.Configuration.UseHealWhenNotAHealer = useHealWhenNotAHealer;
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
                        bool autoStartCountdown = Service.Configuration.AutoStartCountdown;
                        if (ImGui.Checkbox("����ʱʱ�Զ��򿪹���", ref autoStartCountdown))
                        {
                            Service.Configuration.AutoStartCountdown = autoStartCountdown;
                            Service.Configuration.Save();
                        }

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

                        bool changeTargetForFate = Service.Configuration.ChangeTargetForFate;
                        if (ImGui.Checkbox("��Fate��ֻѡ��Fate��", ref changeTargetForFate))
                        {
                            Service.Configuration.ChangeTargetForFate = changeTargetForFate;
                            Service.Configuration.Save();
                        }

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
                        Spacing();
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

            if (ImGui.BeginTabItem("�����ͷ�����"))
            {
                foreach (var pair in IconReplacer.RightComboBaseAction.GroupBy(a => a.CateName).OrderBy(g => g.Key))
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
                    ImGui.Text("");
                    foreach (var item in TargetUpdater.HostileTargets)
                    {
                        ImGui.Text(item.Name.ToString());
                    }
                }

                if (ImGui.CollapsingHeader("��һ������"))
                {
                    BaseAction baseAction = null;
                    baseAction ??= ActionUpdater.NextAction as BaseAction;
                    DrawAction(baseAction);

                    ImGui.Text("Ability Remain: " + ActionUpdater.AbilityRemain.ToString());
                    ImGui.Text("Ability Count: " + ActionUpdater.AbilityRemainCount.ToString());

                }

                if (ImGui.CollapsingHeader("��һ������"))
                {
                    DrawAction(Watcher.LastAction, nameof(Watcher.LastAction));
                    DrawAction(Watcher.LastAbility, nameof(Watcher.LastAbility));
                    DrawAction(Watcher.LastSpell, nameof(Watcher.LastSpell));
                    DrawAction(Watcher.LastWeaponskill, nameof(Watcher.LastWeaponskill));
                    DrawAction(Service.Address.LastComboAction, nameof(Service.Address.LastComboAction));
                }

                if (ImGui.CollapsingHeader("����ʱ������"))
                {
                    ImGui.Text("Count Down: " + CountDown.CountDownTime.ToString());

                    if(ActionUpdater.exception != null)
                    {
                        ImGui.Text(ActionUpdater.exception.Message);
                        ImGui.Text(ActionUpdater.exception.StackTrace);
                    }
                }
            }
#endif

            ImGui.EndTabBar();
        }
        ImGui.End();
    }

    private unsafe static void DrawAction(BaseAction act)
    {
        if (act == null) return;

        DrawTexture(act, () =>
        {
            CommandHelp("Enable" + act.Name, $"ʹ��{act}");
            CommandHelp("Disable" + act.Name, $"�ر�{act}");
            CommandHelp($"Insert{act}-{5}", $"5s��������Ȳ���{act}");
#if DEBUG
            ImGui.Text(act.ToString());
            ImGui.Text("Have One:" + act.HaveOneChargeDEBUG.ToString());
            ImGui.Text("Is General GCD: " + act.IsGeneralGCD.ToString());
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

    private static void DrawTexture<T>(T texture, System.Action otherThing) where T : class, ITexture
    {
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(3f, 3f));

        ImGui.Columns(2, texture.Name, false);
        int size = Math.Min(texture.Texture.Width, 45);
        ImGui.SetColumnWidth(0, size + 5);

        var str = texture.Description;

        ImGui.Image(texture.Texture.ImGuiHandle, new Vector2(size, size));
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

        var attr = Attribute.GetCustomAttribute(texture.GetType(), typeof(ComboDevInfoAttribute));
        if (attr is ComboDevInfoAttribute devAttr)
        {
            if (ImGui.Button("Դ��"))
            {
                System.Diagnostics.Process.Start(devAttr.URL);
            }
            ImGui.SameLine();
            Spacing();
            ImGui.Text($" - {string.Join(", ", devAttr.Authors.Select(a => a.ToName()))}");
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

#if DEBUG
    private static void DrawAction(uint id, string type)
    {
        var action = new BaseAction(id);

        ImGui.Text($"{type}: {action}");

        action.Dispose();
    }
#endif

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
}
