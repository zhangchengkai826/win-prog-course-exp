using System;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zckserver_winform
{
    public struct COPYDATASTRUCT
    {
        public IntPtr dwData;
        public int cbData;
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpData;
    }
    partial class Form1
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
        /// Required method for Designer support.
        /// </summary>
        private void InitializeComponent()
        {
            this.tblLayout = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();

            var lbl = new System.Windows.Forms.Label();
            lbl.Dock = System.Windows.Forms.DockStyle.Fill;
            lbl.Location = new System.Drawing.Point(0, 0);
            lbl.Name = "label1";
            lbl.Text = "服务器端";
            lbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lbl.BackColor = Color.Beige;

            txtBox = new System.Windows.Forms.RichTextBox();
            txtBox.Dock = System.Windows.Forms.DockStyle.Fill;
            txtBox.Location = new System.Drawing.Point(0, 0);
            txtBox.Name = "txtBox";
            txtBox.Text = "";
            txtBox.WordWrap = false;
            txtBox.BackColor = Color.AntiqueWhite;
            txtBox.ReadOnly = true;

            this.tblLayout.ColumnCount = 1;
            this.tblLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblLayout.Location = new System.Drawing.Point(0, 0);
            this.tblLayout.Name = "tableLayoutPanel1";
            this.tblLayout.RowCount = 2;
            this.tblLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50));
            this.tblLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100));
            this.tblLayout.Size = new System.Drawing.Size(800, 450);
            this.tblLayout.Controls.Add(lbl, 0, 0);
            this.tblLayout.Controls.Add(txtBox, 0, 1);

            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tblLayout);
            this.Name = "zckserver-winform-window";
            this.Text = "zck server (winform)";
            this.ResumeLayout(false);

            OnDataRecv += new OnDataRecvHandler(ProcessRecvedData);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblLayout;
        private System.Windows.Forms.RichTextBox txtBox;

        private const int WM_COPYDATA = 0x004A;
        public class OnDataRecvArgs : EventArgs
        {
            public string data;
        }
        public delegate void OnDataRecvHandler(object sender, OnDataRecvArgs args);
        public event OnDataRecvHandler OnDataRecv;
        private void ProcessRecvedData(object sender, OnDataRecvArgs args)
        {
            txtBox.Text += "收到来自客户端的数据： " + args.data + "\n"; ;
        }
        protected override void DefWndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_COPYDATA:
                    var cds = (COPYDATASTRUCT)m.GetLParam(typeof(COPYDATASTRUCT));
                    var txt = cds.lpData;
                    OnDataRecv(this, new OnDataRecvArgs() { data = txt });
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }
    }
}

