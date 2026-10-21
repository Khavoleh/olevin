import { Pie, useChartWidth } from "recharts";
import type { Zone } from "../../../api/home.schemas";
import ArcSector from "./ArcSector";
import ValueSector from "./ValueSector";

interface GaugeArcsProps {
	value: number;
	zone: Zone;
}

/** Width of the gauge in the mockup; every size below is in its units. */
const VIEW_WIDTH = 200;

const ZONE_FILL_CLASSES: Record<Zone, string> = {
	Stress: "fill-zone-stress-graphic",
	Unstable: "fill-zone-unstable-graphic",
	Calm: "fill-zone-calm-graphic",
};

const ZONE_ARCS = [
	{ size: 40, className: ZONE_FILL_CLASSES.Stress },
	{ size: 30, className: ZONE_FILL_CLASSES.Unstable },
	{ size: 30, className: ZONE_FILL_CLASSES.Calm },
];

const TRACK_ARC = [{ size: 1, className: "fill-meter-track" }];

/** Zone ring, track and value of the gauge, sized to the width of the chart. */
const GaugeArcs = ({ value, zone }: Readonly<GaugeArcsProps>) => {
	const scale = (useChartWidth() ?? 0) / VIEW_WIDTH;
	const arc = {
		dataKey: "size",
		cx: 100 * scale,
		cy: 104 * scale,
		startAngle: 180,
		stroke: "none",
		isAnimationActive: false,
	};
	const track = {
		...arc,
		innerRadius: 63 * scale,
		outerRadius: 77 * scale,
		cornerRadius: 7 * scale,
	};

	return (
		<>
			<Pie
				{...arc}
				data={ZONE_ARCS}
				endAngle={0}
				innerRadius={84 * scale}
				outerRadius={88 * scale}
				cornerRadius={2 * scale}
				paddingAngle={1.5}
				shape={ArcSector}
			/>
			<Pie {...track} data={TRACK_ARC} endAngle={0} shape={ArcSector} />
			<Pie
				{...track}
				data={[{ size: 1, className: ZONE_FILL_CLASSES[zone] }]}
				endAngle={180 - 1.8 * value}
				shape={ValueSector}
			/>
		</>
	);
};

export default GaugeArcs;
