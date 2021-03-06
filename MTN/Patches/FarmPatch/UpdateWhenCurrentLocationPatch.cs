﻿using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using XNARectangle = Microsoft.Xna.Framework.Rectangle;

namespace MTN.Patches.FarmPatch {
    public class UpdateWhenCurrentLocationPatch {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            int i;
            var codes = new List<CodeInstruction>(instructions);
            for (i = 23; i < 107; i++) codes[i].opcode = OpCodes.Nop;
            codes[213].operand = -1;
            return codes.AsEnumerable();
        }

        public static void Postfix(Farm __instance) {
            if (__instance.Name != "Farm") return;
            int chimneyTimer = (int)Traverse.Create(__instance).Field("chimneyTimer").GetValue();
            if ((bool)Traverse.Create(__instance).Field("wasUpdated").GetValue() && Game1.gameMode != 0 && chimneyTimer != -1) {
                return;
            }

            if (chimneyTimer <= 0) {
                FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(Game1.MasterPlayer);
                if (homeOfFarmer != null && homeOfFarmer.hasActiveFireplace()) {
                    if (Memory.isCustomFarmLoaded) {
                        __instance.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new XNARectangle(372, 1956, 10, 10), new Vector2((float)((Memory.loadedFarm.farmHousePorchX() + 2) * 64 + 4 * ((Game1.MasterPlayer.houseUpgradeLevel >= 2) ? 9 : -5)), (float)(Memory.loadedFarm.farmHousePorchY() * 64 - 420)), false, 0.002f, Color.Gray) {
                            alpha = 0.75f,
                            motion = new Vector2(0f, -0.5f),
                            acceleration = new Vector2(0.002f, 0f),
                            interval = 99999f,
                            layerDepth = 1f,
                            scale = 2f,
                            scaleChange = 0.02f,
                            rotationChange = (float)Game1.random.Next(-5, 6) * 3.14159274f / 256f
                        });
                    } else {
                        Point porchStandingSpot = homeOfFarmer.getPorchStandingSpot();
                        __instance.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new XNARectangle(372, 1956, 10, 10), new Vector2((float)(porchStandingSpot.X * 64 + 4 * ((Game1.MasterPlayer.houseUpgradeLevel >= 2) ? 9 : -5)), (float)(porchStandingSpot.Y * 64 - 420)), false, 0.002f, Color.Gray) {
                            alpha = 0.75f,
                            motion = new Vector2(0f, -0.5f),
                            acceleration = new Vector2(0.002f, 0f),
                            interval = 99999f,
                            layerDepth = 1f,
                            scale = 2f,
                            scaleChange = 0.02f,
                            rotationChange = (float)Game1.random.Next(-5, 6) * 3.14159274f / 256f
                        });
                    }
                }
            }
            Traverse.Create(__instance).Field("chimneyTimer").SetValue(500);
            return;
        }
    }
}
