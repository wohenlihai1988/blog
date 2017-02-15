using System;

// the same with excel datas
namespace ExcelClass{
	[Serializable]
	public class SectionCfg{
		public string sectionId;
		public string sectionPoint;
		public string sectionSize;
		public string spawnerId;
		public string eventId;
	}
	[Serializable]
	public class ConfigCfg{
		public string startPoint;
		public string endPoint;
		public string cameraArg;
		public string evnetId;
		public string playType;
	}
	[Serializable]
	public class EventCfg{
		public string eventId;
		public string type;
		public string data1;
		public string data2;
		public string data3;
	}
	[Serializable]
	public class SpawnerCfg{
		public string spawnerId;
		public string monsterId;
		public string spawnerType;
		public string monsterGroup;
		public string spawnerPriority;
		public string spawnerPoint;
		public string tiggerType;
		public string data1;
		public string data2;
		public string isFinalOne;
		public string aiType;
		public string appearName;
		public string specialCam;
		public string specialCamAnim;
	}

	[Serializable]
	public class StageCfg{
		public string id;
		public string scenePath;
		public string stagePath;
	}
}
