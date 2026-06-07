using ErpDemo.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static ErpDemo.Models;

namespace ErpDemo.Forms
{
    public partial class AddCustomerForm : Form
    {
        // Champs de saisie
        private TextBox txtName = null!;
        private TextBox txtEmail = null!;
        private TextBox txtPhone = null!;

        public AddCustomerForm()
        {
            Text = "Ajouter un client";
            Size = new Size(380, 220);
            MaximumSize = Size;              // taille fixe, pas redimensionnable
            MinimumSize = Size;
            StartPosition = FormStartPosition.CenterParent; // centré sur la fenêtre parent
            FormBorderStyle = FormBorderStyle.FixedDialog;    // sans bouton maximiser
            MaximizeBox = false;

            // TableLayoutPanel = grille pour aligner les labels et les champs
            // 2 colonnes : labels à gauche, champs à droite
            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12), // marge intérieure
                ColumnCount = 2,
                RowCount = 5
            };
            // Colonne gauche : largeur fixe pour les labels
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
            // Colonne droite : prend tout l'espace restant
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // Crée les champs de saisie
            txtName = new TextBox { Dock = DockStyle.Fill };
            txtEmail = new TextBox { Dock = DockStyle.Fill };
            txtPhone = new TextBox { Dock = DockStyle.Fill };

            // Ajoute chaque ligne label + champ dans la grille
            AddRow(tbl, 0, "Nom *", txtName);
            AddRow(tbl, 1, "Email", txtEmail);
            AddRow(tbl, 2, "Téléphone", txtPhone);

            var btnSave = new Button
            {
                Text = "Enregistrer",
                Width = 110,
                Height = 28
            };
            var btnCancel = new Button
            {
                Text = "Annuler",
                Width = 85,
                Height = 28,
                DialogResult = DialogResult.Cancel // ferme la fenêtre avec résultat Cancel
            };
            btnSave.Click += OnSave;

            // FlowLayoutPanel = aligne les boutons automatiquement de droite à gauche
            var bp = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 8, 0, 0)
            };
            bp.Controls.AddRange(new Control[] { btnCancel, btnSave });
            tbl.Controls.Add(bp, 1, 4); // ajoute dans la colonne 1, ligne 4

            Controls.Add(tbl);
            AcceptButton = btnSave;   // Entrée = clic sur Enregistrer
            CancelButton = btnCancel; // Échap = clic sur Annuler
            ActiveControl = txtName;   // le curseur commence dans le champ Nom
        }

        // Ajoute une ligne (label + champ) dans la grille
        private static void AddRow(TableLayoutPanel t, int row, string label, Control ctrl)
        {
            t.Controls.Add(new Label
            {
                Text = label,
                TextAlign = ContentAlignment.MiddleRight, // texte aligné à droite
                Dock = DockStyle.Fill
            }, 0, row); // colonne 0 = gauche
            t.Controls.Add(ctrl, 1, row); // colonne 1 = droite
        }

        // Appelé quand l'utilisateur clique Enregistrer
        private void OnSave(object? sender, EventArgs e)
        {
            // Validation : le nom est obligatoire
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Le nom est obligatoire.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // stop, ne ferme pas la fenêtre
            }
            try
            {
                // Crée et insère le client en BDD
                CustomerRepository.Add(new Customer
                {
                    Name = txtName.Text.Trim(),  // .Trim() supprime les espaces en trop
                    Email = txtEmail.Text.Trim(),
                    Phone = txtPhone.Text.Trim()
                });
                DialogResult = DialogResult.OK; // signale à MainForm que ça a marché
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erreur SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

}
