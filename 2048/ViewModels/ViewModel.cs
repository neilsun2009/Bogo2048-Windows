using _2048.Models;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.ViewModels
{
    class ViewModel
    {
        static private ObservableCollection<RankModel> allItems = new ObservableCollection<RankModel>();
        public ObservableCollection<RankModel> AllItems
        {
            get
            {
                return allItems;
            }
        }
        private static ViewModel VMInstance;

        public static ViewModel getInstance()
        {
            if (VMInstance == null) VMInstance = new ViewModel();
            return VMInstance;
        }
        public ViewModel()
        {
            update();
            // allItems.Add(new RankModel(10, "neilsun2009", 2048, "ms-appx:///Assets/rank_10.png"));
        }

        public static void update() {
            var db = App.conn;
            int num = 0;
            allItems.Clear();
            using (var statement = db.Prepare("SELECT * FROM Item ORDER BY Score DESC LIMIT 0,10;"))
            {

                SQLiteResult result = statement.Step();

                while (SQLiteResult.ROW == result)
                {
                    ++num;
                    RankModel rm = new RankModel(num, (String)statement[1], Int32.Parse(statement[2].ToString()), "ms-appx:///Assets/rank_" + num + ".png");
                    allItems.Add(rm);
                    // var i = new Windows.UI.Popups.MessageDialog(statement[1].ToString()).ShowAsync();
                    result = statement.Step();
                }
                
            }
        }
    }
}
