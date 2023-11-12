using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Drawing.Imaging;
using System.ComponentModel;
using WeaponClasses;
using System.Media;

namespace PFDB_SS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
        }
        private int weapondisplaynumber;

        public string WeaponDisplayNumber
        {
            get { return weapondisplaynumber.ToString(); }
            set {
                try { weapondisplaynumber = Convert.ToInt32(value); } catch { return; }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WeaponDisplayNumber"));
            }
        }

        private int categorydisplaynumber;

        public string CategoryDisplayNumber
        {
            get { return categorydisplaynumber.ToString(); }
            set { try { categorydisplaynumber = Convert.ToInt32(value); } catch { return; }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CategoryDisplayNumber"));
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        private void Window_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WeaponDisplayNumber"));
            var window = (Window)sender;
            window.Topmost = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            System.Windows.Forms.Screen selected;

            if(SecondaryScreenButtonCheckedProp && !PrimaryScreenButtonCheckedProp)
            {
                selected = System.Windows.Forms.Screen.AllScreens[1];
            }
            else if(PrimaryScreenButtonCheckedProp && !SecondaryScreenButtonCheckedProp)
            {
                selected = System.Windows.Forms.Screen.AllScreens[0];
            }
            else
            {
                selected = System.Windows.Forms.Screen.PrimaryScreen;

            }
            Bitmap bitmap = new Bitmap(selected.Bounds.Width, selected.Bounds.Height);

            Graphics graphics = Graphics.FromImage(bitmap as System.Drawing.Image);

            graphics.CopyFromScreen(selected.Bounds.X,
                                        selected.Bounds.Y,
                                        0,
                                        0,
                                        selected.Bounds.Size,
                                        CopyPixelOperation.SourceCopy);

            bitmap.Save(String.Format("E:\\weaponscreenshots\\{0}_{1}.png", categorydisplaynumber,weapondisplaynumber), ImageFormat.Png);
            WeaponDisplayNumber = Convert.ToInt32(weapondisplaynumber + 1).ToString();
        }

        private void PrimaryScreenButton_Checked(object sender, RoutedEventArgs e)
        {
            SecondaryScreenButtonCheckedProp = false;
            PrimaryScreenButtonCheckedProp = true;
        }

        private void SecondaryScreenButton_Checked(object sender, RoutedEventArgs e)
        {
            SecondaryScreenButtonCheckedProp = true;
            PrimaryScreenButtonCheckedProp = false;

        }

        private bool SecondaryScreenButtonChecked;

        public bool SecondaryScreenButtonCheckedProp
        {
            get { return SecondaryScreenButtonChecked; }
            set { SecondaryScreenButtonChecked = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SecondaryScreenButtonCheckedProp"));
            }
        }

        private bool PrimaryScreenButtonChecked;

        public bool PrimaryScreenButtonCheckedProp
        {
            get { return PrimaryScreenButtonChecked; }
            set { PrimaryScreenButtonChecked = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PrimaryScreenButtonCheckedProp"));
            }
        }

    }

    public class Scraper
    {


    }
}
