    using UnityEngine;

    public class HexMetrics
    {
        public const float outerToInner = 0.866025404f;

        public const float innerToOuter = 1f / outerToInner;

        public const float outerRadius = 5f;

        public const float innerRadius = outerRadius * outerToInner;

    }
