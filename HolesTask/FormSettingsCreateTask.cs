using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HolesTask
{
    public partial class FormSettingsCreateTask : Form
    {

        public FormSettingsCreateTask()
        {
            InitializeComponent();

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;

            this.txtTaskName.Text = Settings.nameTaskWallFamily;
            this.txtWallHoleName.Text = Settings.nameHoleWall;
            this.txtFloorHoleName.Text = Settings.nameHoleFloor;
            double offset = 304.8 * Settings.holeOffset;
            this.numHoleOffset.Value = (decimal)offset;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Settings.nameTaskWallFamily = this.txtTaskName.Text;
            Settings.nameHoleWall = this.txtWallHoleName.Text;
            Settings.nameHoleFloor = this.txtFloorHoleName.Text;
            Settings.holeOffset = (double)this.numHoleOffset.Value / 304.8;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void FormSettingsCreateTask_Load(object sender, EventArgs e)
        {
                
        }
    }
}
