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
    public partial class MainForm : Form
    {
        // Déclaration des contrôles (éléments visuels de la fenêtre)
        private DataGridView dgvCustomers = null!; // tableau pour afficher les clients
        private DataGridView dgvOrders = null!; // tableau pour afficher les commandes
        private Button btnAddCust = null!; 
        private Button btnDelCust = null!; 
        private Button btnAddOrder = null!; 
        private Button btnDelOrder = null!; 
        private Label lblOrders = null!;
        private int? _custId;         

        public MainForm()
        {
            // Propriétés de la fenêtre principale
            Text = "ErpDemo — Clients et Commandes";
            Size = new Size(1100, 650);     // largeur x hauteur en pixels
            MinimumSize = new Size(800, 500);      // taille minimale
            StartPosition = FormStartPosition.CenterScreen; // centré à l'écran
            Font = new Font("Segoe UI", 9f); // police par défaut

            Build();         // construit l'interface
            LoadCustomers(); // charge les clients depuis SQL Server
        }

        // Construit tous les éléments visuels de la fenêtre
        private void Build()
        {
            // Barre de statut en bas de la fenêtre (affiche des messages)
            var status = new StatusStrip();
            var statusLbl = new ToolStripStatusLabel("Prêt");
            status.Items.Add(statusLbl);
            Controls.Add(status); // ajoute à la fenêtre

            // SplitContainer = divise la fenêtre en deux panneaux côte à côte
            // Panel1 = gauche (clients), Panel2 = droite (commandes)
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill, // remplit toute la fenêtre
                SplitterDistance = 85            // largeur du panneau gauche en pixels
            };

            // ── PANNEAU GAUCHE : Clients ──────────────────────────────

            // En-tête bleu "Clients"
            var headerL = MakeHeader("Clients");

            // Boutons ajouter / supprimer client
            btnAddCust = new Button
            {
                Text = "+ Ajouter", // texte affiché sur le bouton
                Width = 90,
                Height = 28,
                Left = 6,  // position horizontale dans le panneau
                Top = 5   // position verticale dans le panneau
            };
            btnDelCust = new Button
            {
                Text = "Supprimer",
                Width = 90,
                Height = 28,
                Left = 102,
                Top = 5,
                ForeColor = Color.Firebrick // texte rouge pour indiquer danger
            };

            // Quand on clique les boutons, appelle ces méthodes
            btnAddCust.Click += (_, _) => OnAddCustomer();
            btnDelCust.Click += (_, _) => OnDeleteCustomer();

            // Panel qui contient les boutons (barre d'outils)
            var toolbarLeft = new Panel { Dock = DockStyle.Top, Height = 38 };
            toolbarLeft.Controls.AddRange(new Control[] { btnAddCust, btnDelCust });

            // Le DataGridView = tableau qui affiche les clients
            dgvCustomers = MakeGrid();

            // EVENT CLÉ WINFORMS : quand l'utilisateur clique sur une ligne du tableau,
            // SelectionChanged se déclenche automatiquement
            // → on charge les commandes du client sélectionné
            dgvCustomers.SelectionChanged += (_, _) => OnCustomerSelected();

            // Ajoute les contrôles au panneau gauche
            // L'ordre est important avec DockStyle.Top : le dernier ajouté est en haut
            split.Panel1.Controls.Add(dgvCustomers);   // remplit le reste
            split.Panel1.Controls.Add(toolbarLeft);    // s'ancre en haut
            split.Panel1.Controls.Add(headerL);        // s'ancre tout en haut

            // ── PANNEAU DROIT : Commandes ─────────────────────────────

            lblOrders = MakeHeader("Commandes — sélectionne un client");

            btnAddOrder = new Button
            {
                Text = "+ Ajouter",
                Width = 90,
                Height = 28,
                Left = 6,
                Top = 5,
                Enabled = false // désactivé tant qu'aucun client n'est sélectionné
            };
            btnDelOrder = new Button
            {
                Text = "Supprimer",
                Width = 90,
                Height = 28,
                Left = 102,
                Top = 5,
                ForeColor = Color.Firebrick
            };

            btnAddOrder.Click += (_, _) => OnAddOrder();
            btnDelOrder.Click += (_, _) => OnDeleteOrder();

            var toolbarRight = new Panel { Dock = DockStyle.Top, Height = 38 };
            toolbarRight.Controls.AddRange(new Control[] { btnAddOrder, btnDelOrder });

            dgvOrders = MakeGrid();

            split.Panel2.Controls.Add(dgvOrders);
            split.Panel2.Controls.Add(toolbarRight);
            split.Panel2.Controls.Add(lblOrders);

            Controls.Add(split); // ajoute le SplitContainer à la fenêtre
        }

        // Crée un en-tête bleu réutilisable
        private static Label MakeHeader(string text) => new()
        {
            Text = "  " + text,
            Dock = DockStyle.Top, // s'ancre en haut du conteneur
            Height = 32,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            BackColor = Color.FromArgb(0, 102, 204), // bleu
            ForeColor = Color.White
        };

        // Crée un DataGridView (tableau) avec les réglages standard
        private static DataGridView MakeGrid() => new()
        {
            Dock = DockStyle.Fill,  // remplit tout l'espace disponible
            SelectionMode = DataGridViewSelectionMode.FullRowSelect, // sélectionne toute la ligne
            MultiSelect = false,            // une seule ligne à la fois
            ReadOnly = true,             // l'utilisateur ne peut pas modifier directement
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
            AllowUserToAddRows = false,            // pas de ligne vide en bas
            AllowUserToDeleteRows = false,            // pas de suppression par touche Suppr
            RowHeadersVisible = false,            // cache la colonne grise à gauche
            BorderStyle = BorderStyle.None, // pas de bordure
            BackgroundColor = SystemColors.Window
        };

        // ── Chargement des données depuis SQL Server ──────────────────

        private void LoadCustomers()
        {
            try
            {
                // DataSource = lie la liste au tableau, il se met à jour automatiquement
                dgvCustomers.DataSource = CustomerRepository.GetAll();

                // Cache la colonne Id (technique, pas utile à l'utilisateur)
                if (dgvCustomers.Columns["Id"] is { } col)
                    col.Visible = false;
            }
            catch (Exception ex)
            {
                // Affiche une boîte d'erreur si SQL Server n'est pas accessible
                MessageBox.Show("Erreur SQL:\n" + ex.Message, "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadOrders(int customerId)
        {
            try
            {
                dgvOrders.DataSource = OrderRepository.GetByCustomer(customerId);


                // Cache les colonnes techniques
                if (dgvOrders.Columns["Id"] is { } c0) c0.Visible = false;
                if (dgvOrders.Columns["CustomerId"] is { } c1) c1.Visible = false;

                // Formate le montant avec 2 décimales : 1500.00
                if (dgvOrders.Columns["Amount"] is { } c2)
                {
                    c2.DefaultCellStyle.Format = "N2";
                    c2.HeaderText = "Montant";
                }
       
                // Formate la date : 15/01/2024
                if (dgvOrders.Columns["OrderDate"] is { } c3)
                {
                    c3.HeaderText = "Date";
                    c3.DefaultCellStyle.Format = "dd/MM/yyyy";
                    c3.MinimumWidth = 80;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur SQL:\n" + ex.Message, "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Gestionnaires d'événements ────────────────────────────────

        // Appelé automatiquement quand l'utilisateur clique sur une ligne du tableau clients
        private void OnCustomerSelected()
        {
            // DataBoundItem = l'objet Customer lié à la ligne sélectionnée
            if (dgvCustomers.CurrentRow?.DataBoundItem is Customer c)
            {
                _custId = c.Id;   // mémorise le client sélectionné
                btnAddOrder.Enabled = true;   // active le bouton "+ Ajouter commande"
                lblOrders.Text = $"  Commandes — {c.Name}"; // met à jour le titre
                LoadOrders(c.Id);             // charge ses commandes
            }
        }

        private void OnAddCustomer()
        {
            // ShowDialog = ouvre la fenêtre en mode modal (bloque la fenêtre principale)
            // DialogResult.OK = l'utilisateur a cliqué Enregistrer
            using var f = new AddCustomerForm();
            if (f.ShowDialog(this) == DialogResult.OK)
                LoadCustomers(); // recharge la liste après ajout
        }

        private void OnDeleteCustomer()
        {
            // Récupère le client de la ligne sélectionnée
            if (dgvCustomers.CurrentRow?.DataBoundItem is not Customer c) return;

            // Demande confirmation avant de supprimer
            if (MessageBox.Show($"Supprimer '{c.Name}' et toutes ses commandes ?",
                "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                CustomerRepository.Delete(c.Id);
                _custId = null;
                btnAddOrder.Enabled = false;
                dgvOrders.DataSource = null;
                lblOrders.Text = "  Commandes — sélectionne un client";
                LoadCustomers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnAddOrder()
        {
            if (_custId is null) return; // sécurité : ne rien faire si aucun client sélectionné
            using var f = new AddOrderForm(_custId.Value);
            if (f.ShowDialog(this) == DialogResult.OK)
                LoadOrders(_custId.Value);
        }

        private void OnDeleteOrder()
        {
            if (dgvOrders.CurrentRow?.DataBoundItem is not Order o) return;
            if (MessageBox.Show($"Supprimer '{o.Reference}' ?", "Confirmation",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                OrderRepository.Delete(o.Id);
                if (_custId.HasValue) LoadOrders(_custId.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
