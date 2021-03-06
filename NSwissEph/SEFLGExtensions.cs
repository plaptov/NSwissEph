namespace NSwissEph
{
	public static class SEFLGExtensions
	{
		/// <see cref="plaus_iflag" />
		public static SEFLG PlausIflag(this SEFLG iflag, SweData swed, PlanetNumber ipl)
		{
			var epheflag = SEFLG.None;

			// either Horizons mode or simplified Horizons mode, not both
			if (iflag.HasFlag(SEFLG.JPLHOR))
				iflag &= ~SEFLG.JPLHOR_APPROX;
			// if topocentric bit, turn helio- and barycentric bits off;
			if (iflag.HasFlag(SEFLG.TOPOCTR))
				iflag &= ~(SEFLG.HELCTR | SEFLG.BARYCTR);
			/* if barycentric bit, turn heliocentric bit off */
			if (iflag.HasFlag(SEFLG.BARYCTR))
				iflag = iflag & ~SEFLG.HELCTR;
			/* if heliocentric bit, turn aberration and deflection off */
			if (iflag.HasFlag(SEFLG.HELCTR))
				iflag |= SEFLG.NOABERR | SEFLG.NOGDEFL; /*iflag |= SEFLG.TRUEPOS;*/
			/* same, if barycentric bit */
			if (iflag.HasFlag(SEFLG.BARYCTR))
				iflag |= SEFLG.NOABERR | SEFLG.NOGDEFL; /*iflag |= SEFLG.TRUEPOS;*/
			/* if no_precession bit is set, set also no_nutation bit */
			if (iflag.HasFlag(SEFLG.J2000))
				iflag |= SEFLG.NONUT;
			/* if sidereal bit is set, set also no_nutation bit *
			 * also turn JPL Horizons mode off */
			if (iflag.HasFlag(SEFLG.SIDEREAL))
			{
				iflag |= SEFLG.NONUT;
				iflag = iflag & ~(SEFLG.JPLHOR | SEFLG.JPLHOR_APPROX);
			}
			/* if truepos is set, turn off grav. defl. and aberration */
			if (iflag.HasFlag(SEFLG.TRUEPOS))
				iflag |= SEFLG.NOGDEFL | SEFLG.NOABERR;
			if (iflag.HasFlag(SEFLG.MOSEPH))
				epheflag = SEFLG.MOSEPH;
			if (iflag.HasFlag(SEFLG.SWIEPH))
				epheflag = SEFLG.SWIEPH;
			if (iflag.HasFlag(SEFLG.JPLEPH))
				epheflag = SEFLG.JPLEPH;
			if (epheflag == SEFLG.None)
				epheflag = SEFLG.DEFAULTEPH;
			iflag = (iflag & ~SEFLG.EPHMASK) | epheflag;
			/* SEFLG.JPLHOR only with JPL and Swiss Ephemeeris */
			if (!epheflag.HasFlag(SEFLG.JPLEPH))
				iflag &= ~(SEFLG.JPLHOR | SEFLG.JPLHOR_APPROX);
			/* planets that have no JPL Horizons mode */
			if (ipl == PlanetNumber.OsculatingApogee || ipl == PlanetNumber.TrueNode
				|| ipl == PlanetNumber.MeanApogee || ipl == PlanetNumber.MeanNode
				|| ipl == PlanetNumber.InterpolatedApogee || ipl == PlanetNumber.InterpolatedPerigee)
				iflag &= ~(SEFLG.JPLHOR | SEFLG.JPLHOR_APPROX);
			if (ipl >= PlanetNumber.FictionalOffset && ipl <= PlanetNumber.MaxFinctionalPlanetNumber)
				iflag &= ~(SEFLG.JPLHOR | SEFLG.JPLHOR_APPROX);

			if (iflag.HasFlag(SEFLG.JPLHOR))
			{
				if (false) // swed.eop_dpsi_loaded <= 0
				{
					iflag &= ~SEFLG.JPLHOR;
					iflag |= SEFLG.JPLHOR_APPROX;
				}
			}

			if (iflag.HasFlag(SEFLG.JPLHOR))
				iflag |= SEFLG.ICRS;
			if (iflag.HasFlag(SEFLG.JPLHOR_APPROX) && swed.JplHorizonsMode == JplHorizonsMode.Two)
				iflag |= SEFLG.ICRS;

			return iflag;
		}
	}
}