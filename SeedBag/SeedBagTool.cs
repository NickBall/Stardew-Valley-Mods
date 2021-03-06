﻿using System;
using System.Collections.Generic;
using System.Linq;

using StardewValley;
using StardewValley.Tools;
using StardewValley.TerrainFeatures;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewValley.Objects;
using SObject = StardewValley.Object;
using PyTK.CustomElementHandler;

namespace SeedBag
{
    class SeedBagTool : Hoe, ISaveElement
    {

        internal static Texture2D texture;
        private static Texture2D attTexture;
        private static Texture2D att2Texture;
        private bool inUse;

        public Dictionary<string, string> getAdditionalSaveData()
        {
            Dictionary<string, string> savedata = new Dictionary<string, string>();
            savedata.Add("name", Name);
            return savedata;
        }

        public dynamic getReplacement()
        {
            Chest replacement = new Chest(true);
            if(attachments.Count() > 0)
            {
                if(attachments[0] != null)
                    replacement.addItem(attachments[0]);

                if (attachments[1] != null)
                    replacement.addItem(attachments[1]);
            }
            
            return replacement;
        }

        public void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
            build();
            Chest chest = (Chest)replacement;
            if (!chest.isEmpty())
            {
                if(new List<Item>(chest.items)[0].Category == -19)
                    attachments[1] = (SObject)chest.items[0];
                else
                    attachments[0] = (SObject)chest.items[0];

                if (chest.items.Count > 1)
                    attachments[1] = (SObject)chest.items[1];
            }
            
        }

        public SeedBagTool()
            :base()
        {
            build();
        }

        public override bool canBeTrashed()
        {
            return true;
        }

        internal static void loadTextures()
        {
            texture = SeedBagMod._helper.Content.Load<Texture2D>(@"Assets/seedbag.png");
            attTexture = SeedBagMod._helper.Content.Load<Texture2D>(@"Assets/seedattachment.png");
            att2Texture = SeedBagMod._helper.Content.Load<Texture2D>(@"Assets/fertilizerattachment.png");
        }


        public override bool actionWhenPurchased()
        {
            return true;
        }

        public override Item getOne()
        {
            return new SeedBagTool();
        }

        public override void setNewTileIndexForUpgradeLevel()
        {
        }

        public override string DisplayName { get => "Seed Bag"; set => base.DisplayName = "Seed Bag"; }

        private void build()
        {
            if (texture == null)
                loadTextures();

            Name = "Seed Bag";
            description = "Empty";

            numAttachmentSlots.Value = 2;
            attachments.SetCount(numAttachmentSlots);
            InitialParentTileIndex = 77;
            CurrentParentTileIndex = 77;
            IndexOfMenuItemView = 0;
            UpgradeLevel = 4;
           
            InstantUse = false;
            inUse = false;
        }
        public override void endUsing(GameLocation location, Farmer who)
        {
            base.endUsing(location, who);
        }

        public override int attachmentSlots()
        {
            return numAttachmentSlots.Value;
        }
        
        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, Color color, bool drawShadow)
        {
            spriteBatch.Draw(texture, location + new Vector2(32f, 32f), new Rectangle?(Game1.getSquareSourceRectForNonStandardTileSheet(texture, 16, 16, this.IndexOfMenuItemView)), color * transparency, 0.0f, new Vector2(8f, 8f), 4f * scaleSize, SpriteEffects.None, layerDepth);

            if (inUse)
            {
                StardewValley.Farmer f = Game1.player;
                Vector2 vector = f.getLocalPosition(Game1.viewport) + f.jitter + f.armOffset;
                int num = (int)vector.Y - ((Game1.tileSize * 5)/2);
                spriteBatch.Draw(texture, new Vector2(vector.X, (float)num), new Rectangle?(Game1.getSquareSourceRectForNonStandardTileSheet(Game1.toolSpriteSheet, 16, 16, this.IndexOfMenuItemView)), color * transparency, 0.0f, new Vector2(8f, 8f), 4f * scaleSize, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingY() + Game1.tileSize / 2) / 10000f));
            }
            
        }

        public override void drawAttachments(SpriteBatch b, int x, int y)
        {
            int offset = 65;
            Rectangle attachementSourceRectangle = new Rectangle(0, 0, 64, 64);
            b.Draw(attTexture, new Vector2(x, y), new Rectangle?(attachementSourceRectangle), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.86f);

            Rectangle attachement2SourceRectangle = new Rectangle(0, 0, 64, 64);
            b.Draw(att2Texture, new Vector2(x, y + offset), new Rectangle?(attachement2SourceRectangle), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.86f);

            if (attachments.Count() > 0)
            {
                if(attachments[0] is SObject)
                    attachments[0].drawInMenu(b, new Vector2(x, y), 1f);

                if (attachments[1] is SObject)
                    attachments[1].drawInMenu(b, new Vector2(x, y + offset), 1f);
            }
            
        }

        public override bool onRelease(GameLocation location, int x, int y, StardewValley.Farmer who)
        {
            inUse = false;
            return base.onRelease(location, x, y, who);
        }

        public override bool beginUsing(GameLocation location, int x, int y, StardewValley.Farmer who)
        {
            inUse = true;
            return base.beginUsing(location, x, y, who);
        }

        public override bool canThisBeAttached(SObject o)
        {
            if (o == null || o.Category == -74 || o.Category == -19) { return true; } else { return false; }
        }


        public override SObject attach(SObject o)
        {
            SObject priorAttachement = null;

            if (o != null && o.Category == -74 && attachments[0] != null)
                priorAttachement = new SObject(Vector2.Zero,attachments[0].ParentSheetIndex,attachments[0].Stack);

            if (o != null && o.Category == -19 && attachments[1] != null)
                priorAttachement = new SObject(Vector2.Zero, attachments[1].ParentSheetIndex, attachments[1].Stack);

            if (o == null)
            {
                if(attachments[0] != null)
                {
                    priorAttachement = new SObject(Vector2.Zero, attachments[0].ParentSheetIndex, attachments[0].Stack);
                    attachments[0] = null;
                }else if (attachments[1] != null)
                {
                    priorAttachement = new SObject(Vector2.Zero, attachments[1].ParentSheetIndex, attachments[1].Stack);
                    attachments[1] = null;
                }

                Game1.playSound("dwop");
                return priorAttachement;
            }

            if (canThisBeAttached(o))
            {
                if(o.Category == -74)
                    attachments[0] = o;

                if (o.Category == -19)
                    attachments[1] = o;

                Game1.playSound("button1");
                return priorAttachement;
            }
 
            return null;
        }

        public override void leftClick(Farmer who)
        {
            base.leftClick(who);
        }

        public override void DoFunction(GameLocation location, int x, int y, int power, StardewValley.Farmer who)
        {
            if (attachments.Count == 0 || (attachments[0] == null && attachments[1] == null))
            {
                Game1.showRedMessage("Out of seeds");
                return;
            }

            who.Stamina -= (float)(2 * power) - (float)who.FarmingLevel * 0.1f;
            power = who.toolPower;
            who.stopJittering();
            Game1.playSound("leafrustle");
            Vector2 vector = new Vector2((float)(x / Game1.tileSize), (float)(y / Game1.tileSize));
            List<Vector2> list = base.tilesAffected(vector, power, who);

            foreach (Vector2 current in list)
            {
                if (location.terrainFeatures.ContainsKey(current) && location.terrainFeatures[current] is HoeDirt hd && hd.crop == null && !location.objects.ContainsKey(current))
                {
                    if (attachments[1] != null && hd.fertilizer.Value <= 0)
                    {
                        if (hd.plant(attachments[1].ParentSheetIndex, (int)current.X, (int)current.Y, who, true,location))
                        {
                            attachments[1].Stack--;
                            if (attachments[1].Stack == 0)
                            {
                                attachments[1] = null;
                                Game1.showRedMessage("Out of fertilizer");
                                break;
                            }
                        }
                    }
                    
                    if (attachments[0] != null)
                    {
                        if (hd.plant(attachments[0].ParentSheetIndex, (int)current.X, (int)current.Y, who, false,location))
                        {
                            attachments[0].Stack--;
                            if (attachments[0].Stack == 0)
                            {
                                attachments[0] = null;
                                Game1.showRedMessage("Out of seeds");
                                break;
                            }
                        }
                    }
                }
            }
        }
       
        public override string getDescription()
        {
            if (attachments.Count > 0)
            {
                if(attachments[0] != null)
                {
                    return attachments[0].name;
                }

                if (attachments[1] != null)
                {
                    return attachments[1].name;
                }

                string text = description;
                SpriteFont smallFont = Game1.smallFont;
                int width = Game1.tileSize * 4 + Game1.tileSize / 4;
                return Game1.parseText(text, smallFont, width);
            }
            else
            {
                string text = description;
                SpriteFont smallFont = Game1.smallFont;
                int width = Game1.tileSize * 4 + Game1.tileSize / 4;
                return Game1.parseText(text, smallFont, width);
            }

        }

    }
}
