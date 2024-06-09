using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Colors;
using System;
using System.Linq;
using System.Collections.Generic;

[assembly: CommandClass(typeof(AutoCADCommands.MtxRenumberCommand))]

namespace AutoCADCommands
{
    public class MtxRenumberCommand
    {
        [CommandMethod("mtxrenumber")]
        public void MtxRenumber()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            PromptStringOptions psoColor = new PromptStringOptions("\nEnter the color name or ACI number: ");
            PromptResult prColor = ed.GetString(psoColor);
            if (prColor.Status != PromptStatus.OK) return;
            string colorInput = prColor.StringResult;

            // Parse color input
            Color userColor;
            short userColorIndex;
            if (int.TryParse(colorInput, out int aci))
            {
                userColorIndex = (short)aci;
                userColor = Color.FromColorIndex(ColorMethod.ByAci, userColorIndex);
            }
            else
            {
                userColor = GetColorFromName(colorInput);
                userColorIndex = userColor.ColorIndex;
            }

            using (Transaction trans = doc.TransactionManager.StartTransaction())
            {
                BlockTable bt = trans.GetObject(doc.Database.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;

                var mtextElements = btr.Cast<ObjectId>()
                                       .Where(id => id.ObjectClass == RXClass.GetClass(typeof(MText)))
                                       .Select(id => trans.GetObject(id, OpenMode.ForWrite) as MText)
                                       .Where(mtext => mtext.Color.ColorIndex == userColorIndex)
                                       .ToList();

                if (mtextElements.Count == 0)
                {
                    ed.WriteMessage($"\nNo MText elements found with color {colorInput}.");
                }
                else
                {
                    int number = 1;
                    foreach (var mtext in mtextElements)
                    {
                        mtext.Contents = $"{mtext.Contents} {number}";
                        number++;
                    }

                    trans.Commit();
                    ed.WriteMessage($"\nRenumbered {mtextElements.Count} MText elements with color {colorInput}.");
                }
            }
        }

        private Color GetColorFromName(string colorName)
        {
            var colorMap = new Dictionary<string, (byte r, byte g, byte b)>
            {
                {"red", (255, 0, 0)},
                {"green", (0, 255, 0)},
                {"blue", (0, 0, 255)},
                {"yellow", (255, 255, 0)},
                {"cyan", (0, 255, 255)},
                {"magenta", (255, 0, 255)},
                {"white", (255, 255, 255)},
                {"black", (0, 0, 0)},
                {"gray", (128, 128, 128)},
                {"grey", (128, 128, 128)},
                {"orange", (255, 165, 0)},
                {"purple", (128, 0, 128)},
                {"brown", (165, 42, 42)},
                {"pink", (255, 192, 203)},
                {"lightblue", (173, 216, 230)},
                {"lightgreen", (144, 238, 144)},
                {"darkblue", (0, 0, 139)},
                {"darkgreen", (0, 100, 0)},
            };

            if (colorMap.TryGetValue(colorName.ToLower(), out var rgb))
            {
                return Color.FromRgb(rgb.r, rgb.g, rgb.b);
            }

            return Color.FromRgb(0, 0, 0);
        }
    }
}
