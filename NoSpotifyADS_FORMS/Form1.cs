using System.Windows.Forms;
using WindowsInput.Native;
using WindowsInput;
using System;
using System.Diagnostics;


namespace NoSpotifyADS_FORMS
{


    public partial class Form1 : Form
    {
        
        
        public Form1()
        {
            InitializeComponent();
            
            NHotkey.WindowsForms.HotkeyManager.Current.AddOrReplace("CLOSE", Keys.F7, Start_stop);
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {




           // this.Resize += new EventHandler(this.HideToTray); //If window gets resize event HideToTray

        }


        void Start_stop(object sender, EventArgs e)
        {


            InputSimulator sim = new InputSimulator();


            try
            {
                foreach (Process spotify in Process.GetProcessesByName("spotify")) //search for spotify processes if found kill them
                {
                    spotify.Kill();
                }
            }

            catch(Exception)
            {
                //If spotify is closed just start it
            }

                        
            System.Threading.Thread.Sleep(1000); //wait 1 sec to start spotify

            string spotify_path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"/AppData/Roaming/Spotify/Spotify.exe";
            System.Diagnostics.Process.Start(spotify_path);
            System.Threading.Thread.Sleep(1000);
            sim.Keyboard.KeyPress(VirtualKeyCode.MEDIA_PLAY_PAUSE);


        }


        private void HideToTray(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized) //Check if window is minimized
            {   
                Hide(); //if it is hide it and
                notifyIcon.Visible = true; //show in tray
            }
        }


        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Text = "Enter Key";
        }



        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (button1.Text == "Enter Key")
            {
                Keys hotkey = e.KeyCode;
                //MessageBox.Show("keypress: " + hotkey);
                label2.Text = "Current Hotkey: " + (char)hotkey;
                NHotkey.WindowsForms.HotkeyManager.Current.AddOrReplace("CLOSE", hotkey, Start_stop);
                button1.Text = "Press to change Hotkey!";
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}