using System;

namespace XIVComboExpandedestPlugin
{
    public enum CustomComboPreset
    {
        // Last enum used: 98
        // Unused enums: 73, 76, 77, 83, 86, 92
        // ====================================================================================
        #region DRAGOON

        [CustomComboInfo("��Ծ - �����", "���ڻ����Ԥ��״̬�£��滻��ԾΪ�����", DRG.JobID, DRG.Jump, DRG.HighJump)]
        DragoonJumpFeature = 44,

        [CustomComboInfo("������Ѫ - ׹�ǳ�", "�����ں�����Ѫ״̬��,�滻������ѪΪ׹�ǳ�", DRG.JobID, DRG.BloodOfTheDragon)]
        DragoonBOTDFeature = 46,

        [CustomComboInfo("ɽ����������", "�滻ɽ������Ϊ��Ӧ������", DRG.JobID, DRG.CoerthanTorment)]
        DragoonCoerthanTormentCombo = 0,

        [CustomComboInfo("ӣ��ŭ������", "�滻ӣ��ŭ��Ϊ��Ӧ������", DRG.JobID, DRG.ChaosThrust)]
        DragoonChaosThrustCombo = 1,

        [CustomComboInfo("ֱ������", "�滻ֱ��Ϊ��Ӧ������", DRG.JobID, DRG.FullThrust)]
        DragoonFullThrustCombo = 2,

        #endregion
        // ====================================================================================
        #region DARK KNIGHT

        [CustomComboInfo("�ɻ�ն����", "�滻�ɻ�նΪ��Ӧ������", DRK.JobID, DRK.Souleater)]
        DarkSouleaterCombo = 3,

        [CustomComboInfo("�ջ�����", "�滻�ջ�Ϊ��Ӧ������", DRK.JobID, DRK.StalwartSoul)]
        DarkStalwartSoulCombo = 4,

        [CustomComboInfo("Ѫ��״̬", "������ǰ���������ִ���Ѫ��״̬ʱ���滻Ѫ�һ�Ѫ��", DRK.JobID, DRK.Souleater, DRK.StalwartSoul)]
        DeliriumFeature = 57,
        
        [CustomComboInfo("��Ѫ״̬", "���㿪��ǰ������ʱ����Ѫ����70�Զ������滻Ѫ�һ�Ѫ��./n�����Զ����滻��ֵ���������ֵС��50ʱ��Ϊ50", DRK.JobID, DRK.StalwartSoul)]
        DRKOvercapFeature = 71,

        [CustomComboInfo("����buff", "����ħ������8000ʱ�滻��Ӱ����߰�Ӱ����", DRK.JobID, DRK.Souleater, DRK.StalwartSoul)]
        DRKMPOvercapFeature = 73,

        [CustomComboInfo("����״̬", "����CD��ʱ���滻����Ӧ������", DRK.JobID, DRK.Souleater, DRK.StalwartSoul)]
        DRKFoLeiFeature = 107,
        #endregion
        // ====================================================================================
        #region PALADIN

        [CustomComboInfo("��Ѫ������", "�滻��Ѫ��Ϊ��Ӧ������", PLD.JobID, PLD.GoringBlade)]
        PaladinGoringBladeCombo = 5,

        [CustomComboInfo("��Ȩ������", "�滻 ��Ȩ��/սŮ��֮ŭΪ��Ӧ������", PLD.JobID, PLD.RoyalAuthority, PLD.RageOfHalone)]
        PaladinRoyalAuthorityCombo = 6,

        [CustomComboInfo("���｣״̬", "��ǰ�濪����Ȩ������ʱ�������｣״̬ʱ�����｣�滻��Ȩ������", PLD.JobID, PLD.RoyalAuthority)]
        PaladinAtonementFeature = 59,

        [CustomComboInfo("����ն����", "�滻����նΪ��Ӧ������", PLD.JobID, PLD.Prominence)]
        PaladinProminenceCombo = 7,

        [CustomComboInfo("������ - ����", "�����ڰ�����״̬��,�滻������Ϊ����", PLD.JobID, PLD.Requiescat)]
        PaladinRequiescatCombo = 55,

        [CustomComboInfo("����״̬", "������ǰ�氲����ʱ�����ڰ���״̬��ʥ���滻��Ӧ����.", PLD.JobID, PLD.HolyCircle,PLD.HolySpirit,PLD.RoyalAuthority, PLD.GoringBlade, PLD.Prominence)]
        PaladinRequiescatFeature = 63,

        [CustomComboInfo("����״̬", "����ħ������4000ʱ���Զ��滻����.", PLD.JobID, PLD.HolySpirit, PLD.HolyCircle)]
        PaladinConfiteorFeature = 68,

        #endregion
        // ====================================================================================
        #region WARRIOR

        [CustomComboInfo("����ն����", "�滻����նΪ��Ӧ������", WAR.JobID, WAR.StormsPath)]
        WarriorStormsPathCombo = 8,

        [CustomComboInfo("����������", "�滻������Ϊ��Ӧ������", WAR.JobID, WAR.StormsEye)]
        WarriorStormsEyeCombo = 9,

        [CustomComboInfo("������������", "�滻��������Ϊ��Ӧ������", WAR.JobID, WAR.MythrilTempest)]
        WarriorMythrilTempestCombo = 10,

        [CustomComboInfo ("�޻�����״̬", "�޻����տ���ʱ����ս���Զ��滻��Ӧ����", WAR.JobID, WAR.MythrilTempest, WAR.StormsEye, WAR.StormsPath)]
        WarriorGaugeOvercapFeature = 67,

        [CustomComboInfo("ս�������滻", "�޻����տ���ʱս���Զ��滻��Ӧ����", WAR.JobID,WAR.Infuriate)]
        WarriorInfuriateOvercapFeature = 104,

        [CustomComboInfo("����buff", "�������ʱ��С��15s���ҿ���ǰ������������滻�ɱ�����", WAR.JobID, WAR.StormsPath)]
        WARBuffpFeature = 105,

        [CustomComboInfo("ԭ���Ľ��״̬", "ԭ�����״̬ʱ������ʯ�ɻ�/�ػ������滻��Ӧ����", WAR.JobID, WAR.MythrilTempest, WAR.StormsPath)]
        WarriorInnerReleaseFeature = 69,

        [CustomComboInfo("ԭ��������״̬", "ͬ����76����ʱ��ԭ����ֱ���滻ԭ��������", WAR.JobID, WAR.NascentFlash)]
        WarriorNascentFlashFeature = 96,

        [CustomComboInfo("����״̬", "�ڿ���ǰ����������¶���CD�����滻��Ӧ����", WAR.JobID, WAR.StormsPath,WAR.StormsEye)]
        WarriorDongLuanFeature = 109,
        #endregion
        // ====================================================================================
        #region SAMURAI

        [CustomComboInfo("ѩ������", "�滻ѩ��Ϊ��Ӧ������", SAM.JobID, SAM.Yukikaze)]
        SamuraiYukikazeCombo = 11,

        [CustomComboInfo("�¹�����", "�滻�¹�Ϊ��Ӧ������", SAM.JobID, SAM.Gekko)]
        SamuraiGekkoCombo = 12,

        [CustomComboInfo("��������", "�滻����Ϊ��Ӧ������", SAM.JobID, SAM.Kasha)]
        SamuraiKashaCombo = 13,

        [CustomComboInfo("��������", "�滻����Ϊ��Ӧ������", SAM.JobID, SAM.Mangetsu)]
        SamuraiMangetsuCombo = 14,

        [CustomComboInfo("ӣ������", "�滻ӣ��Ϊ��Ӧ������", SAM.JobID, SAM.Oka)]
        SamuraiOkaCombo = 15,

        [CustomComboInfo("���� - ����", "��û�д���ʱ���滻����Ϊ����", SAM.JobID, SAM.Seigan)]
        SamuraiThirdEyeFeature = 51,

        [CustomComboInfo("���/ʿ��״̬", "������/ʿ���ĸ���Ҫʱ�滻����ֹˮ.", SAM.JobID, SAM.MeikyoShisui)]
        SamuraiJinpuShifuFeature = 72,

        [CustomComboInfo("����״̬", "����ѹ��ʱ�����滻�Ӻ���/��ط�.", SAM.JobID, SAM.Iaijutsu, SAM.Tsubame)]
        SamuraiShohaFeature = 74,

        [CustomComboInfo("��ط�״̬", "���������ʱ����ط��滻�Ӻ���.", SAM.JobID, SAM.Tsubame)]
        SamuraiTsubameFeature = 91,

        #endregion
        // ====================================================================================
        #region NINJA

        [CustomComboInfo("ǿ���Ƶ�ͻ����", "�滻ǿ���Ƶ�ͻΪ��Ӧ������", NIN.JobID, NIN.ArmorCrush)]
        NinjaArmorCrushCombo = 17,

        [CustomComboInfo("����������", "�滻������Ϊ��Ӧ������", NIN.JobID, NIN.AeolianEdge)]
        NinjaAeolianEdgeCombo = 18,

        [CustomComboInfo("��������ɱ����", "�滻��������ɱΪ��Ӧ������", NIN.JobID, NIN.HakkeMujinsatsu)]
        NinjaHakkeMujinsatsuCombo = 19,

        [CustomComboInfo("�λ����� - �Ͼ�", "�����ڶϾ�Ԥ��״̬�£��滻�λ�����Ϊ�Ͼ�", NIN.JobID, NIN.DreamWithinADream)]
        NinjaAssassinateFeature = 45,

        [CustomComboInfo("��ɱ��ᱳ���滻", "����ʱ�滻��ɱ���.", NIN.JobID, NIN.Kassatsu)]
        NinjaKassatsuTrickFeature = 81,

        [CustomComboInfo("������滻��ˮ", "����������ˮ�滻�����.", NIN.JobID, NIN.TenChiJin)]
        NinjaTCJMeisuiFeature = 82,

        [CustomComboInfo("��ɱʱ���ӡ", "��������ɱ���buffʱ��ѵ�֮ӡ�滻����֮ӡ.", NIN.JobID, NIN.Chi)]
        NinjaKassatsuChiJinFeature = 89,

        [CustomComboInfo("��ȡ�滻����", "����ս��ʱ��ȡ�滻����.", NIN.JobID, NIN.Hide)]
        NinjaHideMugFeature = 90,

        [CustomComboInfo("AOE��״̬", "����ڽ�ӡ�����滻AOE����.", NIN.JobID, NIN.AeolianEdge)]
        NinjaNinjutsuFeature = 97,

        [CustomComboInfo("GCD��״̬", "����ڽ�ӡ�����滻GCD����", NIN.JobID, NIN.AeolianEdge, NIN.ArmorCrush, NIN.HakkeMujinsatsu)]
        NinjaGCDNinjutsuFeature = 98,

        [CustomComboInfo("����", "������CDʱ���滻����", NIN.JobID, NIN.LiuDao)]
        NinjaFenShenFeature = 106,

        [CustomComboInfo("����״̬", "�����������40�����滻��ȡ�ʹ���50�����滻��ˮ", NIN.JobID, NIN.Mug,NIN.Meisui)]
        NinjaLiangPuFeature = 108,

        [CustomComboInfo("�Զ�����", "ʹ����֮ӡ���Զ�����", NIN.JobID, NIN.Tian)]
        NinjaRenShuFeature = 117,
        #endregion
        // ====================================================================================
        #region GUNBREAKER

        [CustomComboInfo("Ѹ��ն����", "�滻Ѹ��նΪ��Ӧ������", GNB.JobID, GNB.SolidBarrel)]
        GunbreakerSolidBarrelCombo = 20,

        [CustomComboInfo("����צ����", "�滻����צΪ��Ӧ������", GNB.JobID, GNB.WickedTalon)]
        GunbreakerGnashingFangCombo = 21,

        [CustomComboInfo("����צ - ����", "��������צ����, �滻����צΪ����", GNB.JobID, GNB.WickedTalon)]
        GunbreakerGnashingFangCont = 52,

        [CustomComboInfo("��ħɱ����", "�滻��ħɱΪ��Ӧ������", GNB.JobID, GNB.DemonSlaughter)]
        GunbreakerDemonSlaughterCombo = 22,

        [CustomComboInfo("����״̬", "���ӵ�����ʱ���Զ��滻�ڵ����AOE����", GNB.JobID, GNB.DemonSlaughter)]
        GunbreakerGaugeOvercapFeature = 30,

        [CustomComboInfo("Ѫ��״̬", "���㾧�����ʱ�ñ������滻Ѫ��.", GNB.JobID, GNB.BurstStrike)]
        GunbreakerBloodfestOvercapFeature = 70,

        [CustomComboInfo("����״̬", "�ù��γ岨�滻����, Ȼ�����������滻, �����鱻����ʱ.", GNB.JobID, GNB.NoMercy)]
        GunbreakerNoMercyFeature = 66,

        [CustomComboInfo("һ������", "ǹ��һ��gcd��.", GNB.JobID, GNB.NoMercy)]
        GunbreakerZiDongeature = 114,

        #endregion
        // ====================================================================================
        #region MACHINIST

        [CustomComboInfo("�ѻ�������", "�滻�ѻ���Ϊ��Ӧ������", MCH.JobID, MCH.CleanShot, MCH.HeatedCleanShot)]
        MachinistMainCombo = 23,

        [CustomComboInfo("ɢ��(����)", "�ڹ���״̬�£��滻ɢ��Ϊ�Զ���", MCH.JobID, MCH.SpreadShot)]
        MachinistSpreadShotFeature = 24,

        [CustomComboInfo("�ȳ��(����)", "�ڹ���״̬�£��滻����Ϊ�ȳ��", MCH.JobID, MCH.HeatBlast, MCH.AutoCrossbow)]
        MachinistOverheatFeature = 47,

        [CustomComboInfo("������״̬", "�������˱�����ʱ���ó���滻��ʽ���������ͺ�ʽ������ż", MCH.JobID, MCH.RookAutoturret, MCH.AutomatonQueen)]
        MachinistOverdriveFeature = 58,

        [CustomComboInfo("������-����״̬", "��������ж�����ܵ����滻������.", MCH.JobID, MCH.GaussRound)]
        MachinistGaussRicochetFeature = 95,

        [CustomComboInfo("����һ��", "�����ѻ����������һ��gcd����", MCH.JobID, MCH.CleanShot, MCH.HeatedCleanShot)]
        MachinistZiDongFeature = 113,
        #endregion
        // ====================================================================================
        #region BLACK MAGE

        [CustomComboInfo("����-����/����", "������Ӧ״̬���滻����Ϊ����/����", BLM.JobID, BLM.Enochian)]
        BlackEnochianFeature = 25,

        [CustomComboInfo("�鼫��/������λ", "���鼫�����ʱ���滻������λΪ�鼫��", BLM.JobID, BLM.Transpose)]
        BlackManaFeature = 26,

        [CustomComboInfo("ħ�Ʋ�/��ħ��", "����ħ�Ƽ���ʱ���滻��ħ��Ϊħ�Ʋ�", BLM.JobID, BLM.LeyLines)]
        BlackLeyLines = 56,

        [CustomComboInfo("����״̬", "���͵���2400MPʱ�������滻��4.\n����״̬���뱻����.", BLM.JobID, BLM.Enochian)]
        BlackDespairFeature = 77,

        [CustomComboInfo("��״̬", "������buff׼����������buff������1/3����ʧʱ����1/3�滻����/��4/��4, ���費���жϼ�ʱ��.\n����״̬���뱻����.", BLM.JobID, BLM.Enochian)]
        BlackThunderFeature = 86,

        [CustomComboInfo("��1/3״̬", "����Ǽ���ʣ6����ǻ�1�������3����ǻ�3.\n����״̬���뱻����.", BLM.JobID, BLM.Enochian)]
        BlackFireFeature = 76,

        [CustomComboInfo("��3-��1״̬", "�����Ǽ���ʱ�����buff׼����ʱ��1�滻��3.\n��Ȼ�������Ǽ���ʱ��4ǰ��1/3�滻������ (���������û�ж�ʧ).", BLM.JobID, BLM.Enochian, BLM.Fire)]
        BlackFire3Feature = 87,

        [CustomComboInfo("������״̬", "���鼫��ʱ���3�滻��1, ͬ�����ڱ�2.", BLM.JobID, BLM.Blizzard, BLM.Freeze)]
        BlackBlizzardFeature = 88,

        #endregion
        // ====================================================================================
        #region ASTROLOGIAN

        [CustomComboInfo("�鿨/����", "û�п������ʱ���滻����Ϊ�鿨", AST.JobID, AST.Play)]
        AstrologianCardsOnDrawFeature = 27,

        [CustomComboInfo("�ȼ�ͬ�������滻����", "�ȼ�ͬ��ʱ�����滻����.", AST.JobID, AST.Benefic2)]
        AstrologianBeneficFeature = 62,

        [CustomComboInfo("С���ؿ� - ���ڳ鿨", "��û�г鿨ʱ���ڳ鿨�滻С���ؿ�.", AST.JobID, AST.MinorArcana)]
        AstrologianSleeveDrawFeature = 75,

        [CustomComboInfo("dot״̬", "��Ŀ��dotʱ��С��3��ʱ��dot�滻ɷ��/n����ɷ��״̬��״̬һ��ʹ��", AST.JobID,AST.����,AST.����,AST.ɷ��,AST.���� )]
        ASTdotFeature = 103,

        [CustomComboInfo("ɷ��״̬", "��Ŀ������dot����3sʱ��dot�滻111/n������dot״̬һ��ʹ��", AST.JobID, AST.����, AST.����, AST.����)]
        AST111Feature = 110,

        [CustomComboInfo("ѧ��״̬", "���Լ���ʲôѧ��ʱ���滻����һ��ѧ��,�ڷ�ս��״̬�м������滻ռ��", AST.JobID, AST.����ѧ��,AST.��ҹѧ��,AST.ռ��)]
        ASTSectFeature = 111,

        [CustomComboInfo("����״̬", "������������Ӧdot����5sʱ���滻������", AST.JobID, AST.������λ_��)]
        ASTYangXingFeature = 119,
        #endregion
        // ====================================================================================
        #region SUMMONER

        [CustomComboInfo("�������ٻ�����", "����������, �����ٻ�, ��������Ϊһ������.\n�������Ǻ˱�, ����ŷ�, ������ŷ�Ϊһ������", SMN.JobID, SMN.DreadwyrmTrance, SMN.Deathflare)]
        SummonerDemiCombo = 28,

        [CustomComboInfo("��Ȫ����", "�ڴ�����Ȫ״̬�£��滻��Ȫ֮��Ϊ����֮��", SMN.JobID, SMN.Ruin1, SMN.Ruin3)]
        SummonerBoPCombo = 38,

        [CustomComboInfo("��������-���ñ���", "̫����δ��������ʱ���滻���ñ���Ϊ��������", SMN.JobID, SMN.Fester)]
        SummonerEDFesterCombo = 39,

        [CustomComboInfo("������ȡ-ʹ��˱�", "��̫����δ��������ʱ���滻ʹ��˱�Ϊ������ȡ", SMN.JobID, SMN.Painflare)]
        SummonerESPainflareCombo = 40,

        [CustomComboInfo("�鹥1/2 -�پ�4״̬", "���پ�4״̬��ʱ�پ�4�滻�鹥1/2.", SMN.JobID, SMN.EgiAssault, SMN.EgiAssault2)]
        SummonerRuinIVFeature = 92,

        [CustomComboInfo("�������ٻ����ϼ�ǿ", "������, �����ٻ�, ��������, ���Ǻ˱�, ����ŷ�, ������ŷ�Ϊһ������.\n��Ҫǰ�����Ͽ���.", SMN.JobID, SMN.DreadwyrmTrance)]
        SummonerDemiComboUltra = 93,

        [CustomComboInfo("�鹦״̬", "�滻�鹥1ʹ�鹥1���鹥2CD���.", SMN.JobID, SMN.EgiAssault)]
        SummonerLingGongFeature = 121,

        #endregion
        // ====================================================================================
        #region SCHOLAR

        [CustomComboInfo("��������/ο��", "������ʹ���ٻ�ʱ���滻��������Ϊο��", SCH.JobID, SCH.FeyBless)]
        ScholarSeraphConsolationFeature = 29,

        [CustomComboInfo("�������� - ��̫����", "�㵵��̫����ʱ,�滻��������Ϊ��̫����", SCH.JobID, SCH.EnergyDrain)]
        ScholarEnergyDrainFeature = 37,

        [CustomComboInfo("dot״̬", "��Ŀ��dotʱ��С��3��ʱ��dot�滻�׷�", SCH.JobID, SCH.����,SCH.���׷�,SCH.ħ�׷�)]
        SCHDotFeature = 101,

        [CustomComboInfo("Ӧ��״̬", "��������Ӧ��������CDʱ���滻��Ⱥ��", SCH.JobID, SCH.Ӧ��ս��)]
        SCHYingJitFeature = 120,

        #endregion
        // ====================================================================================
        #region DANCER

        [CustomComboInfo("��������", "�����衤��Ԥ��ʱ���滻���衤������衤��Ϊ���衤��", DNC.JobID, DNC.FanDance1, DNC.FanDance2)]
        DancerFanDanceCombo = 33,

        [CustomComboInfo("��������", "�Զ�����", DNC.JobID, DNC.StandardStep, DNC.TechnicalStep)]
        DancerDanceStepCombo = 31,

        [CustomComboInfo("�ٻ����޽��̱���", "��ʹ�ú����д��������滻�ɰٻ����޳�", DNC.JobID, DNC.Flourish)]
        DancerFlourishFeature = 34,

        [CustomComboInfo("��������", "��������", DNC.JobID, DNC.Cascade)]
        DancerSingleTargetMultibutton = 43,

        [CustomComboInfo("AOE����", "AOE����", DNC.JobID, DNC.Windmill)]
        DancerAoeMultibutton = 50,

        [CustomComboInfo("�貽״̬", "������ʱ��Ѽ����滻���貽." +
            "\n��������ȷ������Ȼ����ʹ���������У�����ʹ���Զ��赸." +
            "\n������ͨ��Ϊÿ���貽��������ļ���id��������Ӧ���貽." +
            "\nĬ�ϵ�����к���ٻ����ޣ����衤�����衤�ƣ��������Ϊ0�������ó�����." +
            "\n������Garland Tools(http://www.garlandtools.org/)ͨ�����������Ӧ������Ӧ����id.", DNC.JobID, DNC.Cascade, DNC.Flourish, DNC.FanDance1, DNC.FanDance2)]
        DancerDanceComboCompatibility = 64,

        #endregion
        // ====================================================================================
        #region WHITE MAGE

        [CustomComboInfo("��ο/����֮��", "������֮�Ŀ���ʹ��ʱ���滻��ο֮��Ϊ����֮��", WHM.JobID, WHM.AfflatusSolace)]
        WhiteMageSolaceMiseryFeature = 35,

        [CustomComboInfo("��ϲ/����֮��", "������֮�Ŀ���ʹ��ʱ���滻��ϲ֮��Ϊ����֮��", WHM.JobID, WHM.AfflatusRapture)]
        WhiteMageRaptureMiseryFeature = 36,

        [CustomComboInfo("�ȼ�ͬ��ʱ���� -����", "�ȼ�ͬ��ʱ�����滻�ɾ���.", WHM.JobID, WHM.Cure2)]
        WhiteMageCureFeature = 60,

        [CustomComboInfo("�ٺ�״̬", "���ݻ���ʱ��ο֮���滻���ƣ���ϲ�滻ҽ��", WHM.JobID, WHM.Cure2, WHM.Medica)]
        WhiteMageAfflatusFeature = 61,

        [CustomComboInfo("dot״̬", "��Ŀ��dotʱ��С��3��ʱ��dot�滻��ҫ", WHM.JobID, WHM.Stone,WHM.Glare,WHM.StoneFour,WHM.StoneThree,WHM.StoneTwo)]
        WhiteStoneFeature = 100,


        [CustomComboInfo("ҽ��״̬", "������ҽ��dotʱ�����5��ʱ��ҽ���滻ҽ��", WHM.JobID, WHM.ҽ��)]
        WhiteYiJiFeature = 118,


        #endregion
        // ====================================================================================
        #region BARD

        [CustomComboInfo("������ - ��������", "�����ڷ������С������״̬�£��滻�������С������Ϊ��������", BRD.JobID, BRD.WanderersMinuet)]
        BardWandererPPFeature = 41,

        [CustomComboInfo("ǿ����� - ֱ�����", "������ʱ���滻ǿ�����/�������Ϊֱ�����/�Իͼ�", BRD.JobID, BRD.HeavyShot, BRD.BurstShot)]
        BardStraightShotUpgradeFeature = 42,

        [CustomComboInfo("����dot", "��2����û��ʧ��ʱ���Ҷ�ҧ��/���ʴ���滻�������ݡ�/n������ͬ�����������������ݲ���ʹ���������汾������.", BRD.JobID, BRD.IronJaws)]
        BardIronJawsFeature = 84,

        [CustomComboInfo("�������/����� - �����", "��������ʱ������滻�������/�����.", BRD.JobID, BRD.BurstShot, BRD.QuickNock)]
        BardApexFeature = 85,

        #endregion
        // ====================================================================================
        #region MONK

        [CustomComboInfo("AoE����", "�滻���Ҿ�Ϊ��Ӧ��AoE����,����ſ���ʱ���滻Ϊ���Ҿ�", MNK.JobID, MNK.Rockbreaker)]
        MnkAoECombo = 54,

        [CustomComboInfo("��ɮ����״̬", "�����滻˫�������������buff.", MNK.JobID, MNK.DragonKick)]
        MnkBootshineFeature = 65,

        [CustomComboInfo("��ɮ����ȭ״̬", "����ȭ�滻��ȭ�������ȭ�������ʣ�����ʱ�䳬��6s.", MNK.JobID, MNK.SnapPunch)]
        MnkDemolishFeature = 83,

        [CustomComboInfo("��ռ���״̬", "�汱�滻��ռ���ʹ����CD���.", MNK.JobID, MNK.��ռ���)]
        MnkShenWeiFeature =115,

        [CustomComboInfo("���״̬", "�����ʱ���˫��-������û��buff��buff.", MNK.JobID, MNK.DragonKick,MNK.Bootshine,MNK.˫��,MNK.��ȭ,MNK.SnapPunch,MNK.Demolish)]
        MnkZhenJiaoFeature = 116,

        #endregion
        // ====================================================================================
        #region RED MAGE

        [CustomComboInfo("AoE����", "������ӽ��/����ӽ������ʱ���滻���ҷ�/������Ϊ���", RDM.JobID, RDM.Veraero2, RDM.Verthunder2)]
        RedMageAoECombo = 48,

        [CustomComboInfo("ħ��������", "�滻ħ����Ϊ��Ӧ������", RDM.JobID, RDM.Redoublement)]
        RedMageMeleeCombo = 49,

        [CustomComboInfo("ħ����Plus", "���ӳ���ʥ/��˱�/������ħ����������ȡ���ڽ��̺�����. \nҪ��ħ��������", RDM.JobID, RDM.Redoublement)]
        RedMageMeleeComboPlus = 78,

        [CustomComboInfo("���/ʯ - ��", "û�д���ʱ���滻���ʯ/�����Ϊ��/����.", RDM.JobID, RDM.Verstone, RDM.Verfire)]
        RedMageVerprocCombo = 53,

        [CustomComboInfo("���/ʯ - �� Plus", "�����������ӽ��/���̷������ʯ/������滻�༲��/������\nҪ����/ʯ - ��.", RDM.JobID, RDM.Verstone, RDM.Verfire)]
        RedMageVerprocComboPlus= 79,

        [CustomComboInfo("���/ʯ - �� Plus ��������", "���˳�ս���ó༲���滻�����.\nҪ����/ʯ - �� Plus.", RDM.JobID, RDM.Verfire)]
        RedMageVerprocOpenerFeature = 80,

        [CustomComboInfo("��״̬", "����Ӧbuff��ʹ����Ӧ����ʹħԪ���.", RDM.JobID, RDM.Jolt,RDM.Jolt2)]
        RedMageZiDONGFeature = 112,

        #endregion
        // ====================================================================================
        #region DISCIPLE OF MAGIC

        [CustomComboInfo("���м���BUFF", "������û��ȴʱ��(����û������ӽ��buff)�ֱ��滻��ħ/�ٻ�/ѧ��/��ħ/ռ�ǵĸ���.", DoM.JobID, WHM.Raise, ACN.Resurrection, AST.Ascend, RDM.Verraise)]
        DoMSwiftcastFeature = 94,

        #endregion
        // ====================================================================================
    }

    internal class SecretCustomComboAttribute : Attribute { }

    internal class CustomComboInfoAttribute : Attribute
    {
        internal CustomComboInfoAttribute(string fancyName, string description, byte jobID, params uint[] abilities)
        {
            FancyName = fancyName;
            Description = description;
            JobID = jobID;
            Abilities = abilities;
        }

        public string FancyName { get; }
        public string Description { get; }
        public byte JobID { get; }
        public string JobName => JobIDToName(JobID);
        public uint[] Abilities { get; }

        private static string JobIDToName(byte key)
        {
            return key switch
            {
                1 => "����ʦ",
                2 => "�񶷼�",
                3 => "����ʦ",
                4 => "ǹ��ʦ",
                5 => "������",
                6 => "����ʦ",
                7 => "����ʦ",
                8 => "��ľ��",
                9 => "������",
                10 => "���׽�",
                11 => "���",
                12 => "�Ƹｳ",
                13 => "���½�",
                14 => "������ʿ",
                15 => "���ʦ",
                16 => "�ɿ�",
                17 => "԰�չ�",
                18 => "������",
                19 => "��ʿ",
                20 => "��ɮ",
                21 => "սʿ",
                22 => "����ʿ",
                23 => "ʫ��",
                24 => "��ħ��ʦ",
                25 => "��ħ��ʦ",
                26 => "����ʦ",
                27 => "�ٻ�ʦ",
                28 => "ѧ��",
                29 => "˫��ʦ",
                30 => "����",
                31 => "����ʿ",
                32 => "������ʿ",
                33 => "ռ����ʿ",
                34 => "��ʿ",
                35 => "��ħ��ʦ",
                36 => "��ħ��ʦ",
                37 => "��ǹսʿ",
                38 => "����",
                99 => "ħ����ʦ",
                _ => "Unknown",
            };
        }
    }
}
