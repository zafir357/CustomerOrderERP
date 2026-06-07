using ErpDemo.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using static ErpDemo.Models;

namespace ErpDemo.Forms
{
    public partial class AddOrderForm : Form
    {
        private readonly int _customerId; // Id du client pour lequel on crée la commande
        private TextBox txtRef = null!;
        private TextBox txtAmount = null!;
        private ComboBox cmbStatus = null!; // liste déroulante pour le statut

        // Le constructeur reçoit l'Id du client sélectionné
        public AddOrderForm(int customerId)
        {
            InitializeComponent();
            _customerId = customerId;
            Text = "Ajouter une commande";
            Size = new Size(380, 240);
            MaximumSize = Size;
            MinimumSize = Size;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12),
                ColumnCount = 2,
                RowCount = 5
            };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // Référence auto-générée mais modifiable
            txtRef = new TextBox
            {
                Dock = DockStyle.Fill,
                Text = $"CMD-{DateTime.Now:yyyy-MM}-{Random.Shared.Next(100, 999)}"
            };
            txtAmount = new TextBox { Dock = DockStyle.Fill, Text = "0.00" };

            // ComboBox = liste déroulante avec valeurs fixes
            cmbStatus = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList // lecture seule, pas de saisie libre
            };
            cmbStatus.Items.AddRange(new[] { "Pending", "Shipping", "Paid", "Cancelled" });
            cmbStatus.SelectedIndex = 0; // sélectionne "Pending" par défaut

            AddRow(tbl, 0, "Référence *", txtRef);
            AddRow(tbl, 1, "Montant € *", txtAmount);
            AddRow(tbl, 2, "Statut", cmbStatus);

            var btnSave = new Button { Text = "Enregistrer", Width = 110, Height = 28 };
            var btnCancel = new Button
            {
                Text = "Annuler",
                Width = 85,
                Height = 28,
                DialogResult = DialogResult.Cancel
            };
            btnSave.Click += OnSave;

            var bp = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 8, 0, 0)
            };
            bp.Controls.AddRange(new Control[] { btnCancel, btnSave });
            tbl.Controls.Add(bp, 1, 4);

            Controls.Add(tbl);
            AcceptButton = btnSave;
            CancelButton = btnCancel;
            ActiveControl = txtRef;
        }

        private static void AddRow(TableLayoutPanel t, int row, string label, Control ctrl)
        {
            t.Controls.Add(new Label
            {
                Text = label,
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill
            }, 0, row);
            t.Controls.Add(ctrl, 1, row);
        }

        private void OnSave(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRef.Text))
            {
                MessageBox.Show("La référence est obligatoire.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Parse le montant (accepte virgule ou point décimal)
            var amountStr = txtAmount.Text.Replace(",", ".");
            if (!decimal.TryParse(amountStr, NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture, out var amount) || amount < 0)
            {
                MessageBox.Show("Montant invalide. Exemple : 1500.00", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAmount.Focus();
                return;
            }

            try
            {
                OrderRepository.Add(new Order
                {
                    CustomerId = _customerId, // le client sélectionné dans MainForm
                    Reference = txtRef.Text.Trim(),
                    Amount = amount,
                    Status = cmbStatus.SelectedItem?.ToString() ?? "Pending"
                });
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erreur SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
