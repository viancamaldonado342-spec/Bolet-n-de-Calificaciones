using System.Globalization;

namespace BoletíndeCalificaciones
{
    public partial class Form1 : Form
    {
        private const double NotaMinimaAprobatoria = 70;
        private const double NotaMinimaRecuperacion = 60;
        private const int CantidadPeriodos = 4;

        private double sumaPromedios;
        private int cantidadAprobadas;
        private int cantidadRecuperacion;
        private int cantidadReprobadas;

        public Form1()
        {
            InitializeComponent();
        }

        // Evento: valida los datos y agrega la asignatura al boletín.
        private void btnRegistrar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEstudiante.Text))
            {
                MessageBox.Show("Escriba el nombre del estudiante.", "Dato requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEstudiante.Focus();
                return;
            }

            if (!rdbMatutina.Checked && !rdbVespertina.Checked)
            {
                MessageBox.Show("Seleccione la tanda del estudiante.", "Dato requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbAsignatura.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccione una asignatura.", "Dato requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbAsignatura.Focus();
                return;
            }

            if (!ValidarCalificacion(txtP1, "P1", out double p1) ||
                !ValidarCalificacion(txtP2, "P2", out double p2) ||
                !ValidarCalificacion(txtP3, "P3", out double p3) ||
                !ValidarCalificacion(txtP4, "P4", out double p4))
            {
                return;
            }

            double promedio = (p1 + p2 + p3 + p4) / CantidadPeriodos;
            string situacion;

            if (!chkAsistencia.Checked)
            {
                situacion = "Reprobada por asistencia";
                cantidadReprobadas++;
            }
            else if (promedio >= NotaMinimaAprobatoria)
            {
                situacion = "Aprobada";
                cantidadAprobadas++;
            }
            else if (promedio >= NotaMinimaRecuperacion)
            {
                situacion = "En recuperación";
                cantidadRecuperacion++;
            }
            else
            {
                situacion = "Reprobada";
                cantidadReprobadas++;
            }

            sumaPromedios += promedio;
            string nombreAsignatura = cmbAsignatura.SelectedItem!.ToString()!;
            lstBoletin.Items.Add($"{nombreAsignatura} · {promedio.ToString("F2", CultureInfo.CurrentCulture)} · {situacion}");

            txtP1.Clear();
            txtP2.Clear();
            txtP3.Clear();
            txtP4.Clear();
            chkAsistencia.Checked = false;
            cmbAsignatura.SelectedIndex = -1;
            cmbAsignatura.Focus();
        }

        // Evento: calcula y muestra el resumen de las asignaturas registradas.
        private void btnResumen_Click(object? sender, EventArgs e)
        {
            if (lstBoletin.Items.Count == 0)
            {
                MessageBox.Show("Registre al menos una asignatura antes de mostrar el resumen.", "Sin asignaturas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            double promedioGeneral = sumaPromedios / lstBoletin.Items.Count;
            lblPromedioGeneral.Text = promedioGeneral.ToString("F2", CultureInfo.CurrentCulture);
            lblAprobadas.Text = cantidadAprobadas.ToString(CultureInfo.CurrentCulture);
            lblRecuperacion.Text = cantidadRecuperacion.ToString(CultureInfo.CurrentCulture);
            lblReprobadas.Text = cantidadReprobadas.ToString(CultureInfo.CurrentCulture);

            if (cantidadReprobadas > 0)
            {
                lblSituacion.Text = "Tiene asignaturas reprobadas";
            }
            else if (cantidadRecuperacion > 0)
            {
                lblSituacion.Text = "Tiene asignaturas en recuperación";
            }
            else
            {
                lblSituacion.Text = "Aprobó todas las asignaturas";
            }
        }

        // Evento: restablece los campos, el boletín, los resultados y los acumuladores.
        private void btnLimpiar_Click(object? sender, EventArgs e)
        {
            txtEstudiante.Clear();
            rdbMatutina.Checked = false;
            rdbVespertina.Checked = false;
            cmbAsignatura.SelectedIndex = -1;
            txtP1.Clear();
            txtP2.Clear();
            txtP3.Clear();
            txtP4.Clear();
            chkAsistencia.Checked = false;
            lstBoletin.Items.Clear();

            sumaPromedios = 0;
            cantidadAprobadas = 0;
            cantidadRecuperacion = 0;
            cantidadReprobadas = 0;

            lblPromedioGeneral.Text = "—";
            lblAprobadas.Text = "0";
            lblRecuperacion.Text = "0";
            lblReprobadas.Text = "0";
            lblSituacion.Text = "—";
            txtEstudiante.Focus();
        }

        // Evento: cierra la ventana de la aplicación.
        private void btnSalir_Click(object? sender, EventArgs e)
        {
            Close();
        }

        private static bool ValidarCalificacion(TextBox campo, string periodo, out double calificacion)
        {
            if (!double.TryParse(campo.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out calificacion) ||
                calificacion < 0 || calificacion > 100)
            {
                MessageBox.Show($"Ingrese una calificación numérica de 0 a 100 para {periodo}.", "Calificación inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                campo.Focus();
                campo.SelectAll();
                return false;
            }

            return true;
        }
    }
}
