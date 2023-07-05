using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace SCGF
{
    /// <summary>
    /// The primary logic of this entire framework.
    /// This class tries to copy what the vanilla gas grid does, but with added capability for any number of gases.
    /// </summary>
    public class ExtendedGasGrid : GasGrid
    {
        private readonly Map map;
               
        public byte[] customGasDensity;

        public ExtendedGasGrid(Map map) : base(map)
        {
            this.map = map;
            customGasDensity = new byte[map.cellIndices.NumGridCells * GasLibrary.numCustomGasses];
        }

        public void RecalculateEverHadGas(ref bool anyGasEverAdded)
        {
            for (int i = 0, len = customGasDensity.Length; i < len; i++)
            {
                if (customGasDensity[i] > 0)
                {
                    anyGasEverAdded = true;
                    return;
                }
            }
        }

        public bool AnyGasAt(int cellIndex)
        {
            int cellGasStartIndex = cellIndex * GasLibrary.numCustomGasses;
            int cellGasStopIndex = cellGasStartIndex + GasLibrary.numCustomGasses;

            for (int i = cellGasStartIndex; i < cellGasStopIndex; i++)
            {
                if (customGasDensity[i] > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public byte DensityAt(int cellIndex, GasDef gasDef)
        {
            return customGasDensity[cellIndex * GasLibrary.numCustomGasses + gasDef.customIndex];
        }

        public void AddGas(IntVec3 cell, GasDef gasDef, int amount, ref bool anyGasEverAdded, bool canOverflow = true)
        {
            if (amount <= 0 || !GasCanMoveTo(cell))
            {
                return;
            }

            anyGasEverAdded = true;

            int cellIndex = CellIndicesUtility.CellToIndex(cell, map.Size.x);

            byte finalDensity = AdjustedDensity(DensityAt(cellIndex, gasDef) + amount, out int overflow);

            SetDirect(cellIndex, gasDef, finalDensity);

            map.mapDrawer.MapMeshDirty(cell, MapMeshFlag.Gas);

            if (canOverflow && overflow > 0)
            {
                Overflow(cell, gasDef, overflow);
            }
        }

        private byte AdjustedDensity(int newDensity, out int overflow)
        {
            if (newDensity > 255)
            {
                overflow = newDensity - 255;

                return byte.MaxValue;
            }

            overflow = 0;

            if (newDensity < 0)
            {
                return 0;
            }

            return (byte)newDensity;
        }

        public Color ColorAt(IntVec3 cell, FloatRange AlphaRange, Color SmokeColor, Color ToxColor, Color RotColor)
        {
            // this should completely replace the parent ColorAt method, since we now do mixing with custom gas colours as well
            int cellIndex = CellIndicesUtility.CellToIndex(cell, map.Size.x);

            float[] customGasDensities = new float[GasLibrary.numCustomGasses];
            float[] vanillaGasDensities = new float[3];

            // populate custom gas densities
            for (int i = 0; i < GasLibrary.numCustomGasses; i++)
            {
                customGasDensities[i] = DensityAt(cellIndex, GasLibrary.customGassesArray[i]);
            }

            // populate vanilla gas densities
            vanillaGasDensities[0] = (int)DensityAt(cell, GasType.BlindSmoke);
            vanillaGasDensities[1] = (int)DensityAt(cell, GasType.ToxGas);
            vanillaGasDensities[2] = (int)DensityAt(cell, GasType.RotStink);

            float densitySum = customGasDensities.Sum() + vanillaGasDensities.Sum();

            Color result = new Color(0f, 0f, 0f);

            // vanilla gas colouring
            result += SmokeColor * (vanillaGasDensities[0] / densitySum);
            result += ToxColor * (vanillaGasDensities[1] / densitySum);
            result += RotColor * (vanillaGasDensities[2] / densitySum);

            // custom gas colouring
            for (int i = 0; i < GasLibrary.numCustomGasses; i++)
            {
                result += GasLibrary.customGassesArray[i].color * (customGasDensities[i] / densitySum);
            }

            // alpha calculations
            // 3 gases (255 * 3) on a single cell gives the max alpha value
            result.a = AlphaRange.LerpThroughRange(densitySum / 765f);

            return result;
        }

        public new void Notify_ThingSpawned(Thing thing)
        {
            // the parent method for this is used to remove custom gases when 'full' things are spawned on their cells (e.g. walls)
            if (!thing.Spawned || thing.def.Fillage != FillCategory.Full)
            {
                return;
            }

            foreach (IntVec3 item in thing.OccupiedRect())
            {
                if (AnyGasAt(item))
                {
                    int cellIndex = CellIndicesUtility.CellToIndex(item, map.Size.x);

                    for (int i = 0; i < GasLibrary.numCustomGasses; i++)
                    {
                        customGasDensity[cellIndex * GasLibrary.numCustomGasses + i] = 0;
                    }

                    map.mapDrawer.MapMeshDirty(item, MapMeshFlag.Gas);
                }
            }
        }

        private void SetDirect(int cellIndex, byte[] amounts)
        {
            int cellGasStartIndex = cellIndex * GasLibrary.numCustomGasses;
            for (int i = 0; i < GasLibrary.numCustomGasses; i++)
            {
                customGasDensity[cellGasStartIndex + i] = amounts[i];
            }
        }

        private void SetDirect(int cellIndex, GasDef gasDef, byte amount)
        {
            customGasDensity[cellIndex * GasLibrary.numCustomGasses + gasDef.customIndex] = amount;
        }

        private void Overflow(IntVec3 cell, GasDef gasDef, int amount)
        {
            // the parent method for this is used to add gases to surrounding cells when spawning them in (NOT for diffusion)
            if (amount <= 0)
            {
                return;
            }

            int remainingAmount = amount;

            map.floodFiller.FloodFill(cell, (IntVec3 c) => GasCanMoveTo(c), delegate (IntVec3 c)
            {
                int cellIndex = CellIndicesUtility.CellToIndex(c, map.Size.x);

                int num = Mathf.Min(remainingAmount, 255 - DensityAt(cellIndex, gasDef));
                if (num > 0)
                {
                    // since 'anyGasEverAdded' is private in the parent class, we don't have (direct) access to it
                    // and so can't pass a ref to it inside the delegate; this is the next best thing
                    bool anyGasEverAdded = true;

                    AddGas(c, gasDef, num, ref anyGasEverAdded, canOverflow: false);

                    remainingAmount -= num;
                }

                return remainingAmount <= 0;
            }, GenRadial.NumCellsInRadius(40f), rememberParents: true);
        }

        public void TryDissipateGasses(int cellIndex)
        {
            if (!AnyGasAt(cellIndex))
            {
                return;
            }

            bool dirtiedGrid = false;

            for (int i = 0; i < GasLibrary.numCustomGasses; i++)
            {
                byte currentDensity = customGasDensity[cellIndex * GasLibrary.numCustomGasses + i];
                if (currentDensity <= 0)
                {
                    continue;
                }

                GasDef gasDef = GasLibrary.customGassesArray[i];

                byte newDensity = (byte)Mathf.Max(currentDensity - gasDef.dissipationSpeed, 0);

                SetDirect(cellIndex, gasDef, newDensity);

                if (newDensity == 0)
                {
                    dirtiedGrid = true;
                }

            }

            if (dirtiedGrid)
            {
                map.mapDrawer.MapMeshDirty(CellIndicesUtility.IndexToCell(cellIndex, map.Size.x), MapMeshFlag.Gas);
            }
        }

        public void TryDiffuseGasses(IntVec3 cell, List<IntVec3> cardinalDirections)
        {
            int sourceCellIndex = CellIndicesUtility.CellToIndex(cell, map.Size.x);

            for (int i = 0; i < GasLibrary.numCustomGasses; i++)
            {
                GasDef gasDef = GasLibrary.customGassesArray[i];

                int gasDensityAtSource = DensityAt(sourceCellIndex, gasDef);
                if (gasDensityAtSource < gasDef.diffusionThreshold)
                {
                    continue;
                }

                bool dirtiedSourceCell = false;

                cardinalDirections.Shuffle();

                for (int j = 0; j < cardinalDirections.Count; j++)
                {
                    IntVec3 intVec = cell + cardinalDirections[j];

                    if (!GasCanMoveTo(intVec))
                    {
                        continue;
                    }

                    int targetCellIndex = CellIndicesUtility.CellToIndex(intVec, map.Size.x);

                    int gasDensityAtTarget = DensityAt(targetCellIndex, gasDef);

                    if (false | TryDiffuseIndividualGas(ref gasDensityAtSource, ref gasDensityAtTarget, gasDef))
                    {
                        SetDirect(targetCellIndex, gasDef, (byte)gasDensityAtTarget);

                        map.mapDrawer.MapMeshDirty(intVec, MapMeshFlag.Gas);

                        dirtiedSourceCell = true;

                        if (gasDensityAtSource < gasDef.diffusionThreshold)
                        {
                            break;
                        }
                    }
                }

                if (dirtiedSourceCell)
                {
                    SetDirect(sourceCellIndex, gasDef, (byte)gasDensityAtSource);

                    map.mapDrawer.MapMeshDirty(cell, MapMeshFlag.Gas);
                }
            }

        }

        private bool TryDiffuseIndividualGas(ref int gasDensityAtSource, ref int gasDensityAtTarget, GasDef gasDef)
        {
            // the parent method for this checks if the source has a much higher density than the target,
            // and partially equalizes (i.e. diffuses) them if so

            // the main differences here are the injection of gas def attributes such as its diffusion threshold
            if (gasDensityAtSource < gasDef.diffusionThreshold)
            {
                return false;
            }

            int densityDifference = Mathf.Abs(gasDensityAtSource - gasDensityAtTarget);

            if (gasDensityAtSource > gasDensityAtTarget && densityDifference >= gasDef.diffusionThreshold)
            {
                int lossA = densityDifference / 2;
                int gainB = lossA; // we love the laws of physics

                gasDensityAtSource -= (int)(lossA * gasDef.diffusionLossModifier);
                gasDensityAtTarget += (int)(gainB * gasDef.diffusionGainModifier);

                return true;
            }

            return false;
        }

        public new void EqualizeGasThroughBuilding(Building b, bool twoWay)
        {
            // the parent method for this handles gas diffusion logic through permeable buildings such as vents and open doors
            if (!CalculateGasEffects)
            {
                return;
            }

            IntVec3[] beqCells = new IntVec3[4];
            for (int i = 0; i < beqCells.Length; i++)
            {
                beqCells[i] = IntVec3.Invalid;
            }

            int beqCellCount = 0;

            byte[] totalCustomGasAmounts = new byte[GasLibrary.numCustomGasses];

            for (int i = 0; i < GasLibrary.numCustomGasses; i++)
            {
                totalCustomGasAmounts[i] = 0;
            }

            if (twoWay)
            {
                for (int j = 0; j < 2; j++)
                {
                    IntVec3 cell2 = ((j == 0) ? (b.Position + b.Rotation.FacingCell) : (b.Position - b.Rotation.FacingCell));
                    VisitCell(cell2);
                }
            }
            else
            {
                for (int k = 0; k < 4; k++)
                {
                    IntVec3 cell3 = b.Position + GenAdj.CardinalDirections[k];
                    VisitCell(cell3);
                }
            }

            if (beqCellCount <= 1)
            {
                return;
            }

            byte[] newAmounts = new byte[GasLibrary.numCustomGasses];
            for (int i = 0; i < GasLibrary.numCustomGasses; i++)
            {
                newAmounts[i] = (byte) Mathf.Min(totalCustomGasAmounts[i] / beqCellCount, 255);
            }

            for (int l = 0; l < beqCellCount; l++)
            {
                if (!beqCells[l].IsValid)
                {
                    continue;
                }

                byte[] newDensities = new byte[GasLibrary.numCustomGasses];

                int beqCellIndex = map.cellIndices.CellToIndex(beqCells[l]);

                for (int i = 0; i < GasLibrary.numCustomGasses; i++)
                {
                    GasDef gasDef = GasLibrary.customGassesArray[i];
                    if (gasDef.canEqualizeThroughBuildings)
                    {
                        newDensities[i] = newAmounts[i];
                    }
                    else
                    {
                        newDensities[i] = DensityAt(beqCellIndex, gasDef);
                    }
                }

                SetDirect(beqCellIndex, newDensities);

                map.mapDrawer.MapMeshDirty(beqCells[l], MapMeshFlag.Gas);
            }

            void VisitCell(IntVec3 cell)
            {
                if (cell.IsValid && GasCanMoveTo(cell))
                {
                    if (AnyGasAt(cell))
                    {
                        int cellIndex = CellIndicesUtility.CellToIndex(cell, map.Size.x);

                        for (int i = 0; i < GasLibrary.numCustomGasses; i++)
                        {
                            totalCustomGasAmounts[i] += DensityAt(cellIndex, GasLibrary.customGassesArray[i]);
                        }
                    }

                    beqCells[beqCellCount] = cell;
                    beqCellCount++;
                }
            }
        }

        public new void Debug_ClearAll()
        {
            for (int i = 0; i < customGasDensity.Length; i++)
            {
                customGasDensity[i] = 0;
            }
        }

        public new void Debug_FillAll()
        {
            for (int i = 0; i < customGasDensity.Length; i++)
            {
                if (!GasCanMoveTo(map.cellIndices.IndexToCell(i)))
                {
                    continue;
                }

                foreach(GasDef gasDef in GasLibrary.customGassesArray)
                {
                    SetDirect(i, gasDef, byte.MaxValue);
                }
            }
        }

        public new void ExposeData() 
        {
            DataExposeUtility.ByteArray(ref customGasDensity, "customGasDensity");
        }
    }
}
