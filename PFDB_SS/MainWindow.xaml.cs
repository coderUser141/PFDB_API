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
using System.Media;
using Microsoft.Win32;

namespace PFDB_SS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
		/// <summary>
		/// Initializes the Main Window code-behind
		/// </summary>
        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
        }
		private int screenshotdisplaynumber;
        private int weapondisplaynumber;
		private int categorydisplaynumber;
		private bool SecondaryScreenButtonChecked;
		private bool PrimaryScreenButtonChecked;

		private bool OneScreenshotSelected;
		private bool TwoScreenshotSelected;
		private bool ThreeScreenshotSelected;

		private string SaveDirectory = "C:";

		public bool OneSelected
		{
			get { return OneScreenshotSelected; }
			set
			{
				clearAllScreenshotSelections();
				OneScreenshotSelected = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OneScreenshot"));
			}
		}


		public bool TwoSelected
		{
			get { return TwoScreenshotSelected; }
			set
			{
				clearAllScreenshotSelections();
				TwoScreenshotSelected = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TwoScreenshot"));
			}
		}


		public bool ThreeSelected
		{
			get { return ThreeScreenshotSelected; }
			set
			{
				clearAllScreenshotSelections();
				ThreeScreenshotSelected = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ThreeScreenshot"));
			}
		}

		public bool SecondaryScreenButtonCheckedProp
		{
			get { return SecondaryScreenButtonChecked; }
			set
			{
				SecondaryScreenButtonChecked = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SecondaryScreenButtonCheckedProp"));
			}
		}

		public bool PrimaryScreenButtonCheckedProp
		{
			get { return PrimaryScreenButtonChecked; }
			set
			{
				PrimaryScreenButtonChecked = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PrimaryScreenButtonCheckedProp"));
			}
		}

		public string ScreenshotDisplayNumber
		{
			get { return screenshotdisplaynumber.ToString(); }
			set
			{
				try { screenshotdisplaynumber = Convert.ToInt32(value); } catch { return; }
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ScreenshotDisplayNumber"));
			}
		}

		public string WeaponDisplayNumber
        {
            get { return weapondisplaynumber.ToString(); }
            set {
                try { weapondisplaynumber = Convert.ToInt32(value); } catch { return; }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WeaponDisplayNumber")); //update the places where weaponnumber is used
            }
        }

        public string CategoryDisplayNumber
        {
            get { return categorydisplaynumber.ToString(); }
            set { try { categorydisplaynumber = Convert.ToInt32(value); } catch { return; }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CategoryDisplayNumber")); //update the places where categorynumber is used
			}
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        private void Window_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WeaponDisplayNumber"));
            var window = (Window)sender;
            window.Topmost = true;
        }

        private void Screenshot(object sender, RoutedEventArgs e)
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
                selected = System.Windows.Forms.Screen.PrimaryScreen ?? System.Windows.Forms.Screen.AllScreens[0];

            }
            Bitmap bitmap = new Bitmap(selected.Bounds.Width, selected.Bounds.Height);

            Graphics graphics = Graphics.FromImage(bitmap as System.Drawing.Image);

            graphics.CopyFromScreen(selected.Bounds.X,
                                        selected.Bounds.Y,
                                        0,
                                        0,
                                        selected.Bounds.Size,
                                        CopyPixelOperation.SourceCopy);

            //bitmap.Save(String.Format("E:\\weaponscreenshots\\{0}_{1}.png", categorydisplaynumber,weapondisplaynumber), ImageFormat.Png);
			if (OneSelected)
			{
				reset(bitmap);
			}else if (TwoSelected)
			{
				if(screenshotdisplaynumber < 0){ screenshotdisplaynumber = 0; }
				else if(screenshotdisplaynumber > 1) { screenshotdisplaynumber = 1; }
				else if(screenshotdisplaynumber == 0)
				{
					save(bitmap);
					ScreenshotDisplayNumber = "1";
				}else if(screenshotdisplaynumber == 1)
				{
					reset(bitmap);
				}
			}else if (ThreeSelected)
			{
				if (screenshotdisplaynumber < 0) { screenshotdisplaynumber = 0; }
				else if (screenshotdisplaynumber > 2) { screenshotdisplaynumber = 2; }
				else if (screenshotdisplaynumber == 0 || screenshotdisplaynumber == 1)
				{
					save(bitmap);
					ScreenshotDisplayNumber = (screenshotdisplaynumber+1).ToString();
				}
				else if (screenshotdisplaynumber == 2)
				{
					reset(bitmap);
				}
			}

			
			//WeaponDisplayNumber = Convert.ToInt32(weapondisplaynumber + 1).ToString();
		}

		private void reset(Bitmap bitmap)
		{
			save(bitmap);
			ScreenshotDisplayNumber = "0";
			WeaponDisplayNumber = Convert.ToInt32(weapondisplaynumber + 1).ToString();
		}

		private void save(Bitmap bitmap)
		{
			bitmap.Save($"{SaveDirectory}\\{categorydisplaynumber}_{weapondisplaynumber}_{screenshotdisplaynumber}.png", ImageFormat.Png);
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

		public void clearAllScreenshotSelections()
		{
			OneScreenshotSelected = false;
			TwoScreenshotSelected = false;
			ThreeScreenshotSelected = false;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OneScreenshot"));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TwoScreenshot"));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ThreeScreenshot"));
		}

		private void OneScreenshot_Checked(object sender, RoutedEventArgs e)
		{
			OneSelected = true;
		}

		private void TwoScreenshot_Checked(object sender, RoutedEventArgs e)
		{
			TwoSelected = true;
		}

		private void ThreeScreenshot_Checked(object sender, RoutedEventArgs e)
		{
			ThreeSelected = true;
		}

		private void OpenFolder(object sender, RoutedEventArgs e)
		{
			var folderDialog = new OpenFolderDialog
			{
				AddToRecent = true, ShowHiddenItems = true, ValidateNames = true
			};

			if (folderDialog.ShowDialog() == true)
			{
				SaveDirectory = folderDialog.FolderName;
			}
		}
	}

}
