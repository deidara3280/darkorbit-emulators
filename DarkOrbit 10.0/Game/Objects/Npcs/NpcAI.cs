﻿using Ow.Game.Movements;
using Ow.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ow.Game.Objects.Npcs
{
    class NpcAI
    {
        public Npc Npc { get; set; }

        private NpcAIOption AIOption = NpcAIOption.SEARCH_FOR_ENEMIES;
        private static int ALIEN_DISTANCE_TO_USER = 300;

        public NpcAI(Npc npc) { Npc = npc; }

        public DateTime lastMovement = new DateTime();

        public void TickMovement()
        {
            if(lastMovement.AddSeconds(1) < DateTime.Now)
            {
                switch (AIOption)
                {
                    case NpcAIOption.SEARCH_FOR_ENEMIES:
                        foreach (var players in Npc.InRangeCharacters.Values)
                        {
                            if (players is Player)
                            {
                                var player = players as Player;

                                if (player.Storage.IsInDemilitarizedZone || player.Invisible || Npc.Position.DistanceTo(player.Position) > Npc.RenderRange)
                                {
                                    Npc.Selected = null;
                                    AIOption = NpcAIOption.SEARCH_FOR_ENEMIES;
                                }
                                else
                                {
                                    Npc.Selected = player;
                                    AIOption = NpcAIOption.FLY_TO_ENEMY;
                                }
                            }
                        }

                        if (!Npc.Moving && Npc.Selected == null)
                        {
                            int nextPosX = Randoms.random.Next(20000);
                            int nextPosY = Randoms.random.Next(12800);

                            Movement.Move(Npc, new Position(nextPosX, nextPosY));
                        }
                        break;
                    case NpcAIOption.FLY_TO_ENEMY:
                        if (Npc.Selected != null && Npc.Selected is Player && !(Npc.Selected as Player).Storage.IsInDemilitarizedZone && Npc.Position.DistanceTo((Npc.Selected as Player).Position) < Npc.RenderRange)
                        {
                            var player = Npc.Selected as Player;

                            double angle = DegreeToRadian(Randoms.random.Next() * 360);
                            int newPosX = (int)(player.Position.X + (ALIEN_DISTANCE_TO_USER * Math.Cos(angle)));
                            int newPosY = (int)(player.Position.Y + (ALIEN_DISTANCE_TO_USER * Math.Sin(angle)));

                            Movement.Move(Npc, new Position(newPosX, newPosY));
                            AIOption = NpcAIOption.WAIT_PLAYER_MOVE;
                        } 
                        else
                        {
                            Npc.Selected = null;
                            AIOption = NpcAIOption.SEARCH_FOR_ENEMIES;
                        }
                        break;
                    case NpcAIOption.WAIT_PLAYER_MOVE:
                        if (Npc.Selected != null && Npc.Selected is Player && !(Npc.Selected as Player).Storage.IsInDemilitarizedZone)
                        {
                            var player = Npc.Selected as Player;

                            if (player.Moving)
                                AIOption = NpcAIOption.FLY_TO_ENEMY;
                        }
                        else
                        {
                            Npc.Selected = null;
                            AIOption = NpcAIOption.SEARCH_FOR_ENEMIES;
                        }
                        break;
                }

                lastMovement = DateTime.Now;
            }
        }

        private double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}
