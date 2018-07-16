using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public static class Config
    {
        public const int FieldWidth = 4;
        public const int FieldHeight = 4;
        public const int CellViewSpacing = 28;
        public const float MovingTime = 0.15f;
        public const float SpawningTime = 0.2f;
        public const float MultiplicationTime = 0.3f;
        public const float MultiplyAnimationScaleMultiplier = 2f;
        public const float FadeTime = 0.3f;
        public const float PaddingX = 135.5f;
        public const float PaddingY = 135.5f;
        public const float MinSwipeLenght = 0.5f;
    }
}




//TODO restore iddle state when no movement