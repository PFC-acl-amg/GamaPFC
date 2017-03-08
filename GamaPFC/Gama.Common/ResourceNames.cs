﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Common
{
    public static class ResourceNames
    {
        public static string AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string IconsAndImagesFolder = AppDataFolder + @"\IconsAndImages\";
        public static string DefaultSearchIconPath = IconsAndImagesFolder + @"default_search_icon.png";
        public static string DefaultUserIconPath = IconsAndImagesFolder + @"default_user_icon.png";
        public static string AtencionIconPath = IconsAndImagesFolder + @"atencion_icon.png";
    }
}