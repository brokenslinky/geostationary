using Geostationary;
using System;
using System.Windows.Forms;

namespace Geostationary
{
    public partial class GeostationaryForm : Form
    {

        public GeostationaryForm()
        {
            InitializeComponent();
            apoapsisInput.Text = "50000";
            periapsisInput.Text = "200";
            inclinationInput.Text = "30";
            AoPinput.Text = "90";
        }

        private void to_geostationary_Click(object sender, EventArgs e)
        {
            double increment_angle = Math.PI / 16.0;
            double increment_thrust = 1.0 / 64.0;
            double start_apoapsis = Convert.ToDouble(apoapsisInput.Text) + 6378.1;
            double start_periapsis = Convert.ToDouble(periapsisInput.Text) + 6378.1;
            double start_inclination = Convert.ToDouble(inclinationInput.Text) * Math.PI / 180.0;
            double start_AoP = Convert.ToDouble(AoPinput.Text) * Math.PI / 180.0;
            Orbit start_orbit = new Orbit(
                apoapsis: start_apoapsis, periapsis: start_periapsis, 
                inclination: start_inclination, argumentOfPeriapsis: start_AoP);
            double best_delta_v = 32768.0;
            Orbit initial_prograde = new Orbit();
            Orbit inclination_scrub = new Orbit();
            Orbit inclination_prograde = new Orbit();
            Orbit geostationary = new Orbit();
            Orbit best_initial_prograde = new Orbit();
            Orbit best_inclination_scrub = new Orbit();
            Orbit best_inclination_prograde = new Orbit();
            Orbit best_geostationary = new Orbit();
            double mu = Orbit.mu;

            for (double start_true_anomaly = 0.0; start_true_anomaly <= Math.PI; start_true_anomaly += increment_angle)
            {
                double start_altitude = 2.0 * start_orbit.apoapsis * start_orbit.periapsis / (start_orbit.apoapsis * (
                    1.0 + Math.Cos(start_true_anomaly)) + start_orbit.periapsis * (1.0 - Math.Cos(start_true_anomaly)));
                double start_speed = Math.Sqrt(2.0 * mu * (1.0 / start_altitude - 1.0 / (start_orbit.periapsis + start_orbit.apoapsis)));
                double max_initial_thrust = Math.Sqrt(2.0 * mu / start_altitude) - start_speed; // escape velocity
                for (double initial_thrust = 0.0; initial_thrust < max_initial_thrust; initial_thrust += increment_thrust)
                {
                    initial_prograde = start_orbit.Prograde(start_true_anomaly, initial_thrust);
                    inclination_scrub = initial_prograde.ScrubInclination();
                    double altitude_of_scrub = 2.0 * inclination_scrub.apoapsis * inclination_scrub.periapsis / (
                        inclination_scrub.apoapsis * (1.0 + Math.Cos(inclination_scrub.trueAnomalyOut)) + inclination_scrub.periapsis * (
                        1.0 - Math.Cos(inclination_scrub.trueAnomalyOut)));
                    double v = Math.Sqrt(2.0 * mu * (1.0 / altitude_of_scrub - 1.0 / (inclination_scrub.periapsis + inclination_scrub.apoapsis)));
                    for (double prograde_amount = increment_thrust - v; prograde_amount < Math.Sqrt(2.0 * mu / altitude_of_scrub) - v; prograde_amount += increment_thrust)
                    {
                        inclination_prograde = inclination_scrub.Prograde(inclination_scrub.trueAnomalyOut, prograde_amount);
                        geostationary = inclination_prograde.ToCircularGeosynchronous();
                        double delta_v = Math.Abs(initial_thrust) + Math.Sqrt(
                            inclination_scrub.lastDeltaV * inclination_scrub.lastDeltaV +
                            prograde_amount * prograde_amount) + Math.Abs(geostationary.lastDeltaV);
                        if (delta_v < best_delta_v)
                        {
                            best_delta_v = delta_v;
                            best_initial_prograde = initial_prograde;
                            best_inclination_scrub = inclination_scrub;
                            best_inclination_prograde = inclination_prograde;
                            best_geostationary = geostationary;
                        }
                    }
                }
            }
            Refine(start_orbit, best_initial_prograde, best_inclination_scrub, best_inclination_prograde, best_geostationary, increment_angle, increment_thrust);
        }

        public void Refine(Orbit start_orbit, Orbit best_initial_prograde, Orbit best_inclination_scrub, Orbit best_inclination_prograde, Orbit best_geostationary, double previous_increment_angle, double previous_increment_thrust)
        {
            double best_delta_v = 32768.0;
            Orbit initial_prograde = new Orbit();
            Orbit inclination_scrub = new Orbit();
            Orbit inclination_prograde = new Orbit();
            Orbit geostationary = new Orbit();
            double increment_angle = previous_increment_angle / 64.0;
            double increment_thrust = previous_increment_thrust / 64.0;
            double mu = Orbit.mu;
            for (double start_true_anomaly = best_initial_prograde.trueAnomalyIn - previous_increment_angle; start_true_anomaly <= best_initial_prograde.trueAnomalyIn + previous_increment_angle; start_true_anomaly += increment_angle)
            {
                if (start_true_anomaly > Math.PI)
                    break;
                double start_altitude = 2.0 * start_orbit.apoapsis * start_orbit.periapsis / (start_orbit.apoapsis * (
                    1.0 + Math.Cos(start_true_anomaly)) + start_orbit.periapsis * (1.0 - Math.Cos(start_true_anomaly)));
                double start_speed = Math.Sqrt(2.0 * mu * (1.0 / start_altitude - 1.0 / (start_orbit.periapsis + start_orbit.apoapsis)));
                double max_initial_thrust = best_initial_prograde.lastDeltaV + previous_increment_thrust;
                if (max_initial_thrust > Math.Sqrt(2.0 * mu / start_altitude) - start_speed)
                    max_initial_thrust = Math.Sqrt(2.0 * mu / start_altitude) - start_speed; // escape velocity
                for (double initial_thrust = best_initial_prograde.lastDeltaV - previous_increment_thrust; initial_thrust < max_initial_thrust; initial_thrust += increment_thrust)
                {
                    initial_prograde = start_orbit.Prograde(start_true_anomaly, initial_thrust);
                    inclination_scrub = initial_prograde.ScrubInclination();
                    double altitude_of_scrub = 2.0 * inclination_scrub.apoapsis * inclination_scrub.periapsis / (
                        inclination_scrub.apoapsis * (1.0 + Math.Cos(inclination_scrub.trueAnomalyOut)) + inclination_scrub.periapsis * (
                        1.0 - Math.Cos(inclination_scrub.trueAnomalyOut)));
                    double v = Math.Sqrt(2.0 * mu * (1.0 / altitude_of_scrub - 1.0 / (inclination_scrub.periapsis + inclination_scrub.apoapsis)));
                    for (double prograde_amount = best_inclination_prograde.lastDeltaV - previous_increment_thrust; prograde_amount < best_inclination_prograde.lastDeltaV + previous_increment_thrust; prograde_amount += increment_thrust)
                    {
                        inclination_prograde = inclination_scrub.Prograde(inclination_scrub.trueAnomalyOut, prograde_amount);
                        inclination_prograde.trueAnomalyOut = 2.0 * Math.PI - inclination_prograde.trueAnomalyOut;
                        inclination_prograde.relativeRotation = inclination_scrub.relativeRotation + inclination_prograde.trueAnomalyOut - inclination_prograde.trueAnomalyIn;
                        geostationary = inclination_prograde.ToCircularGeosynchronous();
                        double delta_v = Math.Abs(initial_thrust) + Math.Sqrt(
                            inclination_scrub.lastDeltaV * inclination_scrub.lastDeltaV + 
                            prograde_amount * prograde_amount) +
                            Math.Abs(geostationary.lastDeltaV);
                        if (delta_v < best_delta_v)
                        {
                            best_delta_v = delta_v;
                            best_initial_prograde = initial_prograde;
                            best_inclination_scrub = inclination_scrub;
                            best_inclination_prograde = inclination_prograde;
                            best_geostationary = geostationary;
                        }
                    }
                }
            }
            
            for(double true_anomaly = 0.0; true_anomaly <= 2.0 * Math.PI; true_anomaly += Math.PI/ 512.0)
            {
                double c = Math.Cos(true_anomaly);
                double s = Math.Sin(true_anomaly);
                double r1 = 2.0 * start_orbit.apoapsis * start_orbit.periapsis / (start_orbit.apoapsis * (1.0 + c) + start_orbit.periapsis * (1.0 - c));
                this.chart1.Series["Initial Orbit"].Points.AddXY(r1 * c, r1 * s);
                double c2 = Math.Cos(true_anomaly - best_initial_prograde.relativeRotation);
                double s2 = Math.Sin(true_anomaly - best_initial_prograde.relativeRotation);
                double r2 = 2.0 * best_initial_prograde.apoapsis * best_initial_prograde.periapsis / (best_initial_prograde.apoapsis * (1.0 + c) + best_initial_prograde.periapsis * (1.0 - c));
                this.chart1.Series["After Prograde"].Points.AddXY(r2 * c2, r2 * s2);
                double c3 = Math.Cos(true_anomaly - best_inclination_scrub.relativeRotation);
                double s3 = Math.Sin(true_anomaly - best_inclination_scrub.relativeRotation);
                double r3 = 2.0 * best_inclination_scrub.apoapsis * best_inclination_scrub.periapsis / (best_inclination_scrub.apoapsis * (1.0 + c) + best_inclination_scrub.periapsis * (1.0 - c));
                this.chart1.Series["After Inclination Adjustment"].Points.AddXY(r3 * c3, r3 * s3);
                double c4 = Math.Cos(true_anomaly - best_inclination_prograde.relativeRotation);
                double s4 = Math.Sin(true_anomaly - best_inclination_prograde.relativeRotation);
                double r4 = 2.0 * best_inclination_prograde.apoapsis * best_inclination_prograde.periapsis / (best_inclination_prograde.apoapsis * (1.0 + c) + best_inclination_prograde.periapsis * (1.0 - c));
                this.chart1.Series["After Inclination Prograde"].Points.AddXY(r4 * c4, r4 * s4);
                double c5 = Math.Cos(true_anomaly - best_geostationary.relativeRotation);
                double s5 = Math.Sin(true_anomaly - best_geostationary.relativeRotation);
                double r5 = 2.0 * best_geostationary.apoapsis * best_geostationary.periapsis / (best_geostationary.apoapsis * (1.0 + c) + best_geostationary.periapsis * (1.0 - c));
                this.chart1.Series["Geostationary"].Points.AddXY(r5 * c5, r5 * s5);
                this.chart1.Series["Earth"].Points.AddXY(6378.1 * c, 6378.1 * s);
                this.chart1.Series["Earth"].Points.AddXY(0.0, 0.0);
            }
            double max = start_orbit.apoapsis;
            if (max < best_inclination_prograde.apoapsis)
                max = best_inclination_prograde.apoapsis;
            if (max < best_inclination_scrub.apoapsis)
                max = best_inclination_scrub.apoapsis;
            if (max < best_inclination_prograde.apoapsis)
                max = best_inclination_prograde.apoapsis;
            if (max < best_geostationary.apoapsis)
                max = best_geostationary.apoapsis;
            max = Math.Ceiling(max / 1000.0) * 1000.0;
            this.chart1.ChartAreas["ChartArea1"].AxisX.Maximum = max;
            this.chart1.ChartAreas["ChartArea1"].AxisY.Maximum = max;
            this.chart1.ChartAreas["ChartArea1"].AxisX.Minimum = -max;
            this.chart1.ChartAreas["ChartArea1"].AxisY.Minimum = -max;
            this.Refresh();

            /*MessageBox.Show((best_initial_prograde.argument_of_periapsis * 180.0 / Math.PI).ToString() + "\n"
                + (best_initial_prograde.relative_rotation * 180.0 / Math.PI).ToString() + "\n" +
                (best_inclination_scrub.true_anomaly_in * 180.0 / Math.PI));

            MessageBox.Show("Initial prograde of " + best_initial_prograde.last_delta_v.ToString() + " km/s at " + (
                best_initial_prograde.true_anomaly_in * 180.0 / Math.PI).ToString() + " degrees\n" +
                "Total delta v = " + best_delta_v.ToString() + "\n" +
                "Apoapsis = " + best_initial_prograde.apoapsis.ToString() + "     Periapsis = " +
                best_initial_prograde.periapsis.ToString() + "     Rotation = " + best_initial_prograde.relative_rotation.ToString());
                */
            //MessageBox.Show(best_inclination_prograde.relativeRotation.ToString());
            MessageBox.Show("Total delta v = " + (best_delta_v * 1000.0).ToString("f4") + " m/s");
        }

    }
}
