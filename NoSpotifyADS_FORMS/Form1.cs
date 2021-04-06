using System.Windows.Forms;
using WindowsInput.Native;
using WindowsInput;
using System;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;

namespace NoSpotifyADS_FORMS
{


    public partial class Form1 : Form
    {

        bool hotkey_enabled = true;
        Keys global_hotkey;
        public Form1()
        {
            InitializeComponent();
            
            NHotkey.WindowsForms.HotkeyManager.Current.AddOrReplace("CLOSE", Keys.F7, Start_stop); //standard hotkey F7


        }

        private void Form1_Load(object sender, EventArgs e)
        {

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
            System.Diagnostics.Process.Start(spotify_path); //start spotify
            System.Threading.Thread.Sleep(1000);
            sim.Keyboard.KeyPress(VirtualKeyCode.MEDIA_PLAY_PAUSE); //simulate media_play_pause button to continue playing


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
            this.WindowState = FormWindowState.Normal; //if window is back to normal
            notifyIcon.Visible = false; //dont show icon 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Text = "Enter Key"; //change button1.text to Enter Key
        }



        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (button1.Text == "Enter Key")
            {
                global_hotkey = e.KeyCode;
                
                MessageBox.Show(global_hotkey.ToString());
                label2.Text = "Current Hotkey: " + global_hotkey.ToString(); //change label to the current hotkey
                if (hotkey_enabled == true) //if hotkey is enabled replace old hotkey with new one
                {
                    
                    NHotkey.WindowsForms.HotkeyManager.Current.AddOrReplace("CLOSE", global_hotkey, Start_stop); //Replace hotkey with new one
                }
                button1.Text = "Press to change Hotkey!"; //change button text back 
            }

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public static void AutoStartReg(bool an_aus) //set registry entry for autostart
        {
            string run = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
            string AppName = "NoSpotifyADS";
            

            RegistryKey startKey = Registry.LocalMachine.OpenSubKey(run, true);
            if (an_aus == true)
            {
                startKey.SetValue(AppName, Application.ExecutablePath.ToString());
            }
            else
            {
                startKey.DeleteValue(AppName);
            }
            

        }

        private void testToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            if (testToolStripMenuItem1.Checked == false)
            {
                AutoStartReg(true);
                testToolStripMenuItem1.Checked = true;
            }
            else
            {
                AutoStartReg(false);
                testToolStripMenuItem1.Checked = false;
            }
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void deactivateHotkeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (deactivateHotkeyToolStripMenuItem.Checked == true) //if hotkey is disabled and click on deactivaeHotkey

            {
                deactivateHotkeyToolStripMenuItem.Checked = false; //check deactivateHotkey checkbox
                hotkey_enabled = true;
                NHotkey.WindowsForms.HotkeyManager.Current.AddOrReplace("CLOSE", global_hotkey, Start_stop);

            }
            else //if hotkey is enabled and click on deactivaeHotkey
            {
                deactivateHotkeyToolStripMenuItem.Checked = true; //uncheck deactivateHotkey checkbox
                hotkey_enabled = false;
                NHotkey.WindowsForms.HotkeyManager.Current.AddOrReplace("CLOSE", Keys.None, Start_stop);
            }

        }
    }
}