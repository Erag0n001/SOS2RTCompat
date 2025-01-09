using GameClient.Dialogs;
using GameClient.Managers;
using GameClient.Values;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using static Shared.CommonEnumerators;

namespace GameClient.SOS2RTCompat
{
    public class WorldObjectFakeOrbitingShip : WorldObject
    {
        public override Vector3 DrawPos
        {
            get
            {
                return drawPos;
            }
        }
        public override string Label
        {
            get
            {
                if (name == null)
                {
                    return base.Label;
                }
                return name;
            }
        }
        public string name;
        public Vector3 drawPos;
        public float radius = 0;
        public float phi = 0;
        public float theta = 0;
        public float altitude = 1000;
        public override void PostMake()
        {
            base.PostMake();
            OrbitSet();
            Draw();
        }
        public void OrbitSet()
        {
            Vector3 v = Vector3.SlerpUnclamped(new Vector3(0, 0, 1) * radius, new Vector3(0, 0, 1) * radius * -1, theta * -1);
            drawPos = new Vector3(v.x, phi, v.z);
        }
        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (FactionValues.playerFactions.Contains(Faction))
            {
                var gizmoList = new List<Gizmo>();
                gizmoList.Clear();

                Command_Action command_Goodwill = new Command_Action
                {
                    defaultLabel = "Change Goodwill",
                    defaultDesc = "Change the goodwill of this ship",
                    icon = ContentFinder<Texture2D>.Get("Commands/Goodwill"),
                    action = delegate
                    {
                        SOS2SessionValues.chosenWorldObject = this;

                        Action r1 = delegate {
                            SpaceGoodwillManager.TryRequestGoodwill(Goodwill.Enemy,
                            GoodwillTarget.Settlement);
                        };

                        Action r2 = delegate {
                            SpaceGoodwillManager.TryRequestGoodwill(Goodwill.Neutral,
                            GoodwillTarget.Settlement);
                        };

                        Action r3 = delegate {
                            SpaceGoodwillManager.TryRequestGoodwill(Goodwill.Ally,
                            GoodwillTarget.Settlement);
                        };

                        RT_Dialog_3Button d1 = new RT_Dialog_3Button("Change Goodwill", "Set settlement's goodwill to",
                            "Enemy", "Neutral", "Ally", r1, r2, r3, null);

                        DialogManager.PushNewDialog(d1);
                    }
                };

                Command_Action command_FactionMenu = new Command_Action
                {
                    defaultLabel = "Faction Menu",
                    defaultDesc = "Access your faction menu",
                    icon = ContentFinder<Texture2D>.Get("Commands/FactionMenu"),
                    action = delegate
                    {
                        SOS2SessionValues.chosenWorldObject = this;

                        if (SessionValues.actionValues.EnableFactions)
                        {
                            if (SessionValues.chosenSettlement.Faction == FactionValues.yourOnlineFaction) GuildManager.OnFactionOpenOnMember();
                            else GuildManager.OnFactionOpenOnNonMember();
                        }
                    }
                };

                Command_Action command_Event = new Command_Action
                {
                    defaultLabel = "Send Event",
                    defaultDesc = "Send an event to this settlement",
                    icon = ContentFinder<Texture2D>.Get("Commands/Event"),
                    action = delegate
                    {
                        SOS2SessionValues.chosenWorldObject = this;

                        EventManager.ShowEventMenu();
                    }
                };

                if (this.Faction != FactionValues.yourOnlineFaction) gizmoList.Add(command_Goodwill);
                if (ServerValues.hasFaction) gizmoList.Add(command_FactionMenu);
                if (SessionValues.actionValues.EnableEvents) gizmoList.Add(command_Event);
                return gizmoList;
            }

            else if (this.Faction == Find.FactionManager.OfPlayer)
            {
                var gizmoList = new List<Gizmo>();

                Command_Action command_FactionMenu = new Command_Action
                {
                    defaultLabel = "Faction Menu",
                    defaultDesc = "Access your faction menu",
                    icon = ContentFinder<Texture2D>.Get("Commands/FactionMenu"),
                    action = delegate
                    {
                        SOS2SessionValues.chosenWorldObject = this;

                        if (SessionValues.chosenSettlement.Faction == FactionValues.yourOnlineFaction) GuildManager.OnFactionOpenOnMember();
                        else GuildManager.OnFactionOpenOnNonMember();
                    }
                };

                if (SessionValues.actionValues.EnableFactions) gizmoList.Add(command_FactionMenu);
                return gizmoList;
            }
            return base.GetGizmos();
        }
    }
}
