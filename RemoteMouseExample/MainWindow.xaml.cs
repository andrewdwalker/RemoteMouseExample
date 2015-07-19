using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
//using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace RemoteMouseExample
{
    /// <summary>
    /// Simple example of locking out mouse clicks when they are generated from a remote computer.
    /// See: http://stackoverflow.com/questions/14584280/c-sharp-get-mouse-handle-getrawinputdeviceinfo for background info...
    /// Here we look at all Windows MSGs.  If it's a left or right mouse down our GetDeviceID method of clsGetInputID.cs will
    /// return >0 for a local mouse and 0 for a remote mouse.  It will return a negative number for all other Window MSGs
    /// Tested with LogMeIn.com
    /// 
    /// TODO (Left as exercises for the reader)
    /// 1.  Test extensively with LogMeIn and other remote programs.  Does it work?
    /// 2.  Is a remote mouse id guaranteed to be 0 for a remote mouse?  Research and/or test (preferablly both!). 
    ///     If the mouse id is NOT 0, DON'T PANIC!!! Run the other program (WhichMouse program) and look at the output window.  See all the other info we 
    ///     are printing for each mouse?
    /// 3.  In general, check that project was converted from Windows Forms to WPF adequately.
    /// 4.   Are we using ComponentDispatcher_ThreadFilterMessage correctly?  Are we subscribing to event in correct place and unsubcribing
    ///     at correct place?  Probably doesn't matter in this program.  Probably does matter if these ideas are used with windows that come
    ///     and go.  [We don't want to be holding onto event subcriptions...]
    /// 5.   Do we really need to listen to all Windows MSG and have our ThreadFilterMessage deal with them?  Could be a problem!!
    /// 6.   Play with clsGetInputID.cs and get progam to listen to something other than left mouse down and right mouse down.  Such as mouse move, etc.
    /// 7.  Rewrite as MVVM if desired...
    /// </summary>
    public partial class MainWindow : Window
    {

        private bool _allowOperation = false;
        private clsGetInputID MouseHandler;
        public MainWindow()
        {
            InitializeComponent();
           
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MouseHandler = new clsGetInputID(new WindowInteropHelper(this).Handle);
            ComponentDispatcher.ThreadFilterMessage += ComponentDispatcher_ThreadFilterMessage;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            ComponentDispatcher.ThreadFilterMessage -= ComponentDispatcher_ThreadFilterMessage;  
        }

        void ComponentDispatcher_ThreadFilterMessage(ref MSG msg, ref bool handled)
        {
            handled = false;
            if (MouseHandler != null)
            {
                int HardID = MouseHandler.GetDeviceID(msg);

                if (HardID > 0)
                {
                    _allowOperation = true;
                    //System.Diagnostics.Debug.WriteLine("Device ID : " + HardID.ToString());
                    //Return true here if you want to supress the mouse click
                    //bear in mind that mouse down and up messages will still pass through, so you will need to filter these out and return true also.
                }
                else if (HardID == 0)
                {
                    _allowOperation = false;
                    // System.Diagnostics.Debug.WriteLine("Remote Device ID : " + HardID.ToString());
                }
            }
            
        }

        

        private void btnOperate_Click(object sender, RoutedEventArgs e)
        {
            if (_allowOperation)
            {
                txtResults.Foreground = Brushes.Blue;
                txtResults.Text = "Started Heavy Machinery (since you are here and observing things!)";
                //System.Windows.MessageBox.Show("Started Heavy Machinery (since you are here and observing things!)");
            }
            else
            {
                txtResults.Foreground = Brushes.Red;
                txtResults.Text = "Request to start Heavy Machinery DENIED. You must be here, not remote";
               // System.Windows.MessageBox.Show("Request to start Heavy Machinery DENIED. You must be here, not remote");
            }
        }

       

       

        

        
    }
}
