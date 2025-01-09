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

namespace GameClient.SOS2RTCompat
{
    [RTManager]
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
                    SOS2WorldObjectManager.RemoveWorldObjectFromTile<WorldObjectFakeOrbitingShip>(spaceSettlement._settlementFile.Tile);
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
                ship.name = $"{data._settlementFile.Label}'s ship";
                ship.SetFaction(PlanetManagerHelper.GetPlayerFactionFromGoodwill(data._settlementFile.Goodwill));
                ship.phi = data._settlementFile.Phi;
                ship.theta = data._settlementFile.Theta;
                ship.radius = data._settlementFile.Radius;
                ship.OrbitSet();

                ship.altitude = 1000;
                Printer.Warning(StringUtilities.ToString(data));
                spacePlayerSettlement.Add(ship);
                Find.WorldObjects.Add(ship);
            }
            catch (Exception e) 
            { 
                Printer.Error($"[SOS2]Failed to spawn ship at {data._settlementFile.Tile}. Reason: {e}");
                Printer.Error($"[SOS2]Debbuging info:\n{StringUtilities.ToString(data)}");
            }
        }

        public static void ChangeGoodwill(FactionGoodwillData data, WorldObjectFakeOrbitingShip oldship = null)
        {
            if (oldship == null)
            {
                oldship = SOS2WorldObjectManager.FindWorldObjectFromTile<WorldObjectFakeOrbitingShip>(data._tile);
            }
            spacePlayerSettlement.Remove(oldship);
            Find.WorldObjects.Remove(oldship);

            WorldObjectFakeOrbitingShip ship = SetGoodWillShip(data._goodwill);
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
            Printer.Warning(ShipDefOf.RT_ShipEnemy);
            Printer.Warning(Find.UniqueIDsManager.GetNextWorldObjectID());
            Printer.Warning(Find.TickManager.TicksGame);
            switch (goodwill)
            {
                default:
                    ship = (WorldObjectFakeOrbitingShip)WorldObjectMaker.MakeWorldObject(ShipDefOf.RT_ShipNeutral);
                    break;
                case Goodwill.Enemy:
                    ship = (WorldObjectFakeOrbitingShip)WorldObjectMaker.MakeWorldObject(ShipDefOf.RT_ShipEnemy);
                    break;
                case Goodwill.Ally:
                    ship = (WorldObjectFakeOrbitingShip)WorldObjectMaker.MakeWorldObject(ShipDefOf.RT_Ship);
                    break;
                case Goodwill.Faction:
                    ship = (WorldObjectFakeOrbitingShip)WorldObjectMaker.MakeWorldObject(ShipDefOf.RT_Ship);
                    break;
            }
            ship.SetFaction(PlanetManagerHelper.GetPlayerFactionFromGoodwill(goodwill));
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
            spaceSiteData._settlementFile.Theta = orbitShip.Theta;
            spaceSiteData._settlementFile.Radius = orbitShip.Radius;
            orbitShip.Phi = UnityEngine.Random.Range(-70f, 70f);
            spaceSiteData._settlementFile.Phi = orbitShip.Phi;
            Packet packet = Packet.CreatePacketFromObject(nameof(SpaceSettlementManager), spaceSiteData);
            Network.listener.EnqueuePacket(packet);

            SaveManager.ForceSave();
        }
    }
}
