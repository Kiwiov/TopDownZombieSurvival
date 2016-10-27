using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using TiledSharp;

namespace ZombieSurvival
{
    public class PathFinder
    {
        public Astar.Node[,] MapData { get; private set; }
        public int SizeX { get { return MapData.GetLength(1);} }
        public int SizeY { get { return MapData.GetLength(0); } }

        private readonly List<Astar.Node> _openNodes = new List<Astar.Node>();
        private readonly List<Astar.Node> _closeNodes = new List<Astar.Node>();

        private int _startX = -1;
        private int _startY = -1;
        private int _goalX = -1;
        private int _goalY = -1;

        public PathFinder(TmxMap data)
        {
            MapData = new Astar.Node[data.Height, data.Width];

            foreach (var tile in data.Layers[1].Tiles)
            {
                var x = tile.X;
                var y = tile.Y;

                MapData[y, x] = new Astar.Node(x, y, 0, 0, 0, 0);
                MapData[y, x].Type = tile.Gid > 0 && tile.Gid != 27 ? Astar.MapType.Wall : Astar.MapType.Nothing;
            }
        }

        public void MoveFromTo()
        {
            if (_startX == -1 || _startY == -1 || _goalX == -1 || _goalY == -1)
                return;

            //Rensa listor o nollställ
            _openNodes.Clear();
            _closeNodes.Clear();
            ResetMap();

            MoveFromTo(_startX, _startY, _goalX, _goalY);
        }

        public List<Astar.Node> PlotRoute()
        {
            if (_startX == -1 || _startY == -1 || _goalX == -1 || _goalY == -1)
                return null;
            var list = new List<Astar.Node>();

            var node = _closeNodes.Last();

            while (node.G != 0)
            {
                //Hitta föregående nod
                for (var i = 0; i < _closeNodes.Count; i++)
                {
                    if (_closeNodes[i].X == node.Sx && _closeNodes[i].Y == node.Sy)
                    {
                        node = _closeNodes[i];
                        break;
                    }
                }
                if (node.G != 0)
                {
                    MapData[node.Y, node.X].Type = Astar.MapType.Route;
                    list.Add(node);
                }
            }

            MapData[_startY, _startX].Type = Astar.MapType.Start;
            MapData[_goalY, _goalX].Type = Astar.MapType.Stop;
            list.Reverse();
            list.Add(MapData[_goalY, _goalX]);
            return list;
        }

        public void SetWall(int x, int y)
        {
            if (x < 0 || y < 0 || x >= SizeX || y >= SizeY)
                return;

            MapData[y, x].Type = Astar.MapType.Wall;
        }

        public void UnSetWall(int x, int y)
        {
            if (x < 0 || y < 0 || x >= SizeX || y >= SizeY)
                return;

            MapData[y, x].Type = Astar.MapType.Nothing;
        }

        public bool SetStart(int x, int y)
        {
            if (x < 0 || y < 0 || x >= SizeX || y >= SizeY)
                return false;

            //Ingen vägg?
            if (MapData[y, x].Type != Astar.MapType.Wall)
            {
                UnSetWall(_startX, _startY);
                _startX = x;
                _startY = y;
                MapData[y, x].Type = Astar.MapType.Start;
                return true;
            }
            return false;
        }

        public bool SetStop(int x, int y)
        {
            if (x < 0 || y < 0 || x >= SizeX || y >= SizeY)
                return false;

            //Ingen vägg?
            if (MapData[y, x].Type != Astar.MapType.Wall)
            {
                UnSetWall(_goalX, _goalY);
                _goalX = x;
                _goalY = y;
                MapData[y, x].Type = Astar.MapType.Stop;
                return true;
            }
            return false;
        }

        #region Private Methods

        private int Distance(int x1, int y1, int x2, int y2)
        {
            //Beräknar så kallade Manhattan-avståndet
            var x = Math.Abs(x1 - x2);
            var y = Math.Abs(y1 - y2);
            return (x + y) * 10;
        }

        private int MoveCost(int x1, int y1, int x2, int y2)
        {
            var dx = Math.Abs(x1 - x2);
            var dy = Math.Abs(y1 - y2);
            if ((dx + dy) == 2)
                return 14;
            if ((dx + dy) == 1)
                return 10;
            return 0;
        }

        private void ResetMap()
        {
            foreach (var node in MapData)
            {
                node.Reset();
            }
        }

        private void MoveFromTo(int x1, int y1, int x2, int y2)
        {
            //Lägg till startpunkten i listan 
            _openNodes.Add(new Astar.Node(x1, y1, x1, y1, 0, Distance(x1, y1, x2, y2)));
            var finished = false;
            int time = 0;

            while (!finished && _openNodes.Count > 0)
            {
                var bestNode = _openNodes.Aggregate((minNode, nextNode) => minNode.F > nextNode.F ? nextNode : minNode);
                var sx = bestNode.X;
                var sy = bestNode.Y;

                //Lägg till punkter runt om och kollar så att de ligger inom kartans gränser
                for (var y = bestNode.Y - 1; y <= sy + 1 && y >= 0 && y < SizeY; y++)
                {
                    for (var x = bestNode.X - 1; x <= sx + 1 && x >= 0 && x < SizeX; x++)
                    {
                        //Strunta i väggar
                        if (MapData[y, x].Type == Astar.MapType.Wall)
                            continue;

                        //Finns noden redan i den bearbetade listan?
                        var closedNode = _closeNodes.FirstOrDefault(n => n.X == x && n.Y == y);
                        if (closedNode != null)
                        {
                            //Om noden finns och vi kunde ta oss hit till "bestNode" 
                            //snabbare så uppdaterar vi besNode
                            int tmpG = closedNode.G + MoveCost(sx, sy, x, y);
                            if (tmpG < bestNode.G)
                            {
                                bestNode.Sx = closedNode.X;
                                bestNode.Sy = closedNode.Y;
                                bestNode.G = tmpG;
                                bestNode.F = bestNode.G + bestNode.H;
                            }
                        }
                        else if (!_openNodes.Any(n => n.X == x && n.Y == y))
                        {
                            //Om noden inte finns varken i open- eller closelistan så lägger vi 
                            //till den med uppdaterad info
                            MapData[y, x] = new Astar.Node(x, y, sx, sy, bestNode.G + MoveCost(x, y, sx, sy),
                                Distance(x, y, x2, y2))
                            { IsVisited = true };
                            _openNodes.Add(MapData[y, x]);

                        }
                    }
                }

                //Flytta över punkten till den stängda listan
                _closeNodes.Add(bestNode);

                //Var detta målet?
                if (bestNode.H == 0)
                    finished = true;

                //Ta bort den från den öppna listan
                _openNodes.Remove(bestNode);
            }
        }

        #endregion
    }
}
