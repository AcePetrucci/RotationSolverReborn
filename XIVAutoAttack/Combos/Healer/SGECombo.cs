//using Dalamud.Game.ClientState.JobGauge.Types;
//using System.Linq;
//namespace XIVComboPlus.Combos;

//internal class SGECombo : CustomComboJob<DRGGauge>
//{
//    internal override uint JobID => 40;

//    private protected override BaseAction Raise => new BaseAction(24287);

//    internal struct Actions
//    {
//        public static readonly BaseAction
//            //עҩ
//            Dosis = new BaseAction(24283),

//            //����
//            Phlegma = new BaseAction(24289),

//            //���
//            Diagnosis = new BaseAction(24284, true),

//            //�Ĺ�
//            Kardia = new BaseAction(24285, true)
//            {
//                BuffsProvide = new ushort[] { ObjectStatus.Kardia },
//            },

//            //Ԥ��
//            Prognosis = new BaseAction(24286, true),

//            //����
//            Physis = new BaseAction(24288, true),

//            //����
//            Eukrasia = new BaseAction(24290)
//            {
//                BuffsProvide = new ushort[] { ObjectStatus.Eukrasia },
//            },

//            //����
//            Soteria = new BaseAction(24294, true),
//    }

//    private protected override bool HealSingleGCD(uint lastComboActionID, out BaseAction act)
//    {
//        //���
//        if (Actions.Diagnosis.ShouldUseAction(out act)) return true;
//        return false;
//    }

//    private protected override bool GeneralGCD(uint lastComboActionID, out BaseAction act)
//    {
//        //�Ĺ�
//        if (Actions.Kardia.ShouldUseAction(out act)) return true;

//        //����
//        if (Actions.Phlegma.ShouldUseAction(out act)) return true;

//        //עҩ
//        if (Actions.Dosis.ShouldUseAction(out act))
//        {
//            var times = BaseAction.FindStatusFromSelf(Actions.Dosis.Target,
//                new ushort[] { ObjectStatus.EukrasianDosis, ObjectStatus.EukrasianDosis2, ObjectStatus.EukrasianDosis3 });
//            if(times.Length == 0 || times.Max() < 3)
//            {
//                //����
//                if (Actions.Eukrasia.ShouldUseAction(out act)) return true;
//            }
//            act = Actions.Dosis;
//            return true;
//        }

//        return false;
//    }
//    private protected override bool HealAreaGCD(uint lastComboActionID, out BaseAction act)
//    {
//        //Ԥ��
//        if (Actions.Prognosis.ShouldUseAction(out act)) return true;
//        return false;
//    }
//    private protected override bool HealAreaAbility(byte abilityRemain, out BaseAction act)
//    {
//        //����
//        if (Actions.Physis.ShouldUseAction(out act)) return true;
//        return false;
//    }
//    public static class Buffs
//    {
//        public const ushort Placeholder = 0;
//    }

//    public static class Debuffs
//    {
//        public const ushort Placeholder = 0;
//    }

//    public static class Levels
//    {
//        public const ushort Dosis = 1;

//        public const ushort Prognosis = 10;

//        public const ushort Druochole = 45;

//        public const ushort Kerachole = 50;

//        public const ushort Taurochole = 62;

//        public const ushort Ixochole = 52;

//        public const ushort Dosis2 = 72;

//        public const ushort Holos = 76;

//        public const ushort Rizomata = 74;

//        public const ushort Dosis3 = 82;
//    }

//    public const uint Diagnosis = 24284u;

//    public const uint Holos = 24310u;

//    public const uint Ixochole = 24299u;
//}
