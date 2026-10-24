"use client";

import { Line } from "recharts";
import type { ChartPoint } from "../../chart-point";
import TrendDot from "./TrendDot";
import ValueLabel from "./ValueLabel";

interface TrendLineProps {
	points: ChartPoint[];
}

/** Line of the trend with a dot and a value on its points; months without a snapshot break it. */
const TrendLine = ({ points }: Readonly<TrendLineProps>) => {
	const lastIndex = points.findLastIndex((point) => point.value !== null);

	return (
		<Line
			yAxisId="value"
			dataKey="value"
			type="linear"
			stroke="var(--color-chart-line)"
			strokeWidth={3}
			strokeLinejoin="round"
			strokeLinecap="round"
			isAnimationActive={false}
			activeDot={false}
			dot={(props) => (
				<TrendDot
					key={props.index}
					cx={props.cx}
					cy={props.cy}
					isLast={props.index === lastIndex}
				/>
			)}
			label={(props) => (
				<ValueLabel
					key={props.index}
					viewBox={props.viewBox}
					text={points[props.index ?? -1]?.text}
					isLast={props.index === lastIndex}
				/>
			)}
		/>
	);
};

export default TrendLine;
