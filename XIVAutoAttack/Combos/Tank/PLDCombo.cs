using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Attributes;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.Tank.PLDCombo;

namespace XIVAutoAttack.Combos.Tank;

[ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/XIVAutoAttack/Combos/Tank/PLDCombo.cs",
   ComboAuthor.Armolion)]
internal sealed class PLDCombo : JobGaugeCombo<PLDGauge, CommandType>
{
    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //д��ע�Ͱ���������ʾ�û��ġ�
    };
    public override uint[] JobIDs => new uint[] { 19, 1 };

    internal override bool HaveShield => Player.HaveStatus(ObjectStatus.IronWill);

    private protected override BaseAction Shield => IronWill;

    protected override bool CanHealSingleSpell => TargetUpdater.PartyMembers.Length == 1 && base.CanHealSingleSpell;

    /// <summary>
    /// ��4�˱��ĵ����Ѿ��ۺùֿ���ʹ����ؼ���(���ƶ�������д���3ֻС��)
    /// </summary>
    private static bool CanUseSpellInDungeonsMiddle => TargetUpdater.PartyMembers.Length is > 1 and <= 4 && !Target.IsBoss() && !IsMoving
                                                    && TargetFilter.GetObjectInRadius(TargetUpdater.HostileTargets, 5).Length >= 3;

    /// <summary>
    /// ��4�˱��ĵ���
    /// </summary>
    private static bool InDungeonsMiddle => TargetUpdater.PartyMembers.Length is > 1 and <= 4 && !Target.IsBoss();

    private bool SlowLoop = false;

    public static readonly BaseAction
        //��������
        IronWill = new(28, shouldEndSpecial: true),

        //�ȷ潣
        FastBlade = new(9),

        //���ҽ�
        RiotBlade = new(15),

        //��Ѫ��
        GoringBlade = new(3538, isDot: true)
        {
            TargetStatus = new[]
            {
                    ObjectStatus.GoringBlade,
                    ObjectStatus.BladeofValor,
            }
        },

        //սŮ��֮ŭ
        RageofHalone = new(21),

        //��Ȩ��
        RoyalAuthority = new(3539),

        //Ͷ��
        ShieldLob = new(24)
        {
            FilterForTarget = b => TargetFilter.ProvokeTarget(b),
        },

        //ս�ӷ�Ӧ
        FightorFlight = new(20)
        {
            OtherCheck = b =>
            {
                return true;
            },
        },

        //ȫʴն
        TotalEclipse = new(7381),

        //����ն
        Prominence = new(16457),

        //Ԥ��
        Sentinel = new(17)
        {
            BuffsProvide = Rampart.BuffsProvide,
            OtherCheck = BaseAction.TankDefenseSelf,
        },

        //������ת
        CircleofScorn = new(23)
        {
            //OtherCheck = b =>
            //{
            //    if (LocalPlayer.HaveStatus(ObjectStatus.FightOrFlight)) return true;

            //    if (FightorFlight.IsCoolDown) return true;

            //    return false;
            //}
        },

        //���֮��
        SpiritsWithin = new(29)
        {
            //OtherCheck = b =>
            //{
            //    if (LocalPlayer.HaveStatus(ObjectStatus.FightOrFlight)) return true;

            //    if (FightorFlight.IsCoolDown) return true;

            //    return false;
            //}
        },

        //��ʥ����
        HallowedGround = new(30)
        {
            OtherCheck = BaseAction.TankBreakOtherCheck,
        },

        //ʥ��Ļ��
        DivineVeil = new(3540),

        //���ʺ���
        Clemency = new(3541, true, true),

        //��Ԥ
        Intervention = new(7382, true)
        {
            ChoiceTarget = TargetFilter.FindAttackedTarget,
        },

        //��ͣ
        Intervene = new(16461, shouldEndSpecial: true)
        {
            ChoiceTarget = TargetFilter.FindTargetForMoving,
        },

        //���｣
        Atonement = new(16460)
        {
            BuffsNeed = new[] { ObjectStatus.SwordOath },
        },

        //���꽣
        Expiacion = new(25747),

        //Ӣ��֮��
        BladeofValor = new(25750),

        //����֮��
        BladeofTruth = new(25749),

        //����֮��
        BladeofFaith = new(25748)
        {
            BuffsNeed = new[] { ObjectStatus.ReadyForBladeofFaith },
        },

        //������
        Requiescat = new(7383),

        //����
        Confiteor = new(16459)
        {
            OtherCheck = b => Player.CurrentMp >= 1000,
        },

        //ʥ��
        HolyCircle = new(16458)
        {
            OtherCheck = b => Player.CurrentMp >= 2000,
        },

        //ʥ��
        HolySpirit = new(7384)
        {
            OtherCheck = b => Player.CurrentMp >= 2000,
        },

        //��װ����
        PassageofArms = new(7385),

        //����
        Cover = new BaseAction(27, true)
        {
            ChoiceTarget = TargetFilter.FindAttackedTarget,
        },

        //����
        Sheltron = new(3542);
    //�����ͻ�
    //ShieldBash = new BaseAction(16),
    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.��������, $"{Clemency}"},
        {DescType.��Χ����, $"{DivineVeil}, {PassageofArms}"},
        {DescType.�������, $"{Sentinel}, {Sheltron}"},
        {DescType.�ƶ�����, $"{Intervene}"},
    };

    private protected override bool GeneralGCD(out IAction act)
    {
        //��������
        if (BladeofValor.ShouldUse(out act, mustUse: true)) return true;
        if (BladeofFaith.ShouldUse(out act, mustUse: true)) return true;
        if (BladeofTruth.ShouldUse(out act, mustUse: true)) return true;

        //ħ����������
        if (CanUseConfiteor(out act)) return true;

        //AOE ����
        if (Prominence.ShouldUse(out act)) return true;
        if (TotalEclipse.ShouldUse(out act)) return true;

        //���｣
        if (Atonement.ShouldUse(out act))
        {
            if (!SlowLoop && Player.HaveStatus(ObjectStatus.FightOrFlight)
                   && IsLastWeaponSkill(true, Atonement, RoyalAuthority)
                   && !Player.WillStatusEndGCD(2, 0, true, ObjectStatus.FightOrFlight)) return true;
            if (!SlowLoop && Player.FindStatusStack(ObjectStatus.SwordOath) > 1) return true;

            if (SlowLoop) return true;
        }
        //��������
        if (GoringBlade.ShouldUse(out act)) return true;
        if (RageofHalone.ShouldUse(out act)) return true;
        if (RiotBlade.ShouldUse(out act)) return true;
        if (FastBlade.ShouldUse(out act)) return true;

        //Ͷ��
        if (CommandController.Move && MoveAbility(1, out act)) return true;
        if (ShieldLob.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //��ͣ
        if (Intervene.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(out IAction act)
    {
        //���ʺ���
        if (Clemency.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //ʥ��Ļ��
        if (DivineVeil.ShouldUse(out act)) return true;

        //��װ����
        if (PassageofArms.ShouldUse(out act)) return true;

        if (Reprisal.ShouldUse(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak)
        {
            //ս�ӷ�Ӧ ��Buff
            if (abilityRemain == 1 && CanUseFightorFlight(out act))
            {
                return true;
            }

            //������
            //if (SlowLoop && CanUseRequiescat(out act)) return true;
            if (abilityRemain == 1 && CanUseRequiescat(out act)) return true;
        }


        //������ת
        if (CircleofScorn.ShouldUse(out act, mustUse: true))
        {
            if (InDungeonsMiddle) return true;

            if (FightorFlight.ElapsedAfterGCD(2)) return true;

            //if (SlowLoop && inOpener && IsLastWeaponSkill(false, Actions.RiotBlade)) return true;

            //if (!SlowLoop && inOpener && OpenerStatus && IsLastWeaponSkill(true, Actions.RiotBlade)) return true;

        }

        //���֮��
        if (SpiritsWithin.ShouldUse(out act, mustUse: true))
        {
            //if (SlowLoop && inOpener && IsLastWeaponSkill(true, Actions.RiotBlade)) return true;

            if (InDungeonsMiddle) return true;

            if (FightorFlight.ElapsedAfterGCD(3)) return true;
        }

        //��ͣ
        if (Intervene.Target.DistanceToPlayer() < 1 && !IsMoving && Target.HaveStatus(ObjectStatus.GoringBlade))
        {
            if (FightorFlight.ElapsedAfterGCD(2) && Intervene.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

            if (Intervene.ShouldUse(out act)) return true;
        }

        //Special Defense.
        if (JobGauge.OathGauge == 100 && Defense(out act) && Player.CurrentHp < Player.MaxHp) return true;

        act = null;
        return false;
    }
    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //��ʥ���� ���л�����ˡ�
        if (HallowedGround.ShouldUse(out act)) return true;
        return false;
    }
    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (Defense(out act)) return true;

        if (abilityRemain == 1)
        {
            //Ԥ��������30%��
            if (Sentinel.ShouldUse(out act)) return true;

            //���ڣ�����20%��
            if (Rampart.ShouldUse(out act)) return true;
        }
        //���͹���
        //ѩ��
        if (Reprisal.ShouldUse(out act)) return true;

        //��Ԥ������10%��
        if (!HaveShield && Intervention.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    /// <summary>
    /// �ж��ܷ�ʹ��ս�ӷ�Ӧ
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool CanUseFightorFlight(out IAction act)
    {
        if (FightorFlight.ShouldUse(out act))
        {
            //��4�˱�����
            if (InDungeonsMiddle)
            {
                if (CanUseSpellInDungeonsMiddle && !Player.HaveStatus(ObjectStatus.Requiescat)
                    && !Player.HaveStatus(ObjectStatus.ReadyForBladeofFaith)
                    && Player.CurrentMp < 2000) return true;

                return false;
            }

            if (SlowLoop)
            {
                //if (openerFinished && Actions.Requiescat.ElapsedAfterGCD(12)) return true;

            }
            else
            {
                //�������ȷ潣��
                return true;

            }


        }

        act = null;
        return false;
    }

    /// <summary>
    /// �ж��ܷ�ʹ�ð�����
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool CanUseRequiescat(out IAction act)
    {
        //������
        if (Requiescat.ShouldUse(out act, mustUse: true))
        {
            //��4�˱�����
            if (InDungeonsMiddle)
            {
                if (CanUseSpellInDungeonsMiddle) return true;

                return false;
            }

            //��ѭ��
            if (SlowLoop)
            {
                //if (inOpener && IsLastWeaponSkill(true, Actions.FastBlade)) return true;

                //if (openerFinished && Actions.FightorFlight.ElapsedAfterGCD(12)) return true;
            }
            else
            {
                //��ս��buffʱ��ʣ17������ʱ�ͷ�
                if (Player.HaveStatus(ObjectStatus.FightOrFlight) && Player.WillStatusEnd(17, false, ObjectStatus.FightOrFlight) && Target.HaveStatus(ObjectStatus.GoringBlade))
                {
                    //��������ʱ,��Ȩ�����ͷ�
                    return true;
                }
            }

        }

        act = null;
        return false;
    }


    /// <summary>
    /// ����,ʥ��,ʥ��
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool CanUseConfiteor(out IAction act)
    {
        act = null;
        if (Player.HaveStatus(ObjectStatus.SwordOath)) return false;

        //�а�����buff,��û��ս����
        if (Player.HaveStatus(ObjectStatus.Requiescat) && !Player.HaveStatus(ObjectStatus.FightOrFlight))
        {
            if (SlowLoop && (!IsLastWeaponSkill(true, GoringBlade) && !IsLastWeaponSkill(true, Atonement))) return false;

            var statusStack = Player.FindStatusStack(ObjectStatus.Requiescat);
            if (statusStack == 1 || (Player.HaveStatus(ObjectStatus.Requiescat) && Player.WillStatusEnd(3, false, ObjectStatus.Requiescat)) || Player.CurrentMp <= 2000)
            {
                if (Confiteor.ShouldUse(out act, mustUse: true)) return true;
            }
            else
            {
                if (HolyCircle.ShouldUse(out act)) return true;
                if (HolySpirit.ShouldUse(out act)) return true;
            }
        }

        act = null;
        return false;
    }

    private bool Defense(out IAction act)
    {
        act = null;
        if (JobGauge.OathGauge < 50) return false;

        if (HaveShield)
        {
            //����
            if (Sheltron.ShouldUse(out act)) return true;
        }
        else
        {
            //����
            if (Cover.ShouldUse(out act)) return true;
        }

        return false;
    }
}
