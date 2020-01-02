using System;

namespace Geostationary
{
    /// <summary>
    /// Class for the orbit of a spacecraft.
    /// </summary>
    public class Orbit
    {
        /// <summary>
        /// The closest distance to the center of mass in this Orbit.
        /// </summary>
        public double periapsis;
        /// <summary>
        /// The furthest distance to the center of mass in this Orbit.
        /// </summary>
        public double apoapsis;
        /// <summary>
        /// (radians) The inclination angle above Earth's orbital plane.
        /// </summary>
        public double inclination;
        /// <summary>
        /// (radians) The angle between the ascending node and periapsis.
        /// </summary>
        private double argumentOfPeriapsis;
        /// <summary>
        /// (radians) The starting position in this Orbit.
        /// </summary>
        public double trueAnomalyIn;
        /// <summary>
        /// (radians) The ending position in this Orbit.
        /// </summary>
        public double trueAnomalyOut;
        /// <summary>
        /// (m/s) Change in velocity from the most recent maneuver.
        /// </summary>
        public double lastDeltaV;
        /// <summary>
        /// The standard gravitational parameter of Earth.
        /// </summary>
        public static double mu = 398600.4418;

        public double relativeRotation;

        /// <summary>
        /// An Orbit initialized based on the given parameters.
        /// </summary>
        /// <param name="apoapsis">The furthest distance from the center of mass in this Orbit.</param>
        /// <param name="periapsis">The closest distance to the center of mass in this Orbit.</param>
        /// <param name="inclination">Inclination angle above Earth's orbital plane.</param>
        /// <param name="argumentOfPeriapsis">Angle between the ascending node and periapsis.</param>
        public Orbit(double apoapsis, double periapsis, double inclination, double argumentOfPeriapsis)
        {
            this.apoapsis = apoapsis;
            this.periapsis = periapsis;
            this.inclination = inclination;
            this.argumentOfPeriapsis = argumentOfPeriapsis;
            this.relativeRotation = 0.0;
        }

        /// <summary>
        /// A new uninitialized Orbit.
        /// </summary>
        public Orbit() { }

        /// <summary>
        /// Perform a maneuever in prograde at the given position in the current orbit.
        /// </summary>
        /// <param name="trueAnomaly">(radians) The position in this Orbit (relative to periapsis).</param>
        /// <param name="thrust">(m/s) The change in velocity from this maneuver.</param>
        /// <returns>The new Orbit after completing the prograde maneuver.</returns>
        public Orbit Prograde(double trueAnomaly, double thrust)
        {
            Orbit orbit_out = new Orbit
            {
                inclination = this.inclination,
                trueAnomalyIn = trueAnomaly,
                lastDeltaV = thrust
            };
            double c = Math.Cos(trueAnomaly);
            double altitude = 2.0 * this.apoapsis * this.periapsis / (this.apoapsis * (1.0 + c) + this.periapsis * (1.0 - c));
            double in_speed = Math.Sqrt(2.0 * mu * (1.0 / altitude - 1.0 / (this.apoapsis + this.periapsis)));
            double out_speed = in_speed + thrust;
            orbit_out.apoapsis = (mu * altitude) * (1.0 + Math.Sqrt(1.0 - 4.0 * (
                2.0 * mu - altitude * out_speed * out_speed) * this.apoapsis * (
                1.0 - this.apoapsis / (this.apoapsis + this.periapsis)) * (
                out_speed / in_speed) * (out_speed / in_speed) / (
                2.0 * mu * altitude))) / (2.0 * mu - altitude * out_speed * out_speed);
            orbit_out.periapsis = 2.0 * mu * altitude / (2.0 * mu - altitude * out_speed * out_speed) - orbit_out.apoapsis;
            orbit_out.trueAnomalyOut = Math.Acos((orbit_out.apoapsis * orbit_out.periapsis * (
                this.apoapsis * (1.0 + c) + this.periapsis * (1.0 - c)) / (
                this.apoapsis * this.periapsis) - orbit_out.apoapsis - orbit_out.periapsis) /
                (orbit_out.apoapsis - orbit_out.periapsis));
            if ((orbit_out.apoapsis * orbit_out.periapsis * (
                this.apoapsis * (1.0 + c) + this.periapsis * (1.0 - c)) / (
                this.apoapsis * this.periapsis) - orbit_out.apoapsis - orbit_out.periapsis) /
                (orbit_out.apoapsis - orbit_out.periapsis) > 1.0)
                orbit_out.trueAnomalyOut = 0.0;
            if ((orbit_out.apoapsis * orbit_out.periapsis * (
                this.apoapsis * (1.0 + c) + this.periapsis * (1.0 - c)) / (
                this.apoapsis * this.periapsis) - orbit_out.apoapsis - orbit_out.periapsis) /
                (orbit_out.apoapsis - orbit_out.periapsis) < -1.0)
                orbit_out.trueAnomalyOut = Math.PI;
            orbit_out.argumentOfPeriapsis = this.argumentOfPeriapsis + orbit_out.trueAnomalyOut - orbit_out.trueAnomalyIn;
            if (orbit_out.argumentOfPeriapsis < 0.0)
                orbit_out.argumentOfPeriapsis += 2.0 * Math.PI;
            if (orbit_out.periapsis > this.apoapsis)
            {
                double tmp = orbit_out.periapsis;
                orbit_out.periapsis = orbit_out.apoapsis;
                orbit_out.apoapsis = tmp;
            }
            orbit_out.relativeRotation = this.relativeRotation + orbit_out.trueAnomalyOut - orbit_out.trueAnomalyIn;
            return orbit_out;
        }

        /// <summary>
        /// Bring the orbital inclination to zero.
        /// </summary>
        /// <returns>The orbit after scrubbing the inclination angle.</returns>
        public Orbit ScrubInclination()
        {
            Orbit orbit_out = new Orbit();
            orbit_out.inclination = this.inclination;
            orbit_out.trueAnomalyIn = this.argumentOfPeriapsis;
            if (orbit_out.trueAnomalyIn < 0.0)
                orbit_out.trueAnomalyIn += 2.0 * Math.PI;
            if (orbit_out.trueAnomalyIn < Math.PI / 2.0)
                orbit_out.trueAnomalyIn = Math.PI + orbit_out.trueAnomalyIn;
            double c = Math.Cos(orbit_out.trueAnomalyIn);
            double altitude = 2.0 * this.apoapsis * this.periapsis / (this.apoapsis * (1.0 + c) + this.periapsis * (1.0 - c));
            double out_speed = Math.Sqrt(2.0 * mu * (1.0 / altitude - 1.0 / (this.apoapsis + this.periapsis))) * Math.Cos(this.inclination);
            orbit_out.lastDeltaV = Math.Sqrt(2.0 * mu * (1.0 / altitude - 1.0 / (
                this.apoapsis + this.periapsis))) * Math.Sin(this.inclination);
            orbit_out.apoapsis = (mu * altitude) * (1.0 + Math.Sqrt(1.0 - 4.0 * (
                2.0 * mu - altitude * out_speed * out_speed) * this.apoapsis * (
                1.0 - this.apoapsis / (this.apoapsis + this.periapsis)) * Math.Cos(
                this.inclination) * Math.Cos(this.inclination) / (
                2.0 * mu * altitude))) / (2.0 * mu - altitude * out_speed * out_speed);
            orbit_out.periapsis = 2.0 * mu * altitude / (2.0 * mu - altitude * out_speed * out_speed) - orbit_out.apoapsis;
            orbit_out.trueAnomalyOut = Math.Acos((orbit_out.apoapsis * orbit_out.periapsis * (
                this.apoapsis * (1.0 + c) + this.periapsis * (1.0 - c)) / (
                this.apoapsis * this.periapsis) - orbit_out.apoapsis - orbit_out.periapsis) / (
                orbit_out.apoapsis - orbit_out.periapsis));
            if ((orbit_out.apoapsis * orbit_out.periapsis * (
                this.apoapsis * (1.0 + c) + this.periapsis * (1.0 - c)) / (
                this.apoapsis * this.periapsis) - orbit_out.apoapsis - orbit_out.periapsis) / (
                orbit_out.apoapsis - orbit_out.periapsis) > 1.0)
                orbit_out.trueAnomalyOut = 0.0;
            orbit_out.trueAnomalyOut = 2.0 * Math.PI - orbit_out.trueAnomalyOut;
            orbit_out.argumentOfPeriapsis = this.argumentOfPeriapsis + orbit_out.trueAnomalyOut - orbit_out.trueAnomalyIn;
            if (orbit_out.argumentOfPeriapsis < 0.0)
                orbit_out.argumentOfPeriapsis += 2.0 * Math.PI;
            orbit_out.inclination = 0.0;
            orbit_out.relativeRotation = this.relativeRotation + orbit_out.trueAnomalyOut - orbit_out.trueAnomalyIn;
            return orbit_out;
        }

        /// <summary>
        /// Perform the appropriate maneuvers to enter a geosynchronous orbit (circular at 42164 km).
        /// </summary>
        /// <returns>The orbit after entering a geosynchronous orbit.</returns>
        public Orbit ToCircularGeosynchronous()
        {
            Orbit orbit_out = new Orbit();
            double geosynchronous = 42164.0;
            double tmp1 = Math.Sqrt(2.0 * mu) * (Math.Abs(Math.Sqrt(1.0 / this.apoapsis - 1.0 / (
                this.apoapsis + this.periapsis)) - Math.Sqrt(1.0 / this.apoapsis - 1.0 / (
                this.apoapsis + geosynchronous))) + Math.Abs(Math.Sqrt(1.0 / geosynchronous - 1.0 / (
                geosynchronous + this.apoapsis)) - Math.Sqrt(1.0 / (2.0 * geosynchronous))));
            double tmp2 = Math.Sqrt(2.0 * mu) * (Math.Abs(Math.Sqrt(1.0 / this.periapsis - 1.0 / (
                this.apoapsis + this.periapsis)) - Math.Sqrt(1.0 / this.periapsis - 1.0 / (
                this.periapsis + geosynchronous))) + Math.Abs(Math.Sqrt(1.0 / geosynchronous - 1.0 / (
                geosynchronous + this.periapsis)) - Math.Sqrt(1.0 / (2.0 * geosynchronous))));
            if (tmp2 < tmp1)
                orbit_out.lastDeltaV = tmp2;
            else
                orbit_out.lastDeltaV = tmp1;
            orbit_out.periapsis = geosynchronous;
            orbit_out.apoapsis = geosynchronous;
            orbit_out.trueAnomalyIn = 0.0;
            orbit_out.trueAnomalyOut = 0.0;
            orbit_out.inclination = this.inclination;
            orbit_out.argumentOfPeriapsis = this.argumentOfPeriapsis;
            orbit_out.relativeRotation = this.relativeRotation;
            return orbit_out;
        }

        /// <summary>
        /// Perform a maneuver in the radial direction.
        /// </summary>
        /// <param name="true_anomaly">(radians) The position in the orbit to perform this maneuver.</param>
        /// <param name="thrust">(m/s) The change in velocity for this maneuver.</param>
        /// <returns></returns>
        public Orbit RadialBurn(double true_anomaly, double thrust)
        {
            Orbit orbit_out = new Orbit();
            orbit_out.inclination = this.inclination;
            orbit_out.argumentOfPeriapsis = this.argumentOfPeriapsis;
            orbit_out.lastDeltaV = thrust;
            orbit_out.trueAnomalyIn = true_anomaly;
            double c = Math.Cos(orbit_out.trueAnomalyIn);
            double altitude = 2.0 * this.apoapsis * this.periapsis / (this.apoapsis * (
                1.0 + c) + this.periapsis * (1.0 - c));
            double in_speed = Math.Sqrt(2.0 * mu * (1.0 / altitude - 1.0 / (
                this.apoapsis + this.periapsis)));
            double altitude_per_angle = (this.apoapsis - this.periapsis) * Math.Sin(
                true_anomaly) / ((this.apoapsis * (1.0 + Math.Cos(
                true_anomaly)) + this.periapsis * (1.0 - Math.Cos(true_anomaly))) * (
                this.apoapsis * (1.0 + Math.Cos(
                true_anomaly)) + this.periapsis * (1.0 - Math.Cos(true_anomaly))));
            double gamma = -true_anomaly;
            if ((altitude_per_angle * Math.Cos(
                    true_anomaly) - altitude * Math.Sin(true_anomaly)) != 0.0)
                gamma = Math.PI / 2.0 - true_anomaly - Math.Atan((altitude_per_angle * Math.Sin(
                    true_anomaly) + altitude * Math.Cos(true_anomaly)) / (altitude_per_angle * Math.Cos(
                    true_anomaly) - altitude * Math.Sin(true_anomaly)));
            double speed_squared = in_speed * in_speed + 2.0 * in_speed * thrust * Math.Sin(
                gamma) + thrust * thrust;
            orbit_out.periapsis = mu * altitude * (1.0 - Math.Sqrt(1.0 - 4.0 * this.apoapsis * (
                1.0 - this.apoapsis / (this.apoapsis + this.periapsis)) * (
                2.0 * mu - altitude * speed_squared) / (2.0 * mu * altitude))) / (
                2.0 * mu - altitude * speed_squared);
            orbit_out.apoapsis = 2.0 * mu * altitude / (
                2.0 * mu - altitude * speed_squared) - orbit_out.periapsis;
            orbit_out.trueAnomalyOut = Math.Acos((2.0 * orbit_out.apoapsis * orbit_out.periapsis -
                altitude * (orbit_out.apoapsis + orbit_out.periapsis)) / (altitude * (
                orbit_out.apoapsis - orbit_out.periapsis)));
            orbit_out.argumentOfPeriapsis = this.argumentOfPeriapsis +
                orbit_out.trueAnomalyOut - true_anomaly;
            if (orbit_out.argumentOfPeriapsis < 0.0)
                orbit_out.argumentOfPeriapsis += 2.0 * Math.PI;
            return orbit_out;
        }
    }
}
