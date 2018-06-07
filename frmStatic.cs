using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using QuickToggl;

namespace TogglPlus
{
    public partial class frmStatic : Form
    {
        private InteractWithToggl _iwt;
        private ButtonOptionList _buttonDefns;
        private ComboBox _buckets;
        private Timer timer;
        private TimeSpan timerStarted;

        public frmStatic()
        {
            InitializeComponent();

            //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            // Position in Toolbar
            //this.StartPosition = FormStartPosition.Manual;
            //this.Location = new Point(Screen.PrimaryScreen.Bounds.Width - 650, Screen.PrimaryScreen.Bounds.Height - 82);

            CenterToScreen();
            this.Location = new Point(this.Location.X, 0);

            // Always on Top
            TopMost = true;

            //this.BackColor = Color.LimeGreen;
            //this.TransparencyKey = Color.LimeGreen;

            // setup timer
            timerStarted = TimeSpan.FromSeconds(0);
            label1.Text = timerStarted.ToString();

            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += new System.EventHandler(this.timer1_Tick);
        }

        private void frmStatic_Load(object sender, EventArgs e)
        {
            _iwt = new InteractWithToggl();

            _buttonDefns = new NicksButtonOptionsList();

            _buckets = new ComboBox();
            this._buckets.DropDownStyle = ComboBoxStyle.DropDownList;
            this._buckets.Name = "Buckets";
            this._buckets.Size = new Size(275, 28);
            this._buckets.TabIndex = 0;
            _buckets.Items.AddRange(new string[] { "" });
            _buckets.Items.AddRange(_buttonDefns.Buttons.Select(x => x.Text).Where(x => x != "Stop").ToArray());

            _buckets.SelectedIndexChanged += new System.EventHandler(_buckets_SelectIndexChanged);

            flowLayoutPanel1.Controls.Add(_buckets);

            //var btn = new Button
            //{
            //    Text = "Stop",
            //    BackColor = Color.LightGray,
            //    Size = new Size(125, 28)
            //};

            //btn.Click += new System.EventHandler(this.button_Click);
            //flowLayoutPanel1.Controls.Add(btn);

            // Apply opacity (0 to 255)
            flowLayoutPanel1.BackColor = Color.FromArgb(180, flowLayoutPanel1.BackColor);
        }

        private void button_Click(object sender, EventArgs e)
        {
            _iwt.StopCurrentTask();
            timer.Stop();
            timerStarted = TimeSpan.FromSeconds(0);
            label1.Text = timerStarted.ToString();
        }

        private void _buckets_SelectIndexChanged(object sender, EventArgs e)
        {
            var combobox = (ComboBox)sender;

            if (combobox.Text == "")
            {
                // this is the blank stop button
                _iwt.StopCurrentTask();
                timer.Stop();
                timerStarted = TimeSpan.FromSeconds(0);
                label1.Text = timerStarted.ToString();
                return;
            }

            var matchingButtonDefn = _buttonDefns.Buttons.FirstOrDefault(x => x.Text == combobox.Text);
            if (matchingButtonDefn != null)
            {
                string input = "";

                if (matchingButtonDefn.RequiresInput)
                {
                    while (input.Length == 0)
                    {
                        input = Microsoft.VisualBasic.Interaction.InputBox($"Task type {matchingButtonDefn.Text} REQUIRES input:",
                            "Required Input",
                            input);
                    }
                }
                else if (matchingButtonDefn.RecommendsInput)
                {
                    input = Microsoft.VisualBasic.Interaction.InputBox($"Task type {matchingButtonDefn.Text} RECOMMENDS input:",
                        "Recommended Input",
                        input);

                }

                var newTaskName = input.Length > 0 ? $"{matchingButtonDefn.Text} {input}" : matchingButtonDefn.Text;

                _iwt.StopCurrentTask();

                // reset timer
                timer.Stop();
                timerStarted = TimeSpan.FromSeconds(0);
                label1.Text = timerStarted.ToString();

                _iwt.StartNewTask(newTaskName, matchingButtonDefn.Project);
                timer.Start();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (sender == timer)
            {
                timerStarted = timerStarted.Add(TimeSpan.FromSeconds(1));
                label1.Text = timerStarted.ToString();
            }
        }
    }

    class NicksButtonOptionsList : ButtonOptionList
    {
        public NicksButtonOptionsList()
        {
            Buttons = new List<ButtonOption>();

            Buttons.Add(new ButtonOption()
            {
                BackColor = Color.Orange,
                RecommendsInput = true,
                Text = "Ticket",
                Project = "Maintenance"
            });
            Buttons.Add(new ButtonOption()
            {
                BackColor = Color.Orange,
                RecommendsInput = false,
                Text = "Releasing",
                Project = "Maintenance"
            });
            Buttons.Add(new ButtonOption()
            {
                BackColor = Color.LightGreen,
                RequiresInput = false,
                RecommendsInput = true,
                Text = "Sprinting",
                Project = "Velocity"
            });
            Buttons.Add(new ButtonOption()
            {
                BackColor = Color.LightGreen,
                RecommendsInput = false,
                Text = "Sprint Meeting",
                Project = "Other Velocity"
            });
            Buttons.Add(new ButtonOption()
            {
                BackColor = Color.MediumPurple,
                RecommendsInput = false,
                Text = "Lunch",
                Project = "Common"
            });
            Buttons.Add(new ButtonOption()
            {
                BackColor = Color.DodgerBlue,
                RecommendsInput = false,
                Text = "Meeting",
                Project = "Common"
            });
            Buttons.Add(new ButtonOption()
            {
                BackColor = Color.DodgerBlue,
                RecommendsInput = false,
                Text = "Misc Tasks",
                Project = "Common"
            });
        }
    }
}
