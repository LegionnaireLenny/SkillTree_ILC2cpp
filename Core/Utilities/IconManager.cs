using S1API.Utils;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace SkillTree.Core.Utilities
{
    public class IconManager
    {
        public static readonly string IconApp = "Icon_SkillTree_Forked.png";
        public static readonly string IconClock = "Icon_Clock.png";
        public static readonly string IconHeart = "Icon_Heart.png";
        public static readonly string IconTrashcan = "Icon_Trashcan.png";
        public static readonly string IconWashingMachine = "Icon_WashingMachine.png";
        public static readonly string IconCash = "Icon_Cash.png";
        public static readonly string IconEnforcer = "Icon_Enforcer.png";
        public static readonly string IconSupplier = "Icon_Supplier.png";
        public static readonly string IconHustler = "Icon_Hustler.png";
        public static readonly string IconLogistician = "Icon_Logistician.png";
        public static readonly string IconSpecial = "Icon_Special.png";
        public static readonly string IconPolice = "Icon_PoliceOfficer.png";
        public static readonly string IconBenziesDealer = "Icon_BenziesDealer.png";
        public static readonly string IconBenziesGoon = "Icon_BenziesGoon.png";

        public static Sprite LoadSprite(string directory, string filename)
        {
            string path = Path.Combine(directory, filename);
            if (File.Exists(path))
            {
                return ImageUtils.LoadImage(path);
            }
            else
            {
                return ImageUtils.LoadImageFromResource(Assembly.GetExecutingAssembly(), $"SkillTree.Core.Images.Icons.{filename}");
            }
        }

        public static Sprite LoadSprite(string filename)
        {
            string path = Path.Combine(Core.IconDirectory, filename);
            if (File.Exists(path))
            {
                return ImageUtils.LoadImage(path);
            }
            else
            {
                return ImageUtils.LoadImageFromResource(Assembly.GetExecutingAssembly(), $"SkillTree.Core.Images.Icons.{filename}");
            }
        }

        public static void ExtractEmbeddedResource(string directory, string fileName)
        {
            try
            {
                string destination = Path.Combine(directory, fileName);
                if (!File.Exists(destination))
                {
                    using var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream($"SkillTree.Core.Images.Icons.{fileName}");
                    using FileStream stream = new FileStream(destination, FileMode.Create, FileAccess.Write);
                    resource.CopyTo(stream);
                }
            }
            catch (Exception e)
            {
                LogManager.LogMessage($"Error extracting {fileName} from assembly {e}", LogLevel.Warning);
            }
        }

        public static void ExtractIcons()
        {
            if (!Directory.Exists(Core.IconDirectory))
            {
                Directory.CreateDirectory(Core.IconDirectory);
            }

            ExtractEmbeddedResource(Core.IconDirectory, IconApp);
            ExtractEmbeddedResource(Core.IconDirectory, IconClock);
            ExtractEmbeddedResource(Core.IconDirectory, IconHeart);
            ExtractEmbeddedResource(Core.IconDirectory, IconTrashcan);
            ExtractEmbeddedResource(Core.IconDirectory, IconWashingMachine);
            ExtractEmbeddedResource(Core.IconDirectory, IconCash);
            ExtractEmbeddedResource(Core.IconDirectory, IconEnforcer);
            ExtractEmbeddedResource(Core.IconDirectory, IconSupplier);
            ExtractEmbeddedResource(Core.IconDirectory, IconHustler);
            ExtractEmbeddedResource(Core.IconDirectory, IconLogistician);
            ExtractEmbeddedResource(Core.IconDirectory, IconSpecial);
            ExtractEmbeddedResource(Core.IconDirectory, IconPolice);
            ExtractEmbeddedResource(Core.IconDirectory, IconBenziesDealer);
            ExtractEmbeddedResource(Core.IconDirectory, IconBenziesGoon);
        }
    }
}
