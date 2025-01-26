using DnsClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace ParaleloEjercicio1
{  
    public partial class ExportFilterForm : Form
    {
        public string QueryType { get; private set; } // "all", "price", "brand_or_model"
        public string PriceComparison { get; private set; } // "greater" o "less"
        public double? PriceValue { get; private set; } // Precio
        public string Brand { get; private set; } // Marca
        public string Model { get; private set; } // Modelo

        public ExportFilterForm()
        {
            InitializeComponent();

            // Eventos de cambio de selección en los RadioButtons
            rdbAll.CheckedChanged += (s, e) => ToggleControls("all");
            rdbPrice.CheckedChanged += (s, e) => ToggleControls("price");
            rdbBrandModel.CheckedChanged += (s, e) => ToggleControls("brand_or_model");

            // Valores predeterminados
            QueryType = "all";
        }
        private void ToggleControls(string type)
        {
            // Restablecer controles
            lblPrice.Visible = numPrice.Visible = type == "price";
            lblBrand.Visible = txtBrand.Visible = lblModel.Visible = txtModel.Visible = type == "brand_or_model";

            QueryType = type;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                // Validar valores seleccionados
                if (rdbPrice.Checked)
                {
                    PriceComparison = "greater"; // Puedes agregar lógica para elegir "greater" o "less"
                    PriceValue = (double)numPrice.Value;
                }
                else if (rdbBrandModel.Checked)
                {
                    Brand = txtBrand.Text.Trim();
                    Model = txtModel.Text.Trim();

                    if (string.IsNullOrWhiteSpace(Brand) && string.IsNullOrWhiteSpace(Model))
                    {
                        MessageBox.Show("Por favor, ingrese al menos Marca o Modelo.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.Cancel = true;
                    }
                }
            }

            base.OnFormClosing(e);
        }

        private void ExportFilterForm_Load(object sender, EventArgs e)
        {

        }
    }
}
