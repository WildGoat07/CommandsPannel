﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;
using System.Windows.Media;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Net;
using System.Runtime.Serialization;

namespace CommandsPannel
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Public Fields

        public static ImageSource Blank = ConvertImage(CommandsPannel.Properties.Resources.blank);
        public static WebClient Client = new WebClient();
        public static MainWindow Window;

        #endregion Public Fields

        #region Public Properties

        public static Data Data { get; set; }

        #endregion Public Properties

        #region Public Methods

        public static ImageSource ConvertImage(Bitmap bitmap)
        {
            using var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            stream.Seek(0, SeekOrigin.Begin);
            var img = new BitmapImage();
            img.BeginInit();
            img.StreamSource = stream;
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.EndInit();
            img.Freeze();
            return img;
        }

        public static T GetInfo<T>(SerializationInfo info, string str)
        {
            try
            {
                return (T)info.GetValue(str, typeof(T));
            }
            catch (SerializationException)
            {
                return default;
            }
        }

        public static void LoadData()
        {
            var path = Path.Combine(Data.WorkingDir, "data.json");
            if (File.Exists(path))
            {
                var formatter = new Newtonsoft.Json.JsonSerializer();
                using var stream = new Newtonsoft.Json.JsonTextReader(new StreamReader(path)) { CloseInput = true };
                Data = formatter.Deserialize<Data>(stream);
            }
            else
            {
                Data = new Data { Buttons = new List<ActionButton>(), Left = Current.MainWindow.Left, Top = Current.MainWindow.Top };
                Data.Buttons.Add(new ActionButton
                {
                    file = "steam://open/main",
                    name = "Steam"
                });
                Data.Buttons.Add(new ActionButton
                {
                    file = "https://google.com",
                    name = "Google"
                });
                Data.Buttons.Add(new ActionButton
                {
                    file = "explorer",
                    name = "Explorer"
                });
                Directory.CreateDirectory(Data.WorkingDir);
                var formatter = new Newtonsoft.Json.JsonSerializer();
                formatter.Formatting = Newtonsoft.Json.Formatting.Indented;
                using var JSONstream = new StreamWriter(Path.Combine(Data.WorkingDir, "data.json"));
                formatter.Serialize(JSONstream, Data);
                JSONstream.Flush();
            }
        }

        public static void SaveData()
        {
            Data.Top = Current.MainWindow.Top + 200;
            Data.Left = Current.MainWindow.Left;
            var path = Path.Combine(Data.WorkingDir, "data.json");
            if (!File.Exists(path))
                Directory.CreateDirectory(Data.WorkingDir);
            var formatter = new Newtonsoft.Json.JsonSerializer();
            formatter.Formatting = Newtonsoft.Json.Formatting.Indented;
            using var JSONstream = new StreamWriter(path);
            formatter.Serialize(JSONstream, Data);
            JSONstream.Flush();
        }

        #endregion Public Methods
    }
}