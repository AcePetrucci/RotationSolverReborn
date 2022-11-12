using Dalamud.Game.ClientState.JobGauge.Types;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Tank;


internal abstract class GNBCombo<TCmd> : JobGaugeCombo<GNBGauge, TCmd> where TCmd : Enum
{
    public sealed override uint[] JobIDs => new uint[] { 37 };
    internal sealed override bool HaveShield => Player.HaveStatus(StatusIDs.RoyalGuard);
    private protected override BaseAction Shield => RoyalGuard;

    protected override bool CanHealSingleSpell => false;
    protected override bool CanHealAreaSpell => false;


    public static readonly BaseAction
        //��������
        RoyalGuard = new(16142, shouldEndSpecial: true),

        //����ն
        KeenEdge = new(16137),

        //����
        NoMercy = new(16138),

        //�б���
        BrutalShell = new(16139),

        //αװ
        Camouflage = new(16140)
        {
            BuffsProvide = Rampart.BuffsProvide,
            OtherCheck = BaseAction.TankDefenseSelf,
        },

        //��ħ��
        DemonSlice = new(16141),

        //���׵�
        LightningShot = new(16143),

        //Σ������
        DangerZone = new(16144),

        //Ѹ��ն
        SolidBarrel = new(16145),

        //������
        BurstStrike = new(16162)
        {
            OtherCheck = b => JobGauge.Ammo > 0,
        },

        //����
        Nebula = new(16148)
        {
            BuffsProvide = Rampart.BuffsProvide,
            OtherCheck = BaseAction.TankDefenseSelf,
        },

        //��ħɱ
        DemonSlaughter = new(16149),

        //����
        Aurora = new BaseAction(16151, true)
        {
            BuffsProvide = new[] { StatusIDs.Aurora },
        },

        //��������
        Superbolide = new(16152)
        {
            OtherCheck = BaseAction.TankBreakOtherCheck,
        },

        //������
        SonicBreak = new(16153),

        //�ַ�ն
        RoughDivide = new(16154, shouldEndSpecial: true)
        {
            ChoiceTarget = TargetFilter.FindTargetForMoving
        },

        //����
        GnashingFang = new(16146)
        {
            OtherCheck = b => JobGauge.AmmoComboStep == 0 && JobGauge.Ammo > 0,
        },

        //���γ岨
        BowShock = new(16159),

        //��֮��
        HeartofLight = new(16160, true),

        //ʯ֮��
        HeartofStone = new(16161, true)
        {
            BuffsProvide = Rampart.BuffsProvide,
            ChoiceTarget = TargetFilter.FindAttackedTarget,
        },

        //����֮��
        FatedCircle = new(16163)
        {
            OtherCheck = b => JobGauge.Ammo > (Level >= 88 ? 2 : 1),
        },

        //Ѫ��
        Bloodfest = new(16164)
        {
            OtherCheck = b => JobGauge.Ammo == 0,
        },

        //����
        DoubleDown = new(25760)
        {
            OtherCheck = b => JobGauge.Ammo >= 2,
        },

        //����צ
        SavageClaw = new(16147)
        {
            OtherCheck = b => Service.IconReplacer.OriginalHook(GnashingFang.ID) == SavageClaw.ID,
        },

        //����צ
        WickedTalon = new(16150)
        {
            OtherCheck = b => Service.IconReplacer.OriginalHook(GnashingFang.ID) == WickedTalon.ID,
        },

        //˺��
        JugularRip = new(16156)
        {
            OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == JugularRip.ID,
        },

        //����
        AbdomenTear = new(16157)
        {
            OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == AbdomenTear.ID,
        },

        //��Ŀ
        EyeGouge = new(16158)
        {
            OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == EyeGouge.ID,
        },

        //������
        Hypervelocity = new(25759)
        {
            OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == Hypervelocity.ID,
        };
}

