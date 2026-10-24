"use client";

import {
	CartesianGrid,
	LineChart,
	ReferenceArea,
	XAxis,
	YAxis,
} from "recharts";
import type { ChartPoint } from "../../chart-point";
import { MONTH_AXIS_HEIGHT, PLOT_TOP } from "../../constants/chart-layout";
import MonthTick from "./MonthTick";
import ScaleLabels from "./ScaleLabels";
import TrendLine from "./TrendLine";

interface TrendChartProps {
	title: string;
	points: ChartPoint[];
	ticks: number[];
	terms: { value: number; label: string }[];
	hasZones: boolean;
}

const DOMAIN = [0, 100];

const ZONE_BANDS = [
	{ from: 0, to: 40, fill: "var(--color-zone-stress-soft)" },
	{ from: 40, to: 70, fill: "var(--color-zone-unstable-soft)" },
	{ from: 70, to: 100, fill: "var(--color-zone-calm-soft)" },
];

/** Layer of the zone bands, under the grid of Recharts (-100). */
const BANDS_Z_INDEX = -200;

/** Narrowest width of one month; with more months than fit, the plot scrolls. */
const MONTH_WIDTH = 44;

/** Line of one trend value on the 0–100 scale, with the index zones behind it when asked; the plot scrolls between its fixed scales. */
const TrendChart = ({
	title,
	points,
	ticks,
	terms,
	hasZones,
}: Readonly<TrendChartProps>) => (
	<div
		role="img"
		aria-label={`${title}: ${points
			.filter((point) => point.value !== null)
			.map((point) => `${point.month} ${point.year} ${point.text}`)
			.join(", ")}`}
		className="flex min-w-0 items-start"
	>
		<ScaleLabels
			labels={ticks.map((tick) => ({ value: tick, label: String(tick) }))}
			className="w-8 text-xs tabular-nums"
			itemClassName="right-2"
		/>

		<div
			ref={(node) => {
				if (node) node.scrollLeft = node.scrollWidth;
			}}
			className="min-w-0 flex-1 overflow-x-auto"
		>
			<div
				className="h-64 md:h-76"
				style={{ minWidth: points.length * MONTH_WIDTH }}
			>
				<LineChart
					responsive
					accessibilityLayer={false}
					data={points}
					margin={{ top: PLOT_TOP, right: 0, bottom: 0, left: 0 }}
					style={{ width: "100%", height: "100%" }}
				>
					{hasZones
						? ZONE_BANDS.map((band) => (
								<ReferenceArea
									key={band.from}
									yAxisId="value"
									y1={band.from}
									y2={band.to}
									fill={band.fill}
									fillOpacity={1}
									zIndex={BANDS_Z_INDEX}
								/>
							))
						: null}
					<CartesianGrid
						yAxisId="value"
						vertical={false}
						stroke="var(--color-chart-grid)"
					/>
					<XAxis
						dataKey="month"
						axisLine={false}
						tickLine={false}
						interval={0}
						height={MONTH_AXIS_HEIGHT}
						tick={(props) => (
							<MonthTick
								key={props.index}
								x={props.x}
								y={props.y}
								month={props.payload.value}
								year={points[props.index]?.year}
							/>
						)}
						padding={{ left: 20, right: 20 }}
					/>
					<YAxis
						yAxisId="value"
						type="number"
						domain={DOMAIN}
						ticks={ticks}
						hide
					/>
					<TrendLine points={points} />
				</LineChart>
			</div>
		</div>

		<ScaleLabels
			labels={terms}
			className="hidden w-22 text-sm md:block"
			itemClassName="left-2.5"
		/>
	</div>
);

export default TrendChart;
