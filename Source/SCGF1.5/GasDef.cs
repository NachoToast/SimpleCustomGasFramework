using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SCGF
{
    /// <summary>
    /// Properties of a custom gas.
    /// </summary>
    public class GasDef : PerformanceFish.GasDef
    {
        private readonly List<GasFilter> appliesTo = new List<GasFilter>();

        private readonly List<GasFilter> immunityWhen = new List<GasFilter>();

        private readonly List<GasAction> actions = new List<GasAction>();

        private readonly List<GasAction> immuneActions = new List<GasAction>();

        private readonly string texPath = "Things/Gas/GasCloudThickA";

        private readonly string shaderPath = "Map/GasRotating";

        private readonly bool allowNegativeDissipationRateIAcceptTheConsequences = false;

        [Unsaved]
        private Color actualColor;

        public virtual Material MakeMaterial()
        {
            return MaterialPool.MatFrom(
                srcTex: ContentFinder<Texture2D>.Get(texPath),
                shader: ShaderDatabase.LoadShader(shaderPath),
                color: actualColor,
                renderQueue: 3000);
        }

        public virtual void DoEffects(Pawn pawn, byte gasDensity)
        {
            if (!appliesTo.AllFiltersPass(pawn, gasDensity))
            {
                return;
            }

            IEnumerable<GasAction> actionsToRun;

            if (immunityWhen.AnyFiltersPass(pawn, gasDensity))
            {
                actionsToRun = immuneActions;
            }
            else
            {
                actionsToRun = actions;
            }

            foreach (GasAction action in actionsToRun)
            {
                action.DoEffects(pawn, gasDensity);
            }
        }

        public override void ResolveReferences()
        {
            base.ResolveReferences();

            // "Prevent" this gas from being drawn in SectionLayer_Gas
            actualColor = color;
            color = Color.clear;

            foreach (GasFilter filter in appliesTo)
            {
                filter.ResolveReferences();
            }

            foreach (GasFilter filter in immunityWhen)
            {
                filter.ResolveReferences();
            }

            foreach (GasAction action in actions)
            {
                action.ResolveReferences();
            }

            foreach (GasAction action in immuneActions)
            {
                action.ResolveReferences();
            }
        }

        public override void PostLoad()
        {
            base.PostLoad();

            foreach (GasFilter filter in appliesTo)
            {
                filter.PostLoad();
            }

            foreach (GasFilter filter in immunityWhen)
            {
                filter.PostLoad();
            }

            foreach (GasAction action in actions)
            {
                action.PostLoad();
            }

            foreach (GasAction action in immuneActions)
            {
                action.PostLoad();
            }
        }

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string error in base.ConfigErrors())
            {
                yield return error;
            }

            if (string.IsNullOrEmpty(label))
            {
                yield return $"{nameof(label)} cannot be null or empty";
            }

            if (string.IsNullOrEmpty(texPath))
            {
                yield return $"{nameof(texPath)} cannot be null or empty";
            }

            if (string.IsNullOrEmpty(shaderPath))
            {
                yield return $"{nameof(shaderPath)} cannot be null or empty";
            }

            if (dissipationRate < 0)
            {
                if (!allowNegativeDissipationRateIAcceptTheConsequences)
                {
                    yield return $"{nameof(dissipationRate)} should not be less than 0 (got {dissipationRate}), this will lead to lots of lag as the gas fills up the whole map. If, for some reason, you are fine with playing RimWorld in slideshow mode, you can suppress this warning by setting {nameof(allowNegativeDissipationRateIAcceptTheConsequences)} to true";
                }
            }
            else
            {
                if (dissipationRate > 255)
                {
                    yield return $"{nameof(dissipationRate)} cannot be > 255 (got {dissipationRate})";
                }

                if (allowNegativeDissipationRateIAcceptTheConsequences)
                {
                    yield return $"{nameof(allowNegativeDissipationRateIAcceptTheConsequences)} is true but {nameof(dissipationRate)} is not negative (got {dissipationRate})";
                }
            }

            if (color == null)
            {
                yield return $"{nameof(color)} cannot be null";
            }

            for (int i = 0; i < appliesTo.Count; i++)
            {
                foreach (string error in appliesTo[i].ConfigErrors())
                {
                    yield return $"{nameof(appliesTo)}[{i}]: {error}";
                }
            }

            for (int i = 0; i < immunityWhen.Count; i++)
            {
                foreach (string error in immunityWhen[i].ConfigErrors())
                {
                    yield return $"{nameof(immunityWhen)}[{i}]: ${error}";
                }
            }

            for (int i = 0; i < actions.Count; i++)
            {
                foreach (string error in actions[i].ConfigErrors())
                {
                    yield return $"{nameof(actions)}[{i}]: {error}";
                }
            }

            for (int i = 0; i < immuneActions.Count; i++)
            {
                foreach (string error in immuneActions[i].ConfigErrors())
                {
                    yield return $"{nameof(immuneActions)}[{i}]: {error}";
                }
            }
        }
    }
}