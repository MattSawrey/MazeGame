﻿namespace Maze_Game
{
    public abstract class Item
    {
        protected int Health { get; set; }

        protected void DamageItem(int damageValue)
        {
            Health -= damageValue;
        }
    }
}