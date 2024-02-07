using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace WarehouseApp
{
    public partial class Form1 : Form
    {
        private readonly ShelfManager shelfManager = new ShelfManager();
        private List<User> users;
        private Button btnAddProduct, btnDisplayAllProducts, btnSearchProduct, btnLogin;
        private TextBox txtProductName, txtQuantity, txtSearchProduct, txtUsername, txtPassword;
        private DataGridView dgvProducts;
        private System.ComponentModel.IContainer components;
        private bool isLoggedIn = false; 

        public Form1()
        {
            InitializeComponent();
            InitializeControls();
            CustomizeDataGridView();
            InitializeLoginControls();
            users = LoadUsersFromJson();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 500);
            this.Text = "Warehouse App";
            this.Load += new System.EventHandler(this.Form1_Load);
        }

        private void InitializeControls()
        {
            btnAddProduct = new Button
            {
                Location = new Point(10, 10),
                Size = new Size(120, 30),
                Text = "Dodaj produkt",
                BackColor = Color.LightSkyBlue,
                FlatStyle = FlatStyle.Flat
            };
            btnAddProduct.FlatAppearance.BorderSize = 0;
            btnAddProduct.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            btnAddProduct.Click += btnAddProduct_Click;
            Controls.Add(btnAddProduct);

            txtProductName = new TextBox
            {
                Location = new Point(140, 10),
                Size = new Size(180, 30)
            };
            Controls.Add(txtProductName);

            txtQuantity = new TextBox
            {
                Location = new Point(330, 10),
                Size = new Size(50, 30)
            };
            Controls.Add(txtQuantity);

            btnDisplayAllProducts = new Button
            {
                Location = new Point(390, 10),
                Size = new Size(160, 30),
                Text = "Wyœwietl wszystkie"
            };
            btnDisplayAllProducts.Click += btnDisplayAllProducts_Click;
            Controls.Add(btnDisplayAllProducts);

            txtSearchProduct = new TextBox
            {
                Location = new Point(560, 10),
                Size = new Size(180, 30)
            };
            Controls.Add(txtSearchProduct);

            btnSearchProduct = new Button
            {
                Location = new Point(750, 10),
                Size = new Size(120, 30),
                Text = "Wyszukaj produkt"
            };
            btnSearchProduct.Click += btnSearchProduct_Click;
            Controls.Add(btnSearchProduct);

            dgvProducts = new DataGridView
            {
                Location = new Point(10, 50),
                Size = new Size(860, 390),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true
            };
            dgvProducts.Columns.Add("Name", "Nazwa");
            dgvProducts.Columns.Add("Quantity", "Iloœæ");
            dgvProducts.Columns.Add("ShelfLocation", "Lokalizacja na pó³ce");
            Controls.Add(dgvProducts);
        }

        private void CustomizeDataGridView()
        {
            dgvProducts.BorderStyle = BorderStyle.None;
            dgvProducts.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            dgvProducts.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvProducts.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            dgvProducts.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            dgvProducts.BackgroundColor = Color.White;

            dgvProducts.EnableHeadersVisualStyles = false;
            dgvProducts.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvProducts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(20, 25, 72);
            dgvProducts.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        }

        private void InitializeLoginControls()
        {
            txtUsername = new TextBox { Location = new Point(10, 450), Size = new Size(100, 20) };
            txtPassword = new TextBox { Location = new Point(120, 450), Size = new Size(100, 20) };
            btnLogin = new Button { Location = new Point(230, 450), Size = new Size(75, 23), Text = "Login" };
            btnLogin.Click += BtnLogin_Click;

            Controls.Add(txtUsername);
            Controls.Add(txtPassword);
            Controls.Add(btnLogin);
        }

        private List<User> LoadUsersFromJson()
        {
            string jsonFilePath = "users.json";
            if (File.Exists(jsonFilePath))
            {
                string jsonData = File.ReadAllText(jsonFilePath);
                return JsonConvert.DeserializeObject<List<User>>(jsonData);
            }
            return new List<User>();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                isLoggedIn = true; 
                MessageBox.Show("Login successful!");
            }
            else
            {
                MessageBox.Show("Invalid username or password.");
            }
        }

        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            if (!isLoggedIn)
            {
                MessageBox.Show("Proszê siê zalogowaæ przed dodaniem produktu.");
                return;
            }

            string productName = txtProductName.Text;
            int quantity;
            if (!int.TryParse(txtQuantity.Text, out quantity) || quantity <= 0)
            {
                MessageBox.Show("WprowadŸ poprawn¹ iloœæ produktu.");
                return;
            }
            shelfManager.AddOrUpdateProduct(productName, quantity);
            MessageBox.Show($"Dodano/aktualizowano {quantity} produkt(y) '{productName}' do magazynu.");
            DisplayAllProducts();
        }

        private void btnDisplayAllProducts_Click(object sender, EventArgs e)
        {
            if (!isLoggedIn)
            {
                MessageBox.Show("Proszê siê zalogowaæ przed wyœwietleniem wszystkich produktów.");
                return;
            }

            DisplayAllProducts();
        }

        private void btnSearchProduct_Click(object sender, EventArgs e)
        {
            if (!isLoggedIn)
            {
                MessageBox.Show("Proszê siê zalogowaæ przed wyszukaniem produktu.");
                return;
            }

            string productNameToSearch = txtSearchProduct.Text;
            List<Product> foundProducts = shelfManager.SearchProductsByName(productNameToSearch);
            if (foundProducts.Count > 0)
            {
                dgvProducts.Rows.Clear();
                foreach (var product in foundProducts)
                {
                    var aggregatedLocations = string.Join(", ", product.ShelfLocations);
                    dgvProducts.Rows.Add(product.Name, product.Quantity, aggregatedLocations);
                }
            }
            else
            {
                MessageBox.Show($"Brak produktów o nazwie '{productNameToSearch}'.");
            }
        }

        private void DisplayAllProducts()
        {
            var allProducts = shelfManager.GetAllProducts();
            if (allProducts.Count > 0)
            {
                dgvProducts.Rows.Clear();
                foreach (var product in allProducts)
                {
                    var aggregatedLocations = string.Join(", ", product.ShelfLocations);
                    dgvProducts.Rows.Add(product.Name, product.Quantity, aggregatedLocations);
                }
            }
            else
            {
                MessageBox.Show("Brak produktów w magazynie.");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
    }
}
