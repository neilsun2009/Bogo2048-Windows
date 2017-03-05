using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace _2048
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // private int[] data;
        ViewModels.ViewModel ViewModel { get; set; }
        private Image[] ORI_IMAGE_RESOURCE; // XAML中方块元素
        private String[] BLOCK_RESOURCE;    // Assets中图片素材
        private static int[,] MOVE_SEQUENCE = new int[,]{{0,4,8,12,1,5,9,13,2,6,10,14,3,7,11,15,-1},
            {3,7,11,15,2,6,10,14,1,5,9,13,0,4,8,12,1},
            {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,-4},
            {12,13,14,15,8,9,10,11,4,5,6,7,0,1,2,3,4}}; // 方块移动到的位置[方向][方块]
        private static int[,] MOVE_BOUNDRY = new int[,]{{0,1,2,3,0,1,2,3,0,1,2,3,0,1,2,3},
            {3,2,1,0,3,2,1,0,3,2,1,0,3,2,1,0},{0,0,0,0,1,1,1,1,2,2,2,2,3,3,3,3},
            {3,3,3,3,2,2,2,2,1,1,1,1,0,0,0,0}}; // 方块最多移动格数[方向][方块]
        private static int[,] imageLocation = new int[,] { {10, 10}, { 10, 105 }, { 10, 200}, { 10, 295},
            {105, 10}, { 105, 105 }, { 105, 200}, { 105, 295}, {200, 10}, { 200, 105 }, { 200, 200}, { 200, 295},
            {295, 10}, { 295, 105 }, { 295, 200}, { 295, 295},}; // 方块相对位置
        // public int score;

        private static int LEFT = 0;
        private static int RIGHT = 1;
        private static int UP = 2;
        private static int DOWN = 3;

        // 单个动画时间
        private static int animTime = 100;

        public MainPage()
        {
            this.InitializeComponent();
            this.ViewModel = ViewModels.ViewModel.getInstance();
            Init();
            InitGame();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }
            // var i = new Windows.UI.Popups.MessageDialog("Welcome!").ShowAsync();
        }
        // 对2的n次方值进行哈希
        private static int BlockHash(int x)
        {
            switch (x)
            {
                case 0:
                    return 0;
                case 2:
                    return 1;
                case 4:
                    return 2;
                case 8:
                    return 3;
                case 16:
                    return 4;
                case 32:
                    return 5;
                case 64:
                    return 6;
                case 128:
                    return 7;
                case 256:
                    return 8;
                case 512:
                    return 9;
                case 1024:
                    return 10;
                case 2048:
                    return 11;
                case 4096:
                    return 12;
                case 8192:
                    return 13;
                case 16384:
                    return 14;
                case 32768:
                    return 15;
                default:
                    return 0;
            }
        }

        // 初始化元素名称、素材等信息
        private void Init()
        {
            ORI_IMAGE_RESOURCE = new Image[16] {block01, block02, block03, block04, block05,
                block06, block07, block08, block09, block10, block11, block12, block13,
                block14, block15, block16};
            BLOCK_RESOURCE = new String[] { "ms-appx:///Assets/block_0.png", "ms-appx:///Assets/block_2.png", "ms-appx:///Assets/block_4.png",
                 "ms-appx:///Assets/block_8.png",  "ms-appx:///Assets/block_16.png",  "ms-appx:///Assets/block_32.png",  "ms-appx:///Assets/block_64.png",
                 "ms-appx:///Assets/block_128.png",  "ms-appx:///Assets/block_256.png",  "ms-appx:///Assets/block_512.png",  "ms-appx:///Assets/block_1024.png",
                 "ms-appx:///Assets/block_2048.png",  "ms-appx:///Assets/block_4096.png",  "ms-appx:///Assets/block_8192.png",  "ms-appx:///Assets/block_16384.png",
                 "ms-appx:///Assets/block_32768.png"};
            // Data.data = new int[16];
        }

        // 初始化游戏
        private void InitGame()
        {
            // 数据与方块初始化

            for (int i = 0; i <= 15; ++i)
            {
                if (Data.data[i] == 0)
                {
                    SetNewPiece(i, Data.data[i]);
                }
                else {
                    StartNewPieceAnimation(i, Data.data[i]);
                }
            }
            // 分数初始化
            // score = 0;
            score_tblk.Text = Data.score.ToString();
            // 随机生成第一块
            SetRandomNewPiece();
        }
        // private Image newPieceImage;
        private void StartNewPieceAnimation(int position, int num)
        {
            /*newPieceImage = new Image();

            newPieceImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/block_32768.png"));
            newPieceImage.Stretch = Stretch.None;
            Thickness thi = new Thickness();
            thi.Left = imageLocation[position, 0];
            thi.Top = imageLocation[position, 1];
            newPieceImage.Margin = thi;
            canvas.Children.Add(newPieceImage);*/
            SetNewPiece(position, num);
            ScaleTransform st = new ScaleTransform();
            st.CenterX = 42;
            st.CenterY = 42;
            st.ScaleX = 0;
            st.ScaleY = 0;
            // newPieceImage.RenderTransform = st;
            ORI_IMAGE_RESOURCE[position].RenderTransform = st;

            DoubleAnimation anim1 = new DoubleAnimation();
            DoubleAnimation anim2 = new DoubleAnimation();
            anim1.Duration = new Duration(TimeSpan.FromMilliseconds(animTime));
            anim2.Duration = new Duration(TimeSpan.FromMilliseconds(animTime));
            Storyboard sb = new Storyboard();
            sb.Duration = new Duration(TimeSpan.FromMilliseconds(animTime));
            sb.Children.Add(anim1);
            sb.Children.Add(anim2);
            Storyboard.SetTarget(anim1, st);
            Storyboard.SetTargetProperty(anim1, "ScaleX");
            Storyboard.SetTarget(anim2, st);
            Storyboard.SetTargetProperty(anim2, "ScaleY");
            anim1.To = 1;
            anim2.To = 1;

            canvas.Resources.Add("sb"+position, sb);
            sb.Begin();
            canvas.Resources.Remove("sb" + position);
            // await Task.Delay(100);
            /*Task task = new Task<int>(StartNewPiceAnimationTask);
            task.Start();
            // canvas.Resources.Remove("sb"+position);
            // SetNewPiece(position, num);
            // canvas.Children.Remove(tem);
            // canvas.Children.Remove(tem);
            task.ContinueWith(tas =>
            {
                canvas.Children.Remove(newPieceImage);
                
                SetNewPiece(position, num);
            }, TaskScheduler.FromCurrentSynchronizationContext());*/
        }

        // 设置新方块
        private void SetNewPiece(int position, int number)
        {
            

            Data.data[position] = number;
            ORI_IMAGE_RESOURCE[position].Source = new BitmapImage(new Uri(BLOCK_RESOURCE[BlockHash(number)]));
        }

        // 随机放置方块
        private void SetRandomNewPiece()
        {
            Random random = new Random();
            int numBlank = countNumBlank();
            if (numBlank == 0)
            {
                // gameOver();
                return;
            }
            int r = random.Next(numBlank);
            int p = 0;
            for (int i = 0; i < 16; ++i)
            {
                if (Data.data[i] != 0) continue;
                if (p == r)
                {
                    StartNewPieceAnimation(i, 2);
                    // setNewPiece(i, 2);
                    break;
                }
                ++p;
            }

        }

        // 计算空白方块数量
        private int countNumBlank()
        {
            int numBlank = 0;
            for (int i = 0; i < 16; ++i)
            {
                if (Data.data[i] == 0)
                {
                    ++numBlank;
                }
            }
            return numBlank;
        }

        // 开始新游戏
        private void new_btn_Click(object sender, RoutedEventArgs e)
        {
            Data.clear();
            InitGame();
        }

        // 打开排行榜
        private void rank_btn_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.ActualWidth <= 1000)
            {
                Frame.Navigate(typeof(RankPage), "");
            }
        }

        // 检查是否游戏结束
        private void CheckGameOver()
        {
            int numBlank = countNumBlank();

            if (numBlank != 0)
            {
                return;
            }
            for (int i = 0; i < 16; ++i)
            {
                if (i % 4 != 3)
                {
                    int tem = i + 1;
                    if (Data.data[i] == Data.data[tem])
                    {
                        return;
                    }
                }
                if (i < 12)
                {
                    int tem = i + 4;
                    if (Data.data[i] == Data.data[tem])
                    {
                        return;
                    }
                }
            }
            GameOver();
        }
        // 游戏结束
        private void GameOver()
        {
            Frame.Navigate(typeof(GameOver), "");
        }
        // 上下左右按钮点击
        private void btn_up_Click(object sender, RoutedEventArgs e)
        {
            MoveBlock(UP);
        }

        private void btn_down_Click(object sender, RoutedEventArgs e)
        {
            MoveBlock(DOWN);
        }

        private void btn_left_Click(object sender, RoutedEventArgs e)
        {
            MoveBlock(LEFT);
        }

        private void btn_right_Click(object sender, RoutedEventArgs e)
        {
            MoveBlock(RIGHT);
        }

        // 移动方块
        private void MoveBlock(int direction) {
            CheckGameOver();
            bool noChange = true;
            int maxTimes = 0; // 本次移动最多移动次数格数
            bool[] canMerge = new bool[16];
            for (int i = 0; i < 16; ++i)
            {
                canMerge[i] = true;
            }
            for (int i = 4; i < 16; ++i)
            {
                int now = MOVE_SEQUENCE[direction, i], tem = now + MOVE_SEQUENCE[direction, 16];
                if (Data.data[now] == 0)
                {
                    continue;
                }
                int start = now, end = tem, num = Data.data[start];
                bool move = false, merge = false;
                int timeMax = MOVE_BOUNDRY[direction, now], time = 0;
                while ((Data.data[tem] == 0 || Data.data[tem] == Data.data[now])
                        && time < timeMax)
                {
                    noChange = false;
                    move = true;
                    end = tem;
                    ++time;
                    if (time > maxTimes)
                    {
                        maxTimes = time;
                    }
                    if (Data.data[tem] == 0)
                    {
                        // LogUtil.d("Move", now + " " + tem);
                        Data.data[tem] = Data.data[now];
                        Data.data[now] = 0;
                        now = tem;
                        tem = now + MOVE_SEQUENCE[direction, 16];
                        if (time == timeMax) break;
                        if (!canMerge[tem]) break;
                    }
                    else {
                        Data.data[now]= 0;
                        Data.data[tem]= Data.data[tem] * 2;
                        Data.score += Data.data[tem];
                        canMerge[tem] = false;
                        merge = true;
                        break;
                    }
                }
                if (move)
                {
                    StartTranslateAnimation(start, end, num, time, merge);
                }
            }
            if (!noChange)
            {
                score_tblk.Text = Data.score.ToString();
                SetRandomNewPiece();
                CheckGameOver();
            }
            // Task.Delay(animTime * maxTimes).Wait();
            
            // Task.Delay(150).Wait();
           
        }
        private void StartTranslateAnimation(int prev, int now, int num, int time, bool merge)
        {

            SetNewPiece(prev, 0);
            SetNewPiece(now, num);
            if (merge)
            {
                    StartMergePieceAnimation(now, num * 2);
            }
        }
        private void StartMergePieceAnimation(int position, int num)
        {

            SetNewPiece(position, num);

            ScaleTransform st = new ScaleTransform();
            st.CenterX = 42;
            st.CenterY = 42;
            st.ScaleX = 1;
            st.ScaleY = 1;
            // newPieceImage.RenderTransform = st;
            ORI_IMAGE_RESOURCE[position].RenderTransform = st;

            DoubleAnimation anim1 = new DoubleAnimation();
            DoubleAnimation anim2 = new DoubleAnimation();
            anim1.Duration = new Duration(TimeSpan.FromMilliseconds(animTime/2));
            anim2.Duration = new Duration(TimeSpan.FromMilliseconds(animTime/2));
            Storyboard sb = new Storyboard();
            sb.Duration = new Duration(TimeSpan.FromMilliseconds(animTime/2));
            sb.Children.Add(anim1);
            sb.Children.Add(anim2);
            sb.AutoReverse = true;
            Storyboard.SetTarget(anim1, st);
            Storyboard.SetTargetProperty(anim1, "ScaleX");
            Storyboard.SetTarget(anim2, st);
            Storyboard.SetTargetProperty(anim2, "ScaleY");
            anim1.To = 1.1;
            anim2.To = 1.1;

            canvas.Resources.Add("sb_merge" + position, sb);
            sb.Begin();
            canvas.Resources.Remove("sb_merge" + position);
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Up) {
                MoveBlock(UP);
            } else if (e.Key == Windows.System.VirtualKey.Down)
            {
                MoveBlock(DOWN);
            }
            else if(e.Key == Windows.System.VirtualKey.Left) {
                MoveBlock(LEFT);
            } else if (e.Key == Windows.System.VirtualKey.Right)
            {
                MoveBlock(RIGHT);
            }
        }
    }
}
