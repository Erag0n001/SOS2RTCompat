using GameClient;
using GameClient.Managers;
using GameClient.Misc;
using GameClient.TCP;
using RimWorld;
using RimWorld.Planet;
using SaveOurShip2;
using Shared;
using Shared.SOS2RTCompat;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using static Shared.CommonEnumerators;
using CommonValues = Shared.SOS2RTCompat.CommonValues;

namespace GameClient.SOS2RTCompat
{
    public static class SpaceSettlementManager
    {
        public static List<WorldObjectFakeOrbitingShip> spacePlayerSettlement = new List<WorldObjectFakeOrbitingShip>();
            
        public static void ParsePacket(Packet packet) 
        {
            SpaceSettlementData spaceSettlement = Serializer.ConvertBytesToObject<SpaceSettlementData>(packet.contents);
            switch (spaceSettlement._stepMode)
            {
                case SettlementStepMode.Add:
                    SpawnSingleSettlement(spaceSettlement); 
                    break;
                case SettlementStepMode.Remove:
                    WorldObjectManager.RemoveWorldObjectFromTile<WorldObjectFakeOrbitingShip>(spaceSettlement._settlementFile.Tile);
                    break;
            }
        }

        public static void ClearAllSettlements()
        {
            spacePlayerSettlement.Clear();

            WorldObject[] ships = Find.WorldObjects.AllWorldObjects.Where(worldObject => worldObject.def.defName == "RT_Ship " || worldObject.def.defName == "RT_ShipEnemy" || worldObject.def.defName == "RT_ShipNeutral").ToArray();
            foreach (WorldObject ship in ships) Find.WorldObjects.Remove(ship);
        }

        public static void SpawnSingleSettlement(SpaceSettlementData data)
        {
            try
            {
                WorldObjectFakeOrbitingShip ship = SetGoodWillShip(data._settlementFile.Goodwill);
                ship.Tile = data._settlementFile.Tile;
                ship.name = $"{data._settlementFile.UID}'s ship";
                ship.SetFaction(PlanetManagerHelper.GetPlayerFactionFromGoodwill(data._settlementFile.Goodwill));
                ship.phi = data._phi;
                ship.theta = data._theta;
                ship.radius = data._radius;
                ship.OrbitSet();

                ship.altitude = 1000;
                Printer.Warning(StringUtilities.ToString(data));
                Printer.Warning(ship.ToString());
                spacePlayerSettlement.Add(ship);
                Find.WorldObjects.Add(ship);
            }
            catch (Exception e) 
            { 
                Printer.Error($"[SOS2]Failed to spawn ship at {data._settlementFile.Tile}. Reason: {e}");
                Printer.Error($"[SOS2]Debbuging info:\n{StringUtilities.ToString(data)}");
            }
        }

        public static void ChangeGoodwill(int tile, Goodwill goodwill, WorldObjectFakeOrbitingShip oldship = null)
        {
            if (oldship == null)
            {
                oldship = WorldObjectManager.FindWorldObjectFromTile<WorldObjectFakeOrbitingShip>(tile);
            }
            spacePlayerSettlement.Remove(oldship);
            Find.WorldObjects.Remove(oldship);

            WorldObjectFakeOrbitingShip ship = SetGoodWillShip(goodwill);
            ship.Tile = oldship.Tile;
            ship.name = $"{oldship.name}";
            ship.phi = oldship.phi;
            ship.theta = oldship.theta;
            ship.radius = oldship.radius;
            ship.OrbitSet();
            ship.altitude = 1000;

            spacePlayerSettlement.Add(ship);
            Find.WorldObjects.Add(ship);
        }

        public static WorldObjectFakeOrbitingShip SetGoodWillShip(Goodwill goodwill)
        {
            WorldObjectFakeOrbitingShip ship;
            Printer.Warning("test2");
            switch (goodwill)
            {
                default:
                    Printer.Warning("N");
                    ship = (WorldObjectFakeOrbitingShip)WorldObjectMaker.MakeWorldObject(ShipDefOf.RT_ShipNeutral);
                    break;
                case Goodwill.Enemy:
                    Printer.Warning("E");
                    ship = (WorldObjectFakeOrbitingShip)WorldObjectMaker.MakeWorldObject(ShipDefOf.RT_ShipEnemy);
                    break;
                case Goodwill.Ally:
                    Printer.Warning("A");
                    ship = (WorldObjectFakeOrbitingShip)WorldObjectMaker.MakeWorldObject(ShipDefOf.RT_Ship);
                    break;
                case Goodwill.Faction:
                    Printer.Warning("F");
                    ship = (WorldObjectFakeOrbitingShip)WorldObjectMaker.MakeWorldObject(ShipDefOf.RT_Ship);
                    break;
            }
            Printer.Warning("test3");
            ship.SetFaction(PlanetManagerHelper.GetPlayerFactionFromGoodwill(goodwill));
            Printer.Warning("test4");
            return ship;
        }
    }
    public static class SpaceSettlementManagerHelper
    {
        public static void SendSettlementToServer(Map map)
        {
            ShipMapComp comp = map.GetComponent<ShipMapComp>();
            WorldObjectOrbitingShip orbitShip = comp.mapParent;
            SpaceSettlementData spaceSiteData = new SpaceSettlementData();

            spaceSiteData._settlementFile.Tile = map.Tile;
            spaceSiteData._stepMode = SettlementStepMode.Add;
            spaceSiteData._theta = orbitShip.Theta;
            spaceSiteData._radius = orbitShip.Radius;
            orbitShip.Phi = UnityEngine.Random.Range(-70f, 70f);
            spaceSiteData._phi = orbitShip.Phi;
            Packet packet = Packet.CreateModdedPacketFromObject(nameof(SpaceSettlementManager), CommonValues.AssName, spaceSiteData);
            Network.listener.EnqueuePacket(packet);

            SaveManager.ForceSave();
        }
    }
}
