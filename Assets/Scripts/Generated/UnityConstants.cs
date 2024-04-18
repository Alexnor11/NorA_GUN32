using System;
using System.Collections.Generic;

namespace Tanks
{
	public static partial class UnityConstants
	{
	    [Flags]
		public enum LayerEnum
		{
			Default = 1 << 0,
			TransparentFX = 1 << 1,
			IgnoreRaycast = 1 << 2,
			Water = 1 << 4,
			UI = 1 << 5,
			Projectiles = 1 << 6,
			Obstacles = 1 << 7,
			Ground = 1 << 8,
			Players = 1 << 9,
			PostProcessing = 1 << 10,
		}

		public const int DefaultLayerInt = 0;
		public const int TransparentFXLayerInt = 1;
		public const int IgnoreRaycastLayerInt = 2;
		public const int WaterLayerInt = 4;
		public const int UILayerInt = 5;
		public const int ProjectilesLayerInt = 6;
		public const int ObstaclesLayerInt = 7;
		public const int GroundLayerInt = 8;
		public const int PlayersLayerInt = 9;
		public const int PostProcessingLayerInt = 10;

		public const string DefaultLayerName = "Default";
		public const string TransparentFXLayerName = "TransparentFX";
		public const string IgnoreRaycastLayerName = "IgnoreRaycast";
		public const string WaterLayerName = "Water";
		public const string UILayerName = "UI";
		public const string ProjectilesLayerName = "Projectiles";
		public const string ObstaclesLayerName = "Obstacles";
		public const string GroundLayerName = "Ground";
		public const string PlayersLayerName = "Players";
		public const string PostProcessingLayerName = "PostProcessing";

        public static Dictionary<TagEnum, string> Tags = new Dictionary<TagEnum, string>()
        {
        	{TagEnum.Untagged, UntaggedTagName},
        	{TagEnum.Respawn, RespawnTagName},
        	{TagEnum.Finish, FinishTagName},
        	{TagEnum.EditorOnly, EditorOnlyTagName},
        	{TagEnum.MainCamera, MainCameraTagName},
        	{TagEnum.Player, PlayerTagName},
        	{TagEnum.GameController, GameControllerTagName},
        };

		public enum TagEnum
		{
			Untagged = 0,
			Respawn = 1,
			Finish = 2,
			EditorOnly = 3,
			MainCamera = 4,
			Player = 5,
			GameController = 6,
		}

		public const string UntaggedTagName = "Untagged";
		public const string RespawnTagName = "Respawn";
		public const string FinishTagName = "Finish";
		public const string EditorOnlyTagName = "EditorOnly";
		public const string MainCameraTagName = "MainCamera";
		public const string PlayerTagName = "Player";
		public const string GameControllerTagName = "GameController";
	}
}
