using System;

namespace Geostationary
{
    /// <summary>
    /// Class for the orbit of a spacecraft.
    /// </summary>
    public class Orbit
    {
        public double periapsis;
        public double apoapsis;
        public double inclination;
        public double argumentOfPeriapsis;
        public double trueAnomalyIn;
        public double trueAnomalyOut;
        public double lastDeltaV;
        public double relativeRotation;
        public static double mu = 398600.4418;

        public Orbit(double apoapsis, double periapsis, double inclination, double argumentOfPeriapsis)
        {
            this.apoapsis = apoapsis;
            this.periapsis = periapsis;
            this.inclination = inclination;
            this.argumentOfPeriapsis = argumentOfPeriapsis;
            this.relativeRotation = 0.0;
        }

        public Orbit() { }



        public Orbit Prograde(Orbit orbitIn, double trueAnomaly, double thrust)
        {
            Orbit orbit_out = new Orbit
            {
                inclination = orbitIn.inclination,
                trueAnomalyIn = trueAnomaly,
                lastDeltaV = thrust
            };
            double c = Math.Cos(trueAnomaly);
            double altitude = 2.0 * orbitIn.apoapsis * orbitIn.periapsis / (orbitIn.apoapsis * (1.0 + c) + orbitIn.periapsis * (1.0 - c));
            double in_speed = Math.Sqrt(2.0 * mu * (1.0 / altitude - 1.0 / (orbitIn.apoapsis + orbitIn.periapsis)));
            double out_speed = in_speed + thrust;
            orbit_out.apoapsis = (mu * altitude) * (1.0 + Math.Sqrt(1.0 - 4.0 * (
                2.0 * mu - altitude * out_speed * out_speed) * orbitIn.apoapsis * (
                1.0 - orbitIn.apoapsis / (orbitIn.apoapsis + orbitIn.periapsis)) * (
                out_speed / in_speed) * (out_speed / in_speed) / (
                2.0 * mu * altitude))) / (2.0 * mu - altitude * out_speed * out_speed);
            orbit_out.periapsis = 2.0 * mu * altitude / (2.0 * mu - altitude * out_speed * out_speed) - orbit_out.apoapsis;
            orbit_out.trueAnomalyOut = Math.Acos((orbit_out.apoapsis * orbit_out.periapsis * (
                orbitIn.apoapsis * (1.0 + c) + orbitIn.periapsis * (1.0 - c)) / (
                orbitIn.apoapsis * orbitIn.periapsis) - orbit_out.apoapsis - orbit_out.periapsis) /
                (orbit_out.apoapsis - orbit_out.periapsis));
            if ((orbit_out.apoapsis * orbit_out.periapsis * (
                orbitIn.apoapsis * (1.0 + c) + orbitIn.periapsis * (1.0 - c)) / (
                orbitIn.apoapsis * orbitIn.periapsis) - orbit_out.apoapsis - orbit_out.periapsis) /
                (orbit_out.apoapsis - orbit_out.periapsis) > 1.0)
                orbit_out.trueAnomalyOut = 0.0;
            if ((orbit_out.apoapsis * orbit_out.periapsis * (
                orbitIn.apoapsis * (1.0 + c) + orbitIn.periapsis * (1.0 - c)) / (
                orbitIn.apoapsis * orbitIn.periapsis) - orbit_out.apoapsis - orbit_out.periapsis) /
                (orbit_out.apoapsis - orbit_out.periapsis) < -1.0)
                orbit_out.trueAnomalyOut = Math.PI;
            orbit_out.argumentOfPeriapsis = orbitIn.argumentOfPeriapsis + orbit_out.trueAnomalyOut - orbit_out.trueAnomalyIn;
            if (orbit_out.argumentOfPeriapsis < 0.0)
                orbit_out.argumentOfPeriapsis += 2.0 * Math.PI;
            if (orbit_out.periapsis > orbitIn.apoapsis)
            {
                double tmp = orbit_out.periapsis;
                orbit_out.periapsis = orbit_out.apoapsis;
                orbit_out.apoapsis = tmp;
            }
            orbit_out.relativeRotation = orbitIn.relativeRotation + orbit_out.trueAnomalyOut - orbit_out.trueAnomalyIn;
            return orbit_out;
        }

        public Orbit ScrubInclination(Orbit orbit_in)
        {
            Orbit orbit_out = new Orbit();
            orbit_out.inclination = orbit_in.inclination;
            orbit_out.trueAnomalyIn = orbit_in.argumentOfPeriapsis;
            if (orbit_out.trueAnomalyIn < 0.0)
                orbit_out.trueAnomalyIn += 2.0 * Math.PI;
            if (orbit_out.trueAnomalyIn < Math.PI / 2.0)
                orbit_out.trueAnomalyIn = Math.PI + orbit_out.trueAnomalyIn;
            double c = Math.Cos(orbit_out.trueAnomalyIn);
            double altitude = 2.0 * orbit_in.apoapsis * orbit_in.periapsis / (orbit_in.apoapsis * (1.0 + c) + orbit_in.periapsis * (1.0 - c));
            double out_speed = Math.Sqrt(2.0 * mu * (1.0 / altitude - 1.0 / (orbit_in.apoapsis + orbit_in.periapsis))) * Math.Cos(orbit_in.inclination);
            orbit_out.lastDeltaV = Math.Sqrt(2.0 * mu * (1.0 / altitude - 1.0 / (
                orbit_in.apoapsis + orbit_in.periapsis))) * Math.Sin(orbit_in.inclination);
            orbit_out.apoapsis = (mu * altitude) * (1.0 + Math.Sqrt(1.0 - 4.0 * (
                2.0 * mu - altitude * out_speed * out_speed) * orbit_in.apoapsis * (
                1.0 - orbit_in.apoapsis / (orbit_in.apoapsis + orbit_in.periapsis)) * Math.Cos(
                orbit_in.inclination) * Math.Cos(orbit_in.inclination) / (
                2.0 * mu * altitude))) / (2.0 * mu - altitude * out_speed * out_speed);
            orbit_out.periapsis = 2.0 * mu * altitude / (2.0 * mu - altitude * out_speed * out_speed) - orbit_out.apoapsis;
            orbit_out.trueAnomalyOut = Math.Acos((orbit_out.apoapsis * orbit_out.periapsis * (
                orbit_in.apoapsis * (1.0 + c) + orbit_in.periapsis * (1.0 - c)) / (
                orbit_in.apoapsis * orbit_in.periapsis) - orbit_out.apoapsis - orbit_out.periapsis) / (
                orbit_out.apoapsis - orbit_out.periapsis));
            if ((orbit_out.apoapsis * orbit_out.periapsis * (
                orbit_in.apoapsis * (1.0 + c) + orbit_in.periapsis * (1.0 - c)) / (
                orbit_in.apoapsis * orbit_in.periapsis) - orbit_out.apoapsis - orbit_out.periapsis) / (
                orbit_out.apoapsis - orbit_out.periapsis) > 1.0)
                orbit_out.trueAnomalyOut = 0.0;
            orbit_out.trueAnomalyOut = 2.0 * Math.PI - orbit_out.trueAnomalyOut;
            orbit_out.argumentOfPeriapsis = orbit_in.argumentOfPeriapsis + orbit_out.trueAnomalyOut - orbit_out.trueAnomalyIn;
            if (orbit_out.argumentOfPeriapsis < 0.0)
                orbit_out.argumentOfPeriapsis += 2.0 * Math.PI;
            orbit_out.inclination = 0.0;
            orbit_out.relativeRotation = orbit_in.relativeRotation + orbit_out.trueAnomalyOut - orbit_out.trueAnomalyIn;
            return orbit_out;
        }

        public Orbit ToCircularGeosynchronous(Orbit orbit_in)
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
                orbit_out.lastDeltaV = tmp2;
            else
                orbit_out.lastDeltaV = tmp1;
            orbit_out.periapsis = geosynchronous;
            orbit_out.apoapsis = geosynchronous;
            orbit_out.trueAnomalyIn = 0.0;
            orbit_out.trueAnomalyOut = 0.0;
            orbit_out.inclination = orbit_in.inclination;
            orbit_out.argumentOfPeriapsis = orbit_in.argumentOfPeriapsis;
            orbit_out.relativeRotation = orbit_in.relativeRotation;
            return orbit_out;
        }

        public Orbit RadialBurn(Orbit orbit_in, double true_anomaly, double thrust)
        {
            Orbit orbit_out = new Orbit();
            orbit_out.inclination = orbit_in.inclination;
            orbit_out.argumentOfPeriapsis = orbit_in.argumentOfPeriapsis;
            orbit_out.lastDeltaV = thrust;
            orbit_out.trueAnomalyIn = true_anomaly;
            double c = Math.Cos(orbit_out.trueAnomalyIn);
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
            orbit_out.trueAnomalyOut = Math.Acos((2.0 * orbit_out.apoapsis * orbit_out.periapsis -
                altitude * (orbit_out.apoapsis + orbit_out.periapsis)) / (altitude * (
                orbit_out.apoapsis - orbit_out.periapsis)));
            orbit_out.argumentOfPeriapsis = orbit_in.argumentOfPeriapsis +
                orbit_out.trueAnomalyOut - true_anomaly;
            if (orbit_out.argumentOfPeriapsis < 0.0)
                orbit_out.argumentOfPeriapsis += 2.0 * Math.PI;
            return orbit_out;
        }
    }
}
