using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace orion
{
    class tiles
    {
        int[] roomIndex;        //Stores the tiles in the tiles set
        int tileSetWidth;       //Width of tile sprite sheet
        int tileSetHeight;      //height of tile sprite sheet
        int tileSize;           //size of tiles

        public int xOffset;     //X value of tile set's origin 
        public int yOffset;     //Y value of tile set's origin 
        public int width;       //Width of room
        public int height;      //Height of room

        //Default Constructor 
        public tiles()
        {
            roomIndex = new int[16];
            width = 4;
            height = 4;
            tileSetWidth = 4;
            tileSetHeight = 4;
            tileSize = 64;
            xOffset = 0;
            yOffset = 0;
        }

        //Moves tile set so that player appears at the bottom of the map
        public void movePlayerToStart()
        {
            yOffset = -height * tileSize + 720;
        }

        public bool isEnd()
        {
            if (yOffset == 0)
                return true;
            else
                return false;
        }

        //Custom Constructor
        public tiles(int roomWidth, int roomHeight, int sourceWidth, int sourceHeight,
                     int tilesize, int xOff, int yOff)
        {
            roomIndex = new int[roomWidth * roomHeight];
            width = roomWidth;
            height = roomHeight;
            tileSetWidth = sourceWidth;
            tileSetHeight = sourceHeight;
            tileSize = tilesize;
            xOffset = xOff;
            yOffset = yOff;
        }

        //Returns how many tiles are in the tile set
        public int getTileMapLength()
        {
            return roomIndex.Length;
        }

        //Fills all tiles in set with given tile index
        public void clearRoom(int index)
        {
            for (int i = 0; i < roomIndex.Length; i++) roomIndex[i] = index;
        }

        //Set a tile based on index
        public void SetTile(int mapIndex, int tileIndex)
        {
            roomIndex[mapIndex] = tileIndex;
        }

        //Set a tile based on X-Y values
        public void SetTile(int xx, int yy, int tileIndex)
        {
            roomIndex[xx + (yy * width)] = tileIndex;
        }

        //Get tile index based on x-y values
        public int getTileIndex(int xx, int yy)
        {

            return xx + (yy * width);
        }

        //Gets location of a tile based on index
        Rectangle GetTileRect(int index)
        {
            Rectangle target;
            target.Width = tileSize;
            target.Height = tileSize;
            target.Y = (index / width) * tileSize + yOffset;
            target.X = (index % width) * tileSize + xOffset;
            return target;
        }

        //Gets source tile from tile spritesheet
        Rectangle GetSourceRect(int index)
        {
            Rectangle source;
            source.Width = tileSize;
            source.Height = tileSize;
            source.Y = (roomIndex[index] / tileSetWidth) * tileSize;
            source.X = (roomIndex[index] % tileSetWidth) * tileSize;
            return source;
        }

        //Resize the tile set. Doing so will wipe all tiles in current tile set.
        public void resizeRoom(int w, int h)
        {
            width = w;
            height = h;
            roomIndex = new int[w * h];
            clearRoom(0);
        }

        public void loadTiles(int[] array)
        {
            {
                int w = array[0]; //Get width
                int h = array[1]; //Get height
                resizeRoom(w, h);

                //Loads the tile indexes skipping the first two values in file
                //which are used to load width and height.
                for (int i = 2; i < array.Length; i++)
                {
                    SetTile(i - 2, array[i]);
                }
            }
        }

        //Draw tiles
        public void drawTiles(SpriteBatch sb, Texture2D spriteSheet)
        {
            int start = (yOffset / -tileSize) * width;

            //draw tiles
            for (int i = start; i < getTileMapLength() && (i < start + 260); i++)
            {
                sb.Draw(spriteSheet, GetTileRect(i), GetSourceRect(i),
                    Color.White);
            }
        }

        //Detects given tile in top row in drawn area
        public void spawnGunPad(gunManager gunMan, Vectors index)
        {
            if (yOffset % 64 != 0) return;

            int start = (yOffset / -tileSize) * width;

            for (int i = start; i < start + width; i++)
            {
                if (roomIndex[i] == index.x || roomIndex[i] == index.y || roomIndex[i] == index.z)
                {
                    gunMan.guns.Add(new gun(new Vectors((i % width) * tileSize + xOffset + 30, +32)));
                }
            }
        }

        //Detects given tile in top row in drawn area
        public void spawnGassCannon(Levels lvl, Vectors index)
        {
            if (yOffset % 64 != 0) return;

            int start = (yOffset / -tileSize) * width;
            Vectors offset = new Vectors(-32, -32);

            for (int i = start; i < start + width; i++)
            {
                if (roomIndex[i] == index.x)
                {
                    lvl.gassCannons.Add(new GassCannon(new Vectors((i % width) * tileSize + xOffset + 30, +32) + offset, false, true));
                }
                else if (roomIndex[i] == index.y)
                {
                    lvl.gassCannons.Add(new GassCannon(new Vectors((i % width) * tileSize + xOffset + 30, +32) + offset, false, false));
                }

                else if (roomIndex[i] == index.z)
                {
                    lvl.gassCannons.Add(new GassCannon(new Vectors((i % width) * tileSize + xOffset + 30, +32) + offset, true, true));
                }
                else if (roomIndex[i] == index.w)
                {
                    lvl.gassCannons.Add(new GassCannon(new Vectors((i % width) * tileSize + xOffset + 30, +32) + offset, true, false));
                }
            }
        }
    }
}