import type { PieSectorShapeProps } from "recharts";
import ArcSector from "./ArcSector";

const TRACK_WIDTH = 14;
const KNOB_RADIUS = 9;
const KNOB_STROKE = 2;

/** Arc of the index value with the knob at its end. */
const ValueSector = (props: PieSectorShapeProps) => {
	const { cx, cy, innerRadius, outerRadius, endAngle } = props;
	const scale = (outerRadius - innerRadius) / TRACK_WIDTH;
	const radius = (innerRadius + outerRadius) / 2;
	const angle = (endAngle * Math.PI) / 180;

	return (
		<g>
			<ArcSector {...props} />
			<circle
				cx={cx + radius * Math.cos(angle)}
				cy={cy - radius * Math.sin(angle)}
				r={KNOB_RADIUS * scale}
				strokeWidth={KNOB_STROKE * scale}
				className="fill-surface stroke-fg"
			/>
		</g>
	);
};

export default ValueSector;
