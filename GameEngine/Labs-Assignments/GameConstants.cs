using System;
using System.Collections.Generic;
using System.Text;

namespace CPI311.GameEngine
{
    public static class GameConstants //: GameObject 
    {
        public const float CameraHeight = 8000.0f;
        public const int NumBullets = 30;
        public const int NumAsteroids = 10;
        public const float BulletSpeedAdjustment = 10000f;
        public const int ShotPenalty = 10;
        public const int DeathPenalty = 100;
        public const int KillBonus = 100;
        public const float PlayfieldSizeX = 1920f * 5.5f;
        public const float PlayfieldSizeY = 1080f * 7.5f;
        public const float AsteroidMinSpeed = 2000.0f;
        public const float AsteroidMaxSpeed = 5000.0f;
        public const float playerSpawnX = 1920f;
        public const float playerSpawnY = 1080f;
        public const float shipSpeed = 5000f;
    }
}
