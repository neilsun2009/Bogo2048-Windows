using _2048.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace _2048
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class GameOver : Page
    {
        public GameOver()
        {
            this.InitializeComponent();
            tblk_score.Text = Data.score.ToString();
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            Windows.Data.Xml.Dom.XmlDocument doc = new Windows.Data.Xml.Dom.XmlDocument();
            var xml = String.Format(TileTemplateXml, Data.score.ToString());
            doc.LoadXml(WebUtility.HtmlDecode(xml), new XmlLoadSettings
            {
                ProhibitDtd = false,
                ValidateOnParse = false,
                ElementContentWhiteSpace = false,
                ResolveExternals = false
            });

            updater.Update(new TileNotification(doc));
        }
        private const string TileTemplateXml =
  "<tile>" +
   "<visual branding='nameAndLogo'>" +
      "<binding template='TileSmall'>" +
      "<text hint-style='subtitle'>{0}</text>" +
      "<text hint-style='captionSubtle'>最新得分</text>" +
     "</binding>" +
     "<binding template='TileMedium'>" +
      "<text hint-style='subtitle'>{0}</text>" +
      "<text hint-style='captionSubtle'>最新得分</text>" +
     "</binding>" +
     "<binding template='TileWide'>" +
      "<text hint-style='subtitle'>{0}</text>" +
      "<text hint-style='captionSubtle'>最新得分</text>" +
    "</binding>" +
     "<binding template='TileLarge'>" +
      "<text hint-style='subtitle'>{0}</text>" +
      "<text hint-style='captionSubtle'>最新得分</text>" +
    "</binding>" +
   "</visual>" +
 "</tile>";
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(tbox_name.Text))
            {
                var i = new Windows.UI.Popups.MessageDialog("Please enter your name!").ShowAsync();
                return;
            }
            var db = App.conn;
            try
            {
                using (var custstmt = db.Prepare(
                    "INSERT INTO Item (Name, Score) VALUES (?, ?)"))
                {
                    custstmt.Bind(1, tbox_name.Text);
                    custstmt.Bind(2, Data.score);
                    custstmt.Step();
                }
            }
            catch (Exception ex)
            {     // TODO: Handle error
            }
            ViewModel.update();
            Data.clear();
            Frame.Navigate(typeof(MainPage), "");
        }
    }
}
