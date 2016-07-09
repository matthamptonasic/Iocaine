using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using Iocaine2.Memory;

namespace Iocaine2.Data.Client
{
    public static partial class Maps
    {
        public class MapSet
        {
            //Only this DLL's classes can create an instance of this object.
            //But outside classes can use an instance that's been made available.
            internal MapSet(Image iBaseMap, UInt16 iZoneId, Byte iMapId)
            {
                cleanMap = iBaseMap;
                zoneId = iZoneId;
                mapId = iMapId;
                setPosToPxlValues();
                cleanZoomedMap = new Bitmap(cleanMap, cleanMap.Width, cleanMap.Height);
                modMapStatic = new Bitmap(cleanMap, cleanMap.Width, cleanMap.Height);
            }
            #region Private Members
            public const Single PercZoomPerScroll = 10.0f;
            private UInt16 zoneId;
            private Byte mapId;
            private Single offX = 0;
            private Single offY = 0;
            private Single mult = 0.8f;
            private Image cleanMap = null;
            private Image cleanZoomedMap = null;
            private Image modMapStatic = null;
            private Image modMapDynamic = null;
            private List<MapOverlay> overlaysStatic = new List<MapOverlay>();
            private List<MapOverlay> overlaysDynamic = new List<MapOverlay>();
            private Boolean suppressArrow = false;
            private ArrowOverlay overlayArrow = null;
            private Single currentZoom = 1.0f;
            private PointF currentCenter = new PointF(256, 256);
            private Boolean showNpcs = true;
            private Boolean showNpcNames = true;
            private Boolean showMobs = true;
            private Boolean showMobNames = true;
            private Boolean showPets = true;
            private Boolean showPetNames = true;
            private Boolean showPcs = true;
            private Boolean showPcNames = true;
            #endregion Private Members
            #region Properties
            #region Display Settings
            public Boolean ShowNpcs
            {
                get
                {
                    return showNpcs;
                }
                set
                {
                    showNpcs = value;
                }
            }
            public Boolean ShowNpcNames
            {
                get
                {
                    return showNpcNames;
                }
                set
                {
                    showNpcNames = value;
                }
            }
            public Boolean ShowMobs
            {
                get
                {
                    return showMobs;
                }
                set
                {
                    showMobs = value;
                }
            }
            public Boolean ShowMobNames
            {
                get
                {
                    return showMobNames;
                }
                set
                {
                    showMobNames = value;
                }
            }
            public Boolean ShowPets
            {
                get
                {
                    return showPets;
                }
                set
                {
                    showPets = value;
                }
            }
            public Boolean ShowPetNames
            {
                get
                {
                    return showPetNames;
                }
                set
                {
                    showPetNames = value;
                }
            }
            public Boolean ShowPcs
            {
                get
                {
                    return showPcs;
                }
                set
                {
                    showPcs = value;
                }
            }
            public Boolean ShowPcNames
            {
                get
                {
                    return showPcNames;
                }
                set
                {
                    showPcNames = value;
                }
            }
            #endregion Display Settings
            public virtual Image CleanMap
            {
                get
                {
                    return cleanMap;
                }
            }
            public virtual Image CleanZoomedMap
            {
                get
                {
                    return cleanZoomedMap;
                }
            }
            public virtual Image StaticMap
            {
                get
                {
                    if(modMapStatic == null)
                    {
                        CommitStaticChanges();
                    }
                    return modMapStatic;
                }
            }
            public virtual Image DynamicMap
            {
                get
                {
                    drawDynamicMap();
                    return modMapDynamic;
                }
            }
            public List<MapOverlay> StaticOverlayItems
            {
                get
                {
                    return overlaysStatic;
                }
                set
                {
                    overlaysStatic = value;
                }
            }
            public List<MapOverlay> DynamicOverlayItems
            {
                get
                {
                    return overlaysDynamic;
                }
                set
                {
                    overlaysDynamic = value;
                }
            }
            public Boolean SuppressArrow
            {
                get
                {
                    return suppressArrow;
                }
                set
                {
                    suppressArrow = value;
                }
            }
            public Single CurrentZoom
            {
                get
                {
                    return currentZoom;
                }
                set
                {
                    currentZoom = value;
                    if (currentZoom < 0.5f)
                    {
                        currentZoom = 0.5f;
                    }
                    SetZoom(currentZoom, currentCenter);
                }
            }
            public PointF CurrentCenter
            {
                get
                {
                    return currentCenter;
                }
                set
                {
                    currentCenter = value;
                    SetZoom(currentZoom, currentCenter);
                }
            }
            #endregion Properties
            #region Public Methods
            public void SetArrowValues(Single iPosX, Single iPosY, Single iAngle)
            {
                //The x/y have to be translated to the current map's pixel values.
                if(overlayArrow == null)
                {
                    overlayArrow = new ArrowOverlay();
                }
                Single pxlX;
                Single pxlY;
                Maps.posToPixels(offX, offY, mult, iPosX, iPosY, out pxlX, out pxlY);
                overlayArrow.PxlX = pxlX;
                overlayArrow.PxlY = pxlY;
                overlayArrow.Angle = iAngle;
                overlayArrow.Zoom = currentZoom;
                overlayArrow.MapCenter = currentCenter;
            }
            public void CommitStaticChanges()
            {
                modMapStatic = new Bitmap(cleanZoomedMap, cleanZoomedMap.Width, cleanZoomedMap.Height);
                Graphics gr = Graphics.FromImage(modMapStatic);
                foreach (MapOverlay item in overlaysStatic)
                {
                    item.Zoom = currentZoom;
                    item.MapCenter = currentCenter;
                    item.Draw(gr);
                }
            }
            public void AddRangeCircle(Single iRadius, Single iCenterX, Single iCenterY)
            {
                Single pxlX, pxlY, tlX, tlY;
                posToPixels(offX, offY, mult, iCenterX, iCenterY, out pxlX, out pxlY);
                posToPixels(offX, offY, mult, iCenterX - iRadius, iCenterY + iRadius, out tlX, out tlY);
                MapOverlay ovl = new Maps.CircleOverlay(pxlX, pxlY, tlX, tlY);
                overlaysDynamic.Add(ovl);
            }
            public void AddTarget(Single iMyX, Single iMyY, MemReads.NPCs.NPCInfoStruct iTgtInfo)
            {

                Single pxlX, pxlY, tgtPxlX, tgtPxlY;
                posToPixels(offX, offY, mult, iMyX, iMyY, out pxlX, out pxlY);
                posToPixels(offX, offY, mult, iTgtInfo.PosX, iTgtInfo.PosY, out tgtPxlX, out tgtPxlY);
                LineTargetOverlay ovl = new LineTargetOverlay(pxlX, pxlY, tgtPxlX, tgtPxlY);
                if(iTgtInfo.Type == MemReads.NPCs.eType.Player)
                {
                    ovl.TargetType = LineTargetOverlay.TgtType.PC;
                }
                else if(iTgtInfo.Active == MemReads.NPCs.eActive.MobDrawn)
                {
                    ovl.TargetType = LineTargetOverlay.TgtType.Mob;
                }
                else if(iTgtInfo.PetID != 0)
                {
                    ovl.TargetType = LineTargetOverlay.TgtType.Pet;
                }
                else
                {
                    ovl.TargetType = LineTargetOverlay.TgtType.NPC;
                }
                overlaysDynamic.Add(ovl);
            }
            public void SetNPCPositions(List<MemReads.NPCs.NPCInfoStruct> npcItems)
            {
                overlaysDynamic.Clear();
                Single pxlX, pxlY;
                List<MemReads.NPCs.eActive> activeFilter = new List<MemReads.NPCs.eActive>();
                activeFilter.Add(MemReads.NPCs.eActive.NotFound);
                activeFilter.Add(MemReads.NPCs.eActive.PCSitNotDrawn);
                activeFilter.Add(MemReads.NPCs.eActive.CharNotDrawn);
                activeFilter.Add(MemReads.NPCs.eActive.NPCNotDrawn2);
                activeFilter.Add(MemReads.NPCs.eActive.MobNotDrawn);
                Dictionary<UInt32, MemReads.NPCs.NPCInfoStruct> unqIdToInfoMap = new Dictionary<uint, MemReads.NPCs.NPCInfoStruct>();
                String myName = PlayerCache.Vitals.Name;
                UInt32 myTargetCode = UInt32.MaxValue;
                Int16 myIndex = 0;
                foreach (MemReads.NPCs.NPCInfoStruct npc in npcItems)
                {
                    if((npc.Type == MemReads.NPCs.eType.Player) || (npc.PetID != 0))
                    {
                        unqIdToInfoMap[npc.TargetCode] = npc;
                    }
                    if(MemReads.NPCs.getName(npc) == myName)
                    {
                        myTargetCode = npc.TargetCode;
                        myIndex = npc.ID;
                    }
                }
                foreach (MemReads.NPCs.NPCInfoStruct npc in npcItems)
                {
                    if (activeFilter.Contains(npc.Active))
                    {
                        continue;
                    }
                    MapOverlay ovl = null;
                    posToPixels(offX, offY, mult, npc.PosX, npc.PosY, out pxlX, out pxlY);
                    if (npc.Type == MemReads.NPCs.eType.Player)
                    {
                        if (Statics.Settings.Top.ShowPcs)
                        {
                            ovl = new EllipsePCOverlay(pxlX, pxlY, MemReads.NPCs.getName(npc));
                            ovl.ShowText = Statics.Settings.Top.ShowPcNames;
                            overlaysDynamic.Add(ovl);
                        }
                    }
                    else if(npc.Type == MemReads.NPCs.eType.NPC)
                    {
                        //Either
                        //1. Mob (Active = MobDrawn)
                        //2. Pet (PetID != 0)
                        //3. NPC (all else)
                        if(npc.Active == MemReads.NPCs.eActive.MobDrawn)
                        {
                            if (Statics.Settings.Top.ShowMobs)
                            {
                                ovl = new EllipseMobOverlay(pxlX, pxlY, MemReads.NPCs.getName(npc));
                                ovl.ShowText = Statics.Settings.Top.ShowMobNames;
                                // Figure out who has claim on the mob if anyone does.
                                if (npc.LastClaimedID != 0)
                                {
                                    if (!unqIdToInfoMap.ContainsKey(npc.LastClaimedID))
                                    {
                                        // We don't have info on who claimed the mob. Set as other.
                                        ((EllipseMobOverlay)ovl).HasClaim = EllipseMobOverlay.Claim.Other;
                                    }
                                    else
                                    {
                                        MemReads.NPCs.NPCInfoStruct lastClaimer = unqIdToInfoMap[npc.LastClaimedID];
                                        if (MemReads.NPCs.getName(lastClaimer) == MemReads.Self.get_name())
                                        {
                                            ((EllipseMobOverlay)ovl).HasClaim = EllipseMobOverlay.Claim.You;
                                        }
                                        else if (lastClaimer.PetID == myIndex)
                                        {
                                            ((EllipseMobOverlay)ovl).HasClaim = EllipseMobOverlay.Claim.You;
                                        }
                                        else
                                        {
                                            List<String> partyMembers = MemReads.Party.get_members();
                                            if (partyMembers.Contains(MemReads.NPCs.getName(lastClaimer)))
                                            {
                                                ((EllipseMobOverlay)ovl).HasClaim = EllipseMobOverlay.Claim.Party;
                                            }
                                            else if (lastClaimer.PetID != 0)
                                            {
                                                MemReads.NPCs.NPCInfoStruct master = new MemReads.NPCs.NPCInfoStruct();
                                                if(MemReads.NPCs.get_NPCInfoStruct(ref master, lastClaimer.PetID))
                                                {
                                                    if(partyMembers.Contains(MemReads.NPCs.getName(master)))
                                                    {
                                                        ((EllipseMobOverlay)ovl).HasClaim = EllipseMobOverlay.Claim.Party;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if(((EllipseMobOverlay)ovl).HasClaim == EllipseMobOverlay.Claim.None)
                                    {
                                        ((EllipseMobOverlay)ovl).HasClaim = EllipseMobOverlay.Claim.Other;
                                    }
                                }
                                overlaysDynamic.Add(ovl);
                            }
                        }
                        else if(npc.PetID != 0)
                        {
                            if (Statics.Settings.Top.ShowPets)
                            {
                                String name = "";
                                if (Statics.Settings.Top.ShowPetNames)
                                {
                                    name = MemReads.NPCs.getName(npc);
                                    MemReads.NPCs.NPCInfoStruct master = new MemReads.NPCs.NPCInfoStruct();
                                    if(MemReads.NPCs.get_NPCInfoStruct(ref master, npc.PetID))
                                    {
                                        name += " (" + MemReads.NPCs.getName(master) + ")";
                                    }
                                }
                                ovl = new EllipsePetOverlay(pxlX, pxlY, name);
                                ovl.ShowText = Statics.Settings.Top.ShowPetNames;
                                overlaysDynamic.Add(ovl);
                            }
                        }
                        else
                        {
                            if (Statics.Settings.Top.ShowNpcs)
                            {
                                ovl = new EllipseNPCOverlay(pxlX, pxlY, MemReads.NPCs.getName(npc));
                                ovl.ShowText = Statics.Settings.Top.ShowNpcNames;
                                overlaysDynamic.Add(ovl);
                            }
                        }
                    }
                }
            }
            public void AddDotPositions(MapOverlay.ItemType iType, List<Int16> iPosXs, List<Int16> iPosYs)
            {
                if (iPosXs.Count != iPosYs.Count)
                {
                    return;
                }

                List<Int16> fltPosXs;
                List<Int16> fltPosYs;
                List<Int16> pxlXs;
                List<Int16> pxlYs;
                filterPosPerMap(zoneId, mapId, iPosXs, iPosYs, out fltPosXs, out fltPosYs);
                posToPixels(zoneId, mapId, fltPosXs, fltPosYs, out pxlXs, out pxlYs);
                for (int ii = 0; ii < pxlXs.Count; ii++)
                {
                    if(iType == MapOverlay.ItemType.ELLIPSE_ALL_FISH)
                    {
                        overlaysStatic.Add(new EllipseAllFishOverlay(pxlXs[ii], pxlYs[ii]));
                    }
                    else if(iType == MapOverlay.ItemType.ELLIPSE_ONE_FISH)
                    {
                        overlaysStatic.Add(new EllipseOneFishOverlay(pxlXs[ii], pxlYs[ii]));
                    }
                }

                if ((iType == MapOverlay.ItemType.ELLIPSE_ALL_FISH) || (iType == MapOverlay.ItemType.ELLIPSE_ONE_FISH))
                {
                    suppressArrow = true;
                }
                CommitStaticChanges();
            }
            public void SetZoom(Single iZoom, PointF iCenter)
            {
                currentZoom = iZoom;
                currentCenter = iCenter;

                Single nbOrigImgPxls = (Int32)(512 / currentZoom);
                Single xStart = currentCenter.X - (nbOrigImgPxls / 2);
                Single yStart = currentCenter.Y - (nbOrigImgPxls / 2);

                // Keep map in the window by 10 pixels.
                if (xStart < ((nbOrigImgPxls - 10) * -1))
                {
                    //Whole map is to the right
                    xStart = nbOrigImgPxls * -1 + 20;
                }
                else if (xStart > (512 - 10))
                {
                    //While map is to the left
                    xStart = 512 - 10;
                }
                if (yStart < ((nbOrigImgPxls - 10) * -1))
                {
                    yStart = nbOrigImgPxls * -1 + 20;
                }
                else if (yStart > (512 - 10))
                {
                    yStart = 512 - 10;
                }
                currentCenter.X = xStart + (nbOrigImgPxls / 2);
                currentCenter.Y = yStart + (nbOrigImgPxls / 2);

                cleanZoomedMap = new Bitmap(cleanMap.Width, cleanMap.Height);
                Graphics gr = Graphics.FromImage(cleanZoomedMap);
                gr.DrawImage(cleanMap, new RectangleF(0, 0, cleanMap.Width, cleanMap.Height), new RectangleF(xStart, yStart, nbOrigImgPxls, nbOrigImgPxls), GraphicsUnit.Pixel);
                gr.Dispose();
                CommitStaticChanges();
            }
            public void Update()
            {
                SetZoom(currentZoom, currentCenter);
            }
            public void CenterPosition(Single iPosX, Single iPosY)
            {
                Single pxlX, pxlY;
                posToPixels(offX, offY, mult, iPosX, iPosY, out pxlX, out pxlY);
                currentCenter = new PointF(pxlX, pxlY);
                Update();
            }
            public void SetPosWidth(Single iWidth, PointF iCenter)
            {
                //Sets the map width by position units.
                Single pxlWidth = iWidth * mult;
                Single newZoom = 512 / pxlWidth;
                SetZoom(newZoom, iCenter);
            }
            public void SetPosWidth(Single iWidth)
            {
                SetPosWidth(iWidth, currentCenter);
            }
            #endregion Public Methods
            #region Private Methods
            private void setPosToPxlValues()
            {
                String filter = "ZoneID=" + zoneId + " AND MapID=" + mapId;
                MapInfoDS.MapInfoMapsRow[] mapsRows = (MapInfoDS.MapInfoMapsRow[])mapInfoDS.MapInfoMaps.Select(filter);
                if (mapsRows.Length == 0)
                {
                    return;
                }
                offX = mapsRows[0].X;
                offY = mapsRows[0].Y;
                mult = mapsRows[0].Multiplier;
            }
            private void drawDynamicMap()
            {
                modMapDynamic = new Bitmap(modMapStatic, modMapStatic.Width, modMapStatic.Height);
                Graphics gr = Graphics.FromImage(modMapDynamic);
                foreach(MapOverlay item in overlaysDynamic)
                {
                    if((item.OLType == MapOverlay.ItemType.ARROW) && suppressArrow)
                    {
                        continue;
                    }
                    item.Zoom = currentZoom;
                    item.MapCenter = currentCenter;
                    item.Draw(gr);
                }
                if(!suppressArrow && (overlayArrow != null))
                {
                    overlayArrow.Draw(gr);
                }
            }
            #endregion Private Methods
        }

        public class MapSetNoMap : MapSet
        {
            //Only this DLL's classes can create an instance of this object.
            //But outside classes can use an instance that's been made available.
            internal MapSetNoMap(Image iBaseMap, UInt16 iZoneId, Byte iMapId)
                : base(iBaseMap, iZoneId, iMapId) { }
            #region Properties
            public override Image CleanZoomedMap
            {
                get
                {
                    return CleanMap;
                }
            }
            public override Image StaticMap
            {
                get
                {
                    return CleanMap;
                }
            }
            public override Image DynamicMap
            {
                get
                {
                    return CleanMap;
                }
            }
            #endregion Properties
        }
    }
}
