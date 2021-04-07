using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;


namespace NoSpotifyADS_FORMS
{


    public partial class Form1 : Form
    {
        string version = "v1.1";
        bool customPath_enabled;
        bool hotkey_enabled = true;
        string customPath;
        string spotify_path;
        Keys global_hotkey;
        public Form1()
        {
            InitializeComponent();

            loadSettings(); //load settings from config file



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

            catch (Exception)
            {
                //If spotify is closed just return
            }


            System.Threading.Thread.Sleep(1000); //wait 1 sec to start spotify
            try
            {
                openSpotify();
            }
            catch (Exception)
            {
                MessageBox.Show("try to set a custom spotify Path");
            }

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
            setHotkey(Keys.None); //disable hotkey for now to avoid weird behaviour
        }


        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (button1.Text == "Enter Key")
            {

                global_hotkey = e.KeyCode;
                if (global_hotkey != Keys.F12) //every Key except F12
                {

                    // MessageBox.Show(global_hotkey.ToString());
                    
                    if (hotkey_enabled == true) //if hotkey is enabled replace old hotkey with new one
                    {

                        setHotkey(global_hotkey); //Replace hotkey with new one
                    }


                }

                if (global_hotkey == Keys.F12) //disabling F12 as hotkey
                {
                    MessageBox.Show("You cant use " + global_hotkey.ToString() + " as hotkey!");
                }
                    button1.Text = "Press to change Hotkey!"; //change button text back 



            }

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

        private void testToolStripMenuItem1_Click(object sender, EventArgs e) //Autostart toggle
        {
            try
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
            catch (Exception)
            {
                MessageBox.Show("Try to run program as administrator!");

            }
        }


        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void deactivateHotkeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (deactivateHotkeyToolStripMenuItem.Checked == true) //if hotkey is disabled and click on deactivateHotkey

            {
                deactivateHotkey(false); //enable hotkey
                 


            }
            else //if hotkey is enabled and click on deactivateHotkey
            {
                deactivateHotkey(true); //disable hotkey

            }

        }

        private void setHotkey(Keys hotkey_set)
        {
            NHotkey.WindowsForms.HotkeyManager.Current.AddOrReplace("CLOSE", hotkey_set, Start_stop);
            Properties.Settings.Default.savedHotkey = hotkey_set.ToString();
            label2.Text = "Current Hotkey: " + global_hotkey.ToString(); //change label to the current hotkey
        }


        private void customSpotifyPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (customSpotifyPathToolStripMenuItem.Checked == false)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "exe files (*.exe) | *.exe";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;


                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {

                    customPath = openFileDialog.FileName;
                    //MessageBox.Show(customPath.ToString());
                    customPath_enabled = true;
                    customSpotifyPathToolStripMenuItem.Checked = true;
                    Properties.Settings.Default.customPath = customPath;
                }

            }
            else
            {
                customSpotifyPathToolStripMenuItem.Checked = false;
                customPath_enabled = false;
            }
            //MessageBox.Show("Custom path doesnt work yet!");
        }

        private void openSpotify()
        {
            if (customPath_enabled == false)
            {
                defaultPath();
                System.Diagnostics.Process.Start(spotify_path); //start spotify
            }

            else
            {
                System.Diagnostics.Process.Start(customPath); //start spotify with custom path
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Version: " + version + "\nBenjamin Krejci 2021");
        }

        private void loadSettings()
        {
            if (Properties.Settings.Default.customPath_enabled == true) //check if the user entered a custom path before
            {
                customSpotifyPathToolStripMenuItem.Checked = true;
                customPath_enabled = true;
                customPath = Properties.Settings.Default.customPath;
            }

            global_hotkey = (Keys)Enum.Parse(typeof(Keys), Properties.Settings.Default.savedHotkey, true); //convert hotkey stored as "string" to keys
            setHotkey(global_hotkey);

            hotkey_enabled = Properties.Settings.Default.hotkey_enabled;
            bool testvariable; 
            if (hotkey_enabled == true) //revert true and false
            {
                testvariable = false;
            }
            else
            {
                testvariable = true;
            }
            deactivateHotkey(testvariable);
        }

        private void saveSettings(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.customPath_enabled = customPath_enabled;
            Properties.Settings.Default.hotkey_enabled = hotkey_enabled;
            Properties.Settings.Default.savedHotkey = global_hotkey.ToString();
            Properties.Settings.Default.Save();

        }

        private void deactivateHotkey(bool deactivateHotkey_bool)
        {
            if (deactivateHotkey_bool == true)
            {
                hotkey_enabled = false;
                setHotkey(Keys.None);
                deactivateHotkeyToolStripMenuItem.Checked = true;
            }
            else
            {
                hotkey_enabled = true;
                setHotkey(global_hotkey);
                deactivateHotkeyToolStripMenuItem.Checked = false;
            }
        }
        private void defaultPath()
        {
            spotify_path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"/AppData/Roaming/Spotify/Spotify.exe";
            customPath_enabled = false;
            customSpotifyPathToolStripMenuItem.Checked = false;
        }

        private void defaultSpotifyPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            defaultPath();

        }

        private void debugMessage(string message)
        {
            MessageBox.Show(message);
        }

    }
}