using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace geostationary
{
    public partial class Form1 : Form
    {
        public static double mu = 398600.4418;

        public Form1()
        {
            InitializeComponent();
            apoapsisInput.Text = "50000";
            periapsisInput.Text = "200";
            inclinationInput.Text = "30";
            AoPinput.Text = "90";
        }

        public class Orbit
        {
            public double periapsis;
            public double apoapsis;
            public double inclination;
            public double argument_of_periapsis;
            public double true_anomaly_in;
            public double true_anomaly_out;
            public double last_delta_v;
            public double relative_rotation;

            public void initialize(double initial_apoapsis, double initial_periapsis, double initial_inclination, double initial_AoP)
            {
                apoapsis = initial_apoapsis;
                periapsis = initial_periapsis;
                inclination = initial_inclination;
                argument_of_periapsis = initial_AoP;
                relative_rotation = 0.0;
            }
        }

        public Orbit prograde(Orbit orbit_in, double true_anomaly, double thrust)
        {
            Orbit orbit_out = new Orbit();
            orbit_out.inclination = orbit_in.inclination;
            orbit_out.true_anomaly_in = true_anomaly;
            orbit_out.last_delta_v = thrust;
            double c = Math.Cos(true_anomaly);
            double altitude = 2.0 * orbit_in.apoapsis * orbit_in.periapsis / (orbit_in.apoapsis * (1.0 + c) + orbit_in.periapsis * (1.0 - c));
            double in_speed = Math.Sqrt(2.0 * mu * (1.0 / altitude - 1.0 / (orbit_in.apoapsis + orbit_in.periapsis)));
            double out_speed = in_speed + thrust;
            orbit_out.apoapsis = (mu * altitude) * (1.0 + Math.Sqrt(1.0 - 4.0 * (
                2.0 * mu - altitude * out_speed * out_speed) * orbit_in.apoapsis * (
                1.0 - orbit_in.apoapsis / (orbit_in.apoapsis + orbit_in.periapsis)) * (
                out_speed / in_speed) * (out_speed / in_speed) / (
                2.0 * mu * altitude))) / (2.0 * mu - altitude * out_speed * out_speed);
            orbit_out.periapsis = 2.0 * mu * altitude / (2.0 * mu - altitude * out_speed * out_speed) - orbit_out.apoapsis;
            orbit_out.true_anomaly_out = Math.Acos((orbit_out.apoapsis * orbit_out.periapsis * (
                orbit_in.apoapsis * (1.0 + c) + orbit_in.periapsis * (1.0 - c)) / (
                orbit_in.apoapsis * orbit_in.periapsis) - orbit_out.apoapsis - orbit_out.periapsis) / 
                (orbit_out.apoapsis - orbit_out.periapsis));
            if ((orbit_out.apoapsis * orbit_out.periapsis * (
                orbit_in.apoapsis * (1.0 + c) + orbit_in.periapsis * (1.0 - c)) / (
                orbit_in.apoapsis * orbit_in.periapsis) - orbit_out.apoapsis - orbit_out.periapsis) /
                (orbit_out.apoapsis - orbit_out.periapsis) > 1.0)
                orbit_out.true_anomaly_out = 0.0;
            if ((orbit_out.apoapsis * orbit_out.periapsis * (
                orbit_in.apoapsis * (1.0 + c) + orbit_in.periapsis * (1.0 - c)) / (
                orbit_in.apoapsis * orbit_in.periapsis) - orbit_out.apoapsis - orbit_out.periapsis) /
                (orbit_out.apoapsis - orbit_out.periapsis) < -1.0)
                orbit_out.true_anomaly_out = Math.PI;
            orbit_out.argument_of_periapsis = orbit_in.argument_of_periapsis + orbit_out.true_anomaly_out - orbit_out.true_anomaly_in;
            if (orbit_out.argument_of_periapsis < 0.0)
                orbit_out.argument_of_periapsis += 2.0 * Math.PI;
            if (orbit_out.periapsis > orbit_in.apoapsis)
            {
                double tmp = orbit_out.periapsis;
                orbit_out.periapsis = orbit_out.apoapsis;
                orbit_out.apoapsis = tmp;
            }
            orbit_out.relative_rotation = orbit_in.relative_rotation + orbit_out.true_anomaly_out - orbit_out.true_anomaly_in;
            return orbit_out;
        }

        public Orbit scrub_inclination(Orbit orbit_in)
        {
            Orbit orbit_out = new Orbit();
            orbit_out.inclination = orbit_in.inclination;
            orbit_out.true_anomaly_in = orbit_in.argument_of_periapsis;
            if (orbit_out.true_anomaly_in < 0.0)
                orbit_out.true_anomaly_in += 2.0 * Math.PI;
            if (orbit_out.true_anomaly_in < Math.PI / 2.0)
                orbit_out.true_anomaly_in = Math.PI + orbit_out.true_anomaly_in;
            double c = Math.Cos(orbit_out.true_anomaly_in);
            double altitude = 2.0 * orbit_in.apoapsis * orbit_in.periapsis / (orbit_in.apoapsis * (1.0 + c) + orbit_in.periapsis * (1.0 - c));
            double out_speed = Math.Sqrt(2.0 * mu * (1.0 / altitude - 1.0 / (orbit_in.apoapsis + orbit_in.periapsis))) * Math.Cos(orbit_in.inclination);
            orbit_out.last_delta_v = Math.Sqrt(2.0 * mu * (1.0 / altitude - 1.0 / (
                orbit_in.apoapsis + orbit_in.periapsis))) * Math.Sin(orbit_in.inclination);
            orbit_out.apoapsis = (mu * altitude) * (1.0 + Math.Sqrt(1.0 - 4.0 * (
                2.0 * mu - altitude * out_speed * out_speed) * orbit_in.apoapsis * (
                1.0 - orbit_in.apoapsis / (orbit_in.apoapsis + orbit_in.periapsis)) * Math.Cos(
                orbit_in.inclination) * Math.Cos(orbit_in.inclination) / (
                2.0 * mu * altitude))) / (2.0 * mu - altitude * out_speed * out_speed);
            orbit_out.periapsis = 2.0 * mu * altitude / (2.0 * mu - altitude * out_speed * out_speed) - orbit_out.apoapsis;
            orbit_out.true_anomaly_out = Math.Acos((orbit_out.apoapsis * orbit_out.periapsis * (
                orbit_in.apoapsis * (1.0 + c) + orbit_in.periapsis * (1.0 - c)) / (
                orbit_in.apoapsis * orbit_in.periapsis) - orbit_out.apoapsis - orbit_out.periapsis) / (
                orbit_out.apoapsis - orbit_out.periapsis));
            if ((orbit_out.apoapsis * orbit_out.periapsis * (
                orbit_in.apoapsis * (1.0 + c) + orbit_in.periapsis * (1.0 - c)) / (
                orbit_in.apoapsis * orbit_in.periapsis) - orbit_out.apoapsis - orbit_out.periapsis) / (
                orbit_out.apoapsis - orbit_out.periapsis) > 1.0)
                orbit_out.true_anomaly_out = 0.0;
            orbit_out.true_anomaly_out = 2.0 * Math.PI - orbit_out.true_anomaly_out;
            orbit_out.argument_of_periapsis = orbit_in.argument_of_periapsis + orbit_out.true_anomaly_out - orbit_out.true_anomaly_in;
            if (orbit_out.argument_of_periapsis < 0.0)
                orbit_out.argument_of_periapsis += 2.0 * Math.PI;
            orbit_out.inclination = 0.0;
            orbit_out.relative_rotation = orbit_in.relative_rotation + orbit_out.true_anomaly_out - orbit_out.true_anomaly_in;
            return orbit_out;
        }

        public Orbit to_circular_geosynchronous(Orbit orbit_in)
        {
            Orbit orbit_out = new Orbit();
            double geosynchronous = 42164.0;
            double tmp1 = Math.Sqrt(2.0 * mu) * (Math.Abs(Math.Sqrt(1.0 / orbit_in.apoapsis - 1.0 / (
                orbit_in.apoapsis + orbit_in.periapsis)) - Math.Sqrt(1.0 / orbit_in.apoapsis - 1.0 / (
                orbit_in.apoapsis + geosynchronous))) + Math.Abs(Math.Sqrt(1.0 / geosynchronous - 1.0 / (
                geosynchronous + orbit_in.apoapsis)) - Math.Sqrt(1.0 / (2.0 * geosynchronous))));
            double tmp2 = Math.Sqrt(2.0 * mu) * (Math.Abs(Math.Sqrt(1.0 / orbit_in.periapsis - 1.0 / (
                orbit_in.apoapsis + orbit_in.periapsis)) - Math.Sqrt(1.0 / orbit_in.periapsis - 1.0 / (
                orbit_in.periapsis + geosynchronous))) + Math.Abs(Math.Sqrt(1.0 / geosynchronous - 1.0 / (
                geosynchronous + orbit_in.periapsis)) - Math.Sqrt(1.0 / (2.0 * geosynchronous))));
            if (tmp2 < tmp1)
                orbit_out.last_delta_v = tmp2;
            else
                orbit_out.last_delta_v = tmp1;
            orbit_out.periapsis = geosynchronous;
            orbit_out.apoapsis = geosynchronous;
            orbit_out.true_anomaly_in = 0.0;
            orbit_out.true_anomaly_out = 0.0;
            orbit_out.inclination = orbit_in.inclination;
            orbit_out.argument_of_periapsis = orbit_in.argument_of_periapsis;
            orbit_out.relative_rotation = orbit_in.relative_rotation;
            return orbit_out;
        }

        public Orbit radial_burn(Orbit orbit_in, double true_anomaly, double thrust)
        {
            Orbit orbit_out = new Orbit();
            orbit_out.inclination = orbit_in.inclination;
            orbit_out.argument_of_periapsis = orbit_in.argument_of_periapsis;
            orbit_out.last_delta_v = thrust;
            orbit_out.true_anomaly_in = true_anomaly;
            double c = Math.Cos(orbit_out.true_anomaly_in);
            double altitude = 2.0 * orbit_in.apoapsis * orbit_in.periapsis / (orbit_in.apoapsis * (
                1.0 + c) + orbit_in.periapsis * (1.0 - c));
            double in_speed = Math.Sqrt(2.0 * mu * (1.0 / altitude - 1.0 / (
                orbit_in.apoapsis + orbit_in.periapsis)));
            double altitude_per_angle = (orbit_in.apoapsis - orbit_in.periapsis) * Math.Sin(
                true_anomaly) / ((orbit_in.apoapsis * (1.0 + Math.Cos(
                true_anomaly)) + orbit_in.periapsis * (1.0 - Math.Cos(true_anomaly))) * (
                orbit_in.apoapsis * (1.0 + Math.Cos(
                true_anomaly)) + orbit_in.periapsis * (1.0 - Math.Cos(true_anomaly))));
            double gamma = -true_anomaly;
            if ((altitude_per_angle * Math.Cos(
                    true_anomaly) - altitude * Math.Sin(true_anomaly)) != 0.0)
                gamma = Math.PI / 2.0 - true_anomaly - Math.Atan((altitude_per_angle * Math.Sin(
                    true_anomaly) + altitude * Math.Cos(true_anomaly)) / (altitude_per_angle * Math.Cos(
                    true_anomaly) - altitude * Math.Sin(true_anomaly)));
            double speed_squared = in_speed * in_speed + 2.0 * in_speed * thrust * Math.Sin(
                gamma) + thrust * thrust;
            orbit_out.periapsis = mu * altitude * (1.0 - Math.Sqrt(1.0 - 4.0 * orbit_in.apoapsis * (
                1.0 - orbit_in.apoapsis / (orbit_in.apoapsis + orbit_in.periapsis)) * (
                2.0 * mu - altitude * speed_squared) / (2.0 * mu * altitude))) / (
                2.0 * mu - altitude * speed_squared);
            orbit_out.apoapsis = 2.0 * mu * altitude / (
                2.0 * mu - altitude * speed_squared) - orbit_out.periapsis;
            orbit_out.true_anomaly_out = Math.Acos((2.0 * orbit_out.apoapsis * orbit_out.periapsis - 
                altitude * (orbit_out.apoapsis + orbit_out.periapsis)) / (altitude * (
                orbit_out.apoapsis - orbit_out.periapsis)));
            orbit_out.argument_of_periapsis = orbit_in.argument_of_periapsis +
                orbit_out.true_anomaly_out - true_anomaly;
            if (orbit_out.argument_of_periapsis < 0.0)
                orbit_out.argument_of_periapsis += 2.0 * Math.PI;
            return orbit_out;
        }

        private void to_geostationary_Click(object sender, EventArgs e)
        {
            double increment_angle = Math.PI / 16.0;
            double increment_thrust = 1.0 / 64.0;
            double start_apoapsis = Convert.ToDouble(apoapsisInput.Text) + 6378.1;
            double start_periapsis = Convert.ToDouble(periapsisInput.Text) + 6378.1;
            double start_inclination = Convert.ToDouble(inclinationInput.Text) * Math.PI / 180.0;
            double start_AoP = Convert.ToDouble(AoPinput.Text) * Math.PI / 180.0;
            Orbit start_orbit = new Orbit();
            start_orbit.initialize(start_apoapsis, start_periapsis, start_inclination, start_AoP);
            double best_delta_v = 32768.0;
            Orbit initial_prograde = new Orbit();
            Orbit inclination_scrub = new Orbit();
            Orbit inclination_prograde = new Orbit();
            Orbit geostationary = new Orbit();
            Orbit best_initial_prograde = new Orbit();
            Orbit best_inclination_scrub = new Orbit();
            Orbit best_inclination_prograde = new Orbit();
            Orbit best_geostationary = new Orbit();

            for (double start_true_anomaly = 0.0; start_true_anomaly <= Math.PI; start_true_anomaly += increment_angle)
            {
                double start_altitude = 2.0 * start_orbit.apoapsis * start_orbit.periapsis / (start_orbit.apoapsis * (
                    1.0 + Math.Cos(start_true_anomaly)) + start_orbit.periapsis * (1.0 - Math.Cos(start_true_anomaly)));
                double start_speed = Math.Sqrt(2.0 * mu * (1.0 / start_altitude - 1.0 / (start_orbit.periapsis + start_orbit.apoapsis)));
                double max_initial_thrust = Math.Sqrt(2.0 * mu / start_altitude) - start_speed; // escape velocity
                for (double initial_thrust = 0.0; initial_thrust < max_initial_thrust; initial_thrust += increment_thrust)
                {
                    initial_prograde = prograde(start_orbit, start_true_anomaly, initial_thrust);
                    inclination_scrub = scrub_inclination(initial_prograde);
                    double altitude_of_scrub = 2.0 * inclination_scrub.apoapsis * inclination_scrub.periapsis / (
                        inclination_scrub.apoapsis * (1.0 + Math.Cos(inclination_scrub.true_anomaly_out)) + inclination_scrub.periapsis * (
                        1.0 - Math.Cos(inclination_scrub.true_anomaly_out)));
                    double v = Math.Sqrt(2.0 * mu * (1.0 / altitude_of_scrub - 1.0 / (inclination_scrub.periapsis + inclination_scrub.apoapsis)));
                    for (double prograde_amount = increment_thrust - v; prograde_amount < Math.Sqrt(2.0 * mu / altitude_of_scrub) - v; prograde_amount += increment_thrust)
                    {
                        inclination_prograde = prograde(inclination_scrub, inclination_scrub.true_anomaly_out, prograde_amount);
                        geostationary = to_circular_geosynchronous(inclination_prograde);
                        double delta_v = Math.Abs(initial_thrust) + Math.Sqrt(
                            inclination_scrub.last_delta_v * inclination_scrub.last_delta_v +
                            prograde_amount * prograde_amount) + Math.Abs(geostationary.last_delta_v);
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
            refine(start_orbit, best_initial_prograde, best_inclination_scrub, best_inclination_prograde, best_geostationary, increment_angle, increment_thrust);
        }

        public void refine(Orbit start_orbit, Orbit best_initial_prograde, Orbit best_inclination_scrub, Orbit best_inclination_prograde, Orbit best_geostationary, double previous_increment_angle, double previous_increment_thrust)
        {
            double best_delta_v = 32768.0;
            Orbit initial_prograde = new Orbit();
            Orbit inclination_scrub = new Orbit();
            Orbit inclination_prograde = new Orbit();
            Orbit geostationary = new Orbit();
            double increment_angle = previous_increment_angle / 64.0;
            double increment_thrust = previous_increment_thrust / 64.0;
            for (double start_true_anomaly = best_initial_prograde.true_anomaly_in - previous_increment_angle; start_true_anomaly <= best_initial_prograde.true_anomaly_in + previous_increment_angle; start_true_anomaly += increment_angle)
            {
                if (start_true_anomaly > Math.PI)
                    break;
                double start_altitude = 2.0 * start_orbit.apoapsis * start_orbit.periapsis / (start_orbit.apoapsis * (
                    1.0 + Math.Cos(start_true_anomaly)) + start_orbit.periapsis * (1.0 - Math.Cos(start_true_anomaly)));
                double start_speed = Math.Sqrt(2.0 * mu * (1.0 / start_altitude - 1.0 / (start_orbit.periapsis + start_orbit.apoapsis)));
                double max_initial_thrust = best_initial_prograde.last_delta_v + previous_increment_thrust;
                if (max_initial_thrust > Math.Sqrt(2.0 * mu / start_altitude) - start_speed)
                    max_initial_thrust = Math.Sqrt(2.0 * mu / start_altitude) - start_speed; // escape velocity
                for (double initial_thrust = best_initial_prograde.last_delta_v - previous_increment_thrust; initial_thrust < max_initial_thrust; initial_thrust += increment_thrust)
                {
                    initial_prograde = prograde(start_orbit, start_true_anomaly, initial_thrust);
                    inclination_scrub = scrub_inclination(initial_prograde);
                    double altitude_of_scrub = 2.0 * inclination_scrub.apoapsis * inclination_scrub.periapsis / (
                        inclination_scrub.apoapsis * (1.0 + Math.Cos(inclination_scrub.true_anomaly_out)) + inclination_scrub.periapsis * (
                        1.0 - Math.Cos(inclination_scrub.true_anomaly_out)));
                    double v = Math.Sqrt(2.0 * mu * (1.0 / altitude_of_scrub - 1.0 / (inclination_scrub.periapsis + inclination_scrub.apoapsis)));
                    for (double prograde_amount = best_inclination_prograde.last_delta_v - previous_increment_thrust; prograde_amount < best_inclination_prograde.last_delta_v + previous_increment_thrust; prograde_amount += increment_thrust)
                    {
                        inclination_prograde = prograde(inclination_scrub, inclination_scrub.true_anomaly_out, prograde_amount);
                        inclination_prograde.true_anomaly_out = 2.0 * Math.PI - inclination_prograde.true_anomaly_out;
                        inclination_prograde.relative_rotation = inclination_scrub.relative_rotation + inclination_prograde.true_anomaly_out - inclination_prograde.true_anomaly_in;
                        geostationary = to_circular_geosynchronous(inclination_prograde);
                        double delta_v = Math.Abs(initial_thrust) + Math.Sqrt(
                            inclination_scrub.last_delta_v * inclination_scrub.last_delta_v + 
                            prograde_amount * prograde_amount) +
                            Math.Abs(geostationary.last_delta_v);
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
                double c2 = Math.Cos(true_anomaly - best_initial_prograde.relative_rotation);
                double s2 = Math.Sin(true_anomaly - best_initial_prograde.relative_rotation);
                double r2 = 2.0 * best_initial_prograde.apoapsis * best_initial_prograde.periapsis / (best_initial_prograde.apoapsis * (1.0 + c) + best_initial_prograde.periapsis * (1.0 - c));
                this.chart1.Series["After Prograde"].Points.AddXY(r2 * c2, r2 * s2);
                double c3 = Math.Cos(true_anomaly - best_inclination_scrub.relative_rotation);
                double s3 = Math.Sin(true_anomaly - best_inclination_scrub.relative_rotation);
                double r3 = 2.0 * best_inclination_scrub.apoapsis * best_inclination_scrub.periapsis / (best_inclination_scrub.apoapsis * (1.0 + c) + best_inclination_scrub.periapsis * (1.0 - c));
                this.chart1.Series["After Inclination Adjustment"].Points.AddXY(r3 * c3, r3 * s3);
                double c4 = Math.Cos(true_anomaly - best_inclination_prograde.relative_rotation);
                double s4 = Math.Sin(true_anomaly - best_inclination_prograde.relative_rotation);
                double r4 = 2.0 * best_inclination_prograde.apoapsis * best_inclination_prograde.periapsis / (best_inclination_prograde.apoapsis * (1.0 + c) + best_inclination_prograde.periapsis * (1.0 - c));
                this.chart1.Series["After Inclination Prograde"].Points.AddXY(r4 * c4, r4 * s4);
                double c5 = Math.Cos(true_anomaly - best_geostationary.relative_rotation);
                double s5 = Math.Sin(true_anomaly - best_geostationary.relative_rotation);
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
            MessageBox.Show(best_inclination_prograde.relative_rotation.ToString());
            MessageBox.Show("Total delta v = " + (best_delta_v * 1000.0).ToString("f4") + " m/s");
        }


    }
}
