using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace _2048.Models
{
    class RankModel
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public int Score { get; set; }
        public BitmapImage Image { get; set; }

        public RankModel(int id, String name, int score, String image) {
            Id = id;
            Name = name;
            Score = score;
            Image = new BitmapImage(new Uri(image));
        }
    }
}
