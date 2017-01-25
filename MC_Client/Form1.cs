﻿using MySql.Data.MySqlClient;
using System;
using System.Security.Principal;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.IO.Compression;
using Microsoft.VisualBasic.FileIO;

namespace MC_Client
{
    public partial class Form1 : Form
    {
        //Add method of writing and reading custom install 
        public static string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string Path = AppData+"\\.minecraft\\ElementalRealms";
        public string ERConnectionString = "server=51.255.41.80;uid=ermlpublicread;" +
                "pwd=hmDmxuhheilgKXUWTjzC;database=ElementalRealms_ModdedLauncher;";
        public string Temp = Path +"\\TMP";
        public string Path_Config = Path + "\\Config";

        public Form1()
        {
            InitializeComponent();
            bool HasAdminPrivliges;
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            HasAdminPrivliges = principal.IsInRole(WindowsBuiltInRole.Administrator);

            if (HasAdminPrivliges)
            {
                textBox_Path.Enabled = true;
                button_Path.Enabled = true;
                label_admin.Visible = false;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            button_update.Enabled = false;
            button_update.Text = "Loading";
            comboBox_Versions.Text= "";

            MySql.Data.MySqlClient.MySqlConnection conn;
            conn = new MySql.Data.MySqlClient.MySqlConnection(ERConnectionString);

            string query = "SELECT * FROM ElementalRealms_ModdedLauncher.Version";
            MySqlCommand cmd = new MySqlCommand(query, conn);

            
            try
            {
                conn.OpenAsync();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Console.Write(ex.Message);
            }
            MySqlDataReader dataReader = cmd.ExecuteReader();
            comboBox_Versions.Items.Clear();
            while (dataReader.Read())
            {
                comboBox_Versions.Items.Add(dataReader["Version_UID"].ToString());
                comboBox_Versions.Text=(dataReader["Version_UID"].ToString());
            }

            conn.CloseAsync();
            button_update.Enabled = true;
            button_Install.Enabled = true;
            button_update.Text = "Check For updates";
        }



        private void comboBox_Versions_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        
        private void textBox_Path_TextChanged(object sender, EventArgs e)
        {
            
        }
        
        private void button_Path_Click(object sender, EventArgs e)
        {
            
        }

        private void button_Install_Click(object sender, EventArgs e)
        {

            button_Install.Enabled = false;
            comboBox_Versions.Enabled = false;
            button_update.Enabled = false;
            MySql.Data.MySqlClient.MySqlConnection conn;
            conn = new MySql.Data.MySqlClient.MySqlConnection(ERConnectionString);

            try
            {
                conn.OpenAsync();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Console.Write(ex.Message);
            }

            string query = "SELECT * FROM ElementalRealms_ModdedLauncher.Version WHERE Version_UID='" + comboBox_Versions.Text+ "'";
            MySqlCommand cmd = new MySqlCommand(query, conn);

            MySqlDataReader dataReader = cmd.ExecuteReader();
            WebClient webClient = new WebClient();
            Directory.CreateDirectory(Temp);
            
            //If the program is replacing the configs(next line)
            if (true) { 
                string Temp_ConfigPath = (Temp +"\\"+ comboBox_Versions.Text + "_Config.zip");
                try
                {
                    webClient.DownloadFile(new Uri("https://github.com/ElementalRealms/MC_Configs/archive/" + comboBox_Versions.Text + ".zip"), Temp_ConfigPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("The process failed: {0}", ex.ToString());
                }

                Directory.CreateDirectory(Path_Config);

                string[] Tmp122 = Directory.GetFiles(Path_Config);
                foreach (string filePath in Tmp122)
                {
                        File.Delete(filePath);
                }
                string[] Tmp582 = Directory.GetDirectories(Path_Config);
                foreach (string filePath in Tmp582)
                {
                    var name = new FileInfo(filePath).Name.ToLower();
            //Change BIOME STUFF to folder name
                    if (name != "biome stuff")
                    {
                        Directory.Delete(filePath, true);
                    }
                }
                Directory.CreateDirectory(Path_Config);
                   ZipFile.ExtractToDirectory(Temp_ConfigPath, Path_Config);
                    File.Delete(Temp_ConfigPath);
                }

            FileSystem.MoveDirectory((Path_Config + "\\MC_Configs-" + comboBox_Versions.Text), Path_Config, true);

            //Need A ACTUAL copy if the biome folders
            //(dataReader["Biome"].ToString());

            //Still need to look into how it is installed nowdays :P
            //(dataReader["Forge"].ToString());

            //Not sure how it handles with custom directories
            //(dataReader["Script"].ToString());
            //Some one else can deal with mods
            //(dataReader["Mods"].ToString());


            Directory.Delete(Temp, true);
            conn.CloseAsync();
            button_Install.Enabled = true;
            comboBox_Versions.Enabled = true;
            button_update.Enabled = true;

        }
    }
}
