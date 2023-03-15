﻿using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic;
using RotationSolver.Basic.Configuration;
using RotationSolver.Localization;
using RotationSolver.UI;
using System.Numerics;

namespace RotationSolver.Windows.RotationConfigWindow;

internal partial class RotationConfigWindow
{
    private void DrawEventTab()
    {
        if (ImGui.Button(LocalizationManager.RightLang.Configwindow_Events_AddEvent))
        {
            Service.Config.Events.Add(new ActionEventInfo());
            Service.Config.Save();
        }
        ImGui.SameLine();
        ImGuiHelper.Spacing();

        ImGui.TextWrapped(LocalizationManager.RightLang.Configwindow_Events_Description);

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

#if DEBUG
        ImGui.Text(LocalizationManager.RightLang.Configwindow_Events_DutyStart);
        ImGui.SameLine();
        ImGuiHelper.Spacing();
        Service.Config.DutyStart.DisplayMacro();

        ImGui.Text(LocalizationManager.RightLang.Configwindow_Events_DutyEnd);
        ImGui.SameLine();
        ImGuiHelper.Spacing();
        Service.Config.DutyEnd.DisplayMacro();
#endif

        if (ImGui.BeginChild("Events List", new Vector2(0f, -1f), true))
        {
            ActionEventInfo remove = null;
            foreach (var eve in Service.Config.Events)
            {
                eve.DisplayMacro();

                ImGui.SameLine();
                ImGuiHelper.Spacing();

                if (ImGui.Button($"{LocalizationManager.RightLang.Configwindow_Events_RemoveEvent}##RemoveEvent{eve.GetHashCode()}"))
                {
                    remove = eve;
                }
                ImGui.Separator();
            }
            if(remove!= null)
            {
                Service.Config.Events.Remove(remove);
                Service.Config.Save();
            }

            ImGui.EndChild();
        }
        ImGui.PopStyleVar();
    }
}