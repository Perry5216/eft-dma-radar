using SkiaSharp.Views.Desktop;

namespace eft_dma_radar
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox_MapSetup = new System.Windows.Forms.GroupBox();
            this.checkBox_MapFree = new System.Windows.Forms.CheckBox();
            this.button_MapSetupApply = new System.Windows.Forms.Button();
            this.textBox_mapScale = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_mapY = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_mapX = new System.Windows.Forms.TextBox();
            this.label_Pos = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_watchlist = new System.Windows.Forms.Button();
            this.button_RefreshLoot = new System.Windows.Forms.Button();
            this.checkBox_Aimview = new System.Windows.Forms.CheckBox();
            this.button_Restart = new System.Windows.Forms.Button();
            this.checkBox_MapSetup = new System.Windows.Forms.CheckBox();
            this.checkBox_Loot = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.trackBar_Zoom = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.trackBar_AimLength = new System.Windows.Forms.TrackBar();
            this.button_Map = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.richTextBox_PlayersInfo = new System.Windows.Forms.RichTextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.listView_PmcHistory = new System.Windows.Forms.ListView();
            this.columnHeader_Entry = new System.Windows.Forms.ColumnHeader();
            this.columnHeader_ID = new System.Windows.Forms.ColumnHeader();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox_MapSetup.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Zoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_AimLength)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1336, 1061);
            this.tabControl1.TabIndex = 8;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox_MapSetup);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1328, 1033);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Radar";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox_MapSetup
            // 
            this.groupBox_MapSetup.Controls.Add(this.checkBox_MapFree);
            this.groupBox_MapSetup.Controls.Add(this.button_MapSetupApply);
            this.groupBox_MapSetup.Controls.Add(this.textBox_mapScale);
            this.groupBox_MapSetup.Controls.Add(this.label5);
            this.groupBox_MapSetup.Controls.Add(this.textBox_mapY);
            this.groupBox_MapSetup.Controls.Add(this.label4);
            this.groupBox_MapSetup.Controls.Add(this.textBox_mapX);
            this.groupBox_MapSetup.Controls.Add(this.label_Pos);
            this.groupBox_MapSetup.Location = new System.Drawing.Point(8, 6);
            this.groupBox_MapSetup.Name = "groupBox_MapSetup";
            this.groupBox_MapSetup.Size = new System.Drawing.Size(327, 175);
            this.groupBox_MapSetup.TabIndex = 11;
            this.groupBox_MapSetup.TabStop = false;
            this.groupBox_MapSetup.Text = "Map Setup";
            this.groupBox_MapSetup.Visible = false;
            // 
            // checkBox_MapFree
            // 
            this.checkBox_MapFree.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_MapFree.AutoSize = true;
            this.checkBox_MapFree.Location = new System.Drawing.Point(0, 0);
            this.checkBox_MapFree.Name = "checkBox_MapFree";
            this.checkBox_MapFree.Size = new System.Drawing.Size(66, 25);
            this.checkBox_MapFree.TabIndex = 17;
            this.checkBox_MapFree.Text = "Map Free";
            this.checkBox_MapFree.UseVisualStyleBackColor = true;
            this.checkBox_MapFree.CheckedChanged += new System.EventHandler(this.checkBox_MapFree_CheckedChanged);
            // 
            // button_MapSetupApply
            // 
            this.button_MapSetupApply.Location = new System.Drawing.Point(6, 143);
            this.button_MapSetupApply.Name = "button_MapSetupApply";
            this.button_MapSetupApply.Size = new System.Drawing.Size(75, 23);
            this.button_MapSetupApply.TabIndex = 16;
            this.button_MapSetupApply.Text = "Apply";
            this.button_MapSetupApply.UseVisualStyleBackColor = true;
            this.button_MapSetupApply.Click += new System.EventHandler(this.button_MapSetupApply_Click);
            // 
            // textBox_mapScale
            // 
            this.textBox_mapScale.Location = new System.Drawing.Point(46, 101);
            this.textBox_mapScale.Name = "textBox_mapScale";
            this.textBox_mapScale.Size = new System.Drawing.Size(50, 23);
            this.textBox_mapScale.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 104);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 15);
            this.label5.TabIndex = 14;
            this.label5.Text = "Scale";
            // 
            // textBox_mapY
            // 
            this.textBox_mapY.Location = new System.Drawing.Point(102, 67);
            this.textBox_mapY.Name = "textBox_mapY";
            this.textBox_mapY.Size = new System.Drawing.Size(50, 23);
            this.textBox_mapY.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 70);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 15);
            this.label4.TabIndex = 12;
            this.label4.Text = "X,Y";
            // 
            // textBox_mapX
            // 
            this.textBox_mapX.Location = new System.Drawing.Point(46, 67);
            this.textBox_mapX.Name = "textBox_mapX";
            this.textBox_mapX.Size = new System.Drawing.Size(50, 23);
            this.textBox_mapX.TabIndex = 11;
            // 
            // label_Pos
            // 
            this.label_Pos.AutoSize = true;
            this.label_Pos.Location = new System.Drawing.Point(7, 19);
            this.label_Pos.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_Pos.Name = "label_Pos";
            this.label_Pos.Size = new System.Drawing.Size(43, 15);
            this.label_Pos.TabIndex = 10;
            this.label_Pos.Text = "coords";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1328, 1033);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Settings";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_watchlist);
            this.groupBox1.Controls.Add(this.button_RefreshLoot);
            this.groupBox1.Controls.Add(this.checkBox_Aimview);
            this.groupBox1.Controls.Add(this.button_Restart);
            this.groupBox1.Controls.Add(this.checkBox_MapSetup);
            this.groupBox1.Controls.Add(this.checkBox_Loot);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.trackBar_Zoom);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.trackBar_AimLength);
            this.groupBox1.Controls.Add(this.button_Map);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Size = new System.Drawing.Size(525, 1027);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Radar Config";
            // 
            // button_watchlist
            // 
            this.button_watchlist.Location = new System.Drawing.Point(382, 33);
            this.button_watchlist.Name = "button_watchlist";
            this.button_watchlist.Size = new System.Drawing.Size(104, 27);
            this.button_watchlist.TabIndex = 21;
            this.button_watchlist.Text = "Reload Watchlist";
            this.button_watchlist.UseVisualStyleBackColor = true;
            this.button_watchlist.Click += new System.EventHandler(this.button_watchlist_Click);
            // 
            // button_RefreshLoot
            // 
            this.button_RefreshLoot.Location = new System.Drawing.Point(167, 33);
            this.button_RefreshLoot.Name = "button_RefreshLoot";
            this.button_RefreshLoot.Size = new System.Drawing.Size(115, 27);
            this.button_RefreshLoot.TabIndex = 20;
            this.button_RefreshLoot.Text = "Refresh Loot";
            this.button_RefreshLoot.UseVisualStyleBackColor = true;
            this.button_RefreshLoot.Click += new System.EventHandler(this.button_RefreshLoot_Click);
            // 
            // checkBox_Aimview
            // 
            this.checkBox_Aimview.AutoSize = true;
            this.checkBox_Aimview.Location = new System.Drawing.Point(187, 107);
            this.checkBox_Aimview.Name = "checkBox_Aimview";
            this.checkBox_Aimview.Size = new System.Drawing.Size(127, 19);
            this.checkBox_Aimview.TabIndex = 19;
            this.checkBox_Aimview.Text = "Show Aimview (F4)";
            this.checkBox_Aimview.UseVisualStyleBackColor = true;
            this.checkBox_Aimview.CheckedChanged += new System.EventHandler(this.checkBox_Aimview_CheckedChanged);
            // 
            // button_Restart
            // 
            this.button_Restart.Location = new System.Drawing.Point(148, 638);
            this.button_Restart.Name = "button_Restart";
            this.button_Restart.Size = new System.Drawing.Size(94, 27);
            this.button_Restart.TabIndex = 18;
            this.button_Restart.Text = "Restart Game";
            this.button_Restart.UseVisualStyleBackColor = true;
            this.button_Restart.Click += new System.EventHandler(this.button_Restart_Click);
            // 
            // checkBox_MapSetup
            // 
            this.checkBox_MapSetup.AutoSize = true;
            this.checkBox_MapSetup.Location = new System.Drawing.Point(38, 132);
            this.checkBox_MapSetup.Name = "checkBox_MapSetup";
            this.checkBox_MapSetup.Size = new System.Drawing.Size(153, 19);
            this.checkBox_MapSetup.TabIndex = 9;
            this.checkBox_MapSetup.Text = "Show Map Setup Helper";
            this.checkBox_MapSetup.UseVisualStyleBackColor = true;
            this.checkBox_MapSetup.CheckedChanged += new System.EventHandler(this.checkBox_MapSetup_CheckedChanged);
            // 
            // checkBox_Loot
            // 
            this.checkBox_Loot.AutoSize = true;
            this.checkBox_Loot.Location = new System.Drawing.Point(38, 107);
            this.checkBox_Loot.Name = "checkBox_Loot";
            this.checkBox_Loot.Size = new System.Drawing.Size(105, 19);
            this.checkBox_Loot.TabIndex = 17;
            this.checkBox_Loot.Text = "Show Loot (F3)";
            this.checkBox_Loot.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(201, 166);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 15);
            this.label1.TabIndex = 16;
            this.label1.Text = "Zoom (F1 in F2 out)";
            // 
            // trackBar_Zoom
            // 
            this.trackBar_Zoom.LargeChange = 1;
            this.trackBar_Zoom.Location = new System.Drawing.Point(237, 185);
            this.trackBar_Zoom.Maximum = 200;
            this.trackBar_Zoom.Minimum = 1;
            this.trackBar_Zoom.Name = "trackBar_Zoom";
            this.trackBar_Zoom.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar_Zoom.Size = new System.Drawing.Size(45, 403);
            this.trackBar_Zoom.TabIndex = 15;
            this.trackBar_Zoom.Value = 100;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(104, 166);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 15);
            this.label2.TabIndex = 13;
            this.label2.Text = "Player Aimline";
            // 
            // trackBar_AimLength
            // 
            this.trackBar_AimLength.LargeChange = 50;
            this.trackBar_AimLength.Location = new System.Drawing.Point(119, 185);
            this.trackBar_AimLength.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.trackBar_AimLength.Maximum = 1000;
            this.trackBar_AimLength.Minimum = 10;
            this.trackBar_AimLength.Name = "trackBar_AimLength";
            this.trackBar_AimLength.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar_AimLength.Size = new System.Drawing.Size(45, 403);
            this.trackBar_AimLength.SmallChange = 5;
            this.trackBar_AimLength.TabIndex = 11;
            this.trackBar_AimLength.Value = 500;
            // 
            // button_Map
            // 
            this.button_Map.Location = new System.Drawing.Point(44, 33);
            this.button_Map.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button_Map.Name = "button_Map";
            this.button_Map.Size = new System.Drawing.Size(107, 27);
            this.button_Map.TabIndex = 7;
            this.button_Map.Text = "Toggle Map (F5)";
            this.button_Map.UseVisualStyleBackColor = true;
            this.button_Map.Click += new System.EventHandler(this.button_Map_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.richTextBox_PlayersInfo);
            this.tabPage3.Location = new System.Drawing.Point(4, 24);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1328, 1033);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "PMC Loadouts";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // richTextBox_PlayersInfo
            // 
            this.richTextBox_PlayersInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox_PlayersInfo.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.richTextBox_PlayersInfo.Location = new System.Drawing.Point(0, 0);
            this.richTextBox_PlayersInfo.Name = "richTextBox_PlayersInfo";
            this.richTextBox_PlayersInfo.ReadOnly = true;
            this.richTextBox_PlayersInfo.Size = new System.Drawing.Size(1328, 1033);
            this.richTextBox_PlayersInfo.TabIndex = 0;
            this.richTextBox_PlayersInfo.Text = "";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.listView_PmcHistory);
            this.tabPage4.Location = new System.Drawing.Point(4, 24);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(1328, 1033);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "PMC History";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // listView_PmcHistory
            // 
            this.listView_PmcHistory.AutoArrange = false;
            this.listView_PmcHistory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader_Entry,
            this.columnHeader_ID});
            this.listView_PmcHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_PmcHistory.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.listView_PmcHistory.FullRowSelect = true;
            this.listView_PmcHistory.GridLines = true;
            this.listView_PmcHistory.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView_PmcHistory.Location = new System.Drawing.Point(0, 0);
            this.listView_PmcHistory.MultiSelect = false;
            this.listView_PmcHistory.Name = "listView_PmcHistory";
            this.listView_PmcHistory.Size = new System.Drawing.Size(1328, 1033);
            this.listView_PmcHistory.TabIndex = 0;
            this.listView_PmcHistory.UseCompatibleStateImageBehavior = false;
            this.listView_PmcHistory.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader_Entry
            // 
            this.columnHeader_Entry.Text = "Entry";
            this.columnHeader_Entry.Width = 200;
            // 
            // columnHeader_ID
            // 
            this.columnHeader_ID.Text = "ID";
            this.columnHeader_ID.Width = 50;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1336, 1061);
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MainForm";
            this.Text = "EFT Radar";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox_MapSetup.ResumeLayout(false);
            this.groupBox_MapSetup.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Zoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_AimLength)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private GroupBox groupBox1;
        private Label label2;
        private TrackBar trackBar_AimLength;
        private Button button_Map;
        private Label label_Pos;
        private Label label1;
        private TrackBar trackBar_Zoom;
        private CheckBox checkBox_Loot;
        private CheckBox checkBox_MapSetup;
        private Button button_Restart;
        private GroupBox groupBox_MapSetup;
        private Button button_MapSetupApply;
        private TextBox textBox_mapScale;
        private Label label5;
        private TextBox textBox_mapY;
        private Label label4;
        private TextBox textBox_mapX;
        private BindingSource bindingSource1;
        private CheckBox checkBox_Aimview;
        private CheckBox checkBox_MapFree;
        private Button button_RefreshLoot;
        private TabPage tabPage3;
        private RichTextBox richTextBox_PlayersInfo;
        private TabPage tabPage4;
        private Button button_watchlist;
        private ListView listView_PmcHistory;
        private ColumnHeader columnHeader_Entry;
        private ColumnHeader columnHeader_ID;
    }
}

