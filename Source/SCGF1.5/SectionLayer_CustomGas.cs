using PerformanceFish;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace SCGF
{
    [StaticConstructorOnStartup]
    public class SectionLayer_CustomGas : SectionLayer_Gas
    {
        private static readonly Color ColorForShader = new Color(1f, 0f, 0f, 0f);

        private static readonly Dictionary<Material, HashSet<GasDef>> gasMaterialDict = new Dictionary<Material, HashSet<GasDef>>();

        static SectionLayer_CustomGas()
        {
            foreach (GasDef gasDef in DefDatabase<GasDef>.AllDefsListForReading)
            {
                Material material = gasDef.MakeMaterial();

                if (gasMaterialDict.TryGetValue(material, out HashSet<GasDef> gasDefs))
                {
                    gasDefs.Add(gasDef);
                }
                else
                {
                    gasMaterialDict.Add(material, new HashSet<GasDef>() { gasDef });
                }
            }

            int gasDefCount = DefDatabase<GasDef>.DefCount;

            Verse.Log.Message($"[Simple Custom Gas Framework] {gasDefCount} Custom Gas{(gasDefCount == 1 ? "" : "es")} Loaded");
        }

        public SectionLayer_CustomGas(Section section) : base(section)
        {
            //
        }

        public override void Regenerate()
        {
            ClearSubMeshes(MeshParts.All);

            float altitude = AltitudeLayer.Gas.AltitudeFor();

            int num = section.botLeft.x;

            GasGrid gasGrid = Map.gasGrid;

            foreach (KeyValuePair<Material, HashSet<GasDef>> entry in gasMaterialDict)
            {
                Material material = entry.Key;
                HashSet<GasDef> gasDefs = entry.Value;

                LayerSubMesh subMesh = GetSubMesh(material);

                foreach (IntVec3 cell in section.CellRect)
                {
                    if (gasDefs.Any(gasDef => gasGrid.DensityAt(cell, gasDef) > 0))
                    {
                        AddCell(cell, num, subMesh.verts.Count, subMesh, altitude);
                    }

                    num++;
                }

                if (subMesh.verts.Count > 0)
                {
                    subMesh.FinalizeMesh(MeshParts.All);
                }
            }
        }

        public override Color ColorAt(IntVec3 cell) => ColorForShader;
    }
}