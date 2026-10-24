interface MonthTickProps {
	x: number | string;
	y: number | string;
	month: string;
	year?: number;
}

/** Month on the axis with its year under it. */
const MonthTick = ({ x, y, month, year }: Readonly<MonthTickProps>) => (
	<text
		x={x}
		y={y}
		textAnchor="middle"
		fill="var(--color-chart-axis)"
		fontSize={12}
		fontWeight={500}
	>
		<tspan x={x} dy="0.71em">
			{month}
		</tspan>
		<tspan x={x} dy="1.25em">
			{year}
		</tspan>
	</text>
);

export default MonthTick;
