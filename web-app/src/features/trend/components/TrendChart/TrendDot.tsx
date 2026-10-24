interface TrendDotProps {
	cx?: number;
	cy?: number;
	isLast: boolean;
}

/** Dot of a month with a snapshot; the latest one is larger. */
const TrendDot = ({ cx, cy, isLast }: Readonly<TrendDotProps>) =>
	cx === undefined || cy === undefined ? (
		<g />
	) : (
		<circle
			cx={cx}
			cy={cy}
			r={isLast ? 6 : 4.5}
			fill="var(--color-chart-line)"
			stroke="var(--color-surface)"
			strokeWidth={2}
		/>
	);

export default TrendDot;
